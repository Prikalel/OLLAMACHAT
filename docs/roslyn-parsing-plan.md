## 1. Service Interfaces

РЕВЬЮ:

### 1.1 Roslyn parsing service

- ParseResult(
  - Entities: entities.ToList(),
  - Relationships: relationships.ToList(),

- ParsedEntity(
  - Name
  - Type
  - Location
  - Children
  - Modifiers
  - Decorators
  - Inheritance
  - ReturnType
  - Parameters
  - ImportData

### 1.3 Entity Service

### 1.5 Relationship Service

### 1.8 Import Service
 - [ ] `Task<List<string>> ResolveImportPathAsync(string importPath, Document document);`
 - [x] ~~`Task<List<string>> FindCommonInitFilesAsync(string repoPath);`~~

## TODO

- [ ] code complexity (metadata)
- [ ] `Import Entity Type Not Implemented`
- [ ] `улучшить отображение модификаторов для всех типов символов.`

---

### 1. **Пропущены поля класса**
```csharp
public Mesh[] ConvexMeshes = new Mesh[0];
public long[] HashOfSourceMeshes; 
```
**НЕТ в выводе!** Это критически важные члены класса.

### 2. **Неполная информация о типах параметров**
```json
{
  "name": "meshes",
  "type": "",  // ❌ ПУСТОЙ тип!
  "optional": false,
  "defaultValue": null
}
```
Должно быть `"type": "Mesh[]"`

### 3. **Пропущены регионы препроцессора**
```csharp
#if UNITY_EDITOR
// Этот код должен быть отмечен как conditional
#endif
```
**НЕТ информации** о conditional compilation.

### 4. **Нет информации об атрибутах**
```csharp
[SerializeField] // ❌ Пропущено!
public Mesh[] ConvexMeshes;
```

### 5. **Нет информации о using/imports**
```csharp
using UnityEngine;
using System.Linq;
```
**НЕТ** в выводе - важно для понимания зависимостей.

### 6. **Неполная информация о generic типах**
```csharp
public bool SameHash(long[] meshHashes)
{
    return HashOfSourceMeshes.Contains(h); // ❌ Contains - generic метод
}
```

## 🎯 **Что нужно добавить в парсер:**

### 1. **Fields and Properties**
```json
{
  "name": "ConvexMeshes",
  "type": "field",
  "dataType": "Mesh[]",
  "modifiers": ["public"],
  "defaultValue": "new Mesh[0]"
}
```

### 2. **Complete Type Information**
```json
{
  "name": "meshes", 
  "type": "Mesh[]",
  "isArray": true,
  "elementType": "Mesh"
}
```

### 3. **Preprocessor Directives**
```json
{
  "type": "preprocessorRegion",
  "condition": "UNITY_EDITOR",
  "startLine": 20,
  "endLine": 38
}
```

### 4. **Attributes**
```json
{
  "name": "SerializeField",
  "type": "attribute",
  "arguments": []
}
```

### 5. **Using Directives**
```json
{
  "type": "using",
  "namespace": "UnityEngine",
  "line": 1
}
```

### 6. **Method Body Complexity**
```json
{
  "cyclomaticComplexity": 3,
  "lineCount": 10,
  "nestedDepth": 2
}
```

## 🔧 **Идеальный вывод должен включать:**

```json
{
  "filePath": "...",
  "language": "csharp",
  "imports": [
    {"namespace": "UnityEngine", "isStatic": false},
    {"namespace": "System.Linq", "isStatic": false}
  ],
  "entities": [
    {
      "type": "class",
      "name": "NonConvexColliderAsset",
      "baseType": "ScriptableObject",
      "fields": [
        {
          "name": "ConvexMeshes",
          "type": "Mesh[]", 
          "modifiers": ["public"],
          "defaultValue": "new Mesh[0]"
        }
      ],
      "methods": [
        {
          "name": "CreateAsset",
          "parameters": [
            {
              "name": "meshes",
              "type": "Mesh[]",  // ✅ ПОЛНЫЙ тип
              "isArray": true
            }
          ],
          "preprocessorConditions": ["UNITY_EDITOR"]  // ✅ Условная компиляция
        }
      ]
    }
  ],
  "preprocessorRegions": [
    {
      "condition": "UNITY_EDITOR",
      "content": "методы CreateAsset и SameHash"
    }
  ]
}
```

## 💡 **Вывод:**
Информации **недостаточно** для серьезного анализа кода. Парсер извлекает только "верхушку айсберга". Для реального использования нужно добавить извлечение полей, полных типов, атрибутов и информации о препроцессоре.

Хорошая основа, но требует значительной доработки!