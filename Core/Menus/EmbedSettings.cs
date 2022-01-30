using SocialLinker.Core.CloudStorageTables;
using Discord;


namespace SocialLinker.Core.Menus
{
    class EmbedSettings
    {
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

        public static string Get_Profile_Config_Thumbnail(UserInfoFields account)
        {
            // Based on the account's settings, return a thumbnail to be used on embedded menu messages.
            switch (account.Profile_Theme)
            {
                case "P3":
                    return "https://i.imgur.com/7xnoaQ7.png";

                case "P4":
                    return "https://i.imgur.com/4vtG4On.png";

                case "P5":
                    return "https://i.imgur.com/bVSsGsA.png";

                default:
                    return "";
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

        public static Color Get_Game_Color(string title, UserInfoFields account)
        {
            // Return a color to be used on embedded menu messages.
            switch (title)
            {
                case "P1-PS1":
                    return new Color(141, 61, 182);

                case "P1-PSP":
                    return new Color(141, 61, 182);

                case "P2IS-PS1":
                    return new Color(242, 55, 0);

                case "P2IS-PSP":
                    return new Color(242, 55, 0);

                case "P2EP-PS1":
                    return new Color(229, 217, 212);

                case "P2EP-PSP":
                    return new Color(229, 217, 212);

                case "P3F":
                    return new Color(0, 195, 243);

                case "P3P":
                    switch (account.P3P_TS_Color)
                    {
                        case "Male Protagonist":
                            return new Color(0, 195, 243);

                        case "Female Protagonist":
                            return new Color(255, 117, 154);

                        default:
                            return new Color(0, 195, 243);
                    }

                case "P4-PS2":
                    return new Color(241, 233, 0);

                case "P4G":
                    return new Color(241, 233, 0);

                case "P4AU":
                    return new Color(241, 233, 0);

                case "P4D":
                    return new Color(228, 0, 126);

                case "P5-PS4":
                    return new Color(212, 0, 12);

                case "P5R":
                    return new Color(212, 0, 12);

                case "P5S":
                    return new Color(212, 0, 12);

                case "BBTAG":
                    // Assign an embed color based on the user's episode header setting for the BBTAG template.
                    switch (account.BBTAG_TS_Header)
                    {
                        case "Episode P4A":
                            return new Color(241, 233, 0);

                        case "Episode Under Night In-Birth":
                            return new Color(141, 72, 249);

                        case "Episode RWBY":
                            return new Color(250, 50, 85);

                        // If the episode header is set to BlazBlue, Prologue, or Extra, default to a blue color.
                        default:
                            return new Color(66, 119, 255);
                    }

                default:
                    return new Color(0, 0, 0);
            }
        }

        public static string Get_Game_Thumbnail(string title)
        {
            // Return a thumbnail to be used on embedded menu messages.
            switch (title)
            {
                case "P1-PS1":
                    return "https://i.imgur.com/7Z6XU1d.png";

                case "P1-PSP":
                    return "https://i.imgur.com/yBXvJD7.png";

                case "P2IS-PS1":
                    return "https://i.imgur.com/5cXcMMz.png";

                case "P2IS-PSP":
                    return "https://i.imgur.com/5cXcMMz.png";

                case "P2EP-PS1":
                    return "https://i.imgur.com/3Fc0L7q.png";

                case "P2EP-PSP":
                    return "https://i.imgur.com/3Fc0L7q.png";

                case "P3F":
                    return "https://i.imgur.com/HlBRK9l.png";

                case "P3P":
                    return "https://i.imgur.com/y7x9wce.png";

                case "P4-PS2":
                    return "https://i.imgur.com/8Qs9g1d.png";

                case "P4G":
                    return "https://i.imgur.com/B7fyUwl.png";

                case "P4AU":
                    return "https://i.imgur.com/8OovCt2.png";

                case "P4D":
                    return "https://i.imgur.com/MSQjGuu.png";

                case "P5-PS4":
                    return "https://i.imgur.com/1jk1MZw.png";

                case "P5R":
                    return "https://i.imgur.com/WV32GRK.png";

                case "P5S":
                    return "https://i.imgur.com/PE7vGLY.png";

                case "BBTAG":
                    return "https://i.imgur.com/orZV4eI.png";

                default:
                    return "";
            }
        }
    }
}
