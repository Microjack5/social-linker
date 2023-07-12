using System;
using Discord.WebSocket;
using SocialLinker.Core.CloudStorageTables;

namespace SocialLinker.Core.LevelSystem
{
    public static class TimeOut
    {
        internal static void SetTimeOut(SocketUser user, int duration)
        {
            //Get the account information of the user
            var account = UserInfoClasses.GetAccount(user);

            //Set the account's Time_Out_Start to the time this is called and set the duration to the given parameter
            account.Time_Out_Start = DateTime.UtcNow;
            account.Time_Out_Duration = duration;

            //Update user information with new data
            UserInfoClasses.UpdateAccount(account);
        }

        internal static string TimeOutStatus(SocketMessage message)
        {
            var account = UserInfoClasses.GetAccount(message.Author);
            DateTime current_time = DateTime.UtcNow;

            //If the user does not have a time out duration set, return
            if (account.Time_Out_Duration == 0)
            {
                return "No";
            }

            //Create a timespan for how much time has passed since the user was put in time out
            TimeSpan time_out_end = (TimeSpan)(current_time - account.Time_Out_Start);

            //If the number of minutes since the user was put in time out is less than their time out duration, they are still timed out
            if (time_out_end.TotalMinutes < account.Time_Out_Duration)
            {
                return "Yes";
            }

            //If not, reset their time out duration to zero
            account.Time_Out_Duration = 0;

            //Update user information with new data
            UserInfoClasses.UpdateAccount(account);

            return "No";
        }
    }
}
