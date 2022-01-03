CREATE PROCEDURE user_update
(
    @user_id INT,
    @username NVARCHAR(MAX),
    @email NVARCHAR(MAX),
    @password NVARCHAR(MAX),
    @salt VARBINARY(MAX),
    @country_iso2 CHAR(2),
    @first_name NVARCHAR(MAX),
    @last_name NVARCHAR(MAX),
    @gender INT
)
AS
BEGIN

    IF @username IS NULL OR @email IS NULL OR @password IS NULL OR @salt IS NULL OR @country_iso2 IS NULL
    BEGIN
        THROW 50002, '@user_id parameter cannot be null', 15;
    END

    IF NOT EXISTS (SELECT TOP 1 * FROM [user]
                   WHERE user_id = @user_id)
    BEGIN
        DECLARE @message NVARCHAR(MAX) = 'User with id ' + @user_id + ' does not exist';
        THROW 50004, @message, 15;
    END

    DECLARE @country_id INT;
    SET @country_id = ( SELECT TOP 1 country_id FROM country WHERE iso = @country_iso2 );

    IF EXISTS ( SELECT TOP 1 user_id FROM [user] 
                WHERE 
                (
                    email = @email 
                    OR username = @username
                )
                AND user_id <> @user_id )
    BEGIN
        THROW 50003, 'This email address or username is already taken', 15;
    END

    UPDATE [user] SET 
        username = COALESCE(@username, username),
        email = COALESCE(@email, email),
        [password] = COALESCE(@password, [password]),
        [salt] = COALESCE(@salt, [salt]),
        fk_country_id = COALESCE(@country_id, fk_country_id),
        first_name = COALESCE(@first_name, first_name),
        last_name = COALESCE(@last_name, last_name),
        gender = COALESCE(@gender, gender)
    WHERE user_id = @user_id
    
    SELECT TOP 1 * FROM [user] WHERE user_id = @user_id;
END
GO