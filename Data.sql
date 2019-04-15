insert into Computer (PurchaseDate, DecommissionDate, Make, Manufacturer) values ('01 Jan 2019', '01 Jan 2024', 'MacBook Pro', 'Apple')
insert into Computer (PurchaseDate, DecommissionDate, Make, Manufacturer) values ('01 Jan 2018', '01 Jan 2023', 'Inspiron', 'Dell')
insert into Computer (PurchaseDate, DecommissionDate, Make, Manufacturer) values ('01 Jan 2017', '01 Jan 2022', 'MacBook Air', 'Apple')

insert into Department ([Name], Budget) values ('Accounting', 400000)
insert into Department ([Name], Budget) values ('IT', 40000)
insert into Department ([Name], Budget) values ('Sales', 450000)

insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Hernando', 'Rivera', 1, 0)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Mary', 'Phillips', 2, 0)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Lorenzo', 'Lopez', 1, 1)

insert into Customer (FirstName, LastName) values ('Fred', 'Flinstone')
insert into Customer (FirstName, LastName) values ('Barney', 'Rubble')
insert into Customer (FirstName, LastName) values ('George', 'Jetson')

insert into PaymentType (AcctNumber, [Name], CustomerId) values (1000, 'Visa', 1)
insert into PaymentType (AcctNumber, [Name], CustomerId) values (2000, 'MasterCard', 2)
insert into PaymentType (AcctNumber, [Name], CustomerId) values (3000, 'AmEx', 3)

insert into [Order] (CustomerId, PaymentTypeId) values (1, 1)
insert into [Order] (CustomerId, PaymentTypeId) values (2, 2)
insert into [Order] (CustomerId, PaymentTypeId) values (3, 3)

insert into ProductType ([Name]) values ('Electronics')
insert into ProductType ([Name]) values ('Sports Equipment')
insert into ProductType ([Name]) values ('Furniture')

insert into Product (ProductTypeId, CustomerId, Title, [Description], Quantity, Price) values (1, 1, 'Television', 'Classic 1970s antique TV', 1, 10)
insert into Product (ProductTypeId, CustomerId, Title, [Description], Quantity, Price) values (2, 2, 'Baseball Bat', 'Wooden', 1, 5)
insert into Product (ProductTypeId, CustomerId, Title, [Description], Quantity, Price) values (3, 3, 'Chair', 'Folding', 1, 3)

insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (1, 1, '01 Jan 2019', NULL)
insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (2, 2, '01 Jan 2018', NULL)
insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (3, 3, '01 Jan 2017', NULL)

insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Count Beans', '14 Feb 2019', '15 Feb 2019', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Spell "IT"', '14 Feb 2019', '15 Feb 2019', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Sell Beans', '14 Feb 2019', '15 Feb 2019', 10)

insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (1, 1)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (2, 2)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (3, 1)

insert into OrderProduct (OrderId, ProductId) values (1, 1)
insert into OrderProduct (OrderId, ProductId) values (2, 2)
insert into OrderProduct (OrderId, ProductId) values (3, 3)
