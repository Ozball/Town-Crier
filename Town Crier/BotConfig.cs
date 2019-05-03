using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class BotConfig
    {
        string token;
        string username;
        string password;
        char botPrefix;

        public BotConfig()
        {
            // Do loading of BotConfig.json
        }
    }
}
