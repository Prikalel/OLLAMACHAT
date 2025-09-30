namespace TestParser;

/// <summary>
/// Вспомогательный класс для подготовки тестовых данных
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Базовый тестовый класс с простым наследованием
    /// </summary>
    public const string BasicInheritanceTestCode = @"
using System;
using System.Collections.Generic;

namespace TestNamespace
{
    public class BaseClass
    {
        public string BaseProperty { get; set; }
        
        public void BaseMethod()
        {
            Console.WriteLine(""Base method"");
        }
    }

    public interface ITestInterface
    {
        void InterfaceMethod();
    }

    public class DerivedClass : BaseClass, ITestInterface
    {
        public string DerivedProperty { get; set; }
        
        public void InterfaceMethod()
        {
            Console.WriteLine(""Interface method implementation"");
        }
        
        public void DerivedMethod()
        {
            Console.WriteLine(""Derived method"");
        }
        
        public class NestedClass
        {
            public string NestedProperty { get; set; }
            
            public class DeepNestedClass
            {
                public string DeepNestedProperty { get; set; }
            }
        }
    }
}";

    /// <summary>
    /// Сложный тестовый класс с множественным наследованием и обобщениями
    /// </summary>
    public const string ComplexInheritanceTestCode = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplexNamespace
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task SaveAsync(T entity);
    }

    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public abstract class ServiceBase<T> where T : BaseEntity
    {
        protected readonly IRepository<T> _repository;
        
        protected ServiceBase(IRepository<T> repository)
        {
            _repository = repository;
        }
        
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }

    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        
        public class UserSettings
        {
            public string Theme { get; set; }
            public bool NotificationsEnabled { get; set; }
        }
    }

    public class UserRepository : IRepository<User>
    {
        public async Task<User> GetByIdAsync(int id)
        {
            // Имитация реализации
            return await Task.FromResult(new User { Id = id, Name = ""Test User"" });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Task.FromResult(new List<User>());
        }

        public async Task SaveAsync(User entity)
        {
            // Имитация сохранения
            await Task.CompletedTask;
        }
    }

    public class UserService : ServiceBase<User>
    {
        public UserService(IRepository<User> repository) : base(repository)
        {
        }
        
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            var allUsers = await _repository.GetAllAsync();
            // Имитация фильтрации
            return allUsers;
        }
    }
}";

    /// <summary>
    /// Тестовый код для проверки обработки ошибок
    /// </summary>
    public const string ErrorHandlingTestCode = @"
using System;
using System.Collections.Generic;

namespace ErrorNamespace
{
    public class IncompleteClass
    {
        // Неполный класс с синтаксической ошибкой
        public void IncompleteMethod(
    }

    public class ValidClass
    {
        public string ValidProperty { get; set; }
        
        public void ValidMethod()
        {
            Console.WriteLine(""Valid method"");
        }
    }
}";

    /// <summary>
    /// Тестовый код для проверки производительности с большим количеством сущностей
    /// </summary>
    public const string PerformanceTestCode = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerformanceNamespace
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        
        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }
    }

    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
        
        public class Category
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }

    public class Order : BaseEntity
    {
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        
        public class OrderItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }
    }

    public class OrderService
    {
        public async Task<Order> CreateOrderAsync(Customer customer, List<OrderItem> items)
        {
            var order = new Order
            {
                Customer = customer,
                Items = items,
                OrderDate = DateTime.Now,
                TotalAmount = CalculateTotal(items)
            };
            
            return await Task.FromResult(order);
        }
        
        private decimal CalculateTotal(List<OrderItem> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                total += item.Quantity * item.UnitPrice;
            }
            return total;
        }
    }
}";

    /// <summary>
    /// Тестовый код для запечатанного класса: sealed class A : B {}
    /// </summary>
    public const string SealedClassInheritanceTestCode = @"
using System;

namespace SealedClassInheritanceNamespace
{
    public class B
    {
        public string BaseProperty { get; set; }
        
        public virtual void BaseMethod()
        {
            Console.WriteLine(""Base method"");
        }
    }

    public sealed class A : B
    {
        public string SealedProperty { get; set; }
        
        public override void BaseMethod()
        {
            Console.WriteLine(""Sealed override base method"");
        }
        
        public void SealedMethod()
        {
            Console.WriteLine(""Sealed method"");
        }
    }
}";

    /// <summary>
    /// Тестовый код для абстрактного класса: abstract class A : B {}, class C : A {}
    /// </summary>
    public const string AbstractClassInheritanceTestCode = @"
using System;

namespace AbstractClassInheritanceNamespace
{
    public class B
    {
        public string BaseProperty { get; set; }
        
        public void BaseMethod()
        {
            Console.WriteLine(""Base method"");
        }
    }

    public abstract class A : B
    {
        public string AbstractProperty { get; set; }
        
        public abstract void AbstractMethod();
        
        public virtual void VirtualMethod()
        {
            Console.WriteLine(""Virtual method"");
        }
    }

    public class C : A
    {
        public string ConcreteProperty { get; set; }
        
        public override void AbstractMethod()
        {
            Console.WriteLine(""Abstract method implementation"");
        }
        
        public override void VirtualMethod()
        {
            Console.WriteLine(""Override virtual method"");
        }
    }
}";

    /// <summary>
    /// Тестовый код для глубокой цепочки наследования (10+ классов)
    /// </summary>
    public const string DeepInheritanceChainTestCode = @"
using System;

namespace DeepInheritanceChainNamespace
{
    public class Level0
    {
        public string Property0 { get; set; }
    }

    public class Level1 : Level0
    {
        public string Property1 { get; set; }
    }

    public class Level2 : Level1
    {
        public string Property2 { get; set; }
    }

    public class Level3 : Level2
    {
        public string Property3 { get; set; }
    }

    public class Level4 : Level3
    {
        public string Property4 { get; set; }
    }

    public class Level5 : Level4
    {
        public string Property5 { get; set; }
    }

    public class Level6 : Level5
    {
        public string Property6 { get; set; }
    }

    public class Level7 : Level6
    {
        public string Property7 { get; set; }
    }

    public class Level8 : Level7
    {
        public string Property8 { get; set; }
    }

    public class Level9 : Level8
    {
        public string Property9 { get; set; }
    }

    public class Level10 : Level9
    {
        public string Property10 { get; set; }
        
        public void FinalMethod()
        {
            Console.WriteLine(""Final method at level 10"");
        }
    }
}";

    /// <summary>
    /// Тестовый код для проверки циклических ссылок в классах (некорректный код)
    /// </summary>
    public const string CircularReferenceClassTestCode = @"
using System;

namespace CircularReferenceClassNamespace
{
    // Этот код содержит синтаксическую ошибку - циклическое наследование классов
    // В реальном C# это не скомпилируется, но мы можем проверить обработку таких случаев
    
    public class A : B
    {
        public string PropertyA { get; set; }
        
        public void MethodA()
        {
            Console.WriteLine(""Method A"");
        }
    }

    public class B : A
    {
        public string PropertyB { get; set; }
        
        public void MethodB()
        {
            Console.WriteLine(""Method B"");
        }
    }
}";

    /// <summary>
    /// Тестовый код для проверки обработки пустого файла
    /// </summary>
    public const string EmptyFileTestCode = @"";

}