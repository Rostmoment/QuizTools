using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita
{
    class Vseosvita
    {
        public static void SolveTest()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter test ID: ");   
            Console.ResetColor();
            Console.Write("➤ ");
            string id = Console.ReadLine();
            new VseosvitaUser("VseosvitaUser").JoinTest(id);
        }
    }
}
