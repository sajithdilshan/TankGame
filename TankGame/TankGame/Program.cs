using System;
using TankGame.Network;
using TankGame.Intelligence;
using System.Threading;

namespace TankGame
{
#if WINDOWS || XBOX
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                // This is the solution which is currently being developed.
                game.Run();
            }            
        }
    }
#endif
}

