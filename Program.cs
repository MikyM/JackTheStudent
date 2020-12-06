using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Collections.Generic;
using JackTheStudent.Models;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.CommandsNext.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Serilog;
using Newtonsoft.Json.Linq;

namespace JackTheStudent
{
    class Program
    {
    private static HttpClient httpClient = new HttpClient();
    private static Timer reminderTimer;
    private static Timer timeCheckTimer;
    
    public static ulong roleMessageId;
    public static ulong remindRoleId;
    public static List<string> quotes = new List<string>();
    public static List<PersonalReminder> reminderList = new List<PersonalReminder>();
    public static List<Class> classList = new List<Class>();
    public static List<Group> groupList = new List<Group>();
    public static List<ClassType> classTypeList = new List<ClassType>();
    public static List<Exam> examList = new List<Exam>();
    public static List<Test> testList = new List<Test>();
    public static List<Project> projectList = new List<Project>();
    public static List<GroupProjectMember> projectMembersList = new List<GroupProjectMember>();
    public static List<LabReport> labReportList = new List<LabReport>();
    public static List<ShortTest> shortTestList = new List<ShortTest>();
    public static List<TeamsLink> teamsLinkList = new List<TeamsLink>();
    public static List<Homework> homeworkList = new List<Homework>();
    public static List<ClassMaterial> classMaterialList = new List<ClassMaterial>();
    private CancellationTokenSource _cts { get; set; }
    private IConfigurationRoot _config;
    private DiscordClient _discord;
    private CommandsNextExtension _commands;
    private InteractivityExtension _interactivity;
    public readonly EventId BotEventId = new EventId(1, "[Jack]");
    public static Dictionary<string, int> respects = new Dictionary<string, int>();
    public static Dictionary<string, int> limiter = new Dictionary<string, int>();
    static async Task Main(string[] args) => await new Program().InitBotAsync(args);

    public async Task InitBotAsync(string[] args)
    {
        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy HH:mm";
        culture.DateTimeFormat.LongTimePattern = "";
        Thread.CurrentThread.CurrentCulture = culture;
        
        try {
            _cts = new CancellationTokenSource(); 

            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(_config)
               .Enrich.FromLogContext()
               .WriteTo.Console()
               //.WriteTo.Async(a => a.File("log-.txt", rollingInterval: RollingInterval.Day))
               .CreateLogger();

            Log.Logger.Information("[Jack] Starting up..");
            Log.Logger.Debug("[Jack] Loaded config!");
            Log.Logger.Debug("[Jack] Creating discord client..");
            
            _discord = new DiscordClient(new DiscordConfiguration {
                Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                TokenType = TokenType.Bot,
                AutoReconnect = true
            });


            _interactivity = _discord.UseInteractivity(new InteractivityConfiguration() {
                PaginationBehaviour = PaginationBehaviour.WrapAround, 
                Timeout = TimeSpan.FromSeconds(30) 
            }); 

            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] {_config.GetValue<string>("discord:CommandPrefix")}, 
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = false
            });

            Log.Logger.Debug("[Jack] Loading command modules..");

            _commands.RegisterCommands(Assembly.GetExecutingAssembly());

            Log.Logger.Debug("[Jack] Command modules loaded.");

            _discord.Ready += OnClientReady;
            _discord.ClientErrored += ClientError;
            _discord.MessageReactionAdded += OnMessageReaction;
            _discord.MessageReactionRemoved += OnMessageReactionRemoval;
            _discord.MessageCreated += OnMessageCreation;
            _commands.CommandErrored += CommandErrored;
            _commands.CommandExecuted += CommandExecuted;

            await LoadFromDb();
            Log.Logger.Debug("[Jack] All lists have been successfully loaded from database!");
            await LoadFromFiles();
            Log.Logger.Debug("[Jack] All files have been loaded!");

            RunAsync(args).Wait();
            await Task.Delay(-1);
        }
        catch(Exception ex) {
            Log.Logger.Error("[Jack] " + ex.ToString());
        }
    }

    async Task RunAsync(string[] args)
    {   
        Log.Logger.Debug("[Jack] Connecting..");
        await _discord.ConnectAsync();
        Log.Logger.Debug("[Jack] Connected!");     
        
        while (!_cts.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromMinutes(1));
    }



    #region  event handling

    private async Task OnClientReady(DiscordClient _discord, ReadyEventArgs e)
    {
        Log.Logger.Information(BotEventId.ToString() + " Client is ready to process events.");
        DiscordActivity status = new DiscordActivity("you fail exams :')", ActivityType.Watching);
        await _discord.UpdateStatusAsync(status);
        Log.Logger.Debug($"[Jack] Updated status to \"{status.Name}\"!");
        await StartReminders();
        Log.Logger.Debug("[Jack] Personal reminders are up!");
        await StartTimeCheck();  
        Log.Logger.Debug("[Jack] Database timezone has been updated!");
        Log.Logger.Information("[Jack] I'm now fully functional!");
    }
    private Task ClientError(DiscordClient sender, ClientErrorEventArgs e)
    {
        Log.Logger.Error(BotEventId.ToString() + "Exception occured: " + e.Exception);
        return Task.CompletedTask;
    }

    private Task CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
    {
        Log.Logger.Debug(BotEventId.ToString() + $" {e.Context.User.Username}, with ID: {e.Context.User.Id} successfully executed '{e.Command.QualifiedName}'");
        return Task.CompletedTask;
    }

    private async Task CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        Log.Logger.Error(BotEventId.ToString() + $" {e.Context.User.Username}, with ID: {e.Context.User.Id} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}");
        if (e.Exception is ChecksFailedException ex) {
            var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
            var embed = new DiscordEmbedBuilder {
                Title = "Access denied",
                Description = $"{emoji} You do not have the permissions required to execute this command.",
                Color = new DiscordColor(0xFF0000) 
            };
            await e.Context.RespondAsync("", embed: embed);
        } else if (e.Exception is CommandNotFoundException) {   
            await e.Context.RespondAsync("There's no such command, learn to type...");
        } else if (e.Exception is Invalid​Overload​Exception) {
            await e.Context.RespondAsync("... try again");
        }
    }


    private async Task OnMessageReaction(DiscordClient sender, MessageReactionAddEventArgs e) 
    {
        if (e.Message.Id == roleMessageId) {
            if (e.Emoji == DiscordEmoji.FromName(sender, ":books:")) {
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole remindRole = e.Guild.GetRole(remindRoleId);
                await member.GrantRoleAsync(remindRole);
            }
        }
    }

    private async Task OnMessageReactionRemoval(DiscordClient sender, MessageReactionRemoveEventArgs e) 
    {
        if (e.Message.Id == roleMessageId) {
            if (e.Emoji == DiscordEmoji.FromName(sender, ":books:")) {
                DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
                DiscordRole remindRole = e.Guild.GetRole(remindRoleId);
                await member.RevokeRoleAsync(remindRole);
            }
        }
    }

    private async Task OnMessageCreation(DiscordClient sender, MessageCreateEventArgs e) 
    {
        string message = e.Message.Content.ToLower();
        var emoji = DiscordEmoji.FromName(sender, ":man_facepalming:");
        if (message.Contains("link") && message.Contains(" do ") && message.Contains("dysk")) {       
            await sender.SendMessageAsync(e.Channel, $"{e.Author.Mention}... https://bit.ly/3m2Jjms here you go dumbass, could look for stuff on your own next time... <#766960454543081566>. {emoji}");
        } else if (message.Contains("gdzie znajde info") || message.Contains("gdzie znajdę info")) {
            await sender.SendMessageAsync(e.Channel, $"{e.Author.Mention} https://google.com {emoji}");
        }
    }

    #endregion
    private async Task StartReminders()
    {   
        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromSeconds(5);
        reminderTimer = new Timer((e) => {
            Remind().ContinueWith(t => {Log.Logger.Error(t.Exception.ToString());}, TaskContinuationOptions.OnlyOnFaulted);  
            ExamAutoRemind().ContinueWith(t => {Log.Logger.Error(t.Exception.ToString());}, TaskContinuationOptions.OnlyOnFaulted);  
            TestAutoRemind().ContinueWith(t => {Log.Logger.Error(t.Exception.ToString());}, TaskContinuationOptions.OnlyOnFaulted);  
        }, null, startTimeSpan, periodTimeSpan);
    }

    private async Task StartTimeCheck()
    {   
        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromHours(24);
        timeCheckTimer = new Timer((e) => {
            TimeCheck().ContinueWith(t => {Log.Logger.Error(t.Exception.ToString());}, TaskContinuationOptions.OnlyOnFaulted); 
            Limiter().ContinueWith(t => {Log.Logger.Error(t.Exception.ToString());}, TaskContinuationOptions.OnlyOnFaulted);
        }, null, startTimeSpan, periodTimeSpan);
    }
    private async Task Limiter(){
        if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 1 && DateTime.Now.Minute >= 0 && DateTime.Now.Minute < 1){
            limiter.Clear();
        }
    }

    private async Task Remind()
    {   
        if (reminderList.Count == 0) {
            return;
        }
        for (int i = 1; i <= reminderList.Count(); i++) {
            if (DateTime.Now >= reminderList[i-1].SetForDate && !reminderList[i-1].WasReminded) {           
                await reminderList[i-1].Ping(_discord, _config.GetValue<ulong>("discord:LogChannelId"));
                try {
                    using(var db = new JackTheStudentContext()) {
                        var reminder = db.PersonalReminder.SingleOrDefault(r => r.Id == reminderList[i-1].Id);
                        if (reminder != null) {
                            reminder.Reminded();
                        }
                        await db.SaveChangesAsync();
                    }
                } catch(Exception ex) {
                    Log.Logger.Error("[Jack] Personal reminder - " + ex.ToString());
                }
            }
        }
    }

    private async Task ExamAutoRemind()
    {   
        if (examList.Count == 0) {
            return;
        }
        TimeSpan interval = new TimeSpan(7, 00, 00, 00);
        TimeSpan timeLeft = new TimeSpan();
        TimeSpan checkTimeStart = _config.GetValue<TimeSpan>("discord:AutoRemind:Time");
        TimeSpan checkTimeEnd = checkTimeStart + new TimeSpan(00, 01, 00);
        bool isTime = DateTime.Now.TimeOfDay >= checkTimeStart && DateTime.Now.TimeOfDay <= checkTimeEnd;
        bool isLessThanAWeek = false;

        if(!isTime) {
            return;
        }   
        for (int i = 1; i <= examList.Count(); i++) {
            timeLeft = examList[i-1].Date.Date - DateTime.Now.Date;
            isLessThanAWeek = timeLeft <= interval;
            if (isLessThanAWeek && !examList[i-1].WasReminded) {
                await examList[i-1].Ping(_discord, _config.GetValue<ulong>("discord:LogChannelId"), remindRoleId);
                using (var db = new JackTheStudentContext()) {
                    try {
                        var exam = db.Exam.Where(e => e.Id == examList[i-1].Id).FirstOrDefault();
                        exam.Reminded();
                        await db.SaveChangesAsync();
                        Log.Logger.Information($"Automatically reminded about {exam.Class} exam that happens on {exam.Date}");
                    } catch(Exception ex) {
                        Log.Logger.Error("[Jack] Auto reminder - " + ex.ToString());
                    }
                }
            }
        }
    }   

    private async Task TestAutoRemind()
    {   
        if (testList.Count == 0) {
            return;
        }
        TimeSpan interval = new TimeSpan(7, 00, 00, 00);
        TimeSpan timeLeft = new TimeSpan();
        TimeSpan checkTimeStart = _config.GetValue<TimeSpan>("discord:AutoRemind:Time");
        TimeSpan checkTimeEnd = checkTimeStart + new TimeSpan(00, 01, 00);
        bool isTime = DateTime.Now.TimeOfDay >= checkTimeStart && DateTime.Now.TimeOfDay <= checkTimeEnd;
        bool isLessThanAWeek = false;
        DateTime dayOfRemind = new DateTime().Date;
        List<string> remindedClassList = new List<string>();

        if(!isTime) {
            return;
        }   
        for (int i = 1; i <= testList.Count(); i++) {
            timeLeft = testList[i-1].Date.Date - DateTime.Now.Date;
            isLessThanAWeek = timeLeft <= interval;
            if (isLessThanAWeek && !testList[i-1].WasReminded) {
                if(testList[i-1].Date.Date == dayOfRemind.Date && remindedClassList.Contains(testList[i-1].Class)) {
                    testList[i-1].Reminded();
                    using (var db = new JackTheStudentContext()) {
                        try {
                            var test = db.Test.Where(e => e.Id == testList[i-1].Id).FirstOrDefault();
                            test.Reminded();
                            await db.SaveChangesAsync();
                            Log.Logger.Information($"Auto remind has been already triggered for {test.Class} test that happens on {test.Date}");
                        } catch(Exception ex) {
                            Log.Logger.Error("[Jack] Auto reminder - " + ex.ToString());
                        }
                    }
                    continue;
                }
                await testList[i-1].Ping(_discord, _config.GetValue<ulong>("discord:LogChannelId"), remindRoleId);

                dayOfRemind = testList[i-1].Date.Date;
                remindedClassList.Add(testList[i-1].Class);

                Log.Logger.Information($"Automatically reminded about {testList[i-1].Class} test that happens on {testList[i-1].Date}");
                using (var db = new JackTheStudentContext()) {
                    try {
                        var test = db.Test.Where(e => e.Id == testList[i-1].Id).FirstOrDefault();
                        test.WasReminded = true;
                        await db.SaveChangesAsync();
                        Log.Logger.Information($"Automatically reminded about {test.Class} test that happens on {test.Date}");
                    } catch(Exception ex) {
                        Log.Logger.Error("[Jack] Auto reminder - " + ex.ToString());
                    }
                }
            }
        }
    }

    private async Task TimeCheck()
    {
        string baseUrl = "http://worldclockapi.com/api/json/cet/now";
        string offset = String.Empty;
        JObject response;
        bool isDayLightSavingsTime;

        try {
            using (HttpClient client = new HttpClient()) {
                using (HttpResponseMessage res = await client.GetAsync(baseUrl)) {
                    using (HttpContent content = res.Content) {
                        var data = await content.ReadAsStringAsync();

                        response = JObject.Parse(data);                           
                        isDayLightSavingsTime = (bool)response["isDayLightSavingsTime"];

                        if(isDayLightSavingsTime) {
                            offset = "+02:00";
                        } else {
                            offset = "+01:00";
                        }               

                        using (var db = new JackTheStudentContext()){
                            await db.Database.ExecuteSqlRawAsync($"SET time_zone = '{offset}';");
                        }
                        Log.Logger.Information($"[Jack] Database time offset has been set to {offset}!");
                    }
                }
            }
        } catch(Exception ex) {
            Log.Logger.Error("[Jack] Time check - " + ex.ToString());
        }      
    }

    private async Task LoadFromDb()
    {
        try {
            await using (var db = new JackTheStudentContext()){
                classTypeList = db.ClassType.ToList();
                groupList = db.Group.ToList();
                classList = db.Class.ToList();
                reminderList = db.PersonalReminder.ToList();
                examList = db.Exam.ToList();
                shortTestList = db.ShortTest.ToList();
                classMaterialList = db.ClassMaterial.ToList();
                homeworkList = db.Homework.ToList();
                labReportList = db.LabReport.ToList();
                projectList = db.Project.ToList();
                testList = db.Test.ToList();
                projectMembersList = db.GroupProjectMember.ToList();
                teamsLinkList = db.TeamsLink.ToList();
                remindRoleId = db.RoleAssignment.Select(c => c.RoleId).FirstOrDefault();
                roleMessageId = db.RoleAssignment.Select(c => c.MessageId).FirstOrDefault();

                foreach (Project project in projectList) {
                    if (project.IsGroup) {
                        project.SetParticipants(projectMembersList.Where(m => m.ProjectId == project.Id).ToList());
                    }
                }              
            }
        } catch(Exception ex) {
            Log.Logger.Error("[Jack] Load from db - " + ex.ToString());
        }
    }
    private async Task LoadFromFiles()
    {
        try {
            await using (var fileStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", "quotes.txt")))
                using (var reader = new StreamReader(fileStream)) {
                    String quote;
                    while ((quote = reader.ReadLine()) != null) {
                        quotes.Add(quote);
                    }
                reader.Close();
                fileStream.Close();
            } 
        } catch(Exception ex) {
            Log.Logger.Error("[Jack] Load quotes - " + ex.ToString());
        }
    } 
    }
}
