using ContentProcessor.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<MongoRepository>();
builder.Services.AddSingleton<RedisPublisher>();
builder.Services.AddTransient<ProcessorService>();
builder.Services.AddHostedService<RabbitMqConsumer>();

var host = builder.Build();
host.Run();
