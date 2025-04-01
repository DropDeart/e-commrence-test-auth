# Order API Token Management System 🚚🔒

Bu proje, saatlik token alma limiti olan bir REST API'den düzenli aralıklarla sipariş verisi çekmek için geliştirilmiş bir token yönetim sistemidir. 

## Özellikler ✨
- ⏱ **5 Dakikada Bir Otomatik Sipariş Çekme**
- 🔐 **Token Önbellekleme & Otomatik Yenileme**
- 🚫 **Saatlik 5 Token Alma Limiti**
- 🛡 **Thread-Safe Token Yönetimi**
- 📊 **Detaylı Loglama**

## Nasıl Çalışır? 🔧
1. **Token Al:** `client_credentials` flow ile access token alınır.
2. **Önbelleğe Al:** Token, süresi dolana kadar bellekte saklanır.
3. **Sipariş Çek:** Her 5 dakikada bir token ile API'ye istek atılır.
4. **Limit Kontrolü:** Saatlik 5. istekten sonra 1 saat bekletilir.
