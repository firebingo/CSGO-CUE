using System;

namespace CSGO_K70
{
    class InputHandler
    {
        public bool isRunning = false;
        /// <summary>
        /// Handles input to the console.
        /// </summary>
        public void HandleInput()
        {
            //intro
            Console.WriteLine("CSGO Lighting for Corsair RGB Keyboard");
            Console.WriteLine("Use -h for help");

            //while the input thread is set to run
            do
            {
                //Read the input line and act based on it.
                string input = Console.ReadLine();
                switch (input.ToLower())
                {
                    //help
                    case "-h":
                        Console.WriteLine("-s - Asks for ip then starts game listener.");
                        Console.WriteLine("-c - Try to reinitilize keyboard connection.");
                        Console.WriteLine("-quit - Exits the application.");
                        break;
                    //Signal the csgo api core to start if it isin't already running.
                    case "-start":
                    case "-s":
                        if (Core.gameListener.gs1 != null && Core.gameListener.gs1.Running)
                            Console.WriteLine("Game Listener is already running");
                        else
                        {
                            //wait for ip input then signal the main thread to start the api.
                            Console.Write("Please enter ip: ");
                            Core.ip = Console.ReadLine();
                            Core.startAPI = true;
                        }
                        break;
                    //try to reinitilize the keyboard.
                    case "-c":
                        Console.WriteLine("Attempting to reinitilize keyboard connection.");
                        Core.keyController.Start();
                        break;
                    //quit the application.
                    case "-quit":
                    case "-q":
                        Core.isRunning = false;
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Input Given");
                        break;
                }
            }
            while (isRunning);
        }
    }
}
