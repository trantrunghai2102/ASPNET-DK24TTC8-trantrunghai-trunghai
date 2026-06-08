using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient(_ => new MySqlConnection(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var connection = new MySqlConnection(connectionString);
    try
    {
        connection.Open();
        app.Logger.LogInformation("Ket noi MySQL thanh cong! Database: {Database}, Server: {Server}",
            connection.Database, connection.DataSource);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ket noi MySQL that bai!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
