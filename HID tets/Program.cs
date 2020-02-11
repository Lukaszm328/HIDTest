using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace HID_tets
{
    class Program
    {
        private static System.Timers.Timer aTimer;
        public static HidStream _stream;
        public const int timerTimeMs = 500;

        static void Main(string[] args)
        {
            int vendorID = 0x0483;
            int productID = 0x5750;

            Console.WriteLine("*******************************************************");
            Console.WriteLine("*                                                     *");
            Console.WriteLine("*          Pointer3D communication HID                *");
            Console.WriteLine("*                                                     *");
            Console.WriteLine("*******************************************************");
            Console.WriteLine();

            var loader = new HidDeviceLoader();

            var device = loader.GetDevices(vendorID, productID).FirstOrDefault();

            if (device != null)
            {
                Console.WriteLine($"Device: Found device { device.GetFriendlyName() } VID: " + vendorID + " PID: " + productID);
                Console.WriteLine();

                HidStream stream;

                Console.WriteLine(@" - Create HidStream");

                device.TryOpen(out stream);

                _stream = stream;

                Console.WriteLine(@" - Open port HID");

                ShowMenu();
            }
            else
            {
                Console.WriteLine("Device: Don't found device Pionter3D");
                Console.WriteLine("Press ENTER to retry");
            }
            Console.ReadLine();
        }

        public static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("------------------------------");
            Console.WriteLine("- Menu:                      -");
            Console.WriteLine("- 1. Read data in loop 500ms -");
            Console.WriteLine("- 2. Write and read          -");
            Console.WriteLine("------------------------------");
            Console.Write("Option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    ReadDataInLoop();
                    return;
                case "2":
                    WriteAndRead();
                    return;
                default:
                    return;
            }
        }

        public static void ReadDataInLoop()
        {
            Console.WriteLine();
            Console.WriteLine(" Response (" + timerTimeMs + "ms): ");

            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(timerTimeMs);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Console.ReadLine();

            //while (Console.ReadKey().Key != ConsoleKey.E)
            //{
            //    if (Console.ReadKey().Key == ConsoleKey.E)
            //    {
            //        aTimer.Enabled = false;

            //        ShowMenu();
            //    }
            //}
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                _stream.ReadTimeout = 2000;
                var res = _stream.Read();
                var response = Encoding.ASCII.GetString(res);

                Console.WriteLine($"{response} {response.Length}");
                Console.WriteLine("------------------------- PRESS ESCAPE TO STOP-------------------------");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't read data: " + ex.ToString());
                Console.WriteLine("------------------------- PRESS ESCAPE TO STOP-------------------------");
            }

            Console.ReadLine();
        }


        public static void WriteAndRead()
        {
            Console.WriteLine();
            Console.Write("Data to write: ");

            try
            {
                _stream.ReadTimeout = 2000;

                _stream.Write(Encoding.ASCII.GetBytes(Console.ReadLine()));

                var res = _stream.Read();

                var response = Encoding.ASCII.GetString(res);

                Console.WriteLine($"Response: {response}");
                Console.WriteLine($"Response lenght: {response.Length}");
                ShowMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't read data: " + ex.ToString());
            }
            Console.ReadLine();
        }

        //byte[] tab1 = { res[1], res[2], res[3], res[4] };

        //var a = BitConverter.ToSingle(tab1, 0);
    }
}
