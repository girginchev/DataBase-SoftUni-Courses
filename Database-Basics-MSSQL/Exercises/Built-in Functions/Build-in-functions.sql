SELECT FirstName, LastName 
  FROM Employees
 WHERE FirstName LIKE 'SA%'

SELECT FirstName, LastName
  FROM Employees
 WHERE LastName Like '%EI%'

SELECT FirstName
  FROM Employees
 WHERE DepartmentID IN (3,10) AND
 DATEPART(year, HireDate) BETWEEN 1995 AND 2006

 SELECT FirstName, LastName FROM Employees
 WHERE JobTitle NOT LIKE '%Engineer%'

 SELECT Name FROM Towns
 WHERE LEN(Name) BETWEEN 5 AND 6
 ORDER BY Name


 SELECT * FROM Towns
 WHERE Name LIKE '[M,K,B,E]%'
 ORDER BY Name

 SELECT * FROM Towns
 WHERE Name LIKE '[^R,B,D]%'
 ORDER BY Name

CREATE VIEW V_EmployeesHiredAfter2000 AS
SELECT FirstName, LastName FROM Employees
WHERE DATEPART(year,HireDate) > 2000
SELECT * FROM V_EmployeesHiredAfter2000

SELECT FirstName, LastName FROM Employees
WHERE LEN(LastName) = 5

SELECT CountryName, IsoCode FROM [dbo].[Countries]
WHERE CountryName LIKE '%A%A%A%'
ORDER BY IsoCode

SELECT PeakName, RiverName, LOWER(CONCAT(LEFT(PeakName,LEN(PeakName) - 1), RiverName)) AS Mix
FROM Peaks , Rivers
WHERE RIGHT(PeakName,1) = LEFT(RiverName,1)
ORDER BY Mix

   SELECT TOP (50) [Name], FORMAT([Start], 'yyyy-MM-dd' ) AS [Start]
    FROM Games
   WHERE DATEPART(YEAR,Start) IN (2011, 2012)
ORDER BY [Start]

SELECT UserName AS Username, RIGHT(Email, LEN(Email) - CHARINDEX('@', Email))  AS [Email Provider] 
FROM Users
ORDER BY [Email Provider], Username

SELECT Username, IpAddress AS [IP Address] FROM Users
WHERE IpAddress LIKE '___.1%.%.___'
ORDER BY Username

SELECT Name AS Game, 
CASE 
	 WHEN DATEPART(HOUR,Start) >= 0 AND DATEPART(HOUR,Start) <= 11 THEN 'Morning'
	 WHEN DATEPART(HOUR,Start) >= 12 AND DATEPART(HOUR,Start)<= 17 THEN 'Afternoon'
	 ELSE'Evening'
	 END AS [Part if the Day],
CASE
	WHEN Duration <= 3 THEN 'Extra Short'
	WHEN Duration >=4 AND Duration <= 6 THEN 'Short'
	WHEN Duration >6 THEN 'Long'
	ELSE 'Extra Long'
	END AS [Duration]
FROM Games
ORDER BY Game, [Duration], [Part if the Day]

SELECT ProductName, OrderDate, 
	   DATEADD(DAY,3,OrderDate) AS [Pay Due], 
	   DATEADD(MONTH,1,OrderDate) AS [Deliver Due]
FROM Orders

CREATE TABLE People
(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50),
	Birthdate DATETIME
)

INSERT INTO People
VALUES ('Victor', '2000-12-07 00:00:00.000'),
       ('Steven',	'1992-09-10 00:00:00.000'),
('Stephen',	'1910-09-19 00:00:00.000'),
('John',	'2010-01-06 00:00:00.000')

SELECT Name,
	   DATEDIFF(YEAR, Birthdate, GETDATE()) AS [Age in Years],
	   DATEDIFF(MONTH, Birthdate, GETDATE()) AS [Age in Months],
	   DATEDIFF(DAY,Birthdate,GETDATE()) AS [Age in Days],
	   DATEDIFF(MINUTE, Birthdate,GETDATE()) AS [Age in Minutes]
 FROM People