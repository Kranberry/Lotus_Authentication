CREATE TABLE api_key_user_ban
(
    api_key_user_ban_id INT IDENTITY(1, 1) PRIMARY KEY,
    fk_api_key_id INT FOREIGN KEY REFERENCES [api_key](api_key_id),
    fk_user_id INT FOREIGN KEY REFERENCES [user](user_id),
    ban_lift_date DATETIME NOT NULL DEFAULT GETDATE() + 1,
    reason NVARCHAR(MAX) NULL,
    record_status INT NOT NULL DEFAULT 0,
    record_insert_date DATETIME NOT NULL DEFAULT GETDATE(),
    record_update_date DATETIME NULL
)