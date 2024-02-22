using CRM.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using CRM.Repository.GenericRepo;
using CRM.Models;
using NToastNotify;
using Microsoft.AspNetCore.Identity.UI.Services;
using Email;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.HttpOverrides;
using DevExpress.XtraCharts;
using CRM.Services;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#region Identity Configuration

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
builder.Services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
builder.Services.Configure<RazorViewEngineOptions>(o =>
{
    o.ViewLocationFormats.Add("/Shared/Components/InputForm/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("/Pages/Shared/Components/InputForm/{0}" + RazorViewEngine.ViewExtension);
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder.WithOrigins("https://mashaerprod.azurewebsites.net")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endregion

#region "Data Context"

//builder.Services.AddDbContext<PerfumeContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<PerfumeContext>(options => options.UseSqlServer(connectionString, providerOptions => providerOptions.EnableRetryOnFailure(maxRetryCount: 50,
                    maxRetryDelay: System.TimeSpan.FromMinutes(60),
                    errorNumbersToAdd: null)));

#endregion

#region "Localization"
var supportedCultures = new[]
    {
        new CultureInfo("ar-EG"),
        new CultureInfo("en-US"),
    };

// Configure localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar-EG");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null).AddDataAnnotationsLocalization(
               options =>
               {
                   var type = typeof(CRM.SharedResource);
                   var assembblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                   var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                   var localizer = factory.Create("SharedResource", assembblyName.Name);
                   options.DataAnnotationLocalizerProvider = (t, f) => localizer;
               }
               );
builder.Services.AddControllers().AddDataAnnotationsLocalization(
          options =>
          {
              var type = typeof(CRM.SharedResource);
              var assembblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
              var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
              var localizer = factory.Create("SharedResource", assembblyName.Name);
              options.DataAnnotationLocalizerProvider = (t, f) => localizer;
          });
builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization(
          options =>
          {
              var type = typeof(CRM.SharedResource);
              var assembblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
              var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
              var localizer = factory.Create("SharedResource", assembblyName.Name);
              options.DataAnnotationLocalizerProvider = (t, f) => localizer;
          }
          );

#endregion
#region "Reporting"
builder.Services.AddDevExpressControls();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false)
    .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
builder.Services.ConfigureReportingServices(configurator =>
{
    configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
    {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});
#endregion
#region Services
builder.Services.AddScoped<IGenericRepo<Category>,GenericRepo<Category>>();
builder.Services.AddScoped<IGenericRepo<Country>, GenericRepo<Country>>();
builder.Services.AddScoped<OrderService>();

#endregion

//#region IpAddress
//builder.Services.Configure<ForwardedHeadersOptions>(option =>
//{
//    option.ForwardedHeaders =
//                            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
//});
//#endregion

#region Change default route
builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    //options.Conventions.AddPageRoute("/Perfume/home", "");
});

//builder.Services.AddRazorPages().AddRazorPagesOptions(o => {
//    o.RootDirectory = "/Area/Admin/Pages/Configurations";

//});

#endregion
#region session
//builder.Services.AddScoped<SessionShoppingCart>(sc => SessionShoppingCart.GetCart(sc));


//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(5); // Set the session timeout to 5 hours
});
builder.Services.AddHttpContextAccessor();

#endregion
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerGeneratorOptions.IgnoreObsoleteActions = true;
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddEndpointsApiExplorer();

#region Toast
builder.Services.AddRazorPages().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PreventDuplicates = true,
    CloseButton = true
});
#endregion


var app = builder.Build();
app.UseDevExpressControls();
System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseForwardedHeaders();
app.UseCors("AllowOrigin");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseNToastNotify();

app.MapRazorPages();

//var supportedCultures = new[]
//{
//    new CultureInfo("en-US"),

//    new CultureInfo("ar-EG")
//};

// Other middleware here...


app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

//app.UseRequestLocalization(new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture("ar-EG")
//    //SupportedCultures = new List<CultureInfo> { new CultureInfo("ar-EG") },
//    //SupportedUICultures = new List<CultureInfo> { new CultureInfo("ar-EG") }
//});

app.UseSession();
app.UseEndpoints(endpoints =>
{
    
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapSwagger();
});




DevExpress.XtraReports.Web.ClientControls.LoggerService.Initialize(new MyLoggerService(builder.Environment));

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

});

//app.Use(async (context, next) =>
//{
//	await next();

//	if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
//	{
//		// Define and execute your JavaScript function here
//		await context.Response.WriteAsync("<script>window.addEventListener('DOMContentLoaded', (event) => {var masterPage = document.getElementById('masterPageId');if (localStorage.getItem('lang') != \"\" && localStorage.getItem(\"Country\") != \"\")\r\n\t{\r\n\r\n\t\tlocation.href = \"/HomeIndex\"\r\n\t\t\t}\r\n\telse\r\n\t{\r\n\t\tlocation.href = \"/Index\"\r\n\t\t\t}\r\n});\r\n</script>");
//		await context.Response.WriteAsync("<script>myFunction();</script>");
//	}
//});

//    // Other middleware here...



app.Run();
