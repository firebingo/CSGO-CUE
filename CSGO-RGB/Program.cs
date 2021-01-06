using System;
using System.Diagnostics;
using System.Threading;

namespace CSGO_K70
{
    class Program
    {
        static void Main()
        {
            //Tell the core that it is running
            Core.isRunning = true; 
            //the input handler is threaded so it can wait for input while
            // the csgo api and CUE keep running.
            InputHandler input = new InputHandler();
            Thread inputThread = new Thread(new ThreadStart(input.HandleInput));
            inputThread.Name = "Input Thread";
            input.isRunning = true;
            //start the input thread
            inputThread.Start();

            //initilize the CUE SDK
            Core.keyController.Start();

			//DT timer
			DateTime timerDate = new DateTime();
			float dt = 0.0f;

			//continue to run while the core is set to run.
			do
			{
				timerDate = DateTime.Now;
				Thread.Sleep(4);
				Core.MainUpdate(dt);
				dt = Convert.ToSingle(DateTime.Now.Subtract(timerDate).TotalMilliseconds / 1000);
			}
			while (Core.isRunning);

            //kill the input thread before the application closes.
            input.isRunning = false;

            if (Core.gameListener.gs1 != null)
                Core.gameListener.gs1.Stop();

            Environment.Exit(0);
        }
    }
}
