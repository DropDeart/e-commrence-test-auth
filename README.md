# Sipariş API Token Management System 🚚🔒

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

## Açıklama
Önce ilk düşüncem sürekli tetiklenen API Endpoindler için Refresh Token metodu geliştirmekti. Fakat :
1. Ekstra Kod yükü
2. Ektra Risk Faktörü
3. Limit Tüketimi
olduğundan, tercih etmedim. Client Credentials Flow kullanmak daha mantıklı geldi. Bu senaryoda client_id ve client_secret ile doğrudan access_token alınabilmekte ve  alınan token cache'de tutulup (NCache veya Redis bile kullanılabilir.) süre doldukça tekrar tokan alabiliriz.
