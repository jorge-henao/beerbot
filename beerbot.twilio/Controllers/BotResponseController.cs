using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.DirectLine;


namespace beerbot.twilio.Controllers
{
    public class BotResponseController : TwilioController
    {
        // GET: Response
        public async Task<TwiMLResult> Index(VoiceRequest request)
        {
            DirectLineClient client = new DirectLineClient(Constants.BotDirectLineSecret);
            string conversationId = Request.QueryString[Constants.ConversationIdQSKey];
            if (string.IsNullOrEmpty(conversationId))
            {
                var conversation = await client.Conversations.StartConversationAsync();
                conversationId = conversation.ConversationId;
            }

            Activity userMessage = new Activity
            {
                From = new ChannelAccount("Twilio"),
                Text = Request["SpeechResult"],
                Type = ActivityTypes.Message
            };
            await client.Conversations.PostActivityAsync(conversationId, userMessage);
            var result = await client.Conversations.GetActivitiesAsync(conversationId);
            var botStringResponse = result.Activities[result.Activities.Count - 1].Text;

            var response = new VoiceResponse();
            var urlRedirect = GetRedirectUrl(conversationId, botStringResponse);
            response.Redirect(urlRedirect, method: "GET");
            return TwiML(response);
        }

        public Uri GetRedirectUrl(string conversationId, string botStringResponse)
        {
            string result = "http://birrabottwilio.azurewebsites.net/voice";

            result += $"?{Constants.ConversationIdQSKey}={conversationId}" +
                $"&{Constants.MessageQSKey}={HttpUtility.UrlEncode(botStringResponse)}";

            return new Uri(result, UriKind.Absolute);
        }

    }
}