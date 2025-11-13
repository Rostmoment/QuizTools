using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootTypeQuestion : BaseKahootQuestion
    {
        private string[] answers;
        public KahootTypeQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            answers = [];
            if (JSON.TryGetProperty("choices", out JsonElement element))
            {
                JsonElement.ArrayEnumerator array = element.EnumerateArray();
                List<string> list = new List<string>();
                foreach (JsonElement choice in array) {
                    if (choice.TryGetProperty("answer", out JsonElement answer) && choice.GetBooleanOrDefault("correct"))
                        list.Add(answer.GetString());
                }
                answers = list.ToArray();
            }
        }
        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (answers.Length == 0)
            {
                Console.WriteLine("No answers!");
                return;
            }
            foreach (string answer in answers)
                Console.WriteLine($" - {answer}");
        }

        public override async Task<bool> AnswerCorrectAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, float accuracy = 1)
        {
            await base.AnswerCorrectAsync(challenge, player, client, accuracy);
            int pointsToGive = CalculatePoints(accuracy);
            int time = CalculateReactionTime(pointsToGive, challenge);
            string answer = "123";
            if (answers.Length > 0)
                answer = Program.RNG.ChoseRandom(answers);
            
            var payload = new
            {
                quizId = challenge.QuizID,
                quizType = "quiz",
                numQuestions = challenge.NumberOfQuestions,
                startTime = challenge.StartTime.ToUnixTimeMilliseconds(),
                question = new
                {
                    index = Array.IndexOf(challenge.Questions, this),
                    answers = new[]
                    {
                        new
                        {
                            reactionTime = time,
                            playerId = player.Name,
                            playerCid = player.ID,
                            isCorrect = true,
                            points = pointsToGive,
                            text = answer
                        }
                    }
                }
            };
            string json = JsonSerializer.Serialize(payload);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, challenge.AnswersLink);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
