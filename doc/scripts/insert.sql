
-- CARGA DE DADOS (MOCK)
-- Senha de desenvolvimento: Admin@123 (BCrypt cost 11).
-- Novo hash: na raiz do repositório execute: dotnet run --project tools/BcryptQuick -- "SuaSenha"
INSERT INTO `Users` (`Id`, `Name`, `Email`, `PasswordHash`, `Role`) VALUES
('d2c20a8c-8b8a-4f5b-9d41-5825a07c6f08', 'Eduardo', 'admin@gmail.com', '$2a$11$VGRT7qNyZgiCwn14LOE./ui8btIFmVFG0s424010qHppg7oleGxIq', 'Admin');

INSERT INTO `AuthorProfile` (`Id`, `Name`, `Role`, `AvatarUrl`, `SecondaryImageUrl`, `Bio`) VALUES 
(1, 'Ronaldo Pereira', 'Escritor de Ficção Especulativa', 'https://placehold.co/600x800/1e293b/FFF?text=Autor+Pro', 'https://placehold.co/300x300/d97706/FFF?text=Família', 'Olá! Sou o Ronaldo. Minha jornada começou devorando livros na biblioteca.');

INSERT INTO `AuthorTimeline` (`Year`, `Title`, `Description`, `DisplayOrder`) VALUES 
('2015', 'O Início', 'Primeiro conto publicado.', 1),
('2018', 'Primeiro Prêmio', 'Vencedor concurso nacional.', 2),
('2025', 'Carreira Sólida', '10 mil cópias vendidas.', 3);

INSERT INTO `Books` (`Id`, `Title`, `Genre`, `Price`, `PromoPrice`, `Active`, `IsPromotion`, `CoverUrl`, `Pages`, `Rating`, `ReviewsCount`) VALUES 
('b0000000-0000-0000-0000-000000000001', 'O Código da Eternidade', 'Ficção', 31.90, NULL, 1, 0, 'https://placehold.co/300x450/1e293b/FFF', 270, 4.8, 65),
('b0000000-0000-0000-0000-000000000002', 'Livro Vol.2', 'Thriller', 33.90, 20.00, 1, 1, 'https://placehold.co/300x450/475569/FFF', 290, 4.6, 80);

INSERT INTO `Services` (`Id`, `Name`, `Price`, `PromoPrice`, `Unit`, `Active`, `IsPromotion`, `Icon`, `Theme`) VALUES 
('s0000000-0000-0000-0000-000000000001', 'Design de Capa', 800.00, NULL, NULL, 1, 0, 'ph-paint-brush-broad', 'purple'),
('s0000000-0000-0000-0000-000000000002', 'Revisão Textual', 0.05, 0.03, 'p/ palavra', 1, 1, 'ph-text-aa', 'blue');

INSERT INTO `Posts` (`Id`, `Title`, `Category`, `CoverColor`, `CoverText`, `Status`, `LikesCount`) VALUES 
(1, 'Como venci o bloqueio criativo', 'Escrita', '1e293b', 'Bloqueio Criativo', 'published', 142),
(2, 'O futuro da Ficção', 'Opinião', 'd97706', 'Sci-Fi BR', 'published', 89);

INSERT INTO `Comments` (`PostId`, `GuestName`, `Text`) VALUES 
(1, 'Carlos M.', 'Ótima dica!'),
(1, 'Ana Clara', 'Vou testar hoje.');

INSERT INTO `SystemSettings` (`ConfigKey`, `ConfigValue`) VALUES
('SystemConfig', '{"maxActiveProjects": 4, "enableNotifications": true, "autoReply": false, "currency": "BRL", "adminName": "Ronaldo Pereira"}'),
('HomeConfig', '{"heroBookId": "b0000000-0000-0000-0000-000000000001", "showLaunchBadge": true, "quote": {"text": "Escrever é a única forma...", "author": "Ronaldo Pereira"}}'),
('StatsConfig', '{"booksSold": 0, "activeReaders": 0, "averageRating": 0}');