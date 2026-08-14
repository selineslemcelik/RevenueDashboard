# Mimari Karar Günlüğü (Decisions Log)

Bu dosya, projede alınan önemli teknik kararları ve nedenlerini kaydeder.
Yeni bir karar aldıkça en üste yeni bir kayıt ekle (en yeni en üstte).

---
## 0009 — Çok şirketli demo veri yapısı
- Tarih: 2026-07
- Karar: Dashboard tek şirket yerine birden fazla prodüksiyon şirketini destekleyecek demo veri yapısıyla çalışacak.
- Neden: Gerçek kullanım senaryosunu daha iyi temsil etmek ve şirket bazlı filtreleme/karşılaştırma yapabilmek.
- Durum: Kabul edildi.

## 0008 — Gerçekçi trend üreten demo veri
- Tarih: 2026-07
- Karar: Demo veriler mevsimsellik, platform farklılıkları ve şirket bazlı değişken büyüme oranlarıyla üretilecek.
- Neden: Tüm grafiklerin aynı eğilimde görünmesini engellemek ve sunum kalitesini artırmak.
- Durum: Kabul edildi.

## 0007 — Dashboard metrikleri son veri tarihine göre hesaplanacak
- Tarih: 2026-07
- Karar: Dashboard'da CURRENT_DATE yerine veri kümesindeki son tarih referans alınacak.
- Neden: Demo veri gelecekte güncellenmediğinde kartların ve trendlerin yanlış görünmesini engellemek.
- Durum: Kabul edildi.

## 0006 — Dashboard TV ekranına uygun optimize edildi
- Tarih: 2026-07
- Karar: Dashboard büyük ekran/TV sunumu için okunabilir fontlar, otomatik dönen sayfalar ve tam ekran kullanımına göre tasarlandı.
- Neden: Staj sunumlarında uzaktan rahat okunabilmesi.
- Durum: Kabul edildi.

## 0005 — Git guardrail eklendi
- Tarih: 2026-07-01
- Karar: Git henüz kurulmadı. AI'ın git komutlarını kendisi çalıştırması yasaklandı.
- Neden: Proje daha versiyon kontrolüne hazır değil; komutlar kullanıcının kontrolünde olmalı.
- Durum: Kabul edildi.

## 0004 — GitHub Copilot skill set oluşturuldu
- Tarih: 2026-07-01
- Karar: .github altında copilot-instructions + konu bazlı instructions dosyaları hazırlandı.
- Neden: Copilot'un projenin mimarisine ve kurallarına uygun kod önermesini sağlamak.
- Durum: Kabul edildi.

## 0003 — Katmanlı mimari (Controller → Service → Repository)
- Tarih: 2026-06
- Karar: İş mantığı Service'te, tüm SQL Repository'de; katmanlar arayüzlerle ayrıldı.
- Neden: Sorumlulukların ayrılması, test edilebilirlik ve ileride API'ye sancısız geçiş.
- Durum: Kabul edildi.

## 0002 — Entity Framework yerine ham Npgsql + parametreli SQL
- Tarih: 2026-06
- Karar: ORM kullanılmadı; Npgsql ile parametreli ham SQL tercih edildi.
- Neden: Dashboard ağırlıklı okuma/analitik sorgu (SUM, COUNT, GROUP BY);
  ham SQL daha okunaklı ve performanslı, öğrenmesi de şeffaf.
- Durum: Kabul edildi.

## 0001 — Bağlantı için IDbConnectionFactory
- Tarih: 2026-06
- Karar: NpgsqlDataSource yerine, her Npgsql sürümünde çalışan bir bağlantı fabrikası kullanıldı.
- Neden: Paket yükseltmeden çalışması ve Repository'ye temiz şekilde inject edilebilmesi.
- Durum: Kabul edildi.

---

## Bekleyen / gelecekte karara bağlanacak
- Kimlik doğrulama yöntemi: ASP.NET Core Identity mi, cookie tabanlı özel auth mı?
  (Faz D'de karara bağlanacak.)
- Grafik kütüphanesi seçimi (ör. Chart.js) — Faz C'de netleşecek.