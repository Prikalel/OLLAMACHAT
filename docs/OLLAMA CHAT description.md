# OLLAMACHAT Documentation

## Overview

OLLAMACHAT is a sophisticated code analysis tool that leverages Roslyn, the .NET compiler platform, to provide deep insights into C# codebases. This document provides an overview of the system architecture with a focus on dependency resolution capabilities.

## Dependency Resolution Through Roslyn

### Introduction

Dependency resolution is a core feature of OLLAMACHAT that enables the system to understand and analyze the relationships between different components in a C# codebase. By leveraging Roslyn's powerful semantic analysis capabilities, OLLAMACHAT can accurately track and resolve dependencies across files, projects, and solutions.

### How Roslyn is Used for Dependency Resolution

Roslyn provides a rich set of APIs for code analysis that OLLAMACHAT leverages for dependency resolution:

1. **Syntax Analysis**: Roslyn's Syntax API allows OLLAMACHAT to parse C# code into a structured syntax tree, enabling precise identification of code elements and their relationships.

2. **Semantic Analysis**: Through Roslyn's Semantic API, OLLAMACHAT can understand the meaning of code symbols, their types, and how they relate to each other, even across file boundaries.

3. **Symbol Information**: Roslyn provides detailed symbol information that allows OLLAMACHAT to track the definition and usage of types, methods, properties, and other code elements.

4. **Compilation Context**: By working with Roslyn's compilation model, OLLAMACHAT can analyze code in the context of the entire solution, including all referenced assemblies and projects.

### The Role of SolutionLoaderService

The [`SolutionLoaderService`](Source/OLLAMACHAT.Infrastructure/Services/SolutionLoaderService.cs:7) plays a critical role in dependency resolution by loading and maintaining the .NET solution:

1. **Solution Loading**: The service loads the entire solution using Roslyn's MSBuild workspace, providing access to all projects and documents within the solution.

2. **Document Access**: It provides methods to retrieve specific documents and projects, enabling other services to access the code they need to analyze.

3. **Change Monitoring**: The service monitors file system changes and automatically reloads the solution when changes are detected, ensuring that dependency analysis is always based on the current state of the codebase.

4. **Event Notifications**: Through the `SolutionReloaded` event, the service notifies other components when the solution has been reloaded, allowing them to update their analysis accordingly.

The [`SolutionLoaderService`](Source/OLLAMACHAT.Infrastructure/Services/SolutionLoaderService.cs:7) is implemented as a singleton service that maintains the solution state and provides thread-safe access to it through a semaphore, ensuring that concurrent operations don't interfere with each other.

### How ImportService Resolves Import Statements

The [`ImportService`](Source/OLLAMACHAT.Infrastructure/Services/ImportService.cs:8) is responsible for resolving import statements to actual file paths:

1. **Import Path Resolution**: When given an import path (e.g., a namespace from a using statement), the service searches through all documents in the solution to find files that contain matching namespace declarations.

2. **Namespace Matching**: For each using directive in the current document, the service searches for namespace declarations in other documents that match the imported namespace.

3. **Relative Path Calculation**: Once matching files are found, the service calculates relative paths from the current document to the files containing the matching namespaces.

4. **Path Normalization**: The service normalizes paths to ensure they are consistent across different operating systems and environments.

Here's an example of how the [`ImportService.ResolveImportPathAsync`](Source/OLLAMACHAT.Infrastructure/Services/ImportService.cs:21) method works:

```csharp
// Given a using statement like "using MyNamespace;"
// The service will:
// 1. Find all documents in the solution
// 2. Look for namespace declarations matching "MyNamespace"
// 3. Return relative paths to those documents
```

### How RelationshipService Analyzes Code Relationships

The [`RelationshipService`](Source/OLLAMACHAT.Infrastructure/Services/RelationshipService.cs:9) analyzes various types of relationships between code elements:

1. **Inheritance Analysis**: The service identifies class inheritance hierarchies by examining base class declarations and tracking inheritance relationships across files.

2. **Interface Implementation**: It detects which classes implement which interfaces, even when the interface and implementation are in different files.

3. **Method Call Analysis**: The service tracks method calls and references between different classes and components.

4. **Member Access Analysis**: It identifies property, field, and event accesses across the codebase.

The [`RelationshipService.AnalyzeRelationshipsAsync`](Source/OLLAMACHAT.Infrastructure/Services/RelationshipService.cs:18) method performs these analyses by:

1. Building a dictionary of all entities in the current document
2. For each entity, analyzing its relationships based on its type
3. For classes and interfaces, examining inheritance and implementation relationships
4. For methods, analyzing method calls and member accesses

Here's an example of how inheritance relationships are analyzed:

```csharp
// For a class like "public class MyClass : MyBaseClass, IMyInterface"
// The service will:
// 1. Identify that MyClass inherits from MyBaseClass
// 2. Identify that MyClass implements IMyInterface
// 3. Create relationship objects for these connections
```

### How Cross-File Dependencies are Tracked and Resolved

Cross-file dependency tracking is a complex process that involves multiple services working together:

1. **Document Loading**: The [`DocumentService`](Source/OLLAMACHAT.Infrastructure/Services/DocumentService.cs:8) loads individual documents and provides access to their syntax trees and semantic models.

2. **Symbol Resolution**: Using Roslyn's semantic model, symbols are resolved across file boundaries, allowing the system to understand references to types and members defined in other files.

3. **Relationship Mapping**: The [`RelationshipService`](Source/OLLAMACHAT.Infrastructure/Services/RelationshipService.cs:9) maps these cross-file references into structured relationship objects that can be queried and analyzed.

4. **Dependency Graph Construction**: All these relationships are combined to construct a comprehensive dependency graph that represents the entire codebase.

The process works as follows:

1. When analyzing a document, the system first loads all referenced documents using the [`DocumentService`](Source/OLLAMACHAT.Infrastructure/Services/DocumentService.cs:8)
2. It then uses Roslyn's semantic model to resolve symbols across these documents
3. The [`RelationshipService`](Source/OLLAMACHAT.Infrastructure/Services/RelationshipService.cs:9) identifies and categorizes the relationships between symbols
4. These relationships are stored in a structured format that can be used for further analysis

### Benefits of Using Roslyn for Dependency Resolution

Using Roslyn for dependency resolution provides several key benefits compared to other approaches:

1. **Accuracy**: Roslyn provides the same semantic analysis as the C# compiler, ensuring that dependency resolution is accurate and reflects the actual behavior of the code.

2. **Comprehensiveness**: Roslyn understands all aspects of C# code, including complex language features like generics, LINQ, async/await, and more, enabling comprehensive dependency analysis.

3. **Performance**: Roslyn is optimized for performance and can efficiently analyze large codebases, making it suitable for enterprise-level applications.

4. **Extensibility**: Roslyn's extensible architecture allows OLLAMACHAT to easily add new types of analysis and relationship tracking as needed.

5. **Integration**: As the official .NET compiler platform, Roslyn integrates seamlessly with the .NET ecosystem and tooling.

6. **Real-time Analysis**: Roslyn's incremental compilation model enables real-time analysis as code changes, without requiring full recompilation.

### Examples of Dependency Resolution in Practice

#### Resolving a Using Statement to a Specific File

Consider the following scenario:

```csharp
// File: Services/MyService.cs
using MyNamespace.Utilities;

public class MyService 
{
    public void DoWork()
    {
        var helper = new StringHelper();
        helper.Process("data");
    }
}

// File: Utilities/StringHelper.cs
namespace MyNamespace.Utilities
{
    public class StringHelper
    {
        public void Process(string input) { /* ... */ }
    }
}
```

When analyzing `MyService.cs`, OLLAMACHAT will:

1. Identify the `using MyNamespace.Utilities;` statement
2. Use the [`ImportService`](Source/OLLAMACHAT.Infrastructure/Services/ImportService.cs:8) to find files that contain the `MyNamespace.Utilities` namespace
3. Locate `StringHelper.cs` as a match
4. Calculate the relative path from `Services/MyService.cs` to `Utilities/StringHelper.cs`
5. Record this as a dependency relationship

#### Tracking Inheritance Hierarchies Across Multiple Files

Consider this inheritance hierarchy:

```csharp
// File: Models/BaseEntity.cs
namespace MyNamespace.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}

// File: Models/User.cs
namespace MyNamespace.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
    }
}

// File: Models/AdminUser.cs
namespace MyNamespace.Models
{
    public class AdminUser : User
    {
        public string Role { get; set; }
    }
}
```

When analyzing these files, OLLAMACHAT will:

1. Identify that `User` inherits from `BaseEntity`
2. Identify that `AdminUser` inherits from `User`
3. Create inheritance relationships between these classes
4. Build a complete inheritance hierarchy that spans multiple files

#### Identifying Interface Implementations

For interface implementations:

```csharp
// File: Interfaces/IRepository.cs
namespace MyNamespace.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(int id);
        void Save(T entity);
    }
}

// File: Repositories/UserRepository.cs
namespace MyNamespace.Repositories
{
    public class UserRepository : IRepository<User>
    {
        public User GetById(int id) { /* ... */ }
        public void Save(User entity) { /* ... */ }
    }
}
```

OLLAMACHAT will:

1. Identify that `UserRepository` implements `IRepository<User>`
2. Create an implementation relationship between the class and the interface
3. Track this relationship across the file boundary

#### Detecting Method Calls and References

For method calls and references:

```csharp
// File: Services/ValidationService.cs
namespace MyNamespace.Services
{
    public class ValidationService
    {
        public bool Validate(string input)
        {
            return !string.IsNullOrEmpty(input);
        }
    }
}

// File: Services/UserService.cs
namespace MyNamespace.Services
{
    public class UserService
    {
        private readonly ValidationService _validator = new ValidationService();
        
        public bool CreateUser(string name)
        {
            if (_validator.Validate(name))
            {
                // Create user
                return true;
            }
            return false;
        }
    }
}
```

OLLAMACHAT will:

1. Identify that `UserService` creates an instance of `ValidationService`
2. Detect the call to `_validator.Validate(name)` in the `CreateUser` method
3. Create a relationship showing that `UserService` depends on `ValidationService.Validate`

### Integration with the Overall Architecture

The dependency resolution system is integrated with the overall OLLAMACHAT architecture as described in the [Roslyn Parsing Architecture Plan](docs/roslyn-parsing-architecture-plan.md). The key components work together as follows:

1. The [`SolutionLoaderService`](Source/OLLAMACHAT.Infrastructure/Services/SolutionLoaderService.cs:7) loads and maintains the solution
2. The [`DocumentService`](Source/OLLAMACHAT.Infrastructure/Services/DocumentService.cs:8) provides access to individual documents
3. The [`ImportService`](Source/OLLAMACHAT.Infrastructure/Services/ImportService.cs:8) resolves import statements
4. The [`RelationshipService`](Source/OLLAMACHAT.Infrastructure/Services/RelationshipService.cs:9) analyzes relationships between code elements
5. The [`RoslynParsingService`](Source/OLLAMACHAT.Infrastructure/Services/RoslynParsingService.cs:1) orchestrates the overall parsing and analysis process

This integrated approach enables OLLAMACHAT to provide comprehensive dependency analysis that helps developers understand their codebase structure, identify potential issues, and make informed decisions about code organization and refactoring.