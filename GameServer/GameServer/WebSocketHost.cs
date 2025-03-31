using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace GameServer
{
	public class WebSocketHost
	{
		public static async Task Start(RoomManager roomManager)
		{
			Console.WriteLine("서버 시작");
			var builder = WebApplication.CreateBuilder();
			var app = builder.Build();

			app.UseWebSockets();

			app.Map("/ws", async context =>
			{
				var socket = await context.WebSockets.AcceptWebSocketAsync();
				var handler = new GameWebSocketHandler(roomManager);
				await handler.HandleConnection(context, socket);
			});

			await app.RunAsync("http://0.0.0.0:9000"); // WebSocket 수신 포트
		}
	}
}
