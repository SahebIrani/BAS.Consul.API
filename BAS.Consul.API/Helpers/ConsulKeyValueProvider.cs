using System.Text;
using System.Text.Json;

using Consul;

namespace BAS.Consul.API.Helpers;

public static class ConsulKeyValueProvider
{
    public static async Task<T?> GetValueAsync<T>(string key)
    {
        using var client = new ConsulClient();

        var getPair = await client.KV.Get(key);

        if (getPair?.Response == null)
        {
            return default;
        }

        var value = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);

        return JsonSerializer.Deserialize<T>(value);
    }
}