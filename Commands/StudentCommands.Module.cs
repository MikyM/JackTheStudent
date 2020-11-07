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
    public async Task Log(CommandContext ctx, string eventType, string classType, DateTime eventDate)
    {
    
        switch(eventType) {
            case "exam":
                try {
                using (var db = new JackTheStudentContext()){
                var exam = new Exams { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.Exams.Add(exam);
                db.SaveChanges();
                }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Exam log failed");
                }
            
                await ctx.RespondAsync("Exam logged successfully");
                break;
            case "hwork":
                try {
                using (var db = new JackTheStudentContext()){
                var homeWork = new Homeworks { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.Homeworks.Add(homeWork);
                db.SaveChanges();
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
                var shortTest = new ShortTests { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.ShortTests.Add(shortTest);
                db.SaveChanges();
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
                var labReport = new LabReports { Class = classType,
                                      Date = eventDate,
                                      LogById = ctx.Message.Author.Id.ToString(),
                                      LogByUsername = ctx.Message.Author.Username};
                db.LabReports.Add(labReport);
                db.SaveChanges();
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
    public async Task Logs(CommandContext ctx, string eventType, string classType = ".", string span = "planned")
    {
        switch(eventType) {
        case "exam":
            if(classType == "." && span == ".") {
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
            } else if (classType == "." && span == "planned") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var exams = db.Exams
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (exams.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There are no exams planned, PAAAARTTTIEEEHH TIIIIIIIIMEEEEEEE!");
                        } else {
                            foreach (Exams exam in exams) {
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
                            var exams = db.Exams
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (exams.Count == 0) {
                                string response = "There are no " + classType + " exams planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Exams exam in exams) {
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
                            var exams = db.Exams
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (exams.Count == 0) {
                                string response = "There are no exams logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Exams exam in exams) {
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
            if(classType == "." && span == ".") {
                try {
                    using (var db = new JackTheStudentContext()){
                    var homeworks = db.Homeworks.ToList();
                        foreach (Homeworks homework in homeworks) {
                            await ctx.RespondAsync(homework.Class + " " + homework.Date);
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
                    var homeworks = db.Homeworks
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (homeworks.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There is no homework, hmm... league?");
                        } else {
                            foreach (Homeworks homework in homeworks) {
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
                            var homeworks = db.Homeworks
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (homeworks.Count == 0) {
                                string response = "There is no " + classType + " homework planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Homeworks homework in homeworks) {
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
                            var homeworks = db.Homeworks
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (homeworks.Count == 0) {
                                string response = "There is no homework logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Homeworks homework in homeworks) {
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
                    var labReports = db.LabReports.ToList();
                        foreach (LabReports labReport in labReports) {
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
                    var labReports = db.LabReports
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (labReports.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There are no lab reports planned, hmm... wanna have some quality time ( ͡° ͜ʖ ͡°)?");
                        } else {
                            foreach (LabReports labReport in labReports) {
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
                            var labReports = db.LabReports
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (labReports.Count == 0) {
                                string response = "There are no " + classType + " lab reports planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (LabReports labReport in labReports) {
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
                            var labReports = db.LabReports
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (labReports.Count == 0) {
                                string response = "There are no lab reports logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (LabReports labReport in labReports) {
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
                    var shortTests = db.ShortTests.ToList();
                        foreach (ShortTests shortTest in shortTests) {
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
                    var shortTests = db.ShortTests
                        .Where(x => x.Date > DateTime.Now)
                        .ToList();
                        if (shortTests.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There are no short tests planned, let's play some Sims! Hah, just joking, start that Fifa up fucker");
                        } else {
                            foreach (ShortTests shortTest in shortTests) {
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
                            var shortTests = db.ShortTests
                                .Where(x => x.Date > DateTime.Now && x.Class == classType)
                                .ToList();                     

                            if (shortTests.Count == 0) {
                                string response = "There are no " + classType + " short tests planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (ShortTests shortTest in shortTests) {
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
                            var shortTests = db.ShortTests
                                .Where(x => x.Class == classType)
                                .ToList();                     

                            if (shortTests.Count == 0) {
                                string response = "There are no short tests logged for " + classType + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (ShortTests shortTest in shortTests) {
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