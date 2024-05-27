var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DAOs
builder.Services.AddScoped<CombustivelDAO>();
builder.Services.AddScoped<CorDAO>();
builder.Services.AddScoped<MarcaDAO>();
builder.Services.AddScoped<ModeloDAO>();
builder.Services.AddScoped<VeiculoDAO>();
builder.Services.AddScoped<UsuarioDAO>();
builder.Services.AddScoped<TipoUsuarioDAO>();

// Adicionar suporte a sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Adicionar middleware de sessão
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
