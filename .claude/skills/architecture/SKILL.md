---
name: architecture
description: ASP.NET Core MVC katmanlı mimari kuralları. Controller, Service, Repository katmanlarında kod yazarken veya yeni endpoint eklerken bu skill'i kullan.
paths: "**/*.cs"
---

# Katmanlı mimari

Akış her zaman şu yönde: Controller → Service → Repository → PostgreSQL.
Katman atlama (ör. Controller'dan doğrudan Repository çağırma) YOK.

## Controller
- İnce tut. Sadece isteği al, ilgili servisi çağır, sonucu dön (Json veya View).
- İçinde SQL, veritabanı erişimi veya iş mantığı OLMAZ.
- Servise arayüz üzerinden bağımlı ol (ör. IDashboardService).

## Service
- İş mantığı burada yaşar (ör. "Son 7 Gün" gibi hazır filtreleri gerçek tarih aralığına çevirmek).
- Repository'ye arayüz üzerinden bağımlı ol (ör. IRevenueRepository).
- İçinde SQL OLMAZ.

## Repository
- TÜM SQL yalnızca burada. Veriyi çeker ve DTO döner.
- Detaylı kurallar için data-access.instructions.md geçerlidir.

## DTO (Models/Dtos)
- Katmanlar arası veri taşır; veritabanı satırından ayrıdır.
- Detaylı kurallar için dtos.instructions.md geçerlidir.

## Bağımlılıklar (Dependency Injection)
- Somut sınıfa değil, arayüze bağımlı ol.
- Kayıtlar Program.cs'te: Repository ve Service AddScoped;
  bağlantı fabrikası (IDbConnectionFactory) AddSingleton.

## Genişletilebilirlik
- Her Repository bir arayüz uygular. İleride gerçek API'ye geçilince,
  aynı arayüzü uygulayan yeni bir sınıf (ör. HttpClient tabanlı
  ApiRevenueRepository) yazılır ve DI'da değiştirilir.
  Service, Controller ve DTO'lara DOKUNULMAZ.