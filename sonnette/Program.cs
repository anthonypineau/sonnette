using System.Device.Gpio;

namespace sonnette.rasppi;

public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
        GpioController controller = new GpioController(PinNumberingScheme.Board);

        controller.OpenPin(10, PinMode.Output);
        controller.OpenPin(12, PinMode.Input);

        PinValue button;
        PinValue prevButton = false;
        bool isOn = false;

        while (true)
        {
            button = controller.Read(12);
            if (button == PinValue.Low && prevButton != button)
            {
                isOn = !isOn;
                if (isOn)
                {
                    controller.Write(10, PinValue.High);
                }
                else
                {
                    controller.Write(10, PinValue.Low);
                }
            }
            prevButton= controller.Read(12);
            Thread.Sleep(100);
        }
    }
}
