
CREATE DATABASE OBDCdb;
GO

USE OBDCdb;
GO

CREATE TABLE PlayerAccounts (
    AccountId UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL
);

CREATE TABLE CasinoWagers (
    WagerId UNIQUEIDENTIFIER PRIMARY KEY,
    Theme NVARCHAR(100),
    Provider NVARCHAR(100),
    GameName NVARCHAR(100),
    TransactionId UNIQUEIDENTIFIER,
    BrandId UNIQUEIDENTIFIER,
    AccountId UNIQUEIDENTIFIER,
    Username NVARCHAR(100),
    ExternalReferenceId UNIQUEIDENTIFIER,
    TransactionTypeId UNIQUEIDENTIFIER,
    Amount DECIMAL(18, 2),
    CreatedDateTime DATETIME2,
    NumberOfBets INT,
    CountryCode NVARCHAR(2),
    SessionData NVARCHAR(MAX),
    Duration INT
);

-- idx on AccountId for faster lookups
CREATE INDEX IX_CasinoWager_AccountId ON CasinoWagers(AccountId);

-- proc for getting player casino wagers
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetPlayerCasinoWagers')
BEGIN
    DROP PROCEDURE GetPlayerCasinoWagers;
END;
GO
CREATE PROCEDURE GetPlayerCasinoWagers
    @PlayerId UNIQUEIDENTIFIER,
    @Page INT,
    @PageSize INT
AS
BEGIN
    SELECT WagerId, GameName, Provider, Amount, CreatedDateTime
    FROM CasinoWagers
    WHERE AccountId = @PlayerId
    ORDER BY CreatedDateTime DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM CasinoWagers
    WHERE AccountId = @PlayerId;
END;

--proc for getting top spenders
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetTopSpenders')
BEGIN
    DROP PROCEDURE GetTopSpenders;
END;
GO
CREATE PROCEDURE GetTopSpenders
    @Count INT
AS
BEGIN
    SELECT TOP (@Count) 
        pa.AccountId,
        pa.Username,
        SUM(cw.Amount) AS TotalAmountSpent
    FROM PlayerAccounts pa
    INNER JOIN CasinoWagers cw ON pa.AccountId = cw.AccountId
    GROUP BY pa.AccountId, pa.Username
    ORDER BY TotalAmountSpent DESC;
END;