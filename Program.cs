using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace MyClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            string exit = "";
            while (exit != "f")
            {
                Console.WriteLine("Press u to use code or g to generate codes: ");
                string input = Console.ReadLine();
                if (input == "u")
                {
                    UseCode();
                    Console.WriteLine("To comeback to use code or generate codes. ");
                    exit = WriteFToExit();
                }
                else if (input == "g")
                {
                    GenerateCode();
                    Console.WriteLine("To comeback to use code or generate codes. ");
                    exit = WriteFToExit();
                }
                else
                {
                    continue;
                }
            }
        }

        private static void UseCode()
        {
            // creating a WS client, which will be properly disposed
            using (WebSocket ws = new WebSocket("ws://127.0.0.1:80/Use"))
            {
                ws.Connect();

                if (ws.IsAlive)
                {
                    ws.OnMessage += Ws_OnMessage;

                    string exit = "";
                    while (exit != "f")
                    {
                        Console.WriteLine("Write your code: ");
                        var input = Console.ReadLine();
                        ws.Send(input);
                        exit = WriteFToExit();
                    }
                }
                else
                {
                    Console.WriteLine("No connection established.");
                }
            }
        }

        private static void GenerateCode()
        {
            // creating a WS client, which will be properly disposed
            using (WebSocket ws = new WebSocket("ws://127.0.0.1:80/Generate"))
            {
                ws.Connect();

                if (ws.IsAlive)
                {
                    ws.OnMessage += Ws_OnMessage;

                    ushort count;
                    Console.WriteLine("Write number of codes: ");
                    var nrCodes = Console.ReadLine();
                    while (!ushort.TryParse(nrCodes, out count))
                    {
                        Console.WriteLine("Write number of codes: ");
                        nrCodes = Console.ReadLine();
                    }

                    byte length;
                    Console.WriteLine("Write length of codes: ");
                    var lengthCode = Console.ReadLine();
                    while (!byte.TryParse(lengthCode, out length))
                    {
                        Console.WriteLine("Write length of codes: ");
                        lengthCode = Console.ReadLine();
                    }

                    byte[] countArray = BitConverter.GetBytes(count);
                    byte[] lengthArray = new byte[1] { length };

                    byte[] generateArray = new byte[countArray.Length + lengthArray.Length];
                    System.Buffer.BlockCopy(countArray, 0, generateArray, 0, countArray.Length);
                    System.Buffer.BlockCopy(lengthArray, 0, generateArray, countArray.Length, lengthArray.Length);

                    ws.Send(generateArray);

                    Thread.Sleep(100);
                    Console.WriteLine("Write any key to continue: ");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("No connection established.");
                }                
            }
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            if(e.RawData[0] == 0)
            {
                Console.WriteLine("Codes generated successfully.");
            }
            else if(e.RawData[0] == 1)
            {
                Console.WriteLine("Discount applies");
            }
            else if (e.RawData[0] == 2)
            {
                Console.WriteLine("There is no discount for the code entered.");
            }
        }

        private static string WriteFToExit()
        {
            Thread.Sleep(100);
            Console.WriteLine("Write f to exit or any key to try again: ");
            return Console.ReadLine();
        }
    }
}
