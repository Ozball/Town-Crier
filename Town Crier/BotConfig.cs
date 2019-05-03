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

        public BotConfig State { get { return state; } }

        BotConfig state;
        public static BotConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BotConfig();
                }

                return instance;
            }
        }

        public static void Initialize()
        {
            if (instance == null)
            {
                instance = new BotConfig();
            }
        }
        static BotConfig instance;


        BotConfig()
        {
            Load();

            //timer.Elapsed += (sender, e) => Save();
            //timer.Start();

            //Save();
        }

        public void Load()
        {
            // Do loading of BotConfig.json - See ChatCraft.cs:64

            state = FileDatabase.Read<BotConfig>("BotConfig", new Modules.ChatCraft.SlotDictionaryConverter());
            Console.WriteLine("");
        }
    }
}
