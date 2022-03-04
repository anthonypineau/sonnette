using System.Device.Gpio;

namespace sonnette.rasppi;

public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
        GpioController controller = new GpioController(PinNumberingScheme.Board);

        controller.OpenPin(10, PinMode.Output);
        controller.OpenPin(12, PinMode.Input);

        PinValue prevPinValue = PinValue.Low;
        bool isOn = false;

        while (true)
        {
            PinValue button = controller.Read(12);
            if (prevPinValue == PinValue.Low && button == PinValue.High)
                isOn = !isOn;
            prevPinValue = button;
        }
    }
}
