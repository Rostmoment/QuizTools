using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Exceptions;
using QuizTools.Kahoot.Games;
using QuizTools.Kahoot.QuestionTypes;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace QuizTools.Kahoot
{
    class KahootSolver
    {
        private KahootChallenge challenge;
        private KahootPlayer player;
        private HttpClient client;
        private float accuracy;

        public KahootSolver(KahootChallenge challenge, string nickname, float accuracy = 1) : this(challenge, challenge.Join(nickname), accuracy) { }
        public KahootSolver(KahootChallenge challenge, KahootPlayer player, float accuracy = 1)
        {
            this.challenge = challenge;
            this.player = player;
            client = new HttpClient();
            this.accuracy = accuracy;
            if (!challenge.GameOptions.QuestionTimer)
                this.accuracy = 1;
        }

        public void Run()
        {
            for (int i = 0; i < challenge.NumberOfQuestions; i++)
            {
                BaseKahootQuestion question = challenge.Questions[i];
                if (question is KahootUnknownQuestion)
                {
                    Logger.WriteWarningLine($"Q{i + 1} is unkown, skipping it...");
                    continue;
                }
                Logger.WriteInfoLine($"Q{i + 1}: {question.Title}");
                if (question.AnswerCorrect(challenge, player, client, accuracy))
                    Logger.WriteInfoLine($"Successfuly answered to the question number {i + 1}!");
                else
                    Logger.WriteErrorLine($"Failed to answer the question number {i + 1}!");
            }
        }
        public async Task RunAsync()
        {
            for (int i = 0; i < challenge.NumberOfQuestions; i++)
            {
                BaseKahootQuestion question = challenge.Questions[i];
                if (question is KahootUnknownQuestion)
                {
                    Logger.WriteWarningLine($"Q{i + 1} is unkown, skipping it...");
                    continue;
                }
                if (await question.AnswerCorrectAsync(challenge, player, client, accuracy))
                    Logger.WriteInfoLine($"Successfuly answered to the question number {i + 1}!");
                else
                    Logger.WriteErrorLine($"Failed to answer the question number {i + 1}!");
            }
        }
    }
}
