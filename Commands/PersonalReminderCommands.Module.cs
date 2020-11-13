using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;
using JackTheStudent;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace JackTheStudent.Commands
{
public class SemesterAdminCommandsModule : Base​Command​Module
{
    [Command("reminder")]
    [Description("Semester group class table rebuild")]
    public async Task ReminderLog(CommandContext ctx)
    {

    }


    [Command("reminders")]
    [Description("Semester group class table rebuild")]
    public async Task ReminderLogs(CommandContext ctx)
    {

    }

}
}