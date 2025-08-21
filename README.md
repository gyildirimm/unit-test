# Unit Test Nedir? Niçin Gereklidir?

**Unit Test**, yazılımda en küçük bağımsız parçaları (method, sınıf gibi) izole şekilde test ederek doğru çalıştığından emin olmamızı sağlayan otomatik testlerdir.  

### Neden Gereklidir?

- Hataları erken aşamada yakalar.  
- Refactoring sırasında güvence sağlar.  
- Kodun sürdürülebilirliğini ve güvenilirliğini artırır.  
- Dokümantasyon gibi çalışır, metodun beklenen davranışını gösterir.  

---

# xUnit Nedir?

**xUnit**, .NET dünyasında en çok kullanılan test framework’lerinden biridir.  
Basit, hafif ve modern bir yapıya sahiptir. MSTest ve NUnit gibi alternatifleri vardır. Bu makalemizde xUnit kullanacağız.

---

# İçindekiler

1. Arrange, Act, Assert (AAA) Yapısı
2. Fact
3. Theory ve InlineData
4. dotnet test komutu
5. Name Format
6. Moq servis nedir, nasıl kullanılır
7. Moq üzerinde Verify, Throws ve Callback
8. EF Core InMemory ve SQLite InMemory farkları

---

## 1. Arrange, Act, Assert (AAA) Yapısı

Birim testlerin yazımında kullanılan en yaygın format, AAA (Arrange-Act-Assert) yapısıdır. Bu yapı, testlerinizi düzenli ve anlaşılır hale getirir.

- **Arrange:** Test için gerekli veriler hazırlanır.  
- **Act:** Test edilecek metod çağrılır.  
- **Assert:** Sonuçların beklenen ile aynı olup olmadığı kontrol edilir.  

---

## 2. Fact

`[Fact]` özniteliği, xUnit'te parametre almayan, her zaman aynı şekilde çalışan testler için kullanılır.

```csharp
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    // Arrange - Hazırlık (test verilerini ve ortamını hazırlama)
    var calculator = new Calculator();
    int a = 5;
    int b = 7;
    
    // Act - Eylem (test edilecek metodu çağırma)
    int result = calculator.Add(a, b);
    
    // Assert - Doğrulama (beklenen sonuç ile gerçek sonucu karşılaştırma)
    Assert.Equal(12, result);
}
```

---

## 3. Theory ve InlineData

`[Theory]` parametreli testler için kullanılır.  
`[InlineData]` ile farklı test verileri sağlanabilir.

```csharp
[Theory]
[InlineData(2, 3, 5)]
[InlineData(10, 5, 15)]
public void Sum_ShouldReturnCorrectResult(int a, int b, int expected)
{
    var calculator = new Calculator();

    var result = Calculator.Sum(a, b);
    
    Assert.Equal(expected, result);
}
```

```csharp
[Theory]
[InlineData(2, true)]
[InlineData(17, true)]
[InlineData(20, false)]
[InlineData(1, false)]
public void IsPrime_WithVariousNumbers_ReturnsCorrectResult(int number, bool expected)
{
    var calculator = new Calculator();

    bool result = calculator.IsPrime(number);
    
    Assert.Equal(expected, result);
}
```

`InlineData` özniteliği, test metoduna parametre olarak iletilecek değerleri belirtir. Bu sayede tek bir test metodunu farklı girdi kombinasyonlarıyla çalıştırabilirsiniz.

---

## 4. dotnet test komutu

.NET Core projelerinizde testleri çalıştırmak için `dotnet test` komutunu kullanabilirsiniz. Bu komut, test projenizde bulunan tüm testleri çalıştırır.

```bash
# Tüm testleri çalıştırma
dotnet test

# Belirli bir test projesindeki testleri çalıştırma
dotnet test API.Tests/API.Tests.csproj

# Belirli bir test sınıfı çalıştırma
dotnet test --filter "FullyQualifiedName=CalculatorTests"

# Belirli bir test metodunu çalıştırma
dotnet test --filter "FullyQualifiedName=CalculatorTests.Add_TwoNumbers_ReturnsSum"

# Detaylı çıktı ile testleri çalıştırma
dotnet test --verbosity normal

# Belirli isim desenini içeren testleri çalıştırma (~ operatörü)
dotnet test --filter "FullyQualifiedName~CalculatorTests"

# Test sonuçlarını XML formatında kaydetme
dotnet test --logger "trx;LogFileName=testresults.trx"

# 🔹 Console üzerinde detaylı test sonucu alma
dotnet test --logger "console;verbosity=detailed"

# 🔹 HTML raporu oluşturma
dotnet test --logger "html;LogFileName=testresults.html"

# 🔹 JUnit XML raporu oluşturma (Jenkins / GitLab CI gibi araçlarda kullanılır)
dotnet test --logger "junit;LogFileName=testresults.xml"

# 🔹 JSON formatında test sonucu alma
dotnet test --logger "json;LogFileName=testresults.json"
```

---

## 5. Test İsimlendirme Formatı

İyi bir test ismi, testin amacını, test edilen senaryoyu ve beklenen sonucu açıkça ifade etmelidir. Yaygın bir format:
Genel format: **MethodName_StateUnderTest_ExpectedBehavior**

```text
[Test Edilen Metod]_[Test Koşulu]_[Beklenen Sonuç]
```

- `Divide_ByZero_ThrowsDivideByZeroException`
- `IsPrime_WithVariousNumbers_ReturnsCorrectResult`
- `CalculateFactorial_WithNegativeNumber_ThrowsArgumentException`

Bu format, testlerin amacını ve kapsamını net bir şekilde ifade eder. Başarısız bir testle karşılaştığınızda, neyin test edildiğini ve neyin beklendiğini hızlıca anlayabilirsiniz.

---

## 6. Moq Servis Nedir, Nasıl Kullanılır?

Moq, .NET için popüler bir mocking (taklit) kütüphanesidir. Birim testlerde, test edilen sınıfın bağımlılıklarını izole etmek için kullanılır. Moq ile:

1. Interface veya abstract sınıfların mock (sahte) versiyonlarını oluşturabilirsiniz.
2. Mock nesnelerin hangi metodlarının çağrıldığında ne döndüreceğini belirtebilirsiniz.
3. Mock nesnelerin hangi parametrelerle çağrıldığını doğrulayabilirsiniz.

Örnek kullanım:

```csharp
// Mock repository oluşturma
var mockRepository = new Mock<ITodoRepository>();

// Repository'nin davranışını belirleme
mockRepository.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new TodoItem { Id = 1, Title = "Test Todo", IsDone = false });

// Mock nesneyi kullanma
var todoService = new TodoService(mockRepository.Object);
```

---

## 7. Moq Üzerinde Verify, Throws ve Callback

Moq, mock nesnelerin davranışlarını ve etkileşimlerini doğrulamak için çeşitli yöntemler sunar:

### Verify

`Verify` metodu, bir mock nesnenin belirli bir metodunun beklendiği gibi çağrılıp çağrılmadığını kontrol eder:

```csharp
// Repository'nin AddAsync metodunun belirli parametrelerle çağrıldığını doğrulama
//Times ise bu servisin kaç defa çalıştığını belirtir. 'Once' seçeneği bir kez çalıştığını belirtir.

mockRepository.Verify(repo => repo.AddAsync(
    It.Is<TodoItem>(item => item.Title == "New Todo" && item.IsDone == false), 
    It.IsAny<CancellationToken>()), 
    Times.Once);
```

### Throws

`Throws` veya `ThrowsAsync` metodları, mock nesnenin istisna fırlatmasını simüle etmenize olanak tanır:

```csharp
// Repository'nin istisna fırlatmasını ayarlama
mockRepository.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
    .ThrowsAsync(new InvalidOperationException("Database error"));

// İstisnanın fırlatılacağını doğrulama
await Assert.ThrowsAsync<InvalidOperationException>(() => 
    todoService.CreateAsync(new TodoCreateDto { Title = "New Todo", IsDone = false }));
```

### Callback

`Callback` metodu, mock bir metodun çağrıldığında özel bir işlem gerçekleştirmenize olanak tanır:

```csharp
TodoItem capturedItem = null;

mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
    .Callback<TodoItem, CancellationToken>((item, ct) => capturedItem = item)
    .ReturnsAsync(new TodoItem { Id = 1, Title = "Updated Todo", IsDone = true });

// Yakalanan parametreyi kontrol etme
Assert.NotNull(capturedItem);
Assert.Equal(1, capturedItem.Id);
Assert.Equal("Updated Todo", capturedItem.Title);
```

---

## 8. EF Core InMemory ve SQLite InMemory

Entity Framework Core, birim ve entegrasyon testlerinde kullanabileceğiniz iki farklı in-memory veritabanı sağlayıcısı sunar:

### EF Core InMemory

EF Core InMemory, tamamen hafızada çalışan ve gerçek bir veritabanı motoru kullanmayan bir sağlayıcıdır:

```csharp
// InMemory veritabanı için options oluşturma
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
    .Options;

// Context ve repository oluşturma
await using var context = new ApplicationDbContext(options);
var repository = new TodoRepository(context);

// Test verileri ekleme
await context.Todos.AddRangeAsync(
    new TodoItem { Id = 1, Title = "Test Todo 1", IsDone = false },
    new TodoItem { Id = 2, Title = "Test Todo 2", IsDone = true }
);
await context.SaveChangesAsync();

// Testi gerçekleştirme
var todos = await repository.GetAllAsync();
Assert.Equal(2, todos.Count);
```

### SQLite InMemory

SQLite InMemory, gerçek bir SQLite veritabanı motorunu hafızada çalıştırır:

```csharp
// SQLite InMemory bağlantısı oluşturma
var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlite(connection)
    .Options;

// Veritabanını oluşturma
await using (var context = new ApplicationDbContext(options))
{
    await context.Database.EnsureCreatedAsync();
    // Test verileri ekleme...
}

// Yeni bir context ile test etme
await using (var context = new ApplicationDbContext(options))
{
    var repository = new TodoRepository(context);
    var result = await repository.GetByIdAsync(1);
    // Sonuçları doğrulama...
}

// Bağlantıyı kapatma
await connection.CloseAsync();
```

### Farkları

| EF Core InMemory | SQLite InMemory |
|------------------|-----------------|
| Gerçek bir veritabanı motoru kullanmaz | Gerçek bir SQLite motorunu kullanır |
| İlişkisel veritabanı kısıtlamalarını tam olarak desteklemez (foreign key vb.) | Gerçek ilişkisel veritabanı özelliklerini destekler |
| Sadece temel CRUD işlemleri için uygundur | Daha karmaşık sorgular ve işlemler için daha gerçekçi |
| Çok hızlıdır | InMemory'ye göre biraz daha yavaştır |
| Schema migration gerektirmez | Schema migration gerektirir (`EnsureCreated`) |

---

## Sonuç

xUnit ve Moq kullanarak birim testler yazmak, kodunuzun kalitesini artırmak ve hata riskini azaltmak için çok etkili bir yöntemdir. Bu makalede gördüğümüz gibi, testlerinizi düzenli bir yapıda yazarak, farklı senaryoları test ederek ve bağımlılıkları izole ederek kodunuzun beklenen şekilde çalıştığından emin olabilirsiniz.

Ayrıca, EF Core InMemory ve SQLite InMemory gibi araçlarla, gerçek veritabanına bağımlılık olmadan repository katmanınızı test edebilir, gerektiğinde gerçek veritabanı sistemleriyle de entegrasyon testleri yapabilirsiniz.

İyi bir test stratejisi, yazılım geliştirme sürecinizin ayrılmaz bir parçası olmalıdır. Testlerinizi erken ve sık çalıştırarak, kodunuzdaki potansiyel sorunları hızla tespit edebilir ve giderebilirsiniz.