using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Commands
{
    public class Calculator : ModuleBase<SocketCommandContext>
    {
        [Command("calcexp", RunMode = RunMode.Async)]
        public async Task ExpCalculator(int n)
        {
            //Total Exp for Level n = 1/12 (n^4 + 4n^3 + 53n^2 - 58n)
            int current_total_exp = (((int)Math.Pow(n, 4)) + (4 * ((int)Math.Pow(n, 3))) + (53 * ((int)Math.Pow(n, 2))) - (58 * n)) / 12;

            //Next Exp for Level n = 1/6 (2n^3 + 9n^2 + 61n)
            int next_exp = ((2 * ((int)Math.Pow(n, 3))) + (9 * ((int)Math.Pow(n, 2))) + (61 * n)) / 6;

            if (n >= 99)
            {
                next_exp = 0;
            }

            await Context.Channel.SendMessageAsync($"" +
                $"Total Exp at Level {n}: {current_total_exp}\n" +
                $"Next Exp for Level {n + 1}: {next_exp}");
        }

        [Command("calclevel", RunMode = RunMode.Async)]
        public async Task LevelCalculator(int input_exp)
        {
            //Create variables
            int answer = 0;
            int next_exp = 0;
            int level_to_exp = 0;

            for(int i = 1; i <= 99; i++)
            {
                //Total Exp for Level i = 1/12 (n^4 + 4n^3 + 53n^2 - 58n)
                level_to_exp = (((int)Math.Pow(i, 4)) + (4 * ((int)Math.Pow(i, 3))) + (53 * ((int)Math.Pow(i, 2))) - (58 * i)) / 12;
                
                if (input_exp < level_to_exp)
                {
                    //If the input EXP is less than the equation's answer, it belongs to the previous level
                    answer = i - 1;
                    break;
                }
                else if (input_exp == level_to_exp)
                {
                    //If the input EXP is equal to the equation's answer, they are at the same level
                    answer = i;
                    break;
                }
            }

            //Next, calculate how much EXP is needed to level up
            int nextLevelBase = (((int)Math.Pow((answer + 1), 4)) + (4 * ((int)Math.Pow((answer + 1), 3))) + (53 * ((int)Math.Pow((answer + 1), 2))) - (58 * (answer + 1))) / 12;
            next_exp = nextLevelBase - input_exp;

            await Context.Channel.SendMessageAsync($"" +
                $"{input_exp} EXP is in the range of Level {answer}\n" +
                $"Remaining EXP until the next level: {next_exp}");
        }
    }
}
