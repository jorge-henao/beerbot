﻿namespace beerbot.Dialogs
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
    using beerbot.BeerData.Untappd;

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
                        if (beerBrand == "redds")
                        {
                            var gayPic = await GetInternetAttachment("https://pbs.twimg.com/media/C38GsoGUMAQNWKw.jpg");
                            var replyMessage = context.MakeMessage();
                            replyMessage.Attachments = new List<Attachment> { gayPic };
                            await context.PostAsync(replyMessage);

                            await context.PostAsync("¡eso no es una cerveza! es una bebida para m4%1c4$");
                        }
                        else if (beerBrand == "desconocida")
                        {
                            await context.PostAsync($"Parece que se trata de una cerveza {beerBrand}");
                        }
                        else
                        {
                            await ShowBeerData(beerBrand, context, argument);
                        }
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
                await context.PostAsync("Hola! deberías enviarme la foto de una cerveza, inténtalo de nuevo.");
            }

            //context.Done("ok");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ShowBeerData(string beerName, IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            
            BeerSearcher breweryDB = new BeerSearcher();
            var searchResults = await breweryDB.SearchBeer(beerName);
            if (searchResults.response.beers.count > 0)
            {
                var beerData = searchResults.response.beers.items[0].beer;
                if (beerData != null)
                {
                    //var pic = await GetInternetAttachment(beerData.labels.medium);
                    var pic = await GetInternetAttachment(beerData.beer_label);
                    var replyMessage = context.MakeMessage();
                    replyMessage.Attachments = new List<Attachment> { pic };
                    await context.PostAsync(replyMessage);
                }

                var beerInfoString = await GetBeerDataString(beerData);
                await context.PostAsync(beerInfoString);
            }
        }

        private async Task<string> GetBeerDataString(Beer beerData)
        {
            string result = string.Empty;

            var beerStyle = beerData.beer_style != null ? beerData.beer_style : string.Empty;
            var beerIBU = $" - {beerData.beer_ibu} IBU";

            result = $"{beerData.beer_name}"
                + $"{Environment.NewLine} {beerStyle}"
                + $"{Environment.NewLine} {beerData.beer_abv} % ABV {beerIBU}"
                + $"{Environment.NewLine}{Environment.NewLine} {beerData.beer_description}";


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