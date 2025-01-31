﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Newtonsoft.Json; // JSON 파싱을 위한 라이브러리 추
using TestWebSocket;
var builder = WebApplication.CreateBuilder(args);


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
app.Run(); // 애플리케이션 실행

// WebSocket 메시지 수신 및 처리
async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4]; // 수신할 메시지 버퍼
	var cancellationToken = CancellationToken.None;

	try
	{
		while (webSocket.State == WebSocketState.Open)
		{
			// 클라이언트에서 메시지를 수신
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", cancellationToken);
				Console.WriteLine("Client disconnected.");
				return;
			}

			// 받은 데이터 UTF-8 문자열 변환
			var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
			Console.WriteLine($"Received: {message}");

			// JSON 파싱 시도
			try
			{
				var inputData = JsonConvert.DeserializeObject<PlayerInputData>(message);
				Console.WriteLine($"Player {inputData.playerID} moved to X: {inputData.x}, Y: {inputData.y}");

				// 서버에서 클라이언트에게 이동 데이터 응답 (예: 동기화용)
				var responseMessage = $"Player {inputData.playerID} moved to ({inputData.x}, {inputData.y})";
				var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
				await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, cancellationToken);
			}
			catch (Newtonsoft.Json.JsonException)
			{
				Console.WriteLine("Invalid JSON format received.");
			}
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
