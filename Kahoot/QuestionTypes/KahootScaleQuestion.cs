using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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
    class KahootScaleQuestion : BaseKahootQuestion
    {
        public bool IsNPS => base.Type == KahootQuestionType.NPS;
        public int AnswersCount => MaxValue - MinValue + 1;
        public int MinValue { get; }
        public int MaxValue { get; }
        public string Label { get; }
        public string ScaleType { get; }

        public KahootScaleQuestion(JsonElement jSON) : base(jSON)
        {
            if (jSON.TryGetProperty("scaleRange", out JsonElement range))
            {
                MinValue = range.GetProperty("start").GetInt32();
                MaxValue = range.GetProperty("end").GetInt32();
                Label = WebUtility.HtmlDecode(range.GetProperty("labelType").GetString());
                ScaleType = range.GetProperty("type").GetString();
            }
            else
            {
                MinValue = 0;
                MaxValue = 10;
                Label = "N/A";
                ScaleType = "N/A";
            }
        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" - Scale from {MinValue} to {MaxValue} ({Label} --- {Type})");
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
                            choiceValue = Program.RNG.Next(MinValue, MaxValue + 1)
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
