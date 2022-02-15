SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[api_user_add]
(
    @customer NVARCHAR(MAX),
    @email NVARCHAR(MAX),
    @password NVARCHAR(MAX),
    @salt VARBINARY(MAX),
    @country_iso2 CHAR(2),
    @contact_first_name NVARCHAR(MAX),
    @contact_last_name NVARCHAR(MAX),
    @gender INT
)
AS
BEGIN

    IF @email IS NULL OR @password IS NULL OR @salt IS NULL OR @country_iso2 IS NULL
    BEGIN
        THROW 50002, '@email, @password, @salt or @country_iso2 parameters cannot be null', 15;
    END

    DECLARE @country_id INT;
    SET @country_id = ( SELECT TOP 1 country_id FROM country WHERE iso = @country_iso2 );

    IF EXISTS ( SELECT TOP 1 api_user_id FROM [api_user] 
                WHERE 
                email = @email
                )
    BEGIN
        THROW 50003, 'This email address is already taken', 15;
    END
    
    -- insert into api_user table
    INSERT INTO [api_user](email, customer, contact_first_name, contact_last_name, [password], salt, gender, fk_country_id, is_validated) VALUES
    (@email, @customer, @contact_first_name, @contact_last_name, @password, @salt, @gender, @country_id, 1);
    
    DECLARE @api_user_id INT = scope_identity();

    SELECT TOP 1 * FROM [api_user] WHERE api_user_id = @api_user_id;
END
GO
