---
name: add-repository-method
description: Revenue Dashboard projesine yeni bir repository metodu eklemek için kullan. Yeni SQL sorguları, DTO'lar ve repository metotları oluştururken bu skill'i uygula.
disable-model-invocation: true
---

# Yeni Repository Metodu Ekle

Bu projeye yeni bir repository metodu ekle. Şu adımları izle:

1. Önce hangi veriyi çekeceğini sor (hangi tablo, hangi filtreler, ne dönecek).
2. Gerekirse Models/Dtos altında uygun bir DTO oluştur veya mevcut olanı kullan.
3. Metodu önce ilgili arayüze ekle (ör. IRevenueRepository).
4. Sonra somut repository sınıfında (ör. RevenueRepository) uygula.

Uyulacak kurallar:

- Npgsql ile ham, parametreli SQL kullan. Entity Framework KULLANMA.
- Bağlantıyı IDbConnectionFactory.CreateConnection() ile al, await OpenAsync() ile aç.
- connection, command ve reader için await using kullan.
- Kullanıcı değerlerini @param ile parametre olarak geçir; SQL metnine string ekleme.
- Metot async olsun: adı Async ile bitsin, Task/Task<T> dönsün.
- Birden çok satır dönecekse while (await reader.ReadAsync()) ile oku ve DTO'ya doldur.
- Filtre varsa SQL'i WHERE 1 = 1 ile başlat, sadece dolu filtreler için AND ekle.
- Para için decimal kullan; snake_case sütunları PascalCase DTO property'lerine eşle.

Metodu yazdıktan sonra bana özetle:
- Ne ekledin?
- Hangi dosyaları değiştirdin?
- Metot nasıl çağrılır?