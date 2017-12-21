
SELECT SUBSTRING(Email, AtIndex + 1, LEN(Email) - AtIndex) AS Provider, COUNT(*) AS [Number Of Users] FROM
(SELECT Email, CHARINDEX('@', Email ,1) AtIndex FROM Users) AS u
GROUP BY SUBSTRING(Email, AtIndex + 1, LEN(Email) - AtIndex)
ORDER BY [Number Of Users] DESC, Provider 


SELECT G.Name AS Game, GT.Name AS [Game Type], U.Username AS Username, UG.Level AS Level, UG.Cash, C.Name AS Character FROM Games AS G
JOIN UsersGames AS UG ON UG.GameId = G.Id
JOIN GameTypes AS GT ON GT.Id = G.GameTypeId
JOIN Users AS U ON U.Id = UG.UserId
JOIN Characters AS C ON C.Id = UG.CharacterId
ORDER BY Level DESC, Username, Game


SELECT U.Username, G.Name AS Game, COUNT(I.Name) [Items Count], SUM(I.Price) AS [Items Price] FROM UserGameItems AS UGT
JOIN UsersGames AS UG ON UG.Id = UGT.UserGameId
JOIN Games AS G ON G.Id = UG.GameId
JOIN Users AS U ON U.Id = UG.UserId
JOIN Items AS I ON I.Id = UGT.ItemId
GROUP BY U.Username, G.Name
HAVING COUNT(I.Name) >= 10
ORDER BY [Items Count] DESC, [Items Price] DESC, u.Username


SELECT u.Username, 
	   g.Name AS Game, 
	   MAX(c.Name) AS Character,
	   SUM(si.Strength) + MAX(sgt.Strength) + MAX(sc.Strength) AS Strength,
	   SUM(si.Defence) + MAX(sgt.Defence) + MAX(sc.Defence) AS Defence,
	   SUM(si.Speed) + MAX(sgt.Speed) + MAX(sc.Speed) AS Speed,
	   SUM(si.Mind) + MAX(sgt.Mind) + MAX(sc.Mind) AS Mind,
	   SUM(si.Luck) + MAX(sgt.Luck) + MAX(sc.Luck) AS Luck
FROM UsersGames AS ug
JOIN Users AS u ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
JOIN Characters AS c ON c.Id = ug.CharacterId
JOIN UserGameItems AS ugt ON ugt.UserGameId = ug.Id
JOIN Items AS i ON i.Id = ugt.ItemId
JOIN GameTypes AS gt ON gt.Id = g.GameTypeId
JOIN [Statistics] AS sgt ON sgt.Id = gt.BonusStatsId
JOIN [Statistics] AS sc ON sc.Id = c.StatisticId
JOIN [Statistics] AS si ON si.Id = i.StatisticId
GROUP BY u.Username, g.Name
ORDER BY Strength DESC, Defence DESC, Speed DESC, Mind DESC, Luck DESC


SELECT i.Name, i.Price, i.MinLevel, s.Strength, s.Defence, s.Speed, s.Luck, s.Mind FROM Items AS i
JOIN [Statistics] AS s ON s.Id = i.StatisticId
WHERE s.Speed > (SELECT AVG(Speed) FROM [Statistics]) 
AND s.Luck > (SELECT AVG(Luck) FROM [Statistics]) 
AND s.Mind > (SELECT AVG(Mind) FROM [Statistics]) 
ORDER BY i.Name


SELECT i.Name AS Item, i.Price, i.MinLevel, gt.Name AS [Forbidden Game Type] FROM GameTypeForbiddenItems AS gtfi
JOIN GameTypes AS gt ON gt.Id = gtfi.GameTypeId
RIGHT JOIN Items AS i ON i.Id = gtfi.ItemId
ORDER BY gt.Name DESC, i.Name



DECLARE @AlexMoney MONEY =
(SELECT ug.Cash FROM UsersGames AS ug
JOIN Users AS u ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
WHERE u.Username = 'Alex' AND g.Name = 'Edinburgh' )

DECLARE @ItemsTotalPrice MONEY =
(SELECT SUM(Price) FROM Items
WHERE NAME IN ('Blackguard', 'Bottomless Potion of Amplification', 'Eye of Etlich (Diablo III)', 'Gem of Efficacious Toxin',
 'Golden Gorget of Leoric', 'Hellfire Amulet'))

DECLARE @AlexUserGameID INT = 
(SELECT DISTINCT UserGameId FROM UserGameItems
WHERE UserGameId = 
(SELECT ug.Id FROM UsersGames AS ug
JOIN Users AS u ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
WHERE u.Username = 'Alex' AND g.Name = 'Edinburgh')) 

INSERT INTO UserGameItems
SELECT i.Id, @AlexUserGameID FROM Items AS i
WHERE i.Id IN (SELECT Id FROM Items
WHERE NAME IN ('Blackguard', 'Bottomless Potion of Amplification', 'Eye of Etlich (Diablo III)', 'Gem of Efficacious Toxin',
 'Golden Gorget of Leoric', 'Hellfire Amulet'))

 UPDATE UsersGames
 SET Cash-= @ItemsTotalPrice 
 WHERE Id = @AlexUserGameID

SELECT u.Username, g.Name, ug.Cash, i.Name FROM UsersGames AS ug
JOIN Users AS u ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
JOIN UserGameItems AS ugt ON ugt.UserGameId = ug.Id
JOIN Items AS i ON i.Id = ugt.ItemId
WHERE g.Name = 'Edinburgh'
ORDER BY i.Name


SELECT p.PeakName, m.MountainRange AS Mountain, p.Elevation FROM Peaks AS p
JOIN Mountains AS m ON m.Id = p.MountainId
ORDER BY p.Elevation DESC, p.PeakName


SELECT p.PeakName, m.MountainRange AS Mountain, c.CountryName, co.ContinentName FROM Peaks AS p
JOIN Mountains AS m ON m.Id = p.MountainId
JOIN MountainsCountries AS mc ON mc.MountainId = m.Id
JOIN Countries AS c ON mc.CountryCode = c.CountryCode
JOIN Continents AS co ON co.ContinentCode = c.ContinentCode
ORDER BY p.PeakName, c.CountryName


SELECT c.CountryName, co.ContinentName, COUNT(r.Id) AS RiversCount, ISNULL(SUM(r.Length),0) AS TotalLength FROM Countries AS c
LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
JOIN Continents AS  co ON co.ContinentCode = c.ContinentCode
GROUP BY c.CountryName, co.ContinentName
ORDER BY RiversCount DESC, TotalLength DESC, c.CountryName


SELECT cu.CurrencyCode, cu.Description AS Currency, COUNT(c.CountryCode) AS NumberOfCountries FROM Countries AS c
RIGHT JOIN Currencies AS cu ON cu.CurrencyCode = c.CurrencyCode
GROUP BY cu.CurrencyCode, cu.Description
ORDER BY NumberOfCountries DESC, Currency


SELECT co.ContinentName, SUM(CAST(c.AreaInSqKm AS BIGINT)) AS CountriesArea, SUM(CAST(c.Population AS BIGINT)) AS CountriesPopulation  FROM Continents AS co
JOIN Countries AS c ON c.ContinentCode = co.ContinentCode
GROUP BY co.ContinentName
ORDER BY CountriesPopulation DESC


CREATE TABLE Monasteries
(
	Id INT PRIMARY KEY IDENTITY,
	Name VARCHAR(50),
	CountryCode CHAR(2) FOREIGN KEY REFERENCES Countries(CountryCode)
)

INSERT INTO Monasteries(Name, CountryCode) VALUES
('Rila Monastery “St. Ivan of Rila”', 'BG'), 
('Bachkovo Monastery “Virgin Mary”', 'BG'),
('Troyan Monastery “Holy Mother''s Assumption”', 'BG'),
('Kopan Monastery', 'NP'),
('Thrangu Tashi Yangtse Monastery', 'NP'),
('Shechen Tennyi Dargyeling Monastery', 'NP'),
('Benchen Monastery', 'NP'),
('Southern Shaolin Monastery', 'CN'),
('Dabei Monastery', 'CN'),
('Wa Sau Toi', 'CN'),
('Lhunshigyia Monastery', 'CN'),
('Rakya Monastery', 'CN'),
('Monasteries of Meteora', 'GR'),
('The Holy Monastery of Stavronikita', 'GR'),
('Taung Kalat Monastery', 'MM'),
('Pa-Auk Forest Monastery', 'MM'),
('Taktsang Palphug Monastery', 'BT'),
('Sümela Monastery', 'TR')

ALTER TABLE Countries
ADD IsDeleted BIT NOT NULL Default(0)

UPDATE Countries
SET IsDeleted = 1
WHERE CountryName IN (
SELECT c.CountryName FROM Countries AS c
JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
GROUP BY c.CountryName, c.IsDeleted
HAVING COUNT(cr.RiverId) > 3)

SELECT m.Name AS Monastery, c.CountryName AS Country FROM Monasteries AS m
LEFT JOIN Countries AS c ON c.CountryCode = m.CountryCode
WHERE c.IsDeleted = 0
ORDER BY m.Name


UPDATE Countries
SET CountryName = 'Burma'
WHERE CountryName = 'Myanmar'

INSERT INTO Monasteries
VALUES
('Hanga Abbey', (SELECT CountryCode FROM Countries WHERE CountryName = 'Tanzania')),
('Myin-Tin-Daik', (SELECT CountryCode FROM Countries WHERE CountryName = 'Myanmar'))

SELECT co.ContinentName, c.CountryName, COUNT(m.Id) AS MonasteriesCount FROM Continents AS co
JOIN Countries AS c ON c.ContinentCode = co.ContinentCode
LEFT JOIN Monasteries AS m ON m.CountryCode = c.CountryCode
WHERE c.IsDeleted = 0
GROUP BY co.ContinentName, c.CountryName
ORDER BY MonasteriesCount DESC, c.CountryName
