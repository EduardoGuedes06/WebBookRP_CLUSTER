-- Posts.ImageUrl: armazena URL http(s) ou data-uri (base64) para o front consumir no JSON.
-- Rode no banco WebBookRP_db (ou o nome configurado na connection string).

ALTER TABLE Posts
    MODIFY COLUMN ImageUrl LONGTEXT NULL;
