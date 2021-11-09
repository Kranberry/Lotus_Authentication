CREATE TABLE [user]
(
    [user_id] INT PRIMARY KEY IDENTITY(1, 1),
    [first_name] NVARCHAR(MAX),
    [last_name] NVARCHAR(MAX),
    [username] NVARCHAR(MAX) NOT NULL,
    [email] NVARCHAR(MAX) NOT NULL,
    [password] NVARCHAR(MAX) NOT NULL,
    [salt] VARBINARY NOT NULL,
    [gender] INT NOT NULL,
    [fk_country_id] INT NOT NULL FOREIGN KEY REFERENCES country(country_id),
    [record_insert_date] DATETIME DEFAULT GETDATE(),
    [record_update_date] DATETIME,
    [is_validated] BIT DEFAULT 0
)