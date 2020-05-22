using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorotas_Game_Server_v_0._3
{
    public class Player
    {

        public static string cutOfCommand(string receivedData)
        {
            int found1 = receivedData.IndexOf("|");
            if (found1 >= 0)
            {
                string command = receivedData.Substring(found1 + 1, receivedData.Length - (found1 + 1));
                int found2 = command.IndexOf("|");
                command = command.Substring(0, found2);

                return command;
            }
            else
            {
                return "noComand";
            }

        }

        public static string cutOfMessage(string receivedData)
        {
            int found1 = receivedData.IndexOf("|");
            if (found1 >= 0)
            {
                string message = receivedData.Substring(found1 + 1, receivedData.Length - (found1 + 1));
                //message = message.Substring(found + 1);
                int found2 = message.IndexOf("|");
                message = message.Substring(found2 + 1, message.Length - (found2 + 1));

                return message;
            }
            else
            {
                return "";
            }
        }

        public static string cutOfPlayerName(string receivedData)
        {
            int found = 0;

            if (receivedData == null)
                return "wrong player name";

            found = receivedData.IndexOf("|");
            if (found >= 0)
            {
                string playerName = receivedData.Substring(0, found);
                return playerName;
            }
            else
            {
                return "unknown player";
            }
        }
    }
}
