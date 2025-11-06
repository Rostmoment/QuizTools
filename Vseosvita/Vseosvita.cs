using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita
{
    class Vseosvita
    {
        public static void SpamWithBots()
        {

        }
        public static void SolveTest()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter test ID: ");   
            Console.ResetColor();
            Console.Write("➤ ");
            string id = Console.ReadLine();
            VseosvitaUser user = new VseosvitaUser("123", id);
            if (user.JoinTest())
            {
                Console.WriteLine($"Joined test successfully with ID {user.ID}!\nPress enter to start test");
                Console.ReadLine();
                user.StartTest();
            }
        }
    }
}
