using WebServer;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets(); // WebSocket Ȱ��ȭ
app.UseMiddleware<WebSocketHandler>(); // WebSocket �̵���� �߰�

app.MapControllers(); // REST API Ȱ��ȭ
app.Run();