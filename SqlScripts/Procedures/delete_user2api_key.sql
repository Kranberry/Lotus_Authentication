CREATE PROCEDURE [delete_user2api_key]
(
    @user_id INT,
    @api_key_id INT
)
AS
BEGIN

    IF @user_id IS NULL OR @api_key_id IS NULL
    BEGIN
        THROW 50002, '@user_id or @api_key_id cannot be left null!', 15;
    END

    DELETE FROM [user2api_key] WHERE
    fk_user_id = @user_id
    AND fk_api_key_id = @api_key_id;

END
GO