A simple C# console application for Corsair keyboard lighting effects for CSGO using its game state integration.
Currently changes backlight color based on team and backlight flashing for the bomb ticking along with a numpad bomb timer.
Along with lighting number keys green when you have weapons. The keys fade to red as your current clip runs out of ammo.
Also has a health bar for f1 - f6, and a armor bar for f7 - f12.
Note that the name CSGO_K70 is a name only and should work for any RGB Corsair keyboard not just the K70.

Usage:
put the gamestate_integration_CSGO_K70.cfg in your CSGO config folder (ex D:\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\cfg)
-h: list commands
-s: asks to enter ip (ex. 72.111.1.25:3000 or localhost:3000) then starts the game listener.
-quit: quits the application

VAC:
Since I have been asked a few times about VAC. This application only uses data that is offically provided by the game itself for the purpose of integrations such as this. This program uses the exact same type of config and data as the games built in logitech integration (You will see that there is a gamestate_integration_logitech.cfg in the games files). There is no form of direct memory access or other data access that would be classified as cheating. As such you will not be VAC banned for using this application.

Using:
CSGSI: https://github.com/rakijah/CSGSI
RGB.NET: https://github.com/DarthAffe/RGB.NET