using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;
using JackTheStudent;
using System.Globalization;

namespace JackTheStudent.Commands
{
public class FunCommandsModule : IModule
{


    [Command("roll")]
    [Description("Rolls a random number from 1 to 100 which's considered your lucky number.")]
    public async Task Roll(CommandContext ctx)
    {
        Random r = new Random();
        int rnd = r.Next(1,100);
        await ctx.RespondAsync("Your lucky number is: " + rnd);
    }


    [Command("chances")]
    [Description("A complex algorithm used to calculate your chances of passing an exam or any other thing you wish to pass.")]
    public async Task Chances(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
        await ctx.RespondAsync("On a scale from 1 to 10, how do you rate your skills?");

        var intr = ctx.Client.GetInteractivityModule();
        var response = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(15));

        if(response == null) {
            await ctx.RespondAsync("If you not gonna answer, dont bother me dude...");
            
        } else if (response.Message.Content == "1" || response.Message.Content == "2" || response.Message.Content == "3") {
            await ctx.RespondAsync("That's kinda bad... What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);

            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("Low skills and low luck, I'm not sure about this dude... That's the worst you could get.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Low skills + under average luck = some trouble.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Low skills and above average luck, but do you really want to trust on that?");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Low skills and high luck, let the gamble begin!");
            }
             
        } else if (response.Message.Content == "4" || response.Message.Content == "5" || response.Message.Content == "6") {
            await ctx.RespondAsync("Not bad. What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);
            
            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("Average skills and low luck, this will be tough.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Average skills and under average luck, won't be easy.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Average skills and above average luck, kinda boring.");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Average skills and high luck, has some potential to be good.");
            }
             
        } else if (response.Message.Content == "7" || response.Message.Content == "8" || response.Message.Content == "9") {
            await ctx.RespondAsync("Looks promising! What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);

            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("Above average skills and low luck means you need to be careful.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Above average skills and under average luck. Don't worry, just focus.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Above average skills and above average luck. You got this for sure.");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Above average skills and high luck. Nothing can go wrong.");
            }
            
        } else if (response.Message.Content == "10") {
            await ctx.RespondAsync("Wow, you really need this? What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);

            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("You know everything and have low luck. Kinda like an uphill bike climb.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("You know everything and have under average luck, I guess you still got it.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("You know everything and have above average luck. Just go and write it already!");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("You know everything and have super high luck. Daaaaamn. This really can't go wrong.");
            }    
        }    

        await ctx.RespondAsync("Despite my answer, I hope you pass it anyway. Good luck!");

        }

    
    [Command("inspire")]
    [Description("Throws a random inspirational quote straight at you.")]
    public async Task Inspire(CommandContext ctx)    
    {
        var random = new Random();
        List<string> quotes = new List<string>();

        quotes.Add("People often say that motivation doesn't last. Well, neither does bathing ﹘ that's why we recommend it daily. - Zig Ziglar");
        quotes.Add("Sales are contingent upon the attitude of the salesman ﹘ not the attitude of the prospect. - W. Clement Stone");
        quotes.Add("You can waste your lives drawing lines. Or you can live your life crossing them. - Shonda Rhimes");
        quotes.Add("Always do your best. What you plant now, you will harvest later. - Og Mandino");
        quotes.Add("Everyone lives by selling something. - Robert Louis Stevenson");
        quotes.Add("Develop success from failures. Discouragement and failure are two of the surest stepping stones to success. - Dale Carnegie");
        quotes.Add("I’d rather regret the things I’ve done than regret the things I haven’t done. - Lucille Ball");
        quotes.Add("Action is the foundational key to all success. - Pablo Picasso");
        quotes.Add("If you are not taking care of your customer, your competitor will. - Bob Hooey");
        quotes.Add("The golden rule for every businessman is this: Put yourself in your customer's place. - Orison Swett Marden");
        quotes.Add("Hire character. Train skill. - Peter Schutz");
        quotes.Add("The best leaders are those most interested in surrounding themselves with assistants and associates smarter than they are. They are frank in admitting this and are willing to pay for such talents. - Antos Parrish");
        quotes.Add("Beware of monotony; it’s the mother of all the deadly sins.” - Edith Wharton");
        quotes.Add("The secret of joy in work is contained in one word ﹘ excellence. To know how to do something well is to enjoy it. - Pearl Buck");
        quotes.Add("Nothing is really work unless you would rather be doing something else. - J.M. Barrie");
        quotes.Add("Without a customer, you don’t have a business ﹘ all you have is a hobby. - Don Peppers");
        quotes.Add("To be most effective in sales today, it's imperative to drop your 'sales' mentality and start working with your prospects as if they've already hired you. - Jill Konrath");
        quotes.Add("Formula for success: rise early, work hard, strike oil. - J. Paul Getty");
        quotes.Add("Pretend that every single person you meet has a sign around his or her neck that says, 'Make me feel important.' Not only will you succeed in sales, you will succeed in life. - Mary Kay Ash");
        quotes.Add("Don't let the fear of losing be greater than the excitement of winning. - Robert Kiyosaki");
        quotes.Add("The difference between a successful person and others is not a lack of strength, not a lack of knowledge, but rather a lack of will. - Vince Lombardi");
        quotes.Add("Without hustle, talent will only carry you so far. - Gary Vaynerchuk");
        quotes.Add("Working hard for something we don't care about is called stressed; working hard for something we love is called passion. - Simon Sinek");
        quotes.Add("Move out of your comfort zone. You can only grow if you are willing to feel awkward and uncomfortable when you try something new. - Brian Tracy");
        quotes.Add("Obstacles are those frightful things you see when you take your eyes off your goal. - Henry Ford");
        quotes.Add("It's not just about being better. It's about being different. You need to give people a reason to choose your business. - Tom Abbott");
        quotes.Add("How dare you settle for less when the world has made it so easy for you to be remarkable? - Seth Godin");
        quotes.Add("Someday is not a day of the week. - Denise Brennan-Nelson");
        quotes.Add("If you cannot do great things, do small things in a great way. - Napoleon Hill");
        quotes.Add("Your time is limited, so don't waste it living someone else's life. - Steve Jobs");
        quotes.Add("I didn’t get there by wishing for it or hoping for it, but by working for it. - Estée Lauder");
        quotes.Add("Being good in business is the most fascinating kind of art. Making money is art and working is art and good business is the best art. - Andy Warhol");
        quotes.Add("Challenges are what make life interesting and overcoming them is what makes life meaningful. - Joshua J. Marine");
        quotes.Add("Be patient with yourself. Self-growth is tender; it’s holy ground. There’s no greater investment. - Stephen Covey");
        quotes.Add("Done is better than perfect. - Sheryl Sandberg");

        quotes.Add("Don't worry about what anybody else is going to do. The best way to predict the future is to invent it. - Alan Kay");
        quotes.Add("Premature optimization is the root of all evil (or at least most of it) in programming. - Donald Knuth");
        quotes.Add("Lisp has jokingly been called 'the most intelligent way to misuse a computer'. I think that description is a great compliment because it transmits the full flavor of liberation: it has assisted a number of our most gifted fellow humans in thinking previously impossible thoughts. - Edsger Dijkstra, CACM, 15:10");
        quotes.Add("Keep away from people who try to belittle your ambitions. Small people always do that, but the really great make you feel that you, too, can become great. - Mark Twain");
        quotes.Add("What Paul does, and does very well, is to take ideas and concepts that are beautiful in the abstract, and brings them down to a real world level. That's a rare talent to find in writing these days. - Jeff 'hemos' Bates, Director, OSDN; Co-evolver, Slashdot");
        quotes.Add("No problem should ever have to be solved twice. - Eric S. Raymond, How to become a hacker");
        quotes.Add("Attitude is no substitute for competence. - Eric S. Raymond, How to become a hacker");
        quotes.Add("It is said that the real winner is the one who lives in today but able to see tomorrow. - Juan Meng, Reviewing 'The future of ideas' by Lawrence Lessig");
        quotes.Add("Fools ignore complexity. Pragmatists suffer it. Some can avoid it. Geniuses remove it. - Alan J. Perlis (Epigrams in programming)");
        quotes.Add("A year spent in artificial intelligence is enough to make one believe in God. - Alan J. Perlis (Epigrams in programming)");
        quotes.Add("Dealing with failure is easy: Work hard to improve. Success is also easy to handle: You've solved the wrong problem. Work hard to improve. - Alan J. Perlis (Epigrams in programming)");
        quotes.Add("Within a computer natural language is unnatural. - Alan J. Perlis (Epigrams in programming)");
        quotes.Add("You think you know when you learn, are more sure when you can write, even more when you can teach, but certain when you can program. - Alan J. Perlis (Epigrams in programming)");
        quotes.Add("Adapting old programs to fit new machines usually means adapting new machines to behave like old ones. - Alan J. Perlis (Epigrams in programming)");
        quotes.Add("A little learning is a dangerous thing. - Alexander Pope");
        quotes.Add("Computer science education cannot make anybody an expert programmer any more than studying brushes and pigment can make somebody an expert painter. - Eric Raymond");
        quotes.Add("Einstein argued that there must be simplified explanations of nature, because God is not capricious or arbitrary. - Frederick P. Brooks, No Sliver Bullet.");
        quotes.Add("Students should be evaluated on how well they can achieve the goals they strived to achieve within a realistic context. Students need to learn to do things, not know things. - Roger Schank, Engines for Education");
        quotes.Add("There really is no learning without doing. - Roger Schank, Engines for Education");
        quotes.Add("The only problems we can really solve in a satisfactory manner are those that finally admit a nicely factored solution. - E. W. Dijkstra, The humble programmer");
        quotes.Add("The best way to learn to live with our limitations is to know them. -E. W. Dijkstra, The humble programmer");
        quotes.Add("An expert is, according to my working definition 'someone who doesn't need to look up answers to easy questions'. - Eric Lippert.");
        quotes.Add("The programmer must seek both perfection of part and adequacy of collection. - Alan J. Perlis");
        quotes.Add("Thus, programs must be written for people to read, and only incidentally for machines to execute. - Alan J. Perlis");
        quotes.Add("Lisp programmers know the value of everything but the cost of nothing. - Alan J. Perlis");
        quotes.Add("An interpreter raises the machine to the level of the user program; a compiler lowers the user program to the level of the machine language. - SICP");
        quotes.Add("Everything should be made as simple as possible, but no simpler. - Albert Einstein");
        quotes.Add("The great dividing line between success and failure can be expressed in five words: "I did not have time." - WestHost weekly newsletter 14 Feb 2003");
        quotes.Add("When your enemy is making a very serious mistake, don't be impolite and disturb him. - Napoleon Bonaparte (allegedly)");
        quotes.Add("A charlatan makes obscure what is clear; a thinker makes clear what is obscure. - Hugh Kingsmill");
        quotes.Add("The three chief virtues of a programmer are: Laziness, Impatience and Hubris. - Larry Wall (Programming Perl)");
        quotes.Add("All non-trivial abstractions, to some degree, are leaky. - Joel Spolsky (The Law of Leaky Abstractions)");
        quotes.Add("XML wasn't designed to be edited by humans on a regular basis. - Guido van Rossum");
        quotes.Add("Premature abstraction is an equally grevious sin as premature optimization. - Keith Devens");
        quotes.Add("You can have premature generalization as well as premature optimization. - Bjarne Stroustrup");
        quotes.Add("He causes his sun to rise on the evil and the good, and sends rain on the righteous and the unrighteous. - Matthew 5:45");
        quotes.Add("A language that doesn't affect the way you think about programming, is not worth knowing. - Alan Perlis");
        quotes.Add("Men never do evil so completely and cheerfully as when they do it from religious conviction. - Blaise Pascal (attributed)");
        quotes.Add("Everybody makes their own fun. If you don't make it yourself, it ain't fun -- it's entertainment. - David Mamet (as relayed by Joss Whedon)");
        quotes.Add("If we wish to count lines of code, we should not regard them as *lines produced* but as *lines spent*. - Edsger Dijkstra");
        quotes.Add("I have never met a man so ignorant that I couldn't learn something from him. - Galileo Galilei");
        quotes.Add("Philosophy: the finding of bad reasons for what one believes by instinct. - Brave New World (paraphrased)");
        quotes.Add("Fools! Don't they know that tears are a woman's most effective weapon? - Catwoman (The Batman TV Series, episode 83)");
        quotes.Add("It's like a condom; I'd rather have it and not need it than need it and not have it. - some chick in Alien vs. Predator, when asked why she always carries a gun");
        quotes.Add("C++ is history repeated as tragedy. Java is history repeated as farce. - Scott McKay");
        quotes.Add("Simplicity takes effort-- genius, even. - Paul Graham");
        quotes.Add("Show, don't tell. - unknown");
        quotes.Add("In God I trust; I will not be afraid. What can mortal man do to me? - David (Psalm 56:4)")
        quotes.Add("Linux is only free if your time has no value. - Jamie Zawinski");
        quotes.Add("Code is poetry. - wordpress.org");
        quotes.Add("If you choose not to decide, you still have made a choice. - Rush (Freewill)");
        quotes.Add("Civilization advances by extending the number of important operations which we can perform without thinking about them. - Alfred North Whitehead (Introduction to Mathematics)	");	
        quotes.Add("The function of wisdom is to discriminate between good and evil. - Cicero");
        quotes.Add("The reason to do animation is caricature. Good caricature picks out the essense of the statement and removes everything else. It's not simply about reproducing reality; It's about bumping it up. - Brad Bird, writer and director, The Incredibles");
        quotes.Add("Mistakes were made. - Ronald Reagan");
        quotes.Add("I would rather be an optimist and be wrong than a pessimist who proves to be right. The former sometimes wins, but never the latter. - 'Hoots');
        quotes.Add("What is truth? - Pontius Pilate");
        quotes.Add("Life moves pretty fast. If you don't stop and look around once in a while, you could miss it. - Ferris Bueller");
        quotes.Add("The direct pursuit of happiness is a recipe for an unhappy life. - Donald Campbell"); 
        quotes.Add("All problems in computer science can be solved by another level of indirection. - Butler Lampson");
        quotes.Add("For the things we have to learn before we can do them, we learn by doing them. - Aristotle.");
        quotes.Add("There are many ways to avoid success in life, but the most sure-fire just might be procrastination. - Hara Estroff Marano.");
        quotes.Add("PI seconds is a nanocentury. - [fact]");
        quotes.Add("A non negative binary integer value x is a power of 2 iff (x & (x-1)) is 0 using 2's complement arithmetic. - [fact]");
        quotes.Add("Dont give users the opportunity to lock themselves. - unknown");
        quotes.Add("Any fool can make the simple complex, only a smart person can make the complex simple. - unknown");
        quotes.Add("Only bad designers blame their failings on the users. - unknown");
        quotes.Add("When all you have is a hammer, everything looks like a nail. - unknown");
        quotes.Add("If there is a will, there is a way. - unknown");
        quotes.Add("Having large case statements in an object-oriented language is a sure sign your design is flawed. - [Fixing architecture flaws in Rails' ORM]");
        quotes.Add("New eyes have X-ray vision. [someone that hasn't written it is more likely to spot the bug. 'someone' can be you after a break] - William S. Annis");
        quotes.Add("So - what are the most important problems in software engineering? I’d answer “dealing with complexity”. - Mark Chu-Carroll");
        quotes.Add("The choice of the university is mostly important for the piece of paper you get at the end. The education you get depends on you. - Andreas Zwinkau");
        quotes.Add("Remember that you are humans in the first place and only after that programmers. - Alexandru Vancea");
        quotes.Add("As builders and creators finding the perfect solution should not be our main goal. We should find the perfect problem. - Isaac (blog comment)");
        quotes.Add("Just like carpentry, measure twice cut once. - Super-sizing YouTube with Python (Mike Solomon, mike@youtube.com)");
        quotes.Add("The good thing about reinventing the wheel is that you get a round one. - Douglas Crockford (Author of JSON and JsLint)");
        quotes.Add("I feel it is everybodies obligation to reach for the best in themselves and use that for the interest of mankind. - Corneluis (comment on 'Are you going to change the world? (Really?)')");
        quotes.Add("Resume writing is just like dating, or applying for a bank loan, in that nobody wants you if you're desperate. - Steve Yegge.");
        quotes.Add("It(mastering)’s knowing what you are doing. - Joesgoals.com");
        quotes.Add("If I tell you I'm good, you would probably think I'm boasting. If I tell you I'm no good, you know I'm lying. - Bruce Lee");
        quotes.Add("If something isn’t working, you need to look back and figure out what got you excited in the first place. - David Gorman (ImThere.com)");
        quotes.Add("Seize any opportunity, or anything that looks like opportunity. They are rare, much rarer than you think... - Nassim Nicholas Taleb, 'The Black Swan'.");
        quotes.Add("We tend to seek easy, single-factor explanations of success. For most important things, though, success actually requires avoiding many separate causes of failure. - Jared Diamond");
        quotes.Add("Things which matter most must never be at the mercy of things which matter least. - Johann Wolfgang Von Goethe (1749-1832)");
        quotes.Add("I think the root of your mistake is saying that macros don't scale to larger groups. The real truth is that macros don't scale to stupider groups. - Paul Graham, on the Lightweight Languages mailing list.");
        quotes.Add("Argue with idiots, and you become an idiot. If you compete with slaves you become a slave. - Paul Graham and Norbert Weiner, respectively");
        quotes.Add("Don't have good ideas if you aren't willing to be responsible for them. - Alan Perlis");
        quotes.Add("It is impossible to sharpen a pencil with a blunt axe. It is equally vain to try to do it with ten blunt axes instead.  - Edsger Dijkstra");
        quotes.Add("If we wish to count lines of code, we should not regard them as lines produced but as lines spent. - Edsger Dijkstra");
        quotes.Add("The most damaging phrase in the language is, It's always been done that way. - Rear Admiral Grace Hopper");
        quotes.Add("The only thing a man should ever be 100% convinced of is his own ignorance. - DJ MacLean");
        quotes.Add("In theory, there’s no difference between theory and practice. But in practice, there is. - Albert Einstein");
        quotes.Add("Act from reason, and failure makes you rethink and study harder. Act from faith, and failure makes you blame someone and push harder. -  Erik Naggum");
        quotes.Add("Measure everything you can about the product, and you'll start seeing patterns. - Max Levchin, PayPal founder, Talk at StartupSchool2007");
        quotes.Add("Quality of the people is better than the quality of the business idea. Crappy people can screw up the best idea in the world. - Hadi Partovi & Ali Partovi (iLike.com), Talk at StartupSchool2007");
        quotes.Add("The only constant in the world of hi-tech is change. - Mark Ward");
        quotes.Add("Write it properly first. It's easier to make a correct program fast, than to make a fast program correct. - http://www.cpax.org.uk/prg/");
        quotes.Add("You can’t get to version 500 if you don’t start with a version 1. - BetterExplained.com");
        quotes.Add("The wonderful and frustrating thing about understanding yourself is that nobody can do it for you. - BetterExplained.com");
        quotes.Add("When you have eliminated the impossible, whatever remains, however improbable, must be the truth. - Sherlock Holmes");
        quotes.Add("In order to understand what another person is saying, you must assume that it is true and try to find out what it could be true of. - George Miller");
        quotes.Add("A journey of a thousand miles must begin with a single step. - Lao­Tzu");
        quotes.Add("C’s great for what it’s great for. - Ben Hoyts (micropledge)");
        quotes.Add("There is one meaning [for static in C]: a global variable that is invisible outside the current scope, be it a function or a file. - Paolo Bonzini");
        quotes.Add("Processors don't get better so that they can have more free time. Processors get better so _you_ can have more free time. - LeCamarade (freeshells.ch)");
        quotes.Add("Understanding why C++ is the way it is helps a programmer use it well. A deep understanding of a tool is essential for an expert craftsman. - Bjarne Stroustrap");
        quotes.Add("No art, however minor, demands less than total dedication if you want to excel in it. - Alberti");
        quotes.Add("The minute you put the blame on someone else you’ve switch things from being a problem you can control to a problem outside of your control. - engtech (internetducttape.com)");
        quotes.Add("State is the root of all evil. In particular functions with side effects should be avoided. - OO Sucks (bluetail.com)");
        quotes.Add("It is better to be quiet and thought a fool than to open your mouth and remove all doubt. - WikiHow");
        quotes.Add("Simplicity means the achievement of maximum effect with minimum means. - Dr. Koichi Kawana");
        quotes.Add("Normality is the route to nowhere. - Ridderstrale & Nordstorm, Funky Business");
        quotes.Add("The problem is that Microsoft just has no taste. And I don't mean that in a small way, I mean that in a big way. - Steve Jobs");
        quotes.Add("Do you want to sell sugared water all your life or do you want to change the world? - Steve Jobs, to John Sculley (former Pepsi executive)");
        quotes.Add("Good work is no done by ‘humble’ men. - H. Hardy, A mathematician's apology.");
        quotes.Add("Simplicity and pragmatism beat complexity and theory any day. - Dennis (blog comment)");
        quotes.Add("Remember, always be yourself ... unless you suck! - Joss Whedon");
        quotes.Add("All great things require great dedication. - Chuck Norris(?)");
        quotes.Add("I'm always happy to trade performance for readability as long as the former isn't already scarce. - Crayz (Commentor on blog.raganwald.com)");
        quotes.Add("The definition of insanity is doing the same thing over and over again and expecting different results. - Benjamin Franklin");
        quotes.Add("A no uttered from the deepest conviction is better than a yes merely uttered to please or what is worse, to avoid trouble. - Mahatma Gandhi");
        quotes.Add("The general principle for complexity design is this: Think locally, act locally. - Richard P. Gabriel & Ron Goldman, Mob Software: The Erotic Life of Code");
        quotes.Add("Programming is the art of figuring out what you want so precisely that even a machine can do it. - Some guy who isn't famous");
        quotes.Add("Making All Software Into Tools Reduces Risk. - smoothspan.com");
        quotes.Add("Some may say Ruby is a bad rip-off of Lisp or Smalltalk, and I admit that. But it is nicer to ordinary people. - Matz, LL2");
        quotes.Add("Two people should stay together if together they are better people than they would be individually. - ?");
        quotes.Add("Whatever is worth doing at all, is worth doing well. - Earl of Chesterfield");
        quotes.Add("More computing sins are committed in the name of efficiency (without necessarily achieving it) than for any other single reason - including blind stupidity. - W.A. Wulf");
        quotes.Add("We should forget about small efficiencies, say about 97% of the time: premature optimization is the root of all evil. - Donald Knuth");
        quotes.Add("The best is the enemy of the good. - Voltaire");
        quotes.Add("The job of a leader today is not to create followers. It’s to create more leaders. - Ralph Nader");
        quotes.Add("Only make new mistakes. - Phil Dourado");
        quotes.Add("You can recognize truth by its beauty and simplicity. When you get it right, it is obvious that it is right. - Richard Feynman");
        quotes.Add("Talkers are no good doers. - William Shakespeare, 'Henry VI'");
        quotes.Add("Photography is painting with light. - Eric Hamilton");
        quotes.Add("Good artists copy. Great artists steal. - Pablo Picasso");
        quotes.Add("The problem is that small examples fail to convince, and large examples are too big to follow. - Steve Yegge.");
        quotes.Add("We are the sum of our behaviours; excellence therefore is not an act but a habit. - Aristotle.");
        quotes.Add("The purpose of abstraction is not to be vague, but to create a new semantic level in which one can be absolutely precise. - Edsger Dijkstra");
        quotes.Add("Every man prefers belief to the exercise of judgment. - Seneca");
        quotes.Add("It’s hard to grasp abstractions if you don’t understand what they’re abstracting away from. - Nathan Weizenbaum");
        quotes.Add("I find that the harder I work, the more luck I seem to have. - Thomas Jefferson");
        quotes.Add("Don't stay in bed, unless you can make money in bed. - George Burns");
        quotes.Add("If everything seems under control, you're not going fast enough. - Mario Andretti");
        quotes.Add("Chance favors the prepared mind. - Louis Pasteur");
        quotes.Add("Controlling complexity is the essence of computer programming. - Brian Kernigan");
        quotes.Add("The function of good software is to make the complex appear to be simple. - Grady Booch");
        quotes.Add("Measuring programming progress by lines of code is like measuring aircraft building progress by weight. - Bill Gates");
        quotes.Add("First learn computer science and all the theory.  Next develop a programming style.  Then forget all that and just hack. - George Carrette");
        quotes.Add("To iterate is human, to recurse divine. - L. Peter Deutsch");
        quotes.Add("The best thing about a boolean is even if you are wrong, you are only off by a bit. - Anonymous");
        quotes.Add("Should array indices start at 0 or 1?  My compromise of 0.5 was rejected without, I thought, proper consideration. - Stan Kelly-Bootle");
        quotes.Add("The use of COBOL cripples the mind; its teaching should therefore be regarded as a criminal offense. - E.W. Dijkstra");
        quotes.Add("Saying that Java is nice because it works on all OSes is like saying that anal sex is nice because it works on all genders. - Alanna");
        quotes.Add("If Java had true garbage collection, most programs would delete themselves upon execution. - Robert Sewell");
        quotes.Add("Software is like sex: It’s better when it’s free. - Linus Torvalds");
        quotes.Add("Any code of your own that you haven’t looked at for six or more months might as well have been written by someone else. - Eagleson’s Law");
        quotes.Add("Good programmers use their brains, but good guidelines save us having to think out every case. - Francis Glassborow");
        quotes.Add("If debugging is the process of removing bugs, then programming must be the process of putting them in. - Edsger W. Dijkstra");
        quotes.Add("Always code as if the guy who ends up maintaining your code will be a violent psychopath who knows where you live. - Martin Golding");
        quotes.Add("Everything that can be invented has been invented. - Charles H. Duell, Commissioner, U.S. Office of Patents, 1899");
        quotes.Add("I think there’s a world market for about 5 computers. - Thomas J. Watson, Chairman of the Board, IBM, circa 1948");
        quotes.Add("But what is it good for? - Engineer at the Advanced Computing Systems Division of IBM, commenting on the microchip, 1968");
        quotes.Add("There is no reason for any individual to have a computer in his home. - Ken Olson, President, Digital Equipment Corporation, 1977");
        quotes.Add("640K ought to be enough for anybody. - Bill Gates, 1981");
        quotes.Add("Windows NT addresses 2 Gigabytes of RAM, which is more than any application will ever need. - Microsoft, on the development of Windows NT, 1992");
        quotes.Add("We will never become a truly paper-less society until the Palm Pilot folks come out with WipeMe 1.0. - Andy Pierson");
        quotes.Add("If it keeps up, man will atrophy all his limbs but the push-button finger. - Frank Lloyd Wright");
        quotes.Add("Lisp is a programmable programming language. - John Foderaro");
        quotes.Add("I guess, when you're drunk, every woman looks beautiful and every language looks (like) a Lisp :) - Lament, #scheme@freenode.net");
        quotes.Add("Many of life's failures are people who did not realize how close they were to success when they gave up. - Thomas Edison");
        quotes.Add("The only way of discovering the limits of the possible is to venture a little way past them into the impossible. - Arthur C. Clarke");
        quotes.Add("Any sufficiently advanced technology is undistinguishable from magic. - Arthur C. Clarke");
        quotes.Add("Good ideas are out there for anyone with the wit and the will to find them. - Malcolm Gladwell, Who says big ideas are rare?");
        quotes.Add("Beware of bugs in the above code; I have only proved it correct, not tried it. - Donald Knuth");
        quotes.Add("The human brain starts working the moment you are born and never stops until you stand up to speak in public. - Anonymous");
        quotes.Add("The trouble with the world is that the stupid are always cocksure and the intelligent are always filled with doubt. - Bertrand Russell");
        quotes.Add("C++ is like teenage sex: Everybody is talking about it all the time, only few are really doing it. - unknown");
        quotes.Add("Functional programming is to algorithms as the ubiquitous little black dress is to women's fashion. - Mark Tarver (of 'The bipolar Lisp programmer' fame)");
        quotes.Add("Java and C++ make you think that the new ideas are like the old ones. Java is the most distressing thing to hit computing since MS-DOS. - Alan Kay");
        quotes.Add("Simple things should be simple. Complex things should be possible. - Alan Kay");
        quotes.Add("I invented the term Object-Oriented, and I can tell you I did not have C++ in mind. - Alan Kay");
        quotes.Add("All creativity is an extended form of a joke. - Alan Kay");
        quotes.Add("If you don't fail at least 90 percent of the time, you're not aiming high enough. - Alan Kay");
        quotes.Add("Revolutions come from standing on the shoulders of giants and facing in a better direction. - Alan Kay");
        quotes.Add("If it looks like a duck, walks like a duck, and quacks like a duck, it's a duck. - Official definition of 'duck typing'");
        quotes.Add("The greatest challenge to any thinker is stating the problem in a way that will allow a solution. - Bertrand Russell");
        quotes.Add("The ability to simplify means to eliminate the unnecessary so that the necessary may speak. - Hans Hofmann");
        quotes.Add("However beautiful the strategy, you should occasionally look at the results. - Winston Churchill");
        quotes.Add("Genius is 1% inspiration and 99% perspiration. - Thomas Edison");
        quotes.Add("I’d rather write programs to write programs than write programs. - Richard Sites");
        quotes.Add("Heureux l'étudiant qui comme la Rivière peut suivre son cours sans quitter son lit... - Sebastien, sur commentcamarche.net");
        quotes.Add(":nunmap can also be used outside of a monastery. - Vim user manual");
        quotes.Add("I had to learn how to teach less, so that more could be learned. - Tim Gallwey, The inner game of work");
        quotes.Add("The Work Begins Anew, The Hope Rises Again, And The Dream Lives On. - Ted Kennedy");
        quotes.Add("The hardest part of design ... is keeping features out. - Donald Norman");
        quotes.Add("Before software can be reusable it first has to be usable. - Ralph Johnson");
        quotes.Add("Perpetual optimism is a force multiplier. - Colin Powell");
        quotes.Add("Be the change you want to see in the world. - Mahatma Gandhi");
        quotes.Add("The art of getting someone else to do something you want done because he wants to do it [Leadership]. - Dwight D. Enseinhover.");
        quotes.Add("No one is all evil. Everybody has a good side. If you keep waiting, it will comme up. - Randy Pausch");
        quotes.Add("Experience is what you get when you don't get what you want. - Cited by Randy Pausch");
        quotes.Add("Luck is where preparation meets opportunity. - Randy Pausch");
        quotes.Add("The greatest of all weaknesses is the fear of appearing weak. - J. B. Bossuet, Politics from Holy Writ, 1709");
        quotes.Add("It's easier to ask forgiveness than it is to get permission. - Rear Admiral Dr. Grace Hopper");
        quotes.Add("An investment in knowledge always pays the best interest. - Benjamin Franklin");
        quotes.Add("Natives who beat drums to drive off evil spirits are objects of scorn to smart Americans who blow horns to break up traffic jams. - Mary Ellen Kelly");
        quotes.Add("Never do the impossible. People will expect you to do it forever after. - pigsandfishes.com");
        quotes.Add("Give up control. You never really had it anyway. - How to fail: 25 secrets learned through failure");
        quotes.Add("Only two things are infinite, the universe and human stupidity. And I'm not so sure about the former. - Albert Einstein");
        quotes.Add("The important thing is not to stop questioning. - Albert Einstein");
        quotes.Add("Do not accept anything because it comes from the mouth of a respected person. - Buddha");
        quotes.Add("Work as intensely as you play and play as intensely as you work. - Eric S. Raymond, How To Be A Hacker");
        quotes.Add("A witty saying proves nothing - Voltaire");
        quotes.Add("Sound methodology can empower and liberate the creative mind; it cannot inflame or inspire the drudge. - Frederick P. Brooks, No Sliver Bullet.");
        quotes.Add("Do not spoil what you have by desiring what you have not; but remember that what you now have was once among the things only hoped for. - Greek philosopher Epicurus");
        quotes.Add("Nobody can make you feel inferior without your consent. - Eleanor Roosevelt");
        quotes.Add("If you tell the truth, you don't have to remember anything. - Mark Twain");
        quotes.Add("You know you're in love when you can't fall asleep because reality is finally better than your dreams. - Dr. Seuss");
        quotes.Add("The opposite of love is not hate, it's indifference. - Elie Wiesel");
        quotes.Add("Life is what happens to you while you're busy making other plans. - John Lennon");
        quotes.Add("Whenever you find yourself on the side of the majority, it is time to pause and reflect. - Mark Twain");
        quotes.Add("To be yourself in a world that is constantly trying to make you something else is the greatest accomplishment. - Ralph Waldo Emerson");
        quotes.Add("It is not a lack of love, but a lack of friendship that makes unhappy marriages. - Friedrich Nietzsche");
        quotes.Add("In terms of energy, it's better to make a wrong choice than none at all. - George Leonard, Mastery.");
        quotes.Add("Courage is grace under pressure. - Ernest Hemingway");
        quotes.Add("Before enlightenment, chop wood and carry water. After enlightenment, chop wood and carry water. - Ancient Eastern adage");
        quotes.Add("Acknowledging the negative doesn't mean sniveling [whining, complaining]; it means facing the truth and then moving on. - George Leonard, Mastery.");
        quotes.Add("Whatever you can do, or dream you can, begin it. Boldness has genius, power, and magic in it. - Goethe");
        quotes.Add("Are you willing to wear your white belt? - George Leonard, Mastery.");

        int index = random.Next(quotes.Count);
        await ctx.RespondAsync(quotes[index]);
    }
    }
}