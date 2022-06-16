using System;

namespace EventManagement.Application.Strings
{
    public class Constants
    {
        //cookie
        public const string CookieRefreshToken = "cookieTefreshTokenKey";

        //Hangfire recurring jobs

        public const string ClearTokensRecurringJob = "clearTokens";
        public const string MarkAsDeletedExpiredProposalsRecurringJob = "markAsDeleted";

        //Hangfire markers

        public const string MethodSend = "Send";
    }
}