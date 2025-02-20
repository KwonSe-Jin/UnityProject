using Google.FlatBuffers;
using System.Net.WebSockets;
using System.Text;
using FBProtocol;
using Newtonsoft.Json;

namespace WebAPIServer.Handlers
{
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

					//  텍스트 메시지 처리 (Ping 메시지 포함)
					if (result.MessageType == WebSocketMessageType.Text)
					{
						// 받은 데이터를 UTF-8 문자열로 변환
						var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
						Console.WriteLine($"Received Text: {message}");

						// 단순 텍스트 Ping 메시지 처리
						if (message == "Ping from Unity")
						{
							Console.WriteLine("Received a Ping message");
							continue;
						}

						// JSON 메시지 처리 (일반적인 텍스트 메시지용)
						try
						{
							var receivedData = JsonConvert.DeserializeObject<MessageData>(message);
							if (receivedData != null)
							{
								Console.WriteLine("Parsed JSON - Type: {receivedData.Type}, Content: {receivedData.Content}");

								// 클라이언트에게 보낼 응답 JSON 생성
								var responseData = new MessageData
								{
									Type = "Echo",
									Content = $"Received: {receivedData.Content}"
								};

								// JSON을 바이트 배열로 변환하여 전송
								string jsonResponse = JsonConvert.SerializeObject(responseData);
								var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
								await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("JSON Parsing Error: {ex.Message}");
						}
					}
					// FlatBuffers 기반 바이너리 메시지 처리
					else if (result.MessageType == WebSocketMessageType.Binary)
					{
						Console.WriteLine("Received Binary Data...");

						// FlatBuffer ByteBuffer로 변환하여 CS_MOVE_PACKET 파싱
						var byteBuffer = new ByteBuffer(buffer);
						var movePacket = CS_MOVE_PACKET.GetRootAsCS_MOVE_PACKET(byteBuffer);

						Console.WriteLine($"[Server] Received MovePacket: PlayerID={movePacket.PlayerId}, Pos=({movePacket.Position.Value.X}, {movePacket.Position.Value.Y}, {movePacket.Position.Value.Z})");

						// SC_MOVE_PACKET 생성 후 응답
						await SendMovePacketResponse(webSocket, movePacket);
					}
					else if (result.MessageType == WebSocketMessageType.Close)
					{
						Console.WriteLine("WebSocket connection closing...");
						await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("WebSocket communication error: {ex.Message}");
			}
		}


		private async Task SendMovePacketResponse(WebSocket webSocket, CS_MOVE_PACKET receivedPacket)
		{
			// FlatBuffer 빌더 생성
			var builder = new FlatBufferBuilder(1024);

			// 받은 CS_MOVE_PACKET 데이터를 기반으로 SC_MOVE_PACKET 생성
			var positionOffset = Vec3.CreateVec3(builder, receivedPacket.Position.Value.X, receivedPacket.Position.Value.Y, receivedPacket.Position.Value.Z);
			var movePacketOffset = SC_MOVE_PACKET.CreateSC_MOVE_PACKET(builder, receivedPacket.PlayerId, positionOffset);

			// 버퍼 완성
			builder.Finish(movePacketOffset.Value);

			// 직렬화된 바이트 배열 추출
			byte[] responsePacket = builder.SizedByteArray();

			// WebSocket으로 전송
			await webSocket.SendAsync(new ArraySegment<byte>(responsePacket), WebSocketMessageType.Binary, true, CancellationToken.None);
			Console.WriteLine("[Server] Sent SC_MOVE_PACKET: PlayerID={receivedPacket.PlayerId}, Pos=({receivedPacket.Position.Value.X}, {receivedPacket.Position.Value.Y}, {receivedPacket.Position.Value.Z})");
		}
	}

	// JSON 메시지 처리를 위한 클래스
	public class MessageData
	{
		public string Type { get; set; }
		public string Content { get; set; }
	}
}
