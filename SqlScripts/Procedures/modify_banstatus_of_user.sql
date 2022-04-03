CREATE PROCEDURE modify_banstatus_of_user
(
    @api_key NVARCHAR(MAX),
    @user_id INT,
    @email NVARCHAR(MAX),
    @ban_status BIT,
    @wide_ban BIT,
    @ban_lift_date DATETIME,
    @reason NVARCHAR(MAX)
)
AS
BEGIN
    if((@api_key IS NULL) OR (@user_id IS NULL AND @email IS NULL))
    BEGIN
        THROW 50002, '(@email AND @user_id), OR @api_key parameters cannot be null', 15;
    END

    IF(@ban_status = 0)
    BEGIN
        SET @ban_lift_date = GETDATE();
    END

    DECLARE @api_key_id INT = (SELECT TOP(1) api_key_id FROM api_key WHERE api_key = @api_key);
    DECLARE @api_key_user_id INT = (SELECT TOP(1) fk_api_user_id FROM api_key2api_user WHERE fk_api_key_id = @api_key_id);
    SET @user_id = (SELECT TOP(1) user_id FROM [user] WHERE user_id = @user_id OR email = @email);

    IF @wide_ban = 1
    BEGIN
        MERGE INTO api_key_user_ban AS banTable
        USING ( SELECT * FROM [api_key2api_user] 
                WHERE fk_api_user_id = @api_key_user_id ) AS apiKeys
        ON 
            ( banTable.fk_user_id = @user_id  AND banTable.fk_api_key_id = apiKeys.fk_api_key_id)
        WHEN MATCHED THEN
            UPDATE SET
            ban_lift_date = COALESCE(@ban_lift_date, banTable.ban_lift_date),
            reason = COALESCE(@reason, reason),
            record_status = @ban_status,
            record_update_date = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (fk_user_id, fk_api_key_id, ban_lift_date, reason, record_status)
            VALUES 
            (@user_id, apiKeys.fk_api_key_id, COALESCE(@ban_lift_date, GETDATE() + 1), @reason, @ban_status);
    END
    ELSE
    BEGIN
        UPDATE api_key_user_ban SET
            ban_lift_date = COALESCE(@ban_lift_date, GETDATE() + 1),
            reason = COALESCE(@reason, reason),
            record_status = @ban_status,
            record_update_date = GETDATE()
        WHERE
            fk_user_id = @user_id
            AND fk_api_key_id = @api_key_id

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO api_key_user_ban 
                (fk_user_id, fk_api_key_id, ban_lift_date, reason, record_status) VALUES
            (@user_id, @api_key_id, COALESCE(@ban_lift_date, GETDATE() + 1), @reason, @ban_status);
        END
    END
END
GO