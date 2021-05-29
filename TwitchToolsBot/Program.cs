using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace TwitchToolsBot
{
    class Program
    {
        private static Boolean _cleaner = false;
        static void Main(string[] args)
        {
            TrendBot();
        }
        
        #region trendBotRegion
        public static void TrendBot()
        {
            IrcClient client = new IrcClient("irc.twitch.tv", 6667, "chattrack", Secrets.OAuthSecret, Secrets.trackChannel);
            Dictionary<string, int> trendDictionary = new Dictionary<string, int>();
            var pinger = new Pinger(client);
            Timer ıntervalTimer = new System.Timers.Timer {Interval = 10000, Enabled = true};
            pinger.Start();
            ıntervalTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            while (true)
            {
                Console.WriteLine("Reading message");
                var message = client.ReadMessage();
                if(message != null)
                {
                    var output2 = MessageOutputFiner(message);
                    Console.WriteLine($"Message: {output2}");
                    trendDictionary = AddMessageWordsToList(output2, trendDictionary);
                    foreach (KeyValuePair<string, int> kvp in trendDictionary)
                    {
                        Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    }
                    var trend = FindTrend(trendDictionary, 3);
                    if (_cleaner.Equals(true))
                    {
                        trendDictionary.Clear();
                        _cleaner = false;
                    }

                    if (trend == "") continue;
                    Console.WriteLine("We have a trend !!" + trend); client.SendChatMessage("New Trend! : '" + trend.ToUpper() + "' @" + Secrets.trackChannel);
                }
            }
        }

        private static string MessageOutputFiner(string message)
        {
            var output = message.Substring(message.IndexOf(':') + 1);
            var output2 = output.Substring(output.IndexOf(':') + 1);
            return output2;
        }

        private static string FindTrend(Dictionary<string, int> inDictionary,int trendLimit)
        {
            var mostTrendNum = 0;
            var mostTrendStr = "1";
            foreach (var kvp in inDictionary.Where(kvp => mostTrendNum < kvp.Value))
            {
                mostTrendNum = kvp.Value; mostTrendStr = kvp.Key;
            }
            return mostTrendNum > trendLimit ? mostTrendStr : "";
        }

        private static void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            _cleaner = true;
        }

        private static Dictionary<string, int> AddMessageWordsToList(string input,Dictionary<string, int> inDictionary)
        {
            Dictionary<string, int> trendDictionary = inDictionary;
            var inputWords = input.Split(' ');
            foreach (var word in inputWords)
            {
                if (!trendDictionary.ContainsKey(word))
                {
                    trendDictionary.Add(word, 1);
                }
                else if(trendDictionary.ContainsKey(word))
                {
                    int value = trendDictionary[word];
                    trendDictionary[word] = value + 1;
                }
            }
            return trendDictionary;
        }



        
        
    }
    #endregion

    #region desicionMakerRegion

    

    #endregion
}

