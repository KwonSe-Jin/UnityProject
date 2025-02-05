using System.Net.WebSockets;
using System.Text;

public class WebSocketHandler
{
	private readonly RequestDelegate _next;

	public WebSocketHandler(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		Console.WriteLine("Checking WebSocket request...");
		if (context.WebSockets.IsWebSocketRequest)
		{
			Console.WriteLine("WebSocket request received.");
			using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
			await HandleWebSocketCommunication(webSocket);
		}
		else
		{
			Console.WriteLine("Not a WebSocket request. Passing to next middleware.");
			await _next(context); // WebSocket이 아닌 요청은 다음 미들웨어로 넘기기
		}
	}

	private async Task HandleWebSocketCommunication(WebSocket webSocket)
	{
		var buffer = new byte[1024 * 4];
		try
		{
			while (webSocket.State == WebSocketState.Open)
			{
				var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Text)
				{
					var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					Console.WriteLine($"Received: {message}");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error during WebSocket communication: {ex.Message}");
		}
	}
}
