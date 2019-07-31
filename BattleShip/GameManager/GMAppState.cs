using CommSubSys;
using System.Collections.Generic;
using log4net;
using System.Net;
using System.Security.Cryptography;
using System;
using System.Threading;

namespace GameManager
{
    public class GMAppState : AppState
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GMAppState));
        private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        private string privateKey;
        public string publicKey;
        public short gameId;
        public short p1PID;
        public short p2PID;
        public IPEndPoint P1EndPoint;
        public IPEndPoint P2EndPoint;
        public string p1Name; // Set at respondPassOff
        public string p2Name; // Set at respondPassOff
        public bool end = false;
        public List<List<short>> P1grid = null;
        public List<List<short>> P2grid = null;
        public bool P1turn = true;
        public short LastX = -1;
        public short LastY = -1;
        public bool lastShotResult = false;

        public int remainingHitsPl1 = 17; //Remaining hits for player one to win. Should be decrimented when they hit a ship. 
        public int remainingHitsPl2 = 17;

        public readonly AutoResetEvent gameEndingEvent = new AutoResetEvent(false);

        public GMAppState()
        {
            privateKey = rsa.ToXmlString(true);
            publicKey  = rsa.ToXmlString(false);
        }

        public byte[] decrypt(byte[] input)
        {
            rsa.FromXmlString(privateKey);
            byte[] decrypted = rsa.Decrypt(input, false);
            return decrypted;
        }

        public void resetState()
        {
            gameId = 0;
            p1PID = 0;
            p2PID = 0;
            P1EndPoint = null;
            P2EndPoint = null;
            end = false;
            P1grid = null;
            P2grid = null;
            P1turn = true;
            LastX = -1;
            LastY = -1;
        }

        public bool isAHit(short X, short Y, short PID)
        {
            if (PID == p1PID)
            {
                Logger.Debug($"Checking if player {PID} shot is a hit");
                if (P2grid[X][Y] == 1) // is a ship
                {
                    P2grid[X][Y] = 3;
                    remainingHitsPl1--;
                    Logger.Info($"Player 1 hit player two and has {remainingHitsPl1} hits left.");
                    Console.WriteLine($"Player 1 hit player two and has {remainingHitsPl1} hits left.");
                    lastShotResult = true;
                    return true;
                }
                else if (P2grid[X][Y] == 0)
                {
                    P2grid[X][Y] = 2;
                    lastShotResult = false;
                    return false;
                }

                Logger.Error($"error in P2's grid ");
                lastShotResult = false;
                return false;
            }
            if (PID == p2PID)
            {
                Logger.Debug($"Checking if player {PID} shot is a hit");
                if (P1grid[X][Y] == 1) // is a ship
                {
                    P1grid[X][Y] = 3;
                    remainingHitsPl2--;
                    Logger.Info($"Player 2 hit player two and has {remainingHitsPl2} hits left.");
                    Console.WriteLine($"Player 2 hit player 1 and has {remainingHitsPl2} hits left.");
                    lastShotResult = true;
                    return true;
                }
                else if (P1grid[X][Y] == 0)
                {
                    P1grid[X][Y] = 2;
                    lastShotResult = false;
                    return false;
                }

                Logger.Error($"error in P1's grid ");
                lastShotResult = false;
                return false;
            }
            Logger.Error($"{PID} is not tied to a player id in GMAppState");
            lastShotResult = false;
            return false; //setting this always to false just for testing
        }

        public bool isAWin(short PID)
        {
            if(PID== p1PID)
            {
                if (remainingHitsPl1 > 0)
                {
                    return false;
                }
                else
                {
                    Logger.Info($"Player 1 won!");
                    Console.WriteLine($"Player 1 won!");
                    end = true;
                    return true;
                }
            }
            else if(PID == p2PID){
                if (remainingHitsPl2 > 0)
                {
                    return false;
                }
                else
                {
                    Logger.Info($"Player 2 won!");
                    Console.WriteLine($"Player 2 won!");
                    end = true;
                    return true;
                }
            }
            return false; //setting this always to false just for testing
        }
    }
}
