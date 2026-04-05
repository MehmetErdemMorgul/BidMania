sequenceDiagram
    participant U as Kullanıcı
    participant G as API Gateway (Dispatcher)
    participant A as AuthService
    participant B as AuctionService
    participant M as MongoDB

    U->>G: Teklif İsteği (JWT + Bid Data)
    G->>A: Token Doğrulama
    A-->>G: OK
    G->>B: Teklif İşleme
    B->>M: Mevcut En Yüksek Teklifi Sorgula O(log n)
    M-->>B: Data
    B->>B: Teklif Geçerlilik Kontrolü O(1)
    B->>M: Yeni Teklifi Kaydet
    M-->>B: Başarılı
    B-->>G: 201 Created
    G-->>U: Onay Mesajı
