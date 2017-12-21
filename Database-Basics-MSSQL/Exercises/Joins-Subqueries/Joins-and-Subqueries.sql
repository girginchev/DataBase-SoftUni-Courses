SELECT TOP(5) EmployeeID, JobTitle, e.AddressID, AddressText 
FROM Employees AS e
JOIN Addresses AS a ON e.AddressID = a.AddressID 
ORDER BY e.AddressID

SELECT TOP(50) FirstName, LastName, t.Name AS Town, a.AddressText FROM Employees AS e
JOIN Addresses AS a ON a.AddressID = e.AddressID
JOIN Towns AS t ON t.TownID = a.TownID
ORDER BY FirstName, LastName

SELECT EmployeeID, FirstName, LastName, d.Name AS DepartmentName FROM Employees AS e
JOIN Departments AS d ON d.DepartmentID= e.DepartmentID
WHERE d.Name = 'Sales'
ORDER BY EmployeeID

SELECT TOP(5) EmployeeID, FirstName, Salary, d.Name AS DepartmentName FROM Employees AS e
JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
WHERE Salary >	15000
ORDER BY e.DepartmentID

SELECT TOP(3) e.EmployeeID, FirstName FROM Employees AS e
LEFT JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY EmployeeID

SELECT FirstName, LastName, HireDate, d.Name AS DepartmentName FROM Employees AS e
JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
WHERE HireDate > '01/01/1999' AND d.Name IN ('Sales', 'Finance')
ORDER BY HireDate


SELECT TOP(5) e.EmployeeID, FirstName, p.Name AS ProjectName FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects AS p ON p.ProjectID = ep.ProjectID
WHERE p.StartDate > '2002-08-13' AND p.EndDate IS NULL
ORDER BY EmployeeID


SELECT e.EmployeeID, FirstName, 
CASE 
WHEN p.StartDate > '2005-01-01' THEN NULL
ELSE p.Name
END AS ProjectName
FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects AS p ON p.ProjectID = ep.ProjectID
WHERE e.EmployeeID = 24


SELECT e1.EmployeeID, e1.FirstName, e1.ManagerID, e2.FirstName AS ManagerName 
 FROM Employees AS e1
 JOIN Employees AS e2 ON e2.EmployeeID = e1.ManagerID
 WHERE e1.ManagerID IN (3,7)
ORDER BY e1.EmployeeID


SELECT TOP (50) e1.EmployeeID, CONCAT(e1.FirstName + ' ', e1.LastName) AS EmployeeName, 
CONCAT(e2.FirstName + ' ', e2.LastName) AS ManagerName, d.Name AS DepartmentName 
FROM Employees AS e1
JOIN Employees AS e2 ON e1.ManagerID = e2.EmployeeID
JOIN Departments AS d ON d.DepartmentID = e1.DepartmentID
ORDER BY e1.EmployeeID


SELECT TOP(1) AVG(Salary) AS AverageSalary FROM Employees
GROUP BY DepartmentID
ORDER BY AverageSalary


SELECT mc.CountryCode, m.MountainRange, p.PeakName, p.Elevation FROM Peaks AS p
JOIN Mountains AS m ON m.Id = p.MountainId
JOIN MountainsCountries as mc ON mc.MountainId = m.Id
WHERE mc.CountryCode = 'BG' AND p.Elevation > 2835
ORDER BY p.Elevation DESC


SELECT mc.CountryCode, COUNT(m.MountainRange) FROM MountainsCountries AS mc
JOIN  Mountains AS m ON mc.MountainId = m.Id
WHERE mc.CountryCode IN ('US', 'BG', 'RU')
GROUP BY mc.CountryCode


SELECT TOP(5) c.CountryName, r.RiverName FROM CountriesRivers AS cr
RIGHT JOIN Rivers AS r ON r.Id = cr.RiverId
RIGHT JOIN Countries AS c ON c.CountryCode = cr.CountryCode
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName

--------------
SELECT c.ContinentCode, c.CountryName, c.CurrencyCode FROM Countries c
JOIN Countries AS c2 ON c2
----------------

WITH CCYContUsage_CTE (ContinentCode, CurrencyCode, CurrencyUsage) AS (
  SELECT ContinentCode, CurrencyCode, COUNT(CountryCode) AS CurrencyUsage
  FROM Countries 
  GROUP BY ContinentCode, CurrencyCode
  HAVING COUNT(CountryCode) > 1  
)
SELECT ContMax.ContinentCode, ccy.CurrencyCode, ContMax.CurrencyUsage 
  FROM
  (SELECT ContinentCode, MAX(CurrencyUsage) AS CurrencyUsage
   FROM CCYContUsage_CTE 
   GROUP BY ContinentCode) AS ContMax
JOIN CCYContUsage_CTE AS ccy 
ON (ContMax.ContinentCode = ccy.ContinentCode AND ContMax.CurrencyUsage = ccy.CurrencyUsage)
ORDER BY ContMax.ContinentCode


SELECT COUNT(c.CountryCode) AS CountryCode FROM MountainsCountries AS m
RIGHT JOIN Countries AS c ON m.CountryCode = c.CountryCode
WHERE m.CountryCode IS NULL


SELECT TOP(5) c.CountryName, MAX(p.Elevation) AS HighestPeakElevation,  MAX(r.Length) AS LongestRiverLength FROM Countries AS c
LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode 
LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
LEFT JOIN Peaks AS p ON p.MountainId = mc.MountainId
GROUP BY c.CountryName
ORDER BY HighestPeakElevation DESC, LongestRiverLength DESC, c.CountryName

-------------------
SELECT hp.CountryName, MAX(hp.Elevation),   FROM
(SELECT hp.CountryName, MAX(hp.Elevation) FROM
(SELECT c.CountryName, p.PeakName, p.Elevation, m.MountainRange FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
LEFT JOIN Peaks AS p ON p.MountainId = mc.MountainId
LEFT JOIN Mountains AS m ON m.Id = p.MountainId) AS hp
GROUP BY hp.CountryName)
JOIN MountainsCountries AS mc ON mc.CountryCode = hp.CountryCode
LEFT JOIN Peaks AS p ON p.MountainId = mc.MountainId
LEFT JOIN Mountains AS m ON m.Id = p.MountainId) AS hp
--------------------------------

WITH PeaksMountains_CTE (Country, PeakName, Elevation, Mountain) AS (

  SELECT c.CountryName, p.PeakName, p.Elevation, m.MountainRange
  FROM Countries AS c
  LEFT JOIN MountainsCountries as mc ON c.CountryCode = mc.CountryCode
  LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
  LEFT JOIN Peaks AS p ON p.MountainId = m.Id
)

SELECT TOP 5
  TopElevations.Country AS Country,
  ISNULL(pm.PeakName, '(no highest peak)') AS HighestPeakName,
  ISNULL(TopElevations.HighestElevation, 0) AS HighestPeakElevation,	
  ISNULL(pm.Mountain, '(no mountain)') AS Mountain
FROM 
  (SELECT Country, MAX(Elevation) AS HighestElevation
   FROM PeaksMountains_CTE 
   GROUP BY Country) AS TopElevations
LEFT JOIN PeaksMountains_CTE AS pm 
ON (TopElevations.Country = pm.Country AND TopElevations.HighestElevation = pm.Elevation)
ORDER BY Country, HighestPeakName 