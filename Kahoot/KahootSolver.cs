using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using QuizTools.Kahoot.Exceptions;
using QuizTools.Kahoot.Games;
using QuizTools.Kahoot.QuestionTypes;

namespace QuizTools.Kahoot
{
    class KahootSolver
    {
        private KahootChallenge challenge;
        private KahootPlayer player;
        private HttpClient client;
        public Action<BaseKahootQuestion, bool> OnQuestionAnswered;

        public KahootSolver(KahootChallenge challenge, string nickname) : this(challenge, challenge.Join(nickname)) { }
        public KahootSolver(KahootChallenge challenge, KahootPlayer player)
        {
            this.challenge = challenge;
            this.player = player;
            client = new HttpClient();
        }

        public void Run()
        {
            for (int i = 0; i < challenge.NumberOfQuestions; i++)
            {
                BaseKahootQuestion question = challenge.Questions[i];
                if (question is KahootUnknownQuestion)
                {
                    Logger.WriteWarningLine($"Q{i + 1} is unknown, skipping it...");
                    continue;
                }
                if (question.CorrectAnswer == null)
                {
                    Logger.WriteWarningLine($"Q{i + 1} has no correct answer, skipping it...");
                    continue;
                }
                OnQuestionAnswered?.Invoke(question, question.Answer(challenge, player, client, question.CorrectAnswer));
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
                if (question.CorrectAnswer == null)
                {
                    Logger.WriteWarningLine($"Q{i + 1} has no correct answer, skipping it...");
                    continue;
                }
                OnQuestionAnswered?.Invoke(question, await question.AnswerAsync(challenge, player, client, question.CorrectAnswer));
            }
        }
    }
}
