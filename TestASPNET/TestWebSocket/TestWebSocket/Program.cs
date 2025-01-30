using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);


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
app.Run(); // ���ø����̼� ����

// WebSocket �޽��� ���� �� ó��
async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4]; // ������ �޽��� ����
	var cancellationToken = CancellationToken.None;

	try
	{
		while (webSocket.State == WebSocketState.Open)
		{
			// Ŭ���̾�Ʈ���� �޽����� ����
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", cancellationToken);
				Console.WriteLine("Client disconnected.");
				return;
			}

			// ���� ������ UTF-8 ���ڿ� ��ȯ
			var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
			Console.WriteLine($"Received: {message}");

			// Ŭ���̾�Ʈ���� ���� ����
			var response = Encoding.UTF8.GetBytes($"Echo: {message}");
			await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, cancellationToken);
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"WebSocket error: {ex.Message}");
	}
	finally
	{
		if (webSocket.State != WebSocketState.Closed)
			await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Unexpected error", cancellationToken);
	}
}


