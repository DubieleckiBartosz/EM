﻿namespace EventManagement.Application.Settings
{
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
