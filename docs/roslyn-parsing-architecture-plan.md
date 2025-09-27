## 1. Service Interfaces

### 1.1 Core Parsing Service (Orchestrator)

### 1.2 Document Service
 - [ ] `Task<Document?> GetDocumentAsync(string filePath, string repoPath);`
 - [x] ~~`Task<string> GetContentHashAsync(Document document);`~~
### 1.3 Entity Service

### 1.4 ✅ Mapper Service

### 1.5 Relationship Service

### 1.6 ✅ Error Service

### 1.7 ✅ Metadata Service (сделать complexity)

### 1.8 Import Service
 - [ ] `Task<List<string>> ResolveImportPathAsync(string importPath, Document document);`
 - [x] ~~`Task<List<string>> FindCommonInitFilesAsync(string repoPath);`~~