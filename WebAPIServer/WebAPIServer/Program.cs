using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using WebAPIServer.Handlers;
using WebAPIServer.Models;
using WebAPIServer.Services;

var builder = WebApplication.CreateBuilder(args);

// DB ���� ���� �߰�
builder.Services.AddDbContext<DataBaseContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

// jwt ���� �ҷ����� 
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// JWT ���� �̵���� ����
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false; // ���� ���߿� (HTTPS �̻��)
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = true,
		ValidIssuer = jwtSettings["Issuer"],
		ValidateAudience = true,
		ValidAudience = jwtSettings["Audience"],
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero // ��ū ���� �ð� ��Ȯ�ϰ� ����
	};
});

// CORS ���� (��� ��û ���)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		});
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ���� ���
builder.Services.AddSingleton<RedisService>(); // Redis ����
builder.Services.AddSingleton<MatchingRedisService>(); // Redis ����
builder.Services.AddScoped<JwtService>(); // JWT ��ū ����
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");  // CORS Ȱ��ȭ
app.UseAuthentication();  // JWT ���� Ȱ��ȭ
app.UseAuthorization();
app.MapControllers();

app.UseWebSockets();

app.UseMiddleware<WebSocketHandler>(); // WebSocket �̵���� �߰�

app.Run();
