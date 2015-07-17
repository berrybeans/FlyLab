
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/09/2014 09:26:59
-- Generated from EDMX file: C:\Users\daa0006\Documents\Visual Studio 2013\Projects\FlyLab\FlyLab\Models\LabEntity.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FlyLab];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_GenderFly]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fly] DROP CONSTRAINT [FK_GenderFly];
GO
IF OBJECT_ID(N'[dbo].[FK_ModuleFly]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fly] DROP CONSTRAINT [FK_ModuleFly];
GO
IF OBJECT_ID(N'[dbo].[FK_CategoryTrait]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trait] DROP CONSTRAINT [FK_CategoryTrait];
GO
IF OBJECT_ID(N'[dbo].[FK_FlyTrait_Fly]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FlyTrait] DROP CONSTRAINT [FK_FlyTrait_Fly];
GO
IF OBJECT_ID(N'[dbo].[FK_FlyTrait_Trait]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FlyTrait] DROP CONSTRAINT [FK_FlyTrait_Trait];
GO
IF OBJECT_ID(N'[dbo].[FK_LabUserUseInstance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UseInstance] DROP CONSTRAINT [FK_LabUserUseInstance];
GO
IF OBJECT_ID(N'[dbo].[FK_UseInstanceModule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UseInstance] DROP CONSTRAINT [FK_UseInstanceModule];
GO
IF OBJECT_ID(N'[dbo].[FK_UseInstanceFly]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fly] DROP CONSTRAINT [FK_UseInstanceFly];
GO
IF OBJECT_ID(N'[dbo].[FK_TraitTrait]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trait] DROP CONSTRAINT [FK_TraitTrait];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Fly]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fly];
GO
IF OBJECT_ID(N'[dbo].[Trait]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Trait];
GO
IF OBJECT_ID(N'[dbo].[Gender]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Gender];
GO
IF OBJECT_ID(N'[dbo].[Module]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Module];
GO
IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[LabUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LabUser];
GO
IF OBJECT_ID(N'[dbo].[UseInstance]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UseInstance];
GO
IF OBJECT_ID(N'[dbo].[ImageSettings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageSettings];
GO
IF OBJECT_ID(N'[dbo].[FlyTrait]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FlyTrait];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Fly'
CREATE TABLE [dbo].[Fly] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GenderId] int  NOT NULL,
    [ModuleId] int  NOT NULL,
    [UseInstanceId] int  NOT NULL
);
GO

-- Creating table 'Trait'
CREATE TABLE [dbo].[Trait] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [CategoryId] int  NOT NULL,
    [IsDominant] bit  NOT NULL,
    [IsIncompleteDominant] bit  NOT NULL,
    [IsLethal] bit  NOT NULL,
    [ChromosomeNumber] tinyint  NOT NULL,
    [Distance] float  NOT NULL,
    [IsHeterozygous] bit  NULL,
    [ImagePath] nvarchar(max)  NOT NULL,
    [Father_Id] int  NULL
);
GO

-- Creating table 'Gender'
CREATE TABLE [dbo].[Gender] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GenderName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Module'
CREATE TABLE [dbo].[Module] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ModuleName] nvarchar(max)  NOT NULL,
    [Call_id] int  NULL
);
GO

-- Creating table 'Category'
CREATE TABLE [dbo].[Category] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CatName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'LabUser'
CREATE TABLE [dbo].[LabUser] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GID] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Active] bit  NOT NULL
);
GO

-- Creating table 'UseInstance'
CREATE TABLE [dbo].[UseInstance] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Time] datetime  NOT NULL,
    [LabUserId] int  NOT NULL,
    [ModuleId] int  NOT NULL,
    [Stage] varchar(50)  NULL,
    [Browser] varchar(50)  NULL,
    [OS] varchar(50)  NULL,
    [IP] varchar(50)  NULL,
    [Active] bit  NULL
);
GO

-- Creating table 'ImageSettings'
CREATE TABLE [dbo].[ImageSettings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Prefix] nvarchar(max)  NOT NULL,
    [FirstCat] nvarchar(max)  NOT NULL,
    [SecCat] nvarchar(max)  NOT NULL,
    [Suffix] varchar(50)  NOT NULL
);
GO

-- Creating table 'FlyTrait'
CREATE TABLE [dbo].[FlyTrait] (
    [Flies_Id] int  NOT NULL,
    [Traits_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Fly'
ALTER TABLE [dbo].[Fly]
ADD CONSTRAINT [PK_Fly]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Trait'
ALTER TABLE [dbo].[Trait]
ADD CONSTRAINT [PK_Trait]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Gender'
ALTER TABLE [dbo].[Gender]
ADD CONSTRAINT [PK_Gender]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Module'
ALTER TABLE [dbo].[Module]
ADD CONSTRAINT [PK_Module]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Category'
ALTER TABLE [dbo].[Category]
ADD CONSTRAINT [PK_Category]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LabUser'
ALTER TABLE [dbo].[LabUser]
ADD CONSTRAINT [PK_LabUser]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UseInstance'
ALTER TABLE [dbo].[UseInstance]
ADD CONSTRAINT [PK_UseInstance]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ImageSettings'
ALTER TABLE [dbo].[ImageSettings]
ADD CONSTRAINT [PK_ImageSettings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Flies_Id], [Traits_Id] in table 'FlyTrait'
ALTER TABLE [dbo].[FlyTrait]
ADD CONSTRAINT [PK_FlyTrait]
    PRIMARY KEY CLUSTERED ([Flies_Id], [Traits_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [GenderId] in table 'Fly'
ALTER TABLE [dbo].[Fly]
ADD CONSTRAINT [FK_GenderFly]
    FOREIGN KEY ([GenderId])
    REFERENCES [dbo].[Gender]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GenderFly'
CREATE INDEX [IX_FK_GenderFly]
ON [dbo].[Fly]
    ([GenderId]);
GO

-- Creating foreign key on [ModuleId] in table 'Fly'
ALTER TABLE [dbo].[Fly]
ADD CONSTRAINT [FK_ModuleFly]
    FOREIGN KEY ([ModuleId])
    REFERENCES [dbo].[Module]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ModuleFly'
CREATE INDEX [IX_FK_ModuleFly]
ON [dbo].[Fly]
    ([ModuleId]);
GO

-- Creating foreign key on [CategoryId] in table 'Trait'
ALTER TABLE [dbo].[Trait]
ADD CONSTRAINT [FK_CategoryTrait]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[Category]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CategoryTrait'
CREATE INDEX [IX_FK_CategoryTrait]
ON [dbo].[Trait]
    ([CategoryId]);
GO

-- Creating foreign key on [Flies_Id] in table 'FlyTrait'
ALTER TABLE [dbo].[FlyTrait]
ADD CONSTRAINT [FK_FlyTrait_Fly]
    FOREIGN KEY ([Flies_Id])
    REFERENCES [dbo].[Fly]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Traits_Id] in table 'FlyTrait'
ALTER TABLE [dbo].[FlyTrait]
ADD CONSTRAINT [FK_FlyTrait_Trait]
    FOREIGN KEY ([Traits_Id])
    REFERENCES [dbo].[Trait]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FlyTrait_Trait'
CREATE INDEX [IX_FK_FlyTrait_Trait]
ON [dbo].[FlyTrait]
    ([Traits_Id]);
GO

-- Creating foreign key on [LabUserId] in table 'UseInstance'
ALTER TABLE [dbo].[UseInstance]
ADD CONSTRAINT [FK_LabUserUseInstance]
    FOREIGN KEY ([LabUserId])
    REFERENCES [dbo].[LabUser]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LabUserUseInstance'
CREATE INDEX [IX_FK_LabUserUseInstance]
ON [dbo].[UseInstance]
    ([LabUserId]);
GO

-- Creating foreign key on [ModuleId] in table 'UseInstance'
ALTER TABLE [dbo].[UseInstance]
ADD CONSTRAINT [FK_UseInstanceModule]
    FOREIGN KEY ([ModuleId])
    REFERENCES [dbo].[Module]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UseInstanceModule'
CREATE INDEX [IX_FK_UseInstanceModule]
ON [dbo].[UseInstance]
    ([ModuleId]);
GO

-- Creating foreign key on [UseInstanceId] in table 'Fly'
ALTER TABLE [dbo].[Fly]
ADD CONSTRAINT [FK_UseInstanceFly]
    FOREIGN KEY ([UseInstanceId])
    REFERENCES [dbo].[UseInstance]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UseInstanceFly'
CREATE INDEX [IX_FK_UseInstanceFly]
ON [dbo].[Fly]
    ([UseInstanceId]);
GO

-- Creating foreign key on [Father_Id] in table 'Trait'
ALTER TABLE [dbo].[Trait]
ADD CONSTRAINT [FK_TraitTrait]
    FOREIGN KEY ([Father_Id])
    REFERENCES [dbo].[Trait]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TraitTrait'
CREATE INDEX [IX_FK_TraitTrait]
ON [dbo].[Trait]
    ([Father_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------