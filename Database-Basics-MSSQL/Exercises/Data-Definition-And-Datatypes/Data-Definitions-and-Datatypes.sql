CREATE DATABASE Minions

USE Minions

CREATE TABLE Minions (
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL,
	Age INT NOT NULL,
)

CREATE TABLE Towns (
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL
)

ALTER TABLE Minions
ALTER COLUMN TownId INT

ALTER TABLE Minions
ADD CONSTRAINT FK_TownId
FOREIGN KEY (TownId) REFERENCES Towns(Id)

USE Minions

INSERT INTO Towns (Id, Name)
VALUES (1,'Sofia'), (2, 'Plovdiv'), (3, 'Varna')

INSERT INTO Minions (Id, Name, Age, TownId)
VALUES (1, 'Kevin', 22, 1), (2, 'Bob', 15, 3), (3, 'Steward', Null, 2)

TRUNCATE TABLE Minions

DROP TABLE Minions
DROP TABLE Towns

DROP TABLE People

CREATE TABLE People (
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(200) NOT NULL,
	Picture VARBINARY(MAX),
	Height DECIMAL(15,2),
	Weight DECIMAL(15,2),
	Gender CHAR(1) NOT NULL,
	BirthDate DATE NOT NULL,
	Biography NVARCHAR(MAX)
)

INSERT INTO People ( Name, Picture, Height, Weight, Gender, BirthDate, Biography)
VALUES ( 'Ivan', null, 180.50, 220.35, 'M', '1985-10-10', 'Fermer'),
		( 'Ivana', null, 140.50, 153.35, 'F', '1999-10-10', 'Fermerka'),
		( 'Stoqn', null, 43.50, 54.543, 'M', '1985-12-12', 'Psyho'),
		( 'Gosho', null, 2.50, 3.35, 'M', '2000-05-10', 'Producer'),
		( 'Pencho', null, 45.5, 453.53, 'M', '2003-06-11', 'Banker')


CREATE TABLE Users (
	Id INT PRIMARY KEY IDENTITY,
	Username VARCHAR(30) UNIQUE NOT NULL,
	Password VARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY(MAX),
	LastLoginTime DATETIME,
	IsDeleted BIT
)

INSERT INTO Users ( Username, Password, ProfilePicture, IsDeleted)
VALUES ( 'Ivan', '4frysfs', Null, 1),
		( 'Ivana', 'ete5325', Null, 1),
		( 'Stoqn', 'ygfh3255', Null, 1),
		( 'Gosho', 'nu6436ll', Null, 0),
		( 'Pencho', 'nyteyuell', Null, 0)

USE Minions

ALTER TABLE Users
DROP CONSTRAINT [PK__Users__3214EC07ACC8ECD0]

ALTER TABLE Users
ADD CONSTRAINT PK_Users PRIMARY KEY (Id, Username)

ALTER TABLE Users
ADD CONSTRAINT PasswordLenght
CHECK(LEN(Password) > 5)

DROP TABLE Users

ALTER TABLE Users
ADD DEFAULT GETDATE()
FOR LastLoginTime

CREATE DATABASE Movies
USE Movies

CREATE TABLE Directors (
	Id INT PRIMARY KEY IDENTITY,
	DirectorName NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
)
CREATE TABLE Genres (
	Id INT PRIMARY KEY IDENTITY,
	GenreName NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Categories (
	Id INT PRIMARY KEY IDENTITY,
	CategoryName NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Movies (
	Id INT PRIMARY KEY IDENTITY,
	Title NVARCHAR(50) NOT NULL,
	DirectorId INT FOREIGN KEY REFERENCES Directors(Id) NOT NULL,
	CopyRightYear DATE,
	Lenght INT,
	GenreId INT FOREIGN KEY REFERENCES Genres(Id) NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	Rating DECIMAL(5,2) NOT NULL,
	Notes NVARCHAR(MAX)
)

INSERT INTO Directors (DirectorName, Notes)
VALUES ('Ivan', 'koi'), ('Kiro', null),('Penchho', 'ti'),('Gosho', 'toi'),('Stoqn', null)

INSERT INTO Genres (GenreName, Notes)
VALUES ('horor', 'koi'), ('comedy', null),('action', 'ti'),('thrilller', 'toi'),('drama', null)

INSERT INTO Categories (CategoryName, Notes)
VALUES ('bad', 'koi'), ('good', null),('all set', 'ti'),('excelent', 'toi'),('worst', null)

INSERT INTO Movies(Title, DirectorId,CopyRightYear,Lenght,GenreId, CategoryId,Rating, Notes)
VALUES ('Friends', 2, '2015', 120, 4, 2, 9.5, 'nqma'),
		('It', 1, '2017', 122, 2, 2, 8, 'nqma'),
		('The One', 4, '2000', 120, 4, 5, 2, 'me'),
		('Too', 3, '2032', 90, 2, 3, 1, 'nqa'),
		('GOT', 2, '2001', 100, 3, 1, 6.7, 'da')


CREATE DATABASE CarRental

USE CarRental

CREATE TABLE Categories 
(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName NVARCHAR(50) NOT NULL,
	DailyRate DECIMAL(15,2) NOT NULL,
	WeeklyRate DECIMAL(15,2) NOT NULL,
	MonthlyRate DECIMAL(15,2) NOT NULL,
	WeekendRate DECIMAL(15,2) NOT NULL
)

CREATE TABLE Cars 
(
	Id INT PRIMARY KEY IDENTITY,
	PlateNumber CHAR(8) NOT NULL,
	Manufacture NVARCHAR(50) NOT NULL,
	Model NVARCHAR(50) NOT NULL,
	CarYear DATE NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
	Doors INT,
	Picture VARBINARY(MAX),
	Condition NVARCHAR(MAX),
	Available BIT NOT NULL 
)

CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL, 
	LastName NVARCHAR(50) NOT NULL,
	Title NVARCHAR(50),
	Notes NVARCHAR(MAX)
)

CREATE TABLE Customers
(
	Id INT PRIMARY KEY IDENTITY, 
	DriverLicenceNumber NVARCHAR(20) NOT NULL, 
	FullName NVARCHAR(50) NOT NULL, 
	Address NVARCHAR(100) NOT NULL, 
	City NVARCHAR(50) NOT NULL, 
	ZIPCode INT, 
	Notes NVARCHAR(MAX)
)

CREATE TABLE RentalOrders
(
	Id INT PRIMARY KEY IDENTITY, 
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id), 
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id), 
	CarId INT FOREIGN KEY REFERENCES Cars(Id), 
	TankLevel DECIMAL(15,2) NOT NULL, 
	KilometrageStart INT NOT NULL, 
	KilometrageEnd INT NOT NULL, 
	TotalKilometrage INT NOT NULL, 
	StartDate DATE NOT NULL, 
	EndDate DATE NOT NULL, 
	TotalDays INT NOT NULL, 
	RateApplied DECIMAL(15,2) NOT NULL, 
	TaxRate DECIMAL(15,2) NOT NULL,
	OrderStatus BIT NOT NULL, 
	Notes NVARCHAR(MAX)
)

INSERT INTO Categories 
VALUES ('jeep',50,300,5000,40), ('couple',20,100,1000,20), ('Van',30.50,140.40,2000,30)

INSERT INTO Cars
VALUES ('CA2354KA', 'BMW', 'X4','2015',1,5,null,'no',1), ('CA2323SA', 'Opel', 'Corsa','2000',2,2,null,'mmmmmo',1), ('PA2254KA', 'FORD', 'CMAX','2010',3,5,null,'yes',0)

INSERT INTO Customers
VALUES ('ssfs421354', 'Ivan Ivanov', 'Slivo Pole 2', 'Ivanovo', 5000, 'nqma'), ('a4342fs54', 'Pencho Ivanov', 'Slivo Pole 2', 'Sandrovo', 7000, 'nqma'), ('rewrwe', 'Georgi Georgiev', 'Slivo Pole 2', 'Marten', 7000, 'nqma')

INSERT INTO Employees
VALUES ('Stoqn', 'Penchev', 'mehanik','.......'), ('Gencho', 'Tenchev', 'ofis','.......'), ('Sam', 'Sam', 'goriva','.......')

INSERT INTO RentalOrders
VALUES (1,2,3,20,1000,1500,500,'2017-01-01','2017-01-10',15,36,10,1,'...'),
		(2,3,1,10,2000,3500,1500,'2017-01-02','2017-01-11',33,32,11,0,'...'),
		(3,1,2,50,11000,11500,600,'2017-02-01','2017-03-10',55,16,20,1,'...')


CREATE DATABASE Hotel

Use Hotel

CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Title NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Customers
(
	AccountNumber INT PRIMARY KEY IDENTITY, 
	FirstName NVARCHAR(30) NOT NULL, 
	LastName NVARCHAR(30) NOT NULL, 
	PhoneNumber DECIMAL(10,0) NOT NULL, 
	EmergencyName NVARCHAR(50), 
	EmergencyNumber DECIMAL(10,0), 
	Notes NVARCHAR(MAX)
)

CREATE TABLE RoomStatus
(
	RoomStatus BIT,
	Notes NVARCHAR(MAX)
)

CREATE TABLE RoomTypes
(
	RoomType INT PRIMARY KEY IDENTITY,
	Notes NVARCHAR(MAX)
)

CREATE TABLE BedTypes
(
	BedType INT PRIMARY KEY IDENTITY,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Rooms
(
	RoomNumber INT PRIMARY KEY IDENTITY, 
	RoomType INT FOREIGN KEY REFERENCES RoomTypes(RoomType), 
	BedType INT FOREIGN KEY REFERENCES BedTypes(BedType), 
	Rate INT, 
	RoomStatus BIT NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Payments
(
	Id INT PRIMARY KEY IDENTITY, 
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id), 
	PaymentDate DATE NOT NULL, 
	AccountNumber CHAR(15) NOT NULL, 
	FirstDateOccupied DATE, 
	LastDateOccupied DATE, 
	TotalDays INT, 
	AmountCharged DECIMAL(15,2) NOT NULL, 
	TaxRate DECIMAL(15,2) NOT NULL, 
	TaxAmount DECIMAL(15,2), 
	PaymentTotal DECIMAL(15,2), 
	Notes NVARCHAR(MAX)
)

CREATE TABLE Occupancies
(
	Id INT PRIMARY KEY IDENTITY, 
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	DateOccupied DATE NOT NULL, 
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber), 
	RoomNumber INT FOREIGN KEY REFERENCES Rooms(RoomNumber), 
	RateApplied DECIMAL(15,2) NOT NULL, 
	PhoneCharge DECIMAL(10,0), 
	Notes NVARCHAR(MAX)
)

INSERT INTO Employees
VALUES ('Ivan', 'Ivanov', 'Director', 'none'), ('Petar', 'Petrov', 'Vale', 'none'), ('Minka', 'Ivanov', 'Host', 'none')

INSERT INTO Customers
VALUES ('sss', 'sss',  23410,'NameS',23410,'none'),
		('www', 'www', 23410,'NameW',24310,'none'),
		('Eee', 'Eee', 23410,'NameE',23410,'none')

INSERT INTO RoomStatus
VALUES (1,null), (0,null), (1,null)

INSERT INTO RoomType
VALUES (1, null), (2,null), (3, null)

INSERT INTO BedType
VALUES (1, null), (2, null), (3,null)

INSERT INTO Rooms
VALUES (2,2,7,1,null),(3,3,5,0,null), (1,1,6,1,null)

INSERT INTO Payments
VALUES (1,'2017-01-01',10101056,'2017-10-10','2017-11-11',15,200,15,30,230,null),
		(2,'2017-01-02',124101056,'2017-12-12','2017-12-12',12,300,45,60,530,null),
		(3,'2017-04-04',1010024256,'2017-07-07','2017-11-10',11,800,85,770,1230,null)

INSERT INTO Occupancies
VALUES (3, '2017-06-05',2,2,15,26867846,null),
		(2, '2017-07-03',1,3,45,26867846,null),
		(1, '2017-11-02',3,1,55,26867846,null)


CREATE DATABASE SoftUni

USE SoftUni

CREATE TABLE Towns
(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL,
)

CREATE TABLE Addresses
(
	Id INT PRIMARY KEY IDENTITY,
	AddressText NVARCHAR(100) NOT NULL,
	TownId INT FOREIGN KEY REFERENCES Towns(Id),
)

CREATE TABLE Departments
(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(30) NOT NULL
)

CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	MiddleName NVARCHAR(30), 
	LastName NVARCHAR(30) NOT NULL, 
	JobTitle NVARCHAR(50) NOT NULL, 
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id), 
	HireDate DATE DEFAULT GETDATE(), 
	Salary DECIMAL(15,2) CHECK (Salary > 0) NOT NULL, 
	AddressId INT FOREIGN KEY REFERENCES Addresses(Id)
)

BACKUP DATABASE SoftUni
TO DISK = 'C:\DB\softuni-backup.bak'

DROP DATABASE SoftUni

RESTORE DATABASE SoftUni
FROM DISK = 'C:\DB\softuni-backup.bak'

INSERT INTO Towns
VALUES ('Sofia'), ('Plovdiv'), ('Varna'), ('Burgas')

INSERT INTO Departments
VALUES ('Engineering'), ('Sales'), ('Marketing'), ('Software Development'), ('Quality Assurance')

INSERT INTO Addresses
VALUES ('Skobelev', 1), ('Lipnik',2), ('Zdravec',4), ('Alexandrovska',1), ('Petkov',3)


INSERT INTO Employees
VALUES ('Ivan', 'Ivanov', 'Ivanov', '.NET Developer', 4, '02-01-2013', 3500.00, 1),
		('Petar', 'Petrov', 'Petrov',	'Senior Engineer',	1,	'03-02-2004', 4000.00, 3),
		('Maria', 'Petrova', 'Ivanova',	'Intern',	5,	'08-28-2016',	525.25, 4),
		('Georgi', 'Teziev', 'Ivanov',	'CEO', 2,	'12-09-2007',	3000.00, 2),
		('Peter', 'Pan', 'Pan',	'Intern', 3,	'08-28-2016',	599.88, 3)

USE SoftUni

SELECT * FROM Towns
ORDER BY Name
SELECT * FROM Departments
ORDER BY Name
SELECT * FROM Employees
ORDER BY Salary DESC

SELECT (Name) FROM Towns 
ORDER BY Name
SELECT (Name) FROM Departments 
ORDER BY Name
SELECT (FirstName), (LastName), (JobTitle), (Salary) FROM Employees 
ORDER BY Salary DESC

UPDATE Employees
SET Salary = Salary *1.1
SELECT Salary FROM Employees 

USE Hotel

UPDATE Payments
SET TaxRate *=0.97
SELECT TaxRate FROM Payments

TRUNCATE TABLE Occupancies
