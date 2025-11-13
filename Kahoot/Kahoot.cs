using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Exceptions;
using QuizTools.Kahoot.Games;
using QuizTools.Kahoot.QuestionTypes;

namespace QuizTools.Kahoot
{
    class Kahoot
    {
        public static void WriteInfo(KahootGame game)
        {
            ConsoleUtils.SaveColors();
            Console.ForegroundColor = ConsoleColor.Green;
            string text = $"\nName: {game.Name.MakeHyperlink(game.DetailsLink)}" +
                $"\nQuiz ID: {game.QuizID.MakeHyperlink(game.QuizJSONLink)}" +
                $"\nCreator: {game.Creator.ToString().MakeHyperlink(game.Creator.Link)}" +
                $"\nLanguage: {game.Language}" +
                $"\nCreated: {game.CreatedTime}" +
                $"\nModified: {game.ModifiedTime}" +
                $"\nNumber of question: {game.NumberOfQuestions}" +
                $"\nMax points: {game.TotalMaxPoints}";

            if (game is KahootChallenge challenge)
            {
                text += $"\nTitle: {challenge.Title.MakeHyperlink(challenge.JoinLink)}" +
                    $"\nChallenge ID: {challenge.ChallengeID.MakeHyperlink(challenge.ChallengeIDJSONLink)}" +
                    $"\nPin: {challenge.Pin.MakeHyperlink(challenge.PinJSONLink)}" +
                    $"\nStart: {challenge.StartTime}" +
                    $"\nEnd: {challenge.EndTime}" +
                    $"\nMax players: {challenge.MaxPlayers}" +
                    $"\nPlayers ({challenge.Players.Length}): {challenge.Players.ToSeperateString(", ")}" +
                    $"\nHost: {challenge.Host.ToString().MakeHyperlink(challenge.Host.Link)}";
            }

            Console.WriteLine(text);
            ConsoleUtils.RestoreColors();
        }
        public static void GetGamenfo()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter kahoot link, pin, challenge or quiz id");
            Console.ResetColor();
            Console.Write("➤ ");
            string? input = Console.ReadLine();

            try
            {
                KahootGame game = KahootGame.Get(input);
                WriteInfo(game);
            }
            catch (KahootNotFoundException e)
            {
                Logger.WriteWarningLine(e.Message);
            }
            catch (Exception e)
            {
                Logger.WriteErrorLine(e);
            }
        }
        public static void GetAnswers()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter kahoot link, pin, challenge or quiz id");
            Console.ResetColor();
            Console.Write("➤ ");
            string? input = Console.ReadLine();
            KahootGame quiz = null;

            try
            {
                quiz = KahootGame.Get(input);
            }
            catch (KahootNotFoundException e)
            {
                Logger.WriteWarningLine(e.Message);
                return;
            }
            catch (Exception e)
            {
                Logger.WriteErrorLine(e);
                return;
            }
            for (int i = 0; i < quiz.NumberOfQuestions; i++)
            {
                Console.ResetColor();
                BaseKahootQuestion question = quiz.Questions[i];
                if (question is KahootUnknownQuestion)
                {
                    Logger.WriteWarningLine($"Q{i + 1} is of unknown type, answers can't be shown");
                    continue;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Q{i + 1}: {question.Title} --- [{question.MaxPoints} points --- {question.SecondsTime} second(s)]");
                if (question.Image != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(question.Image);
                }
                question.WriteAnswers();
            }
            Console.ResetColor();
        }
        public static void SolveKahoot()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter game link or pin");
            Console.ResetColor();
            Console.Write("➤ ");
            string game = Console.ReadLine();
            if (game == null)
                return;

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter nickname");
            Console.ResetColor();
            Console.Write("➤ ");
            string name = Console.ReadLine();
            if (name == null || name == "")
                return;

            Console.ResetColor();
            try
            {
                string answer = "";
                while (answer != "y" && answer != "n")
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Use async method (y/n)?");
                    Console.ResetColor();
                    Console.Write("➤ ");
                    answer = Console.ReadLine().ToLower();
                }
                float accuracy = 1;

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Enter accuracy (0.0 - 1.0), empty or invalid for 1");
                Console.ResetColor();
                Console.Write("➤ ");
                try
                {
                    accuracy = float.Parse(Console.ReadLine());
                }
                catch { }

                KahootChallenge challenge = KahootChallenge.Get(game);
                KahootSolver solver = new KahootSolver(challenge, name, accuracy);
                solver.OnQuestionAnswered += (question, success) =>
                {
                    if (success)
                        Logger.WriteInfoLine($"Answered to Q{Array.IndexOf(challenge.Questions, question)} [{question.Title}] successfuly!");
                    else
                        Logger.WriteInfoLine($"Something went wrong when was trying to answer Q{Array.IndexOf(challenge.Questions, question)} [{question.Title}]");
                };
                if (answer == "n")
                    solver.Run();
                else
                {
                    Task task = Task.Run(solver.RunAsync);
                    task.Wait();
                }
            }
            catch (Exception e)
            {
                Logger.WriteErrorLine(e);
            }
        }
        public static void SpamWithBots()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter game link, challenge id or pin");
            Console.ResetColor();
            Console.Write("➤ ");
            string game = Console.ReadLine();
            if (game == null)
                return;
            KahootChallenge challenge = null;
            try
            {
                challenge = KahootChallenge.Get(game);
            }
            catch (Exception e)
            {
                Logger.WriteErrorLine(e);
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter nickname of bots (empty for random), lenght should be between 2 and 47");
            Console.ResetColor();
            Console.Write("➤ ");
            string nick = Console.ReadLine().Trim();
            if ((nick.Length < 2 || nick.Length > 47) && nick.Length != 0)
            {
                Console.WriteLine("Nickname lenght should be between 2 and 47");
                return;
            }
            KahootBotSpamer spamer = null;
            if (challenge != null) 
                spamer = new KahootBotSpamer(nick, challenge);
            else
            {
                Logger.WriteErrorLine("For some reason kahoot is private");
                return;
            }

            spamer.OnBotSent += (bot) =>
            {
                Logger.WriteInfoLine($"Bot number {spamer.Spamed} has been sent {bot}");
            };
            string answer = "";
            while (answer != "y" && answer != "n")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Use async method (y/n)?");
                Console.ResetColor();
                Console.Write("➤ ");
                answer = Console.ReadLine().ToLower();
            }
            if (answer == "n")
                spamer.Spam();
            else
            {
                Task task = Task.Run(spamer.SpamAsync);
                task.Wait();
            }
            Console.ResetColor();
        }
        public static bool KahootExists(string id, out KahootGame game)
        {
            game = null;
            if (id.Length == 7 && int.TryParse(id, out _))
                return Kahoot.KahootIsPrivate(id);
            try
            {
                game = KahootGame.Get(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool KahootIsPrivate(string id)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_PRIVATE_KAHOOT, id));
            HttpResponseMessage response = httpClient.Send(request);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        public static void BruteForceKahoots()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Enter start pin");
            Console.ResetColor();
            Console.Write("➤ ");
            string startPinStr = Console.ReadLine();
            if (!int.TryParse(startPinStr, out int startPin) || startPin > KahootConstants.MAX_KAHOOT_PIN)
            {
                Console.WriteLine($"Start pin must be a number between 0 and {KahootConstants.MAX_KAHOOT_PIN}");
                return;
            }
            KahootBruteForcer bruteForcer = new KahootBruteForcer(startPin);
            bruteForcer.OnKahootFound += WriteInfo;
            bruteForcer.BruteForce();
        }
    }
}
