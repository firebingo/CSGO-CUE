A simple C# console application for Corsair keyboard lighting effects for CSGO using its game state integration.
Currently changes backlight color based on team and backlight flashing for the bomb ticking along with a numpad bomb timer.
Along with lighting number keys green when you have weapons. The keys fade to red as your current clip runs out of ammo.
Also has a health bar for f1 - f6, and a armor bar for f7 - f12.
Note that the name CSGO_K70 is a name only and should work for any RGB Corsair keyboard not just the K70.
The build.zip in the root of the project is the current running build.

Usage:
put the gamestate_integration_CSGO_K70.cfg in your CSGO config folder (ex D:\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\cfg)
-h: list commands
-s: asks to enter ip (ex. 72.111.1.25:3000 or localhost:3000) then starts the game listener.
-quit: quits the application


Built for Visual Studio 2017, .NET 4.5.2, and x64.

Using:
CSGSI: https://github.com/rakijah/CSGSI
RGB.NET: https://github.com/DarthAffe/RGB.NET
Fody: https://github.com/Fody/Fody/