using UnityEngine;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;

public class NetWorkManager : MonoBehaviour
{
	private ClientWebSocket webSocket;
	private CancellationTokenSource cancellationTokenSource;
	private bool isConnected = false;

	async void Start()
	{
		webSocket = new ClientWebSocket();
		cancellationTokenSource = new CancellationTokenSource();
		var serverUri = new System.Uri("wss://localhost:5001/ws");

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

	// ���������� �޽��� ����
	private async Task ReceiveMessages()
	{
		var buffer = new byte[1024 * 4];

		while (isConnected && webSocket.State == WebSocketState.Open)
		{
			var result = await webSocket.ReceiveAsync(new System.ArraySegment<byte>(buffer), CancellationToken.None);

			if (result.MessageType == WebSocketMessageType.Close)
			{
				Debug.Log("WebSocket connection closed.");
				isConnected = false;
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
				break;
			}

			var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
			Debug.Log($"Received: {receivedMessage}");
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
