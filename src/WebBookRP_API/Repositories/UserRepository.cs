using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class UserRepository(IDbConnection connection) : IUserRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<User?> GetByEmailAsync(string email)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<User>(
            """
            SELECT Id, Name, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@Email))
            LIMIT 1
            """,
            new { Email = email });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}