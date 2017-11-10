using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.DirectLine;
using Twilio.TwiML.Voice;

namespace beerbot.twilio.Controllers
{
    public class VoiceController : TwilioController
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<TwiMLResult> Index(VoiceRequest request)
        {
            return await HandleIncomig(request);
        }

        private async Task<TwiMLResult> HandleIncomig(VoiceRequest request)
        {
            string promptMessage = string.IsNullOrWhiteSpace(Request.QueryString[Constants.MessageQSKey]) ? "Hola Soy Birra Bot. ¿en qué te puedo ayudar?" : Request.QueryString[Constants.MessageQSKey];

            var response = new VoiceResponse();
            var gather = new Gather(input: "speech", action: GetActionUrl(), speechTimeout : "auto", 
                language: Constants.Language, method: "POST", bargeIn: true);
            gather.Say(promptMessage, language: Constants.Language, voice: Constants.Voice);
            
            response.Gather(gather);
            //}

            response.Redirect(GetRedirectUrl());
            return TwiML(response, System.Text.Encoding.UTF8);
        }

        public Uri GetRedirectUrl()
        {
            string result = "http://birrabottwilio.azurewebsites.net/voice";

            if (Request.QueryString.AllKeys.Contains(Constants.MessageQSKey))
            {
                result += $"?{Constants.ConversationIdQSKey}={Request.QueryString[Constants.ConversationIdQSKey]}" +
                    $"&{Constants.MessageQSKey}={HttpUtility.UrlEncode(Request.QueryString[Constants.MessageQSKey])}";
            }

            return new Uri(result, UriKind.Absolute);
        }

        private Uri GetActionUrl()
        {
            var result = "http://birrabottwilio.azurewebsites.net/botResponse";

            if (Request.QueryString.AllKeys.Contains(Constants.ConversationIdQSKey))
                result += $"?{Constants.ConversationIdQSKey}={Request.QueryString[Constants.ConversationIdQSKey]}";

            return new Uri(result, UriKind.Absolute);
        }

    }
}