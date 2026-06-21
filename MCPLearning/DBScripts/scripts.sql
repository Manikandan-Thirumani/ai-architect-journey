CREATE DATABASE MCPBankingDb;
GO

USE MCPBankingDb;
GO

CREATE TABLE Customers
(
    CustomerId INT PRIMARY KEY IDENTITY,

    CustomerName NVARCHAR(100),

    CustomerType NVARCHAR(50),

    InsuranceAmount DECIMAL(18,2),

    LoanLimit DECIMAL(18,2)
);
GO

INSERT INTO Customers
(
    CustomerName,
    CustomerType,
    InsuranceAmount,
    LoanLimit
)
VALUES
('Manikandan','Premium',2500000,5000000),
('Ravi','Regular',500000,1000000),
('Priya','Premium',2500000,5000000);
GO