using UnityEngine;
using System.Text;
using System.Threading;
using System.Net.WebSockets;

public class NetWorkManager : MonoBehaviour
{
	private ClientWebSocket webSocket;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	async void Start()
    {
		webSocket = new ClientWebSocket();
		var serverUri = new System.Uri("wss://localhost:5001/ws");

		try
		{
			// 서버와 WebSocket 연결
			await webSocket.ConnectAsync(serverUri, CancellationToken.None);
			Debug.Log("Connected to WebSocket server!");

			// 메시지 전송
			await SendMessage("Connect");

			// 메시지 수신
			await ReceiveMessages();
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"WebSocket Error: {ex.Message}");
		}
	}

	async System.Threading.Tasks.Task SendMessage(string message)
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

	async System.Threading.Tasks.Task ReceiveMessages()
	{
		var buffer = new byte[1024 * 4];

		while (webSocket.State == WebSocketState.Open)
		{
			var result = await webSocket.ReceiveAsync(new System.ArraySegment<byte>(buffer), CancellationToken.None);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				Debug.Log("WebSocket connection closed.");
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
				break;
			}

			var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
			Debug.Log($"Received: {receivedMessage}");
		}
	}

	private void OnApplicationQuit()
	{
		if (webSocket != null)
		{
			webSocket.Dispose();
		}
	}
}

