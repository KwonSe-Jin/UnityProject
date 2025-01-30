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

	// 지속적으로 메시지 수신
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
