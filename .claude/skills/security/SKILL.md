---
name: security
description: Revenue Dashboard güvenlik kuralları. Veritabanı erişimi, Excel import, kimlik doğrulama ve güvenli kod geliştirme sırasında bu skill'i kullan.
paths: "**/*.cs"
---

# Güvenlik kuralları

## Veritabanı
- SQL her zaman parametreli (@param). Kullanıcı girdisini asla SQL metnine ekleme
  (SQL injection riski).
- Bağlantı dizesini yapılandırmadan oku (ConnectionStrings:DefaultConnection).
  Bağlantı dizesi, kullanıcı adı veya şifreyi asla kod içine gömme.

## Excel import (dosya yükleme)
- Dosyanın gerçekten seçildiğini ve boş olmadığını kontrol et
  (file == null || file.Length == 0 durumunu ele al).
- Sadece beklenen uzantıya izin ver (.xlsx). Diğer uzantıları reddet.
- Aşırı büyük dosyalara karşı bir boyut sınırı uygula.
- Her hücreyi güvenli oku: boş/hatalı hücrede uygulamayı çökertme,
  satırı atla veya anlamlı bir hata döndür.
- Import ekranı yalnızca yetkili kullanıcılara açık olacak (aşağıya bakınız).

## Kimlik doğrulama ve yetkilendirme (ileride eklenecek)
- Şifreler asla düz metin saklanmaz; güçlü bir hash ile saklanır.
- Roller: Admin ve User. Dashboard'u görüntülemek giriş gerektirir;
  Excel import ve yönetim işlemleri yalnızca Admin rolüne açıktır.
- Korunması gereken controller/action'lar [Authorize] ile işaretlenir;
  role özel olanlar [Authorize(Roles = "Admin")] kullanır.
- Gizli anahtarlar (JWT anahtarı vb.) yapılandırmadan okunur, koda gömülmez.

## Genel
- Kullanıcıya ham hata/stack trace gösterme; genel bir hata mesajı dön.