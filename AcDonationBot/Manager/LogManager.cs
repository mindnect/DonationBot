using System.Collections.Generic;
using ChatAppLib;
using Verse;

namespace AlcoholV.Manager
{
    public static class LogManager
    {
        private static readonly Queue<string> LogQueue = new Queue<string>();

        public static void Init()
        {
            Client.OnLogging += OnLogging;
        }

        public static void Update()
        {
            if (LogQueue.Count == 0) return;
            Log.Message(Dequeue());
        }

        private static void OnLogging(string str)
        {
            Enqueue(str);
        }


        public static void Enqueue(string message)
        {
            LogQueue.Enqueue(message);
        }

        public static string Dequeue()
        {
            return LogQueue.Dequeue();
        }

    }
}
