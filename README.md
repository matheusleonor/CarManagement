## CarManagement

Este projeto é um sistema de gerenciamento de veículos desenvolvido em ASP.NET Core, destinado a empresas que necessitam gerenciar sua frota de veículos. Ele permite que administradores e usuários criem, visualizem, editem e excluam registros de veículos, bem como gerenciem informações relacionadas a combustível, cores, marcas, modelos e usuários.

## Instruções de Uso
- Entre na branch Master.
- Clone o projeto em sua máquina local.
- Abra o projeto em um ambiente de desenvolvimento compatível (Visual Studio, Visual Studio Code, etc.).
- Configure a conexão com o banco de dados no arquivo appsettings.json.
- Execute as migrações para criar o banco de dados e as tabelas necessárias.
- Execute a aplicação e acesse-a em seu navegador.
- Cadastre novos veículos, edite informações existentes e experimente as funcionalidades oferecidas pela aplicação.

## Funcionalidades
- Cadastro e edição de veículos
- Upload e gestão de múltiplas fotos por veículo
- Associação de veículos a combustíveis, cores, marcas e modelos
- Filtros para visualização da lista de veículos
- Interface amigável e responsiva

## Tecnologias Utilizadas
- ASP.NET Core MVC
- ADO.NET para acesso a dados
- HTML, CSS, JavaScript
- Bootstrap
- Razor

## Banco De Dados

-- DataBase
CREATE DATABASE [CarManagement]

-- Tabela de Combustíveis
CREATE TABLE Combustivel (
    Id INT PRIMARY KEY IDENTITY,
    Descricao NVARCHAR(100) NOT NULL UNIQUE,
    Status BIT NOT NULL
);

-- Tabela de Cores
CREATE TABLE Cor (
    Id INT PRIMARY KEY IDENTITY,
    Descricao NVARCHAR(100) NOT NULL UNIQUE,
    Status BIT NOT NULL
);

-- Tabela de Marcas
CREATE TABLE Marca (
    Id INT PRIMARY KEY IDENTITY,
    Nome NVARCHAR(100) NOT NULL UNIQUE,
    Status BIT NOT NULL
);

-- Tabela de Modelos
CREATE TABLE Modelo (
    Id INT PRIMARY KEY IDENTITY,
    MarcaId INT NOT NULL,
    Nome NVARCHAR(100) NOT NULL UNIQUE,
    Status BIT NOT NULL,
    FOREIGN KEY (MarcaId) REFERENCES Marca(Id)
);

-- Tabela de Veiculos
CREATE TABLE [dbo].[Veiculo] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Placa] NVARCHAR(10) NOT NULL,
    [Renavam] NVARCHAR(11) NOT NULL,
    [NumeroChassi] NVARCHAR(17) NOT NULL,
    [NumeroMotor] NVARCHAR(20) NOT NULL,
    [MarcaId] INT NOT NULL,
    [ModeloId] INT NOT NULL,
    [CombustivelId] INT NOT NULL,
    [CorId] INT NOT NULL,
    [AnoFabricacao] INT NOT NULL,
    [Status] BIT NOT NULL,
    [Fotos] NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Veiculo_Marca FOREIGN KEY (MarcaId) REFERENCES Marca(Id),
    CONSTRAINT FK_Veiculo_Modelo FOREIGN KEY (ModeloId) REFERENCES Modelo(Id),
    CONSTRAINT FK_Veiculo_Combustivel FOREIGN KEY (CombustivelId) REFERENCES Combustivel(Id),
    CONSTRAINT FK_Veiculo_Cor FOREIGN KEY (CorId) REFERENCES Cor(Id)
);

-- Tabela "TipoUsuario"
CREATE TABLE TipoUsuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descricao NVARCHAR(100) NOT NULL,
    Status BIT NOT NULL
);

-- Tabela "Usuarios"
CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL,
    Senha NVARCHAR(255) NOT NULL,
    TipoUsuarioId INT NOT NULL,
    FOREIGN KEY (TipoUsuarioId) REFERENCES TipoUsuario(Id)
);

-- Insert's necessário
INSERT INTO [dbo].[TipoUsuario] (Descricao, Status) VALUES ('Administrador', 1);
INSERT INTO [dbo].[TipoUsuario] (Descricao, Status) VALUES ('Cliente', 1);
