using System;
using System.Timers;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using Discord.Rest;
using SocialLinker.Core.CloudStorageTables;
using Discord;
using SocialLinker.Core.LocalStorageTables;
using SocialLinker.Config;
using SocialLinker.Core.Menus;

namespace SocialLinker.Core.SceneMaker
{
    class ErrorHandling : ModuleBase<SocketCommandContext>
    {
        public static async Task Char_Keyword_Not_Found(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("There doesn’t seem to be a character found before the last template keyword.");
            embed.AddField("Tips", "Make sure to type the character’s name and then a template keyword after it to specify their sprite set from that title.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Missing(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A quotation mark was found earlier than expected. There might be a sprite number missing.");
            embed.AddField("Tips", "When creating a scene, make sure to include a sprite number after the character keyword.");

            embed.WithImageUrl("https://i.imgur.com/qGWGAAG.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Missing_With_Game_Keyword(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A quotation mark was found earlier than expected. There might be a sprite number missing or misplaced.");
            embed.AddField("Tips", "When creating a scene, make sure to include a sprite number after the game keyword.");

            embed.WithImageUrl("https://i.imgur.com/Yu6fAAu.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_And_Dialogue_Missing(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Cross-compatibility and character keywords seem to be there, but the sprite number and dialogue are missing.");
            embed.AddField("Tips", "Make sure to include a sprite number after the character keyword and check that the input dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Pre_Cross_Sprite_Sheet_Syntax(SocialLinkerCommand command) // Temp error before cross-compatibility
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A character keyword was found after a game keyword."); // A character keyword was found after a game keyword with no sprite number or dialogue afterwards.
            embed.AddField("Tips", "When creating a game-specific sprite sheet, make sure the game keyword comes after the character keyword.");

            embed.WithImageUrl("https://i.imgur.com/n6U4wUg.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Pre_Cross_Anime_Frames_Syntax(SocialLinkerCommand command) // Temp error before cross-compatibility
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A character keyword was found after a game keyword."); // A character keyword was found after a game keyword with no sprite number or dialogue afterwards.
            embed.AddField("Tips", "When viewing a character's animation frames, make sure the game keyword comes after the character keyword.");

            embed.WithImageUrl("https://i.imgur.com/n6U4wUg.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Pre_Cross_Full_Scene_Syntax(SocialLinkerCommand command) // Temp error before cross-compatibility
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A character keyword was found after a game keyword.");
            embed.AddField("Tips", "When creating a scene, make sure the game keyword only comes after the character keyword.");

            embed.WithImageUrl("https://i.imgur.com/Yu6fAAu.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Too_Many_Animation_Frames(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("There seems to be more digits than needed for specifying animation frames.");
            embed.AddField("Tips", "" +
                "Start with the base sprite number first, then connect an eye frame number to it with a hyphen. If the character sprite also has mouth frames, connect it after the eye frame number with a hyphen, too.");

            embed.WithImageUrl("https://i.imgur.com/wQ72B6I.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Non_Digit_In_Sprite_Number(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A non-digit was found when specifying the animation frames.");
            embed.AddField("Tips", "" +
                "Start with the base sprite number first, then connect an eye frame number to it with a hyphen. If the character sprite also has mouth frames, connect it after the eye frame number with a hyphen, too.");

            embed.WithImageUrl("https://i.imgur.com/wQ72B6I.png");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Before_Char_Keyword(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A sprite number was found, but there doesn’t seem to be a character keyword before it.");
            embed.AddField("Tips", "Make sure to specify the character you want to use first before including a sprite number.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frame_With_Blank_Sprite(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames can’t be used if the sprite number is 0.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Viewing_Sprite_Details_With_Blank_Sprite(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames can’t be viewed if the sprite number is 0.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frames_Without_Dialogue(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames were specified, but there doesn’t seem to be any dialogue after them.");
            embed.AddField("Tips", "After selecting a sprite’s animation frames, dialogue should always come next. Make sure all dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frame_With_Blank_Sprite_And_Without_Dialogue(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
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
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Text_After_Sprite_Number_Not_Quoted(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("There’s text after the sprite number that isn’t placed within quotation marks.");
            embed.AddField("Tips", "Make sure all of the character’s dialogue is placed within quotation marks.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Cutins_Not_Cross_Compatible(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Cut-ins aren’t cross-compatible with other templates.");
            embed.AddField("Tips", "Remove the template keyword at the start of the command, or change it to match the template the cut-in belongs to.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Set_Not_Found_Generic(SocialLinkerCommand command, string user_input)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"There doesn’t seem to be a sprite set with the keyword \"{user_input}\".");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly and try again.");

            /*embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly, or if using a custom sprite set, check your keywords in the **`{BotConfig.bot.cmdPrefix}settings`** menu by selecting [Scene Maker Settings] > [Custom Sprites].");*/

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Set_Not_Found_In_Template(SocialLinkerCommand command, string char_keyword, string template)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"There doesn’t seem to be a sprite set with the keyword \"{char_keyword}\" in {OfficialSetMethods.AcronymToFullTitle(template)}.");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly and try again.");

            /*embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly, or if using a custom sprite set, check your keywords in the **`{BotConfig.bot.cmdPrefix}settings`** menu by selecting [Scene Maker Settings] > [Custom Sprites].");*/

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Cutin_Not_Found(SocialLinkerCommand command, string user_input)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"There doesn’t seem to be a cut-in sprite set with the keyword {user_input}.");
            embed.AddField("Tips", "" +
                $"Make sure the character’s keyword is typed correctly, or if using a custom sprite set, check your keywords in the **`{BotConfig.bot.cmdPrefix}settings`** menu by selecting [Scene Maker Settings] > [Custom Sprites].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Not_Found(SocialLinkerCommand command, string character_name, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"That sprite number doesn’t seem to be in {character_name}'s sprite set from {game_version}.");
            embed.AddField("Tips", "" +
                $"Use **`{BotConfig.bot.cmdPrefix}maker {character_name}`** to view which character sprites are available.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Image_Upload_Failed(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Something went wrong while trying to upload the image. Try again soon.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incompatible_Custom_Sprite_Set(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("This custom sprite set has not been made compatible with the chosen template.");
            embed.AddField("Tips", $"To set compatibility for other templates, visit the **`{BotConfig.bot.cmdPrefix}settings`** menu and choose [Scene Maker Settings] > [Custom Sprites] > [Manage Custom Sprite Sets].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incompatible_Template_Setting(SocialLinkerCommand command, string template_setting, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription($"This custom sprite set has not been made compatible with the active **{template_setting}** setting for the {game_version} template.");
            embed.AddField("Tips", $"To set compatibility, visit the **`{BotConfig.bot.cmdPrefix}settings`** menu and choose [Scene Maker Settings] > [Custom Sprites] > [Manage Custom Sprite Sets].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Max_Custom_Sprite_Sets_Reached(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Users can hold a maximum of 50 custom sprite sets they created at once.");
            embed.AddField("Tips", $"To delete sets and make room, visit the **`{BotConfig.bot.cmdPrefix}settings`** menu and choose [Scene Maker Settings] > [Custom Sprites] > [Manage Custom Sprite Sets].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Restricted_Channel_Access(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("This custom sprite set has been marked as **Not Safe For Work**. Visit a NSFW channel to view and use its contents.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Incompatible_File_Type(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("This file type isn't supported for uploads. Try using JPG, PNG, or GIF files instead.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Eye_Frame_Not_Found(SocialLinkerCommand command, MakerCommandData command_data, string character_name, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
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
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Mouth_Frame_Not_Found(SocialLinkerCommand command, MakerCommandData command_data, string character_name, string game_version)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            embed.WithDescription($"That mouth frame doesn’t seem to be part of {character_name}'s {command_data.Base_Sprite}{Number_Suffix(command_data.Base_Sprite)} {game_version} sprite.");
            embed.AddField("Tips", "" +
                $"Use **`{BotConfig.bot.cmdPrefix}maker {command_data.Character_Keyword} {command_data.Base_Sprite}`** to view which animation frames are available for the sprite.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        // Warnings
        public static async Task Unsupported_Character_In_Display_Name(SocialLinkerCommand command)
        {
            SocketTextChannel channel = (SocketTextChannel)command.Channel;
            await channel.SendMessageAsync(":warning: One or more of the characters in the display name is not supported by this template's font set and will not be rendered.");
        }

        public static async Task Unsupported_Character_In_Dialogue(SocialLinkerCommand command)
        {
            SocketTextChannel channel = (SocketTextChannel)command.Channel;
            await channel.SendMessageAsync(":warning: One or more of the characters entered is not supported by this template's font set and will not be rendered.");
        }

        public static async Task Content_Filter_Enabled(SocialLinkerCommand command, string template)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

            // Get the account information of the command's target
            var account = UserInfoClasses.GetAccount(user);

            var embed = new EmbedBuilder();
            var author = new EmbedAuthorBuilder
            {
                Name = "Content Filter Enabled",
                IconUrl = user.GetAvatarUrl()
            };

            embed.WithAuthor(author);

            // Determine the color and thumbnail for the embeded message.
            embed.WithColor(EmbedSettings.Get_Game_Color(template, account));
            embed.WithThumbnailUrl(EmbedSettings.Get_Game_Logo(template));

            // Write an appropriate description for the error.
            embed.WithDescription($"This command accesses content from {OfficialSetMethods.AcronymToFullTitle(template)}, which you've filtered out in your settings.");
            embed.AddField("Tips", "" +
                $"You can change your content filter settings at any time from the **`{BotConfig.bot.cmdPrefix}settings`** menu by choosing [Profile Settings] > [Content Filter].");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task API_Timeout(SocialLinkerCommand command)
        {
            SocketTextChannel channel = (SocketTextChannel)command.Channel;
            await channel.SendMessageAsync(":warning: There was some trouble retrieving template data, so we'll use some default settings for now.");
        }

        // System messages
        public static async Task Sprite_Sheet_Called_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages do not have sprite sheets to display.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frame_Sheet_Called_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages do not have animation frames to display.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Animation_Frames_Specified_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Animation frames cannot be specified for system messages.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_Missing_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("A quotation mark was found earlier than expected. There might be a sprite number missing.");
            embed.AddField("Tips", "When creating a scene, make sure to include a sprite number after the character keyword.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Missing_Dialogue_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Dialogue must be input for system messages.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Sprite_Number_And_Dialogue_Missing_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("System messages must have a sprite number and dialouge specified.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
                AutoReset = false,
                Enabled = true
            };

            // If the timer runs out, activate a function.
            error_timer.Elapsed += (sender, e) => ErrorTimer_Elapsed(sender, e, error_message, account);
        }

        public static async Task Template_Specified_First_On_System_Message(SocialLinkerCommand command)
        {
            // Create two variables for the command user and the command channel, derived from the message object taken in.
            SocketUser user = command.User;
            SocketTextChannel channel = (SocketTextChannel)command.Channel;

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
            embed.WithColor(EmbedSettings.Get_Profile_Embed_Color(account));
            embed.WithThumbnailUrl(Get_Profile_Help_Thumbnail(account));

            // Write an appropriate description for the error.
            embed.WithDescription("Template keywords must only come after the `System` character keyword.");

            var error_message = await channel.SendMessageAsync("", false, embed.Build());

            Timer error_timer = new Timer()
            {
                // Create a timer that expires as a "time out" duration for the user.
                Interval = Global.error_duration,
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