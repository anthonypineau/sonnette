﻿using System.Device.Gpio;

namespace sonnette.rasppi;

public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");
        GpioController controller = new GpioController(PinNumberingScheme.Board);

        controller.OpenPin(10, PinMode.Output);
        controller.OpenPin(9, PinMode.Input);

        while (true)
        {
            controller.Write(10, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(10, PinValue.High);
            Thread.Sleep(1000);
            Console.WriteLine(controller.Read(9));
        }
    }
}
