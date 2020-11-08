using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;

namespace JackTheStudent.Commands
{

public class ExamCommandsModule : IModule
{
    private List<string> classList = new List<string>() {"eng", "mat", "ele", "db", "prog"};
    private List<string> eventList = new List<string>() {"exam", "hwork", "lreport", "stest"};

    [Command("exam")]
    [Description("Command logging an event of specified type")]
    public async Task ExamLog(CommandContext ctx, string eventType, string classType, DateTime eventDate)
    { 
        try {
            using (var db = new JackTheStudentContext()){
                var exam = new Exam {  Class = classType,
                                        Date = eventDate,
                                        LogById = ctx.Message.Author.Id.ToString(),
                                        LogByUsername = ctx.Message.Author.Username};
                
            
            db.Exam.Add(exam);
            await db.SaveChangesAsync();
            }
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
            await ctx.RespondAsync("Exam log failed");
            return;
        }
            
        await ctx.RespondAsync("Exam logged successfully");
        return;
    }

    [Command("exams")]
    [Description("Command retrieving all logged events of specified type")]
    public async Task ExamLogs(CommandContext ctx, string eventType, string classType = ".", string span = "planned", string group = "")
    {
        if(classType == "." && span == ".") {
            try {
                using (var db = new JackTheStudentContext()){
                    var exams = db.Exam.ToList();
                        foreach (Exam exam in exams) {
                            await ctx.RespondAsync(exam.Class + " " + exam.Date);
                        }
                        
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            return;
            } else if (classType == "." && span == "planned") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var exams = db.Exam
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (exams.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There are no exams planned, PAAAARTTTIEEEHH TIIIIIIIIMEEEEEEE!");
                        } else {
                            foreach (Exam exam in exams) {
                                await ctx.RespondAsync(exam.Class + " " + exam.Date);
                            }
                        }
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            return;
            } else if (classType != "." && span == "planned") {

                if(classList.Contains(classType)) {
                    try {
                        using (var db = new JackTheStudentContext()){
                            var exams = db.Exam
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (exams.Count == 0) {
                                string response = "There are no " + classType + " exams planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Exam exam in exams) {
                                    await ctx.RespondAsync(exam.Class + " " + exam.Date);
                                }
                            }                           
                        }
                        } catch(Exception ex) {
                            Console.Error.WriteLine("[Jack] " + ex.ToString());
                            await ctx.RespondAsync("Show logs failed");
                            return;
                        }
                        return;
                } else {
                    await ctx.RespondAsync("There's no such class, you high bruh?");
                    return;
                }                    
            } else if (classType != "." && span == ".") {
                if(classList.Contains(classType)) {
                    try {
                        using (var db = new JackTheStudentContext()){
                            var exams = db.Exam
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (exams.Count == 0) {
                                string response = "There are no exams logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Exam exam in exams) {
                                    await ctx.RespondAsync(exam.Class + " " + exam.Date);
                                }
                            }                           
                        }
                        } catch(Exception ex) {
                            Console.Error.WriteLine("[Jack] " + ex.ToString());
                            await ctx.RespondAsync("Show logs failed");
                            return;
                        }
                        return;
                } else {
                    await ctx.RespondAsync("Ya know there's only either all possible events or the ones that didn't happen right? Get yo facts straight negro!");
                    return;
                }                   
            }  
        }

    }
}