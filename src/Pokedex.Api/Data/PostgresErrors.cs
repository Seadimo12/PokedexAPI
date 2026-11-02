using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Pokedex.Api.Data;

public static class PostgresErrors
{
    public static bool IsUniqueViolation(this DbUpdateException exception) =>
        exception.InnerException is PostgresException postgres
        && postgres.SqlState == PostgresErrorCodes.UniqueViolation;
}
