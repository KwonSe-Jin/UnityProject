using UnityEngine;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Google.FlatBuffers;
using FBProtocol;
using System;

public class NetWorkManager : MonoBehaviour
{
	private ClientWebSocket webSocket;
	private CancellationTokenSource cancellationTokenSource;
	private bool isConnected = false;

	async void Start()
	{
		webSocket = new ClientWebSocket();
		cancellationTokenSource = new CancellationTokenSource();
		var serverUri = new System.Uri("wss://localhost:7187/ws");

		try
		{
			// 서버와 WebSocket 연결
			await webSocket.ConnectAsync(serverUri, CancellationToken.None);
			isConnected = true;
			Debug.Log("Connected to WebSocket server!");

			// 서버에 연결 메시지 전송
			await SendMessage("Connect");

			// 지속적으로 메시지 수신
			_ = Task.Run(() => ReceiveMessages(), cancellationTokenSource.Token);
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"WebSocket Error: {ex.Message}");
		}
	}

	// 메시지 보내기 (언제든 호출 가능)


	public async void SendPlayerPosition(int playerId, Vector3 position)
	{
		if (!isConnected || webSocket.State != WebSocketState.Open)
		{
			Debug.LogWarning("WebSocket is not connected!");
			return;
		}

		// FlatBuffer 빌더 생성
		var builder = new FlatBufferBuilder(1024);

		// VEC3 구조체 생성 (FlatBuffers는 struct를 먼저 만들어야 함)
		var positionOffset = Vec3.CreateVec3(builder, position.x, position.y, position.z);
		var movePacketOffset = CS_MOVE_PACKET.CreateCS_MOVE_PACKET(builder, playerId, positionOffset);

		// 버퍼 완성
		builder.Finish(movePacketOffset.Value);

		// 직렬화된 바이트 배열 추출
		byte[] packetData = builder.SizedByteArray();

		// WebSocket으로 전송
		await webSocket.SendAsync(new ArraySegment<byte>(packetData), WebSocketMessageType.Binary, true, CancellationToken.None);
		Debug.Log("Sent MovePacket: PlayerID={playerId}, Pos=({position.x}, {position.y}, {position.z})");
	}


	public async Task SendMessage(string message)
	{
		if (webSocket == null || webSocket.State != WebSocketState.Open)
		{
			Debug.LogWarning("WebSocket is not connected!");
			return;
		}

		var messageBytes = Encoding.UTF8.GetBytes(message);
		await webSocket.SendAsync(new System.ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
		Debug.Log($"Sent: {message}");
	}


	private async Task ReceiveMessages()
	{
		var buffer = new byte[1024 * 4];

		while (isConnected && webSocket.State == WebSocketState.Open)
		{
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			if (result.MessageType == WebSocketMessageType.Close)
			{
				Debug.Log("WebSocket connection closed.");
				isConnected = false;
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
				break;
			}

			// 수신 데이터가 Binary일 경우 FlatBuffers 역직렬화 처리
			if (result.MessageType == WebSocketMessageType.Binary)
			{
				var byteBuffer = new ByteBuffer(buffer);
				var movePacket = SC_MOVE_PACKET.GetRootAsSC_MOVE_PACKET(byteBuffer);

				var position = movePacket.Position.Value;

				Debug.Log($"Received MovePacket: PlayerID={movePacket.PlayerId}, Pos=({position.X}, {position.Y}, {position.Z})");
			}
			else // 텍스트 메시지 처리
			{
				var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
				Debug.Log($"Received Text: {receivedMessage}");
			}
		}
	}

	// 일정 주기마다 메시지 보내기 (Update 사용)
	private float sendInterval = 3.0f; // 3초마다 메시지 전송
	private float nextSendTime = 0f;

	void Update()
	{
		if (isConnected && Time.time >= nextSendTime)
		{
			nextSendTime = Time.time + sendInterval;
			_ = SendMessage("Ping from Unity");
		}
	}

	private void OnApplicationQuit()
	{
		isConnected = false;
		cancellationTokenSource?.Cancel();
		webSocket?.Dispose();
	}
}


