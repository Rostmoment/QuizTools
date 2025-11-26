using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootPinQuestion : BaseKahootQuestion
    {
        public float Width { get; } 
        public float Height { get; }
        public float X { get; }
        public float Y { get; }

        public bool IsDropPin => Type == KahootQuestionType.DropPin;

        public KahootPinQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            JsonElement pinData;

            if (Type == KahootQuestionType.PinIt)
            {
                pinData = jSON.GetProperty("choiceShapes").EnumerateArray().First();
                X = (float)pinData.GetDoubleOrDefault("x");
                Y = (float)pinData.GetDoubleOrDefault("y");
            }
            else
            {
                pinData = jSON.GetProperty("imageMetadata");
                X = float.NaN;
                Y = float.NaN;
            }
            Width = pinData.GetInt32OrDefault("width");
            Height = pinData.GetInt32OrDefault("height");

            if (!IsDropPin)
                correctAnswer = new KahootAnswer(this, (X + Width / 2) / Image.Width * 100, (Y + Height / 2) / Image.Height * 100);
        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (Type == KahootQuestionType.PinIt)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" - Correct answer at X={CorrectAnswer.XY.Value.X}, Y={CorrectAnswer.XY.Value.Y}");
            }
            else
                Console.WriteLine($" - This is a Drop Pin question, no correct answer. Width={Width}, Height={Height}, X={X}, Y={Y}");
        }

        public override async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, KahootAnswer answer)
        {
            await base.AnswerAsync(challenge, player, client, answer);

            ArgumentNullException.ThrowIfNull(answer.XY, nameof(answer.XY));

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
                            pin = new
                            {
                                x = answer.XY.Value.X,
                                y = answer.XY.Value.Y
                            }
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
