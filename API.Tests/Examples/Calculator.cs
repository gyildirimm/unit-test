using System;

namespace CalculatorApp
{
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        
        public int Subtract(int a, int b) => a - b;
        
        public int Multiply(int a, int b) => a * b;
        
        public double Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero");
            return (double)a / b;
        }
        
        public bool IsEven(int number) => number % 2 == 0;
        
        public bool IsPrime(int number)
        {
            if (number <= 1)
                return false;
            if (number == 2)
                return true;
            if (number % 2 == 0)
                return false;
                
            var boundary = (int)Math.Floor(Math.Sqrt(number));
            
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                    return false;
            }
            
            return true;
        }
    }
}