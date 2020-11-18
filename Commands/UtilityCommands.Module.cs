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
            await ctx.RespondAsync("You're an idiot.");
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
            var url = "https://pogoda.wprost.pl/prognoza-pogody/" + city;

            if (!JackTheStudent.Program.weatherCities.Contains(city)){
                await ctx.RespondAsync("This city is not supported, sorry!");
                return;
            }

            var tableTempTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[1]";
            var tablePressureTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[2]";
            var tableWindTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[3]";
            var tableHumidityTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[4]";
            var tableCloudinessTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[5]";

            var tempTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[1]/span";
            var pressureTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[2]/span";
            var WindTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[3]/span";
            var humidityTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[4]/span";
            var cloudinessTodayXpath = "/html/body/div[2]/div/section[1]/section/div/div[1]/dl/dd[5]/span";

            var htmlDoc = new HtmlWeb().Load(url);

            var tableTempToday = htmlDoc.DocumentNode.SelectNodes(tableTempTodayXpath).First();
            var temp = tableTempToday.SelectNodes(tempTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tablePressureToday = htmlDoc.DocumentNode.SelectNodes(tablePressureTodayXpath).First();
            var pressure = tablePressureToday.SelectNodes(pressureTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableWindToday = htmlDoc.DocumentNode.SelectNodes(tableWindTodayXpath).First();
            var wind = tableWindToday.SelectNodes(WindTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableHumidityToday = htmlDoc.DocumentNode.SelectNodes(tableHumidityTodayXpath).First();
            var humidity = tableHumidityToday.SelectNodes(humidityTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();

            var tableCloudinessToday = htmlDoc.DocumentNode.SelectNodes(tableCloudinessTodayXpath).First();
            var cloudiness = tableCloudinessToday.SelectNodes(cloudinessTodayXpath).Select(n => n.GetDirectInnerText().Trim()).SingleOrDefault();
            
            var weatherEmbed = new DiscordEmbedBuilder
            {
                Title = ":sun_with_face: Weather for today :cloud_lightning:",
                Description = "Temperature: "+temp+"°C"+"\nPressure: "+pressure+" hPa"+"\nWind: "+wind+" m/s"+"\nHumidity: "+humidity+"%"+"\nCloudiness: "+cloudiness+"%"+"\n\nHave a nice day!"
            };
            
            await ctx.RespondAsync(embed: weatherEmbed);
        }
    }
}
