---
name: data-access
description: Repository katmanında PostgreSQL ve Npgsql kullanılarak veri erişimi yapılırken uygulanacak kurallar. Yeni SQL sorguları veya repository metotları yazarken bu skill'i kullan.
paths: "Repositories/**/*.cs"
---

# Veri erişimi (Repository)

- TÜM SQL yalnızca burada. Controller veya Service içinde SQL yok.
- Npgsql ile ham, parametreli SQL kullan. Entity Framework veya ORM KULLANMA.
- Bağlantıyı IDbConnectionFactory.CreateConnection() ile al,
  await connection.OpenAsync() ile aç.
- connection, command ve reader için await using kullan.
- Kullanıcı değerlerini her zaman parametre olarak geçir
  (command.Parameters.AddWithValue("ad", deger)); SQL metnine string ekleme.
- Satırları while (await reader.ReadAsync()) döngüsüyle oku ve bir DTO'ya doldur.
- snake_case sütunları (date, channel_name, company, revenue)
  PascalCase DTO property'lerine eşle.
- Dinamik filtrede: sorguyu WHERE 1 = 1 ile başlat, sadece dolu (null olmayan)
  filtre değerleri için AND koşulu ekle.
- Her repository bir arayüz uygular (ör. IRevenueRepository) ki ileride
  API tabanlı bir sürümle değiştirilebilsin.