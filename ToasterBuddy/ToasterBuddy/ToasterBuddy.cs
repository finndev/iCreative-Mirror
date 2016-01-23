using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.Networking;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using Color = System.Drawing.Color;

namespace ToasterBuddy
{
    public static class ToasterBuddy
    {
        private static short _firstHeader = -1;
        private static int _toasterHeader = -1;
        private static GamePacket _toasterGamePacket;
        private static string _toasterText = "Waiting for packet.";
        private static int _timeLimit = 250;
        private static float _startTime;
        private static bool _toasterGamePacketSent;
        private static bool _keyPressed ;

        private static bool ToasterGamePacketIsReady
        {
            get { return _toasterGamePacket != null; }

        }

        static void Main()
        {
            _startTime = Game.Time;
            Game.OnSendPacket += Game_OnSendPacket;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnTick += Game_OnTick;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (ToasterGamePacketIsReady)
            {
                if (_timeLimit <= (Game.Time - _startTime) || _keyPressed)
                {
                    Send();
                }
                if (_toasterGamePacketSent)
                {
                    _toasterText = "Packet sent, the game will start :) Have fun.";
                }
                else
                {
                    _toasterText = "Packet found, you can start the game when you want (using escape or space).\nTime Left: " + Math.Truncate(_timeLimit - (Game.Time - _startTime)) + "s\nIf you want to add or remove time, just press your up/down keys from your keyboard.";
                }
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            Drawing.DrawText(10, 10, Color.White, _toasterText, 20);
        }
        private static void Game_OnWndProc(WndEventArgs args)
        {
            switch (args.Msg)
            {
                case (uint)WindowMessages.KeyDown:
                    var escapeKeys = new List<uint> { 27, 32 };
                    if (escapeKeys.Contains(args.WParam))
                    {
                        _keyPressed = true;
                    }
                    switch (args.WParam)
                    {
                        case 38:
                            _timeLimit += 10;
                            break;
                        case 40:
                            _timeLimit -= 10;
                            break;
                    }
                    break;
            }
        }

        private static void Game_OnSendPacket(GamePacketEventArgs args)
        {
            if (_firstHeader == -1)
            {
                _firstHeader = args.GamePacket.Header.OpCode;
                return;
            }
            if (_firstHeader != args.GamePacket.Header.OpCode && _toasterHeader == -1)
            {
                _startTime = Game.Time;
                _toasterGamePacket = args.GamePacket;
                _toasterHeader = args.GamePacket.Header.OpCode;
                if (!_keyPressed)
                {
                    args.Process = false;
                }
                else
                {
                    args.Process = true;
                    _toasterGamePacketSent = true;
                }
            }
        }

        private static void Send()
        {
            if (ToasterGamePacketIsReady)
            {
                if (!_toasterGamePacketSent)
                {
                    _toasterGamePacket.Send();
                    _toasterGamePacketSent = true;
                }
            }
        }

        /*
        private static void Print(this GamePacket p)
        {
            Console.WriteLine("Header: " + p.Header.OpCode);
            Console.WriteLine("vTable: " + p.Header.Algorithm.Address);
            for (var i = 0; i < p.Data.Length; i++)
            {
                Console.Write("[" + i + "]" + " = " + p.Data[i] + ", ");
            }
            Console.WriteLine();
        }*/

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Game.OnTick -= Game_OnTick;
            Game.OnSendPacket -= Game_OnSendPacket;
            Game.OnWndProc -= Game_OnWndProc;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

    }
}
