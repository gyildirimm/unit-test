using API.DTOs;
using API.Models;
using API.Repositories;
using API.Services;
using Moq;

namespace API.Tests.Services;

public class TodoServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTodos()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var todoItems = new List<TodoItem>
        {
            new() { Id = 1, Title = "Test Todo 1", IsDone = false },
            new() { Id = 2, Title = "Test Todo 2", IsDone = true }
        };
        
        mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItems);
        
        var todoService = new TodoService(mockRepository.Object);
        
        // Act
        var result = await todoService.GetAllAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Todo 1", result[0].Title);
        Assert.Equal("Test Todo 2", result[1].Title);
        Assert.False(result[0].IsDone);
        Assert.True(result[1].IsDone);
    }
    
    //Tek bir kayıt getirme testi
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTodo()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var todoItem = new TodoItem { Id = 1, Title = "Test Todo", IsDone = false };
        
        mockRepository.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItem);
        
        var todoService = new TodoService(mockRepository.Object);
        
        var result = await todoService.GetByIdAsync(1);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Todo", result.Title);
        Assert.False(result.IsDone);
    }
    
    [Theory]
    [InlineData(1, "Test Todo 1")]
    [InlineData(2, "Test Todo 2")]
    [InlineData(3, "Test Todo 3")]
    public async Task GetByIdAsync_WithDifferentIds_ShouldReturnCorrectTodo(int id, string expectedTitle)
    {
        var mockRepository = new Mock<ITodoRepository>();
        var todoItem = new TodoItem { Id = id, Title = expectedTitle, IsDone = false };
        
        mockRepository.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItem);
        
        var todoService = new TodoService(mockRepository.Object);
        
        var result = await todoService.GetByIdAsync(id);
        
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedTitle, result.Title);
    }
    
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(repo => repo.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem)null);
        
        var todoService = new TodoService(mockRepository.Object);
        
        var result = await todoService.GetByIdAsync(999);
        
        Assert.Null(result);
    }
    
    // CreateAsync metodu için test
    [Fact]
    public async Task CreateAsync_ShouldCreateNewTodoAndReturnDto()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var createDto = new TodoCreateDto { Title = "New Todo", IsDone = false };
        var createdItem = new TodoItem { Id = 1, Title = "New Todo", IsDone = false };
        
        mockRepository.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdItem);
        
        var todoService = new TodoService(mockRepository.Object);
        
        var result = await todoService.CreateAsync(createDto);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Todo", result.Title);
        Assert.False(result.IsDone);
    }
    
    // Moq.Verify kullanımı - Repository'nin doğru çağrıldığını doğrulama
    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryWithCorrectParameters()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var createDto = new TodoCreateDto { Title = " New Todo ", IsDone = false }; // Boşluklu başlık
        var expectedTrimmedTitle = "New Todo"; // Servis içinde trim edilecek
        var createdItem = new TodoItem { Id = 1, Title = expectedTrimmedTitle, IsDone = false };
        
        mockRepository.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdItem);
        
        var todoService = new TodoService(mockRepository.Object);
        
        await todoService.CreateAsync(createDto);
        
        // Assert - Verify kullanımı
        mockRepository.Verify(repo => repo.AddAsync(
            It.Is<TodoItem>(item => item.Title == expectedTrimmedTitle && item.IsDone == false), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    // Throws örneği - Hata fırlatma durumunun test edilmesi
    [Fact]
    public async Task CreateAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var createDto = new TodoCreateDto { Title = "New Todo", IsDone = false };
        
        mockRepository.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));
        
        var todoService = new TodoService(mockRepository.Object);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            todoService.CreateAsync(createDto));
    }
    
    // Callback örneği - Repository çağrıldığında ek işlem yapma
    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingTodo()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var updateDto = new TodoUpdateDto { Title = "Updated Todo", IsDone = true };
        var updatedItem = new TodoItem { Id = 1, Title = "Updated Todo", IsDone = true };
        
        TodoItem capturedItem = null;
        
        mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .Callback<TodoItem, CancellationToken>((item, ct) => capturedItem = item)
            .ReturnsAsync(updatedItem);
        
        var todoService = new TodoService(mockRepository.Object);
        
        // Act
        var result = await todoService.UpdateAsync(1, updateDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Todo", result.Title);
        Assert.True(result.IsDone);
        
        // Callback ile yakaladığımız parametreyi kontrol edelim
        Assert.NotNull(capturedItem);
        Assert.Equal(1, capturedItem.Id);
        Assert.Equal("Updated Todo", capturedItem.Title);
        Assert.True(capturedItem.IsDone);
    }
    
    // Dinamik InlineData örneği - MemberData kullanımı
    public static IEnumerable<object[]> TodoTestData()
    {
        yield return new object[] { new TodoCreateDto { Title = "Task 1", IsDone = false }, 1 };
        yield return new object[] { new TodoCreateDto { Title = "Task 2", IsDone = true }, 2 };
        yield return new object[] { new TodoCreateDto { Title = "Task 3", IsDone = false }, 3 };
    }
    
    [Theory]
    [MemberData(nameof(TodoTestData))]
    public async Task CreateAsync_WithDynamicData_ShouldCreateTodoWithCorrectData(TodoCreateDto dto, int expectedId)
    {
        var mockRepository = new Mock<ITodoRepository>();
        var createdItem = new TodoItem { Id = expectedId, Title = dto.Title, IsDone = dto.IsDone };
        
        mockRepository.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem item, CancellationToken ct) => 
                new TodoItem { Id = expectedId, Title = item.Title, IsDone = item.IsDone });
        
        var todoService = new TodoService(mockRepository.Object);
        
        var result = await todoService.CreateAsync(dto);
        
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.IsDone, result.IsDone);
    }
    
    // DeleteAsync için test
    [Theory]
    [InlineData(1, true)]   // Başarılı silme
    [InlineData(999, false)] // Başarısız silme
    public async Task DeleteAsync_ShouldReturnCorrectResult(int id, bool expectedResult)
    {
        var mockRepository = new Mock<ITodoRepository>();
        
        mockRepository.Setup(repo => repo.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        
        var todoService = new TodoService(mockRepository.Object);
        
        var result = await todoService.DeleteAsync(id);
        
        Assert.Equal(expectedResult, result);
    }
}