using System;

namespace CoopDrivingSim
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CoopDrivingSim game = new CoopDrivingSim())
            {
                game.Run();
            }
        }
    }
}

