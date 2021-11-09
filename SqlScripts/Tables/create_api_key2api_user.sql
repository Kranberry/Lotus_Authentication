CREATE TABLE api_key2api_user
(
    api_key2api_user_id INT PRIMARY KEY IDENTITY(1, 1),
    fk_api_user_id INT NOT NULL FOREIGN KEY REFERENCES api_user(api_user_id),
    fk_api_key_id INT NOT NULL FOREIGN KEY REFERENCES api_key(api_key_id),
    record_insert_date DATETIME NOT NULL DEFAULT GETDATE(),
    record_update_date DATETIME
)