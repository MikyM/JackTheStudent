using System;

namespace JackTheStudent.CommandDescriptions
{
    public class TeamsDescriptions
    {
    public const string teamslinkLogDescription = "Command logging a teams link, last argument is optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!teamsLink <classShortName> <classTypeshortName> <dayOfTheWeek> <teamsLinkTime> <groupId> <link> <additionalInfo>\n" + 
        "\nExamples:\n" +
        "\n!teamsLink mat lec wednesday 13:30 3 teams.microsoft.com/l/meetup-join/19%3ameeting" + 
        "\n!teamsLink mat lec wednesday 13:30 3 teams.microsoft.com/l/meetup-join/19%3ameeting \"Calculator required\"" + 
        "\n!teamsLink mat lec wednesday 13:30 . teams.microsoft.com/l/meetup-join/19%3ameeting";

    public const string teamslinkLogsDescription = "Command retrieving logged teams links based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!teamsLinks <classShortName> <classTypeShortName> <group>\n" + 
        "\nType !classes to retrieve short names, !classtypes to retrieve types short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument.\n" +
        "\nExamples:\n" +
        "\n!teamsLinks - will retrieve all teams links for all the groups, all the classes and all the types" + 
        "\n!teamsLinks mat - will retrieve all teams links for Math classes" +
        "\n!teamsLinks mat lec - will retrieve all teams links for Math lectures" +
        "\n!teamsLinks mat exe 3 - will retrieve all teams links for group 3 for Maths exercises" +
        "\nso on, so forth.";
    }

}