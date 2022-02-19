SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[create_api_key]
(
    @api_user_id INT,
    @api_key NVARCHAR(MAX),
    @alias NVARCHAR(15)
)
AS
BEGIN

    IF @api_user_id IS NULL OR @api_key IS NULL
    BEGIN
        THROW 50002, '@api_user_id or @api_key_id cannot be left null!', 15;
    END

    IF NOT EXISTS ( SELECT TOP 1 * FROM api_user WHERE api_user_id = @api_user_id )
    BEGIN
        THROW 50004, 'This user does not exist', 15;
    END

    IF EXISTS( SELECT TOP 1 * FROM api_key WHERE [api_key] = @api_key )
    BEGIN
        THROW 50005, 'This api key already exists', 15;
    END

    INSERT INTO [api_key](api_key, alias, record_insert_date) VALUES
    (@api_key, @alias, GETDATE());

    DECLARE @apiKeyId INT = ( SELECT TOP 1 api_key_id FROM [api_key] WHERE [api_key] = @api_key );
    INSERT INTO [api_key2api_user](fk_api_key_id, fk_api_user_id, record_insert_date) VALUES
    (@apiKeyId, @api_user_id, GETDATE());

    SELECT TOP 1 * FROM [api_key] WHERE [api_key] = @api_key;
END
GO
