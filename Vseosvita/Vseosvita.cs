using QuizTools.Vseosvita.QuestionTypes;
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
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter test ID: ");
            Console.ResetColor();
            Console.Write("➤ ");
            string id = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter player bot nickname (empty for random): ");
            Console.ResetColor();
            Console.Write("➤ ");
            string name = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            Console.WriteLine("Enter number of bots to spawn: ");
            Console.ResetColor();
            Console.Write("➤ ");
            int count = int.Parse(Console.ReadLine());

            string[] nicknames = new string[count];
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(name))
                    nicknames[i] = NicknameGenerator.RandomNickname();
                else
                    nicknames[i] = name;
            }

            foreach (string nickname in nicknames)
            {
                VseosvitaUser user = new VseosvitaUser(nickname, id);
                if (user.JoinTest())
                    Console.WriteLine($"Bot {nickname} joined test successfully with ID {user.ID}!");
                else
                    Console.WriteLine($"Bot {nickname} failed to join test.");
            }
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
                BaseVseosvitaQuestion question = user.StartTest();
                Console.WriteLine("Started test!\nPress enter to answer first question");
                Console.ReadLine();
                question.AnswerQuestion(user, 1);
            }
        }
    }
}
