using Application.UseCases.NonConformityUseCase;
using Application.UseCases.NonConformityUseCase.Interfaces;
using Microsoft.AspNetCore.Http.Connections;
using OfficeOpenXml;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var emailSettings = builder.Configuration.GetSection("MailSettings");

    //TODO: Email logging set up
    //var emailConnectionInfo = new EmailConnectionInfo
    //{
    //    FromEmail = emailSettings["FromEmail"],
    //    ToEmail = emailSettings["ToEmail"],
    //    MailServer = emailSettings["MailServer"],
    //    EnableSsl = true,
    //    Port = Convert.ToInt32(emailSettings["Port"]),
    //    EmailSubject = "Application logs",

    //    NetworkCredentials = new NetworkCredential
    //    {
    //        UserName = emailSettings["FromEmail"],
    //        Password = emailSettings["Password"]
    //    }
    //};
    
    builder.Host.UseSerilog((context, logger) => logger
         .ReadFrom.Configuration(context.Configuration));
    //.WriteTo.Email(emailConnectionInfo, outputTemplate: "[{Timestamp:MM-dd-yyyy HH:mm:ss} {Level:u3}] ==> {Message:lj}{NewLine:l} {Exception}"));
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddBlazoredToast();

    builder.Services.AddServerSideBlazor(options =>
    {
        options.DetailedErrors = false;
        options.DisconnectedCircuitMaxRetained = 100;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(60);
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
        options.MaxBufferedUnacknowledgedRenderBatches = 10;
    }).AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(60);
        options.EnableDetailedErrors = false;
        options.MaximumParallelInvocationsPerClient = 1;
        options.MaximumReceiveMessageSize = 32 * 1024;
        options.StreamBufferCapacity = 10;
    });


    builder.Services.AddScoped<IBaseDataAccess>(baseDataAccess => new BaseDataAccess(builder.Configuration.GetConnectionString("Quasar")));

    builder.Services.AddSingleton<ThemeService>();
    builder.Services.AddScoped<DialogService>();
    builder.Services.AddScoped<NotificationService>();
    builder.Services.AddScoped<TooltipService>();
    builder.Services.AddScoped<ContextMenuService>();

    builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IEvaluationTypesRepository, EvaluationTypesRepository>();
    builder.Services.AddScoped<IEvaluationTypesUseCases, EvaluationTypesUseCases>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    builder.Services.AddScoped<IFileBuilder, FileBuilder>();
    builder.Services.AddScoped<IEvaluationUseCases, EvaluationUseCases>();
    builder.Services.AddScoped<IEvaluatorProjectUseCases, EvaluatorProjectUseCases>();
    builder.Services.AddScoped<INonConformityRepository, NonConformityRepository>();
    builder.Services.AddScoped<INonConformityUseCase, NonConformityUseCase>();
    builder.Services.AddScoped<StateChangeNotifier>();
    builder.Services.AddScoped<ValidateCredentialService>();
    builder.Services.AddScoped<IApplicationService, ApplicationService>();

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Cookie.Name = ".Quasar.Cookies";
                    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("IsEvaluator", policy => policy.RequireRole("Evaluator"));
        options.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
        options.AddPolicy("IsModerator", policy => policy.RequireRole("Admin", "Evaluator"));
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseForwardedHeaders();
    app.UsePathBase("/quasar/");

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseWebSockets();
    app.MapRazorPages();
    app.MapBlazorHub(options =>
    {
        options.Transports = HttpTransportType.LongPolling;
    });
    app.MapFallbackToPage("/_Host");

    app.Run();

    return 1;
}
catch (Exception exception)
{

    Log.Fatal(exception, "Host terminated unexpectedly");
    return 0;
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
