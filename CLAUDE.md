# Revenue Dashboard

## Proje Durumu

Bu proje aktif olarak geliştirilen bir staj projesidir.

Öncelik:
1. Mevcut çalışan yapıyı korumak.
2. Performansı düşürmemek.
3. Demo sunum kalitesini artırmak.
4. Kod okunabilirliğini korumak.
5. Yeni özellikleri mevcut mimariye uygun eklemek.

Yeni geliştirilecek özellikler mümkün olduğunca mevcut dosyalar üzerinde uygulanmalıdır. Gereksiz yeni dosya veya mimari değişikliği önerilmemelidir.

## Proje Özeti

Bu proje ASP.NET Core MVC (.NET 10) ile geliştirilen bir Revenue Dashboard uygulamasıdır.

Dashboard yöneticilerin gelir verilerini analiz edebilmesini sağlar.

Veriler şu anda Excel dosyalarından PostgreSQL veritabanına aktarılmaktadır.

İlerleyen aşamalarda veri kaynağı REST API'ye taşınabilecek şekilde katmanlı mimari kullanılmaktadır.

---

## Kullanılan Teknolojiler

- ASP.NET Core MVC
- .NET 10
- PostgreSQL
- Npgsql
- Bootstrap 5

---

## Mimari

Katman sırası her zaman:

Controller
→ Service
→ Repository
→ PostgreSQL

Bu sıra bozulmaz.

Controller doğrudan Repository çağırmaz.

Repository dışına SQL yazılmaz.

İş kuralları Service katmanında bulunur.

---

## Dependency Injection

Program.cs içerisinde:

- Repository sınıfları AddScoped
- Service sınıfları AddScoped
- IDbConnectionFactory AddSingleton

olarak kayıt edilir.

---

## Kod Standartları

- File-scoped namespace kullan.
- PascalCase isimlendirme kullan.
- private alanlar _camelCase şeklinde yazılır.
- Database işlemleri async olur.
- Para birimi decimal kullanır.
- SQL sorguları parametreli yazılır.

---

## Proje Amacı

Dashboard ekranında aşağıdaki bilgiler gösterilecektir:

- Toplam gelir
- Kanal bazlı gelirler
- Şirket bazlı gelirler
- Tarih bazlı analiz
- Grafikler
- Filtreleme
- Özet kartları (Summary Cards)

Amaç büyük veri kümelerini yöneticilerin kolay analiz edebilmesini sağlamaktır.

---

## Claude'dan Beklenenler

Kod yazarken:

- Mevcut katmanlı mimariyi bozma.
- Gereksiz dosya oluşturma.
- Aynı isimde farklı DTO üretme.
- Repository dışında SQL yazma.
- Async yapıyı koru.
- Mevcut kod stiline sadık kal.
- Yeni özellik eklerken önce mevcut yapıyı incele.
- Değiştirilen dosyaları işlem sonunda özetle.

## Sık Karşılaşılan Hatalar

### CS1061
DashboardViewModel'de property bulunamadığında önce:
- DashboardViewModel
- DashboardController
- Index.cshtml

dosyalarını karşılaştır.

---

### CS1501
Method parametresi değiştiyse şu zinciri kontrol et:

Controller
↓
Service
↓
Repository
↓
Interface

---

### Yeni metot eklenirse

Yeni metot eklendiğinde aşağıdaki dosyaların tamamı güncellenmelidir.

- IRevenueRepository
- RevenueRepository
- IDashboardService
- DashboardService
- DashboardController
- DashboardViewModel (gerekiyorsa)
- Razor View

---

### Build hatalarında izlenecek sıra

1. İlk build hatasını çöz.
2. Interface ve implementasyonları karşılaştır.
3. Controller → Service → Repository zincirini kontrol et.
4. Razor View'da silinmiş property kullanılıyor mu kontrol et.
5. Program.cs içindeki Dependency Injection kayıtlarını doğrula.

---

### UI Kuralları

Bu proje artık filtre kullanılan bir dashboard değildir.

TV ekranında sürekli açık duran yönetici dashboard'ı hedeflenmektedir.

Kartlar aynı hizada olmalıdır.

Grafikler tek ekranda mümkün olduğunca görünmelidir.

Boşluklar minimum tutulmalıdır.

Grafikler ve tablolar okunabilir olmalıdır.

---

## Dashboard Tasarım Prensipleri

Dashboard sunum odaklıdır.

Masaüstü kullanımından çok büyük ekran/TV görüntülemesi hedeflenmektedir.

Bu nedenle:

- Sayfa otomatik dönebilecek şekilde tasarlanmalıdır.
- Gereksiz scroll oluşturulmamalıdır.
- Bütün önemli metrikler ilk bakışta görülebilmelidir.
- Kart yükseklikleri mümkün olduğunca eşit tutulmalıdır.
- Grafikler responsive olmalı ancak TV çözünürlüğünde bozulmamalıdır.
- Animasyonlar dikkat dağıtmayacak seviyede kullanılmalıdır.

---

## Demo Veri Kuralları

Demo veriler gerçek üretim ortamını taklit etmelidir.

Veri üretirken:

- Her şirket farklı büyüme trendine sahip olmalıdır.
- Platform gelirleri birbirini birebir takip etmemelidir.
- Mevsimsellik dikkate alınmalıdır.
- Hafta sonları ve özel günlerde gelir değişimleri bulunmalıdır.
- Ani sıfırlanmalar oluşturulmamalıdır.
- Tüm şirketler aynı gelir seviyesinde olmamalıdır.

---

## Excel Import Kuralları

Excel dosyaları sistemin temel veri giriş yöntemidir.

Yüklenen dosyalarda:

- Aynı kolon isimleri korunmalıdır.
- Veri tipleri değiştirilmemelidir.
- Tarihler PostgreSQL ile uyumlu olmalıdır.
- Decimal alanlarda kültür bağımsız nokta kullanımı tercih edilmelidir.
- Büyük veri yüklemelerinde performans korunmalıdır.

---

## Repository Kuralları

Repository katmanı yalnızca veri erişiminden sorumludur.

Yeni sorgular yazılırken:

- SQL mümkün olduğunca tek sorguda çözülmelidir.
- Gereksiz bellek kullanımı oluşturulmamalıdır.
- COUNT, SUM ve GROUP BY işlemleri veritabanında yapılmalıdır.
- LINQ yerine PostgreSQL tarafındaki hesaplamalar tercih edilmelidir.
- AsNoTracking benzeri performans yaklaşımları uygulanmalıdır.

---

## Grafik Kuralları

Dashboard grafikleri okunabilirliği ön planda tutmalıdır.

- Aynı renk farklı anlamlarda kullanılmamalıdır.
- Donut grafiklerde yazılar taşmamalıdır.
- Label yoğunluğu azaltılmalıdır.
- Büyük sayılar uygun formatta gösterilmelidir.
- Grafik renkleri proje temasına uygun olmalıdır.

---

## Kod Değişikliği Kuralları

Claude mevcut çalışan kodu gereksiz yere yeniden yazmamalıdır.

Öncelik sırası:

1. Mevcut yapıyı incele.
2. Minimum dosyada değişiklik yap.
3. Gereksiz refactoring yapma.
4. Çalışan kodu bozacak mimari değişiklik önermeden önce mevcut yapıya uy.
5. Değişiklik sonunda hangi dosyaların değiştiğini özetle.