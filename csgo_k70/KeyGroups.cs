using System.Collections.Generic;
using System;
using RGB.NET.Devices.Corsair;
using RGB.NET.Groups;
using RGB.NET.Core;

namespace CSGO_K70
{
	static class KeyGroups
	{
		public static RectangleLedGroup numpad; //a selection of the numpad keys.
		public static List<ListLedGroup> numpadDigital; //a list of combinations of numpad keys meant to look like a digital number display.
		public static ListLedGroup WASD;
		public static ListLedGroup healthFunction;
		public static ListLedGroup armorFunction;

		/// <summary>
		/// Intial setup of keygroups using the keyboard provided.
		/// </summary>
		/// <param name="keyboard"></param>
		/// <param name="useNumpad"></param>
		public static void setupKeyGroups(CorsairKeyboardRGBDevice keyboard)
		{
			try
			{
				WASD = new ListLedGroup(
					keyboard[CorsairLedId.W],
					keyboard[CorsairLedId.A],
					keyboard[CorsairLedId.S],
					keyboard[CorsairLedId.D]);

				healthFunction = new ListLedGroup(
					keyboard[CorsairLedId.F1],
					keyboard[CorsairLedId.F2],
					keyboard[CorsairLedId.F3],
					keyboard[CorsairLedId.F4],
					keyboard[CorsairLedId.F5],
					keyboard[CorsairLedId.F6]);
				armorFunction = new ListLedGroup(
					keyboard[CorsairLedId.F7],
					keyboard[CorsairLedId.F8],
					keyboard[CorsairLedId.F9],
					keyboard[CorsairLedId.F10],
					keyboard[CorsairLedId.F11],
					keyboard[CorsairLedId.F12]);

				numpad = new RectangleLedGroup(keyboard[CorsairLedId.NumLock], keyboard[CorsairLedId.KeypadEnter], 1.0f, true);

				numpadDigital = new List<ListLedGroup>();
				for (int i = 0; i < 10; ++i)
				{
					numpadDigital.Add(null);
				}

				numpadDigital[1] = new ListLedGroup(
					keyboard[CorsairLedId.NumLock],
					keyboard[CorsairLedId.KeypadSlash],
					keyboard[CorsairLedId.Keypad8],
					keyboard[CorsairLedId.Keypad5],
					keyboard[CorsairLedId.Keypad2],
					keyboard[CorsairLedId.Keypad0],
					keyboard[CorsairLedId.KeypadPeriodAndDelete]);

				var numpad2Keys = new Led[] {
					keyboard[CorsairLedId.NumLock],
					keyboard[CorsairLedId.KeypadSlash],
					keyboard[CorsairLedId.KeypadAsterisk],
					keyboard[CorsairLedId.Keypad9],
					keyboard[CorsairLedId.Keypad6],
					keyboard[CorsairLedId.Keypad5],
					keyboard[CorsairLedId.Keypad4],
					keyboard[CorsairLedId.Keypad1],
					keyboard[CorsairLedId.Keypad0],
					keyboard[CorsairLedId.KeypadPeriodAndDelete]};
				numpadDigital[2] = new ListLedGroup(numpad2Keys);

				numpadDigital[3] = new ListLedGroup(numpad2Keys);
				numpadDigital[3].AddLed(keyboard[CorsairLedId.Keypad3]);
				numpadDigital[3].RemoveLed(keyboard[CorsairLedId.Keypad1]);

				var numpad4Keys = new Led[] {  keyboard[CorsairLedId.NumLock],
					keyboard[CorsairLedId.Keypad7],
					keyboard[CorsairLedId.Keypad4],
					keyboard[CorsairLedId.Keypad5],
					keyboard[CorsairLedId.Keypad6],
					keyboard[CorsairLedId.Keypad9],
					keyboard[CorsairLedId.KeypadAsterisk],
					keyboard[CorsairLedId.Keypad3],
					keyboard[CorsairLedId.KeypadPeriodAndDelete]};
				numpadDigital[4] = new ListLedGroup(numpad4Keys);

				numpadDigital[5] = new ListLedGroup(numpad2Keys);
				numpadDigital[5].AddLed(keyboard[CorsairLedId.Keypad7], keyboard[CorsairLedId.Keypad3]);
				numpadDigital[5].RemoveLed(keyboard[CorsairLedId.Keypad9], keyboard[CorsairLedId.Keypad1]);

				numpadDigital[6] = new ListLedGroup(numpad2Keys);
				numpadDigital[6].AddLed(keyboard[CorsairLedId.Keypad3], keyboard[CorsairLedId.Keypad7]);
				numpadDigital[6].RemoveLed(keyboard[CorsairLedId.Keypad9]);

				var numpad7Keys = new Led[] {
					keyboard[CorsairLedId.Keypad7],
					keyboard[CorsairLedId.NumLock],
					keyboard[CorsairLedId.KeypadSlash],
					keyboard[CorsairLedId.KeypadAsterisk],
					keyboard[CorsairLedId.Keypad9],
					keyboard[CorsairLedId.Keypad6],
					keyboard[CorsairLedId.Keypad3],
					keyboard[CorsairLedId.KeypadPeriodAndDelete] };
				numpadDigital[7] = new ListLedGroup(numpad7Keys);

				numpadDigital[8] = new ListLedGroup(numpad2Keys);
				numpadDigital[8].AddLed(
					keyboard[CorsairLedId.Keypad7],
					keyboard[CorsairLedId.Keypad3]);

				numpadDigital[9] = new ListLedGroup(numpad4Keys);
				numpadDigital[9].AddLed(keyboard[CorsairLedId.KeypadSlash]);

				numpadDigital[0] = new ListLedGroup(numpad7Keys);
				numpadDigital[0].AddLed(
					keyboard[CorsairLedId.Keypad0],
					keyboard[CorsairLedId.Keypad1],
					keyboard[CorsairLedId.Keypad4]);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception in creating key groups!");
				Console.WriteLine("Message: " + ex.Message);
			}
		}
	}
}
