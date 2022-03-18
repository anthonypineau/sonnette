
using Microsoft.Extensions.Configuration;
using sonnette.rasppi.Models;
using System.Device.Gpio;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace sonnette.rasppi;

public class Program {
    static string apiUri;

    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
        
        // Build a config object, using env vars and JSON providers.
        IConfigurationBuilder builder = new ConfigurationBuilder();

        JsonConfigurationExtensions.AddJsonFile(builder, "appsettings.json");

        IConfiguration config = builder.Build();

        // Get values from the config given their key and their target type.
        apiUri = config.GetRequiredSection("apiUri").Value;
        
        GpioController controller = new GpioController(PinNumberingScheme.Board);

        controller.OpenPin(10, PinMode.Output);
        controller.OpenPin(12, PinMode.Input);

        PinValue button;
        PinValue prevButton = false;
        bool isOn = false;

        while (true) {
            button = controller.Read(12);
            if (button == PinValue.Low && prevButton != button) {
                RunAsync().GetAwaiter().GetResult();
                isOn = !isOn;
                if (isOn) {
                    controller.Write(10, PinValue.High);
                } else {
                    controller.Write(10, PinValue.Low);
                }
            }
            prevButton= controller.Read(12);
            Thread.Sleep(100);
        }
    }

    static async Task RunAsync() {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(apiUri);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try {
            Sonnette sonnette = new Sonnette {
                Id = 1,
                Date = DateTime.Now,
                TypeAppui = 1
            };

            var statusCode = await CreateProductAsync(sonnette, client);
            Console.WriteLine($"Status code returned {statusCode} ({(int)statusCode})");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static async Task<HttpStatusCode> CreateProductAsync(Sonnette sonnette, HttpClient client)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Notification", sonnette);

        return response.StatusCode;
    }
}
