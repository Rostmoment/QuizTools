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
        public double Width { get; } 
        public double Height { get; }
        public double X { get; }
        public double Y { get; }

        public double CorrectX
        {
            get
            {
                if (IsDropPin)
                    return double.NaN;
                return (X + Width / 2) / Image.Width * 100;
            }
        }
        public double CorrectY
        {
            get
            {
                if (IsDropPin)
                    return double.NaN;
                return (Y + Height / 2) / Image.Height * 100;
            }
        }
        public bool IsDropPin => Type == KahootQuestionType.DropPin;

        public KahootPinQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            JsonElement pinData;

            if (Type == KahootQuestionType.PinIt)
            {
                pinData = jSON.GetProperty("choiceShapes").EnumerateArray().First();
                X = pinData.GetDoubleOrDefault("x");
                Y = pinData.GetDoubleOrDefault("y");
            }
            else
            {
                pinData = jSON.GetProperty("imageMetadata");
                X = double.NaN;
                Y = double.NaN;
            }
            Width = pinData.GetInt32OrDefault("width");
            Height = pinData.GetInt32OrDefault("height");
        }

        public override void WriteAnswers()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (Type == KahootQuestionType.PinIt)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" - Correct answer at X={X}, Y={Y}. Width={Width}, Height={Height}");
            }
            else
                Console.WriteLine($" - This is a Drop Pin question, no correct answer. Width={Width}, Height={Height}");
        }

        public override async Task<bool> AnswerCorrectAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, float accuracy = 1)
        {
            await base.AnswerCorrectAsync(challenge, player, client, accuracy);
            double correctX = Program.RNG.NextDouble() * 100;
            double correctY = Program.RNG.NextDouble() * 100;
            int pointsToGive = CalculatePoints(accuracy);
            int time = CalculateReactionTime(pointsToGive, challenge);

            if (!IsDropPin)
            {
                correctX = CorrectX + Program.RNG.Next(1, 2) + Program.RNG.NextDouble() * Program.RNG.RandomSign();
                correctY = CorrectY + Program.RNG.Next(1, 2) + Program.RNG.NextDouble() * Program.RNG.RandomSign();
            }

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
                            pin = new
                            {
                                x = correctX,
                                y = correctY
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
