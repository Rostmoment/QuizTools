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
        public string[] Choices => choices.ToArray();
        private string[] choices;
        public KahootJumbleQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            List<string> correctOrder = new();
            if (jSON.TryGetProperty("choices", out JsonElement answers))
            {
                foreach (JsonElement choice in answers.EnumerateArray())
                {
                    correctOrder.Add(WebUtility.HtmlDecode(choice.GetProperty("answer").GetString()));
                }
            }

            choices = correctOrder.ToArray();
            correctAnswer = new KahootAnswer(this, Enumerable.Range(0, choices.Length).ToArray());
        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" - Correct Order: {CorrectAnswer.Inputs.ToSeparatedString(" --- ")}");
        }

        public override async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, KahootAnswer answer)
        {
            await base.AnswerAsync(challenge, player, client, answer);
            ArgumentNullException.ThrowIfNull(answer.Answers, nameof(answer.Answers));

            if (answer.Answers.Length == 0)
                throw new ArgumentException("Array of answers should not be empty");

            int[] indexes = answer.Answers;
            if (!answer.IsCheated)
                indexes = indexes.Distinct().ToArray();

            if (indexes.Length != correctAnswer.Answers.Length)
                throw new ArgumentException($"Expected {correctAnswer.Answers.Length} answers, but received {indexes.Length}.");


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
                            isCorrect = true,
                            points = answer.Points,
                            selectedJumbleOrder = indexes
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
