
CREATE TABLE Logs(
	LogId INT PRIMARY KEY IDENTITY,
	AccountId INT FOREIGN KEY REFERENCES Accounts(Id),
	OldSum MONEY,
	NewSum MONEY
	)

CREATE TRIGGER T_Accounts_After_UPDATE ON Accounts FOR UPDATE
AS
BEGIN
	INSERT INTO Logs (AccountId, OldSum,NewSum)
	SELECT i.Id, d.Balance, i.Balance FROM inserted AS i
	JOIN deleted AS d ON d.Id = i.Id
END


CREATE Table NotificationEmails(
Id INT PRIMARY KEY IDENTITY,
Recipient INT,
Subject VARCHAR(100),
Body VARCHAR(MAX)
)


CREATE TRIGGER T_Logs_After_Insert ON Logs FOR Insert
AS
BEGIN
	DECLARE @recipient INT = (SELECT AccountId FROM Logs)
	DECLARE @oldBalance MONEY = (SELECT OldSum FROM Logs)
	DECLARE @newBalance MONEY = (SELECT NewSum FROM Logs)

	INSERT INTO NotificationEmails (Recipient, Subject, Body)
	VALUES (@recipient, CONCAT('Balance change for account: ', @recipient), CONCAT('On ',GETDATE(),' your balance was changed from ',@oldBalance,' to ',@newBalance,'.'))
END



CREATE PROC usp_DepositMoney (@accountID INT, @moneyAmount MONEY)
AS
BEGIN
	UPDATE Accounts
	SET Balance += @moneyAmount
	WHERE Id = @accountID 
END


CREATE PROC usp_WithdrawMoney  (@accountId INT, @moneyAmount MONEY)
AS
BEGIN
	UPDATE Accounts
	SET Balance -= @moneyAmount
	WHERE Id = @accountId
END


CREATE PROC usp_TransferMoney (@senderId INT, @receiverId INT, @amount MONEY)
AS
BEGIN
	DECLARE @senderBalance MONEY = 
	(SELECT Balance FROM Accounts
	WHERE Id = @senderId)
	BEGIN TRANSACTION
	IF(@amount < 0)
	BEGIN
		rollback
	END
	ELSE
	BEGIN
		IF(@senderBalance - @amount < 0)
		BEGIN
			rollback
		END
		ELSE
		BEGIN
			EXEC dbo.usp_WithdrawMoney @senderId,@amount
			EXEC dbo.usp_DepositMoney @receiverId, @amount
			COMMIT
		END
	END
END


CREATE PROC usp_AssignProject @emloyeeId INT, @projectID INT
AS
BEGIN
	BEGIN TRAN
	INSERT INTO EmployeesProjects (EmployeeID, ProjectID)
	VALUES (@emloyeeId, @projectID)
	DECLARE @employeeProjectNumber INT = (SELECT COUNT(*) FROM EmployeesProjects WHERE EmployeeID = @emloyeeId)
	IF(@employeeProjectNumber > 3)
	BEGIN
		RAISERROR('The employee has too many projects!',16,1)
		ROLLBACK
	END
	ELSE
	BEGIN
		COMMIT
	END
END

EXEC dbo.ufn_CashInUsersGames 'Love in a mist'

CREATE TABLE Deleted_Employees
(
	EmployeeId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50),
	LastName VARCHAR(50),
	MiddleName VARCHAR(50),
	JobTitle VARCHAR(50),
	DepartmentID INT,
	Salary MONEY
)

CREATE TRIGGER tr_Employees_After_Delete ON Employees FOR DELETE
AS
BEGIN
	INSERT INTO Deleted_Employees (FirstName, LastName, MiddleName, JobTitle, DepartmentID, Salary)
	SELECT d.FirstName, d.LastName, d.MiddleName, d.JobTitle, d.DepartmentID, d.Salary FROM deleted AS d
END