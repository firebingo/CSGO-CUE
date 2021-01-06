using CSGSI;
using CSGSI.Nodes;
using System;
using System.Collections.Generic;

namespace CSGO_K70
{
    static class Core
    {
        public static CSGOAPI gameListener = new CSGOAPI();
        public static KeyLighter keyController = new KeyLighter();
        public static bool isRunning = false;
        public static string ip = "";
        public static bool startAPI = false; //makes sure the CSGO connection is only started once

        public static bool isT = false;  //whether or not the player is a terrorist.
        public static bool inGame = false; //whether or not the player is actually in a game on a team.

        //Bomb variables
        public static bool bombPlanted = false; //Whether or not the bomb has been planted.
        public const int maxBomb = 40; //The amount of time a bomb starts with. Currently 40s in CSGO.
        public static float currentBomb = 0; //The current time left on the bomb.
        public static float flashTime = 0; //A timer for how long the keys should stay lit for bomb ticking.
        public const float flashMax = 0.2f; //How long the keys should stay lit.
        public static float deadTime = 0; //A timer for how long the keys should stay unlit for bomb ticking.
        public static int bombTick = 0; //the current index to pull from bombTimes.
        //a list of dead times for the bomb ticking.
        public static List<float> bombTimes = new List<float>()
         //36
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
        1f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f };

        //Ammo variables
        public static int primaryReserve = 0;
        public static int primaryMax = 0;
        public static int primaryCurrent = 0;
        public static int secondaryReserve = 0;
        public static int secondaryMax = 0;
        public static int secondaryCurrent = 0;
        public static int grenadeCount = 0;
        public static bool hasC4 = false;

        //Health variables
        public const int healthMax = 100;
        public const int armorMax = 100;
        public static int healthCurrent = 0;
        public static int armorCurrent = 0;
        public static bool hasHelmet = false;

        //Flash and Fire variables
        public static int flashAmount = 0;
        public static int burningAmount = 0;

        /// <summary>
        /// The main loop update function.
        /// </summary>
        /// <param name="dt"></param>
        public static void MainUpdate(float dt)
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
        public static void HandleGameState(GameState gs)
        {
            if (keyController.keyboard != null)
            {
                //Find what team the player is on.
                if (gs.Player.Team.ToString() != "Undefined")
                {
                    if (gs.Player.Team.ToString() == "T")
                        isT = true;
                    else if (gs.Player.Team.ToString() == "CT")
                        isT = false;
                    inGame = true;
                }
                else
                {
                    inGame = false;
                }
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
                //Weapon and ammo counts.
                //reset these two first.
                hasC4 = false;
                grenadeCount = 0;
                bool hasPrimary = false;
                bool hasSecondary = false;
                for (int i = 0; i < gs.Player.Weapons.Count; ++i)
                {
                    if (gs.Player.Weapons[i].Type.ToString() != "Undefined")
                    {

                        switch (gs.Player.Weapons[i].Type.ToString())
                        {
                            case ("Rifle"):
                            case ("SubmachineGun"):
                            case ("Shotgun"):
                            case ("SniperRifle"):
                            case ("MachineGun"):
                                primaryMax = gs.Player.Weapons[i].AmmoClipMax;
                                primaryCurrent = gs.Player.Weapons[i].AmmoClip;
                                primaryReserve = gs.Player.Weapons[i].AmmoReserve;
                                hasPrimary = true;
                                break;
                            case ("Pistol"):
                                secondaryMax = gs.Player.Weapons[i].AmmoClipMax;
                                secondaryCurrent = gs.Player.Weapons[i].AmmoClip;
                                secondaryReserve = gs.Player.Weapons[i].AmmoReserve;
                                hasSecondary = true;
                                break;
                            case ("Grenade"):
                                ++grenadeCount;
                                break;
                            case ("C4"):
                                hasC4 = true;
                                break;
                            default:
                                break;
                        }

                    }
                }
                if (!hasPrimary)
                {
                    primaryCurrent = 0;
                    primaryReserve = 0;
                }
                if(!hasSecondary)
                {
                    secondaryCurrent = 0;
                    secondaryReserve = 0;
                }
                //Health and Armor status.
                hasHelmet = false;
                healthCurrent = gs.Player.State.Health;
                armorCurrent = gs.Player.State.Armor;
                hasHelmet = gs.Player.State.Helmet;
                flashAmount = gs.Player.State.Flashed;
                burningAmount = gs.Player.State.Burning;

                keyController.HandleGameState();
            }
        }

		public static RGB.NET.Core.Color SysColorToRGBColor(System.Drawing.Color c)
		{
			return new RGB.NET.Core.Color(c.A, c.R, c.G, c.B);
		}
    }
}
