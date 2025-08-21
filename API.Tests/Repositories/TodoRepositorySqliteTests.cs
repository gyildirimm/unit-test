using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace API.Tests.Repositories;

/// <summary>
/// SQLite bellek içi veritabanı kullanarak yapılan repository testleri.
/// </summary>
public class TodoRepositorySqliteTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    
    public TodoRepositorySqliteTests()
    {
        // Her test için yeni bir SQLite bellek içi bağlantısı açıyoruz
        //:memory: yazıldığında SQLite fiziksel bir dosya oluşturmaz.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        // Veritabanı şemasını oluşturuyoruz
        using var context = new ApplicationDbContext(_contextOptions);
        context.Database.EnsureCreated();
    }
    
    // IDisposable implemantasyonu ile bağlantıyı kapatıyoruz
    public void Dispose()
    {
        _connection.Dispose();
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectTodo()
    {
        // Arrange - Test verilerini ekliyoruz
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            await context.Todos.AddAsync(
                new TodoItem { Id = 1, Title = "Test Todo", IsDone = false }
            );
            await context.SaveChangesAsync();
        }
        
        // Act - Yeni bir context ve repository oluşturup test ediyoruz
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var repository = new TodoRepository(context);
            var result = await repository.GetByIdAsync(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Todo", result.Title);
            Assert.False(result.IsDone);
        }
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddTodoToDatabase()
    {
        // Arrange
        var newTodo = new TodoItem { Title = "New Todo", IsDone = false };
        
        // Act - Ekleme işlemi yapıyoruz
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var repository = new TodoRepository(context);
            var result = await repository.AddAsync(newTodo);
            
            // Assert - Ekleme işlemi sonucunu doğruluyoruz
            Assert.NotEqual(0, result.Id);
            Assert.Equal("New Todo", result.Title);
        }
        
        // Ayrı bir context ile gerçekten eklendiğini kontrol ediyoruz
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var todoInDb = await context.Todos.FindAsync(newTodo.Id);
            Assert.NotNull(todoInDb);
            Assert.Equal("New Todo", todoInDb.Title);
        }
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodo()
    {
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            await context.Todos.AddAsync(
                new TodoItem { Id = 1, Title = "Original Todo", IsDone = false }
            );
            await context.SaveChangesAsync();
        }
        
        TodoItem? result;
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var repository = new TodoRepository(context);
            var updatedTodo = new TodoItem { Id = 1, Title = "Updated Todo", IsDone = true };
            result = await repository.UpdateAsync(updatedTodo);
        }
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Todo", result.Title);
        Assert.True(result.IsDone);
        
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var todoInDb = await context.Todos.FindAsync(1);
            Assert.NotNull(todoInDb);
            Assert.Equal("Updated Todo", todoInDb.Title);
            Assert.True(todoInDb.IsDone);
        }
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveTodoFromDatabase()
    {
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            await context.Todos.AddAsync(
                new TodoItem { Id = 1, Title = "Todo to Delete", IsDone = false }
            );
            await context.SaveChangesAsync();
        }
        
        bool result;
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var repository = new TodoRepository(context);
            result = await repository.DeleteAsync(1);
        }
        
        Assert.True(result);
        
        using (var context = new ApplicationDbContext(_contextOptions))
        {
            var deletedTodo = await context.Todos.FindAsync(1);
            Assert.Null(deletedTodo);
        }
    }
    
    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        using var context = new ApplicationDbContext(_contextOptions);
        var repository = new TodoRepository(context);
        var result = await repository.DeleteAsync(999); // Var olmayan ID
        
        Assert.False(result);
    }
}