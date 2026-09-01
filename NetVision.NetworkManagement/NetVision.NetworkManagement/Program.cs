using NetVision.NetworkManagement.Services;

// ==============================================================================
// NETVISION AĞ YÖNETİM PLATFORMU - UYGULAMA BAŞLANGIÇ YAPILANDIRMASI (Program.cs)
// Bu dosya ASP.NET Core MVC uygulamasının servis bağımlılıklarını (Dependency Injection),
// yönlendirme (Routing), statik dosya sunumunu ve HTTP işlem boru hattını yapılandırır.
// ==============================================================================

var builder = WebApplication.CreateBuilder(args);

// 1. MVC Controller ve Razor View Servislerinin Eklenmesi
builder.Services.AddControllersWithViews();

// 2. Ağ Veri Servisinin (INetworkDataService) Singleton Olarak Tanımlanması
// Bellekte 157 cihazlık CMDB envanteri, canlı alarmlar ve Cisco konfigürasyonlarını tutar.
builder.Services.AddSingleton<INetworkDataService, NetworkDataService>();

var app = builder.Build();

// 3. HTTP İstek Hattı (Request Pipeline) Yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 4. CSS, JavaScript, İkon ve Görsel Dosyaları için Statik Dosya Middleware'i
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 5. Varsayılan Rota Tanımı: /Home/Index -> Ana Sayfa
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
