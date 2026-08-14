---
name: dtos
description: Revenue Dashboard DTO kuralları. Yeni DTO oluştururken veya mevcut DTO'ları düzenlerken bu skill'i kullan.
paths: "Models/Dtos/**/*.cs"
---

# DTO'lar

- DTO'lar sadece veri taşır — mantık ve metot yok.
- Çıktı DTO'ları: string property'ler = string.Empty ile başlar (null uyarısını önler).
- Filtre/girdi DTO'ları: nullable tip kullan (string?, DateTime?);
  null = "kullanıcı bu filtreyi seçmedi" demektir.
- Her amaç için ayrı DTO; veritabanı şeklindeki bir sınıfı DTO olarak kullanma.
- Property adları PascalCase ve değeri anlatır (TotalRevenue, ChannelName).
