using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using Serilog;
using DSharpPlus.Entities;
using JackTheStudent.CommandDescriptions;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace JackTheStudent.Commands
{
public class MaterialCommandsModule : Base​Command​Module
{
    private string groupGuid = Environment.GetEnvironmentVariable("BITLY_GROUPGUID");
    private string bitlyToken = Environment.GetEnvironmentVariable("BITLY_TOKEN");

    [Command("material")]
    [Description(MaterialsDescriptions.MaterialLogDescription)]
    public async Task MaterialLog(CommandContext ctx,
        [Description ("\nTakes class' short names, type !classes to retrive all classes.\n")] string className = "", 
        [Description ("\nTakes any links or so. If you don't want to provide a link just type \".\"\n")] string link = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        if (className == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !test <group> <class> <testDate> <testTime> Try again!");
            return;      
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == className)) {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (link == ""){
            await ctx.RespondAsync("There's link missing, fix it!");
            return;
        } else {
            try {
                string baseUrl = "https://api-ssl.bitly.com/v4/shorten";
                string shortenedUrl = "";
                var content = new BitlyCall{
                    long_url = link,
                    group_guid = groupGuid
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                using (HttpClient client = new HttpClient()) {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bitlyToken);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, baseUrl);
                    request.Content = jsonContent;
                    var response = await client.SendAsync(request);
                    JObject rss = JObject.Parse(await response.Content.ReadAsStringAsync());                           
                    shortenedUrl = (string)rss["link"];
                }

                using (var db = new JackTheStudentContext()){
                var material = new ClassMaterial {
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == className).Select(c => c.Name).FirstOrDefault(),
                    Link = link,
                    ShortenedLink = shortenedUrl,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.classMaterialList.Add(material);
                db.ClassMaterial.Add(material);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {material.Id}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Material logged successfully");     
        return;
        }   
    }

    [Command("materials")]
    [Description(MaterialsDescriptions.MaterialLogsDescription)]
    public async Task MaterialLogs(CommandContext ctx, 
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve test for ALL classes.\n")] string className = ".")
    {       
        if (!JackTheStudent.Program.classList.Any(c => c.ShortName == className) && className != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } 

        var materials = JackTheStudent.Program.classMaterialList;
        string chosenClass = JackTheStudent.Program.classList
            .Where(c => c.ShortName == className)
            .Select(c => c.Name)
            .FirstOrDefault();
        string result = String.Empty;
        try{
            if (className == ".") {
                materials = materials.ToList();
                if (materials.Count == 0) {
                        await ctx.RespondAsync("There are no materials logged!");
                        return;
                } else {                  
                    foreach (ClassMaterial material in materials) {
                        result = $"{result} \nMaterial link for {material.Class} class - {material.ShortenedLink}. {(material.AdditionalInfo.Equals("") ? "" : $"Additional info: {material.AdditionalInfo}")}";
                    }
                }      
            } else {
                materials = materials
                .Where(m => 
                    m.Class == chosenClass)
                .ToList();                     
                if (materials.Count == 0) {
                    await ctx.RespondAsync($"There are no materials logged for {chosenClass} class!");
                    return;
                } else {
                    foreach (ClassMaterial material in materials) {
                        result = $"{result} \nMaterial link for {material.Class} class - {material.ShortenedLink}. {(material.AdditionalInfo.Equals("") ? "" : $"Additional info: {material.AdditionalInfo}")}";
                    }
                }                          
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":books:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Found materials:",
            Description = result,
            Color = new DiscordColor(0x6b6510) 
        };
        await ctx.RespondAsync("", embed: embed);    
    }
}
}