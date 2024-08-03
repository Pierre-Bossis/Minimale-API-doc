using Microsoft.Data.Sqlite;
using MiniAPI.Models;
using MiniAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "Data Source=books.db";
builder.Services.AddSingleton(new BookService(connectionString));
builder.Services.AddCors(o => o.AddPolicy("aspnetcore", options =>
{
    options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Créer et initialiser la base de données SQLite si elle n'existe pas
EnsureDatabaseExists(connectionString);

//si je veux modifier le chemin de l'accès a mon api
//var apiGroup = app.MapGroup("/api"); et mettre apiGroup.MapGet() en bas au lieu de app.MapGet()

app.MapGet("/book", async (BookService bookService) =>
{
    var books = await bookService.GetBooks();
    return Results.Ok(books);
});

app.MapGet("/book/{id:int}", async(BookService bookService, int id) =>
{
    Book book = await bookService.GetOne(id);
    if (book is null) return Results.NotFound("Book doesn't exist.");
    return Results.Ok(book);
});

app.MapPost("/book", async(BookService bookService,BookForm form) =>
{
    await bookService.AddBook(form.Title,form.Author);
    return Results.NoContent();
});

app.MapPut("/book/{id:int}", async(BookService bookService, BookForm form, int id) =>
{
    bool success = await bookService.Update(form.Title,form.Author,id);
    if(success) return Results.NoContent();
    return Results.BadRequest("Problème lors de la modification");
});

app.MapDelete("/book/delete/{id:int}", async(BookService bookService, int id) =>
{
    try
    {
        await bookService.DeleteBook(id);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
    return Results.NoContent();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseCors("aspnetcore");

app.Run();

void EnsureDatabaseExists(string connectionString)
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    var command = connection.CreateCommand();
    command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Books (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Author TEXT NOT NULL
        );
    ";
    command.ExecuteNonQuery();
}