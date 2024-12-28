# Kuaför/Berber Yönetim Sistemi

Bu proje, Sakarya Üniversitesi Bilgisayar Mühendisliği Bölümü Web Programlama dersi kapsamında hazırlanmıştır. Proje, kuaför/berber işletmelerine özel olarak tasarlanmış bir yönetim sistemidir. Uygulama, çalışanların ve müşterilerin ihtiyaçlarını karşılamak amacıyla randevu planlama, işlem yönetimi özellikleri sunmaktadır.

---

## Proje Hakkında
Bu projede, ASP.NET Core MVC teknolojisi kullanılarak bay/bayan kuaför salonları için kapsamlı bir işletme yönetim uygulaması geliştirilmiştir. Projenin temel amaçları şunlardır:
- Çalışan ve müşteri randevu süreçlerini kolaylaştırmak,
- Çalışanların müsaitlik durumlarına göre uygun randevular oluşturmak,
- REST API kullanarak veritabanı ile etkili bir şekilde haberleşmek.
---

## Kullanılan Teknolojiler
- **Backend:** ASP.NET Core 6 MVC, C#
- **Frontend:** HTML5, CSS3, Bootstrap, JavaScript
- **Veritabanı:** SQL Server
- **ORM:** Entity Framework Core
- **API:** RESTful API ile veri akışı

---

## Proje Özellikleri
### 1. Kuaför/Berber Yönetimi
- Salonlar, sundukları işlemleri, işlem sürelerini ve ücretlerini tanımlayabilir.
- Hem erkek hem de kadın kuaför salonları sisteme tanımlanabilir.

### 2. Çalışan Yönetimi
- Çalışanlar sisteme tanımlanabilir.
- Çalışanların uzmanlık alanları ve yapabildikleri işlemler seçilebilir.
- Çalışanların müsaitlik saatleri belirtilebilir.

### 3. Randevu Yönetimi
- Kullanıcılar, uygun çalışanlara ve işlemlere göre randevu alabilir.
- Randevu saati, çalışanların uygunluk durumu ve mevcut randevulara göre kontrol edilir.
- Randevu detayları (işlem, süre, ücret) sistemde saklanır.

### 4. REST API
- LINQ sorguları kullanılarak veritabanına REST API üzerinden erişim sağlanır.
