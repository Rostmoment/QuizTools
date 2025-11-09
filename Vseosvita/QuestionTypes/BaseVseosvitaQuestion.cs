using QuizTools.GeneralUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita.QuestionTypes
{
    class BaseVseosvitaQuestion
    {
        public VseosvitaQuestionType Type { get; }
        public int ID { get; }
        public string Description { get; }
        public int Weight { get; }
        public JsonElement Data { get; }

        public BaseVseosvitaQuestion(JsonElement data)
        {
            JsonElement testQuest = data.GetProperty("data").GetProperty("testQuests").EnumerateArray().First();
            Type = (VseosvitaQuestionType)testQuest.GetProperty("quest_type").GetInt32();
            ID = testQuest.GetProperty("id").GetInt32();
            Description = testQuest.GetStringOrDefault("quest_desc", "");
            Weight = testQuest.GetProperty("weight").GetInt32();

            Data = data;
        }

        // I need to use array of objects here because different question types can have different answer types
        public BaseVseosvitaQuestion AnswerQuestion(VseosvitaUser user, params object[] answers) => AnswerQuestion(user.Client, user, answers);
        public BaseVseosvitaQuestion AnswerQuestion(HttpClient client, VseosvitaUser user, params object[] answers) => AnswerQuestionAsync(client, user, answers).GetAwaiter().GetResult();

        public Task<BaseVseosvitaQuestion> AnswerQuestionAsync(VseosvitaUser user, params object[] answers) => AnswerQuestionAsync(user.Client, user, answers);
        public virtual async Task<BaseVseosvitaQuestion> AnswerQuestionAsync(HttpClient client, VseosvitaUser user, params object[] answers)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(user, nameof(user));

            if (!user.StartedTest)
                throw new InvalidOperationException("User has not started the test yet");

            return null;
        }

        public static BaseVseosvitaQuestion FromJSON(JsonElement element)
        {
            JsonElement testQuest = element.GetProperty("data").GetProperty("testQuests").EnumerateArray().First();
            VseosvitaQuestionType questionType = (VseosvitaQuestionType)testQuest.GetProperty("quest_type").GetInt32();
            return questionType switch
            {
                VseosvitaQuestionType.SingleAnswer or VseosvitaQuestionType.MultipleAnswers => new VseosvitaChoicesQuestion(element),
                _ => new BaseVseosvitaQuestion(element),
            };
        }
    }
}
