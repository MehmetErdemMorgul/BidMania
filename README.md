# 🚀 BidMania (Grup: 59)

## 📌 1. Proje Künyesi
- **Proje Adı:** BidMania  
- **Ekip Üyeleri:**  
  - Mehmet Erdem Morgül - 231307099  
  - Mustafa Küçükvızıltı - 231307098  
- **Tarih:** 5 Nisan 2026  
- **Kurum:** Kocaeli Üniversitesi - Bilişim Sistemleri Mühendisliği  
- **Ders:** Yazılım Laboratuvarı II  

---

## 🎯 2. Giriş: Problem Tanımı ve Vizyon

### ❗ Problem Tanımı
Modern açık artırma sistemleri, saniyeler içinde binlerce kullanıcının aynı ürün için rekabet ettiği, yüksek eşzamanlılık (**concurrency**) ve düşük gecikme (**latency**) gerektiren kritik sistemlerdir.  

Monolitik yapılar:
- Trafik dalgalanmalarında (traffic spikes) çökebilir
- Tüm sistemi tek noktadan kilitler
- Bir modül hatası tüm sistemi etkiler  

---

### 🎯 Amaç
BidMania:

- Mikroservis mimarisi kullanır
- Docker ile containerize edilmiştir
- Yatay ölçeklenebilirlik sağlar
- Hata izolasyonu sunar  

Kullanılan yaklaşımlar:
- ✅ TDD (Test Driven Development)
- ✅ API Gateway Pattern
- ✅ Containerization
- ✅ Real-time Monitoring

---

## 🧠 3. Tasarım ve Mikroservis Mimarisi

### 📊 Richardson Olgunluk Modeli (RMM)

Sistemimiz **Seviye 2 (HTTP Verbs)** seviyesindedir:

#### ✔ Seviye 0 & 1
- `/api/auctions`
- `/api/products`

#### ✔ Seviye 2 (HTTP Verbs)
- `POST` → Yeni kayıt / teklif oluşturma  
- `GET` → Veri çekme  
- `PUT` → Güncelleme  
- `DELETE` → Silme  

#### 🔁 Durum Kodları
- `201 Created`
- `401 Unauthorized`
- `400 Bad Request`

---

### ⚙️ Karmaşıklık Analizi

- **Veri Erişimi:**  
  `O(log n)` → MongoDB B-Tree indeksleme

- **İş Mantığı:**  
  `O(1)` → Teklif doğrulama algoritması

---

### 🔄 Müzayede Teklif Akışı

![Bidding Flow](https://github.com/user-attachments/assets/6ce511fa-edd2-4828-8fa9-7874c1253828)

---

## 🧩 4. Proje Yapısı ve Modüller

### 🛡️ A. AuthService (Kimlik Yönetimi)

**Görev:** JWT üretimi ve kullanıcı yönetimi  

#### 📁 Yapı
- `Controllers/AuthController.cs`
- `Models/User.cs`
- `Models/UserDto.cs`
- `Program.cs`

#### 📦 NuGet Paketleri
- MongoDB.Driver
- JwtBearer
- Prometheus.Net

#### 🧪 Testler
- Integration Test (xUnit)
- Mock Test (Moq)

---

### 📦 B. ProductService (Katalog)

#### 📁 Yapı
- `Models/Product.cs`
- `Program.cs`

#### 📦 Paketler
- MongoDB.Driver
- Prometheus

#### 🧪 Test
- POST / GET testleri
- 400 Bad Request kontrolü

---

### 🔨 C. AuctionService (Müzayede Motoru)

#### 📁 Yapı
- `Models/Bid.cs`
- `Services/BiddingLogic.cs`

#### 🧪 Testler
- Concurrent bidding testleri
- Geçersiz teklif kontrolü

---

### 🚪 D. Dispatcher (API Gateway)

#### 🎯 Görev
- Tüm istekleri karşılar
- Servislere yönlendirir

#### 🔐 Güvenlik
- `X-Internal-Key`
- Tek giriş noktası: **Port 5147**

#### 📦 Paket
- YARP Reverse Proxy (opsiyonel)

---

### 🏗️ E. Docker & Altyapı

#### 📁 Yapılar
- `docker-compose.yml`
- `prometheus.yml`

#### 📊 İçerik
- MongoDB
- Prometheus
- Grafana
- Mikroservisler

![Docker Architecture](https://github.com/user-attachments/assets/56694497-90eb-4dab-bf75-6eb6b369b14d)

---

## 🧪 5. Uygulama Detayları ve Test Stratejisi

### 🔁 TDD & xUnit

- Red → Green → Refactor
- Moq ile bağımlılık izolasyonu

---

### ⚡ k6 Yük Testi

- **200 eşzamanlı kullanıcı**
- Ortalama: **3.72 ms**
- Başarı: **%100**

---

### 🖼️ Ekran Görüntüleri

#### 🔹 Postman
![Postman](https://github.com/user-attachments/assets/12863ac6-2cbe-4160-a403-09df443d8690)
![Postman](https://github.com/user-attachments/assets/0b5be2ae-dae7-4541-804a-5ae1403a947f)
![Postman](https://github.com/user-attachments/assets/65800e1e-2ac7-4b39-9414-ebf4cf766351)
![Postman](https://github.com/user-attachments/assets/63982fca-0f76-42e3-9054-151f76fd432d)

#### 🔹 Docker
![Docker](https://github.com/user-attachments/assets/83dc02c5-007e-4104-b99f-135f0d065392)

#### 🔹 Grafana
![Grafana](https://github.com/user-attachments/assets/df0570b8-2e7b-4a7a-b572-94c4381bc879)
![Grafana](https://github.com/user-attachments/assets/c3e13f02-c9c1-414b-ba08-2ec23838822f)

#### 🔹 MongoDB
![MongoDB](https://github.com/user-attachments/assets/ecb669dd-d694-4b31-a2af-ad23fee8d3e7)

---

## 📌 6. Sonuç ve Tartışma

### ✅ Başarılar
- Mikroservis izolasyonu sağlandı  
- Monitoring (Prometheus + Grafana) aktif  
- TDD ile yüksek kod kalitesi  

---

### ⚠️ Sınırlılıklar
- Frontend eksik  
- Real-time bidding eksik  

---

### 🚧 Karşılaşılan Sorunlar

#### 1. DI ve Test Hataları
- Constructor bağımlılığı problemi
- Moq ile çözüldü

#### 2. Veri Modeli Uyuşmazlığı
- DTO kullanılarak çözüldü

#### 3. Dispatcher Güvenlik Problemleri
- IP bazlı istisna ile çözüldü

---

## 📚 7. Kaynakça

1. Fielding, R. T. (2000)  
2. Richardson & Ruby (2007)  
3. https://xunit.net/  
4. https://prometheus.io/docs/  
5. https://k6.io/docs/  
6. https://github.com/mermaid-js/mermaid  
7. https://www.geeksforgeeks.org/devops/what-is-docker-compose-up/  

---

## 🔗 GitHub

👉 https://github.com/MehmetErdemMorgul/BidMania
