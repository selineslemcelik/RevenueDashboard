---
name: csharp
description: Revenue Dashboard C# kod standartları. Yeni C# sınıfı, metodu veya DTO yazarken bu skill'i kullan.
paths: "**/*.cs"
---

# C# kuralları

- file-scoped namespace kullan (namespace X; şeklinde).
- Tip, metot, public üye ve DTO property'leri PascalCase.
- private alanlar _camelCase; enjekte edilen bağımlılıkları readonly yap.
- Tüm DB/IO metotları async: adı Async ile biter, Task/Task<T> döner, await kullanılır.
- Para için daima decimal, asla double/float.
- Namespace'ler klasörleri yansıtır (RevenueDashboard.Controllers, .Services,
  .Repositories, .Models.Dtos, .Infrastructure).