using NewsAggregator;
using NewsAggregator.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<NewsFetcher>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("UnifiedNewsPlatform/1.0");
});
builder.Services.AddSingleton<RabbitMqProducer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
