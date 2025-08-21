using System;
using Xunit;
using CalculatorApp;

namespace API.Tests.Examples
{
    public class CalculatorTests
    {
        // 1. AAA (Arrange, Act, Assert) Örneği
        [Fact]
        public void Add_TwoNumbers_ReturnsSum()
        {
            // Arrange - Test için gerekli ortamı ve verileri hazırlıyoruz
            var calculator = new Calculator();
            int a = 5;
            int b = 7;
            
            // Act - Test edilecek metodu çağırıyoruz
            int result = calculator.Add(a, b);
            
            // Assert - Sonucun beklediğimiz gibi olduğunu doğruluyoruz
            Assert.Equal(12, result);
        }
        
        // İstisna fırlatma durumu için test
        [Fact]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            var calculator = new Calculator();
            
            Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));
        }
        
        // 3. Theory ve InlineData Örneği - Farklı çarpma işlemleri için test
        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(5, 5, 25)]
        [InlineData(0, 100, 0)]
        [InlineData(-4, 3, -12)]
        public void Multiply_TwoNumbers_ReturnsProduct(int a, int b, int expected)
        {
            var calculator = new Calculator();
            
            int result = calculator.Multiply(a, b);
            
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void IsEven_WithVariousNumbers_ReturnsCorrectResult()
        {
            var calculator = new Calculator();
            
            Assert.True(calculator.IsEven(2));
            Assert.True(calculator.IsEven(0));
            Assert.False(calculator.IsEven(3));
            Assert.False(calculator.IsEven(-5));
        }
        
        // Asal sayı kontrolü
        [Theory]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(17, true)]
        [InlineData(20, false)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        public void IsPrime_WithVariousNumbers_ReturnsCorrectResult(int number, bool expected)
        {
            var calculator = new Calculator();
            
            bool result = calculator.IsPrime(number);
            
            Assert.Equal(expected, result);
        }
    }
}