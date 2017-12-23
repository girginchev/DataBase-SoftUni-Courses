SELECT COUNT(*) FROM WizzardDeposits

SELECT Max(MagicWandSize) AS LongestMagicWand FROM WizzardDeposits

SELECT DepositGroup, 
	   Max(MagicWandSize) AS LongestMagicWand 
FROM WizzardDeposits
GROUP BY DepositGroup

SELECT TOP (2) DepositGroup
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY AVG(MagicWandSize)

SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
GROUP BY DepositGroup

SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
WHERE MagicWandCreator = 'Ollivander family'
GROUP BY DepositGroup

SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
WHERE MagicWandCreator = 'Ollivander family'
GROUP BY DepositGroup
HAVING SUM(DepositAmount) < 150000
ORDER BY TotalSum DESC

SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge)
FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
ORDER BY MagicWandCreator, DepositGroup

SELECT * FROM WizzardDeposits

SELECT AgeGroup, COUNT(AgeGroup) AS WizzardCount FROM
(
SELECT 
CASE 
WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
WHEN Age >=61 THEN '[61+]'
END AS AgeGroup
FROM WizzardDeposits
) AS AgeGroup
GROUP BY AgeGroup

SELECT DISTINCT LEFT(FirstName, 1) AS FirstLetter FROM WizzardDeposits
WHERE DepositGroup = 'Troll Chest'
ORDER BY FirstLetter


SELECT DepositGroup, IsDepositExpired, AVG(DepositInterest) AS AverageInterest
  FROM WizzardDeposits 
 WHERE DepositStartDate > '01.01.1985'
GROUP BY DepositGroup, IsDepositExpired
ORDER BY DepositGroup DESC, IsDepositExpired 

SELECT SUM([Difference]) FROM 
(SELECT FirstName AS [Host Wizzard],
	   DepositAmount AS [Host Wizard Deposit],
	   LEAD(FirstName) OVER (ORDER BY Id) AS [Guest Wizard],
	   LEAD(DepositAmount) OVER (ORDER BY Id) AS [Guest Wizard Deposit],
	   DepositAmount - LEAD(DepositAmount) OVER (ORDER BY Id) AS [Difference] 
FROM WizzardDeposits)
AS WizzardDepositsCompare


SELECT DepartmentID, SUM(Salary) AS TotalSalary 
  FROM Employees
GROUP BY DepartmentID
ORDER BY DepartmentID

SELECT DepartmentID, MIN(Salary) AS MinimumSalary 
FROM Employees
WHERE HireDate > '01.01.2000' AND DepartmentID IN (2,5,7)
GROUP BY DepartmentID


SELECT * INTO NewTable FROM Employees
WHERE Salary > 30000

DELETE FROM NewTable
WHERE ManagerID = 42

UPDATE NewTable
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentId, AVG(Salary) AS AverageSalary
FROM NewTable
GROUP BY DepartmentID

SELECT DepartmentID, MAX(Salary) AS MaxSalary 
  FROM Employees
GROUP BY DepartmentID
HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000


SELECT COUNT(Salary) 
  FROM Employees
GROUP BY ManagerID
HAVING ManagerID IS NULL


SELECT SalaryRanking.DepartmentID, SalaryRanking.Salary FROM
(SELECT DepartmentID, 
       MAX(Salary) AS Salary,
	   DENSE_RANK() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS Rank
FROM Employees
GROUP BY DepartmentID, Salary) AS SalaryRanking
WHERE Rank = 3

SELECT TOP(10) FirstName, LastName, DepartmentID 
FROM Employees AS e1
WHERE Salary > 
(SELECT AVG(Salary) FROM Employees AS e2
WHERE e1.DepartmentID = e2.DepartmentID
GROUP BY DepartmentID)



DECLARE @CurrentAmount DECIMAL(8,2)
DECLARE @PreviusAmount DECIMAL(8,2)
DECLARE @TotalSum DECIMAL(8,2) = 0

DECLARE wizzardCursor CURSOR FOR SELECT DepositAmount FROM WizzardDeposits
OPEN wizzardCursor
FETCH NEXT FROM wizzardCursor INTO @CurrentAmount

WHILE @@FETCH_STATUS = 0
BEGIN
	IF(@PreviusAmount IS NOT NULL)
	SET	@TotalSum += @PreviusAmount - @CurrentAmount
	SET @PreviusAmount = @CurrentAmount
	FETCH NEXT FROM wizzardCursor INTO @CurrentAmount
END
CLOSE wizzardCursor
DEALLOCATE wizzardCursor
SELECT @TotalSum