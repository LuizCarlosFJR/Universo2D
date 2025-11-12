-- 1. CRIAÇÃO DO BANCO DE DADOS (SCHEMA)
CREATE DATABASE IF NOT EXISTS universo_db;

-- 2. DEFINIR O BANCO DE DADOS COMO ATIVO
USE universo_db;

-- 3. CRIAÇÃO DA TABELA DE METADADOS DA SIMULAÇÃO
CREATE TABLE IF NOT EXISTS Simulacao (
    IdSimulacao INT NOT NULL AUTO_INCREMENT,
    NomeSimulacao VARCHAR(150) NOT NULL,
    DataGravacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    TotalCorpos INT NOT NULL,
    NumInterac INT NOT NULL,
    NumTempoInterac INT NOT NULL,
    PRIMARY KEY (IdSimulacao)
);

-- 4. CRIAÇÃO DA TABELA DE ESTADO DOS CORPOS (MODELO 2D)
CREATE TABLE IF NOT EXISTS Corpo (
    IdCorpo INT NOT NULL AUTO_INCREMENT,
    IdSimulacao INT NOT NULL,
    Nome VARCHAR(100),
    Massa DOUBLE NOT NULL,
    Densidade DOUBLE NOT NULL,
    PosX DOUBLE NOT NULL,
    PosY DOUBLE NOT NULL,
    VelX DOUBLE NOT NULL,
    VelY DOUBLE NOT NULL,
    PRIMARY KEY (IdCorpo),
    FOREIGN KEY (IdSimulacao) REFERENCES Simulacao(IdSimulacao)
);

---- Area pra Testes ---

USE universo_db;
SELECT * FROM Simulacao WHERE IdSimulacao = 22;

USE universo_db;
SELECT * FROM Simulacao;

SELECT * FROM Simulacao
WHERE Nomesimulacao = 666;


SELECT COUNT(*) FROM Corpo WHERE IdSimulacao = 03;

SELECT PosX, PosY, VelX, VelY, Massa, Densidade 
FROM Corpo 
WHERE IdSimulacao = 22 
LIMIT 10;

SELECT IdSimulacao, NomeSimulacao, DataGravacao, TotalCorpos
FROM Simulacao
ORDER BY IdSimulacao DESC
LIMIT 5;

-- Mostra os dados principais da simulação, incluindo a contagem de interações.
SELECT 
    IdSimulacao, 
    NomeSimulacao, 
    DataGravacao, 
    TotalCorpos, 
    NumInterac, 
    NumTempoInterac
FROM Simulacao
WHERE IdSimulacao = 3;



