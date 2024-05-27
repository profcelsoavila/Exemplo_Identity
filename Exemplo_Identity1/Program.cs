using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Exemplo_Identity1;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = false; // Ativa a validação de escopo. False evita erro de BD inexistente
});

//adiciona os serviços de autenticação. IdentityUser é uma classe do Identity que identifica 
//um usuário cadastrado no banco de dados
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<Context>();

builder.Services.AddControllersWithViews();


// Add services to the container.
builder.Services.AddControllersWithViews();

//Configuração da Entity Framework Core
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"], 
      
    //evita que o BD não seja criado por problemas de timeout com o servidor
    sqlServerOptionsAction: sqlOptions =>
      {
          sqlOptions.EnableRetryOnFailure(
              maxRetryCount: 10,
              maxRetryDelay: TimeSpan.FromSeconds(30),
              errorNumbersToAdd: null);
      }));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

//ativa a autenticação e autorização de usuários
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
