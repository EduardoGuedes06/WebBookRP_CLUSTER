/**
 * Gera WebBookRP_API.postman_collection.json (Postman v2.1).
 * Execute na raiz do repo: node doc/postman/build-collection.mjs
 */
import { writeFileSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __dirname = dirname(fileURLToPath(import.meta.url));
const out = join(__dirname, "WebBookRP_API.postman_collection.json");

const authHeader = () => [{ key: "Authorization", value: "Bearer {{token}}", type: "text" }];
const jsonHeader = () => [
  { key: "Content-Type", value: "application/json", type: "text" },
];

const R = (name, method, path, opts = {}) => {
  const h = [...(opts.headers || [])];
  if (opts.bearer) h.push(...authHeader());
  if (opts.json && (method === "POST" || method === "PUT" || method === "PATCH"))
    h.push(...jsonHeader());
  const req = {
    method,
    header: h,
    url: "{{baseUrl}}" + path,
  };
  if (opts.body)
    req.body = { mode: "raw", raw: typeof opts.body === "string" ? opts.body : JSON.stringify(opts.body, null, 2) };
  const item = { name, request: req };
  if (opts.test)
    item.event = [
      {
        listen: "test",
        script: {
          type: "text/javascript",
          exec: Array.isArray(opts.test) ? opts.test : [opts.test],
        },
      },
    ];
  return item;
};

const collection = {
  info: {
    name: "WebBookRP API",
    description:
      "Collection gerada para testes em **localhost** (pasta `doc/postman/`).\n\n1. Importe também o arquivo **WebBookRP_API.local.postman_environment.json**.\n2. Selecione o environment **WebBookRP — Localhost**.\n3. Execute **Auth → Login**; o script salva `token` no environment e na collection.\n4. Rode as demais pastas (Bearer usa `{{token}}`).\n\nIDs padrão: `doc/insert.sql`. **POST /posts/{id}/likes/toggle** exige migração `doc/migrations/001_post_like_ips.sql` se a tabela ainda não existir.\n\nRegenerar: `node doc/postman/build-collection.mjs`",
    schema: "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
    _exporter_id: "webbookrp-api",
  },
  variable: [
    { key: "baseUrl", value: "http://localhost:5028" },
    { key: "token", value: "" },
    { key: "loginEmail", value: "admin@gmail.com" },
    { key: "loginPassword", value: "Admin@123" },
    { key: "bookId", value: "b0000000-0000-0000-0000-000000000001" },
    { key: "serviceId", value: "s0000000-0000-0000-0000-000000000001" },
    { key: "postId", value: "1" },
    { key: "commentId", value: "1" },
    { key: "leadId", value: "1" },
  ],
  item: [
    {
      name: "Auth",
      item: [
        R(
          "Login (salva token)",
          "POST",
          "/auth/login",
          {
            json: true,
            body: {
              email: "{{loginEmail}}",
              password: "{{loginPassword}}",
            },
            test: [
              "pm.test('HTTP 200', () => pm.response.to.have.status(200));",
              "const j = pm.response.json();",
              "pm.test('Tem token', () => pm.expect(j.token).to.be.a('string').and.not.empty);",
              "pm.collectionVariables.set('token', j.token);",
              "pm.environment.set('token', j.token);",
            ],
          }
        ),
        R("Logout", "POST", "/auth/logout"),
        R("Session (sem token)", "GET", "/auth/session"),
        R("Session (com Bearer)", "GET", "/auth/session", { bearer: true }),
      ],
    },
    {
      name: "Books",
      item: [
        R("GET /books (admin)", "GET", "/books", { bearer: true }),
        R("GET /books/catalog", "GET", "/books/catalog"),
        R("GET /books/{id}", "GET", "/books/{{bookId}}"),
        R(
          "POST /books",
          "POST",
          "/books",
          {
            bearer: true,
            json: true,
            body: {
              title: "Postman Test Book",
              genre: "Teste",
              price: 9.99,
              promoPrice: null,
              active: true,
              isPromotion: false,
              cover: "",
              synopsis: "Syn",
              coverSynopsis: null,
              linkAmazon: null,
              linkML: null,
              linkShopee: null,
              linkGeneric: null,
              fullSynopsis: "<p>HTML</p>",
              pages: 100,
              rating: 4.0,
              reviews: 0,
            },
          }
        ),
        R(
          "PUT /books/{id}",
          "PUT",
          "/books/{{bookId}}",
          {
            bearer: true,
            json: true,
            body: {
              title: "Postman Update",
              genre: "Teste",
              price: 19.99,
              promoPrice: null,
              active: true,
              isPromotion: false,
              cover: "",
              synopsis: null,
              coverSynopsis: null,
              linkAmazon: null,
              linkML: null,
              linkShopee: null,
              linkGeneric: null,
              fullSynopsis: null,
              pages: 100,
              rating: 4.2,
              reviews: 1,
            },
          }
        ),
        R("DELETE /books/{id} (cuidado)", "DELETE", "/books/{{bookId}}", { bearer: true }),
      ],
    },
    {
      name: "Services",
      item: [
        R("GET /services (público)", "GET", "/services"),
        R("GET /services/all", "GET", "/services/all", { bearer: true }),
        R("GET /services/{id}", "GET", "/services/{{serviceId}}", { bearer: true }),
        R(
          "POST /services",
          "POST",
          "/services",
          {
            bearer: true,
            json: true,
            body: {
              name: "Serviço Postman",
              price: 99,
              unit: "hora",
              active: true,
              isPromotion: false,
              promoPrice: null,
              icon: "",
              theme: "blue",
              description: "Criado via Postman",
            },
          }
        ),
        R(
          "PUT /services/{id}",
          "PUT",
          "/services/{{serviceId}}",
          {
            bearer: true,
            json: true,
            body: {
              name: "Design de Capa",
              price: 850,
              promoPrice: null,
              unit: null,
              active: true,
              isPromotion: false,
              icon: "ph-paint-brush-broad",
              theme: "purple",
              description: "Atualizado Postman",
            },
          }
        ),
        R("DELETE /services/{id} (cuidado)", "DELETE", "/services/{{serviceId}}", { bearer: true }),
      ],
    },
    {
      name: "Leads",
      item: [
        R(
          "POST /leads (público)",
          "POST",
          "/leads",
          {
            json: true,
            body: {
              serviceId: "{{serviceId}}",
              name: "Lead Postman",
              email: "lead-postman@example.com",
              phone: "11999990000",
              description: "Teste collection",
            },
          }
        ),
        R("GET /leads", "GET", "/leads?page=1&pageSize=20&status=pending&search=", { bearer: true }),
        R("GET /leads/{id}", "GET", "/leads/{{leadId}}", { bearer: true }),
        R(
          "PATCH /leads/{id}/status",
          "PATCH",
          "/leads/{{leadId}}/status",
          { bearer: true, json: true, body: { status: "analyzing" } }
        ),
        R(
          "PATCH /leads/{id}/notes",
          "PATCH",
          "/leads/{{leadId}}/notes",
          { bearer: true, json: true, body: { notes: "Nota Postman" } }
        ),
        R(
          "PUT /leads/{id}",
          "PUT",
          "/leads/{{leadId}}",
          {
            bearer: true,
            json: true,
            body: {
              serviceId: "{{serviceId}}",
              name: "Lead Atualizado",
              email: "lead-updated@example.com",
              phone: null,
              description: "PUT",
              value: 100,
              status: "working",
              notes: "OK",
            },
          }
        ),
        R("DELETE /leads/{id} (cuidado)", "DELETE", "/leads/{{leadId}}", { bearer: true }),
      ],
    },
    {
      name: "Posts",
      item: [
        R("GET /posts?status=published", "GET", "/posts?status=published"),
        R("GET /posts (admin — requer token)", "GET", "/posts", { bearer: true }),
        R("GET /posts/{id}", "GET", "/posts/{{postId}}"),
        R(
          "POST /posts",
          "POST",
          "/posts",
          {
            bearer: true,
            json: true,
            body: {
              title: "Post Postman",
              category: "Teste",
              coverType: "color",
              coverColor: "#111827",
              coverText: "T",
              coverTextColor: "#fff",
              imageUrl: null,
              content: "Conteúdo",
              externalLink: null,
              allowLikes: true,
              allowComments: true,
              status: "draft",
            },
          }
        ),
        R(
          "PUT /posts/{id}",
          "PUT",
          "/posts/{{postId}}",
          {
            bearer: true,
            json: true,
            body: {
              title: "Post Atualizado Postman",
              category: "Escrita",
              coverType: "color",
              coverColor: "1e293b",
              coverText: "Bloqueio Criativo",
              coverTextColor: "#ffffff",
              imageUrl: null,
              content: "Conteúdo atualizado",
              externalLink: null,
              allowLikes: true,
              allowComments: true,
              status: "published",
            },
          }
        ),
        R("POST /posts/{id}/likes/toggle", "POST", "/posts/{{postId}}/likes/toggle"),
        R(
          "POST /posts/{id}/comments",
          "POST",
          "/posts/{{postId}}/comments",
          {
            json: true,
            body: { text: "Comentário Postman", guestName: "Visitante PM" },
          }
        ),
        R(
          "PATCH comments author-like",
          "PATCH",
          "/posts/{{postId}}/comments/{{commentId}}/author-like",
          { bearer: true, json: true, body: { authorLike: true } }
        ),
        R(
          "DELETE comment",
          "DELETE",
          "/posts/{{postId}}/comments/{{commentId}}",
          { bearer: true }
        ),
        R("DELETE /posts/{id} (cuidado)", "DELETE", "/posts/{{postId}}", { bearer: true }),
      ],
    },
    {
      name: "Author",
      item: [
        R("GET /author", "GET", "/author"),
        R(
          "PUT /author",
          "PUT",
          "/author",
          {
            bearer: true,
            json: true,
            body: {
              name: "Ronaldo Pereira",
              role: "Escritor",
              avatarUrl: null,
              secondaryImageUrl: null,
              bio: "Bio via Postman",
              timeline: [
                { id: null, year: "2015", title: "O Início", description: "...", sortOrder: 0 },
              ],
            },
          }
        ),
      ],
    },
    {
      name: "Config & Stats",
      item: [
        R("GET /config/system", "GET", "/config/system", { bearer: true }),
        R(
          "PUT /config/system",
          "PUT",
          "/config/system",
          {
            bearer: true,
            json: true,
            body: {
              value: { maxActiveProjects: 4, currency: "BRL", adminName: "Ronaldo Pereira" },
            },
          }
        ),
        R("GET /config/home", "GET", "/config/home", { bearer: true }),
        R(
          "PUT /config/home",
          "PUT",
          "/config/home",
          {
            bearer: true,
            json: true,
            body: {
              value: { heroBookId: "{{bookId}}", showLaunchBadge: true },
            },
          }
        ),
        R("GET /stats", "GET", "/stats", { bearer: true }),
      ],
    },
  ],
};

writeFileSync(out, JSON.stringify(collection, null, 2), "utf8");
console.log("Written:", out);
