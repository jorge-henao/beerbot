using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace beerbot.Dialogs
{
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private readonly string channelId;

        public RootLuisDialog(string channelId) : base(new LuisService(new LuisModelAttribute("5761a0de-5321-42da-be00-1f2f1d702549", "5e9cbea2a30b49f184b2cbcad31b94f3")))
        {
            this.channelId = channelId;
        }


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Lo siento, aun no estoy preparado para entender que quisiste decir con '{result.Query}'. Escribe 'ayuda' si necesitas asistencia";
            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Saludo")]
        public async Task Saludar(IDialogContext context, LuisResult result)
        {
            string message = $"Hola ¿En qué te puedo ayudar?";
            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Agregar-meta-ahorro")]
        public async Task AgregarMetaAhorro(IDialogContext context, LuisResult result)
        {

        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'search hotels in Seattle', 'search hotels near LAX airport' or 'show me the reviews of The Bot Resort'");

            context.Wait(this.MessageReceived);
        }
    }
}