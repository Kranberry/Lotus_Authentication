CREATE TABLE api_key(
    api_key_id INT PRIMARY KEY IDENTITY(1, 1),
    api_key NVARCHAR(41) NOT NULL,
    alias NVARCHAR(MAX) NOT NULL,
    record_status INT NOT NULL DEFAULT 0,
    record_insert_date DATETIME DEFAULT GETDATE(),
    record_update_date DATETIME
)