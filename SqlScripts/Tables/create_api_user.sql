CREATE TABLE api_user
(
    [api_user_id] INT PRIMARY KEY IDENTITY(1, 1),
    [email] NVARCHAR(MAX),
    [customer] NVARCHAR(MAX),
    [contact_first_name] NVARCHAR(MAX) NOT NULL,
    [contact_last_name] NVARCHAR(MAX),
    [password] NVARCHAR(MAX) NOT NULL,
    [salt] VARBINARY(MAX) NOT NULL,
    [gender] INT NOT NULL,
    [fk_country_id] INT NOT NULL FOREIGN KEY REFERENCES country(country_id),
    [record_insert_date] DATETIME DEFAULT GETDATE(),
    [record_update_date] DATETIME,
    [is_validated] BIT DEFAULT 0
)