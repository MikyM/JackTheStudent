using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;

/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class StudentCommandsModule : IModule
{
    private List<string> classList = new List<string>() {"eng", "mat", "ele", "db", "prog"};
    private List<string> eventList = new List<string>() {"exam", "hwork", "lreport", "stest"};

    [Command("log")]
    [Description("Command logging an event of specified type")]
    public async Task Log(CommandContext ctx, string eventType, string classType, DateTime eventDate, string groupId = "")
    {
    
        switch(eventType) {
            case "exam":
                try {
                using (var db = new JackTheStudentContext()){
                var exam = new Exam { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.Exam.Add(exam);
                await db.SaveChangesAsync();
                }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Exam log failed");
                }
            
                await ctx.RespondAsync("Exam logged successfully");
                break;
            case "hwork":
                if (groupId == "") {
                    await ctx.RespondAsync("How can I know which group you're in you dumbass. Try again!");
                    return;
                }
                try {
                using (var db = new JackTheStudentContext()){
                var homeWork = new Homework { Class = classType,
                                      Date = eventDate,
                                      GroupId = groupId,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.Homework.Add(homeWork);
                await db.SaveChangesAsync();
                }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Homework log failed");
                }
            
                await ctx.RespondAsync("Homework logged successfully");
                break;
            case "stest":
                try {
                using (var db = new JackTheStudentContext()){
                var shortTest = new ShortTest { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.ShortTest.Add(shortTest);
                await db.SaveChangesAsync();
                }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Short test log failed");
                }
            
                await ctx.RespondAsync("Short test logged successfully");
                break;
            case "lreport":
                try {
                using (var db = new JackTheStudentContext()){
                var labReport = new LabReport { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.LabReport.Add(labReport);
                await db.SaveChangesAsync();
                }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Lab report log failed");
                }
            
                await ctx.RespondAsync("Lab report logged successfully");
                break;
            default:
                await ctx.RespondAsync("Unknown log type");
                break;
        } 
        
    }

    [Command("logs")]
    [Description("Command retrieving all logged events of specified type")]
    public async Task Logs(CommandContext ctx, string eventType, string classType = ".", string span = "planned", string group = "")
    {
        switch(eventType) {
        case "exam":
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
            break;
        case "hwork":
            if (group == "") {
                await ctx.RespondAsync("How can I know which group you're in you dumbass. Try again!");
                return;
            }
            if(classType == "." && span == "." ) {
                try {
                    using (var db = new JackTheStudentContext()){
                    var homeworks = db.Homework
                        .Where( x => x.GroupId == group)
                        .ToList();
                        if (homeworks.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There is no " + classType + " homework for group " + group + "!");
                        } else {
                            foreach (Homework homework in homeworks) {
                                await ctx.RespondAsync(homework.Class + " " + homework.Date);
                            }
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
                    var homeworks = db.Homework
                        .Where(x => x.Date > DateTime.Now && x.GroupId == group)
                        .ToList();
                        if (homeworks.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There is no homework, hmm... league?");
                        } else {
                            foreach (Homework homework in homeworks) {
                                await ctx.RespondAsync(homework.Class + " " + homework.Date);
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
                            var homeworks = db.Homework
                                .Where(x => x.Date > DateTime.Now && x.Class == classType && x.GroupId == group)
                                .ToList();                     

                            if (homeworks.Count == 0) {
                                string response = "There is no " + classType + " homework planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Homework homework in homeworks) {
                                    await ctx.RespondAsync(homework.Class + " " + homework.Date);
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
                            var homeworks = db.Homework
                                .Where(x => x.Class == classType && x.GroupId == group)
                                .ToList();                     

                            if (homeworks.Count == 0) {
                                string response = "There is no homework logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Homework homework in homeworks) {
                                    await ctx.RespondAsync(homework.Class + " " + homework.Date);
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
            break;
        case "lreport":
            if(classType == "." && span == ".") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var labReports = db.LabReport.ToList();
                        foreach (LabReport labReport in labReports) {
                            await ctx.RespondAsync(labReport.Class + " " + labReport.Date);
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
                    var labReports = db.LabReport
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (labReports.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There are no lab reports planned, hmm... wanna have some quality time ( ͡° ͜ʖ ͡°)?");
                        } else {
                            foreach (LabReport labReport in labReports) {
                                await ctx.RespondAsync(labReport.Class + " " + labReport.Date);
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
                            var labReports = db.LabReport
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (labReports.Count == 0) {
                                string response = "There are no " + classType + " lab reports planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (LabReport labReport in labReports) {
                                    await ctx.RespondAsync(labReport.Class + " " + labReport.Date);
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
                            var labReports = db.LabReport
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (labReports.Count == 0) {
                                string response = "There are no lab reports logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (LabReport labReport in labReports) {
                                    await ctx.RespondAsync(labReport.Class + " " + labReport.Date);
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
            break;
        case "stest":
            if(classType == "." && span == ".") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var shortTests = db.ShortTest.ToList();
                        foreach (ShortTest shortTest in shortTests) {
                            await ctx.RespondAsync(shortTest.Class + " " + shortTest.Date);
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
                    var shortTests = db.ShortTest
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (shortTests.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There are no short tests planned, let's play some Sims! Hah, just joking, start that Fifa up fucker");
                        } else {
                            foreach (ShortTest shortTest in shortTests) {
                                await ctx.RespondAsync(shortTest.Class + " " + shortTest.Date);
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
                            var shortTests = db.ShortTest
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (shortTests.Count == 0) {
                                string response = "There are no " + classType + " short tests planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (ShortTest shortTest in shortTests) {
                                    await ctx.RespondAsync(shortTest.Class + " " + shortTest.Date);
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
                            var shortTests = db.ShortTest
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (shortTests.Count == 0) {
                                string response = "There are no short tests logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (ShortTest shortTest in shortTests) {
                                    await ctx.RespondAsync(shortTest.Class + " " + shortTest.Date);
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
            break;
        default:
            await ctx.RespondAsync("Unknown log type");
            break;
    }

}
}
}