using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using WebAPIServer.Handlers;
using WebAPIServer.Models;
using WebAPIServer.Services;

var builder = WebApplication.CreateBuilder(args);

// DB 연결 설정 추가
builder.Services.AddDbContext<DataBaseContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

// jwt 설정 불러오기 
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// JWT 인증 미들웨어 설정
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false; // 로컬 개발용 (HTTPS 미사용)
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
		ClockSkew = TimeSpan.Zero // 토큰 만료 시간 정확하게 설정
	};
});

// CORS 설정 (모든 요청 허용)
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

// 서비스 등록
builder.Services.AddSingleton<RedisService>(); // Redis 서비스
builder.Services.AddSingleton<MatchingRedisService>(); // Redis 서비스
builder.Services.AddScoped<JwtService>(); // JWT 토큰 서비스
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");  // CORS 활성화
app.UseAuthentication();  // JWT 인증 활성화
app.UseAuthorization();
app.MapControllers();

app.UseWebSockets();

app.UseMiddleware<WebSocketHandler>(); // WebSocket 미들웨어 추가

app.Run();
