using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace beerbot.Dialogs
{

    [Serializable]
    public class BeerOrderForm
    {
        [Prompt("¿Qué cerveza quieres ordenar?")]
        public string Beer{ get; set; }

        [Prompt("¿Cuantas cervezas quieres?")]
        public int Ammount{ get; set; }

        [Prompt("¿A donde quieres que te las llevemos?")]
        public string addressOrder { get; set; }

    }

    public class BeerOrderDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Vamos a empezar con algunas preguntas para verificar tu ubicación");

            var hotelsFormDialog = FormDialog.FromForm(BuildDialogForm, FormOptions.PromptInStart);
            context.Call(hotelsFormDialog, OnFormCompleted);
        }

        public static async Task OnFormCompleted(IDialogContext context, IAwaitable<BeerOrderForm> result)
        {
            //await context.PostAsync($"He abierto el tiquete de soporte #{new Random().Next(1, int.MaxValue) } para que un experto revise tu caso en lás próximas horas");
        }

        public static IForm<BeerOrderForm> BuildDialogForm()
        {
            OnCompletionAsyncDelegate<BeerOrderForm> completedOrder = async (context, state) =>
            {
                await context.PostAsync($"Listo, tu pedido de {state.Ammount} cervezas {state.Beer} está en camino a {state.addressOrder}. {Environment.NewLine}¿Te puedo ayudar en algo mas?");

            };

            return new FormBuilder<BeerOrderForm>()
                .AddRemainingFields()
                .OnCompletion(completedOrder)
                .Build();
        }

    }
}