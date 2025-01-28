using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// HTTPS를 강제 설정 (기본적으로 HTTPS로 실행)
builder.WebHost.UseUrls("https://localhost:5001"); // HTTPS로만 실행

var app = builder.Build();

app.UseHttpsRedirection(); // HTTPS 리디렉션 (HTTP -> HTTPS)

// WebSocket을 사용할 수 있도록 설정
app.UseWebSockets(); // WebSocket을 활성화

app.MapGet("/", () => "Hello World!");

// WebSocket 경로 처리
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest) // WebSocket 요청인지 확인
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync(); // WebSocket 연결 수락
        await HandleWebSocketAsync(webSocket); // 연결된 WebSocket 처리
    }
    else
    {
        context.Response.StatusCode = 400; // Bad Request (WebSocket이 아닌 요청)
    }
});

// WebSocket 메시지 수신 및 처리
async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4]; // 수신할 메시지 버퍼

    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None); // 메시지 수신
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count); // 메시지 디코딩

        // 수신된 메시지 로그 출력
        Console.WriteLine($"Message received: {message}");

        // 메시지 처리 후 응답
        var response = Encoding.UTF8.GetBytes($"Server received: {message}");
        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None); // 응답 전송
    }
}

app.Run(); // 애플리케이션 실행
