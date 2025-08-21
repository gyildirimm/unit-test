using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace API.Tests.Repositories;

/// <summary>
/// Entity Framework Core InMemory Provider kullanılarak yapılan repository testleri.
/// </summary>
public class TodoRepositoryInMemoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _inMemoryOptions;
    
    public TodoRepositoryInMemoryTests()
    {
        // Her test için benzersiz bir veritabanı adı oluşturuluyor & Bu sayede testler birbirini etkilemiyor
        _inMemoryOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TodoAppTestDb_{Guid.NewGuid()}")
            .Options;
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectTodo()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_inMemoryOptions);
        var repository = new TodoRepository(context);
        
        // Veritabanına test verilerini ekliyoruz
        var todoItem = new TodoItem { Id = 1, Title = "Test Todo", IsDone = false };
        await context.Todos.AddAsync(todoItem);
        await context.SaveChangesAsync();
        
        // Act
        var result = await repository.GetByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Todo", result.Title);
        Assert.False(result.IsDone);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddTodoToDatabase()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_inMemoryOptions);
        var repository = new TodoRepository(context);
        var newTodo = new TodoItem { Title = "New Todo", IsDone = false };
        
        // Act
        var result = await repository.AddAsync(newTodo);
        
        // Assert
        Assert.NotEqual(0, result.Id); // Id otomatik atanmalı
        Assert.Equal("New Todo", result.Title);
        
        // Veritabanında gerçekten eklendiğini kontrol edelim
        var todoInDb = await context.Todos.FindAsync(result.Id);
        Assert.NotNull(todoInDb);
        Assert.Equal("New Todo", todoInDb.Title);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodo()
    {
        await using var context = new ApplicationDbContext(_inMemoryOptions);
        var repository = new TodoRepository(context);
        
        var todo = new TodoItem { Id = 1, Title = "Original Todo", IsDone = false };
        await context.Todos.AddAsync(todo);
        await context.SaveChangesAsync();
        
        var updatedTodo = new TodoItem { Id = 1, Title = "Updated Todo", IsDone = true };
        
        var result = await repository.UpdateAsync(updatedTodo);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Todo", result.Title);
        Assert.True(result.IsDone);
        
        var todoInDb = await context.Todos.FindAsync(1);
        Assert.NotNull(todoInDb);
        Assert.Equal("Updated Todo", todoInDb.Title);
        Assert.True(todoInDb.IsDone);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveTodoFromDatabase()
    {
        await using var context = new ApplicationDbContext(_inMemoryOptions);
        var repository = new TodoRepository(context);
        
        var todo = new TodoItem { Id = 1, Title = "Todo to Delete", IsDone = false };
        await context.Todos.AddAsync(todo);
        await context.SaveChangesAsync();
        
        var result = await repository.DeleteAsync(1);
        
        Assert.True(result);
        
        var deletedTodo = await context.Todos.FindAsync(1);
        Assert.Null(deletedTodo);
    }
    
    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        await using var context = new ApplicationDbContext(_inMemoryOptions);
        var repository = new TodoRepository(context);
        
        var result = await repository.DeleteAsync(999);
        
        Assert.False(result);
    }
}