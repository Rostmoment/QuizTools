using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;
using System.Numerics;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootSliderQuestion : BaseKahootQuestion
    {
        public int Step { get; }
        public int Start { get; }
        public int End { get; }
        public int Min { get; }
        public int Max { get; }
        public int Tolerance { get; }
        public Vector2 AlmostCorrectRange => new Vector2(CorrectAnswer.Value.Value - Tolerance, CorrectAnswer.Value.Value + Tolerance);
        public int Count => (Max - Min) / Step + 1;

        public KahootSliderQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            JsonElement slider = jSON.GetProperty("choiceRange");
            int correct = (int)slider.GetProperty("correct").GetDouble();

            Step = (int)slider.GetProperty("step").GetDouble();
            Start = (int)slider.GetProperty("start").GetDouble();
            End = (int)slider.GetProperty("end").GetDouble();
            Min = Start + correct % Step;
            Max = End - Step + correct % Step;
            Tolerance = (int)(End * slider.GetProperty("tolerance").GetDouble());

            correctAnswer = new KahootAnswer(this, correct);
        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" - Answer = {CorrectAnswer.Value.Value}\n - Min = {Min}\n - Max = {Max}\n - Step = {Step}\n - Count = {Count}");
        }


        public override async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, KahootAnswer answer)
        {
            await base.AnswerAsync(challenge, player, client, answer);
            ArgumentNullException.ThrowIfNull(answer.Value, nameof(answer.Value));

            var payload = new
            {
                quizId = challenge.QuizID,
                quizType = "quiz",
                numQuestions = challenge.NumberOfQuestions,
                startTime = challenge.StartTime.ToUnixTimeMilliseconds(),
                question = new
                {
                    index = Index,
                    answers = new[]
                    {
                        new
                        {
                            reactionTime = answer.ReactionTime,
                            playerId = player.Name,
                            playerCid = player.ID,
                            isCorrect = answer.IsCorrect,
                            isAlmostCorrect = answer.AlmostCorrect,
                            points = answer.Points,
                            choiceValue = answer.Value.Value
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
