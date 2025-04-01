Bu commit, belirli bir REST API'ye yapılan isteklerde token yönetimini düzgün şekilde ele alır. Bu bağlamda aşağıdaki adımlar gerçekleştirilmiştir:

1. **JWT Authentication ve Authorization:**  
   - JWT tabanlı kimlik doğrulama eklendi. API'ye yapılan her istek, geçerli bir JWT token ile doğrulanmaktadır.
   - `AddAuthentication` ve `AddJwtBearer` kullanılarak JWT doğrulama süreci yapılandırıldı. `TokenValidationParameters` içinde issuer, audience ve signing key gibi güvenlik ayarları yapıldı.
   
2. **Token Cache Yönetimi ve Yenileme:**
   - Token yönetimi, her istek için yeni bir token almaktansa, geçerli bir token cache'lenerek kullanıldı. Eğer token süresi dolmuşsa, yeni token alındı.
   - `RefreshToken` mantığı uygulandı. Token süresi sona erdiğinde, yeni bir token almak yerine refresh token kullanılarak token yenileme işlemi gerçekleştirilebilecek şekilde yapılandırıldı.

3. **API Rate Limiting Yönetimi:**
   - API üzerinden yapılacak istekler için, saatlik 5 isteklik limit eklenerek, bu limitin aşılmasının önüne geçildi. 
   - Token yenileme işlemi, istek limitine takılmadan önce yapılması için optimize edildi.

4. **Swagger ve Güvenlik Ayarları:**
   - Swagger UI, geliştirme ortamında aktif hale getirildi.
   - HTTPS kullanımı zorunlu kılındı ve güvenlik başlıkları doğru şekilde yapılandırıldı.
