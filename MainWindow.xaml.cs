using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dorotas_Game_Server_v_0._3
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Hashtable clientsList = new Hashtable();
        public static Hashtable readyPlayers = new Hashtable();
        public static Hashtable sentQuestionsPlayers = new Hashtable();
        public static Hashtable sentAnswersPlayers = new Hashtable();
        public Hashtable PlayersPoints = new Hashtable();

        public List<int> intList = new List<int>();
        public List<string> playersNames = new List<string>();
        public List<string> readyPlearsList = new List<string>();

        Game game = new Game();
        Server_comunication serverComunication = new Server_comunication();

        public bool sendedAnswerBack = false;

        public MainWindow()
        {
            InitializeComponent();
            game.AmountOfPlayers = 0;
        }

        public void Dorotasgame()
        {
            game.amountOfquestions = 0;
            game.amountOfAnswers = 0;

            Thread.Sleep(100);
            wypisz("początek gry");
            wypisz("oczekiwanie na graczy...");
           
            while(true)
            {
                if (game.AmountOfPlayers > 1)
                    break;
                Thread.Sleep(200);
            }
                wypisz("osiagnieto minimalna lizbe graczy");

            while (true)
            {
                if ((game.AmountOfPlayers) == readyPlayers.Count)
                    break;
                Thread.Sleep(200);
            }
            wypisz("wszyscy gracze są gotowi. Gra się zaczyna");

            wypisz("oczekiwanie na wysłanie pytan i odpowiedzi");
            while(true)
            {
                if (((game.AmountOfPlayers) == sentQuestionsPlayers.Count) && ((game.AmountOfPlayers) == sentAnswersPlayers.Count))
                    break;

                Thread.Sleep(200);
            }

            wypisz("lista pytan dodanych do gry: \n\r");
            //wypisz wszytskie pytania:
            for(int i = 0; i != game.amountOfquestions; i++) 
            {
                wypisz(game.questions[i]);
            }

            wypisz("lista slow dodanych do gry: \n\r");
            //wypisz wszytskie slowa:
            for(int i = 0; i != game.amountOfAnswers; i++)
            {
                wypisz(game.answers[i]);
            }


            wypisz("wszyscy gracze są wysłali pytania i odpowiedzi");

            //a
            broadcast("Server|MainGameStart|Server","server",false, "statusPlayers", 0);

            wypisz("losowanie pierwszego gracza");

            int randReadingPlayer = game.RandomNumber(0, game.AmountOfPlayers);
            int randQuest = game.RandomNumber(0, game.amountOfquestions);
            int randAnswer = 0;
            wypisz("wylsowano gracza:" + randReadingPlayer.ToString()+ " wylosowano pytanie: " + randQuest.ToString() + " " + game.questions[randQuest]);

            //wyslij pytanie do gracza 
            broadcast(game.questionsList[randQuest], "Question", false, "SendQuestion", randReadingPlayer);
            game.removingQuestion(randQuest);

            //głowna faza gry
            while (true)
            {
                
                //wyslij slowa do pozostalych graczy
                for(int i=0; i!=game.AmountOfPlayers;i++)
                {
                    randAnswer = game.RandomNumber(0, game.amountOfAnswers);
                    if (randReadingPlayer != i)
                    {
                        broadcast(game.ansewrsList[randAnswer], "Answer", false, "SendQuestion", i);
                        wypisz("wyslano slowa do gracza: " + randAnswer + " slowo: " + game.ansewrsList[randAnswer]);
                        game.removingAnswer(randAnswer);
                    }

                }

                //czekaj na odpowiedz od wylosowanego gracza
                while (true)
                {
                    if (sendedAnswerBack == true)
                    {
                        sendedAnswerBack = false;
                        break;
                    }
                        

                    Thread.Sleep(200);
                }
                
                wypisz("received call back fro client");
                
                broadcast("Server|ResetLayouts|Server", "server", false, "statusPlayers", 0);
                Thread.Sleep(500);

                //sprawdz czy gra sie nie skonczyla


                //ustaw kolejnego gracza jako aktywny
                randReadingPlayer++;
                if (randReadingPlayer == game.AmountOfPlayers)
                    randReadingPlayer = 0;
                randQuest = game.RandomNumber(0, game.amountOfquestions);

                //wyslij pytanie do gracza 
                broadcast(game.questionsList[randQuest], "Question", false, "SendQuestion", randReadingPlayer);
                game.removingQuestion(randQuest);
                wypisz("wybrano gracza nr: " + randReadingPlayer);

                //sprawdz czy gra sie nie skonczyla
                if(game.endGame()=="END")
                {
                    broadcast("Server|EndGame|koniec pytan, koniec gry", "server", false, "statusPlayers", 0);
                }

                //Thread.Sleep(500);
            }
        }

        //start server
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button1.IsEnabled = false;
            Thread serverThread = new Thread(server);
            serverThread.Start();
            Thread gameThread = new Thread(Dorotasgame);
            gameThread.Start();
        }

        private void wypisz(string message)
        {
            if (textBox.Dispatcher.CheckAccess())
            {
                textBox.Text += message + "\r\n";
            }
            else
            {
                textBox.Dispatcher.Invoke(delegate
                {
                    textBox.Text += message + "\r\n";
                });
            }
        }

        public void server()
        {
            int port = 8001;
            string IpAddress = "192.168.0.164";
            Socket ServerListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAddress), port);

            wypisz("uruchamiam server... ");

            ServerListener.Bind(ep);
            ServerListener.Listen(100);

            wypisz(" ");

            int counter = 0;
            game.AmountOfPlayers = 0;

            while (true)
            {
                counter++;
                
                Socket Clientsocket = default(Socket);
                Clientsocket = ServerListener.Accept();
                wypisz("Gracz " + counter.ToString() + " dołączył do gry");

                Thread t = new Thread(new ThreadStart(() => userThred(Clientsocket, counter)));
                t.Start();

            }
        }

        public void userThred(Socket Clientsocket, int numerGracza)
        {
            string receivedData = null;
            string receivedCommand = null;
            string receivedMessage = null;
            string receivedPlayerName = null;

            bool newConnection = true;

            game.AmountOfPlayers++;

            while (true)
            {
                byte[] buff = new byte[4096];
                int size = Clientsocket.Receive(buff);

                receivedData = System.Text.Encoding.ASCII.GetString(buff);
                if (newConnection == true)
                {
                    clientsList.Add(receivedData, Clientsocket);
                    newConnection = false;
                    wypisz("akrualna liczba graczy na serverze: " + game.AmountOfPlayers);
                }

                receivedCommand = Player.cutOfCommand(receivedData);
                receivedMessage = Player.cutOfMessage(receivedData);
                receivedPlayerName = Player.cutOfPlayerName(receivedData);

                wypisz(receivedPlayerName + " (" + numerGracza.ToString() + "): " + "napisal: " + receivedData);

                switch (receivedCommand)
                {
                    case "SetName":
                        newPlayerConnected(receivedPlayerName);
                        break;
                    case "Chat":
                        broadcast(receivedMessage, receivedPlayerName, false, "Chat", 0);
                        break;
                    case "Ready":
                        playerIsReady(receivedMessage, receivedPlayerName);
                        break;
                    case "question":
                        playerSentQuestions(receivedMessage, receivedPlayerName);
                        break;
                    case "answer":
                        playerSentAnswers(receivedMessage, receivedPlayerName);
                        break;
                    case "ChosenPlayer":
                        givePointsToPlayer(receivedMessage, receivedPlayerName);
                        break;
                    case "ReRoll":
                        ReRoll(receivedMessage, receivedPlayerName);
                        break;
                    default:
                        break;
                }
                receivedData = "server respond: " + receivedData;
                size = receivedData.Length;
            }
        }

        private void ReRoll(string receivedMessage, string receivedPlayerName)
        {
            game.ReRoll(receivedMessage);

            int randAnswer = game.RandomNumber(0, game.amountOfAnswers);

            for(int i=0;i!=game.AmountOfPlayers;i++)
            {
                if(playersNames[i].ToString() == receivedPlayerName)
                {
                    broadcast(game.ansewrsList[randAnswer], "Answer", false, "ReRollBack", i);
                }
            }
            
            wypisz("wyslano slowa do gracza: " + randAnswer + " slowo: " + game.ansewrsList[randAnswer]);
            game.removingAnswer(randAnswer);

        }

        private void givePointsToPlayer(string receivedMessage, string receivedPlayerName)
        {
            string temp = null;
            int index = receivedMessage.IndexOf(";");
            receivedMessage = receivedMessage.Substring(0, index);

            string key = null;
            int temp2 = 0;
            foreach (DictionaryEntry Item in PlayersPoints)
            {
                
                if (Item.Key.ToString().Equals(receivedMessage))
                {
                    temp2 = (int)Item.Value;
                    key = Item.Key.ToString();
                    temp2 = temp2 + 1;

                }
            }

            PlayersPoints[key] = temp2;
            string msg = serverComunication.makePointsString(PlayersPoints);
            broadcast("Server|Points|" + msg, "server", false, "statusPlayers", 0);
            sendedAnswerBack = true;

        }

        private void newPlayerConnected(string playerName)
        {
            playersNames.Add(playerName);
            int intivalue = 0;
            PlayersPoints.Add(playerName, intivalue);
            string msg = serverComunication.makePlayersStatusString("gamePhase1", playersNames);
            wypisz(msg);
            broadcast(msg, "server", false, "statusPlayers", 0);
        }

        private void playerSentAnswers(string receivedMessage, string receivedPlayerName)
        {
            broadcast("gracz wyslal slowa", receivedPlayerName, false, "Chat", 0);
            sentAnswersPlayers.Add(receivedPlayerName, receivedMessage);
            game.addingAnswers(receivedMessage);
        }

        private void playerSentQuestions(string receivedMessage, string receivedPlayerName)
        {
            broadcast("gracz wyslal pytania", receivedPlayerName, false, "Chat", 0);
            sentQuestionsPlayers.Add(receivedPlayerName, receivedMessage);
            game.addingQuestions(receivedMessage);
        }

        public void playerIsReady(string receivedMessage, string receivedPlayerName)
        {

            broadcast("gracz jest gotowy do gry", receivedPlayerName, false, "Chat", 0);
            readyPlayers.Add(receivedPlayerName, receivedMessage);
            readyPlearsList.Add(receivedPlayerName);
            string msg = serverComunication.makePlayersStatusString("gamePhase2", readyPlearsList);
            broadcast(msg, "server", false, "statusPlayers", 0);
            serverComunication.makePlayersStatusString("gamePhase2", playersNames);
        }

        public static void broadcast(string msg, string uName, bool flag, string command, int rand)
        {
            int i = 0;
            foreach (DictionaryEntry Item in clientsList)
            {
                Socket broadcastSocket;
                broadcastSocket = (Socket)Item.Value;

                if(command == "statusPlayers")
                {
                    broadcastSocket.Send(Encoding.ASCII.GetBytes(msg));
                }

                if (flag == true)
                {
                    //broadcastSocket.Send(Encoding.ASCII.GetBytes(uName + " says : " + msg));
                }
                else
                {
                    if(command == "Chat")
                    {
                        broadcastSocket.Send(Encoding.ASCII.GetBytes("Server|Chat|"+uName + ": " + msg));
                    }      
                    else if(command == "SendQuestion")
                    {
                        if (rand == i)
                            broadcastSocket.Send(Encoding.ASCII.GetBytes("Server|"+uName + "|" + msg ));
                        
                    }else if(command == "ReRollBack")
                    {
                        if(rand == i)
                        broadcastSocket.Send(Encoding.ASCII.GetBytes("Server|" + "ReRollBack" + "|" + msg));
                    }
                    else if(command == "SendAnswers")
                        {
                            if(rand != i)
                                broadcastSocket.Send(Encoding.ASCII.GetBytes("Server|" + uName + "|" + msg));
                        }
                    else
                            {

                            }

                }
                i++;

            }
        }  //end broadcast function

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }


}
