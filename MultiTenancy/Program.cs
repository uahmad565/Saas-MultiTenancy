using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MultiTenancy.Data;
using MultiTenancy.Middlewares;
using MultiTenancy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ITenantGetter, TenantService>();
builder.Services.AddScoped<ITenantSetter, TenantService>();

builder.Services.AddScoped<MultiTenantServiceMiddleware>();

builder.Services.AddDbContext<MultiTenancy.Data.MyDatabase>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TenantConnection")));

builder.Services.AddRazorPages();

var app = builder.Build();

// initialize the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MultiTenancy.Data.MyDatabase>();
    await db.Database.MigrateAsync();
}




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// middleware that reads and sets the tenant
app.UseMiddleware<MultiTenantServiceMiddleware>();

app.UseRouting();

// multi-tenant request, try adding ?tenant=Khalid or ?tenant=Internet (default)
app.MapGet("/", async (MyDatabase db) => await db
    .Animals
    // hide the tenant, which is response noise
    .Select(x => new { x.Id, x.Name, x.Kind })
    .ToListAsync());


app.UseAuthorization();

app.MapRazorPages();

app.Run();
