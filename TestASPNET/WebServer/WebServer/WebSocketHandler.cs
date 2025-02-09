using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

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
			await _next(context);
		}
	}

	// WebSocket 통신을 처리하는 메서드
	private async Task HandleWebSocketCommunication(WebSocket webSocket)
	{
		var buffer = new byte[1024 * 4]; // 4KB 버퍼 생성

		try
		{
			// WebSocket 연결이 열려 있는 동안 메시지 수신 대기
			while (webSocket.State == WebSocketState.Open)
			{
				// 클라이언트로부터 메시지 수신
				var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				// 텍스트 메시지를 받은 경우
				if (result.MessageType == WebSocketMessageType.Text)
				{
					// 받은 데이터를 UTF-8 문자열로 변환
					var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					Console.WriteLine($"Received: {message}");

					// 메시지가 "Ping from Unity"처럼 단순 텍스트인 경우
					if (message == "Ping from Unity")
					{
						Console.WriteLine("Received a Ping message");
						// 단순 텍스트 메시지는 별도로 처리
					}
					else
					{
						try
						{
							// JSON 데이터를 객체로 변환
							var receivedData = JsonConvert.DeserializeObject<MessageData>(message);
							if (receivedData != null)
							{
								Console.WriteLine($"Parsed JSON - Type: {receivedData.Type}, Content: {receivedData.Content}");

								// 클라이언트에게 보낼 응답 JSON 생성
								var responseData = new MessageData
								{
									Type = "Echo",
									Content = $"Received: {receivedData.Content}"
								};

								// 객체를 JSON 문자열로 변환
								string jsonResponse = JsonConvert.SerializeObject(responseData);

								// JSON 데이터를 바이트 배열로 변환
								var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

								// 클라이언트에게 메시지 전송
								await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine($"JSON Parsing Error: {ex.Message}");
						}
					}
				}
				// 클라이언트가 연결을 종료하려는 경우
				else if (result.MessageType == WebSocketMessageType.Close)
				{
					Console.WriteLine("WebSocket connection closing...");
					await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
				}
			}
		}
		catch (Exception ex)
		{
			// WebSocket 통신 중 오류 발생 시 콘솔에 출력
			Console.WriteLine($"Error during WebSocket communication: {ex.Message}");
		}
	}
}

// JSON 데이터를 담을 클래스
public class MessageData
{
	public string Type { get; set; }
	public string Content { get; set; }
}
