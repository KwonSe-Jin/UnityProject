using WebServer;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets(); // WebSocket 활성화
app.UseMiddleware<WebSocketHandler>(); // WebSocket 미들웨어 추가

app.MapControllers(); // REST API 활성화
app.Run();