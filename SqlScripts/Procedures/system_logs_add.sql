CREATE PROCEDURE [system_logs_add]
    @application NVARCHAR(MAX),
    @severity NVARCHAR(30),
    @exception_type NVARCHAR(256),
    @message NVARCHAR(MAX),
    @page NVARCHAR(MAX),
    @stacktrace NVARCHAR(MAX)
AS
    IF @application IS NULL OR @severity IS NULL
    BEGIN
        THROW 50001, 'application or severity parameters cannot be null', 15;
    END;

    INSERT INTO system_logs(application, severity, exception_type, [message], page, stacktrace, entered_date) VALUES
    (@application, @severity, @exception_type, @message, @page, @stacktrace, GETDATE());

GO