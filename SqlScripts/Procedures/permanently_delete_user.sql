SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[permanently_delete_user]
(
    @user_id INT
)
AS
BEGIN

    IF @user_id IS NULL
    BEGIN
        THROW 50002, '@user_id cannot be left null!', 15;
    END

    DELETE FROM [user2api_key] WHERE
    fk_user_id = @user_id;

    DELETE FROM [api_key_user_ban] WHERE
    fk_user_id = @user_id;

    DELETE FROM [user] WHERE [user_id] = @user_id;

END
GO
