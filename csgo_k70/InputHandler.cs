using System;

namespace CSGO_K70
{
    class InputHandler
    {
        public bool isRunning = false;
        /// <summary>
        /// Handles input to the console.
        /// </summary>
        public void handleInput()
        {
            string input = "";

            //intro
            Console.WriteLine("CSGO Lighting for Corsair RGB Keyboard");
            Console.WriteLine("Use -h for help");

            //while the input thread is set to run
            do
            {
                //Read the input line and act based on it.
                input = Console.ReadLine();
                switch (input.ToLower())
                {
                    //help
                    case "-h":
                        Console.WriteLine("-s - Asks for ip then starts game listener.");
                        Console.WriteLine("-quit - Exits the application.");
                        break;
                    //Start the csgo api core if it isint already running.
                    case "-s":
                        if (Core.gameListener.gs1 != null && Core.gameListener.gs1.Running)
                            Console.Write("Game Listener is already running");
                        else
                        {
                            //wait for ip input then signal the main thread to start the api.
                            Console.Write("Please enter ip: ");
                            Core.ip = Console.ReadLine();
                            Core.startAPI = true;
                        }
                        break;
                    case "-quit":
                        Core.isRunning = false;
                        isRunning = false;
                        break;
                    default:
                        Console.Write("Invalid Input Given");
                        break;
                }
            }
            while (isRunning);
        }
    }
}
