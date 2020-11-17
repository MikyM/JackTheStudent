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


namespace JackTheStudent
{
    class Program
    {
    private CancellationTokenSource _cts { get; set; }
    private IConfigurationRoot _config;
    private DiscordClient _discord;   
    private CommandsNextExtension _commands;
    private InteractivityExtension _interactivity;
    public static List<PersonalReminder> reminderList = new List<PersonalReminder>();
    public static List<Class> classList = new List<Class>();
    public static List<string> groupList = new List<string>();
    public static List<ClassType> classTypeList = new List<ClassType>();
    public static List<string> quotes = new List<string>();
    public static List<string> weatherCities = new List<string>();
    public static List<Exam> examList = new List<Exam>();
    public static List<Test> testList = new List<Test>();
    public static List<Project> projectList = new List<Project>();
    public static List<LabReport> labReportList = new List<LabReport>();
    public static List<ShortTest> shortTestList = new List<ShortTest>();
    public static List<TeamsLink> teamsLinkList = new List<TeamsLink>();
    public static List<Homework> homeworkList = new List<Homework>();
    public static List<ClassMaterial> ClassMaterialList = new List<ClassMaterial>();
    static async Task Main(string[] args) => await new Program().InitBot(args);

    async Task InitBot(string[] args)
    {
        try {
            using (var db = new JackTheStudentContext()){
                classTypeList = db.ClassType.ToList();
                groupList = db.Group.Select(x => x.GroupId).ToList();
                classList = db.Class.ToList();
                reminderList = db.PersonalReminder.ToList();
                examList = db.Exam.ToList();
                shortTestList = db.ShortTest.ToList();
                ClassMaterialList = db.ClassMaterial.ToList();
                homeworkList = db.Homework.ToList();
                labReportList = db.LabReport.ToList();
                projectList = db.Project.ToList();
                testList = db.Test.ToList();
                teamsLinkList = db.TeamsLink.ToList();
            }
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }

        try {
            using (var fileStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", "quotes.txt")))
                using (var reader = new StreamReader(fileStream)) {
                    String quote;
                    while ((quote = reader.ReadLine()) != null) {
                        quotes.Add(quote);
                    }
                reader.Close();
                fileStream.Close();
            } 
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }

        try {
            using (var fileStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", "weatherCities.txt")))
                using (var reader = new StreamReader(fileStream)) {
                    String city;
                    while ((city = reader.ReadLine()) != null) {
                        weatherCities.Add(city);
                    }
                reader.Close();
                fileStream.Close();
            } 
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }

        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy HH:mm";
        culture.DateTimeFormat.LongTimePattern = "";
        Thread.CurrentThread.CurrentCulture = culture;
        
        try {
            Console.WriteLine("[Jack] Welcome!");
            _cts = new CancellationTokenSource(); 

            Console.WriteLine("[Jack] Loading config file..");
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            Console.WriteLine("[Jack] Creating discord client..");
            
            _discord = new DiscordClient(new DiscordConfiguration {
                Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                TokenType = TokenType.Bot,
                AutoReconnect = true
            });


            _discord.UseInteractivity(new InteractivityConfiguration() {
                PaginationBehaviour = PaginationBehaviour.WrapAround, 
                Timeout = TimeSpan.FromSeconds(30) 
            }); 

            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] {_config.GetValue<string>("discord:CommandPrefix")}, 
                EnableDms = true,
                EnableMentionPrefix = true,
                DmHelp = true
            });

            Console.WriteLine("[Jack] Loading command modules..");

            _commands.RegisterCommands(Assembly.GetExecutingAssembly());

            Console.WriteLine("[Jack] Command modules loaded.");

            _discord.Ready += OnClientReady;

            RunAsync(args).Wait();
        }
        catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }
    }

    async Task RunAsync(string[] args)
    {   
        Console.WriteLine("[Jack] Connecting..");
        await _discord.ConnectAsync();
        Console.WriteLine("[Jack] Connected!");     
        
        while (!_cts.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromMinutes(1));
    }

    private Task OnClientReady(DiscordClient _discord, ReadyEventArgs e)
    {
        DiscordActivity status = new DiscordActivity("you fail exams :')", ActivityType.Watching);
        _discord.UpdateStatusAsync(status);
        Console.WriteLine($"[Jack] Updated status to \"{status.Name}\"!");
        StartReminders();
        Console.WriteLine($"[Jack] Personal reminders are up!");
        return Task.CompletedTask;
    }

    private Task StartReminders()
    {   
        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromSeconds(1);
        var timer = new System.Threading.Timer((e) => {
            Remind().ContinueWith(t => {Console.WriteLine(t.Exception);}, TaskContinuationOptions.OnlyOnFaulted);  
            AutoRemind().ContinueWith(t => {Console.WriteLine(t.Exception);}, TaskContinuationOptions.OnlyOnFaulted);  
        }, null, startTimeSpan, periodTimeSpan);
        return Task.CompletedTask;
    }

    private async Task Remind()
    {   
        if (reminderList.Count == 0) {
            return;
        }
        for (int i = 1; i <= reminderList.Count(); i++) {
            if (DateTime.Now >= reminderList[i-1].SetForDate) {
                await reminderList[i-1].Ping(_discord);
                try {
                    using(var db = new JackTheStudentContext()) {
                        db.PersonalReminder.Remove(reminderList[i-1]);
                        await db.SaveChangesAsync();
                    }
                    reminderList.Remove(reminderList[i-1]);
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                }
            }
        }
    }

    private async Task AutoRemind()
    {   
        if (examList.Count == 0) {
            return;
        }
        TimeSpan interval = new TimeSpan(7, 00, 00, 00);
        TimeSpan timeLeft = new TimeSpan();
        TimeSpan checkTime = new TimeSpan(15, 20, 00);
        bool isTime = DateTime.Now.TimeOfDay >= checkTime;
        bool isLessThanAWeek =  false;

        if(!isTime) {
            return;
        }   

        for (int i = 1; i <= examList.Count(); i++) {
            timeLeft = examList[i-1].Date.Date - DateTime.Now.Date;
            isLessThanAWeek = timeLeft <= interval;
            if (isLessThanAWeek) {
                await examList[i-1].Ping(_discord);
                examList[i-1].wasReminded = true;
                using (var db = new JackTheStudentContext()) {
                    try {
                        var exam = db.Exam.Where(e => e.Id == examList[i-1].Id).FirstOrDefault();
                        exam.wasReminded = true;
                        await db.SaveChangesAsync();
                    } catch(Exception ex) {
                        Console.Error.WriteLine("[Jack] " + ex.ToString());
                    }
                }
            }
        }
    }   
    
}
}
