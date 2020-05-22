using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dorotas_Game_Server_v_0._3
{
    class Server_comunication
    {
        public string makePlayersStatusString(string gameStatus, List<string> playersNames)
        {
            string returnString = null;

            switch (gameStatus)
            {
                case "gamePhase1":
                    returnString = makeConnectedPLayersStrig(playersNames);
                    break;
                case "gamePhase2":
                    returnString = makeReadyPlayersString(playersNames);
                    break;
                case "gamePhase3":

                    break;
                default:
                    break;
            }
            
            return returnString;
        }

        private string makeReadyPlayersString(List<string> playersNames)
        {
            String message = "Server|ReadyPlayers|";

            int i = 0;
            foreach (var name in playersNames)
            {
                message = message + playersNames[i].ToString() + ";";
                i++;
            }

            return message;
        }

        private string makeConnectedPLayersStrig(List<string> playersNames)
        {
            String message = "Server|ConnectedPlayers|";

            int i = 0;
            foreach (var name in playersNames)
            {
                message = message + playersNames[i].ToString() + ";";
                i++;
            }

            return message;
        }

        internal string makePointsString(Hashtable PlayersPoints)
        {
            string msg=null;

            string a;
            string b;
            foreach (DictionaryEntry Item in PlayersPoints)
            {
                a = Item.Key.ToString();
                b = Item.Value.ToString();
                msg = msg + a + ": "+ b + ";";
            }

            return msg;
        }
    }
}
