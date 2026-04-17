DROP DATABASE IF EXISTS `WebBookRP_db`;
CREATE DATABASE `WebBookRP_db`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `WebBookRP_db`;

-- ESTRUTURA
CREATE TABLE `Users` (
    `Id` CHAR(36) NOT NULL PRIMARY KEY,
    `Name` VARCHAR(255) NOT NULL,
    `Email` VARCHAR(255) NOT NULL UNIQUE,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `Role` VARCHAR(50) NOT NULL DEFAULT 'Admin',
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE `AuthorProfile` (
    `Id` INT NOT NULL PRIMARY KEY DEFAULT 1,
    `Name` VARCHAR(255) NOT NULL,
    `Role` VARCHAR(255),
    `AvatarUrl` VARCHAR(500),
    `SecondaryImageUrl` VARCHAR(500),
    `Bio` TEXT
) ENGINE=InnoDB;

CREATE TABLE `AuthorTimeline` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `AuthorId` INT DEFAULT 1,
    `Year` VARCHAR(10) NOT NULL,
    `Title` VARCHAR(255) NOT NULL,
    `Description` TEXT,
    `DisplayOrder` INT DEFAULT 0,
    CONSTRAINT `FK_Timeline_Author` FOREIGN KEY (`AuthorId`) REFERENCES `AuthorProfile` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE `Books` (
    `Id` CHAR(36) NOT NULL PRIMARY KEY,
    `Title` VARCHAR(255) NOT NULL,
    `Genre` VARCHAR(100),
    `Price` DECIMAL(10, 2) DEFAULT 0.00,
    `PromoPrice` DECIMAL(10, 2),
    `Active` TINYINT(1) DEFAULT 1,
    `IsPromotion` TINYINT(1) DEFAULT 0,
    `CoverUrl` VARCHAR(500),
    `CoverSynopsis` VARCHAR(100),
    `FullSynopsis` TEXT,
    `Pages` INT,
    `Rating` DECIMAL(3, 1),
    `ReviewsCount` INT DEFAULT 0,
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE `Services` (
    `Id` CHAR(36) NOT NULL PRIMARY KEY,
    `Name` VARCHAR(255) NOT NULL,
    `Price` DECIMAL(10, 2) DEFAULT 0.00,
    `PromoPrice` DECIMAL(10, 2),
    `Unit` VARCHAR(50),
    `Active` TINYINT(1) DEFAULT 1,
    `IsPromotion` TINYINT(1) DEFAULT 0,
    `Icon` VARCHAR(100),
    `Theme` VARCHAR(50),
    `Description` TEXT,
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE `Leads` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ServiceId` CHAR(36),
    `Name` VARCHAR(255) NOT NULL,
    `Email` VARCHAR(255) NOT NULL,
    `Phone` VARCHAR(50),
    `Description` TEXT,
    `AgreedValue` DECIMAL(10, 2) DEFAULT 0.00,
    `Status` VARCHAR(50) DEFAULT 'pending',
    `InternalNotes` TEXT,
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `FK_Leads_Services` FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE `Posts` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Title` VARCHAR(255) NOT NULL,
    `Category` VARCHAR(100),
    `CoverType` VARCHAR(50) DEFAULT 'color',
    `CoverColor` VARCHAR(7),
    `CoverText` VARCHAR(255),
    `CoverTextColor` VARCHAR(20),
    `ImageUrl` VARCHAR(500),
    `Content` LONGTEXT,
    `Status` VARCHAR(50) DEFAULT 'draft',
    `LikesCount` INT DEFAULT 0,
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE `Comments` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `PostId` INT NOT NULL,
    `UserId` CHAR(36),
    `GuestName` VARCHAR(100),
    `Text` TEXT NOT NULL,
    `AuthorLike` TINYINT(1) DEFAULT 0,
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `FK_Comments_Posts` FOREIGN KEY (`PostId`) REFERENCES `Posts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE `SystemSettings` (
    `ConfigKey` VARCHAR(100) PRIMARY KEY,
    `ConfigValue` JSON NOT NULL,
    `UpdatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE `SecurityLogs` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `EmailAttempt` VARCHAR(255),
    `Success` TINYINT(1),
    `IpAddress` VARCHAR(45),
    `UserAgent` VARCHAR(500),
    `CreatedAt` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;