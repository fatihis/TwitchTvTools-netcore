using System;
using System.Collections.Generic;
using System.Timers;

namespace TwitchToolsBot
{
    class Program
    {
        private static Boolean cleaner = false;
        static void Main(string[] args)
        {
            Timer aTimer  = new System.Timers.Timer();
            aTimer.Interval = 10000;
            aTimer.Enabled = true;
            IrcClient client = new IrcClient("irc.twitch.tv", 6667, "chattrack", Secrets.OAuthSecret, Secrets.trackChannel);
            Dictionary<string, int> trendDictionary = new Dictionary<string, int>();
            var pinger = new Pinger(client);
            pinger.Start();
            aTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            while (true)
            {
                Console.WriteLine("Reading message");
                var message = client.ReadMessage();
                if(message != null)
                {

                    string output2 = messageOutputFiner(message);
                    Console.WriteLine($"Message: {output2}");
                    trendDictionary = addMessageWordsToList(output2, trendDictionary);
                    foreach (KeyValuePair<string, int> kvp in trendDictionary)
                    {
                        Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    }
                    string trend = findTrend(trendDictionary, 3);
                    if (cleaner.Equals(true))
                    {
                        trendDictionary.Clear();
                        cleaner = false;
                    }
                    if (trend != "") { Console.WriteLine("We have a trend !!" + trend); client.SendChatMessage("New Trend! : '" + trend.ToUpper() + "' @" + Secrets.trackChannel); }
                }
            }
        }

        private static string messageOutputFiner(string message)
        {
            var output = message.Substring(message.IndexOf(':') + 1);
            var output2 = output.Substring(output.IndexOf(':') + 1);
            return output2;
        }

        private static string findTrend(Dictionary<string, int> inDictionary,int trendLimit)
        {
            int mostTrendNum = 0;
            string mostTrendStr = "1";
            foreach (KeyValuePair<string, int> kvp in inDictionary)
            {

                if(mostTrendNum < kvp.Value) { mostTrendNum = kvp.Value; mostTrendStr = kvp.Key; }

            }
            if(mostTrendNum > trendLimit)
            {
                return mostTrendStr;
            }

            return "";
        }

        private static void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            cleaner = true;
        }

        private static Dictionary<string, int> addMessageWordsToList(string input,Dictionary<string, int> inDictionary)
        {
            Dictionary<string, int> trendDictionary = inDictionary;
            string[] inputWords = input.Split(' ');
            for(int i = 0; i < inputWords.Length; i++)
            {
                if (!trendDictionary.ContainsKey(inputWords[i]))
                {
                    trendDictionary.Add(inputWords[i], 1);
                }
                else if(trendDictionary.ContainsKey(inputWords[i]))
                {
                    int value = trendDictionary[inputWords[i]];
                    trendDictionary[inputWords[i]] = value + 1;
                }
            }
            return trendDictionary;
        }



        
        
    }
}
