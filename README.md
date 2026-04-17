# WebBookRP

API REST em **ASP.NET Core (.NET 10)** com **Dapper**, **MySQL** (via [MySqlConnector](https://mysqlconnector.net/)) e autenticação **JWT**. O backend substitui o mock do front-end anterior, expondo DTOs em todas as respostas, consultas parametrizadas e datas persistidas em **UTC**.

## Conteúdo do repositório

| Caminho | Descrição |
|---------|-----------|
| `src/WebBookRP_API/` | Projeto da API Web |
| `src/WebBookRP_API/Database/schema.sql` | Script de criação das tabelas e dados iniciais mínimos (autor id=1, `system_settings`) |
| `src/WebBookRP_API/WebBookRP_API.http` | Exemplos de requisições para testar os endpoints no Visual Studio ou REST Client |

## Arquitetura

Camadas simples, com injeção de dependência:

- **`Controllers`** — rotas HTTP, validação de modelo, autorização `[Authorize]` onde o painel exige login.
- **`Services`** — regras de negócio (catálogo público só ativos, leads públicos com `value`/`status`/`notes` fixos, placeholders de capa/ícone, likes por IP, etc.).
- **`Repositories`** — acesso a dados com **Dapper** (`QueryAsync`, `ExecuteAsync`, `QueryFirstOrDefaultAsync`), sempre com parâmetros (`@Nome`), nunca SQL concatenado.
- **`Models`** — entidades alinhadas às tabelas MySQL.
- **`DTOs`** — contratos de entrada/saída da API (JSON camelCase por padrão do ASP.NET Core).
- **`Interfaces`** — contratos de repositórios e serviços.
- **`Configuration`** — opções do JWT (`JwtSettings`).
- **`Infrastructure`** — constantes compartilhadas (URLs de placeholder para livro/serviço).

A conexão MySQL é registrada como **`IDbConnection`** em escopo **Scoped** (uma conexão por requisição), implementada por `MySqlConnection`.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Servidor **MySQL** acessível a partir da máquina de desenvolvimento

## Banco de dados

1. Crie o banco (ou use um existente) e aponte a connection string em `src/WebBookRP_API/appsettings.json` (chave `ConnectionStrings:DefaultConnection`).
2. Execute o script:

`src/WebBookRP_API/Database/schema.sql`

Ele cria tabelas: `books`, `services`, `leads`, `posts`, `comments`, `post_like_ips` (toggle de like por IP), `author_profiles`, `author_timeline`, `system_settings`, `security_logs`.

**Integridade:** `leads.ServiceId` referencia `services.Id` (GUID). Livros e serviços usam **GUID** gerado na API; leads, posts e comentários usam **INT** auto-incremento.

## Como rodar o projeto

Na raiz da API:

```bash
cd src/WebBookRP_API
dotnet restore
dotnet run
```

Por padrão o perfil **http** usa `http://localhost:5028` (veja `Properties/launchSettings.json`). Em desenvolvimento o OpenAPI pode estar mapeado em `/openapi/v1.json` conforme configuração do template.

## Autenticação JWT

Configuração em `appsettings.json`, seção **`Jwt`**:

- `Issuer`, `Audience`, `SigningKey`, `ExpirationMinutes`

**Importante:** altere `SigningKey` para um segredo longo e aleatório em ambientes reais; a chave precisa ser compatível com HMAC (tamanho adequado).

### Credenciais iniciais (desenvolvimento)

- **Usuário:** `admin`
- **Senha:** `admin`

Toda tentativa de login é registrada em **`security_logs`** (`EmailAttempt`, `Success`, `IpAddress`, `UserAgent`, `CreatedAtUtc`).

### Exemplo: obter token

```http
POST http://localhost:5028/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin"
}
```

Resposta (campos principais):

```json
{
  "token": "<jwt>",
  "tokenType": "Bearer",
  "expiresInMinutes": 480
}
```

Nas rotas protegidas, envie:

```http
Authorization: Bearer <jwt>
```

### Logout e sessão

- **`POST /auth/logout`** — API stateless; o cliente apenas descarta o token.
- **`GET /auth/session`** — público; se enviar um JWT válido, reflete o usuário autenticado nos claims.

## Endpoints da API

Legenda: **(P)** público (ou comportamento público definido) · **(A)** requer JWT válido (`[Authorize]`).

### Autenticação

| Método | Rota | Auth |
|--------|------|------|
| POST | `/auth/login` | — |
| POST | `/auth/logout` | — |
| GET | `/auth/session` | — |

### Livros

| Método | Rota | Auth |
|--------|------|------|
| GET | `/books` | (A) lista completa (painel) |
| GET | `/books/catalog` | (P) apenas `active == true` |
| GET | `/books/{id}` | (P) detalhe se livro ativo |
| POST | `/books` | (A) |
| PUT | `/books/{id}` | (A) |
| DELETE | `/books/{id}` | (A) |

Regras: catálogo público só ativos; capa vazia recebe URL de placeholder ao criar/atualizar e na serialização pública.

### Serviços

| Método | Rota | Auth |
|--------|------|------|
| GET | `/services` | (P) apenas ativos |
| GET | `/services/all` | (A) todos |
| GET | `/services/{id}` | (A) |
| POST | `/services` | (A) |
| PUT | `/services/{id}` | (A) |
| DELETE | `/services/{id}` | (A) |

Ícone vazio usa placeholder, análogo aos livros.

### Leads

| Método | Rota | Auth |
|--------|------|------|
| POST | `/leads` | (P) cria lead: `value = 0`, `status = pending`, `notes` vazio; valida `serviceId` (GUID) existente e ativo |
| GET | `/leads` | (A) paginação: `page`, `pageSize`, filtros `status`, `search` (Dapper + LIMIT/OFFSET) |
| GET | `/leads/{id}` | (A) |
| PATCH | `/leads/{id}/status` | (A) |
| PATCH | `/leads/{id}/notes` | (A) |
| PUT | `/leads/{id}` | (A) |
| DELETE | `/leads/{id}` | (A) |

Status aceitos: `pending`, `analyzing`, `working`, `finished`, `cancelled`.

### Posts e comentários

| Método | Rota | Auth |
|--------|------|------|
| GET | `/posts` | (P/A) anônimo: só `published`; autenticado: pode usar `?status=` (ex.: `draft`) |
| GET | `/posts/{id}` | (P/A) mesmo critério de status; comentários incluídos no detalhe |
| POST | `/posts` | (A) |
| PUT | `/posts/{id}` | (A) |
| DELETE | `/posts/{id}` | (A) |
| POST | `/posts/{id}/likes/toggle` | (P) alterna like por **par IP + post** (`post_like_ips`), só post `published` com `allowLikes` |
| POST | `/posts/{id}/comments` | (P) comentário anônimo exige `guestName`; usuário logado pode usar JWT (`sub` / name identifier) |
| PATCH | `/posts/{postId}/comments/{commentId}/author-like` | (A) |
| DELETE | `/posts/{postId}/comments/{commentId}` | (A) |

### Autor

| Método | Rota | Auth |
|--------|------|------|
| GET | `/author` | (P) perfil id=1 + timeline |
| PUT | `/author` | (A) atualiza perfil e substitui itens da timeline |

### Configuração e estatísticas

| Método | Rota | Auth |
|--------|------|------|
| GET | `/config/system` | (A) JSON em `system_settings` (`SystemConfig`) |
| PUT | `/config/system` | (A) corpo `{ "value": { ... } }` |
| GET | `/config/home` | (A) `HomeConfig` |
| PUT | `/config/home` | (A) corpo `{ "value": { ... } }` |
| GET | `/stats` | (A) lê `StatsConfig` |

## Segurança e boas práticas implementadas

- Consultas Dapper **parametrizadas** (mitigação a SQL injection).
- Respostas em **DTOs**, sem expor entidades do banco diretamente.
- **UTC** para `DateTime.UtcNow` em persistência; respostas em ISO 8601 quando serializadas como `DateTime`.
- CORS liberado para desenvolvimento (`AllowAnyOrigin`); restrinja em produção conforme o domínio do front-end.

## Testes manuais

Use o arquivo **`src/WebBookRP_API/WebBookRP_API.http`**: após o login, copie o `token` para a variável `@token` e ajuste GUIDs/IDs de exemplo (`@bookId`, `@serviceId`, etc.) conforme os dados do seu banco.

## Pacotes NuGet principais (API)

- `Dapper`
- `MySqlConnector`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.AspNetCore.OpenApi`

---

Documentação alinhada ao estado atual do código em `src/WebBookRP_API`. Para evoluir o sistema (usuários no banco, refresh token, políticas de CORS, etc.), estenda serviços e repositórios mantendo o mesmo padrão de camadas e DTOs.
