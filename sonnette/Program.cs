
using Microsoft.Extensions.Configuration;
using sonnette.rasppi.Models;
using System.Device.Gpio;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace sonnette.rasppi;

public class Program {
    static string apiUri;
    static int pinTransistor;
    static int pinButton;
    static int sonnetteId;
    static int pinServoMotor;
    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
        
        // Build a config object, using env vars and JSON providers.
        IConfigurationBuilder builder = new ConfigurationBuilder();

        JsonConfigurationExtensions.AddJsonFile(builder, "appsettings.json");

        IConfiguration config = builder.Build();

        // Get values from the config given their key and their target type.
        apiUri = config.GetRequiredSection("apiUri").Value;
        pinTransistor = int.Parse(config.GetRequiredSection("pinTransistor").Value);
        pinButton = int.Parse(config.GetRequiredSection("pinButton").Value);
        sonnetteId = int.Parse(config.GetRequiredSection("sonnetteId").Value);
        pinServoMotor = int.Parse(config.GetRequiredSection("pinServoMotor").Value);

        GpioController controller = new GpioController(PinNumberingScheme.Board);

        controller.OpenPin(pinTransistor, PinMode.Output);
        controller.OpenPin(pinButton, PinMode.Input);

        //SERVO
        // LOW HELICE MEME SENS BOITIER
        controller.OpenPin(pinServoMotor, PinMode.Output);
        //controller.OpenPin(pinServoMotor, PinMode.Input);
        //Servo servo = new Servo(Device.CreatePwmPort(Device.Pins.D08), NamedServoConfigs.SG90);
        //Console.WriteLine(controller.Read(pinServoMotor));
        controller.Write(pinServoMotor, PinValue.High);

        PinValue button;
        PinValue prevButton = false;
        bool isOn = false;
        while (true) {
            button = controller.Read(pinButton);
            if (button == PinValue.Low && prevButton != button) {
                RunAsync().GetAwaiter().GetResult();
                isOn = !isOn;
                if (isOn) {
                    controller.Write(pinTransistor, PinValue.High);
                } else {
                    controller.Write(pinTransistor, PinValue.Low);
                }
            }
            prevButton= controller.Read(pinButton);
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
                Id = sonnetteId,
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
