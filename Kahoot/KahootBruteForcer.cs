using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot
{
    class KahootBruteForcer
    {
        private string PublicKahootsFileName { get; }
        private int StartPin { get; }
        private List<KahootChallenge> found = new();

        public Action<KahootChallenge> OnKahootFound { get; set; }

        private void SaveToFile()
        {
            if (found.Count == 0)
                return;

            List<string[]> data = new List<string[]>()
            {
                new string[] {"Title", "Language", "Pin", "Host", "Host Link", "Start", "End", "Max Players", "Players", "Questions Number", "Details JSON", "Quiz JSON", "Challenge JSON", "Pin JSON", "Join" }
            };
            foreach (KahootChallenge challenge in found)
            {
                data.Add(new string[]
                {
                    challenge.Name,
                    challenge.Language,
                    challenge.Pin,
                    challenge.Host.ToString(),
                    challenge.Host.Link,
                    challenge.StartTime.ToString(),
                    challenge.EndTime.ToString(),
                    challenge.MaxPlayers.ToString(),
                    challenge.Players.ToSeperateString(", "),
                    challenge.NumberOfQuestions.ToString(),
                    challenge.DetailsLink,
                    challenge.QuizJSONLink,
                    challenge.ChallengeIDJSONLink,
                    challenge.PinJSONLink,
                    challenge.JoinLink,
                });
            }
            var csvLines = data.Select(row => string.Join(",", row.Select(field =>
            {
                if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                {
                    return $"\"{field.Replace("\"", "\"\"")}\"";
                }
                return field;
            })));

            File.WriteAllLines(PublicKahootsFileName, csvLines);
        }

        
        public KahootBruteForcer(int startPin)
        {
            string now = DateTime.Now.ToString("yyyy.MM.dd HH.MM.ss");
            PublicKahootsFileName = $"{now}.csv";
            StartPin = startPin;

        }
        public void BruteForce()
        {
            bool stopRequested = false;
            void Stop(object? sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                stopRequested = true;
                Console.CancelKeyPress -= Stop;
            }

            Console.CancelKeyPress += Stop;


            for (int i = StartPin; i <= KahootConstants.MAX_KAHOOT_PIN; i++)
            {
                if (stopRequested)
                {
                    Logger.WriteInfoLine("\nStopping brute force...");
                    Logger.WriteInfoLine($"{found.Count} kahoot(s) was/were found!");
                    break;
                }
                string pin = i.ToString("D8");
                Logger.WriteInfo($"Checking pin {pin}...       ", "\r");
                if (Kahoot.KahootExists(pin, out KahootGame game))
                {
                    if (game == null)
                        continue;

                    if (game is KahootChallenge challenge)
                    {
                        OnKahootFound?.Invoke(challenge);
                        found.Add(challenge);
                    }
                }
            }
            SaveToFile();
        }
    }
}
