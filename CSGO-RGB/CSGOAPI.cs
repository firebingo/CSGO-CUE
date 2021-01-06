using System;
using CSGSI;

//documentation on game state returns
//https://www.reddit.com/r/GlobalOffensive/comments/3w26kq/game_state_integration_quick_and_dirty/
namespace CSGO_K70
{
    class CSGOAPI
    {
        public GameStateListener gs1 = null;
        KeyLighter keyController = new KeyLighter();

        /// <summary>
        /// Starts the CSGSI connection
        /// </summary>
        /// <param name="ip"></param>
        public void connectToCS(string ip)
        {
            try
            {
                gs1 = new GameStateListener(string.Format("http://{0}/", ip));
                gs1.NewGameState += new NewGameStateHandler(OnNewGameState);
                if (gs1.Start())
                {
                    Console.Write("Game Listener Started\n");
                }
                else
                {
                    Console.Write("Couldn't Start Game Listener. Try running as admin.\n");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception occured in starting Game Listener!");
                Console.WriteLine("Message: " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Handle a new gamestate
        /// </summary>
        /// <param name="gs"></param>
        public void OnNewGameState(GameState gs)
        {
            Core.HandleGameState(gs);
        }
    }
}
