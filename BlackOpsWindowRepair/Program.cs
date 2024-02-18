using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BlackOpsWindowRepair
{
    internal class Program
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static void Main()
        {
            Dictionary<string, string> supportedGames = new Dictionary<string, string>
            {
                { "1", "BlackOps" },
                { "2", "t6zm" }
            };

            Console.WriteLine("Please enter your game id: ");
            Console.WriteLine("1 for Black Ops 1");
            Console.WriteLine("2 for Black Ops 2");

            string gameId;
            while (true)
            {
                gameId = Console.ReadLine();
                if (gameId == "1" || gameId == "2")
                {
                    Console.WriteLine("{0} selected!", gameId == "1" ? "Black Ops" : "Black Ops 2");
                    break;
                }

                Console.WriteLine("Please enter a valid ID!");
            }

            Process[] processes = Process.GetProcessesByName(supportedGames[gameId]);
            Process? blackOpsProcess = processes.FirstOrDefault();

            if (blackOpsProcess == null)
            {
                Console.WriteLine("Unable to find BlackOps process!");
                Environment.Exit(1);
            }

            // get interaction key
            Console.WriteLine("Please enter your interaction key for repairing windows:");
            string interactionKey = Console.ReadLine();
            if (interactionKey.Length > 1)
            {
                Console.WriteLine("Interaction key cannot be multiple letters: {0}", interactionKey);
                Environment.Exit(2);
            }

            int blackOpsPid = blackOpsProcess.Id;
            IntPtr h = blackOpsProcess.MainWindowHandle;
            SetForegroundWindow(h);
            Thread.Sleep(2000);

            Console.WriteLine("Starting to send keys...");

            while (true)
            {
                IntPtr hWnd = GetForegroundWindow();
                GetWindowThreadProcessId(hWnd, out uint processID);
                if (processID != blackOpsPid)
                {
                    continue;
                }

                Thread.Sleep(20);
                SendKeys.SendWait(interactionKey);
            }
        }
    }
}
