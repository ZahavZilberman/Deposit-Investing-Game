using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Media;

namespace Deposit_Investing_Game
{
    public class Program
    {
        #region Everything else

        #region Converting month and year to actual datatime

        public static DateTime AnyDataUpdate(double month, double year)
        {
            string countYearZeros = "000";
            string countMonthZeros = "0";

            for(int i = 1; i <= year; i = i * 10)
            {
                countYearZeros.Remove((countYearZeros.Count() - 1));
                if (i <= month)
                {
                    countMonthZeros.Remove((countMonthZeros.Count() - 1));
                }
            }

            countYearZeros = NumOfZerosForDateTime(countYearZeros);
            countMonthZeros = NumOfZerosForDateTime(countMonthZeros);

            return DateTime.Parse($"01/{countMonthZeros}{month}/{countYearZeros}{year}");
            
        }

        public static string NumOfZerosForDateTime(string zeros)
        {
            if(zeros == "0")
            {
                return "";
            }
            return zeros;
        }

        #endregion

        #region Player entering name and then calling the actual game object

        public static void PlayerNameEnterAndGameStart(AGame game, string mode)
        {
            Console.Clear();
            Console.WriteLine();

            int maxPlayersNum = 0;
            List<Player> players = new List<Player>();

            if(mode == "time trial")
            {
                maxPlayersNum = 1;
            }

            else if(mode == "multiplayer")
            {
                maxPlayersNum = 2;
            }

            for (int playerNum = 1; playerNum <= maxPlayersNum; playerNum++)
            {
                Console.WriteLine($"Enter player {playerNum} name.");
                Console.WriteLine("You can also enter 'm' to return to the main meun.");
                Console.WriteLine();
                string name = Console.ReadLine();

                if (ShouldIReturnToMeunByEndingFunction(name))
                {
                    return;
                }

                bool isPlayerNameLegal = true;

                foreach (Deposit bankDeposit in game.bank.deposits)
                {
                    if (bankDeposit.name.ToLower() == name.ToLower())
                    {
                        isPlayerNameLegal = false;
                    }
                }

                if (name.ToLower() == game.bank.name.ToLower())
                {
                    isPlayerNameLegal = false;
                }

                if(playerNum == 2)
                {
                    if(name.ToLower() == players.ElementAt(0).name.ToLower())
                    {
                        isPlayerNameLegal = false;
                    }
                }

                while (!isPlayerNameLegal)
                {
                    Console.WriteLine();
                    Console.WriteLine("This name is illegal because something in the game (or another player) is called the same. Try typing another one:");
                    Console.WriteLine();
                    name = Console.ReadLine();

                    if (ShouldIReturnToMeunByEndingFunction(name))
                    {
                        return;
                    }

                    isPlayerNameLegal = true;

                    foreach (Deposit bankDeposit in game.bank.deposits)
                    {
                        if (bankDeposit.name.ToLower() == name.ToLower())
                        {
                            isPlayerNameLegal = false;
                        }
                    }

                    if (name.ToLower() == game.bank.name.ToLower())
                    {
                        isPlayerNameLegal = false;
                    }

                    if (playerNum == 2)
                    {
                        if (name.ToLower() == players.ElementAt(0).name.ToLower())
                        {
                            isPlayerNameLegal = false;
                        }
                    }
                }

                Player player = new Player(game, name);

                players.Add(player);

                Console.Clear();
            }

            Console.Clear();

            if (mode.ToLower() == "time trial")
            {
                Console.WriteLine($"Welcome, '{players.ElementAt(0).name}'!");
            }

            else if (mode.ToLower() == "multiplayer")
            {
                Console.WriteLine($"Welcome, '{players.ElementAt(0).name}' and '{players.ElementAt(1).name}'!");
            }

            Console.WriteLine();
            Console.WriteLine($"You will now be transfered to your 1st turn in this turn-based game.");
            Console.WriteLine();
            Console.WriteLine("Enter anything to start getting owned by the bank!");
            Console.WriteLine();
            Console.ReadLine();

            Console.Clear();

            if (mode.ToLower() == "time trial")
            {
                TimeTrial timeTrialGame = new TimeTrial();
                TimeTrial.NextTurn(game, players.ElementAt(0), timeTrialGame);
            }

            else if(mode.ToLower() == "multiplayer")
            {
                P1VsP2 multiplayerGame = new P1VsP2();
                P1VsP2.NextPlayerTurn(game, players.ElementAt(0),
                    players.ElementAt(1), multiplayerGame);
            }

            return;
        }

        #endregion

        #region Return to main meun

        public static void MainMeunMessage()
        {
            Console.WriteLine("Enter 'm' to return to the main meun.");
            Console.WriteLine();
        }

        public static bool ShouldIReturnToMeunByEndingFunction(string input)
        {
            if (input.ToLower() == "m")
            {
                return true;
            }

            return false;
        }

        #endregion

        #region writing any text

        public static void WritingText(string path)
        {
            string[] entireFile = File.ReadAllLines(path);
            foreach (string line in entireFile)
            {
                Console.WriteLine(line);
            }
        }

        #endregion

        #region writing into high score

        public static void EnteringScoreIntoHighScore(AGame game, Player player, TimeTrial timeTrial = null,
            P1VsP2 multiplayer = null)
        {
            bool gameFound = false;

            XDocument DataBase = new XDocument(XDocument.Load(@"DepositInvestingGame\HighScore.xml"));

            XElement root = DataBase.Root;

            List<XElement> games = new List<XElement>(root.Elements("Game"));

            XElement relevantGame = null;

            foreach (XElement Agame in games)
            {
                if (Agame.Element("GameName").Value.ToLower() == game.name.ToLower())
                {
                    gameFound = true;
                    relevantGame = Agame;
                }
            }

            if (gameFound)
            {
                #region The actual update

                XElement GameName = new XElement(relevantGame.Element("GameName"));
                XElement NumOfRecords = new XElement(relevantGame.Element("NumOfRecords"));
                XElement Records = new XElement(relevantGame.Element("RecordsInGameTime"));

                int newNumOfRecords = (int.Parse(NumOfRecords.Value)) + 1;
                NumOfRecords.SetValue($"{(newNumOfRecords).ToString()}");

                XElement record = new XElement(XName.Get("Record"));

                XElement month = new XElement(XName.Get("Month"));

                if (timeTrial != null)
                {
                    month.SetValue(timeTrial.month);
                }

                else if (multiplayer != null)
                {
                    month.SetValue(multiplayer.month);
                }

                XElement year = new XElement(XName.Get("Year"));

                if (timeTrial != null)
                {
                    year.SetValue(timeTrial.year);
                }

                else if (multiplayer != null)
                {
                    year.SetValue(multiplayer.year);
                }

                XElement playerName = new XElement(XName.Get("Player"));
                playerName.SetValue(player.name);

                XElement mode = new XElement(XName.Get("Mode"));

                if (timeTrial != null)
                {
                    mode.SetValue("Time trial");
                }

                else if(multiplayer != null)
                {
                    mode.SetValue("Multiplayer");
                }

                XElement additionalMoneyLeft = new XElement(XName.Get("Money"));
                additionalMoneyLeft.SetValue($"{Math.Round((player.savingsAviliabe - game.moneyToEndGame)).ToString()}");

                record.Add(playerName);
                record.Add(month);
                record.Add(year);
                record.Add(mode);
                record.Add(additionalMoneyLeft);

                Records.Add(record);

                relevantGame.RemoveNodes();
                relevantGame.Add(GameName);
                relevantGame.Add(NumOfRecords);
                relevantGame.Add(Records);

                games.RemoveAt(games.IndexOf(relevantGame));
                games.Add(relevantGame);

                root.RemoveNodes();
                root.Add(games);

                DataBase.ReplaceNodes(root);
                File.Delete(@"DepositInvestingGame\HighScore.xml");
                DataBase.Save(@"DepositInvestingGame\HighScore.xml");

                #endregion

                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Your score has been sumbmitted.");
                Console.WriteLine();
                Console.WriteLine("You can view it by going into the 'high score' chart of this game via the main meun.");
                Console.WriteLine();

                return;
            }

            throw new Exception("Game not found");
        }

        #endregion

        #region Writing a game's details

        public static void ViewGameDetailsInAllModesSoFar(AGame Agame)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"The details of the game: {Agame.name}");
            Console.WriteLine();
            Console.WriteLine($"Amount of money that all players of this game start it with: {Agame.moneyToStartWith} dollars");
            Console.WriteLine();
            Console.WriteLine($"Amount of money needed to finish this game: {Agame.moneyToEndGame} dollars");
            Console.WriteLine();
            Console.WriteLine($"Risk profile of all players in this game: {Agame.riskProfile * 100}%");
            Console.WriteLine();
            Console.WriteLine($"This game's default bank has the name: '{Agame.bank.name}'");
            Console.WriteLine();
            Console.WriteLine($"The amount of money that the bank starts the game with: {Agame.bank.startGameMoney} dollars.");
            Console.WriteLine();
            Console.WriteLine($"The income of all players in every turn in this game is: {Agame.playersIncome} dollars.");
            Console.WriteLine();
            Console.WriteLine();
        }

        #endregion

        #region Viewing a game's details before selecting a game to play

        public static void ViewGameDetailsForSelectingOne(AGame Agame, List<AGame> games, string mode)
        {
            ViewGameDetailsInAllModesSoFar(Agame);

            Console.WriteLine();
            Console.WriteLine("Enter 'd' to view all the deposits this game's bank has at the start of this game,");
            Console.WriteLine("Or enter anything else to return to selecting a game to play or view its details");
            Console.WriteLine();

            string input2 = Console.ReadLine();

            if (input2.ToLower() == "d")
            {
                ViewDeposits(Agame.bank, "choosing a game to play", Agame, null, null, games
                    , null, null, null, mode);
            }

            else
            {
                ChooseGameToPlay(games, mode);
            }

            return;
        }

        #endregion

        #region Choosing a game to play (time trial mode)

        static void ChooseGameToPlay(List<AGame> games, string mode)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("The following are a list of all the games, each of the mentioned with his name and numbered.");
            Console.WriteLine("Enter the number of the game you want to play,");
            Console.WriteLine("Or enter 'm' to return to the main meun:");
            Console.WriteLine();
            Console.WriteLine();

            for (int gameNum = 0; gameNum < games.Count; gameNum++)
            {
                Console.WriteLine();
                AGame currentGame = games.ElementAt(gameNum);
                Console.WriteLine($"{gameNum + 1}. '{currentGame.name}'");
                Console.WriteLine();
                Console.WriteLine($"(Enter '{gameNum + 1} info' [without the quote marks] to view information about this game)");
                Console.WriteLine();
            }

            string input = Console.ReadLine();

            if(ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            AGame choosenGame = null;

            bool doesThePlayerWantToPlay = true;

            for(int gameNumInput = 0; gameNumInput < games.Count; gameNumInput++)
            {
                
                if (input == (gameNumInput + 1).ToString())
                {
                    choosenGame = games.ElementAt(gameNumInput);
                }

                else if(input.ToLower() == $"{(gameNumInput + 1).ToString()} info")
                {
                    choosenGame = games.ElementAt(gameNumInput);
                    doesThePlayerWantToPlay = false;
                }

            }

            while (choosenGame == null)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again:");
                Console.WriteLine();
                input = Console.ReadLine();

                if (ShouldIReturnToMeunByEndingFunction(input))
                {
                    return;
                }

                for (int gameNumInput = 0; gameNumInput < games.Count; gameNumInput++)
                {

                    if (input == (gameNumInput + 1).ToString())
                    {
                        choosenGame = games.ElementAt(gameNumInput);
                    }

                    else if (input.ToLower() == $"{(gameNumInput + 1).ToString()} info")
                    {
                        choosenGame = games.ElementAt(gameNumInput);
                        doesThePlayerWantToPlay = false;
                    }
                }
            }

            if (doesThePlayerWantToPlay)
            {
                PlayerNameEnterAndGameStart(choosenGame, mode);
            }

            else if (!doesThePlayerWantToPlay)
            {
                ViewGameDetailsForSelectingOne(choosenGame, games, mode);
            }

            return;
        }

        #endregion

        #region all about the info viewing

        #region Choosing which info to view

        public static void ViewingAllInfo(Player player1, AGame game)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Choose the number of what to view its information: ");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. View the 'bank board' - deposits's details & states, and the bank status.");
            Console.WriteLine();
            Console.WriteLine("2. View your player status: your savings and deposits.");
            Console.WriteLine();
            Console.WriteLine("3. to view the current game's general details.");
            Console.WriteLine();
            Console.WriteLine("Or enter 'm' to go back to choosing to act/save/return to main meun.");
            Console.WriteLine();
            string input = Console.ReadLine();

            while (input != "1" && input != "2" && input != "3" && input.ToLower() != "m")
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again:");
                Console.WriteLine();
                input = Console.ReadLine();
            }

            if (input == "1")
            {
                ViewDeposits(game.bank, "bank board", game, player1);
            }

            else if (input == "2")
            {
                ViewPlayerStatus(player1, game);
            }

            else if (input == "3")
            {
                ViewingTheGameDetails(game);
            }

            else if (input.ToLower() == "m")
            {
                return;
            }

            return;
        }

        #endregion

        #region Writing a bank's deposits

        public static void ViewDeposits(Bank bank, string mode, AGame game, Player player1 = null,
            TimeTrial timeTrial = null, List<AGame> games = null,
            Player player2 = null, P1VsP2 multiplayer = null, Player currentPlayer = null,
            string gameModeForSelection = null)
        {
            #region for high score mode and choosing game mode

            if (mode.ToLower() == "high score" || mode.ToLower() == "choosing a game to play")
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine($"The list of the deposits offered by the bank named '{bank.name}',");
                Console.WriteLine($"While this bank starts the game with {bank.startGameMoney} dollars,");
                Console.WriteLine($"In the game called '{game.name}':");
                Console.WriteLine();

                for (int depositNum = 0; depositNum < bank.deposits.Count; depositNum++)
                {
                    Deposit currentDeposit = bank.deposits.ElementAt(depositNum);

                    Console.WriteLine();
                    Console.WriteLine($"{depositNum + 1}. '{currentDeposit.name}':");
                    Console.WriteLine();
                    Console.WriteLine($"Time span of the deposit: {currentDeposit.timeSpan} years");
                    Console.WriteLine();
                    Console.WriteLine($"The default (lowest offered) interest of this deposit per year: {(currentDeposit.DefaultinterestPerYear - 1) * 100}%");
                    Console.WriteLine();
                    Console.WriteLine($"The minimum money necessary to put in this deposit to buy it is: {currentDeposit.minimumMoneyForDeposit} dollars");
                    Console.WriteLine();
                    Console.WriteLine($"The gap of months between each exit point from this deposit is: {currentDeposit.exitPointsGap}");
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine();

                if (mode.ToLower() == "high score")
                {
                    Console.WriteLine($"Enter anything to return to view the records for the game called '{game.name}'.");
                    Console.WriteLine();
                    Console.ReadLine();
                }

                else if (mode.ToLower() == "choosing a game to play")
                {
                    Console.WriteLine("Enter anything to return to viewing this game's general details.");
                    Console.WriteLine();
                    Console.ReadLine();

                    ViewGameDetailsForSelectingOne(game, games, gameModeForSelection);
                }

            }

            #endregion

            #region viewing bank board in any game mode (either to buy or view)

            if (mode.ToLower() == "bank board" || mode.ToLower() == "time trial buy deposit" 
                || mode.ToLower() == "time trial release deposit"
                || mode.ToLower() == "multiplayer buy deposit"
                || mode.ToLower() == "multiplayer release deposit")
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine($"This is the list of all {bank.numOfDeposits} deposits offered by the bank '{bank.name}',");
                Console.WriteLine();
                Console.WriteLine($"Who stated the game with {bank.startGameMoney} dollars,");
                Console.WriteLine();
                Console.WriteLine($"currently has {bank.money} dollars left,");
                Console.WriteLine();
                Console.WriteLine($"and owns {bank.numOfDepositsAviliabe} deposits right now:");
                Console.WriteLine();
                Console.WriteLine();

                for (int depositNum = 0; depositNum < bank.deposits.Count; depositNum++)
                {
                    Deposit currentDeposit = bank.deposits.ElementAt(depositNum);

                    Console.WriteLine();
                    Console.WriteLine($"{depositNum + 1}. '{currentDeposit.name}'");
                    Console.WriteLine();

                    if(currentPlayer == null &&
                        player1.depositsOwned.Contains(currentDeposit))
                    {
                        Console.WriteLine("(You currently own this deposit)");
                        Console.WriteLine();
                    }

                    else if(mode.ToLower() == "multiplayer buy deposit"
                        || mode.ToLower() == "multiplayer release deposit")
                    {
                        if(currentPlayer.depositsOwned.Contains(currentDeposit))
                        {
                            Console.WriteLine("(You currently own this deposit)");
                            Console.WriteLine();
                        }

                        else if(currentPlayer.name == player1.name
                            && player2.depositsOwned.Contains(currentDeposit))
                        {
                            Console.WriteLine($"(The other player, '{player2.name}', currently owns this deposit)");
                            Console.WriteLine();
                        }

                        else if(currentPlayer.name == player2.name
                            && player1.depositsOwned.Contains(currentDeposit))
                        {
                            Console.WriteLine($"(The other player, '{player1.name}', currently owns this deposit)");
                            Console.WriteLine();
                        }
                    }

                }

                Console.WriteLine();
                Console.WriteLine();

                if (mode.ToLower() == "bank board")
                {
                    Console.WriteLine("Enter the number of a deposit to view all its current details,");
                    Console.WriteLine("Or enter 'm' to return to choosing if to act/save/view other info/return to the main meun");
                    Console.WriteLine();
                    string input = Console.ReadLine();

                    bool DidTheInputIncludeADeposit = false;

                    for (int choosenDepositNum = 0; choosenDepositNum < bank.deposits.Count; choosenDepositNum++)
                    {
                        if (input == (choosenDepositNum + 1).ToString())
                        {
                            DidTheInputIncludeADeposit = true;
                        }
                    }

                    while(input.ToLower() != "m" && !DidTheInputIncludeADeposit)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid input. Enter again:");
                        Console.WriteLine();
                        input = Console.ReadLine();

                        for (int choosenDepositNum = 0; choosenDepositNum < bank.deposits.Count; choosenDepositNum++)
                        {
                            if (input == (choosenDepositNum + 1).ToString())
                            {
                                DidTheInputIncludeADeposit = true;
                            }
                        }
                    }

                    if (ShouldIReturnToMeunByEndingFunction(input))
                    {
                        return;
                    }

                    for (int choosenDepositNum = 0; choosenDepositNum < bank.deposits.Count; choosenDepositNum++)
                    {
                        if (input == (choosenDepositNum + 1).ToString())
                        {
                            ViewASingleDeposit(bank, bank.deposits.ElementAt(choosenDepositNum));

                            if(multiplayer != null)
                            {
                                ViewDeposits(game.bank, "bank board", game, player1,
                    null, null, player2, multiplayer, currentPlayer);
                            }

                            else if(timeTrial != null)
                            {
                                ViewDeposits(game.bank, "bank board", game, player1, timeTrial);
                            }
                        }
                    }

                    return;
                }
            }

            #endregion
        }

        #region Viewing a deposit

        public static void ViewASingleDeposit(Bank bank, Deposit choosenDeposit)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" == Deposit page ===");
            Console.WriteLine();
            Console.WriteLine($"Deposit name: '{choosenDeposit.name}'");
            Console.WriteLine();

            if (choosenDeposit.whoItBelongsTo == bank.name)
            {
                Console.WriteLine($"This deposit is currently owned by: '{choosenDeposit.whoItBelongsTo}' (the bank)");
            }

            else
            {
                Console.WriteLine($"This deposit is currently owned by: '{choosenDeposit.whoItBelongsTo}' (a player)");
            }

            Console.WriteLine($"(If the bank owns it, it means no player has it right now)");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Time span of the deposit: {choosenDeposit.timeSpan} years");
            Console.WriteLine();
            Console.WriteLine($"The default (lowest offered) interest of this deposit is: {((choosenDeposit.DefaultinterestPerYear - 1) * 100)}% per year");
            Console.WriteLine();
            Console.WriteLine($"The minimum money necessary to put in this deposit to buy it is: {choosenDeposit.minimumMoneyForDeposit} dollars");
            Console.WriteLine();
            Console.WriteLine($"The gap of months between each exit point from this deposit is: {choosenDeposit.exitPointsGap}");
            Console.WriteLine();

            if (choosenDeposit.exitPointsGap == 0)
            {
                Console.WriteLine("(Which means, there are no dates in which the deposit can be released by any player owning it).");
                Console.WriteLine();
            }

            if (choosenDeposit.whoItBelongsTo != bank.name)
            {
                Console.WriteLine($"The actual interest the deposit is supposed to produce to its owner is: {(choosenDeposit.actualInterestPerYear - 1) * 100}% per year");
                Console.WriteLine();
                Console.WriteLine($"The amount of money the owner put in the deposit is: {choosenDeposit.amountOfMoneyPutInDeposit} dollars");
                Console.WriteLine();
                Console.WriteLine($"The deposit was bought in: year {choosenDeposit.whenWasItBought.Year}, month {choosenDeposit.whenWasItBought.Month}");
                Console.WriteLine();
                Console.WriteLine($"The deposit is supposed to be released in: year {choosenDeposit.whenItShouldBeReleased.Year}, month {choosenDeposit.whenItShouldBeReleased.Month}");
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Enter anything in order to return to the bank board.");
            Console.WriteLine();
            Console.ReadLine();

            return;
        }

        #endregion

        #endregion

        #region Viewing the player's status

        public static void ViewPlayerStatus(Player player1, AGame game)
        {
            bool doesThePlayerEvenHaveDeposits = player1.depositsOwned.Count > 0;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"The status of the player: {player1.name}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"The amount of money the player has (disincluding money invested in deposits): {player1.savingsAviliabe}");
            Console.WriteLine();
            Console.WriteLine();

            if (!doesThePlayerEvenHaveDeposits)
            {
                Console.WriteLine($"The player has no deposits right now.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"The player has deposit/s.");
                Console.WriteLine();
            }

            Console.WriteLine();
            if (doesThePlayerEvenHaveDeposits)
            {
                Console.WriteLine("Enter 'd' to view the relevant details of the player's deposits.");
                Console.WriteLine("(Doing that will not open the bank board, but simply the list of the deposit's with their names)");
                Console.WriteLine();
            }

            Console.WriteLine("Enter 'm' to go back into choosing if to act/save/view other info/return to the main meun.");
            Console.WriteLine();
            string input = Console.ReadLine();

            if (doesThePlayerEvenHaveDeposits)
            {
                while (input.ToLower() != "d" && input.ToLower() != "m")
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Enter again: ");
                    Console.WriteLine();
                    input = Console.ReadLine();
                }
            }

            else
            {
                while (input.ToLower() != "m")
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Enter again: ");
                    Console.WriteLine();
                    input = Console.ReadLine();
                }
            }

            if (input.ToLower() == "m")
            {
                return;
            }

            else if (input.ToLower() == "d")
            {
                ViewPlayerDeposits(game, player1);

                ViewPlayerStatus(player1, game);

                return;
            }

        }

        #region Viewing the player's deposits in player view

        public static void ViewPlayerDeposits(AGame game, Player player1)
        {
            List<Deposit> playerDeposits = new List<Deposit>();
            playerDeposits = player1.depositsOwned;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"The player '{player1.name}' has the following {playerDeposits.Count} deposits:");
            Console.WriteLine();

            for (int i = 0; i < playerDeposits.Count; i++)
            {
                Deposit currentDeposit = playerDeposits.ElementAt(i);
                Console.WriteLine();
                Console.WriteLine($"{i + 1}. {currentDeposit.name}:");
                Console.WriteLine();
                Console.WriteLine($"The amount of money the player put in this deposit: {currentDeposit.amountOfMoneyPutInDeposit} dollars");
                Console.WriteLine();
                Console.WriteLine($"The actual interest this player is supposed to receive for this deposit per year (when it'll be released) is:");
                Console.WriteLine($"{(currentDeposit.actualInterestPerYear - 1) * 100}% per year");
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("(All the other information about those deposits is avliabe in the bank board)");
            Console.WriteLine();
            Console.WriteLine("Enter anything to return viewing the player's general status.");
            Console.WriteLine();
            Console.ReadLine();

            return;
        }

        #endregion

        #endregion

        #region Viewing the game's general details

        public static void ViewingTheGameDetails(AGame game)
        {
            ViewGameDetailsInAllModesSoFar(game);

            Console.WriteLine("Enter anything in order to return to choose if to act/save/view other info/return to main meun");
            Console.WriteLine();
            Console.ReadLine();

            return;
        }

        #endregion

        #endregion

        #region Text that appears when choosing an action at a turn

        public static void WritingActionChoices()
        {
            Console.WriteLine("Enter the number of the action to take in this turn:");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. Buy a deposit");
            Console.WriteLine();
            Console.WriteLine("2. Ask to release a deposit you own");
            Console.WriteLine();
            Console.WriteLine("3. Do nothing");
            Console.WriteLine();
            Console.WriteLine("You also may enter 'm' to return to choose if to act/view/save/return to main meun.");
            Console.WriteLine();
        }

        #endregion

        #region Text that appears when choosing a deposit to buy or release

        public static void WritingTextForChoosingADeposit(string mode = null)
        {
            Console.WriteLine("To view the full details of every deposit, go back to view the bank board");
            Console.WriteLine();

            if(mode == null)
            {
                mode = "buy";
            }

            Console.WriteLine($"Enter the number of the deposit you wish to {mode}.");
            Console.WriteLine();
            Console.WriteLine("Or enter 'v' to go stright into viewing the bank board,");
            Console.WriteLine();
            Console.WriteLine("Or enter 'm' to return to choosing if to act/view/save/return to the main meun.");
            Console.WriteLine();
        }

        #endregion

        #region Text that blocks the player from buying a deposit

        public static void WritingYouAlreadyOwnTheDeposit()
        {
            Console.WriteLine();
            Console.WriteLine("You alredy own this deposit!");
            Console.WriteLine();
            Console.WriteLine("Enter anything to return to choosing a deposit to buy.");
            Console.WriteLine();
            Console.ReadLine();
        }

        public static void WritingYouDontHaveEnoughMoneyForDeposit(AGame game, int depositNumIfExists)
        {
            Console.WriteLine();
            Console.WriteLine("You don't have enough money to buy this deposit!");
            Console.WriteLine($"(You need to have at least {game.bank.deposits.ElementAt(depositNumIfExists).minimumMoneyForDeposit / game.riskProfile} dollars for this deposit -.-)");
            Console.WriteLine("(That's the minimum money defined for this deposit divided by your risk profile - that's the actual amount of money you need)");
            Console.WriteLine();
            Console.WriteLine("Enter anything to return to choosing a deposit to buy.");
            Console.WriteLine();
            Console.ReadLine();
        }

        #endregion

        #region Text that appears when a panic mode turn starts

        public static void PanickModeStateNotification(bool isMultiplayer, Player currentPlayer = null)
        {
            Console.Clear();

            if (isMultiplayer)
            {
                P1VsP2.WritingWhichPlayerTurnItIs(currentPlayer);
            }

            Console.WriteLine();
            WritingText(@"DepositInvestingGame\Notifications\PanickMode.txt");

            Console.ReadLine();
            return;
        }

        #endregion

        #region Finding the deposit to release for player in panic mode

        public static Deposit WhichDepositToReleaseInPanicMode(Player player)
        {
            List<string[]> depositsMoneyPotential = new List<string[]>();

            double maxPotential = 0;

            foreach (Deposit deposit in player.depositsOwned)
            {
                string[] nameAndPotential = new string[2];

                double theFinalInterestAtSupposedRelease = deposit.actualInterestPerYear;

                for (int i = 1; i < deposit.timeSpan; i++)
                {
                    theFinalInterestAtSupposedRelease = theFinalInterestAtSupposedRelease * deposit.actualInterestPerYear;
                }

                double moneyItCanProduce = deposit.amountOfMoneyPutInDeposit * (theFinalInterestAtSupposedRelease - 1);

                nameAndPotential[0] = deposit.name;
                nameAndPotential[1] = moneyItCanProduce.ToString();

                if (maxPotential < moneyItCanProduce)
                {
                    maxPotential = moneyItCanProduce;
                }

                depositsMoneyPotential.Add(nameAndPotential);
            }

            Deposit depositToRelease = new Deposit("", 0, "", 0, 0, 0);

            foreach (string[] depositDetails in depositsMoneyPotential)
            {
                if (maxPotential.ToString() == depositDetails[1]) // if this is the riskiest deposit
                {
                    foreach (Deposit playerDeposit in player.depositsOwned)
                    {
                        if (playerDeposit.name == depositDetails[0]) // once you find it in the player deposits..
                        {
                            depositToRelease = playerDeposit;
                        }
                    }
                }
            }

            return depositToRelease;
        }

        #endregion

        #region function that gets the games and calls the main meun

        static void getTheGamesAndStart()
        {
            List<AGame> games = new List<AGame>();
            games = WritingAllGamesInformation();

            for (int i = 0; i < double.MaxValue; i++)
            {
                MainMeun(games);

                games = WritingAllGamesInformation();
            }
        }

        #endregion

        #region function that builds and returns all games into objects

        public static List<AGame> WritingAllGamesInformation()
        {
            List<AGame> games = new List<AGame>();

            List<string> gamePaths = new List<string>();

            XDocument gamesNavigator = new XDocument(XDocument.Load(@"DepositInvestingGame\GamesNavigator.xml"));
            XElement root = gamesNavigator.Root;

            IEnumerable<XElement> docGames = new List<XElement>(root.Elements("Game"));
            
            foreach(XElement docGame in docGames)
            {
                XElement docGamePath = new XElement(docGame.Element("Path"));

                gamePaths.Add(docGamePath.Value);
            }

            foreach(string aGamePath in gamePaths)
            {
                AGame game = new AGame(aGamePath);
                games.Add(game);
            }

            return games;
        }

        #endregion

        #region returning bank to its default

        public static Bank ReturnBankToDefault(List<AGame> games, string gameName)
        {
            foreach(AGame game in games)
            {
                if(game.name.ToLower() == gameName.ToLower())
                {
                    return game.bank;
                }
            }

            throw new Exception("Game not found");
        }

        #endregion

        #region viewing high score

        static void ViewHighScore(List<AGame> games)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("The following are a list of all the games, each of the mentioned with his name.");
            Console.WriteLine("Enter the number of the game you want to view his high score table.");
            Console.WriteLine();
            MainMeunMessage();

            for (int gameNum = 0; gameNum < games.Count; gameNum++)
            {
                Console.WriteLine($"{gameNum+1}. {games.ElementAt(gameNum).name}");
                Console.WriteLine();
            }

            string input = Console.ReadLine();

            if(ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            #region checking user input and then opening the high score

            bool isGameFound = false;
            int numForGame = 0;

            for(int gameNum = 0; gameNum < games.Count; gameNum++)
            {
                if(input == (gameNum + 1).ToString())
                {
                    isGameFound = true;
                    numForGame = gameNum;
                }
            }

            if(isGameFound)
            {
                HighScore highScore = new HighScore(games.ElementAt(numForGame));
                highScore.next(games.ElementAt(numForGame));
            }

            else
            {
                while(!isGameFound)
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Enter again: ");
                    Console.WriteLine();
                    input = Console.ReadLine();

                    if (ShouldIReturnToMeunByEndingFunction(input))
                    {
                        return;
                    }

                    for (int gameNum = 0; gameNum < games.Count; gameNum++)
                    {
                        if (input == (gameNum + 1).ToString())
                        {
                            isGameFound = true;
                            numForGame = gameNum;
                        }
                    }
                }

                HighScore highScore = new HighScore(games.ElementAt(numForGame));
                highScore.next(games.ElementAt(numForGame));
            }

            #endregion

            return;
        }

        #endregion

        #region Saving game

        public static void SavingProgress(AGame game, Player player1,
            Player player2 = null, TimeTrial timeTrial = null, P1VsP2 multiplayer = null)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Enter the name of the state you want to save,");
            Console.WriteLine("Or enter 'm' to return to choose if to do an action/view info/return to the main meun");
            Console.WriteLine();
            string saveName = Console.ReadLine();

            if (saveName.ToLower() == "m")
            {
                return;
            }

            string overOrNot =
                OverwritingSave(saveName, game, player1, player2, timeTrial, multiplayer);

            if (overOrNot == "yes")
            {
                return;
            }

            else
            {
                List<string> allSaveDetails = new List<string>();

                allSaveDetails.Add($"Game:");
                allSaveDetails.Add($"{game.name}");

                allSaveDetails.Add($"Bank name:");
                allSaveDetails.Add($"{game.bank.name}");

                allSaveDetails.Add("All deposits:");

                List<Deposit> allDeposits = new List<Deposit>();
                allDeposits = game.bank.deposits;

                for (int depositNum = 0; depositNum < allDeposits.Count; depositNum++)
                {
                    Deposit currentDeposit = allDeposits.ElementAt(depositNum);

                    allSaveDetails.Add($"{currentDeposit.name}");
                    allSaveDetails.Add($"{currentDeposit.whoItBelongsTo}");
                    allSaveDetails.Add($"{currentDeposit.wasItReleasedInLastTurn.ToString()}");

                    if (currentDeposit.whoItBelongsTo != game.bank.name)
                    {
                        allSaveDetails.Add($"{currentDeposit.actualInterestPerYear}");
                        allSaveDetails.Add($"{currentDeposit.amountOfMoneyPutInDeposit}");
                        allSaveDetails.Add($"{currentDeposit.whenWasItBought}");
                        allSaveDetails.Add($"{currentDeposit.whenItShouldBeReleased}");
                    }

                    if (currentDeposit.wasItReleasedInLastTurn)
                    {
                        allSaveDetails.Add($"{currentDeposit.whoReleasedItLastTurn}");
                    }
                }

                allSaveDetails.Add("Time:");

                if (timeTrial != null)
                {
                    allSaveDetails.Add($"{timeTrial.month}");
                    allSaveDetails.Add($"{timeTrial.year}");
                }

                if(multiplayer != null)
                {
                    allSaveDetails.Add($"{multiplayer.month}");
                    allSaveDetails.Add($"{multiplayer.year}");
                }

                allSaveDetails = playerInfoSave(player1, allSaveDetails, 1);

                if (player2 != null)
                {
                    allSaveDetails = playerInfoSave(player2, allSaveDetails, 2);
                }

                allSaveDetails.Add("Bank:");
                allSaveDetails.Add($"{game.bank.name}");
                allSaveDetails.Add($"{game.bank.isBankrupt.ToString()}");
                allSaveDetails.Add($"{game.bank.money}");
                allSaveDetails.Add($"{game.bank.numOfDepositsAviliabe}");

                string[] allInfo = new string[allSaveDetails.Count];

                for (int lineNum = 0; lineNum < allSaveDetails.Count; lineNum++)
                {
                    allInfo[lineNum] = allSaveDetails.ElementAt(lineNum);
                }

                if (timeTrial != null)
                {
                    File.WriteAllLines($@"DepositInvestingGame\Saves\TimeTrial\{saveName}.txt", allInfo);
                }

                if(multiplayer != null)
                {
                    File.WriteAllLines($@"DepositInvestingGame\Saves\Multiplayer\{saveName}.txt", allInfo);
                }

                Console.WriteLine();
                Console.WriteLine("Your state has been saved successfully.");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return to choose if to act/view info/return to the main meun");
                Console.WriteLine();
                Console.ReadLine();

                return;
            }
        }

        #region saving the player/s information specifcally

        public static List<string> playerInfoSave(Player player, List<string> allSaveDetails,
            int playerNum)
        {
            allSaveDetails.Add($"Player {playerNum}:");
            allSaveDetails.Add($"{player.name}");
            allSaveDetails.Add($"This player's deposits number:");
            allSaveDetails.Add($"{player.depositsOwned.Count}");

            foreach (Deposit deposit in player.depositsOwned)
            {
                allSaveDetails.Add($"{deposit.name}");
            }

            allSaveDetails.Add($"Other player {playerNum} details:");
            allSaveDetails.Add($"{player.InPanickMode.ToString()}");
            allSaveDetails.Add($"{player.RowOfChoosingToDoNothing}");
            allSaveDetails.Add($"{player.savingsAviliabe}");
            allSaveDetails.Add($"{player.hasThePlayerFinishedHisTurn.ToString()}");

            return allSaveDetails;
        }

        #endregion

        #region Asking the player if to overwrite

        public static string OverwritingSave(string saveName, AGame game, Player player1,
            Player player2 = null, TimeTrial timeTrial = null, P1VsP2 multiplayer = null)
        {
            string pathForDirectory = "";

            if(timeTrial != null)
            {
                pathForDirectory = $@"DepositInvestingGame\Saves\TimeTrial";
            }

            else if(multiplayer != null)
            {
                pathForDirectory = $@"DepositInvestingGame\Saves\Multiplayer";
            }

            DirectoryInfo info = new DirectoryInfo(pathForDirectory);
            FileInfo[] files = info.GetFiles();
            foreach (FileInfo file in files)
            {
                if (saveName.ToLower() == $"{file.Name.Remove(file.Name.Count() - 4, 4).ToLower()}")
                {
                    Console.WriteLine();
                    Console.WriteLine("There's already a game save with this name in this mode! Overwrite it?");
                    Console.WriteLine("(Enter 'y' to overwrite, or enter 'n' to cancel the save)");
                    Console.WriteLine();
                    string overwrite = Console.ReadLine();

                    if (overwrite.ToLower() == "n")
                    {
                        SavingProgress(game, player1, player2, timeTrial, multiplayer);

                        return "yes";
                    }

                    if(overwrite.ToLower() == "y")
                    {
                        return "no";
                    }

                    else if (overwrite.ToLower() != "y" && overwrite.ToLower() != "n")
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine($"I don't get it!!!111 overwrite '{saveName}' or not?!");
                        Console.WriteLine();
                        OverwritingSave(saveName, game, player1, player2, timeTrial, multiplayer);
                    }
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Loading a game file for any mode

        #region choosing the mode

        public static void ChooseAGameToLoad()
        {
            List<string> modes = new List<string>();
            modes.Add("Time Trial");
            modes.Add("Multiplayer"); //P1VsP2 object

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Enter the number of the mode the saved game belongs to,");
            Console.WriteLine("Or enter 'm' to return to the main meun: ");
            Console.WriteLine();

            for(int modeNum = 0; modeNum < modes.Count; modeNum++)
            {
                Console.WriteLine($@"{modeNum + 1}. {modes.ElementAt(modeNum)}");
                Console.WriteLine();
            }

            Console.WriteLine();
            string input = Console.ReadLine();

            if(ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            bool doesTheInputMatchAMode = false;

            if(input == "1")
            {
                doesTheInputMatchAMode = true;

                ChoosingAGameToLoadInAMode("time trial");
            }

            else if(input == "2")
            {
                doesTheInputMatchAMode = true;

                ChoosingAGameToLoadInAMode("player vs player");
            }


            else
            {
                while(!doesTheInputMatchAMode)
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Enter again: ");
                    Console.WriteLine();
                    input = Console.ReadLine();
                    if (ShouldIReturnToMeunByEndingFunction(input))
                    {
                        return;
                    }

                    if(input == "1")
                    {
                        doesTheInputMatchAMode = true;

                        ChoosingAGameToLoadInAMode("time trial");
                    }

                    else if (input == "2")
                    {
                        doesTheInputMatchAMode = true;

                        ChoosingAGameToLoadInAMode("player vs player");
                    }
                }

            }

            return;
        }

        #endregion

        #region choosing a game to load

        public static void ChoosingAGameToLoadInAMode(string mode)
        {
            DirectoryInfo info = null;

            if (mode.ToLower() == "time trial")
            {
                info = new DirectoryInfo($@"DepositInvestingGame\Saves\TimeTrial");
            }

            else if(mode.ToLower() == "player vs player")
            {
                info = new DirectoryInfo($@"DepositInvestingGame\Saves\Multiplayer");
            }

            FileInfo[] files = info.GetFiles();

            if(files.Count() == 0)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("There are no saved files for this mode.");
                Console.WriteLine("Enter anything to return to the main meun.");
                Console.WriteLine();
                Console.ReadLine();

                return;
            }

            else
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Enter the number of the save you want to open,");
                Console.WriteLine("Or enter 'm' to return to the main meun:");
                Console.WriteLine();

                for (int fileNum = 0; fileNum < files.Count(); fileNum++)
                {
                    string countedFileName = $"{files.ElementAt(fileNum).Name}".Remove((files.ElementAt(fileNum).Name.Length - 4), 4);
                    Console.WriteLine();
                    Console.WriteLine($"{fileNum + 1}. {countedFileName}");
                }
                Console.WriteLine();

                string wantedSave = Console.ReadLine();

                if(ShouldIReturnToMeunByEndingFunction(wantedSave))
                {
                    return;
                }

                FileInfo wantedFile = new FileInfo("a.t");

                for(int fileNum = 0; fileNum < files.Count(); fileNum++)
                {
                    if(wantedSave == (fileNum + 1).ToString())
                    {
                        wantedFile = files.ElementAt(fileNum);
                    }
                }

                if(wantedFile.ToString() != "a.t")
                {
                    LoadGame(wantedFile, mode);
                }

                else
                {
                    while (wantedFile.ToString() == "a.t")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid input. Enter again:");
                        Console.WriteLine();
                        wantedSave = Console.ReadLine();

                        if (ShouldIReturnToMeunByEndingFunction(wantedSave))
                        {
                            return;
                        }

                        for (int fileNum = 0; fileNum < files.Count(); fileNum++)
                        {
                            if (wantedSave == (fileNum + 1).ToString())
                            {
                                wantedFile = files.ElementAt(fileNum);
                            }
                        }

                    }

                    LoadGame(wantedFile, mode);
                }

                return;
            }
        }

        #endregion

        #region The actual loading of a specific game

        public static void LoadGame(FileInfo saveFile, string mode)
        {
            Console.Clear();

            if (saveFile.Extension != ".txt")
            {
                throw new Exception("Error: Incorrect file format");
            }

            string[] fileInfo = null;

            if (mode.ToLower() == "time trial")
            {
                fileInfo = File.ReadAllLines($@"DepositInvestingGame\Saves\TimeTrial\{saveFile.Name}");
            }

            else if(mode.ToLower() == "player vs player")
            {
                fileInfo = File.ReadAllLines($@"DepositInvestingGame\Saves\Multiplayer\{saveFile.Name}");
            }

            #region and its list variable

            List<string> fileInfoInList = new List<string>();

            foreach(string line in fileInfo)
            {
                fileInfoInList.Add(line);
            }

            #endregion

            #region Creating the current game object (including bank)

            List<AGame> allGames = WritingAllGamesInformation();
            string gameName = fileInfo.ElementAt(1);

            foreach(AGame game in allGames)
            {
                if (game.name == gameName)
                {
                    #region updating bank and its deposits

                    int bankStartIndex = fileInfoInList.IndexOf("Bank:");
                    game.bank.name = fileInfoInList.ElementAt(bankStartIndex + 1);
                    game.bank.isBankrupt = bool.Parse(fileInfoInList.ElementAt(bankStartIndex + 2));
                    game.bank.money = double.Parse(fileInfoInList.ElementAt(bankStartIndex + 3));
                    game.bank.numOfDepositsAviliabe = int.Parse(fileInfoInList.ElementAt(bankStartIndex + 4));

                    List<Deposit> updatedDeposits = new List<Deposit>();

                    List<Deposit> allDeposits = new List<Deposit>();
                    foreach (Deposit deposit in game.bank.deposits)
                    {
                        allDeposits.Add(deposit);
                    }

                    int lineWhereDepositStart = fileInfoInList.IndexOf("All deposits:");

                    for (int depositNum = 0; depositNum < allDeposits.Count; depositNum++)
                    {
                        Deposit currentDeposit = allDeposits.ElementAt(depositNum);

                        foreach (Deposit depositAtStart in game.bank.deposits)
                        {
                            if (depositAtStart.name == currentDeposit.name)
                            {
                                #region this is where every deposit will actually update

                                int depositIndexStartName = fileInfoInList.IndexOf(currentDeposit.name);

                                currentDeposit.whoItBelongsTo = fileInfoInList.ElementAt(depositIndexStartName + 1);
                                currentDeposit.wasItReleasedInLastTurn = bool.Parse(fileInfoInList.ElementAt(depositIndexStartName + 2));

                                if (currentDeposit.whoItBelongsTo != game.bank.name)
                                {
                                    currentDeposit.actualInterestPerYear = double.Parse(fileInfoInList.ElementAt(depositIndexStartName + 3));
                                    currentDeposit.amountOfMoneyPutInDeposit = double.Parse(fileInfoInList.ElementAt(depositIndexStartName + 4));
                                    currentDeposit.whenWasItBought = DateTime.Parse(fileInfoInList.ElementAt(depositIndexStartName + 5));
                                    currentDeposit.whenItShouldBeReleased = DateTime.Parse(fileInfoInList.ElementAt(depositIndexStartName + 6));
                                }

                                if (currentDeposit.whoItBelongsTo != game.bank.name && currentDeposit.wasItReleasedInLastTurn)
                                {
                                    currentDeposit.whoReleasedItLastTurn = fileInfoInList.ElementAt(depositIndexStartName + 7);
                                }

                                else if (currentDeposit.whoItBelongsTo == game.bank.name && currentDeposit.wasItReleasedInLastTurn)
                                {
                                    currentDeposit.whoReleasedItLastTurn = fileInfoInList.ElementAt(depositIndexStartName + 3);
                                }

                                updatedDeposits.Add(currentDeposit);

                                #endregion
                            }
                        }
                    }

                    game.bank.deposits.RemoveRange(0, game.bank.deposits.Count);

                    foreach (Deposit anUpdatedDeposit in updatedDeposits)
                    {
                        game.bank.deposits.Add(anUpdatedDeposit);
                    }

                    List<Deposit> depositsWithExitPoints = new List<Deposit>();

                    foreach (Deposit depositForUpdatingExitPoints in game.bank.deposits)
                    {
                        if (depositForUpdatingExitPoints.whoItBelongsTo != game.bank.name)
                        {
                            depositsWithExitPoints.Add(depositForUpdatingExitPoints);
                        }
                    }

                    foreach (Deposit depositForUpdatingExitPoint in depositsWithExitPoints)
                    {
                        if (depositForUpdatingExitPoint.exitPointsGap == 0)
                        {
                            depositForUpdatingExitPoint.allPossibleExitPoints = null;
                        }

                        else if (depositForUpdatingExitPoint.exitPointsGap > 0)
                        {
                            DateTime currentdate = depositForUpdatingExitPoint.whenWasItBought;

                            while (currentdate < depositForUpdatingExitPoint.whenItShouldBeReleased)
                            {
                                DateTime exitPoint = new DateTime();

                                if (currentdate.Month + depositForUpdatingExitPoint.exitPointsGap > 12)
                                {
                                    int yearsAddition = (int)(currentdate.Month + depositForUpdatingExitPoint.exitPointsGap) / 12;
                                    double actualMonths = (currentdate.Month + depositForUpdatingExitPoint.exitPointsGap) - (yearsAddition * 12);

                                    exitPoint = AnyDataUpdate(actualMonths, currentdate.Year + yearsAddition);
                                }

                                else
                                {
                                    exitPoint = AnyDataUpdate(currentdate.Month + depositForUpdatingExitPoint.exitPointsGap, currentdate.Year);
                                }

                                if (((exitPoint.Month + (exitPoint.Year * 12)) <= ((depositForUpdatingExitPoint.whenItShouldBeReleased.Month)
                                    + (depositForUpdatingExitPoint.whenItShouldBeReleased.Year * 12))))
                                {
                                    depositForUpdatingExitPoint.allPossibleExitPoints.Add(exitPoint);
                                }

                                currentdate = exitPoint;
                            }
                        }
                    }

                    List<Deposit> theRestOfTheDeposits = new List<Deposit>();

                    foreach (Deposit deposit in game.bank.deposits)
                    {
                        if (deposit.whoItBelongsTo == game.bank.name)
                        {
                            theRestOfTheDeposits.Add(deposit);
                        }
                    }

                    game.bank.deposits.RemoveRange(0, game.bank.deposits.Count);

                    foreach (Deposit anotherDeposit in theRestOfTheDeposits)
                    {
                        game.bank.deposits.Add(anotherDeposit);
                    }

                    foreach (Deposit finalUpdatedDeposit in depositsWithExitPoints)
                    {
                        game.bank.deposits.Add(finalUpdatedDeposit);
                    }

                    #endregion

                    #region Creating the current player/s object/s

                    int playerStartInfoIndex = fileInfoInList.IndexOf("Player 1:");

                    string playerName = fileInfoInList.ElementAt(playerStartInfoIndex + 1);

                    Player player = new Player(game, playerName);

                    int numOfDeposits = int.Parse(fileInfoInList.ElementAt(playerStartInfoIndex + 3));

                    if (numOfDeposits > 0)
                    {
                        List<Deposit> playerUpdatedDeposits = new List<Deposit>();

                        for (int playerDepositNumber = 0; playerDepositNumber < numOfDeposits; playerDepositNumber++)
                        {
                            for (int depositNumBank = 0; depositNumBank < game.bank.deposits.Count; depositNumBank++)
                            {
                                if (fileInfoInList.ElementAt(playerStartInfoIndex + playerDepositNumber + 4) == game.bank.deposits.ElementAt(depositNumBank).name)
                                {
                                    playerUpdatedDeposits.Add(game.bank.deposits.ElementAt(depositNumBank));
                                }
                            }
                        }

                        player.depositsOwned.RemoveRange(0, player.depositsOwned.Count);
                        foreach (Deposit UpdatedDeposit in playerUpdatedDeposits)
                        {
                            player.depositsOwned.Add(UpdatedDeposit);
                        }
                    }

                    int playerDetailsIndex = fileInfoInList.IndexOf("Other player 1 details:");

                    player.InPanickMode = bool.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 1));
                    player.RowOfChoosingToDoNothing = int.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 2));
                    player.savingsAviliabe = double.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 3));
                    player.hasThePlayerFinishedHisTurn = bool.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 4));

                    Player player2 = null;

                    #region updating the 2nd player for multiplayer mode/s

                    if (mode == "player vs player")
                    {
                        int playerStartInfoIndex2 = fileInfoInList.IndexOf("Player 2:");

                        string playerName2 = fileInfoInList.ElementAt(playerStartInfoIndex2 + 1);

                        player2 = new Player(game, playerName2);

                        int numOfDeposits2 = int.Parse(fileInfoInList.ElementAt(playerStartInfoIndex2 + 3));

                        if (numOfDeposits2 > 0)
                        {
                            List<Deposit> playerUpdatedDeposits = new List<Deposit>();

                            foreach(Deposit depositInBank in game.bank.deposits)
                            {
                                if(depositInBank.whoItBelongsTo == player2.name)
                                {
                                    playerUpdatedDeposits.Add(depositInBank);
                                }
                            }

                            player2.depositsOwned.RemoveRange(0, player2.depositsOwned.Count);
                            foreach (Deposit UpdatedDeposit in playerUpdatedDeposits)
                            {
                                player2.depositsOwned.Add(UpdatedDeposit);
                            }
                        }

                        int playerDetailsIndex2 = fileInfoInList.IndexOf("Other player 2 details:");

                        player2.InPanickMode = bool.Parse(fileInfoInList.ElementAt(playerDetailsIndex2 + 1));
                        player2.RowOfChoosingToDoNothing = int.Parse(fileInfoInList.ElementAt(playerDetailsIndex2 + 2));
                        player2.savingsAviliabe = double.Parse(fileInfoInList.ElementAt(playerDetailsIndex2 + 3));
                        player2.hasThePlayerFinishedHisTurn = bool.Parse(fileInfoInList.ElementAt(playerDetailsIndex2 + 4));
                    }

                    #endregion

                    #endregion

                    #region Creating the current time trial mode

                    int gameCurrentTimeStartIndex = fileInfoInList.IndexOf("Time:");

                    int CurrentGameTimeMonth = int.Parse(fileInfoInList.ElementAt(gameCurrentTimeStartIndex + 1));
                    int CurrentGameTimeYear = int.Parse(fileInfoInList.ElementAt(gameCurrentTimeStartIndex + 2));

                    TimeTrial currentTimeTrialGame = null;
                    P1VsP2 currentPlayerVSPlayerGame = null;

                    if (mode.ToLower() == "time trial")
                    {
                        currentTimeTrialGame = new TimeTrial();
                        currentTimeTrialGame.month = CurrentGameTimeMonth;
                        currentTimeTrialGame.year = CurrentGameTimeYear;
                    }

                    if(mode.ToLower() == "player vs player")
                    {
                        currentPlayerVSPlayerGame = new P1VsP2();
                        currentPlayerVSPlayerGame.month = CurrentGameTimeMonth;
                        currentPlayerVSPlayerGame.year = CurrentGameTimeYear;
                    }

                    #endregion

                    #region And finally, brining the player to the game

                    Console.Clear();
                    Console.WriteLine($"The saved game '{saveFile.Name}' has been loaded successfully.");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to enter the saved game.");
                    Console.WriteLine();
                    Console.ReadLine();

                    if (mode.ToLower() == "time trial")
                    {
                        TimeTrial.NextTurn(game, player, currentTimeTrialGame);
                    }

                    if (mode.ToLower() == "player vs player")
                    {
                        P1VsP2.NextPlayerTurn(game, player, player2, currentPlayerVSPlayerGame);
                    }

                    #endregion

                    return;
                }
            }

            throw new Exception("Which game is in the save file?");

            #endregion
        }

        #endregion

        #endregion

        #region When unlocking a tip or enrichement for any reason

        #region Writing into the xml file

        public static void changeUnlockTipOrEnrichementInXML(XDocument doc,
            XElement root, IEnumerable<XElement> screens, XElement path,
            XElement nextScreen, XElement previousScreen, XElement unlocked,
            XElement currentScreen, string pathForFile)
        {
            List<XElement> theScreens = screens.ToList();
            unlocked.SetValue(bool.TrueString);

            currentScreen.RemoveNodes();
            currentScreen.Add(path);
            currentScreen.Add(nextScreen);
            currentScreen.Add(previousScreen);
            currentScreen.Add(unlocked);

            theScreens.RemoveAt(theScreens.IndexOf(theScreens.Find(x => x.Element("Path").Value == currentScreen.Element("Path").Value)));
            theScreens.Add(currentScreen);

            root.RemoveNodes();
            root.Add(theScreens);

            doc.ReplaceNodes(root);
            File.Delete(pathForFile);
            doc.Save(pathForFile);

            return;
        }

        #endregion

        public static void MessagesPopUpWhenAPlayerUnlocksTipOrEnrichement(string textType, string filePath, string textName)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(" == Notification ==");
            Console.WriteLine();
            Console.WriteLine($"Congrats! you unlocked a/an {textType}! And it seems to be about: '{textName}'");
            Console.WriteLine();
            Console.WriteLine($"You can now read it by going into '{textType}s' in the main meun.");
            Console.WriteLine();
            Console.WriteLine($"Do you want to read the {textType} right now?");
            Console.WriteLine();
            Console.WriteLine($"(Enter 'y' for 'yes', or 'n' for 'no')");
            Console.WriteLine();

            string input = Console.ReadLine();

            while (input.ToLower() != "y" && input.ToLower() != "n")
            {
                Console.WriteLine();
                Console.WriteLine("Enter either 'y' or 'n'. You know, like 'yes' or 'no'.");
                Console.WriteLine();
                input = Console.ReadLine();
            }

            if (input.ToLower() == "n")
            {
                return;
            }

            else if (input.ToLower() == "y")
            {
                Console.Clear();
                WritingText(filePath);
                Console.WriteLine();
                Console.WriteLine("Enter anything to continue");
                Console.WriteLine();
                Console.ReadLine();

                return;
            }
        }

        #endregion

        #region Creating your own game

        static void OpeningAnotherDetailInput()
        {
            Console.Clear();
            Console.WriteLine();
            MainMeunMessage();
        }

        static void CreatingCustomGame()
        {
            #region general game details

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Enter the game's name:");
            Console.WriteLine();
            string gameName = Console.ReadLine();

            if(ShouldIReturnToMeunByEndingFunction(gameName))
            {
                return;
            }

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Alright. Now enter the risk profile all players in this game will have:");
            Console.WriteLine();
            string riskProfile = Console.ReadLine();

            if (ShouldIReturnToMeunByEndingFunction(riskProfile))
            {
                return;
            }

            Console.WriteLine();

            double riskProfileNum;
            while(!double.TryParse(riskProfile, out riskProfileNum))
            {
                Console.WriteLine();
                Console.WriteLine("Your input for the risk profile isn't even in the correct 'format'.");
                Console.WriteLine("It needs to be a number between 0 and 1, which is the 'risk profile precentage' divided by 100.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                riskProfile = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(riskProfile))
                {
                    return;
                }
            }

            while(!(0 < double.Parse(riskProfile) && double.Parse(riskProfile) <= 1))
            {
                Console.WriteLine();
                Console.WriteLine("The risk profile must be between 0 and 1, which is the 'risk profile precentage' divided by 100.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                riskProfile = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(riskProfile))
                {
                    return;
                }
            }

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Enter the amount of money a player must have to finish the game:");
            Console.WriteLine();
            string moneyToWin = Console.ReadLine();
            if (ShouldIReturnToMeunByEndingFunction(moneyToWin))
            {
                return;
            }

            double endMoneyIsNum;
            while(!double.TryParse(moneyToWin, out endMoneyIsNum))
            {
                Console.WriteLine();
                Console.WriteLine("That's not even a number. Enter a number higher than 0 at least.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                moneyToWin = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(moneyToWin))
                {
                    return;
                }
            }

            while(double.Parse(moneyToWin) <= 0)
            {
                Console.WriteLine();
                Console.WriteLine("The amount of money required to finish cant be negative or 0.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                moneyToWin = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(moneyToWin))
                {
                    return;
                }
            }

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Enter the amount of money any player would start this game with:");
            Console.WriteLine();
            string startPlayerMoney = Console.ReadLine();
            if (ShouldIReturnToMeunByEndingFunction(startPlayerMoney))
            {
                return;
            }

            double startPlayerMoneyNum;
            while (!double.TryParse(startPlayerMoney, out startPlayerMoneyNum))
            {
                Console.WriteLine();
                Console.WriteLine("That's not even a number. Enter a number.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                startPlayerMoney = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(startPlayerMoney))
                {
                    return;
                }
            }

            while(double.Parse(startPlayerMoney) >= double.Parse(moneyToWin))
            {
                Console.WriteLine();
                Console.WriteLine("The amount a player starts this game with must be smaller");
                Console.WriteLine("than the amount neccesary to finish this game.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                startPlayerMoney = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(startPlayerMoney))
                {
                    return;
                }
            }

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Enter the income a player will receive between each turn:");
            Console.WriteLine();
            string incomePerTurn = Console.ReadLine();
            if(ShouldIReturnToMeunByEndingFunction(incomePerTurn))
            {
                return;
            }

            double incomeIsNum;
            while (!double.TryParse(incomePerTurn, out incomeIsNum))
            {
                Console.WriteLine();
                Console.WriteLine("That's not even a number. Enter a number higher than 0 at least.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                incomePerTurn = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(incomePerTurn))
                {
                    return;
                }
            }

            while (double.Parse(incomePerTurn) <= 0)
            {
                Console.WriteLine();
                Console.WriteLine("The amount of money a player receives between each turn can't be negative or 0.");
                Console.WriteLine("(Or else.. How will he ever.. HUHHHHHH!!!111)");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                incomePerTurn = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(incomePerTurn))
                {
                    return;
                }
            }

            while(double.Parse(incomePerTurn) >= double.Parse(moneyToWin))
            {
                Console.WriteLine();
                Console.WriteLine("The amount of money a player receives between each turn can't be bigger than");
                Console.WriteLine("the amount of money to finish this game.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                incomePerTurn = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(incomePerTurn))
                {
                    return;
                }
            }

            #endregion

            #region General bank details

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Now let's define the bank. Enter the bank name:");
            Console.WriteLine();
            string bankName = Console.ReadLine();
            if(ShouldIReturnToMeunByEndingFunction(bankName))
            {
                return;
            }

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Nice name LOL. Now enter the amount of money the bank has at the start of this game:");
            Console.WriteLine();
            string bankStartMoney = Console.ReadLine();
            if(ShouldIReturnToMeunByEndingFunction(bankStartMoney))
            {
                return;
            }

            double bankStartMoneyIsNum;
            while (!double.TryParse(bankStartMoney, out bankStartMoneyIsNum))
            {
                Console.WriteLine();
                Console.WriteLine("That's not even a number. Enter a number higher than 0 at least.");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                bankStartMoney = Console.ReadLine();
                if(ShouldIReturnToMeunByEndingFunction(bankStartMoney))
                {
                    return;
                }
            }

            while(((double.Parse(bankStartMoney) * 2) > double.Parse(moneyToWin)) || ((double.Parse(bankStartMoney) * 50) < double.Parse(moneyToWin)))
            {
                Console.WriteLine();
                Console.WriteLine("Sorry, but for balance, make the bank's start money between 2%-20% of the");
                Console.WriteLine($"amount of money needed to finish this game (which is {double.Parse(moneyToWin)} dollars)");
                Console.WriteLine("Enter again:");
                Console.WriteLine();
                bankStartMoney = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(bankStartMoney))
                {
                    return;
                }
            }

            #endregion

            #region Deposits details

            List<string[]> allDeposits = new List<string[]>();

            string moreDeposits;
            bool continueWithDeposits = true;

            OpeningAnotherDetailInput();

            Console.WriteLine();
            Console.WriteLine("Now let's enter the bank's deposits.");
            Console.WriteLine();

            for(int depositNum = 0; continueWithDeposits; depositNum++)
            {
                string[] depositDetails = new string[5];

                if(depositNum > 0)
                {
                    OpeningAnotherDetailInput();
                    Console.WriteLine();
                }
                Console.WriteLine($"Deposit number {(depositNum + 1).ToString()}");
                Console.WriteLine();
                Console.WriteLine("Enter the deposits name:");
                Console.WriteLine();
                string depositName = Console.ReadLine();
                if(ShouldIReturnToMeunByEndingFunction(depositName))
                {
                    return;
                }

                depositDetails[0] = depositName;

                OpeningAnotherDetailInput();

                Console.WriteLine();
                Console.WriteLine("Enter the deposit's time span:");
                Console.WriteLine();
                string depositTimeSpan = Console.ReadLine();
                if (ShouldIReturnToMeunByEndingFunction(depositTimeSpan))
                {
                    return;
                }

                int depositTimeSpanIsNum;
                while(!int.TryParse(depositTimeSpan, out depositTimeSpanIsNum))
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter a number only between 1-100 and only an integer.");
                    Console.WriteLine("Enter again:");
                    Console.WriteLine();
                    depositTimeSpan = Console.ReadLine();
                    if(ShouldIReturnToMeunByEndingFunction(depositTimeSpan))
                    {
                        return;
                    }
                }

                while (int.Parse(depositTimeSpan) < 1 || int.Parse(depositTimeSpan) > 100)
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter a number only between 1-100 and only an integer.");
                    Console.WriteLine("Enter again:");
                    Console.WriteLine();
                    depositTimeSpan = Console.ReadLine();
                    if(ShouldIReturnToMeunByEndingFunction(depositTimeSpan))
                    {
                        return;
                    }
                }

                depositDetails[1] = depositTimeSpan;

                OpeningAnotherDetailInput();

                Console.WriteLine();
                Console.WriteLine("Enter the deposit's default interest per year:");
                Console.WriteLine();
                string depositDefaultInterest = Console.ReadLine();
                if(ShouldIReturnToMeunByEndingFunction(depositDefaultInterest))
                {
                    return;
                }

                double depositDefaultInterestIsNum;
                while (!double.TryParse(depositDefaultInterest, out depositDefaultInterestIsNum))
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter a number only between 1.001-1.1 (interest can range between 0.1%-10%).");
                    Console.WriteLine("Enter again:");
                    Console.WriteLine();
                    depositDefaultInterest = Console.ReadLine();
                    if (ShouldIReturnToMeunByEndingFunction(depositDefaultInterest))
                    {
                        return;
                    }
                }

                while (double.Parse(depositDefaultInterest) < 1.001 || double.Parse(depositDefaultInterest) > 1.1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter a number only between 1.001-1.1 (interest can range between 0.1%-10%).");
                    Console.WriteLine("Enter again:");
                    Console.WriteLine();
                    depositDefaultInterest = Console.ReadLine();
                    if (ShouldIReturnToMeunByEndingFunction(depositDefaultInterest))
                    {
                        return;
                    }
                }

                depositDetails[2] = depositDefaultInterest;

                OpeningAnotherDetailInput();

                Console.WriteLine();
                Console.WriteLine("Enter the minimum money neccesary to put in the deposit in order to buy it:");
                Console.WriteLine();

                string minimumMoney = Console.ReadLine();
                if(ShouldIReturnToMeunByEndingFunction(minimumMoney))
                {
                    return;
                }

                double depositMinimumMoneyIsNum;

                while (!double.TryParse(minimumMoney, out depositMinimumMoneyIsNum))
                {
                    Console.WriteLine();
                    Console.WriteLine("That's not a number! Enter again:");
                    Console.WriteLine();
                    minimumMoney = Console.ReadLine();

                    if(ShouldIReturnToMeunByEndingFunction(minimumMoney))
                    {
                        return;
                    }
                }

                while(double.Parse(minimumMoney) >= double.Parse(moneyToWin))
                {
                    Console.WriteLine();
                    Console.WriteLine("The minimal money for a deposit can't be higher than the money needed to finish the game.");
                    Console.WriteLine("Enter again:");
                    Console.WriteLine();
                    minimumMoney = Console.ReadLine();

                    if(ShouldIReturnToMeunByEndingFunction(minimumMoney))
                    {
                        return;
                    }
                }

                depositDetails[3] = minimumMoney;

                OpeningAnotherDetailInput();

                Console.WriteLine();
                Console.WriteLine("Enter the gap of months between each exit point of this deposit (enter '0' for no exit points):");
                Console.WriteLine("(That's to decide at which dates a player owning the deposit will be accepted to release it by the bank,");
                Console.WriteLine("When he isn't at panick mode)");
                Console.WriteLine();
                string depositGapOfMonths = Console.ReadLine();

                if (ShouldIReturnToMeunByEndingFunction(depositGapOfMonths))
                {
                    return;
                }

                double depositGapOfMonthsIsNum;
                while (!double.TryParse(depositGapOfMonths, out depositGapOfMonthsIsNum))
                {
                    Console.WriteLine();
                    Console.WriteLine("That's not a number. Enter a number that is divideable by 6");
                    Console.WriteLine("Enter again:");
                    Console.WriteLine();
                    depositGapOfMonths = Console.ReadLine();
                    if (ShouldIReturnToMeunByEndingFunction(depositGapOfMonths))
                    {
                        return;
                    }
                }

                int depositGapOfMonthsDivideableBySix;
                double gapOfMonthsNum = double.Parse(depositGapOfMonths);

                if (gapOfMonthsNum != 0)
                {
                    while ((!int.TryParse((gapOfMonthsNum / 6).ToString(), out depositGapOfMonthsDivideableBySix))
                        || gapOfMonthsNum > (double.Parse(depositTimeSpan) * 12)
                        || gapOfMonthsNum < 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Enter a positive number (or 0) that is divideable by 6, and not higher than {(double.Parse(depositTimeSpan) * 12)} months");
                        Console.WriteLine("(Unless you're a nyn cat who likes the earliest exit point being after the bank will release the deposit by itself..)");
                        Console.WriteLine("Enter again:");
                        Console.WriteLine();
                        depositGapOfMonths = Console.ReadLine();

                        if (ShouldIReturnToMeunByEndingFunction(depositGapOfMonths))
                        {
                            return;
                        }

                        gapOfMonthsNum = double.Parse(depositGapOfMonths);
                    }
                }

                depositDetails[4] = depositGapOfMonths;

                OpeningAnotherDetailInput();

                allDeposits.Add(depositDetails);

                Console.WriteLine();
                Console.WriteLine("Add more deposits?");
                Console.WriteLine("(Enter 'y' for yes, or 'n' for no)");
                Console.WriteLine();
                moreDeposits = Console.ReadLine();
                if(ShouldIReturnToMeunByEndingFunction(moreDeposits))
                {
                    return;
                }

                while(moreDeposits.ToLower() != "y" && moreDeposits.ToLower() != "n")
                {
                    Console.WriteLine();
                    Console.WriteLine("That's neither 'y' nor 'n'..");
                    Console.WriteLine("Enter 'y' for yes, or 'n' for no:");
                    Console.WriteLine();
                    moreDeposits = Console.ReadLine();
                    if (ShouldIReturnToMeunByEndingFunction(moreDeposits))
                    {
                        return;
                    }
                }

                if(moreDeposits.ToLower() == "y")
                {
                    // continue the loop..
                }

                else if(moreDeposits.ToLower() == "n")
                {
                    continueWithDeposits = false;
                }
            }

            #endregion

            #region Writing the info to a new XML file and the game navigator

            // relevant variables:
            // gameName, riskProfile, moneyToWin, startPlayerMoney, incomePerTurn,
            // bankName, bankStartMoney,
            // allDeposits (with arrays that each includes: depositName, depositTimeSpan, depositDefaultInterest)

            List<string> whatToWriteToXML = new List<string>();

            whatToWriteToXML.Add("<AGame>");
            whatToWriteToXML.Add("");
            whatToWriteToXML.Add($"<Name>{gameName}</Name>");
            whatToWriteToXML.Add($"<RiskProfile>{riskProfile}</RiskProfile>");
            whatToWriteToXML.Add($"<MoneyToEnd>{moneyToWin}</MoneyToEnd>");
            whatToWriteToXML.Add($"<StartMoney>{startPlayerMoney}</StartMoney>");
            whatToWriteToXML.Add($"<Income>{incomeIsNum}</Income>");
            whatToWriteToXML.Add($"<BankName>{bankName}</BankName>");
            whatToWriteToXML.Add("");

            whatToWriteToXML.Add($"<Bank>");
            whatToWriteToXML.Add($"<StartBankMoney>{bankStartMoney}</StartBankMoney>");
            whatToWriteToXML.Add($"</Bank>");
            whatToWriteToXML.Add("");

            for(int depositPosition = 0; depositPosition < allDeposits.Count; depositPosition++)
            {
                whatToWriteToXML.Add("");
                whatToWriteToXML.Add("<Deposit>");
                whatToWriteToXML.Add("");
                whatToWriteToXML.Add($"<Name>{allDeposits.ElementAt(depositPosition)[0]}</Name>");
                whatToWriteToXML.Add($"<TimeSpan>{allDeposits.ElementAt(depositPosition)[1]}</TimeSpan>");
                whatToWriteToXML.Add($"<DefaultInterestPerYear>{allDeposits.ElementAt(depositPosition)[2]}</DefaultInterestPerYear>");
                whatToWriteToXML.Add($"<MinimumMoney>{allDeposits.ElementAt(depositPosition)[3]}</MinimumMoney>");
                whatToWriteToXML.Add($"<ExitPointsGap>{allDeposits.ElementAt(depositPosition)[4]}</ExitPointsGap>");
                whatToWriteToXML.Add("");
                whatToWriteToXML.Add("</Deposit>");
            }

            whatToWriteToXML.Add("");
            whatToWriteToXML.Add("</AGame>");

            File.WriteAllLines($@"DepositInvestingGame\Games\{gameName}.xml", whatToWriteToXML);
            XDocument theDoc = new XDocument(XDocument.Load($@"DepositInvestingGame\Games\{gameName}.xml"));

            XDocument gameNavigatorToEdit = new XDocument(XDocument.Load(@"DepositInvestingGame\GamesNavigator.xml"));
            XElement root = gameNavigatorToEdit.Root;
            IEnumerable<XElement> games = root.Elements("Game");

            XElement path = new XElement(XName.Get("Path"));
            path.SetValue($@"DepositInvestingGame\Games\{gameName}.xml");

            XElement game = new XElement(XName.Get("Game"));
            game.Add(path);

            List<XElement> gamesInList = games.ToList();
            gamesInList.Add(game);

            root.RemoveNodes();
            root.Add(gamesInList);

            gameNavigatorToEdit.RemoveNodes();
            gameNavigatorToEdit.Add(root);

            File.Delete(@"DepositInvestingGame\GamesNavigator.xml");
            gameNavigatorToEdit.Save(@"DepositInvestingGame\GamesNavigator.xml");

            #endregion

            #region Writing the info needed to the high score file

            XDocument highScoreDoc = new XDocument(XDocument.Load(@"DepositInvestingGame\HighScore.xml"));
            XElement highScoreRoot = new XElement(highScoreDoc.Root);
            IEnumerable<XElement> gamesInHighScore = new List<XElement>(highScoreRoot.Elements("Game"));

            XElement newGameInHighScore = new XElement(XName.Get("Game"));

            XElement newGameNameInHighScore = new XElement(XName.Get("GameName"));
            newGameNameInHighScore.SetValue($"{gameName}");

            XElement newGameRecordNum = new XElement(XName.Get("NumOfRecords"));
            newGameRecordNum.SetValue("0");

            XElement newRecordListForGame = new XElement(XName.Get("RecordsInGameTime"));
            newRecordListForGame.SetValue("");

            newGameInHighScore.Add(newGameNameInHighScore);
            newGameInHighScore.Add(newGameRecordNum);
            newGameInHighScore.Add(newRecordListForGame);

            List<XElement> highScoreGamesInList = gamesInHighScore.ToList();
            highScoreGamesInList.Add(newGameInHighScore);

            highScoreRoot.RemoveNodes();
            highScoreRoot.Add(highScoreGamesInList);

            highScoreDoc.RemoveNodes();
            highScoreDoc.Add(highScoreRoot);

            File.Delete(@"DepositInvestingGame\HighScore.xml");
            highScoreDoc.Save(@"DepositInvestingGame\HighScore.xml");

            #endregion

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"Your game ('{gameName}') has been saved successfully.");
            Console.WriteLine("However, to play it, you may first have to restart the game (close and re-open the its window).");
            Console.WriteLine();
            Console.WriteLine("Enter anything to return to the main meun");
            Console.WriteLine();
            Console.ReadLine();

            return;
        }

        #endregion

        #region Main Meun

        static void MainMeun(List<AGame> games)
        {
            Console.Clear();
            WritingText(@"DepositInvestingGame\MainMeun.txt");
            string input = Console.ReadLine();

            if(input == "1")
            {
                ChooseGameToPlay(games, "time trial");
            }

            if(input == "2")
            {
                ChooseGameToPlay(games, "multiplayer");
            }

            if(input == "3")
            {
                CreatingCustomGame();
            }

            else if(input == "4")
            {
                ViewHighScore(games);
            }

            else if (input == "5")
            {
                ChooseAGameToLoad();
            }

            else if(input == "6")
            {
                BookScroll manual = new BookScroll(XDocument.Load(@"DepositInvestingGame\Manual\Manual.xml"), "manual");
                manual.next("manual");
            }

            else if(input == "7")
            {
                BookScroll tips = new BookScroll(XDocument.Load(@"DepositInvestingGame\Tips\Tips.xml"), "tips");
                tips.next("tip");
            }

            else if (input == "8")
            {
                BookScroll enrichement = new BookScroll(XDocument.Load(@"DepositInvestingGame\Enrichement\Enrichement.xml"), "enrichements");
                enrichement.next("enrichement");
            }

            return;
        }

        #endregion

        #endregion

        #region Main function

        static void Main()
        {
            Console.Title = "Deposit Investing Game";
            Console.SetWindowSize(110, 50);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Gray;

            //SoundPlayer sp = new SoundPlayer();

            getTheGamesAndStart();
        }

        #endregion
    }
}
 