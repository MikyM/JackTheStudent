using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using JackTheStudent.CommandDescriptions;
using Serilog;
using DSharpPlus.Entities;

namespace JackTheStudent.Commands
{
public class ExamCommandsModule : Base​Command​Module
{
    
    [Command("exam")]
    [Description(ExamDescriptions.examLogDescription)]
    public async Task ExamLog(CommandContext ctx,
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string examDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string examTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "", 
        [Description ("\nTakes material links, multiple links must be wrapped with \"\".\n")] string materials = "")
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();

         if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !exam <class> <examDate> <examTime> Try again!");
            return;      
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (examDate == ""){
            await ctx.RespondAsync("There's date missing, fix it!");
            return;
        } else if (!DateTime.TryParse(examDate, out parsedEventDate)) {
            await ctx.RespondAsync("That's not a valid date you retard, learn to type!");
            return;
        } else if (examTime == ""){
            await ctx.RespondAsync("There's time missing, fix it!");
            return;
        } else if (!DateTime.TryParse(examTime, out parsedEventTime)) {
            await ctx.RespondAsync("That's not a valid time you retard, learn to type!");
            return;
        } else if(JackTheStudent.Program.examList
            .Any(e => 
                e.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && 
                e.Class == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                    .FirstOrDefault())) {
            await ctx.RespondAsync("Someone has already logged this exam.");
            return;
        } else if(JackTheStudent.Program.examList.Any(e => e.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay))) {
            await ctx.RespondAsync("There's an exam logged that takes place same time.");
            return;
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var exam = new Exam {
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.examList.Add(exam);
                db.Exam.Add(exam);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {exam.Id}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] New exam log, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Exam log failed");
                return;
            }
        await ctx.RespondAsync("Logged successfully");     
        return;
        }   
    }

    [Command("exams")]
    [Description(ExamDescriptions.examLogsDescription)]
    public async Task ExamLogs(CommandContext ctx, 
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve exam for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve ALL logged exams, \"planned\" retrieves only future events.\n")] string span = "planned")
    {   
        if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (span != "." && span != "planned") {
            await ctx.RespondAsync("Span only accepts . and planned values");
            return;
        }    

        var exams = JackTheStudent.Program.examList;
        string chosenClass = JackTheStudent.Program.classList
            .Where(c => c.ShortName == classType)
            .Select(c => c.Name)
            .FirstOrDefault();
        var result = String.Empty;
        try {
            if (classType == "." && span == "planned") {
                exams = exams
                    .Where(e => 
                        e.Date > DateTime.Now)
                    .ToList();
                if (exams.Count == 0) {
                        await ctx.RespondAsync("Wait what!? There are no exams planned, PAAAARTTTIEEEHH TIIIIIIIIMEEEEEEE!");
                        return;
                } else {
                    foreach (Exam exam in exams) {
                            result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(exam.Class)} exam will happen on {exam.Date.ToString().Trim()}.{(exam.AdditionalInfo.Equals("") ? "" : $"Additional info: {exam.AdditionalInfo}")}";
                    }
                }     
            } else if (classType == "." && span == ".") {
                exams = exams.ToList();
                if (exams.Count == 0) {
                        await ctx.RespondAsync("There are no exams logged!");
                        return;
                } else {
                    foreach (Exam exam in exams) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(exam.Class)} exam will happen / happened on {exam.Date.ToString().Trim()}.{(exam.AdditionalInfo.Equals("") ? "" : $"Additional info: {exam.AdditionalInfo}")}";
                    }
                }
            } else if (classType != "." && span == "planned") {
                exams = exams
                    .Where(e => 
                        e.Date > DateTime.Now && 
                        e.Class == chosenClass)
                    .ToList();                     
                if (exams.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClass} exams planned!");
                    return;
                } else {
                    foreach (Exam exam in exams) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(exam.Class)} exam will happen on {exam.Date.ToString().Trim()}.{(exam.AdditionalInfo.Equals("") ? "" : $"Additional info: {exam.AdditionalInfo}")}";
                    }
                }                                                                
            } else {
                exams = exams
                    .Where (e => 
                        e.Class == chosenClass)
                    .ToList();                  
                if (exams.Count == 0) {
                    await ctx.RespondAsync($"There are no logged exams for {exams.Select(e => e.Class).FirstOrDefault()} class!");
                    return;
                } else {
                    foreach (Exam exam in exams) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(exam.Class)} exam will happen / happened on {exam.Date.ToString().Trim()}.{(exam.AdditionalInfo.Equals("") ? "" : $"Additional info: {exam.AdditionalInfo}")}";
                    }
                }                       
            }
        } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Exam logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":heart_exclamation:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Found exams:",
            Description = result,
            Color = new DiscordColor(0xb01a38) 
        };
        await ctx.RespondAsync("", embed: embed);     
    }
}
}
