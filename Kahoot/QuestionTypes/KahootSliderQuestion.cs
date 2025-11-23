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
    class KahootSliderQuestion : BaseKahootQuestion
    {
        public int Step { get; }
        public int Correct { get; }
        public int Min { get; }
        public int Max { get; }
        public int Count => (Max - Min) / Step + 1;

        public KahootSliderQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            JsonElement slider = jSON.GetProperty("choiceRange");
            Step = (int)slider.GetProperty("step").GetDouble();
            Correct = (int)slider.GetProperty("correct").GetDouble();
            Min = (int)slider.GetProperty("start").GetDouble() + Correct % Step;
            Max = (int)slider.GetProperty("end").GetDouble() - Step + Correct % Step;

        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" - Answer = {Correct}\n - Min = {Min}\n - Max = {Max}\n - Step = {Step}\n - Count = {Count}");
        }


        public override async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, int points)
        {
            await base.AnswerCorrectAsync(challenge, player, client, points);
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
                            isAlmostCorrect = false,
                            points = pointsToGive,
                            choiceValue = Correct
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
