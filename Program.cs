using QuizTools.GeneralUtils;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using QuizTools.Kahoot;
using QuizTools.Categories;

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

            new Category("Settings").AddOption(new DelayOption("Change Answers Delay", "Delay before answering correct ({0}-{1}ms)", Settings.Current.ChangeDelay))
                .AddOption(new BrowserOption("Change Browser", "Used broswer: {0}", Settings.Current.ChangeBrowser))
                .AddOption("Reset To Default", "Reset settings to default", Settings.Current.ResetToDefault);

            new Category("Other").AddOption("About", "Shows information about program", About)
                .AddOption("Exit", "Exits the program", Exit);

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Settings.Initialize();
            Start();
        }
        private static void Start()
        {
            Console.Clear();
            Console.Title = GeneralConstants.NAME;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("" +
                                "░██████╗░██╗░░░██╗██╗███████╗      ████████╗░█████╗░░█████╗░██╗░░░░░░██████╗" + "\n" +
                                "██╔═══██╗██║░░░██║██║╚════██║      ╚══██╔══╝██╔══██╗██╔══██╗██║░░░░░██╔════╝" + "\n" +
                                "██║██╗██║██║░░░██║██║░░███╔═╝      ░░░██║░░░██║░░██║██║░░██║██║░░░░░╚█████╗░" + "\n" +
                                "╚██████╔╝██║░░░██║██║██╔══╝░░      ░░░██║░░░██║░░██║██║░░██║██║░░░░░░╚═══██╗" + "\n" +
                                "░╚═██╔═╝░╚██████╔╝██║███████╗      ░░░██║░░░╚█████╔╝╚█████╔╝███████╗██████╔╝" + "\n" +
                                "░░░╚═╝░░░░╚═════╝░╚═╝╚══════╝      ░░░╚═╝░░░░╚════╝░░╚════╝░╚══════╝╚═════╝░");
            Console.ResetColor();
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
            Console.Write("➤ ");
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