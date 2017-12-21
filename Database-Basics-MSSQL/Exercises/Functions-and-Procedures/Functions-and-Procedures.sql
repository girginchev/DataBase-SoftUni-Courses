CREATE PROC usp_GetEmployeesSalaryAbove35000 
AS
SELECT FirstName, LastName FROM Employees
WHERE Salary > 35000


CREATE PROC usp_GetEmployeesSalaryAboveNumber (@number MONEY)
AS
SELECT FirstName, LastName FROM Employees
WHERE Salary >= @number



CREATE PROCEDURE usp_GetTownsStartingWith (@startingString NVARCHAR(MAX))
AS
SELECT [Name] FROM Towns
WHERE [Name] LIKE CONCAT(@startingString, '%')



CREATE PROC usp_GetEmployeesFromTown (@town NVARCHAR(MAX))
AS
SELECT FirstName, LastName FROM Employees AS e
JOIN Addresses AS a ON a.AddressID = e.AddressID
JOIN Towns AS t ON t.TownID = a.TownID
WHERE t.Name = @town



CREATE FUNCTION ufn_GetSalaryLevel(@salary MONEY)
RETURNS VARCHAR(20)
AS
BEGIN
	DECLARE @salaryLevel VARCHAR(20);
	IF(@salary < 30000)
	BEGIN
		SET @salaryLevel = 'Low'
	END
	ELSE IF (@salary > 50000)
	BEGIN 
		SET @salaryLevel = 'High'
	END
	ELSE 
	BEGIN
		SET @salaryLevel = 'Average'
	END
	RETURN @salaryLevel
END

SELECT FirstName, LastName, Salary, dbo.ufn_GetSalaryLevel(Salary) FROM Employees


CREATE PROC usp_EmployeesBySalaryLevel (@SalaryLevel VARCHAR(20))
AS
SELECT FirstName, LastName FROM Employees
WHERE dbo.ufn_GetSalaryLevel(Salary) = @SalaryLevel

EXEC usp_EmployeesBySalaryLevel 'low'


CREATE FUNCTION ufn_IsWordComprised(@setOfLetters NVARCHAR(MAX), @word NVARCHAR(MAX))
RETURNS BIT
AS
BEGIN 
	DECLARE @Result BIT;
	DECLARE @wordLenght INT = LEN(@word);
	DECLARE @index INT = 1;
	WHILE(@wordLenght > 0)
	BEGIN
		IF(@setOfLetters LIKE CONCAT('%','[', SUBSTRING(@word,@index,1),']', '%'))
			SET @Result = 1
		ELSE
		BEGIN 
			SET @Result = 0
			RETURN @Result;
		END
	SET @wordLenght -= 1;
	SET @index +=1;
	END
RETURN @Result
END



ALTER TABLE Departments
ALTER COLUMN ManagerID INT NULL

DELETE FROM EmployeesProjects
WHERE EmployeeID IN (
					SELECT e.EmployeeID FROM Employees AS e
					JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
					WHERE d.Name IN ('Production', 'Production Control')
					)

UPDATE Departments
SET ManagerID = NULL 
WHERE ManagerID IN (
					SELECT e.EmployeeID FROM Employees AS e
					JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
					WHERE d.Name IN ('Production', 'Production Control')
					)

UPDATE Employees
SET ManagerID = NULL
WHERE ManagerID IN (
					SELECT e.EmployeeID FROM Employees AS e
					JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
					WHERE d.Name IN ('Production', 'Production Control')
					)

DELETE FROM Employees
WHERE EmployeeID IN (
					SELECT e.EmployeeID FROM Employees AS e
					JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
					WHERE d.Name IN ('Production', 'Production Control')
					)

DELETE FROM Departments
WHERE Name IN ('Production', 'Production Control')



CREATE PROC usp_GetHoldersFullName
AS 
SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name] FROM AccountHolders

EXEC usp_GetHoldersFullName


CREATE PROC usp_GetHoldersWithBalanceHigherThan (@CheckBalance MONEY)
AS
BEGIN
WITH CTE_MinBalance (AccountHolderId, TotalBalance) AS
(SELECT AccountHolderId, SUM(Balance) FROM Accounts
GROUP BY AccountHolderId
HAVING SUM(Balance) > @CheckBalance) 

SELECT ah.FirstName AS [First Name], ah.LastName AS [Last Name] FROM CTE_MinBalance AS MinBalance
JOIN AccountHolders AS ah ON ah.Id = MinBalance.AccountHolderId
END


CREATE FUNCTION ufn_CalculateFutureValue (@balance MONEY, @interestRate FLOAT, @years INT)
RETURNS MONEY
AS
BEGIN 
	DECLARE @interest FLOAT;
	WHILE(@years > 0)
	BEGIN
		SET @interest = @balance * @interestRate
		SET @balance += @interest
		SET @years -=1
	END
	RETURN @balance;
END

SELECT dbo.ufn_CalculateFutureValue(1000, 0.1, 5)



CREATE PROC usp_CalculateFutureValueForAccount (@AccountId INT, @InterestRate FLOAT)
AS
BEGIN
	SELECT a.Id AS [Account Id], 
		   ah.FirstName AS [First Name], 
		   ah.LastName AS [Last Name], 
		   a.Balance AS [Current Balance],
		   dbo.ufn_CalculateFutureValue (a.Balance,@InterestRate, 5) AS [Balance in 5 years]
	  FROM Accounts AS a
	JOIN AccountHolders AS ah ON ah.Id = a.AccountHolderId
	WHERE a.Id = @AccountId
END

EXEC dbo.usp_CalculateFutureValueForAccount 1,0.1


CREATE FUNCTION ufn_CashInUsersGames (@GameName NVARCHAR(MAX))
RETURNS @resultTable Table(SumCash MONEY)
AS
BEGIN

INSERT INTO @resultTable
SELECT SUM(UserCash.Cash) FROM (
SELECT ug.Cash, ROW_NUMBER() OVER (ORDER BY ug.Cash DESC) AS rowNum FROM UsersGames AS ug
JOIN Games AS g ON g.Id = ug.GameId
WHERE g.Name = @GameName) AS UserCash
WHERE UserCash.rowNum % 2 != 0
RETURN 

END

