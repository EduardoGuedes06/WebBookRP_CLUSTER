# Postman — WebBookRP API

## Importar

1. Abra o **Postman** → **Import** → arraste ou selecione:
   - `WebBookRP_API.postman_collection.json`
   - `WebBookRP_API.local.postman_environment.json`
2. No canto superior direito, selecione o environment **WebBookRP — Localhost**.
3. Suba a API (`dotnet run` em `src/WebBookRP_API`) e confirme que `baseUrl` aponta para a mesma porta (padrão `http://localhost:5028`).
4. Execute a pasta **Auth** → **Login (salva token)**. O script de testes grava `token` no environment e na collection.
5. Rode as outras pastas na ordem desejada. Requisições **(cuidado)** apagam dados de seed se os IDs não forem trocados.

## Regenerar a collection

Com [Node.js](https://nodejs.org/) instalado, na raiz do repositório:

```bash
node doc/postman/build-collection.mjs
```

## Testes rápidos no terminal (PowerShell)

Com a API no ar e o banco populado (`doc/create.sql` + `doc/insert.sql`):

```powershell
.\doc\postman\smoke-test.ps1 -BaseUrl "http://localhost:5028"
```

O script chama rotas públicas, faz login e valida algumas rotas autenticadas (códigos HTTP esperados).

## Newman (opcional)

```bash
npm i -g newman
newman run doc/postman/WebBookRP_API.postman_collection.json -e doc/postman/WebBookRP_API.local.postman_environment.json --delay-request 100
```

Observação: o **Login** precisa rodar antes (ou defina `token` manualmente no environment) para as rotas com Bearer.
