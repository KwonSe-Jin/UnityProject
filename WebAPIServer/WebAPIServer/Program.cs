using Microsoft.EntityFrameworkCore;
using System;
using WebAPIServer.Handlers;
using WebAPIServer.Models;

var builder = WebApplication.CreateBuilder(args);

// DB 연결 설정 추가
builder.Services.AddDbContext<DataBaseContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseWebSockets();

app.UseMiddleware<WebSocketHandler>(); // WebSocket 미들웨어 추가

app.Run();
