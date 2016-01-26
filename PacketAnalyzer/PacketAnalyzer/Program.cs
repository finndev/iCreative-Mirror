using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using EloBuddy;
using EloBuddy.Networking;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace PacketAnalyzer
{
    class Program
    {
        public static readonly string ResultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EloBuddy_Results");
        public static StreamWriter PacketsSentWriter;
        public static StreamWriter PacketsReceivedWriter;
        public static int Count;
        static void Main()
        {
            if (!Directory.Exists(ResultPath))
            {
                Directory.CreateDirectory(ResultPath);
            }
            while (File.Exists(Path.Combine(ResultPath, "PacketsSent" + Count + ".txt")))
            {
                Count++;
            }
            PacketsSentWriter = File.CreateText(Path.Combine(ResultPath, "PacketsSent " + Count + ".txt"));
            PacketsReceivedWriter = File.CreateText(Path.Combine(ResultPath, "PacketsReceived " + Count + ".txt"));
            Game.OnSendPacket += GameOnOnSendPacket;
            Game.OnProcessPacket += GameOnOnProcessPacket;
        }


        private static void GameOnOnProcessPacket(GamePacketEventArgs args)
        {
            if (PacketsReceivedWriter != null)
            {
                PacketsReceivedWriter.WriteLine("Time: " + Core.GameTickCount + ", GamePacket: " + args.GamePacket);
            }
        }
        private static void GameOnOnSendPacket(GamePacketEventArgs args)
        {
            if (PacketsSentWriter != null)
            {
                PacketsSentWriter.WriteLine("Time: " + Core.GameTickCount + ", GamePacket: " + args.GamePacket);
            }
        }
    }
}
