using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;

/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class StudentCommandsModule : IModule
{
    /* Commands in DSharpPlus.CommandsNext are identified by supplying a Command attribute to a method in any class you've loaded into it. */
    /* The description is just a string supplied when you use the help command included in CommandsNext. */
    [Command("log")]
    [Description("Command logging an event of specified type")]
    public async Task Log(CommandContext ctx, string eventType, string classType, DateTime eventDate)
    {
    // Null if the user didn't respond before the timeout
        if(eventType == "exam") {
            try {
                using (var db = new JackTheStudentContext()){
                var exam = new Exams { Class = classType,
                                      Date = eventDate,
                                      LogBy = ctx.Message.Author.Id.ToString()};
                db.Exams.Add(exam);
                db.SaveChanges();
                }
            } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
            await ctx.RespondAsync("Exam log failed");
            }
            
            await ctx.RespondAsync("Exam logged successfully");
            return;
        } else if (eventType == "homework") {
            await ctx.RespondAsync("homework");
        } else if (eventType == "lreport") {
            await ctx.RespondAsync("lreport");
        } else if (eventType == "qtest") {
            await ctx.RespondAsync("qtest");
        } else {
            await ctx.RespondAsync("Unknown log type");
        } 
        
    }

    [Command("logs")]
    [Description("Command retrieving all logged events of specified type")]
    public async Task Logs(CommandContext ctx, string type, string span = "all")
    {
        if(type == "exam") {
            if(span == "all") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var exams = db.Exams.ToList();
                        foreach (Exams exam in exams) {
                            await ctx.RespondAsync(exam.Class + " " + exam.Date);
                        }
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            return;
            } else if (span == "planned") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var exams = db.Exams
                        .Where(x => x.Date < DateTime.Now)
                        .ToList();
                        foreach (Exams exam in exams) {
                            await ctx.RespondAsync(exam.Class + " " + exam.Date);
                        }
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            return;
            }
            
        } else if (type == "homework") {
            await ctx.RespondAsync("homeworks");
        } else if (type == "lreport") {
            await ctx.RespondAsync("lreports");
        } else if (type == "qtest") {
            await ctx.RespondAsync("qtests");
        } else {
            await ctx.RespondAsync("Unknown log type");
        }  
    }

}
}