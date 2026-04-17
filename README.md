# WebBookRP

API REST em **ASP.NET Core (.NET 10)** com **Dapper**, **MySQL** (via [MySqlConnector](https://mysqlconnector.net/)) e autenticação **JWT**. O backend substitui o mock do front-end anterior, expondo DTOs em todas as respostas, consultas parametrizadas e datas persistidas em **UTC**.

## Conteúdo do repositório

| Caminho | Descrição |
|---------|-----------|
| `src/WebBookRP_API/` | Projeto da API Web |
| `src/WebBookRP_API/doc/create.sql` | Script oficial de criação do banco (`WebBookRP_db`) e tabelas |
| `src/WebBookRP_API/doc/insert.sql` | Carga inicial (usuário, autor, livros, serviços, posts, configs) |
| `src/WebBookRP_API/doc/migrations/001_post_like_ips.sql` | Opcional: tabela para **toggle de likes por IP** (`POST /posts/{id}/likes/toggle`) |
| `tools/BcryptQuick/` | Utilitário opcional para gerar `PasswordHash` BCrypt ao atualizar usuários |
| `src/WebBookRP_API/WebBookRP_API.http` | Exemplos de requisições (REST Client / Visual Studio) |
| `doc/postman/` | **Collection Postman** + **environment** localhost + script `smoke-test.ps1` (ver `doc/postman/README.md`) |

## Arquitetura

Camadas simples, com injeção de dependência:

- **`Controllers`** — rotas HTTP, validação de modelo, `[Authorize]` no painel.
- **`Services`** — regras de negócio (catálogo público, leads públicos, placeholders, likes, JWT emitido após validar **Users** + BCrypt).
- **`Repositories`** — Dapper com parâmetros (`@Param`), nunca SQL concatenado.
- **`Models`** — entidades alinhadas ao esquema em `doc/create.sql` (PascalCase no MySQL: `Books`, `Users`, etc.).
- **`DTOs`** — contratos JSON (camelCase padrão do ASP.NET Core).
- **`Interfaces`** — contratos de repositórios e serviços.
- **`Configuration`** — `JwtSettings`.
- **`Infrastructure`** — placeholders de mídia.

Conexão **`IDbConnection`** scoped por requisição (`MySqlConnection`).

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **MySQL** acessível

## Banco de dados (script oficial)

1. Ajuste `ConnectionStrings:DefaultConnection` em `src/WebBookRP_API/appsettings.json` (banco `WebBookRP_db` ou o nome que usar no script).
2. Execute, **nesta ordem**:
   - `src/WebBookRP_API/doc/create.sql`
   - `src/WebBookRP_API/doc/insert.sql`
3. **Opcional:** para likes por IP funcionarem sem erro, rode também `src/WebBookRP_API/doc/migrations/001_post_like_ips.sql`.

Tabelas principais: `Users`, `Books`, `Services`, `Leads`, `Posts`, `Comments`, `AuthorProfile`, `AuthorTimeline`, `SystemSettings`, `SecurityLogs`.

## Como rodar

```bash
cd src/WebBookRP_API
dotnet restore
dotnet run
```

HTTP padrão: `http://localhost:5028` (`Properties/launchSettings.json`).

## Autenticação (tabela `Users` + BCrypt)

- Login busca o usuário por **e-mail** (`Users.Email`), compara a senha com **`PasswordHash`** usando **BCrypt** (`BCrypt.Net-Next`). Hashes que não começam com `$2` caem em verificação texto puro apenas como migração legada (evite em produção).
- Cada tentativa de login grava em **`SecurityLogs`** (`EmailAttempt`, `Success`, `IpAddress`, `UserAgent`, `CreatedAt`).
- JWT inclui `sub` / `nameidentifier` = **GUID do usuário**, `name`, `email`, `role` (coluna `Role`).

### JWT em `appsettings.json` (seção `Jwt`)

Defina `Issuer`, `Audience`, `SigningKey` (segredo longo), `ExpirationMinutes`.

### Credenciais após `doc/insert.sql`

O seed atual define:

- **E-mail:** `admin@gmail.com`
- **Senha:** `Admin@123`  
  (hash BCrypt cost 11 no `insert.sql`; comentário no arquivo indica como regenerar com `tools/BcryptQuick`.)

Corpo aceito no login:

- `email` **ou** `username` (ambos tratados como identificador de login = e-mail cadastrado).
- `password` (obrigatório).

### Exemplo: obter token

```http
POST http://localhost:5028/auth/login
Content-Type: application/json

{
  "email": "admin@gmail.com",
  "password": "Admin@123"
}
```

Resposta:

```json
{
  "token": "<jwt>",
  "tokenType": "Bearer",
  "expiresInMinutes": 480
}
```

Rotas protegidas:

```http
Authorization: Bearer <jwt>
```

### Sessão

- **`GET /auth/session`** — com JWT válido retorna `authenticated`, `userId`, `name`, `email`, `role` e `username` (alias do e-mail para compatibilidade).

## Endpoints da API

Legenda: **(P)** público · **(A)** JWT obrigatório.

### Autenticação

| Método | Rota | Auth |
|--------|------|------|
| POST | `/auth/login` | — |
| POST | `/auth/logout` | — |
| GET | `/auth/session` | — |

### Livros

| Método | Rota | Auth |
|--------|------|------|
| GET | `/books` | (A) |
| GET | `/books/catalog` | (P) ativos |
| GET | `/books/{id}` | (P) se ativo |
| POST | `/books` | (A) |
| PUT | `/books/{id}` | (A) |
| DELETE | `/books/{id}` | (A) |

O modelo SQL não possui colunas de links da API; o repositório devolve `synopsis`/links como nulos nas leituras. Campos extras da API continuam aceitos no POST/PUT onde mapeados no serviço (valores não persistidos se não existirem colunas).

### Serviços

| Método | Rota | Auth |
|--------|------|------|
| GET | `/services` | (P) ativos |
| GET | `/services/all` | (A) |
| GET | `/services/{id}` | (A) |
| POST | `/services` | (A) |
| PUT | `/services/{id}` | (A) |
| DELETE | `/services/{id}` | (A) |

### Leads

| Método | Rota | Auth |
|--------|------|------|
| POST | `/leads` | (P) |
| GET | `/leads` | (A) paginação `page`, `pageSize`, `status`, `search` |
| GET | `/leads/{id}` | (A) |
| PATCH | `/leads/{id}/status` | (A) |
| PATCH | `/leads/{id}/notes` | (A) |
| PUT | `/leads/{id}` | (A) |
| DELETE | `/leads/{id}` | (A) |

### Posts e comentários

| Método | Rota | Auth |
|--------|------|------|
| GET | `/posts` | (P/A) |
| GET | `/posts/{id}` | (P/A) |
| POST | `/posts` | (A) |
| PUT | `/posts/{id}` | (A) |
| DELETE | `/posts/{id}` | (A) |
| POST | `/posts/{id}/likes/toggle` | (P) requer migração `001_post_like_ips.sql` |
| POST | `/posts/{id}/comments` | (P) |
| PATCH | `/posts/{postId}/comments/{commentId}/author-like` | (A) |
| DELETE | `/posts/{postId}/comments/{commentId}` | (A) |

Leituras de `Posts` assumem `allowLikes` / `allowComments` / `externalLink` com valores padrão compatíveis com o front até que colunas existam no banco.

### Autor

| Método | Rota | Auth |
|--------|------|------|
| GET | `/author` | (P) |
| PUT | `/author` | (A) |

### Config e estatísticas

| Método | Rota | Auth |
|--------|------|------|
| GET/PUT | `/config/system` | (A) chave `SystemConfig` |
| GET/PUT | `/config/home` | (A) chave `HomeConfig` |
| GET | `/stats` | (A) chave `StatsConfig` (seed em `insert.sql`) |

`SystemSettings.ConfigValue` é tipo **JSON** no MySQL; o repositório usa `CAST` na leitura/escrita.

## Segurança

- SQL parametrizado; respostas em DTOs; UTC nos serviços onde aplicável.
- CORS aberto no código de desenvolvimento; restrinja em produção.

## Testes manuais

`src/WebBookRP_API/WebBookRP_API.http` — após login, copie o `token` para `@token`.

## Pacotes principais

- `BCrypt.Net-Next`, `Dapper`, `MySqlConnector`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.OpenApi`
