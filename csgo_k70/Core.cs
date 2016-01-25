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
        public static bool startAPI = false; //makes sure the CSGO connection is only started once

        //Bomb variables
        public static bool bombPlanted = false; //Whether or not the bomb has been planted.
        public const int maxBomb = 40; //The amount of time a bomb starts with. Currently 40s in CSGO.
        public static float currentBomb = 0; //The current time left on the bomb.
        public static float flashTime = 0; //A timer for how long the keys should stay lit for bomb ticking.
        public const float flashMax = 0.25f; //How long the keys should stay lit.
        public static float deadTime = 0; //A timer for how long the keys should stay unlit for bomb ticking.
        public static int bombTick = 0; //the current index to pull from bombTimes.
        //a list of dead times for the bomb ticking.
        public static List<float> bombTimes = new List<float>()
        { 2f, 1.5f, 1.5f, 1.5f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
            0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f};

        /// <summary>
        /// The main loop update function.
        /// </summary>
        /// <param name="dt"></param>
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
                keyController.keyboard.Update(true); //updates the keyboard leds.
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
                //if the new game state says the bomb is planted and the application thinks the bomb isin't
                // planted, start the bomb timer and set bombPlanted.
                if (gs.Round.Bomb.ToString() != "Undefined" && !bombPlanted)
                {
                    currentBomb = maxBomb;
                    bombPlanted = true;
                    deadTime = bombTimes[0];
                }
                //if the new game state says the round has been won and the bomb state is uundefined reset
                // the bomb related variables
                if (gs.Round.WinTeam.ToString() != "Undefined" || gs.Round.Bomb.ToString() == "Undefined")
                {
                    bombPlanted = false;
                    deadTime = 0;
                    flashTime = 0;
                    currentBomb = 0;
                    bombTick = 0;
                }
                keyController.handleGameState(gs);
            }
        }
    }
}
