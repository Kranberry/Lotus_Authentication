CREATE PROCEDURE user_add
(
    @username NVARCHAR(MAX),
    @email NVARCHAR(MAX),
    @password NVARCHAR(MAX),
    @salt VARBINARY(MAX),
    @country_iso2 CHAR(2),
    @first_name NVARCHAR(MAX),
    @last_name NVARCHAR(MAX),
    @gender INT,
    @api_key NVARCHAR(41)
)
AS
BEGIN

    IF @username IS NULL OR @email IS NULL OR @password IS NULL OR @salt IS NULL OR @country_iso2 IS NULL
    BEGIN
        THROW 50002, '@username, @email, @password, @salt and @country_iso2 parameters cannot be null', 15;
    END

    DECLARE @country_id INT;
    SET @country_id = ( SELECT TOP 1 country_id FROM country WHERE iso = @country_iso2 );

    IF EXISTS ( SELECT TOP 1 user_id FROM [user] 
                WHERE 
                email = @email 
                OR username = @username
                )
    BEGIN
        SELECT NULL;
    END
    
    -- insert into user table
    INSERT INTO [user](first_name, last_name, username, email, [password], salt, gender, fk_country_id, is_validated) VALUES
    (@first_name, @last_name, @username, @email, @password, @salt, @gender, @country_id, 1);
    
    DECLARE @user_id INT = scope_identity();
    -- insert into user2api_key table if api_key is not null
    IF @api_key IS NOT NULL
    BEGIN
        DECLARE @api_key_id INT = ( SELECT TOP 1 api_key_id FROM api_key WHERE api_key = @api_key );
        INSERT INTO user2api_key(fk_user_id, fk_api_key_id) VALUES
        (@user_id, @api_key_id);
    END

    SELECT TOP 1 * FROM [user] WHERE user_id = @user_id;
END
GO