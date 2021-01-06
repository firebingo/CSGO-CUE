using System;
using System.Linq;
using RGB.NET.Core;
using RGB.NET.Devices.Corsair;
using Color = System.Drawing.Color;

namespace CSGO_K70
{
	class KeyLighter
	{
		public CorsairKeyboardRGBDevice keyboard = null;
		public RGBSurface surface = RGBSurface.Instance;
		public RGB.NET.Core.Color whiteColor = Core.SysColorToRGBColor(Color.White);
		public RGB.NET.Core.Color blackColor = Core.SysColorToRGBColor(Color.Black);

		/// <summary>
		/// Initilize the keylighter
		/// </summary>
		public void Start()
		{
			//initilize the keyboard or return an exception if it fails.
			try
			{
				surface.Exception += args =>
				{
					Console.WriteLine("RGB Wrapper Exception!");
					Console.WriteLine("Message:" + args.Exception.Message);
				};
				surface.LoadDevices(CorsairDeviceProvider.Instance);

				Console.WriteLine("CUE Started");

				keyboard = surface.Devices.FirstOrDefault(x => x.DeviceInfo.Manufacturer.ToUpper() == "CORSAIR" && x.DeviceInfo.DeviceType == RGBDeviceType.Keyboard) as CorsairKeyboardRGBDevice;
				if (keyboard != null)
				{
					keyboard.UpdateMode = DeviceUpdateMode.Sync;
					//keyboard.UpdateFrequency = 1f/30f;
					KeyGroups.SetupKeyGroups(keyboard);
				}
			}
			catch (CUEException ex)
			{
				Console.WriteLine("CUE Exception!");
				Console.WriteLine("ErrorCode: " + Enum.GetName(typeof(CorsairError), ex.Error));
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
				FlashTimeIncrement();

			if (Core.inGame)
			{
				//the bomb backlighting is done "per frame" while the bomb is planted instead of just on a new game state.
				if (Core.bombPlanted)
				{
					BombBackLight();
				}

				if (Core.burningAmount > 0)
					FireLights();

				WeaponLights();
				HealthLights();
				WASDLights();

				if (Core.flashAmount > 0)
					FlashLights();

			}
		}

		/// <summary>
		/// Handle a new game state.
		/// </summary>
		/// <param name="gs"></param>
		public void HandleGameState()
		{
			if (Core.inGame)
			{
				//team backlight only needs to be done on a new gamestate while the bomb is not planted.
				if (!Core.bombPlanted)
				{
					TeamBacklight();
				}
				//these are done here also to prevent the keys from flashing.
				if (Core.burningAmount > 0)
					FireLights();
				WeaponLights();
				HealthLights();
				WASDLights();
				if (Core.flashAmount > 0)
					FlashLights();
			}
			else
			{
				foreach (var key in keyboard)
					key.Color = Core.SysColorToRGBColor(Color.Gray);
			}
		}

		/// <summary>
		/// Backlight the keyboard with the team colors.
		/// </summary>
		/// <param name="gs"></param>
		void TeamBacklight()
		{
			if (Core.isT) //terrorist
			{
				foreach (var key in keyboard)
				{
					key.Color = new RGB.NET.Core.Color(255, 204, 51);
				}
			}
			else //counter-terrorist
			{
				foreach (var key in keyboard)
				{
					key.Color = new RGB.NET.Core.Color(89, 157, 225);
				}
			}
		}

		void WASDLights()
		{
			foreach (var key in KeyGroups.WASD.GetLeds())
			{
				key.Color = whiteColor;
			}
			keyboard[CorsairLedId.Space].Color = whiteColor;
			keyboard[CorsairLedId.LeftCtrl].Color = whiteColor;
			keyboard[CorsairLedId.LeftShift].Color = whiteColor;
		}

		/// <summary>
		/// Backlight for the bomb timer.
		/// </summary>
		void BombBackLight()
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
					foreach (var key in keyboard)
					{
						key.Color = blackColor;
					}
				}
				else
				{
					foreach (var key in keyboard)
					{
						key.Color = new RGB.NET.Core.Color(255, 0, 0);
					}
				}
				//numpad time ticker
				//blank the numpad
				foreach (var key in KeyGroups.numpad.GetLeds())
				{
					key.Color = blackColor;
				}
				//convert bomb time to a int
				int intBombTime = (int)Math.Floor(Core.currentBomb);
				//if the bombtime is 2 digits display the first digit, ex 32 displays 3
				if (intBombTime.ToString().Length > 1)
				{
					if (int.TryParse(intBombTime.ToString().Substring(0, 1), out var digit))
					{
						foreach (var key in KeyGroups.numpadDigital[digit].GetLeds())
						{
							key.Color = new RGB.NET.Core.Color(255, 0, 0);
						}
						//turn the two digits into a char array so they can be turned back into single digits
						char[] cDigit = intBombTime.ToString().ToCharArray();
						keyboard[FindKeypadKey(int.Parse(cDigit[0].ToString()))].Color = new RGB.NET.Core.Color(0, 255, 0);
						keyboard[FindKeypadKey(int.Parse(cDigit[1].ToString()))].Color = new RGB.NET.Core.Color(0, 255, 0);
					}
				}
				//after the bombtime drops below 10 display each digit
				else
				{
					foreach (var key in KeyGroups.numpadDigital[intBombTime].GetLeds())
					{
						key.Color = new RGB.NET.Core.Color(255, 0, 0);
					}
					keyboard[FindKeypadKey(intBombTime)].Color = new RGB.NET.Core.Color(0, 255, 0);
				}
			}
			else
			{
				foreach (var key in keyboard)
				{
					key.Color = new RGB.NET.Core.Color(255, 0, 0);
				}
			}
		}

		/// <summary>
		/// handles the increasing flash time for the bomb.
		/// </summary>
		void FlashTimeIncrement()
		{
			if (Core.flashTime <= 0)
			{
				//increment the bomb timer then set the next flash time.
				++Core.bombTick;
				if (Core.bombTick > Core.bombTimes.Count - 1)
					Core.deadTime = 0.28f;
				else
					Core.deadTime = Core.bombTimes[Core.bombTick];
				Core.flashTime = Core.flashMax;
			}
		}

		/// <summary>
		/// Handles the lighting of number keys for weapon ammo and etc.
		/// </summary>
		void WeaponLights()
		{
			//PRIMARY
			//if the weapon has no reserve just blank the key.
			if (Core.primaryReserve == 0 && Core.primaryCurrent == 0)
				keyboard[CorsairLedId.D1].Color = blackColor;
			else
			{
				//if the weapon has no ammo loaded turn the key red.
				if (Core.primaryCurrent == 0)
					keyboard[CorsairLedId.D1].Color = new RGB.NET.Core.Color(255, 0, 0);
				else
				{
					//this system makes the key turn from green to red based on how much ammo is left.
					float r;
					float g;
					//vs says the float cast is redundent but it doesnt work without it.
					if ((float)Core.primaryCurrent / (float)Core.primaryMax > 0.5)
					{
						g = 1f * 255f;
						r = (2.0f - ((float)Core.primaryCurrent / (float)Core.primaryMax / 0.5f)) * 255f;
					}
					else
					{
						g = ((float)Core.primaryCurrent / (float)Core.primaryMax / 0.5f) * 255;
						r = 1f * 255f;
					}
					keyboard[CorsairLedId.D1].Color = new RGB.NET.Core.Color((int)r, (int)g, 0);
				}
			}
			//SECONDARY
			//if the weapon has no reserve just blank the key.
			if (Core.secondaryReserve == 0 && Core.secondaryCurrent == 0)
				keyboard[CorsairLedId.D2].Color = blackColor;
			else
			{
				//if the weapon has no ammo loaded turn the key red.
				if (Core.secondaryCurrent == 0)
					keyboard[CorsairLedId.D2].Color = new RGB.NET.Core.Color(255, 0, 0);
				else
				{
					//this system makes the key turn from green to red based on how much ammo is left.
					float r;
					float g;
					if ((float)Core.secondaryCurrent / (float)Core.secondaryMax > 0.5)
					{
						g = 1f * 255f;
						r = (2.0f - (((float)Core.secondaryCurrent / (float)Core.secondaryMax) / 0.5f)) * 255f;
					}
					else
					{
						g = (((float)Core.secondaryCurrent / (float)Core.secondaryMax) / 0.5f) * 255f;
						r = 1f * 255f;
					}
					keyboard[CorsairLedId.D2].Color = new RGB.NET.Core.Color((int)r, (int)g, 0);
				}
			}

			//You always have a knife so keep the key green.
			keyboard[CorsairLedId.D3].Color = new RGB.NET.Core.Color(0, 255, 0);

			//if you have grenades turn the key green otherwise blank it.
			if (Core.grenadeCount == 0)
				keyboard[CorsairLedId.D4].Color = blackColor;
			else
				keyboard[CorsairLedId.D4].Color = new RGB.NET.Core.Color(0, 255, 0);

			//if you have C4 turn the key green otherwise blank it.
			if (!Core.hasC4)
				keyboard[CorsairLedId.D5].Color = blackColor;
			else
				keyboard[CorsairLedId.D5].Color = new RGB.NET.Core.Color(0, 255, 0);
		}

		/// <summary>
		/// Handles the lighting of the function keys for health and armor values
		/// </summary>
		void HealthLights()
		{
			//HEALTH
			if (Core.healthCurrent == 0) //if no health blank keys
			{
				foreach (var key in KeyGroups.healthFunction.GetLeds())
				{
					key.Color = blackColor;
				}
			}
			else if (Core.healthCurrent == Core.healthMax) //if health is full set keys to green
			{
				foreach (var key in KeyGroups.healthFunction.GetLeds())
				{
					key.Color = new RGB.NET.Core.Color(0, 255, 0);
				}
			}
			else //have the health key group act as a health bar.
			{
				foreach (var key in KeyGroups.healthFunction.GetLeds())
					key.Color = blackColor;
				float healthRatio = (float)Core.healthCurrent / (float)Core.healthMax;
				if (healthRatio < 0.17f)
				{
					keyboard[CorsairLedId.F1].Color = new RGB.NET.Core.Color(0, (int)((healthRatio / 0.17f) * 255f), 0);
				}
				else if (healthRatio < 0.33f)
				{
					keyboard[CorsairLedId.F1].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F2].Color = new RGB.NET.Core.Color(0, (int)(((healthRatio - 0.17f) / 0.17f) * 255f), 0);
				}
				else if (healthRatio < 0.5f)
				{
					keyboard[CorsairLedId.F1].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F2].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F3].Color = new RGB.NET.Core.Color(0, (int)(((healthRatio - 0.33f) / 0.17) * 255f), 0);
				}
				else if (healthRatio < 0.67f)
				{
					keyboard[CorsairLedId.F1].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F2].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F3].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F4].Color = new RGB.NET.Core.Color(0, (int)(((healthRatio - 0.5f) / 0.17) * 255f), 0);
				}
				else if (healthRatio < 0.83f)
				{
					keyboard[CorsairLedId.F1].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F2].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F3].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F4].Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F5].Color = new RGB.NET.Core.Color(0, (int)(((healthRatio - 0.67f) / 0.17) * 255f), 0);
				}
				else
				{
					foreach (var key in KeyGroups.healthFunction.GetLeds())
						key.Color = new RGB.NET.Core.Color(0, 255, 0);
					keyboard[CorsairLedId.F6].Color = new RGB.NET.Core.Color(0, (int)(((healthRatio - 0.83f) / 0.17) * 255f), 0);
				}
			}

			//ARMOR
			RGB.NET.Core.Color armorColor = Core.SysColorToRGBColor(Color.Black);
			if (Core.hasHelmet)
				armorColor = new RGB.NET.Core.Color(0, 0, 255);
			else
				armorColor = new RGB.NET.Core.Color(0, 128, 255);
			if (Core.armorCurrent == 0) //if no armor turn keys red
			{
				foreach (var key in KeyGroups.armorFunction.GetLeds())
				{
					key.Color = new RGB.NET.Core.Color(255, 0, 0);
				}
			}
			else if (Core.armorCurrent == Core.armorMax) //if armor is full set keys to armor color
			{
				foreach (var key in KeyGroups.armorFunction.GetLeds())
				{
					key.Color = armorColor;
				}
			}
			else //have the armor key group act as an armor bar.
			{
				foreach (var key in KeyGroups.armorFunction.GetLeds())
					key.Color = blackColor;
				float armorRatio = (float)Core.armorCurrent / (float)Core.armorMax;
				if (armorRatio < 0.17f)
				{
					var armorColorGrad = new RGB.NET.Core.Color(0, armorColor.G, (int)((armorRatio / 0.17f) * 255f));
					keyboard[CorsairLedId.F12].Color = armorColorGrad;
				}
				else if (armorRatio < 0.33f)
				{
					var armorColorGrad = new RGB.NET.Core.Color(0, armorColor.G, (int)(((armorRatio - 0.17f) / 0.17f) * 255f));
					keyboard[CorsairLedId.F12].Color = armorColor;
					keyboard[CorsairLedId.F11].Color = armorColorGrad;
				}
				else if (armorRatio < 0.5f)
				{
					var armorColorGrad = new RGB.NET.Core.Color(0, armorColor.G, (int)(((armorRatio - 0.33f) / 0.17f) * 255f));
					keyboard[CorsairLedId.F12].Color = armorColor;
					keyboard[CorsairLedId.F11].Color = armorColor;
					keyboard[CorsairLedId.F10].Color = armorColorGrad;
				}
				else if (armorRatio < 0.67f)
				{
					var armorColorGrad = new RGB.NET.Core.Color(0, armorColor.G, (int)(((armorRatio - 0.5f) / 0.17f) * 255f));
					keyboard[CorsairLedId.F12].Color = armorColor;
					keyboard[CorsairLedId.F11].Color = armorColor;
					keyboard[CorsairLedId.F10].Color = armorColor;
					keyboard[CorsairLedId.F9].Color = armorColorGrad;
				}
				else if (armorRatio < 0.83f)
				{
					var armorColorGrad = new RGB.NET.Core.Color(0, armorColor.G, (int)(((armorRatio - 0.67f) / 0.17f) * 255f));
					keyboard[CorsairLedId.F12].Color = armorColor;
					keyboard[CorsairLedId.F11].Color = armorColor;
					keyboard[CorsairLedId.F10].Color = armorColor;
					keyboard[CorsairLedId.F9].Color = armorColor;
					keyboard[CorsairLedId.F8].Color = armorColorGrad;
				}
				else
				{
					var armorColorGrad = new RGB.NET.Core.Color(0, armorColor.G, (int)(((armorRatio - 0.83f) / 0.17f) * 255f));
					foreach (var key in KeyGroups.armorFunction.GetLeds())
						key.Color = armorColor;
					keyboard[CorsairLedId.F7].Color = armorColorGrad;
				}
			}
		}

		/// <summary>
		/// Handles the lighting of keys for when you are affected by a flash bang.
		/// </summary>
		void FlashLights()
		{
			foreach (var key in keyboard)
			{
				int red = Math.Clamp((int)(key.Color.R * 255 + Core.flashAmount), 0, 255);
				int green = Math.Clamp((int)(key.Color.G * 255 + Core.flashAmount), 0, 255);
				int blue = Math.Clamp((int)(key.Color.B * 255 + Core.flashAmount), 0,  255);
				key.Color = new RGB.NET.Core.Color(red, green, blue);
			}
		}

		/// <summary>
		/// Handles the lighting of keys for when you are burning.
		/// </summary>
		void FireLights()
		{
			foreach (var key in keyboard)
			{
				int red = Math.Clamp((int)(key.Color.R * 255 + Core.burningAmount), 0, 255);
				int green = Math.Clamp((int)(key.Color.G * 255 + Core.burningAmount), 0, 200);
				key.Color = new RGB.NET.Core.Color(red, green, 0);
			}
		}

		/// <summary>
		/// Finds a keypad number based on the number fed in.
		/// Only for single digits.
		/// </summary>
		private static CorsairLedId FindKeypadKey(int inI)
		{
			return inI switch
			{
				1 => CorsairLedId.Keypad1,
				2 => CorsairLedId.Keypad2,
				3 => CorsairLedId.Keypad3,
				4 => CorsairLedId.Keypad4,
				5 => CorsairLedId.Keypad5,
				6 => CorsairLedId.Keypad6,
				7 => CorsairLedId.Keypad7,
				8 => CorsairLedId.Keypad8,
				9 => CorsairLedId.Keypad9,
				_ => CorsairLedId.Keypad0
			};
		}
	}
}
