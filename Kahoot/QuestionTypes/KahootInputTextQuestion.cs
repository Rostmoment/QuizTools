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
    class KahootInputTextQuestion : BaseKahootQuestion
    {
        public KahootInputTextQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            if (JSON.TryGetProperty("choices", out JsonElement element))
            {
                List<string> list = new List<string>();
                JsonElement.ArrayEnumerator array = element.EnumerateArray();
                foreach (JsonElement choice in array)
                {
                    if (choice.TryGetProperty("answer", out JsonElement answer) && choice.GetBooleanOrDefault("correct"))
                        list.Add(answer.GetString());
                }
                correctAnswer = new KahootAnswer(this, list.ToArray());
            }
            else
                correctAnswer = null;
        }
        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (CorrectAnswer.Inputs.ArrayIsNullOrEmpty())
            {
                Console.WriteLine("No answers!");
                return;
            }
            foreach (string answer in CorrectAnswer.Inputs)
                Console.WriteLine($" - {answer}");
        }

        public override async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, KahootAnswer answer)
        {
            await base.AnswerAsync(challenge, player, client, answer);

            ArgumentNullException.ThrowIfNull(answer.Inputs, nameof(answer.Inputs));
            if (answer.Inputs.Length == 0)
                throw new ArgumentException("Array of answers should not be empty");

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
                            points = answer.Points,
                            text = answer.Inputs[0],
                            originalText = answer.Inputs[0]
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
