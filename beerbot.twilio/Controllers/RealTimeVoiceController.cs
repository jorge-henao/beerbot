using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.DirectLine;

namespace beerbot.twilio.Controllers
{
    public class RealTimeVoiceController : TwilioController
    {
        int mayorVoiceSequence = 0;
        string speechResult = string.Empty;

        DateTime lastPieceReceivedTime = DateTime.Now;

        // GET: RealTimeVoice
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            var voiceSequence = Convert.ToInt32(Request["SequenceNumber"]);
            if (voiceSequence > mayorVoiceSequence)
            {
                mayorVoiceSequence = voiceSequence;
                speechResult = Request["UnstableSpeechResult"];
            }

            //Trace.TraceInformation($"Sequence number: '{Request["SequenceNumber"]}'");
            Trace.TraceInformation($"Unstable Speech result: '{Request["UnstableSpeechResult"]}'");
            //Trace.TraceInformation($"Mayor voice sequence: '{mayorVoiceSequence}'");

            return new System.Web.Mvc.HttpStatusCodeResult(200);
        }
    }
}