using System;

namespace JackTheStudent.CommandDescriptions
{
    public class FunDescriptions
    {
    public const string rollDescription = "Command rolling a random number from range 1 to 100." + 
    "\n!roll to get your lucky number.";

    public const string chancesDescription = "Command calculating chances of passing anything you want to pass." + 
    "\n!chances and follow the chat to successfully calcuate your chances.";

    public const string inspireDescription = "Command inspiring you with a random quote." + 
    "\n!inspire to get a random quote.";

    public const string pollDescription = "Command making poll with specified topic and voting based on discord reactions." + 
    "\n!poll <Topic> <Options> <TimeSpan> <DiscordEmojis>\n" + 
    "\nExample:\n" +
    "\n!poll \"math exam\" \"today,tomorrow\" 00:02:30 :watermelon: :bread: - will create a poll with 2 minutes 30 seconds duration, topic: exam, options: today and tomorrow with watermelon and bread emotes.\n" +
    "\nMultiple words in topic must be wrapped with \"\".\n" +
    "\nOptions must be separated with \",\".\n";

    public const string weatherDescription = "Command showing weather for today." + 
    "\n!weather <city>\n" + 
    "\nExample:\n" +
    "\n!weather krynica-morska - will show today's weather in Krynica Morska.";


    }

}