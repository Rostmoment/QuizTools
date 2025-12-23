using QuizTools.Categories;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace QuizTools
{
    class Program
    {
        public static Random RNG { get; } = new Random();

        private static void Main(string[] args)
        {
            Settings.Initialize();
            new Category("Kahoot").AddOption("Get Info", "Shows information about kahoot", Kahoot.Kahoot.GetGamenfo)
                .AddOption("Get Answers", "Shows answers to Kahoot", Kahoot.Kahoot.GetAnswers)
                .AddOption("Solve Kahoot", "Solves test on Kahoot", Kahoot.Kahoot.SolveKahoot)
                .AddOption("Spam With Bots", "Spams with bots to Kahoot test", Kahoot.Kahoot.SpamWithBots)
                .AddOption("Brute Force", "Brute forces pins for Kahoot", Kahoot.Kahoot.BruteForceKahoots);

            new Category("Vseosvita").AddOption("Solve Test", "Solves test on Vseosvita", Vseosvita.Vseosvita.SolveTest)
                .AddOption("Spam With Bots", "Spam with bots to Vseosvita test", Vseosvita.Vseosvita.SpamWithBots);

            new Category("Settings").AddOption(new LogoGradientOption("Change Logo Gradient", "{0} --- {1}", Settings.SetLogoGradient))
                .AddOption("Reset To Default", "", Settings.Reset)
                .AddOption("Open Settings Folder", "", Settings.OpenFolder);

            new Category("Other").AddOption("About", "Shows information about program", About)
                .AddOption("Exit", "Exits The Program", Exit);

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Start();
        }
        private static void Start()
        {
            Console.Clear();
            Console.Title = GeneralConstants.NAME;
            Banner();
            ConsoleUtils.WriteToEndOfLine("=");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Welcome to {GeneralConstants.NAME}! Type number of option that you want to do and then press enter!");
            Console.ResetColor();

            int i = 1;
            foreach (Category category in Category.all)
            {
                category.WriteName();
                foreach (CategoryOption option in category.options)
                {
                    option.AddToList();
                    ConsoleUtils.Center2Strings($"[{i}] {option.Name}", option.Description);
                    i++;
                }
            }

            Console.ResetColor();

            ConsoleUtils.WriteToEndOfLine("=");
            ConsoleUtils.WriteArrow();
            string? answer = Console.ReadLine();
            if (answer == null)
                Start();

            Console.WriteLine();
            
            if (int.TryParse(answer, out int index) && index > 0 && index <= CategoryOption.Count)
                CategoryOption.Invoke(index);
            else
                Logger.WriteErrorLine("Ivalid Input");

            Console.Title = GeneralConstants.NAME;
            Console.WriteLine("Press Any Key To Continue");
            Console.ReadKey();

            Start();
        }
        private static void Banner()
        {
            List<string> colors = ColorHelper.GetGradient(Settings.Instance.LogoGradientFrom, Settings.Instance.LogoGradientTo, 6);
            Console.WriteLine("" +
                                "░██████╗░██╗░░░██╗██╗███████╗      ████████╗░█████╗░░█████╗░██╗░░░░░░██████╗".ColorHex(colors[0]) + "\n" +
                                "██╔═══██╗██║░░░██║██║╚════██║      ╚══██╔══╝██╔══██╗██╔══██╗██║░░░░░██╔════╝".ColorHex(colors[1]) + "\n" +
                                "██║██╗██║██║░░░██║██║░░███╔═╝      ░░░██║░░░██║░░██║██║░░██║██║░░░░░╚█████╗░".ColorHex(colors[2]) + "\n" +
                                "╚██████╔╝██║░░░██║██║██╔══╝░░      ░░░██║░░░██║░░██║██║░░██║██║░░░░░░╚═══██╗".ColorHex(colors[3]) + "\n" +
                                "░╚═██╔═╝░╚██████╔╝██║███████╗      ░░░██║░░░╚█████╔╝╚█████╔╝███████╗██████╔╝".ColorHex(colors[4]) + "\n" +
                                "░░░╚═╝░░░░╚═════╝░╚═╝╚══════╝      ░░░╚═╝░░░░╚════╝░░╚════╝░╚══════╝╚═════╝░".ColorHex(colors[5]));
            Console.ResetColor();
        }

        private static void About()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Logger.WriteInfoLine($"{GeneralConstants.NAME} || Version: {GeneralConstants.VERSION}");
            Console.ResetColor();
        }
        private static void Exit()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("👋 Goodbye!");
            Console.ResetColor();
            Environment.Exit(0);
        }
    }
}