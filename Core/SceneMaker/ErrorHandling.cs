using System;
using System.Timers;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using Discord;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Config;

namespace SocialLinker.Core.SceneMaker
{
    class ErrorHandling : InteractiveBase<SocketCommandContext>
    {
        // Create a global variable for the class that contains the desired duration for error messages.
        // In this case, 60000 milliseconds equates to 1 minute.
        public static double error_duration = 60000;

        public static async Task Char_Keyword_Not_Found(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("There doesn’t seem to be a character found before the last template keyword.");
            embed.AddField("Tips", "Make sure to type the character’s name and then a template keyword after it to specify their sprite set from that title.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Missing(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A quotation mark was found earlier than expected. There might be a sprite number missing.");
            embed.AddField("Tips", "When creating a scene, make sure to include a sprite number after the character keyword.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_And_Dialogue_Missing(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Template and character keywords seem to be there, but the sprite number and dialogue are missing.");
            embed.AddField("Tips", "Make sure to include a sprite number after the character keyword and check that the input dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Too_Many_Animation_Frames(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("There seems to be more digits than needed for specifying animation frames.");
            embed.AddField("Tips", "" +
                "Start with the base sprite number first, then connect an eye frame number to it with a hyphen. If the character sprite also has mouth frames, connect it after the eye frame number with a hyphen, too.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Non_Digit_In_Sprite_Number(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A non-digit was found when specifying the animation frames.");
            embed.AddField("Tips", "" +
                "Start with the base sprite number first, then connect an eye frame number to it with a hyphen. If the character sprite also has mouth frames, connect it after the eye frame number with a hyphen, too.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Before_Char_Keyword(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A sprite number was found, but there doesn’t seem to be a character keyword before it.");
            embed.AddField("Tips", "Make sure to specify the character you want to use first before including a sprite number.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frame_With_Blank_Sprite(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames can’t be used if the sprite number is 0.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Viewing_Sprite_Details_With_Blank_Sprite(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames can’t be viewed if the sprite number is 0.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frames_Without_Dialogue(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames were specified, but there doesn’t seem to be any dialogue after them.");
            embed.AddField("Tips", "After selecting a sprite’s animation frames, dialogue should always come next. Make sure all dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frame_With_Blank_Sprite_And_Without_Dialogue(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("" +
                ":one: Animation frames can’t be used if the sprite number is 0.\n" +
                ":two: Animation frames were specified, but there doesn’t seem to be any dialogue after them.");
            embed.AddField("Tips", "After selecting a sprite’s animation frames, dialogue should always come next. Make sure all dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Text_After_Sprite_Number_Not_Quoted(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incorrect Syntax",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("There’s text after the sprite number that isn’t placed within quotation marks.");
            embed.AddField("Tips", "Make sure all of the character’s dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Cutins_Not_Cross_Compatible(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Cut-ins aren’t cross-compatible with other templates.");
            embed.AddField("Tips", "Remove the template keyword at the start of the command, or change it to match the template the cut-in belongs to.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Set_Not_Found_Generic(SocketMessage message, string user_input)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Set Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"There doesn’t seem to be a sprite set with the keyword \"{user_input}\".");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly, or if using a custom sprite set, check your keywords in the **`{BotConfig.bot.cmdPrefix}settings`** menu by selecting [Scene Maker Settings] > [Custom Sprites].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Set_Not_Found_In_Template(SocketMessage message, string char_keyword, string template)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Set Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"There doesn’t seem to be a sprite set with the keyword \"{char_keyword}\" in {OfficialSetMethods.AcronymToFullTitle(template)}.");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly, or if using a custom sprite set, check your keywords in the **`{BotConfig.bot.cmdPrefix}settings`** menu by selecting [Scene Maker Settings] > [Custom Sprites].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Cutin_Not_Found(SocketMessage message, string user_input)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Cut-in Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"There doesn’t seem to be a cut-in sprite set with the keyword {user_input}.");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly, or if using a custom sprite set, check your keywords in the **`{BotConfig.bot.cmdPrefix}settings`** menu by selecting [Scene Maker Settings] > [Custom Sprites].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Not_Found(SocketMessage message, string character_name, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Sprite Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"That sprite number doesn’t seem to be in {character_name}'s sprite set from {game_version}.");
            embed.AddField("Tips", "" +
                $"Use **`{BotConfig.bot.cmdPrefix}maker {character_name}`** to view which character sprites are available.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Scene_Upload_Failed(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Image Upload Failed",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Something went wrong while trying to upload the image. Try again soon.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incompatible_Custom_Sprite_Set(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incompatible Sprite Set",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("This custom sprite set has not been made compatible with the chosen template.");
            embed.AddField("Tips", $"To set compatibility for other templates, visit the **`{BotConfig.bot.cmdPrefix}settings`** menu and choose [Scene Maker Settings] > [Custom Sprites] > [Manage Custom Sprite Sets].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incompatible_Template_Setting(SocketMessage message, string template_setting, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incompatible Template Setting",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"This custom sprite set has not been made compatible with the active **{template_setting}** setting for the {game_version} template.");
            embed.AddField("Tips", $"To set compatibility, visit the **`{BotConfig.bot.cmdPrefix}settings`** menu and choose [Scene Maker Settings] > [Custom Sprites] > [Manage Custom Sprite Sets].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Max_Custom_Sprite_Sets_Reached(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Maximum Custom Set Limit Reached",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Users can hold a maximum of 50 custom sprite sets they created at once.");
            embed.AddField("Tips", $"To delete sets and make room, visit the **`{BotConfig.bot.cmdPrefix}settings`** menu and choose [Scene Maker Settings] > [Custom Sprites] > [Manage Custom Sprite Sets].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Restricted_Channel_Access(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Restricted Channel Access",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("This custom sprite set has been marked as **Not Safe For Work**. Visit a NSFW channel to view and use its contents.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incompatible_File_Type(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Incompatible File Type",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("This file type isn't supported for uploads. Try using JPG, PNG, or GIF files instead.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Eye_Frame_Not_Found(SocketMessage message, MakerCommandData command_data, string character_name, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Eye Frame Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"That eye frame doesn’t seem to be part of {character_name}'s {command_data.Base_Sprite}{Number_Suffix(command_data.Base_Sprite)} {game_version} sprite.");
            embed.AddField("Tips", "" +
                $"Use **`{BotConfig.bot.cmdPrefix}maker {command_data.Character_Keyword} {command_data.Base_Sprite}`** to view which animation frames are available for the sprite.");

            // Send the message to the channel.
            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            // Create a timer that expires as a "time out" duration for the user.
            Timer error_timer = new Timer()
            {
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Mouth_Frame_Not_Found(SocketMessage message, MakerCommandData command_data, string character_name, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Mouth Frame Not Found",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            embed.WithDescription($"That mouth frame doesn’t seem to be part of {character_name}'s {command_data.Base_Sprite}{Number_Suffix(command_data.Base_Sprite)} {game_version} sprite.");
            embed.AddField("Tips", "" +
                $"Use **`{BotConfig.bot.cmdPrefix}maker {command_data.Character_Keyword} {command_data.Base_Sprite}`** to view which animation frames are available for the sprite.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        // Warnings
        public static async Task Unsupported_Character(SocketMessage message)
        {
            SocketTextChannel channel = (SocketTextChannel)message.Channel;
            await channel.SendMessageAsync(":warning: One or more of the characters entered is not supported by this template's font set and will not be rendered.");
        }

        public static async Task API_Timeout(SocketMessage message)
        {
            SocketTextChannel channel = (SocketTextChannel)message.Channel;
            await channel.SendMessageAsync(":warning: There was some trouble retrieving template data, so we'll use some default settings for now.");
        }

        // System messages
        public static async Task Sprite_Sheet_Called_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages do not have sprite sheets to display.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frame_Sheet_Called_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages do not have animation frames to display.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frames_Specified_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames cannot be specified for system messages.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incorrect_Sprite_Number_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages for this template can only be called with the sprite number `0`.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Missing_Dialogue_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Dialogue must be input for system messages.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_And_Dialogue_Missing_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages must have a sprite number and dialouge specified.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Template_Specified_First_On_System_Message(SocketMessage message)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = message.Author;
            SocketTextChannel channel = (SocketTextChannel)message.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Input Error",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Template keywords must only come after the `System` character keyword.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async void ErrorTimer_Elapsed(object sender, ElapsedEventArgs e, RestUserMessage error_message, UserInfoFields account)
        {
            // If the user has their auto-delete settings for error messages set to on, attempt deleting the message if it hasn't been deleted by the user yet.
            // If it has, catch the exception and return.
            if (account.Auto_Delete_Error_Messages == "On")
            {
                try
                {
                    // Delete the current message from the channel.
                    await error_message.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }

        // Misc.
        public static string Number_Suffix(int input_number)
        {
            char[] number_char_array = input_number.ToString().ToCharArray();

            string suffix = "";

            if (input_number >= 10 && input_number < 20)
            {
                suffix = "th";
            }
            else
            {
                switch (number_char_array[number_char_array.Length - 1])
                {
                    case '1':
                        suffix = "st";
                        break;

                    case '2':
                        suffix = "nd";
                        break;

                    case '3':
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }

            return suffix;
        }

        public static Color Get_Profile_Embed_Color(UserInfoFields account)
        {
            // Based on the account's settings, return a color to be used on embedded menu messages.
            switch (account.Profile_Theme)
            {
                case "P3":
                    return new Color(37, 149, 255);

                case "P4":
                    return new Color(255, 229, 49);

                case "P5":
                    return new Color(213, 27, 4);

                default:
                    return new Color(0, 0, 0);
            }
        }

        public static string Get_Profile_Help_Thumbnail(UserInfoFields account)
        {
            // Based on the account's settings, return a thumbnail to be used on embedded menu messages.
            switch (account.Profile_Theme)
            {
                case "P3":
                    return "https://i.imgur.com/CguM1ql.png";

                case "P4":
                    return "https://i.imgur.com/PW7VtuB.png";

                case "P5":
                    return "https://i.imgur.com/tubdL8K.png";

                default:
                    return "";
            }
        }
    }
}