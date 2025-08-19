using BlazorApp2.Components;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddHttpClient("ServerAPI", (provider, client) =>
{
    var contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var request = contextAccessor.HttpContext?.Request;
    var baseUrl = builder.Configuration["BaseApiUrl"] ?? $"{request?.Scheme}://{request?.Host}";
    client.BaseAddress = new Uri(baseUrl);
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddCors();

var app = builder.Build();

// ������������ pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ��������� CORS
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// API endpoints
float blinkPeriod = 1.0f;

app.MapGet("/api/blink-period", () =>
{
    return Results.Json(new { period = blinkPeriod });
});

app.MapPost("/api/blink-period", ([FromBody] float newPeriod) => 
{
    Console.WriteLine($"�������� ��������: {newPeriod}"); // �����������
    if (newPeriod > 0 && newPeriod <= 10)
    {
        blinkPeriod = newPeriod;
        return Results.Ok(new { period = blinkPeriod });
    }
    return Results.BadRequest("���������� ��������: 0.1-10 ������");
});

app.Run();