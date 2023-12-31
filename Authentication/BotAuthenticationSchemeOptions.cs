﻿using Microsoft.AspNetCore.Authentication;

namespace IpDeputyApi.Authentication
{
    public class BotAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string DefaultSchemeName = "BotAuthenticationScheme";
        public string TokenHeaderName { get; set; } = "X-BOT-TOKEN";
        public string BotToken { get; set; } = "";
    }
}
