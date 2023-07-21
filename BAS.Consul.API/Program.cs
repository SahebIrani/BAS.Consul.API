using System.Text;

using BAS.Consul.API.Extensions;
using BAS.Consul.API.Models;

using Consul;

using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);

var consulResult = await HelloConsul();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var consulHost = "http://localhost:8500";
builder.Services.AddConsulConfig(configKey: consulHost);

builder.Services.Configure<DemoAppSettings>(builder.Configuration.GetSection(nameof(DemoAppSettings)));
builder.Services.AddConsul(builder.Configuration);

{
    builder.Configuration.AddEnvironmentVariables();
    var environment = builder.Environment.EnvironmentName;
    string key = Environment.GetEnvironmentVariable("ApplicationName") + "/appsettings." + environment + ".json";
    var key2 = "App1/appsettings.json";
    var cancellationTokenSource = new CancellationTokenSource();
    var address = builder.Configuration["Consul:Host"];

    builder.Configuration.AddConsul(key, options =>
    {
        //Configure Consul Connection Details, i.e. Address, DataCenter, Certificates and Auth details
        options.ConsulConfigurationOptions = cco =>
        {
            //cco.Address = new Uri(Environment.GetEnvironmentVariable("ConsulURL")!);
            cco.Address = new Uri(address);
        };

        //Making Configuration either optional or not
        options.Optional = true;

        //Wait Time before pulling an change from Consul
        options.PollWaitTime = TimeSpan.FromSeconds(5);

        //Whether Reload the Configuration if any changes are detected
        options.ReloadOnChange = true;

        //What action to perform if On Load Fails
        options.OnLoadException = (consulLoadExceptionContext) =>
        {
            Console.WriteLine(
                $"Error onLoadException {consulLoadExceptionContext.Exception.Message}" +
                $" and stacktrace {consulLoadExceptionContext.Exception.StackTrace}")
            ;

            consulLoadExceptionContext.Ignore = true;

            throw consulLoadExceptionContext.Exception;
        };

        //What action to perform if Watching Changes failed
        options.OnWatchException = (consulWatchExceptionContext) =>
        {
            Console.WriteLine($"Unable to watchChanges in Consul due to {consulWatchExceptionContext.Exception.Message}");
            return TimeSpan.FromSeconds(2);
        };
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static async ValueTask<string> HelloConsul()
{
    using var client = new ConsulClient();

    var putPair = new KVPair("hello")
    {
        Value = Encoding.UTF8.GetBytes("Hello Consul")
    };

    var putAttempt = await client.KV.Put(putPair);

    if (putAttempt.Response)
    {
        var getPair = await client.KV.Get("hello");

        var data =
            Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);

        return data;
    }

    return "";
}