CREATE PROCEDURE [permanently_delete_user]
(
    @user_id INT
)
AS
BEGIN

    IF @user_id IS NULL
    BEGIN
        THROW 50004, '@user_id cannot be left null!', 15;
    END

    DELETE FROM [user2api_key] WHERE
    fk_user_id = @user_id;

    DELETE FROM [user] WHERE [user_id] = @user_id;

END
GO