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
using System.Collections.Generic;
using JackTheStudent.Models;
namespace JackTheStudent
{
    class Program
    {
        /* Cancellation token*/
    private CancellationTokenSource _cts { get; set; }

    /*App config loading*/
    private IConfigurationRoot _config;

    /* These are the discord library's main classes */
    private DiscordClient _discord;
    private CommandsNextModule _commands;
    private InteractivityModule _interactivity;

    public static List<ClassTypes> classList = new List<ClassTypes>();
    public static List<string> groupList = new List<string>();

    /* Use the async main to create an instance of the class and await it*/
    static async Task Main(string[] args) => await new Program().InitBot(args);

    async Task InitBot(string[] args)
    {
        try {
            using (var db = new JackTheStudentContext()){
                classList = db.ClassTypes
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

        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy HH:mm";
        culture.DateTimeFormat.LongTimePattern = "";
        Thread.CurrentThread.CurrentCulture = culture;
        
        try {
            Console.WriteLine("[Jack] Welcome!");
            _cts = new CancellationTokenSource(); 
          // Load the config file
            Console.WriteLine("[Jack] Loading config file..");
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            // Create the DSharpPlus client
            Console.WriteLine("[Jack] Creating discord client..");
            _discord = new DiscordClient(new DiscordConfiguration {
                Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                TokenType = TokenType.Bot
            });

            // Create the interactivity module
            _interactivity = _discord.UseInteractivity(new InteractivityConfiguration() {
                PaginationBehaviour = TimeoutBehaviour.Delete, // What to do when a pagination request times out
                PaginationTimeout = TimeSpan.FromSeconds(30), // How long to wait before timing out
                Timeout = TimeSpan.FromSeconds(30) // Default time to wait for interactive commands like waiting for a message or a reaction
            });

            // Build dependancies and then create the commands module.
            var deps = BuildDeps();
            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefix = _config.GetValue<string>("discord:CommandPrefix"), // Load the command prefix(what comes before the command, eg "!" or "/") from our config file
                Dependencies = deps // Pass the dependancies
            });

            // Command loading
            Console.WriteLine("[Jack] Loading command modules..");

            var type = typeof(IModule); // Get the type of our interface
            var types = AppDomain.CurrentDomain.GetAssemblies() // Get the assemblies associated with our project
                .SelectMany(s => s.GetTypes()) // Get all the types
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface); // Filter to find any type that can be assigned to an IModule

            var typeList = types as Type[] ?? types.ToArray(); // Convert to an array
            foreach (var t in typeList)
                _commands.RegisterCommands(t); // Loop through the list and register each command module with CommandsNext

            Console.WriteLine($"[Jack] Loaded {typeList.Count()} modules.");

            RunAsync(args).Wait();
        }
        catch(Exception ex) {
            Console.Error.WriteLine("[Jack] " + ex.ToString());
        }
    }

    async Task RunAsync(string[] args)
    {
        // Connect to discord's service
        Console.WriteLine("[Jack] Connecting..");
        await _discord.ConnectAsync();
        Console.WriteLine("[Jack] Connected!");

        // Keep the bot running until the cancellation token requests we stop
        while (!_cts.IsCancellationRequested)
            await Task.Delay(TimeSpan.FromMinutes(1));
    }

    /* 
     DSharpPlus has dependancy injection for commands, this builds a list of dependancies. 
     We can then access these in our command modules.
    */
    private DependencyCollection BuildDeps()
    {
        using var deps = new DependencyCollectionBuilder();
            deps.AddInstance(_interactivity) // Add interactivity
                .AddInstance(_cts) // Add the cancellation token
                .AddInstance(_config) // Add our config
                .AddInstance(_discord); // Add the discord client

            return deps.Build();
        }
    }

    
}
