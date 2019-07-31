using CommSubSys;
using System;
using System.Net;
using System.Collections.Concurrent;
using Lobby.AppLogic;
using log4net;
using System.Threading;
using System.Security.Cryptography; 

namespace Lobby
{
    public class LobbyAppState : AppState
    {
        private static readonly RijndaelManaged rm = new RijndaelManaged();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LobbyAppState));
        public ConcurrentQueue<LobbyGM> GMs = new ConcurrentQueue<LobbyGM>();
        public ConcurrentQueue<LobbyPlayer> Pls = new ConcurrentQueue<LobbyPlayer>();
        public ConcurrentDictionary<short, LobbyGame> GamesBeingPlayed = new ConcurrentDictionary<short, LobbyGame>();

        public static AutoResetEvent gameIsReadyEvent { get; set; } = new AutoResetEvent(false);

        public Tuple<byte[], byte[]> encryptSysKeys(string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            Console.WriteLine($"sysmetric key is {BitConverter.ToString(rm.Key)}");
            Console.WriteLine($"sysmetric IV is {BitConverter.ToString(rm.IV)}");

            byte[] encryptedSymmetricKey = rsa.Encrypt(rm.Key, false);  
            byte[] encryptedSymmetricIV = rsa.Encrypt(rm.IV, false);  
            return new Tuple<byte[], byte[]>(encryptedSymmetricKey, encryptedSymmetricIV);
        }

        public void addGM(IPEndPoint UdpEp,IPEndPoint TcpEp, short gameId)
        {
            GMs.Enqueue(new LobbyGM(TcpEp, UdpEp, gameId));
        }

        public void addPlayer(IPEndPoint ep, short playerId, string name)
        {
            Pls.Enqueue(new LobbyPlayer(ep, playerId, name));
        }

        public bool gameIsReady()
        {
            bool ready = false;
            if (GMs.Count >= 1 && Pls.Count >= 2)
            {
                Logger.Debug("Setting the GameIsReadyEvent.");
                gameIsReadyEvent.Set();
                ready = true;
            }
            return ready;
        }

        private short startGame(LobbyGM GM, LobbyPlayer Pl1, LobbyPlayer Pl2)
        {
            short startedGameId = 0; 
            if (GamesBeingPlayed.TryAdd(GM.gameID, new LobbyGame(GM, Pl1, Pl2)))
                startedGameId = GM.gameID ;
            else
                Logger.Debug("Unable to start game to queue");
            return startedGameId; //Returns 0 on failure. 
        }

        //Returns true if a game was started. This may change to return a game for creating a conversation or having the private startGame function start a new converstation. 
        public short startGame()
        {
            LobbyGM gm;
            LobbyPlayer pl1;
            LobbyPlayer pl2;

            //Getting the next GM and players. 
            if (GMs.Count >= 1 && Pls.Count >= 2) //This check if a game is ready without setting the AutoReset. 
            {
                if (!GMs.TryDequeue(out gm))  { throw new Exception("Lobby is unable to add a Game Manager to a game."); }
                if (!Pls.TryDequeue(out pl1)) { throw new Exception("Lobby is unable to add a player to a game."); }
                if (!Pls.TryDequeue(out pl2)) { throw new Exception("Lobby is unable to add a player to a game."); }
                
                Logger.Debug("Starting a new game");
                return startGame(gm, pl1, pl2);
            }

            Logger.Error("Failed to start a game");
            return 0; //If it doesn't start. returns 0 on failure. 
        }

        public LobbyGame getGame(short gameID)
        {
            LobbyGame game = null;
            //if (GamesBeingPlayed.TryGetValue(gameID, out game))
            //{
            //    return game;
            //}
            //return null;
            GamesBeingPlayed.TryGetValue(gameID, out game);
            return game;
        }

        public bool removeGame(short gameID)
        {
            return GamesBeingPlayed.TryRemove(gameID, out LobbyGame removedGame);
        }

        public static RijndaelManaged getRm()
        {
            return rm;
        }
    }
}
