CREATE TABLE [dbo].[IPDetails]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [City] NVARCHAR(100) NULL, 
    [Country] NVARCHAR(100) NULL, 
    [Continent] NVARCHAR(100) NULL, 
    [Latitude] DECIMAL(12, 9) NULL, 
    [Longitude] DECIMAL(12, 9) NULL
)
