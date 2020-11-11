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
using JackTheStudent.Commands;
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
    public static List<Class> classList = new List<Class>();
    public static List<string> groupList = new List<string>();
    public static List<ClassType> classTypeList = new List<ClassType>();
    public static List<string> quotes = new List<string>();

    static async Task Main(string[] args) => await new Program().InitBot(args);

    async Task InitBot(string[] args)
    {
        try {
            using (var db = new JackTheStudentContext()){
                classList = db.Class
                    .ToList();
            }
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }

        try {
            using (var db = new JackTheStudentContext()){
                groupList = db.Group
                    .Select( x => x.GroupId) 
                    .ToList();
            }
        } catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }

        try {
            using (var db = new JackTheStudentContext()){
                classTypeList = db.ClassType
                    .ToList();
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
        DiscordActivity status = new DiscordActivity("... wish you knew, huh?");
        _discord.UpdateStatusAsync(status);
        Console.WriteLine($"[Jack] Updated status to {status.Name}!");

        return Task.CompletedTask;
    }
    
}
}
