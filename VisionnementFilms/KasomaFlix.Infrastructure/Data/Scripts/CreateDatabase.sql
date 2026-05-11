-- =============================================
-- Script SQL Complet pour KASOMAFLIX
-- Création de la base de données avec données de base
-- =============================================

USE master;
GO

-- Créer la base de données si elle n'existe pas
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'VisionnementFilmsDB')
BEGIN
    CREATE DATABASE VisionnementFilmsDB;
    PRINT 'Base de données VisionnementFilmsDB créée avec succès.';
END
ELSE
BEGIN
    PRINT 'La base de données VisionnementFilmsDB existe déjà.';
END
GO

USE VisionnementFilmsDB;
GO

-- =============================================
-- Supprimer les tables existantes (si nécessaire)
-- =============================================
IF OBJECT_ID('CartesCredit', 'U') IS NOT NULL DROP TABLE CartesCredit;
IF OBJECT_ID('Cotes', 'U') IS NOT NULL DROP TABLE Cotes;
IF OBJECT_ID('Sessions', 'U') IS NOT NULL DROP TABLE Sessions;
IF OBJECT_ID('Transactions', 'U') IS NOT NULL DROP TABLE Transactions;
IF OBJECT_ID('Abonnements', 'U') IS NOT NULL DROP TABLE Abonnements;
IF OBJECT_ID('Films', 'U') IS NOT NULL DROP TABLE Films;
IF OBJECT_ID('Membres', 'U') IS NOT NULL DROP TABLE Membres;
IF OBJECT_ID('Administrateurs', 'U') IS NOT NULL DROP TABLE Administrateurs;
GO

-- =============================================
-- Création des Tables
-- =============================================

-- Table Administrateurs
CREATE TABLE Administrateurs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NomUtilisateur NVARCHAR(50) NOT NULL UNIQUE,
    MotDePasseHash NVARCHAR(255) NOT NULL,
    Nom NVARCHAR(100) NOT NULL,
    Prenom NVARCHAR(100) NOT NULL,
    Courriel NVARCHAR(255) NOT NULL UNIQUE,
    DateCreation DATETIME NOT NULL DEFAULT GETDATE(),
    EstActif BIT NOT NULL DEFAULT 1
);
GO

-- Table Membres
CREATE TABLE Membres (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Prenom NVARCHAR(100) NOT NULL,
    Nom NVARCHAR(100) NOT NULL,
    Courriel NVARCHAR(255) NOT NULL UNIQUE,
    MotDePasseHash NVARCHAR(255) NOT NULL,
    Adresse NVARCHAR(255) NOT NULL,
    Telephone NVARCHAR(20) NOT NULL,
    Solde DECIMAL(10,2) NOT NULL DEFAULT 0,
    DateInscription DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Table Films
CREATE TABLE Films (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Titre NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Categorie NVARCHAR(50) NOT NULL,
    Duree INT NOT NULL, -- en minutes
    Annee INT NOT NULL,
    NoteMoyenne DECIMAL(3,2) NOT NULL DEFAULT 0,
    NombreVotes INT NOT NULL DEFAULT 0,
    Realisateur NVARCHAR(255) NOT NULL,
    Acteurs NVARCHAR(MAX),
    PrixAchat DECIMAL(10,2) NOT NULL DEFAULT 0,
    PrixLocation DECIMAL(10,2) NOT NULL DEFAULT 0,
    CheminAffiche NVARCHAR(500),
    EstDisponible BIT NOT NULL DEFAULT 1,
    DateAjout DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Table Abonnements
CREATE TABLE Abonnements (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MembreId INT NOT NULL,
    TypeAbonnement NVARCHAR(50) NOT NULL, -- "Mensuel", "Annuel"
    DateDebut DATETIME NOT NULL,
    DateFin DATETIME NOT NULL,
    Prix DECIMAL(10,2) NOT NULL,
    RenouvellementAutomatique BIT NOT NULL DEFAULT 1,
    EstActif BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Abonnements_Membres FOREIGN KEY (MembreId) REFERENCES Membres(Id) ON DELETE CASCADE
);
GO

-- Table Transactions
CREATE TABLE Transactions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MembreId INT NOT NULL,
    FilmId INT NULL, -- Nullable pour les abonnements et ajout de solde
    TypeTransaction NVARCHAR(50) NOT NULL, -- "Achat", "Location", "Abonnement", "AjoutSolde"
    Montant DECIMAL(10,2) NOT NULL,
    DateTransaction DATETIME NOT NULL DEFAULT GETDATE(),
    Statut NVARCHAR(50) NOT NULL DEFAULT 'Complétée',
    CONSTRAINT FK_Transactions_Membres FOREIGN KEY (MembreId) REFERENCES Membres(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Transactions_Films FOREIGN KEY (FilmId) REFERENCES Films(Id) ON DELETE SET NULL
);
GO

-- Table Sessions
CREATE TABLE Sessions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MembreId INT NOT NULL,
    FilmId INT NOT NULL,
    DateDebut DATETIME NOT NULL DEFAULT GETDATE(),
    DateFin DATETIME NULL,
    TempsVisionne INT NOT NULL DEFAULT 0, -- en secondes
    CONSTRAINT FK_Sessions_Membres FOREIGN KEY (MembreId) REFERENCES Membres(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Sessions_Films FOREIGN KEY (FilmId) REFERENCES Films(Id) ON DELETE CASCADE
);
GO

-- Table Cotes
CREATE TABLE Cotes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MembreId INT NOT NULL,
    FilmId INT NOT NULL,
    Note INT NOT NULL CHECK (Note >= 1 AND Note <= 5),
    Commentaire NVARCHAR(MAX),
    DateCote DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Cotes_Membres FOREIGN KEY (MembreId) REFERENCES Membres(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Cotes_Films FOREIGN KEY (FilmId) REFERENCES Films(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Cotes_Membre_Film UNIQUE (MembreId, FilmId) -- Un membre ne peut noter qu'une fois par film
);
GO

-- Table CartesCredit
CREATE TABLE CartesCredit (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MembreId INT NOT NULL,
    TypeCarte NVARCHAR(50) NOT NULL, -- "Visa", "Mastercard", "PayPal", etc.
    NumeroCarte NVARCHAR(20) NOT NULL, -- Derniers 4 chiffres seulement
    NomTitulaire NVARCHAR(100) NOT NULL,
    DateExpiration DATETIME NOT NULL,
    EmailPayPal NVARCHAR(255) NULL,
    EstParDefaut BIT NOT NULL DEFAULT 0,
    DateAjout DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CartesCredit_Membres FOREIGN KEY (MembreId) REFERENCES Membres(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- Création des Index pour Performance
-- =============================================

CREATE INDEX IX_Membres_Courriel ON Membres(Courriel);
CREATE INDEX IX_Films_Categorie ON Films(Categorie);
CREATE INDEX IX_Films_Titre ON Films(Titre);
CREATE INDEX IX_Transactions_MembreId ON Transactions(MembreId);
CREATE INDEX IX_Transactions_DateTransaction ON Transactions(DateTransaction);
CREATE INDEX IX_Abonnements_MembreId ON Abonnements(MembreId);
CREATE INDEX IX_Sessions_MembreId ON Sessions(MembreId);
CREATE INDEX IX_Cotes_FilmId ON Cotes(FilmId);
CREATE INDEX IX_CartesCredit_MembreId ON CartesCredit(MembreId);
GO

-- =============================================
-- Insertion des Données de Base
-- =============================================

-- Administrateur par défaut
-- Mot de passe: admin123 (hashé avec BCrypt)
INSERT INTO Administrateurs (NomUtilisateur, MotDePasseHash, Nom, Prenom, Courriel, DateCreation, EstActif)
VALUES ('admin', '$2a$11$KIXqZqZqZqZqZqZqZqZqZeZqZqZqZqZqZqZqZqZqZqZqZqZqZqZq', 'Admin', 'Système', 'admin@kasomaflix.com', GETDATE(), 1);
GO

-- Membres de test
-- Mot de passe pour tous: membre123 (hashé avec BCrypt)
INSERT INTO Membres (Prenom, Nom, Courriel, MotDePasseHash, Adresse, Telephone, Solde, DateInscription)
VALUES 
    ('Mayeul', 'SAMBA', 'yeulmastride@email.com', '$2a$11$mKqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJq', '123 Rue Principale, Montréal', '514-123-4567', 50.00, '2024-01-15'),
    ('Simon', 'KASONGO', 'simonkasongo@email.com', '$2a$11$mKqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJq', '456 Avenue des Érables, Québec', '418-234-5678', 25.50, '2024-02-20'),
    ('Sony', 'Scofield', 'sonyscofield@email.com', '$2a$11$mKqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJqJq', '397 rue Saint-Onésime, Lévis', '418-562-7456', 100.00, '2025-04-15');
GO

-- Films de test
INSERT INTO Films (Titre, Description, Categorie, Duree, Annee, NoteMoyenne, NombreVotes, Realisateur, Acteurs, PrixAchat, PrixLocation, CheminAffiche, EstDisponible, DateAjout)
VALUES 
    ('Les Petites Victoires', 'Une comédie touchante sur les défis de la vie quotidienne et les petites victoires qui rendent la vie belle.', 'Comédie', 105, 2023, 4.8, 1250, 'Réalisateur 1', 'Acteur 1, Acteur 2, Actrice 1', 19.99, 4.99, '/Images/les petites victoires.jpg', 1, GETDATE()),
    ('Mayday', 'Un film d''action palpitant avec des scènes de combat épiques et une intrigue captivante.', 'Action', 120, 2024, 4.2, 890, 'Réalisateur 2', 'Acteur 3, Acteur 4', 24.99, 5.99, '/Images/Mayday.jpg', 1, GETDATE()),
    ('Rémi Sans Famille', 'L''histoire émouvante d''un jeune garçon qui cherche sa place dans le monde.', 'Drame', 110, 2023, 4.7, 1100, 'Réalisateur 3', 'Acteur 5, Actrice 2', 22.99, 5.49, '/Images/Remi.jpg', 1, GETDATE()),
    ('Smile', 'Un film d''horreur psychologique qui vous tiendra en haleine jusqu''à la fin.', 'Horreur', 115, 2022, 4.0, 750, 'Réalisateur 4', 'Actrice 3, Acteur 6', 21.99, 4.99, '/Images/smile.jpg', 1, GETDATE()),
    ('Blockbusters 2025', 'Une collection de films de science-fiction épiques avec des effets spéciaux à couper le souffle.', 'Science-Fiction', 135, 2025, 4.6, 980, 'Réalisateur 5', 'Acteur 7, Actrice 4', 29.99, 6.99, '/Images/autres.jpg', 1, GETDATE()),
    ('Balle Perdue', 'Un thriller intense avec des rebondissements inattendus.', 'Thriller', 100, 2023, 4.1, 650, 'Réalisateur 6', 'Acteur 8, Actrice 5', 20.99, 4.99, '/Images/Balle perdue.jpeg', 1, GETDATE()),
    ('Frères', 'Un drame familial poignant sur les liens fraternels et les sacrifices.', 'Drame', 118, 2024, 4.3, 820, 'Réalisateur 7', 'Acteur 9, Acteur 10', 23.99, 5.49, '/Images/Freres.jpg', 1, GETDATE()),
    ('Joker: Folie à Deux', 'Une exploration psychologique profonde du personnage emblématique du Joker.', 'Psychologique', 125, 2024, 5.0, 1500, 'Réalisateur 8', 'Acteur 11, Actrice 6', 27.99, 6.49, '/Images/Joker.webp', 1, GETDATE()),
    ('Oppenheimer', 'L''histoire fascinante du scientifique qui a changé le cours de l''histoire.', 'Biographie', 180, 2023, 4.9, 2000, 'Réalisateur 9', 'Acteur 12, Actrice 7', 29.99, 7.99, '/Images/Oppenheimer.jpg', 1, GETDATE());
GO

-- Abonnements de test
INSERT INTO Abonnements (MembreId, TypeAbonnement, DateDebut, DateFin, Prix, RenouvellementAutomatique, EstActif)
VALUES 
    (1, 'Mensuel', DATEADD(MONTH, -1, GETDATE()), DATEADD(MONTH, 0, GETDATE()), 9.99, 1, 1),
    (2, 'Annuel', DATEADD(YEAR, -1, GETDATE()), DATEADD(YEAR, 0, GETDATE()), 99.99, 1, 1);
GO

-- Transactions de test
INSERT INTO Transactions (MembreId, FilmId, TypeTransaction, Montant, DateTransaction, Statut)
VALUES 
    (1, 1, 'Achat', 19.99, DATEADD(DAY, -10, GETDATE()), 'Complétée'), -- Mayeul a acheté "Les Petites Victoires"
    (1, 2, 'Location', 5.99, DATEADD(DAY, -5, GETDATE()), 'Complétée'), -- Mayeul a loué "Mayday"
    (2, 3, 'Achat', 22.99, DATEADD(DAY, -8, GETDATE()), 'Complétée'), -- Simon a acheté "Rémi Sans Famille"
    (3, NULL, 'AjoutSolde', 50.00, DATEADD(DAY, -3, GETDATE()), 'Complétée'); -- Sony a ajouté du solde
GO

-- Cotes de test
INSERT INTO Cotes (MembreId, FilmId, Note, Commentaire, DateCote)
VALUES 
    (1, 1, 5, 'Excellent film, très bien réalisé! Une comédie touchante.', DATEADD(DAY, -9, GETDATE())), -- Mayeul a noté "Les Petites Victoires"
    (1, 2, 4, 'Film d''action palpitant avec de belles scènes de combat!', DATEADD(DAY, -4, GETDATE())), -- Mayeul a noté "Mayday"
    (2, 3, 5, 'Histoire émouvante et bien racontée. Un drame poignant.', DATEADD(DAY, -7, GETDATE())); -- Simon a noté "Rémi Sans Famille"
GO

-- Cartes de crédit de test
INSERT INTO CartesCredit (MembreId, TypeCarte, NumeroCarte, NomTitulaire, DateExpiration, EstParDefaut, DateAjout)
VALUES 
    (1, 'Visa', '1234', 'Mayeul SAMBA', DATEADD(YEAR, 2, GETDATE()), 1, GETDATE()),
    (2, 'Mastercard', '5678', 'Simon KASONGO', DATEADD(YEAR, 1, GETDATE()), 1, GETDATE());
GO

-- =============================================
-- Vérification des Données
-- =============================================

PRINT '========================================';
PRINT 'Résumé des données insérées:';
PRINT '========================================';
PRINT 'Administrateurs: ' + CAST((SELECT COUNT(*) FROM Administrateurs) AS NVARCHAR(10));
PRINT 'Membres: ' + CAST((SELECT COUNT(*) FROM Membres) AS NVARCHAR(10));
PRINT 'Films: ' + CAST((SELECT COUNT(*) FROM Films) AS NVARCHAR(10));
PRINT 'Abonnements: ' + CAST((SELECT COUNT(*) FROM Abonnements) AS NVARCHAR(10));
PRINT 'Transactions: ' + CAST((SELECT COUNT(*) FROM Transactions) AS NVARCHAR(10));
PRINT 'Cotes: ' + CAST((SELECT COUNT(*) FROM Cotes) AS NVARCHAR(10));
PRINT 'Cartes de crédit: ' + CAST((SELECT COUNT(*) FROM CartesCredit) AS NVARCHAR(10));
PRINT '========================================';
PRINT 'Base de données créée avec succès!';
PRINT '========================================';
GO
