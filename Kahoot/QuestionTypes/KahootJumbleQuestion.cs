using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootJumbleQuestion : BaseKahootQuestion
    {
        public string[] CorrectOrder { get; }

        public KahootJumbleQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            List<string> correctOrder = new();
            if (jSON.TryGetProperty("choices", out JsonElement choices))
            {
                foreach (JsonElement choice in choices.EnumerateArray())
                {
                    correctOrder.Add(WebUtility.HtmlDecode(choice.GetProperty("answer").GetString()));
                }
            }
            CorrectOrder = correctOrder.ToArray();
        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" - Correct Order: {CorrectOrder.ToSeperateString(" --- ")}");
        }

        public override async Task<bool> AnswerCorrectAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, float accuracy = 1)
        {
            await base.AnswerCorrectAsync(challenge, player, client, accuracy);
            int pointsToGive = CalculatePoints(accuracy);
            int time = CalculateReactionTime(pointsToGive, challenge);

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
                            selectedJumbleOrder = Enumerable.Range(0, CorrectOrder.Length).ToArray()
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
