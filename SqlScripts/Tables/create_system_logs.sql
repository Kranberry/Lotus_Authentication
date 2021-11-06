CREATE TABLE system_logs
(
    "system_logs_id" SERIAL PRIMARY KEY NOT NULL,
    "application" TEXT NOT NULL,
    "severity" TEXT NOT NULL,
    "exception_type" TEXT,
    "message" TEXT,
    "page" TEXT,
    "stacktrace" TEXT,
    "entered_date" TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW()::TIMESTAMP
)