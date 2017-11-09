namespace beerbot.Dialogs
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using beerbot.BeerData;
    using System.Collections.Generic;

    [Serializable]
    internal class ReceiveAttachmentDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Attachments != null && message.Attachments.Any())
            {
                var attachment = message.Attachments.First();
                using (HttpClient httpClient = new HttpClient())
                {
                    // Skype & MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                    if ((message.ChannelId.Equals("skype", StringComparison.InvariantCultureIgnoreCase) || message.ChannelId.Equals("msteams", StringComparison.InvariantCultureIgnoreCase))
                        && new Uri(attachment.ContentUrl).Host.EndsWith("skype.com"))
                    {
                        var token = await new MicrosoftAppCredentials().GetTokenAsync();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);
                    var contentLenghtBytes = responseMessage.Content.Headers.ContentLength;
                    var picBytes = await responseMessage.Content.ReadAsByteArrayAsync();

                    BeerVision beerVision = new BeerVision();
                    var beerBrand = await beerVision.IdentifyBeer(picBytes);

                    if (beerBrand != null)
                    {
                        //await context.PostAsync($"Parece que se trata de una cerveza {beerBrand}");
                        await ShowBeerData(beerBrand, context, argument);
                    }
                    else
                    {
                        await context.PostAsync("lo siento , no encuentro ninguna cerveza en la imágen");
                    }

                    //await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLenghtBytes} bytes received.");
                }
            }
            else
            {
                await context.PostAsync("Hi there! I'm a bot created to show you how I can receive message attachments, but no attachment was sent to me. Please, try again sending a new message including an attachment.");
            }

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ShowBeerData(string beerName, IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            BreweryDB breweryDB = new BreweryDB();
            var searchResults = await breweryDB.SearchBeer(beerName);
            if (searchResults.data != null & searchResults.data.Count > 0)
            {
                var beerData = searchResults.data[0];

                if (beerData.labels != null)
                {
                    var pic = await GetInternetAttachment(beerData.labels.medium);
                    var replyMessage = context.MakeMessage();
                    replyMessage.Attachments = new List<Attachment> { pic };
                    await context.PostAsync(replyMessage);
                }

                var beerInfoString = await GetBeerDataString(beerData);
                await context.PostAsync(beerInfoString);
            }
        }

        private async Task<string> GetBeerDataString(BeerData beerData)
        {
            string result = string.Empty;

            var beerStyle = beerData.style != null ? beerData.style.shortName : string.Empty;
            var beerIBU = beerData.ibu != null ? $" - {beerData.ibu} IBU" : string.Empty;

            result = $"{beerData.nameDisplay}"
                + $"{Environment.NewLine} {beerStyle}"
                + $"{Environment.NewLine} {beerData.abv}% ABV {beerIBU}"
                + $"{Environment.NewLine}{Environment.NewLine} {beerData.description}";


            return result;
        }

        private async Task<Attachment> GetInternetAttachment(string picUrl)
        {
            return new Attachment
            {
                Name = "BotFrameworkOverview.png",
                ContentType = "image/png",
                ContentUrl = picUrl
            };
        }

    }
}