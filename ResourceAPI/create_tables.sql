  IF EXISTS(SELECT 1 FROM information_schema.tables 
  WHERE table_name = '
'__EFMigrationsHistory'' AND table_schema = DATABASE()) 
BEGIN
CREATE TABLE `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

END;

CREATE TABLE `Authors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` text NULL,
    `Email` text NULL,
    `UserId` text NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Tags` (
    `Url` varchar(767) NOT NULL,
    `Name` text CHARACTER SET utf8 NULL,
    `PostCount` int NOT NULL,
    PRIMARY KEY (`Url`)
);

CREATE TABLE `Exercises` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Content` text CHARACTER SET utf8 NULL,
    `Points` int NOT NULL,
    `AuthorId` int NULL,
    `Created` datetime NOT NULL,
    `Edited` datetime NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Exercises_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Problems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Content` text CHARACTER SET utf8 NULL,
    `Points` int NOT NULL,
    `AuthorId` int NULL,
    `Created` datetime NOT NULL,
    `Edited` datetime NOT NULL,
    `Title` text NULL,
    `Source` text NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Problems_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `VoteElement` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ElementId` int NOT NULL,
    `Vote` int NOT NULL,
    `AuthorId` int NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_VoteElement_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Answers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Content` text CHARACTER SET utf8 NULL,
    `Points` int NOT NULL,
    `AuthorId` int NULL,
    `Created` datetime NOT NULL,
    `Edited` datetime NOT NULL,
    `ParentId` int NOT NULL,
    `IsApproved` bit NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Answers_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Answers_Problems_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Problems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProblemTags` (
    `ProblemId` int NOT NULL,
    `TagUrl` varchar(767) NOT NULL,
    PRIMARY KEY (`TagUrl`, `ProblemId`),
    CONSTRAINT `FK_ProblemTags_Problems_ProblemId` FOREIGN KEY (`ProblemId`) REFERENCES `Problems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ProblemTags_Tags_TagUrl` FOREIGN KEY (`TagUrl`) REFERENCES `Tags` (`Url`) ON DELETE CASCADE
);

CREATE TABLE `ProblemVotes` (
    `AuthorId` int NOT NULL,
    `ProblemId` int NOT NULL,
    `Vote` int NOT NULL,
    PRIMARY KEY (`ProblemId`, `AuthorId`),
    CONSTRAINT `FK_ProblemVote_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ProblemVote_Problems_ProblemId` FOREIGN KEY (`ProblemId`) REFERENCES `Problems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AnswerVotes` (
    `AnswerId` int NOT NULL,
    `AuthorId` int NOT NULL,
    `Vote` int NOT NULL,
    PRIMARY KEY (`AnswerId`, `AuthorId`),
    CONSTRAINT `FK_AnswerVote_Answers_AnswerId` FOREIGN KEY (`AnswerId`) REFERENCES `Answers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AnswerVote_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Comments` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Content` text CHARACTER SET utf8 NULL,
    `Points` int NOT NULL,
    `AuthorId` int NULL,
    `Created` datetime NOT NULL,
    `Edited` datetime NOT NULL,
    `AnswerId` int NULL,
    `CommentId` int NULL,
    `ExerciseId` int NULL,
    `ProblemId` int NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Comments_Answers_AnswerId` FOREIGN KEY (`AnswerId`) REFERENCES `Answers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Comments_Authors_AuthorId` FOREIGN KEY (`AuthorId`) REFERENCES `Authors` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Comments_Comments_CommentId` FOREIGN KEY (`CommentId`) REFERENCES `Comments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Comments_Exercises_ExerciseId` FOREIGN KEY (`ExerciseId`) REFERENCES `Exercises` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Comments_Problems_ProblemId` FOREIGN KEY (`ProblemId`) REFERENCES `Problems` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_Answers_AuthorId` ON `Answers` (`AuthorId`);

CREATE INDEX `IX_Answers_ParentId` ON `Answers` (`ParentId`);

CREATE INDEX `IX_AnswerVote_AuthorId` ON `AnswerVote` (`AuthorId`);

CREATE INDEX `IX_Comments_AnswerId` ON `Comments` (`AnswerId`);

CREATE INDEX `IX_Comments_AuthorId` ON `Comments` (`AuthorId`);

CREATE INDEX `IX_Comments_CommentId` ON `Comments` (`CommentId`);

CREATE INDEX `IX_Comments_ExerciseId` ON `Comments` (`ExerciseId`);

CREATE INDEX `IX_Comments_ProblemId` ON `Comments` (`ProblemId`);

CREATE INDEX `IX_Exercises_AuthorId` ON `Exercises` (`AuthorId`);

CREATE INDEX `IX_Problems_AuthorId` ON `Problems` (`AuthorId`);

CREATE INDEX `IX_ProblemTags_ProblemId` ON `ProblemTags` (`ProblemId`);

CREATE INDEX `IX_ProblemVote_AuthorId` ON `ProblemVote` (`AuthorId`);

CREATE INDEX `IX_VoteElement_AuthorId` ON `VoteElement` (`AuthorId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200701200738_Initial', '3.1.5');

