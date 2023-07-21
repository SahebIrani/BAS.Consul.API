namespace BAS.Consul.API.Models;

public class ConsulDemoKey
{
    public bool IsEnabled { get; set; }

    public bool ShowMessage { get; set; }

    public string Message { get; set; } = default!;
}

public class DemoAppSettings
{
    public string Key1 { get; set; } = default!;
    public string Key2 { get; set; } = default!;
}