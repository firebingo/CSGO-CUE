A simple C# console application for Corsair keyboard lighting effects for CSGO using its game state integration.
Currently changes backlight color based on team and backlight flashing for the bomb ticking along with a numpad bomb timer.
Bomb lighting will be improved in the future and ammo related lighting for the number keys is next on the list.
Note that the name CSGO_K70 is a name only and should work for any RGB Corsair keyboard not just the K70.

Usage:
put the gamestate_integration_CSGO_K70.cfg in your CSGO config folder (ex D:\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\cfg)
-h: list commands
-s: asks to enter ip (ex. 72.111.1.25:3000 or localhost:3000) then starts the game listener.
-quit: quits the application


Built for Visual Studio 2015 and .NET 4.5.2

Using:
CSGSI: https://github.com/rakijah/CSGSI
CUE.NET: https://github.com/DarthAffe/CUE.NET
Fody: https://github.com/Fody/Fody/