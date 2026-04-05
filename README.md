BidMania (Grup:59)

Proje Künyesi • Proje Adı: BidMania • Ekip Üyeleri: Mehmet Erdem Morgül-231307099, Mustafa Küçükvızıltı-231307098 • Tarih: 5 Nisan 2026 • Kurum: Kocaeli Üniversitesi - Bilişim Sistemleri Mühendisliği • Ders: Yazılım Laboratuvarı II
Giriş: Problem Tanımı ve Vizyon Problem Tanımı: Modern açık artırma sistemleri, saniyeler içinde binlerce kullanıcının aynı ürün için rekabet ettiği, yüksek eşzamanlılık (concurrency) ve düşük gecikme süresi (latency) gerektiren kritik sistemlerdir. Monolitik yapılar, bu ani trafik dalgalanmalarında (traffic spikes) tüm kaynakları tek bir noktada tüketerek sistemin tamamen kilitlenmesine neden olur. Ayrıca, bir modüldeki (örn: ürün listeleme) arıza, tüm müzayede sürecini felç eder. Amaç: BidMania, müzayede süreçlerini tamamen bağımsız, Docker üzerinde konteynerize edilmiş mikroservisler haline getirerek hata izolasyonunu ve yatay ölçeklenebilirliği sağlamayı amaçlar. Proje; TDD (Test-Driven Development), API Gateway Pattern, Containerization ve Real-time Monitoring disiplinlerini harmanlayarak endüstriyel standartlarda bir çözüm sunar.
Tasarım ve Mikroservis Mimarisi Richardson Olgunluk Modeli (RMM) Uygulaması Sistemimiz, RESTful servis kalitesini belirleyen Richardson modelinde Seviye 2 (HTTP Verbs) standartlarını tam teşekküllü olarak uygular: • Seviye 0 & 1: Fonksiyonlar kaynak bazlı URI'lara (/api/auctions, /api/products) ayrılmıştır. • Seviye 2 (HTTP Verbs): CRUD operasyonları HTTP semantiğine uygun şekilde yönetilir: o POST: Yeni kayıt (Register), yeni ürün veya yeni teklif (Bid) oluşturma. o GET: Ürün listeleme, kullanıcı bilgisi veya aktif müzayede detaylarını çekme. o PUT: Mevcut ürün bilgilerini veya müzayede statüsünü güncelleme. o DELETE: Süresi dolan veya iptal edilen kayıtları sistemden temizleme. o Durum Kodları: Başarıda 201 Created, yetkisiz girişte 401 Unauthorized, geçersiz tekliflerde 400 Bad Request dönülerek protokol seviyesinde iletişim kurulmuştur. Karmaşıklık Analizi • Veri Erişimi: MongoDB üzerinde B-Tree indeksleme sayesinde 
O
(
log
⁡
n
)
 sorgu hızı sağlanmıştır. • İş Mantığı: Teklif doğrulama algoritmaları sabit zamanlı 
O
(
1
)
 karmaşıklığına sahiptir. Müzayede Teklif Verme Akışı:
image
Proje Yapısı ve Modüller Projemiz, görselde görüldüğü üzere Separation of Concerns (Sorumlulukların Ayrılması) prensibiyle tasarlanmış olup, her mikroservis kendi içinde bağımsız bir "Solution" gibi davranmaktadır. 🛡️ A. BidMania.AuthService (Kimlik Yönetimi) Sistemin kapı bekçisidir; kullanıcı güvenliği ve JWT üretiminden sorumludur. • Klasör ve Dosya Yapısı: o Controllers/AuthController.cs: Register ve Login endpoint'lerini barındırır. IMongoDatabase bağımlılığını Constructor üzerinden alarak Dependency Injection uygular. o Models/User.cs: MongoDB'de saklanan asıl "User" dokümanıdır. [BsonId] niteliğiyle eşsiz kimlik yönetimi sağlar. o Models/UserDto.cs: "Data Transfer Object". Dışarıdan gelen ham veriyi karşılar; veritabanı şemasını dış dünyadan gizler. o Program.cs: MongoDB bağlantı ayarları, JWT konfigürasyonu ve servis kayıtlarının yapıldığı "merkez üssü"dür.
• Kullanılan NuGet Paketleri: o MongoDB.Driver: NoSQL veritabanı erişimi için. o Microsoft.AspNetCore.Authentication.JwtBearer: Token tabanlı güvenlik için. o Prometheus.Net: Metrik toplama işlemleri için. • Test Yaklaşımı (BidMania.Auth.Tests): o UserTests.cs: xUnit ve WebApplicationFactory kullanılarak "Integration Test" (Entegrasyon Testi) yapılmıştır. Gerçek bir HTTP isteği simüle edilerek şifre hataları ve başarılı kayıt durumları test edilmiştir. o AuthControllerTests.cs: Moq kütüphanesiyle MongoDB taklit edilmiş (Mocking), veritabanı olmadan Controller mantığı saniyeler içinde test edilmiştir.

📦 B. BidMania.ProductService (Katalog Servisi) Müzayedeye çıkacak ürünlerin tüm yaşam döngüsünü yönetir. • Klasör ve Dosya Yapısı: o Models/Product.cs: Ürün adı, fiyatı ve ID bilgilerini içeren veritabanı şemasıdır. o Program.cs: Minimal API veya Controller bazlı yönlendirmelerin yapıldığı, ürün listeleme ve ekleme mantığının kurulduğu dosyadır. • Kullanılan NuGet Paketleri: o MongoDB.Driver: Ürünlerin NoSQL ortamında saklanması için. o Prometheus.Client.AspNetCore: Ürün çekme hızlarını izlemek için. • Test Yaklaşımı (BidMania.Product.Tests): o ProductTests.cs: Ürün ekleme (POST) ve listeleme (GET) fonksiyonlarının doğruluğunu denetler. Boş ürün gönderildiğinde sistemin 400 Bad Request döndüğü burada kanıtlanmıştır.

🔨 C. BidMania.AuctionService (Müzayede Motoru) Sistemin en dinamik parçasıdır; canlı teklif verme (Bidding) süreçlerini yönetir. • Klasör ve Dosya Yapısı: o Models/Bid.cs: Teklif veren kullanıcı ID'si, ürün ID'si ve teklif miktarını tutan sınıftır. o Services/BiddingLogic.cs: Teklifin geçerliliğini (mevcut fiyattan yüksek mi?) kontrol eden algoritmayı barındırır. • Test Yaklaşımı (BidMania.Auction.Tests): o TDD Kanıtı: En karmaşık testler buradadır. "Aynı anda iki kişi teklif verirse ne olur?" veya "Düşük teklif reddediliyor mu?" gibi senaryolar xUnit ile kodlanmıştır.

🚪 D. BidMania.Dispatcher (API Gateway) Dış dünyaya açılan tek penceremizdir. • İşlev: Gelen tüm istekleri karşılar, X-Internal-Key güvenliğini denetler ve isteği ilgili servise (Auth, Product veya Auction) yönlendirir. • Güvenlik: Hiçbir mikroservis dışarıya port açmaz; sadece Dispatcher'ın 5147 portu üzerinden içeri sızılabilir. • NuGet Paketleri: YARP.ReverseProxy (opsiyonel) veya özel middleware yazılımları kullanılmıştır.

🏗️ E. Docker & Altyapı Tüm bu yapıları tek bir komutla ayağa kaldıran orkestrasyon katmanıdır. • docker-compose.yml: MongoDB, Prometheus, Grafana ve tüm servislerin ağ (network) ayarlarını ve bağımlılıklarını (depends_on) tanımlar. • prometheus.yml: Hangi servisin metriklerinin hangi aralıklarla çekileceğini belirleyen yapılandırma dosyasıdır. image

Uygulama Detayları ve Test Stratejisi TDD ve xUnit ile Birim Testleri Proje, xUnit framework'ü kullanılarak TDD (Test-Driven Development) prensipleriyle geliştirilmiştir. • Red-Green-Refactor: Önce başarısız testler yazılmış, ardından bu testleri geçirecek kodlar eklenmiş ve son aşamada kod kalitesi artırılmıştır. • Moq Kullanımı: Veritabanı ve dış servis bağımlılıkları Moq kütüphanesi ile taklit (mock) edilerek, testlerin sadece iş mantığına odaklanması sağlanmıştır. k6 ile Yük Testi Sistemin dayanıklılığı, k6 aracıyla simüle edilen 200 eşzamanlı kullanıcı (VU) altında ölçülmüştür. 3.72 ms alınmış ve test %100 başarı ile sonuçlanmıştır. Ekran Görüntüleri Paneli: • Postman API Testleri:
image image image image
• Docker Desktop: image

• Grafana Dashboard: image

image
• MongoDB Compass: image

Sonuç ve Tartışma Başarılar: • Servisler arası tam izolasyon sağlandı; AuthService kapansa dahi mevcut ürünler listelenebilmektedir. • Prometheus ve Grafana ile sistemdeki darboğazlar (bottleneck) görünür hale getirildi. • TDD yaklaşımıyla kod kalitesi en üst seviyede tutuldu. Sınırlılıklar ve Gelecek Geliştirmeler: • Gerekli frontend kısımları ile projemizi gerçek bir web uygulaması haline getirmek en büyük hedeflerimizden bir tanesi • Teklif güncellemelerini anında iletmek için gerekli entegrasyonlar yapılmalıdır. Karşılaştığımız sorunlar: • Bağımlılık Enjeksiyonu (DI) ve Test Hataları: TDD (Test-Driven Development) sürecinde, AuthController sınıfının test ortamında ayağa kaldırılamaması (1 bağımsız değişken alan oluşturucu içermiyor) sorunu ile karşılaşılmıştır. Bu sorun, Controller yapısının IMongoDatabase arayüzünü constructor üzerinden alacak şekilde yeniden modellenmesi ve testlerde Moq kütüphanesiyle bu bağımlılığın taklit edilmesiyle çözülmüştür.. • Veri Modeli Uyuşmazlığı: Dışarıdan gelen kullanıcı verileri ile veritabanına kaydedilen şemalar arasındaki farklar (User vs UserDto), DTO (Data Transfer Object) deseni tam kapasiteyle uygulanarak ve tip dönüşümleri (Mapping) yapılarak standartlaştırılmıştır. • Dispatcher ve Güvenlik Duvarı Engelleri: API Gateway (Dispatcher) üzerinden geçmeyen isteklerin 403 Forbidden ile reddedilmesi, geliştirme aşamasında lokal testleri zorlaştırmıştır. X-Internal-Key kontrolü yapan middleware yapısı, Docker iç ağı (Internal Network) ve lokal geliştirme ortamı (localhost) için IP bazlı istisnalar tanımlanarak optimize edilmiştir.
Kaynakça
Fielding, R. T. (2000). Architectural Styles and the Design of Network-based Software Architectures.
Richardson, L. & Ruby, S. (2007). RESTful Web Services. O'Reilly.
xUnit.net Documentation: https://xunit.net/
Prometheus & Grafana Lab: https://prometheus.io/docs/
k6 Load Testing: https://k6.io/docs/
https://github.com/mermaid-js/mermaid
https://www.geeksforgeeks.org/devops/what-is-docker-compose-up/
GitHub: https://github.com/MehmetErdemMorgul/BidMania
