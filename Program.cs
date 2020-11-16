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
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace JackTheStudent
{
    class Program
    {
    private static HttpClient httpClient = new HttpClient();
    private CancellationTokenSource _cts { get; set; }
    private IConfigurationRoot _config;
    private DiscordClient _discord;
    private CommandsNextExtension _commands;
    private InteractivityExtension _interactivity;
    private static Timer reminderTimer;
    private static Timer timeCheckTimer;
    public static List<PersonalReminder> reminderList = new List<PersonalReminder>();
    public static List<Class> classList = new List<Class>();
    public static List<Group> groupList = new List<Group>();
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
    public static List<ClassMaterial> classMaterialList = new List<ClassMaterial>();
    static async Task Main(string[] args) => await new Program().InitBot(args);

    async Task InitBot(string[] args)
    {
        try {
            using (var db = new JackTheStudentContext()){
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

            Console.WriteLine("[Jack] All lists have been successfully loaded from database!");

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

    private async Task OnClientReady(DiscordClient _discord, ReadyEventArgs e)
    {
        DiscordActivity status = new DiscordActivity("you fail exams :')", ActivityType.Watching);
        await _discord.UpdateStatusAsync(status);
        Console.WriteLine($"[Jack] Updated status to \"{status.Name}\"!");
        await StartReminders();
        Console.WriteLine("[Jack] Personal reminders are up!");
        await StartTimeCheck();  
        Console.WriteLine("[Jack] Database timezone has been updated!");
        Console.WriteLine("[Jack] I'm now fully functional!");
        //return Task.CompletedTask;
    }

    private async Task StartReminders()
    {   
        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromSeconds(5);
        reminderTimer = new Timer((e) => {
            Remind().ContinueWith(t => {Console.WriteLine(t.Exception);}, TaskContinuationOptions.OnlyOnFaulted);  
            AutoRemind().ContinueWith(t => {Console.WriteLine(t.Exception);}, TaskContinuationOptions.OnlyOnFaulted);  
        }, null, startTimeSpan, periodTimeSpan);
    }

    private async Task StartTimeCheck()
    {   
        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromHours(24);
        timeCheckTimer = new Timer((e) => {
            TimeCheck().ContinueWith(t => {Console.WriteLine(t.Exception);}, TaskContinuationOptions.OnlyOnFaulted); 
        }, null, startTimeSpan, periodTimeSpan);
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

    private async Task TimeCheck()
    {
        string baseUrl = "http://worldtimeapi.org/api/timezone/Europe/Warsaw";
        try {
            using (HttpClient client = new HttpClient()) {
                using (HttpResponseMessage res = await client.GetAsync(baseUrl)) {
                    using (HttpContent content = res.Content) {
                        var data = await content.ReadAsStringAsync();
                        string[] splitResponse = data.Split(new char[] {'"'});
                        string dateTime = splitResponse[11];
                        string[] splitDatetime = dateTime.Split(new char[] {'+'});
                        string timezone = splitDatetime[1];

                        using (var db = new JackTheStudentContext()){
                            await db.Database.ExecuteSqlRawAsync($"SET time_zone = '+{timezone}';");
                        }
                        Console.WriteLine($"[Jack] Database timezone has been set to +{timezone}!");
                    }
                }
            }
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }      
    }
    
}
}
