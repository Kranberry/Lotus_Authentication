CREATE TABLE system_logs
(
    [system_logs_id] INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    [severity] VARCHAR(MAX) NOT NULL,
    [exception_type] VARCHAR(MAX),
    [message] VARCHAR(MAX),
    [page] VARCHAR(MAX),
    [stacktrace] VARCHAR(MAX),
    [entered_date] TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW()::TIMESTAMP
)