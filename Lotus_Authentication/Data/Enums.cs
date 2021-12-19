namespace Lotus_Authentication.Data;

public enum LogSeverity
{
    Informational = 1,
    Warning = 2,
    Error = 3,
    Fatal = 4,
}

public enum UserType
{
    Regular,
    Api
}

public enum Gender
{
    Other,
    Male,
    Female
}

public enum DatabaseException
{
    ParameterIsNull = 50002,
    UserAlreadyExists,
    UserDoesNotExist
}