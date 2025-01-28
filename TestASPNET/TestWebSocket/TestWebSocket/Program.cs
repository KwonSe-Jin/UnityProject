using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// HTTPS�� ���� ���� (�⺻������ HTTPS�� ����)
builder.WebHost.UseUrls("https://localhost:5001"); // HTTPS�θ� ����

var app = builder.Build();

app.UseHttpsRedirection(); // HTTPS ���𷺼� (HTTP -> HTTPS)

// WebSocket�� ����� �� �ֵ��� ����
app.UseWebSockets(); // WebSocket�� Ȱ��ȭ

app.MapGet("/", () => "Hello World!");

// WebSocket ��� ó��
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest) // WebSocket ��û���� Ȯ��
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync(); // WebSocket ���� ����
        await HandleWebSocketAsync(webSocket); // ����� WebSocket ó��
    }
    else
    {
        context.Response.StatusCode = 400; // Bad Request (WebSocket�� �ƴ� ��û)
    }
});

// WebSocket �޽��� ���� �� ó��
async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4]; // ������ �޽��� ����

    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None); // �޽��� ����
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count); // �޽��� ���ڵ�

        // ���ŵ� �޽��� �α� ���
        Console.WriteLine($"Message received: {message}");

        // �޽��� ó�� �� ����
        var response = Encoding.UTF8.GetBytes($"Server received: {message}");
        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None); // ���� ����
    }
}

app.Run(); // ���ø����̼� ����
