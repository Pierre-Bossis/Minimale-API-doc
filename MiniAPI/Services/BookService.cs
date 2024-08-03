using Dapper;
using Microsoft.Data.Sqlite;
using MiniAPI.Models;
using System.Runtime.CompilerServices;

namespace MiniAPI.Services
{
    public class BookService
    {
        private readonly string _connectionString;

        public BookService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "SELECT * FROM Books";
            IEnumerable<Book> books = await connection.QueryAsync<Book>(sql);
            return books;
        }

        public async Task<Book> GetOne(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "SELECT * FROM Books WHERE Id = @id";
            return await connection.QuerySingleOrDefaultAsync<Book>(sql, new { id = id });
        }

        public async Task AddBook(string title, string author)
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "INSERT INTO Books(Title,Author) VALUES(@title,@author)";
            await connection.ExecuteScalarAsync(sql, new {title = title, author = author});
        }

        public async Task DeleteBook(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "DELETE FROM Books WHERE Id = @id";
            await connection.ExecuteScalarAsync(sql, new { id = id });
        }

        public async Task<bool> Update(string title, string author, int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "UPDATE Books SET Title = @title, Author = @author WHERE Id = @id";
            int rowsAffected = await connection.ExecuteAsync(sql, new {title = title,author = author, id = id});

            return rowsAffected > 0;
        }
    }
}
