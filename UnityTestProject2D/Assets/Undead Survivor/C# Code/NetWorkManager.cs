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
			// ������ WebSocket ����
			await webSocket.ConnectAsync(serverUri, CancellationToken.None);
			isConnected = true;
			Debug.Log("Connected to WebSocket server!");

			// ������ ���� �޽��� ����
			await SendMessage("Connect");

			// ���������� �޽��� ����
			_ = Task.Run(() => ReceiveMessages(), cancellationTokenSource.Token);
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"WebSocket Error: {ex.Message}");
		}
	}

	// �޽��� ������ (������ ȣ�� ����)


	public async void SendPlayerPosition(int playerId, Vector3 position)
	{
		if (!isConnected || webSocket.State != WebSocketState.Open)
		{
			Debug.LogWarning("WebSocket is not connected!");
			return;
		}

		// FlatBuffer ���� ����
		var builder = new FlatBufferBuilder(1024);

		// VEC3 ����ü ���� (FlatBuffers�� struct�� ���� ������ ��)
		var positionOffset = Vec3.CreateVec3(builder, position.x, position.y, position.z);
		var movePacketOffset = CS_MOVE_PACKET.CreateCS_MOVE_PACKET(builder, playerId, positionOffset);

		// ���� �ϼ�
		builder.Finish(movePacketOffset.Value);

		// ����ȭ�� ����Ʈ �迭 ����
		byte[] packetData = builder.SizedByteArray();

		// WebSocket���� ����
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

			// ���� �����Ͱ� Binary�� ��� FlatBuffers ������ȭ ó��
			if (result.MessageType == WebSocketMessageType.Binary)
			{
				var byteBuffer = new ByteBuffer(buffer);
				var movePacket = SC_MOVE_PACKET.GetRootAsSC_MOVE_PACKET(byteBuffer);

				var position = movePacket.Position.Value;

				Debug.Log($"Received MovePacket: PlayerID={movePacket.PlayerId}, Pos=({position.X}, {position.Y}, {position.Z})");
			}
			else // �ؽ�Ʈ �޽��� ó��
			{
				var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
				Debug.Log($"Received Text: {receivedMessage}");
			}
		}
	}

	// ���� �ֱ⸶�� �޽��� ������ (Update ���)
	private float sendInterval = 3.0f; // 3�ʸ��� �޽��� ����
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


