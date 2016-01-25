using System;
using CUE.NET;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Exceptions;
using CSGSI;
using System.Drawing;
using CUE.NET.Devices.Keyboard.Enums;

namespace CSGO_K70
{
    class KeyLighter
    {
        public CorsairKeyboard keyboard = null;
        public bool useNumpad = true;

        /// <summary>
        /// Initilize the keylighter
        /// </summary>
        public void Start()
        {
            //initilize the keyboard or return an exception if it fails.
            try
            {
                CueSDK.Initialize();
                Console.WriteLine("CUE Started with " + CueSDK.LoadedArchitecture + "-SDK");

                keyboard = CueSDK.KeyboardSDK;
                if (keyboard != null)
                {
                    keyboard.UpdateMode = UpdateMode.Manual;
                    //keyboard.UpdateFrequency = 1f/30f;
                    if (keyboard.DeviceInfo.Model.Contains("K65"))
                        useNumpad = true;
                    KeyGroups.setupKeyGroups(keyboard, useNumpad);
                }

            }
            catch (CUEException ex)
            {
                Console.WriteLine("CUE Exception!");
                Console.WriteLine("ErrorCode: " + Enum.GetName(typeof(CorsairError), ex.Error));
                keyboard = null;
            }
            catch (WrapperException ex)
            {
                Console.WriteLine("CUE Wrapper Exception!");
                Console.WriteLine("Message:" + ex.Message);
                keyboard = null;
            }
        }

        public void Update(float dt)
        {
            if (Core.currentBomb > 0)
                Core.currentBomb -= dt;

            //This section is discussed in detail in bombBackLight.
            if (Core.deadTime > 0)
                Core.deadTime -= dt;

            if (Core.flashTime > 0 && Core.deadTime <= 0)
                Core.flashTime -= dt;

            if (Core.flashTime <= 0 && Core.deadTime <= 0 && Core.currentBomb > 5)
                flashTimeIncrement();

            //the bomb backlighting is done "per frame" while the bomb is planted instead of just on a new game state.
            if (Core.bombPlanted)
            {
                bombBackLight();
            }
        }

        /// <summary>
        /// Handle a new game state.
        /// </summary>
        /// <param name="gs"></param>
        public void handleGameState(GameState gs)
        {
            //team backlight only needs to be done on a new gamestate while the bomb is not planted.
            if (!Core.bombPlanted)
            {
                teamBacklight(gs);
            }
        }

        /// <summary>
        /// Backlight the keyboard with the team colors.
        /// </summary>
        /// <param name="gs"></param>
        void teamBacklight(GameState gs)
        {
            if (gs.Player.Team.ToString() == "T") //terrorist
            {
                foreach (var key in keyboard.Keys)
                {
                    key.Led.Color = Color.FromArgb(255, 231, 60);
                }
            }
            else if (gs.Player.Team.ToString() == "CT") //counter-terrorist
            {
                foreach (var key in keyboard.Keys)
                {
                    key.Led.Color = Color.FromArgb(255, 200, 111);
                }
            }
        }

        /// <summary>
        /// Backlight for the bomb timer.
        /// </summary>
        void bombBackLight()
        {
            if (Core.currentBomb > 0)
            {
                //full backlight
                //I'm going to rubber duck this so when I look at it in the future i'm not confused.
                //Dead time is used for timing how long the keyboard should NOT be lit.
                //Flash time is used for timing how long the keyboard SHOULD be lit.
                //Dead time ticks down from the start until it hits < 0. Then flash time starts ticking down.
                //Once they both hit < 0 flashTimeIncrement is called which resets flash time, and increments bombTick,
                // which then resets dead time to the next time from bombTimes, or 0.35 if its over the length of bombTimes.
                //While dead time is > 0 the keyboard stays unlit.
                //While dead time is < 0 and flashtime is > 0 the keyboard is lit.
                //None of this matters once currentBomb has reached under 5 seconds and the lights just stay solid.
                if (Core.deadTime > 0 && Core.currentBomb > 5)
                {
                    foreach (var key in keyboard.Keys)
                    {
                        key.Led.Color = Color.FromArgb(0, 0, 0);
                    }
                }
                else
                {
                    foreach (var key in keyboard.Keys)
                    {
                        key.Led.Color = Color.FromArgb(255, 0, 0);
                    }
                }
                //numpad time ticker
                if (useNumpad)
                {
                    //blank the numpad
                    foreach (var key in KeyGroups.numpad.Keys)
                    {
                        key.Led.Color = Color.FromArgb(0, 0, 0);
                    }
                    //convert bomb time to a int
                    int intBombTime = (int)Math.Floor(Core.currentBomb);
                    //if the bombtime is 2 digits display the first digit, ex 32 displays 3
                    if (intBombTime.ToString().Length > 1)
                    {
                        int digit = 0;
                        int.TryParse(intBombTime.ToString().Substring(0, 1), out digit);
                        foreach (var key in KeyGroups.numpadDigital[digit].Keys)
                        {
                            key.Led.Color = Color.FromArgb(255, 0, 0);
                        }
                        //turn the two digits into a char array so they can be turned back into single digits
                        char[] cDigit = intBombTime.ToString().ToCharArray();
                        keyboard[findKeypadKey(int.Parse(cDigit[0].ToString()))].Led.Color = Color.FromArgb(0, 255, 0);
                        keyboard[findKeypadKey(int.Parse(cDigit[1].ToString()))].Led.Color = Color.FromArgb(0, 255, 0);
                    }
                    //after the bombtime drops below 10 display each digit
                    else
                    {
                        foreach (var key in KeyGroups.numpadDigital[intBombTime].Keys)
                        {
                            key.Led.Color = Color.FromArgb(255, 0, 0);
                        }
                        keyboard[findKeypadKey(intBombTime)].Led.Color = Color.FromArgb(0, 255, 0);
                    }
                }
            }
            else
            {
                foreach (var key in keyboard.Keys)
                {
                    key.Led.Color = Color.FromArgb(255, 0, 0);
                }
            }
        }

        /// <summary>
        /// handles the increasing flash time for the bomb.
        /// </summary>
        void flashTimeIncrement()
        {
            if (Core.flashTime <= 0)
            {
                //increment the bomb timer then set the next flash time.
                ++Core.bombTick;
                if (Core.bombTick > Core.bombTimes.Count - 1)
                    Core.deadTime = 0.35f;
                else
                    Core.deadTime = Core.bombTimes[Core.bombTick];
                Core.flashTime = Core.flashMax;
            }
        }

        /// <summary>
        /// Finds a keypad number based on the number fed in.
        /// Only for single digits.
        /// </summary>
        CorsairKeyboardKeyId findKeypadKey(int inI)
        {
            switch (inI)
            {
                case 1:
                    return CorsairKeyboardKeyId.Keypad1;
                case 2:
                    return CorsairKeyboardKeyId.Keypad2;
                case 3:
                    return CorsairKeyboardKeyId.Keypad3;
                case 4:
                    return CorsairKeyboardKeyId.Keypad4;
                case 5:
                    return CorsairKeyboardKeyId.Keypad5;
                case 6:
                    return CorsairKeyboardKeyId.Keypad6;
                case 7:
                    return CorsairKeyboardKeyId.Keypad7;
                case 8:
                    return CorsairKeyboardKeyId.Keypad8;
                case 9:
                    return CorsairKeyboardKeyId.Keypad9;
            }
            return CorsairKeyboardKeyId.Keypad0;
        }

    }
}
