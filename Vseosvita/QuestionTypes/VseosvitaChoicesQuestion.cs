using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita.QuestionTypes
{
    class VseosvitaChoicesQuestion : BaseVseosvitaQuestion
    {
        public string[] Answers { get; }
        public bool IsMultipleChoice => Type == VseosvitaQuestionType.MultipleAnswers;

        public VseosvitaChoicesQuestion(JsonElement data) : base(data)
        {
            Answers = data.GetProperty("data").GetProperty("testQuests").EnumerateArray().First()
                .GetProperty("answer_arr").EnumerateArray().Select(x => WebUtility.HtmlDecode(x.GetString())).ToArray();
        }

        public override async Task<BaseVseosvitaQuestion> AnswerQuestionAsync(HttpClient client, VseosvitaUser user, params object[] answers)
        {
            await base.AnswerQuestionAsync(client, user, answers);

            List<int> answerIndexes = new List<int>();
            for (int i = 0; i < answers.Length; i++)
            {
                if (answers[i] is int answerIndex)
                {
                    answerIndexes.Add(answerIndex);
                }
                else
                    throw new ArgumentException($"Answer at position {i} is not an integer index", nameof(answers));
            }

            Dictionary<string, string> postData = new Dictionary<string, string>
            {
                { "id_quest", ID.ToString() },
                { $"files[{ID}]", "" }
            };

            if (!IsMultipleChoice && answerIndexes.Count > 0)
                postData.Add($"answers[{ID}][0]", answerIndexes[0].ToString());
            else
            {
                for (int i = 0; i < answerIndexes.Count; i++)
                    postData.Add($"answers[{ID}][{i}]", answerIndexes[i].ToString());
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, string.Format(VseosvitaConstants.ANSWER_QUESTION_URL, user.UserKey))
            {
                Content = content
            };
            requestMessage.Headers.Referrer = new Uri(VseosvitaConstants.GO_OPL_URL);
            requestMessage.Headers.Add("accept", "application/json, text/javascript, */*; q=0.01");
            requestMessage.Headers.Add("x-requested-with", "XMLHttpRequest");

            Logger.WriteInfoLine($"Answering question ID {ID} with answers: {requestMessage}");

            HttpResponseMessage response = await client.SendAsync(requestMessage);
            string responseContent = await response.Content.ReadAsStringAsync();
            Logger.WriteInfoLine($"Received answer response: {responseContent}");
            return null;
        }
    }
}
