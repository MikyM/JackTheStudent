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
    "\n!poll <TimeSpan> <Topic> <DiscordEmojis>\n" + 
    "\nExample:\n" +
    "\n!poll 00:02:30 \"exam today or tomorrow?\" :watermelon: :bread: :cheese: - will create a poll with 2 minutes 30 seconds duration about exam today or tomorrow with watermelon, bread and cheese emotes.\n" +
    "\nMultiple words in topic must be wrapped with \"\".\n";

    public const string weatherDescription = "Command showing weather for today." + 
    "\n!weather <city>\n" + 
    "\nExample:\n" +
    "\n!weather krynica-morska - will show today's weather in Krynica Morska.";


    }

}