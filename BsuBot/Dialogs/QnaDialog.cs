using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BsuBot.Dialogs
{
    [Serializable]
    
    public class QnaDialog : IDialog<object>
    {

        private const string CANT_FIND_TEXT = "I am sorry, I am not able to find an answer for your question.Can you reword your question by providing details and context";
        private const string LOW_SCORE_TEXT = "I was not able to find a suitable answer. Please rephrase/reword your question by providing more information";

        public QnaDialog()
        {

        }


        public class QnAMakerResult
        {
            [JsonProperty(PropertyName = "answers")]
            public List<Result> Answers { get; set; }
        }

        public class Result
        {
            [JsonProperty(PropertyName = "answer")]
            public string Answer { get; set; }

            [JsonProperty(PropertyName = "questions")]
            public List<string> Questions { get; set; }

            [JsonProperty(PropertyName = "score")]
            public double Score { get; set; }
        }


        public async Task StartAsync(IDialogContext context)
        {
            await AskToAskQuestion(context);

            // return Task.CompletedTask;
        }

        private async Task AskToAskQuestion(IDialogContext context)
        {
            await context.PostAsync("please type in your question or type **exit** to go back to menu");
            context.Wait(QuestionReceivedAsync);
        }

        private async Task QuestionReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.ToLower() == "exit")
            {
                context.Done(1);
                return;
            }

            await context.PostAsync(GetAnswer(activity.Text));
            await AskToAskQuestion(context);
        }

        private string GetAnswer(string query)
        {
            string responseString = string.Empty;

            var knowledgebaseId = Convert.ToString(ConfigurationManager.AppSettings["KNOWLEDGE_BASE_ID"], CultureInfo.InvariantCulture);

            //Build the URI  
            var builder = new UriBuilder(string.Format(Convert.ToString(ConfigurationManager.AppSettings["QNA_SERVICE_URL"], CultureInfo.InvariantCulture), knowledgebaseId));

            //Add the question as part of the body  
            var postBody = string.Format("{{\"question\": \"{0}\"}}", query);

            //Send the POST request  
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8  
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header  
                var qnamakerSubscriptionKey = Convert.ToString(ConfigurationManager.AppSettings["SUBSCRIPTION_KEY"], CultureInfo.InvariantCulture);
                // client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Authorization", $"EndpointKey {qnamakerSubscriptionKey}");
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }
            QnAMakerResult result = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
            if (result.Answers[0].Answer.Contains("No good match found in KB"))
                return CANT_FIND_TEXT;
            if (result.Answers[0].Score < 50)
                return LOW_SCORE_TEXT;
            return result.Answers[0].Answer;
        }

    }
}