CREATE TABLE system_logs
(
    [system_logs_id] INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    [application] NVARCHAR(MAX) NOT NULL,
    [severity] NVARCHAR(MAX) NOT NULL,
    [exception_type] NVARCHAR(MAX),
    [message] NVARCHAR(MAX),
    [page] NVARCHAR(MAX),
    [stacktrace] NVARCHAR(MAX),
    [entered_date] DATETIME DEFAULT GETDATE()
)