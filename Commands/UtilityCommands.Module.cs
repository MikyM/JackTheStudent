using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharp​Plus;
using System;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Interactivity.Extensions;
using HtmlAgilityPack;
using JackTheStudent.CommandDescriptions; 

namespace JackTheStudent.Commands
{
public class UtilityCommandsModule : Base​Command​Module
{
    [Command("poll")]
    [Description(FunDescriptions.pollDescription)]
    public async Task Poll(CommandContext ctx, string topic = "", string options = "", string dur = "" , params DiscordEmoji[] emojiOptions)
    {
        var intr = ctx.Client.GetInteractivity();
        TimeSpan duration;

        if (topic == null) {
            await ctx.RespondAsync("No topic, dude, I'm out.");
            return;
        } else if(options == null) {
            await ctx.RespondAsync("No options, dude, I'm out.");
            return;
        } else
        if (emojiOptions.Length == 0){
            await ctx.RespondAsync("No emojis specified.");
            return;
        } else if (emojiOptions.Count() != options.Split(new Char [] {','}).Count()) {
            await ctx.RespondAsync("You must supply same amount of options and emoticons.");
            return;
        }

        try {
            duration = TimeSpan.Parse(dur);
        }
        catch (FormatException) {
            await ctx.RespondAsync(dur + " is not a valid format. Use hh:mm:ss format instead. Sorry for that.");
        return;
        } catch (OverflowException) {
            await ctx.RespondAsync("Overflow duuude. Seconds max is 60, same for minutes.");
        return;
        }

        if(duration.Days >= 1){
            await ctx.RespondAsync("Max poll duration is 23:59:59. Use hh:mm:ss format.");
            return;
        } 
 
        string [] optionsList = options.Split(new Char [] {','});

        var pollEmojiOptionsList = emojiOptions.Select(e => e.ToString()).ToList();

        string desc = "";
        for (int i = 0; i < optionsList.Length; i++) {
            desc = $"{desc} \n{optionsList[i]} - {pollEmojiOptionsList[i]}";
        }
        var pollEmbed = new DiscordEmbedBuilder
        {
            Title = "The poll is about: " + topic,
            Description = $"To show others what you think react with:{desc}\n\nYou've got {duration}.\nGo go go!"
        };

        var pollMsg = await ctx.RespondAsync(embed: pollEmbed);

        foreach(var option in emojiOptions){
            await pollMsg.CreateReactionAsync(option).ConfigureAwait(false);
            await Task.Delay(500);
        }

        var result = await intr.CollectReactionsAsync(pollMsg, duration).ConfigureAwait(false);
        var disctinctResult = result.Distinct();
        var results = disctinctResult.Select(x => $"{x.Emoji}: {x.Total}");

        await ctx.RespondAsync("The poll about: " + topic + " has ended. Results:\n" + string.Join("\n", results) + "\nI hope you all enjoyed!");
    }    


    [Command("weather")]
    [Description(FunDescriptions.weatherDescription)]
    public async Task Weather(CommandContext ctx, string city = "")
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding(28591).GetBytes(city);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes).Replace(" ", "-").ToLower();

            if (asciiStr == null){
                await ctx.RespondAsync("This city is not supported sorry!");
                return;
            }
            
            var url = "https://pogoda.net/" + asciiStr;

            var tableTempTodayXpath = "/html/body/div[2]/div[2]/div[1]/div/em";
            var tablePressureTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[2]";
            var tableWindTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[5]";
            var tableHumidityTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[4]";
            var tableTempFeelingTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[3]";
            var tableDateTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[1]";

            var tempTodayXpath = "/html/body/div[2]/div[2]/div[1]/div/em/text()";
            var pressureTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[2]/strong"; 
            var windTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[5]/strong";
            var humidityTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[4]/strong";
            var tempFeelingTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[3]/strong";
            var dateTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[1]/span";
            var timeTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[1]/strong";

            if (asciiStr == "lodz" || asciiStr == "zielona-gora" || asciiStr == "wroclaw" || asciiStr == "warszawa" || asciiStr == "szczytno" || 
            asciiStr == "szczecin" || asciiStr == "poznan" || asciiStr == "rzeszow" || asciiStr == "katowice" || asciiStr == "krakow" || 
            asciiStr == "bydgoszcz"){
                tableTempTodayXpath = "/html/body/div[2]/div[2]/div[1]/div/em";
                tablePressureTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[3]";
                tableWindTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[6]";
                tableHumidityTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[5]";
                tableTempFeelingTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[4]";
                tableDateTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[2]";

                tempTodayXpath = "/html/body/div[2]/div[2]/div[1]/div/em/text()";
                pressureTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[3]/strong"; 
                windTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[6]/strong";
                humidityTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[5]/strong";
                tempFeelingTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[4]/strong";
                dateTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[2]/span";
                timeTodayXpath = "/html/body/div[2]/div[2]/div[1]/p[2]/strong";
            }

            var htmlDoc = new HtmlWeb().Load(url);

            var tableTempToday = htmlDoc.DocumentNode.SelectNodes(tableTempTodayXpath).First();
            var temp = tableTempToday.SelectNodes(tempTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tablePressureToday = htmlDoc.DocumentNode.SelectNodes(tablePressureTodayXpath).First();
            var pressure = tablePressureToday.SelectNodes(pressureTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableWindToday = htmlDoc.DocumentNode.SelectNodes(tableWindTodayXpath).First();
            var wind = tableWindToday.SelectNodes(windTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableHumidityToday = htmlDoc.DocumentNode.SelectNodes(tableHumidityTodayXpath).First();
            var humidity = tableHumidityToday.SelectNodes(humidityTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableTempFeelingToday = htmlDoc.DocumentNode.SelectNodes(tableTempFeelingTodayXpath).First();
            var tempFeeling = tableTempFeelingToday.SelectNodes(tempFeelingTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableDateToday = htmlDoc.DocumentNode.SelectNodes(tableDateTodayXpath).First();
            var date = tableDateToday.SelectNodes(dateTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();
            var time = tableDateToday.SelectNodes(timeTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            
            
            var weatherEmbed = new DiscordEmbedBuilder
            {
                Title = ":sun_with_face: Weather for today :cloud_lightning:",
                Description = "Temperature: "+temp+"°C"+"\nFeeling temp: "+tempFeeling.Replace("&deg;","°C")+
                    "\nPressure: "+pressure+"\nWind: "+wind+"\nHumidity: "+humidity+"\n\nLast updated: "+date+" "+time+"\n\nHave a nice day! :hugging:"
            };
            
            await ctx.RespondAsync(embed: weatherEmbed);
        }
    }
}
