CREATE DATABASE EnterpriseSqlCopilotDb;
GO

USE EnterpriseSqlCopilotDb;
GO
CREATE TABLE Branches
(
    BranchId INT IDENTITY(1,1) PRIMARY KEY,
    BranchName NVARCHAR(100) NOT NULL,
    City NVARCHAR(100) NOT NULL
);
GO
INSERT INTO Branches
(
    BranchName,
    City
)
VALUES
('Chennai Main','Chennai'),
('Coimbatore Central','Coimbatore'),
('Madurai City','Madurai'),
('Trichy Branch','Trichy'),
('Salem Branch','Salem');
GO
CREATE TABLE Customers
(
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(200) NOT NULL,
    DateOfBirth DATE,
    CustomerType NVARCHAR(50),
    BranchId INT,
    CreatedDate DATE,

    CONSTRAINT FK_Customers_Branches
    FOREIGN KEY (BranchId)
    REFERENCES Branches(BranchId)
);
GO
INSERT INTO Customers
(
    CustomerName,
    DateOfBirth,
    CustomerType,
    BranchId,
    CreatedDate
)
VALUES
('Manikandan','1985-05-10','Premium',1,'2024-01-15'),
('Rajesh','1988-02-12','Regular',2,'2024-03-10'),
('Kumar','1990-07-25','Premium',3,'2025-01-05'),
('Suresh','1982-11-18','Regular',4,'2025-02-20'),
('Priya','1992-09-08','Premium',5,'2025-03-01'),
('Anitha','1987-01-22','Regular',1,'2025-04-12'),
('Vijay','1984-06-15','Premium',2,'2025-05-18'),
('Deepa','1993-08-10','Regular',3,'2025-06-05'),
('Arun','1986-12-05','Premium',4,'2025-06-10'),
('Karthik','1991-04-30','Regular',5,'2025-06-15');
GO
CREATE TABLE Accounts
(
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    AccountNumber NVARCHAR(30),
    AccountType NVARCHAR(50),
    Balance DECIMAL(18,2),
    OpenedDate DATE,
    Status NVARCHAR(20),

    CONSTRAINT FK_Accounts_Customers
    FOREIGN KEY (CustomerId)
    REFERENCES Customers(CustomerId)
);
GO
INSERT INTO Accounts
(
    CustomerId,
    AccountNumber,
    AccountType,
    Balance,
    OpenedDate,
    Status
)
VALUES
(1,'SB100001','Savings',2500000,'2024-01-15','Active'),
(2,'SB100002','Savings',150000,'2024-03-10','Active'),
(3,'SB100003','Current',5000000,'2025-01-05','Active'),
(4,'SB100004','Savings',250000,'2025-02-20','Active'),
(5,'SB100005','Current',7000000,'2025-03-01','Active'),
(6,'SB100006','Savings',100000,'2025-04-12','Active'),
(7,'SB100007','Current',6000000,'2025-05-18','Active'),
(8,'SB100008','Savings',300000,'2025-06-05','Active'),
(9,'SB100009','Current',8000000,'2025-06-10','Active'),
(10,'SB100010','Savings',200000,'2025-06-15','Active');
GO
CREATE TABLE Loans
(
    LoanId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    LoanType NVARCHAR(50),
    LoanAmount DECIMAL(18,2),
    OutstandingAmount DECIMAL(18,2),
    StartDate DATE,

    CONSTRAINT FK_Loans_Customers
    FOREIGN KEY (CustomerId)
    REFERENCES Customers(CustomerId)
);
GO
INSERT INTO Loans
(
    CustomerId,
    LoanType,
    LoanAmount,
    OutstandingAmount,
    StartDate
)
VALUES
(1,'Home Loan',5000000,3500000,'2024-02-01'),
(3,'Car Loan',1000000,500000,'2025-01-15'),
(5,'Business Loan',7500000,7000000,'2025-03-10'),
(7,'Personal Loan',500000,250000,'2025-05-20'),
(9,'Home Loan',9000000,8500000,'2025-06-12');
GO
