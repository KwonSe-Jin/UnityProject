using WebServer;

var builder = WebApplication.CreateBuilder(args);

// Add the required services for controllers
builder.Services.AddControllers(); // REST API ���� ���

var app = builder.Build();

app.UseWebSockets(); // WebSocket Ȱ��ȭ
Console.WriteLine("WebSocket middleware activated.");
app.UseMiddleware<WebSocketHandler>(); // WebSocket �̵���� �߰�

app.MapControllers(); // REST API Ȱ��ȭ

app.Run();