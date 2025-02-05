using WebServer;

var builder = WebApplication.CreateBuilder(args);

// Add the required services for controllers
builder.Services.AddControllers(); // REST API 서비스 등록

var app = builder.Build();

app.UseWebSockets(); // WebSocket 활성화
Console.WriteLine("WebSocket middleware activated.");
app.UseMiddleware<WebSocketHandler>(); // WebSocket 미들웨어 추가

app.MapControllers(); // REST API 활성화

app.Run();