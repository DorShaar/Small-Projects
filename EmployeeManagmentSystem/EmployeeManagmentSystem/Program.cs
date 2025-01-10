using EmployeeManagementSystem.Data;
using EmployeeManagmentSystem.DBHandler;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmployeeDbHandler>();
builder.Services.AddScoped<EmployeeProjectDbHandler>();
builder.Services.AddScoped<DepartmentDbHandler>();
builder.Services.AddScoped<ProjectDbHandler>();

builder.Services.AddDbContext<EmployeeManagementContext>(options =>
															 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default",
					   pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();