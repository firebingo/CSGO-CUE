using CSGSI;
using System;
using System.Collections.Generic;

namespace CSGO_K70
{
    static class Core
    {
        public static InputHandler input = new InputHandler();
        public static CSGOAPI gameListener = new CSGOAPI();
        public static KeyLighter keyController = new KeyLighter();
        public static bool isRunning = false;
        public static string ip = "";
        public static bool startAPI = false;

        //Bomb variables
        public static bool bombPlanted = false;
        public static int maxBomb = 40;
        public static float currentBomb = 0;
        public static float flashTime = 0;
        public static float deadTime = 0;
        public static int bombTick = 0;
        public static List<float> bombTimes = new List<float>()
        { 2f, 1.5f, 1.5f, 1.5f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
            0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f};

        public static void mainUpdate(float dt)
        {
            if (startAPI)
            {
                gameListener.connectToCS(ip);
                startAPI = false;
            }
            if (keyController.keyboard != null)
            {
                keyController.Update(dt);
                keyController.keyboard.Update(true);
            }

        }

        /// <summary>
        /// Handle a new game state
        /// </summary>
        /// <param name="gs"></param>
        public static void handleGameState(GameState gs)
        {
            if (keyController.keyboard != null)
            {
                if (gs.Round.Bomb.ToString() != "Undefined" && !bombPlanted)
                {
                    currentBomb = maxBomb;
                    bombPlanted = true;
                    deadTime = bombTimes[0];
                }
                if (gs.Round.WinTeam.ToString() != "Undefined" || gs.Round.Bomb.ToString() == "Undefined")
                {
                    bombPlanted = false;
                    currentBomb = 0;
                    bombTick = 0;
                }
                keyController.handleGameState(gs);
            }
        }
    }
}
