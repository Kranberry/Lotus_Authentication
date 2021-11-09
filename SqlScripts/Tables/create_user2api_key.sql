CREATE TABLE user2api_key
(
    user2api_key_id INT PRIMARY KEY IDENTITY(1, 1),
    fk_user_id INT NOT NULL FOREIGN KEY REFERENCES [user](user_id),
    fk_api_key_id INT NOT NULL FOREIGN KEY REFERENCES [api_key](api_key_id),
    record_insert_date DATETIME NOT NULL DEFAULT GETDATE(),
    record_update_date DATETIME
)