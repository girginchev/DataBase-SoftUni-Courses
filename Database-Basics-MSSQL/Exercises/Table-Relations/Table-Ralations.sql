CREATE TABLE Persons
(
	PersonID INT IDENTITY NOT NULL,
	FirstName NVARCHAR(30) NOT NULL,
	Salary DECIMAL(15,2) NOT NULL,
	PassportID INT
)

ALTER TABLE Persons
ADD CONSTRAINT PK_persons
PRIMARY KEY (PersonID)

CREATE TABLE Passports
(
	PassportID INT PRIMARY KEY IDENTITY,
	PassportNumber CHAR(8) NOT NULL
)

ALTER TABLE Persons
ADD CONSTRAINT FK_Passports
FOREIGN KEY (PassportID) REFERENCES Passports(PassportID) 


CREATE TABLE Models
(
	ModelID INT PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL,
	ManufacturerID INT NOT NULL
)

CREATE TABLE Manufacturers
(
	ManufacturerID INT PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL,
	EstablishedOn DATE
)

ALTER TABLE Models
ADD CONSTRAINT FK_Manufacturer
FOREIGN KEY (ManufacturerID) REFERENCES Manufacturers(ManufacturerID)

INSERT INTO Manufacturers (ManufacturerID, Name, EstablishedOn)
VALUES 
(1, 'BMW', '07/03/1916'),
(2, 'Tesla', '01/01/2003'),
(3, 'Lada', '01/05/1966')

INSERT INTO Models
VALUES 
(101, 'X1', 1),
(102, 'i6', 1),
(103, 'Model S', 2),
(104, 'Model X', 2),
(105, 'Model 3', 2),
(106, 'Nova', 3)


CREATE TABLE Students
(
	StudentID INT PRIMARY KEY,
	Name NVARCHAR(50)
)
CREATE TABLE Exams
(
	ExamID INT PRIMARY KEY,
	Name NVARCHAR(50)
)
CREATE TABLE StudentsExams
(
	StudentID INT NOT NULL,
	ExamID INT NOT NULL,
	PRIMARY KEY (StudentID, ExamID),
	FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
	FOREIGN KEY (ExamID) REFERENCES Exams(ExamID)
)

INSERT INTO Students (StudentID, Name)
VALUES (1, 'Mila'), (2, 'Toni'), (3, 'Ron')

INSERT INTO Exams
VALUES (101, 'SpringMVC'), (102, 'Neo4j'), (103, 'Oracle 11g')

INSERT INTO StudentsExams VALUES
  (1, 101), 
  (1, 102), 
  (2, 101), 
  (3, 103), 
  (2, 102), 
  (2, 103)

  CREATE TABLE Teachers
  (
	TeacherID INT NOT NULL,
	Name NVARCHAR(50) NOT NULL,
	ManagerID INT NOT NULL,
	PRIMARY KEY (TeacherID),
	FOREIGN KEY (ManagerID) REFERENCES Teachers(TeacherID)
  )


CREATE DATABASE University
CREATE TABLE Majors (
MajorID INT PRIMARY KEY, 
Name NVARCHAR(50) NOT NULL)
CREATE TABLE Students(
StudentID INT PRIMARY KEY,
StudentNumber INT NOT NULL,
StudentName NVARCHAR(50),
MajorID INT FOREIGN KEY REFERENCES Majors(MajorID)) 
CREATE TABLE Payments(
PaymentID INT PRIMARY KEY,
PaymentDate DATE,
PaymentAmount DECIMAL(15,2),
StudentID INT FOREIGN KEY REFERENCES Students(StudentID))
CREATE TABLE Subjects(
SubjectID INT PRIMARY KEY,
SubjectName NVARCHAR(50))
CREATE TABLE Agenda(
StudentID INT FOREIGN KEY REFERENCES Students(StudentID),
SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID),
PRIMARY KEY (StudentID, SubjectID))


CREATE TABLE Cities(
CityID INT PRIMARY KEY,
Name VARCHAR(50) NOT NULL)
CREATE TABLE Customers(
CustomerID INT PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
Birthday DATE NOT NULL,
CityID INT FOREIGN KEY REFERENCES Cities(CityID))
CREATE TABLE Orders(
OrderID INT PRIMARY KEY,
CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID))
CREATE TABLE ItemTypes(
ItemTypeID INT PRIMARY KEY,
Name VARCHAR(50) NOT NULL)
CREATE TABLE Items(
ItemID INT PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
ItemTypeID INT FOREIGN KEY REFERENCES ItemTypes(ItemTypeID))
CREATE TABLE OrderItems(
OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
ItemID INT FOREIGN KEY REFERENCES Items(ItemID)
PRIMARY KEY (OrderID, ItemID))


SELECT m.MountainRange, PeakName, Elevation 
FROM Peaks AS P
JOIN Mountains AS M ON M.Id = p.MountainId
WHERE m.MountainRange = 'Rila'
ORDER BY p.Elevation DESC