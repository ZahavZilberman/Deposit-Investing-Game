﻿using System;
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

        public static void PlayerNameEnterAndTimeTrialGameStart(AGame game)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Enter your player name.");
            Console.WriteLine("You can also enter 'm' to return to the main meun.");
            Console.WriteLine();
            string name = Console.ReadLine();

            if(ShouldIReturnToMeunByEndingFunction(name))
            {
                return;
            }

            bool isPlayerNameLegal = true;

            foreach(Deposit bankDeposit in game.bank.deposits)
            {
                if(bankDeposit.name.ToLower() == name.ToLower())
                {
                    isPlayerNameLegal = false;
                }
            }

            if(name.ToLower() == game.bank.name.ToLower())
            {
                isPlayerNameLegal = false;
            }

            while(!isPlayerNameLegal)
            {
                Console.WriteLine();
                Console.WriteLine("This name is illegal because something in the game is called the same. Try typing another one:");
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
            }

            Player player1 = new Player(game, name);

            Console.Clear();
            Console.WriteLine($"Welcome, '{player1.name}'!");
            Console.WriteLine();
            Console.WriteLine($"You will now be transfered to your 1st turn in this turn-based game.");
            Console.WriteLine();
            Console.WriteLine("Enter anything to start getting owned by the bank!");
            Console.WriteLine();
            Console.ReadLine();

            Console.Clear();
            TimeTrial timeTrialGame = new TimeTrial();
            TimeTrial.NextTurn(game, player1, timeTrialGame);

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

        #region writing into high score (time trial mode)

        public static void EnteringScoreIntoHighScoreInTimeTrial(AGame game, Player player, TimeTrial timeTrial)
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
                month.SetValue(timeTrial.month);

                XElement year = new XElement(XName.Get("Year"));
                year.SetValue(timeTrial.year);

                XElement playerName = new XElement(XName.Get("Player"));
                playerName.SetValue(player.name);

                XElement mode = new XElement(XName.Get("Mode"));
                mode.SetValue("Time trial");

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

        #region Viewing a game's details from time trial mode

        public static void ViewGameDetailsInTimeTrialMode(AGame Agame, List<AGame> games)
        {
            ViewGameDetailsInAllModesSoFar(Agame);

            Console.WriteLine();
            Console.WriteLine("Enter 'd' to view all the deposits this game's bank has at the start of this game,");
            Console.WriteLine("Or enter anything else to return to selecting a game to play or view its details");
            Console.WriteLine();

            string input2 = Console.ReadLine();

            if (input2.ToLower() == "d")
            {
                ViewDeposits(Agame.bank, "choosing a game to play", Agame, null, null, games);
            }

            else
            {
                ChooseGameToPlayInTimeTrial(games);
            }

            return;
        }

        #endregion

        #region Choosing a game to play (time trial mode)

        static void ChooseGameToPlayInTimeTrial(List<AGame> games)
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
                PlayerNameEnterAndTimeTrialGameStart(choosenGame);
            }

            else if (!doesThePlayerWantToPlay)
            {
                ViewGameDetailsInTimeTrialMode(choosenGame, games);
            }

            return;
        }

        #endregion

        #region Writing a bank's deposits

        public static void ViewDeposits(Bank bank, string mode, AGame game, Player player1 = null, TimeTrial timeTrial = null, List<AGame> games = null)
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

                    ViewGameDetailsInTimeTrialMode(game, games);
                }

            }

            #endregion

            #region viewing bank board in time trial mode (either to buy or view)

            if (mode.ToLower() == "bank board" || mode.ToLower() == "time trial buy deposit" || mode.ToLower() == "time trial release deposit")
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
                    if(player1.depositsOwned.Contains(currentDeposit))
                    {
                        Console.WriteLine("(You currently own this deposit)");
                        Console.WriteLine();
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

                    if (input.ToLower() == "m")
                    {
                        return;
                    }

                    for (int choosenDepositNum = 0; choosenDepositNum < bank.deposits.Count; choosenDepositNum++)
                    {
                        if (input == (choosenDepositNum + 1).ToString())
                        {
                            ViewASingleDeposit(bank, bank.deposits.ElementAt(choosenDepositNum), game, player1, timeTrial);

                            ViewDeposits(bank, "bank board", game, player1, timeTrial);
                        }
                    }

                    return;
                }
            }

            #endregion
        }

        #region Viewing a deposit

        public static void ViewASingleDeposit(Bank bank, Deposit choosenDeposit, AGame game, Player player1, TimeTrial timeTrial)
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

        public static void SavingProgress(AGame game, Player player1, Player player2 = null, TimeTrial timeTrial = null)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Enter the name of the state you want to save,");
            Console.WriteLine("Or enter 'm' to return to choose if to do an action/view info/return to the main meun");
            Console.WriteLine();
            string saveName = Console.ReadLine();

            if(saveName.ToLower() == "m" && timeTrial != null)
            {
                return;
            }

            if(timeTrial != null)
            {
                OverwritingTimeTrialSave(saveName, game, player1, timeTrial);
            }

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

            allSaveDetails = playerInfoSave(player1, allSaveDetails);

            if(player2 != null)
            {
                allSaveDetails = playerInfoSave(player2, allSaveDetails);
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

            Console.WriteLine();
            Console.WriteLine("Your state has been saved successfully.");
            Console.WriteLine();
            Console.WriteLine("Enter anything to return to choose if to act/view info/return to the main meun");
            Console.WriteLine();
            Console.ReadLine();

            return;
        }

        #region saving the player/s information specifcally

        public static List<string> playerInfoSave(Player player, List<string> allSaveDetails)
        {
            allSaveDetails.Add("Player:");
            allSaveDetails.Add($"{player.name}");
            allSaveDetails.Add($"This player's deposits number:");
            allSaveDetails.Add($"{player.depositsOwned.Count}");

            foreach (Deposit deposit in player.depositsOwned)
            {
                allSaveDetails.Add($"{deposit.name}");
            }

            allSaveDetails.Add("Other player details:");
            allSaveDetails.Add($"{player.InPanickMode.ToString()}");
            allSaveDetails.Add($"{player.RowOfChoosingToDoNothing}");
            allSaveDetails.Add($"{player.savingsAviliabe}");

            return allSaveDetails;
        }

        #endregion

        #region Asking the player if to overwrite

        public static void OverwritingTimeTrialSave(string saveName, AGame game, Player player1, TimeTrial timeTrial)
        {
            DirectoryInfo info = new DirectoryInfo($@"DepositInvestingGame\Saves\TimeTrial");
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
                        SavingProgress(game, player1, null, timeTrial);
                    }

                    if(overwrite.ToLower() == "y")
                    {
                        return;
                    }

                    else if (overwrite.ToLower() != "y" && overwrite.ToLower() != "n")
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine($"I don't get it!!!111 overwrite '{saveName}' or not?!");
                        Console.WriteLine();
                        OverwritingTimeTrialSave(saveName, game, player1, timeTrial);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Loading a game file for time trial mode

        #region choosing the mode

        public static void ChooseAGameToLoad()
        {
            List<string> modes = new List<string>();
            modes.Add("Time Trial");

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
                ChoosingAGameToLoadInTimeTrial();
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
                    }
                }

                ChoosingAGameToLoadInTimeTrial();
            }

            return;
        }

        #endregion

        #region choosing a game to load

        public static void ChoosingAGameToLoadInTimeTrial()
        {
            DirectoryInfo info = new DirectoryInfo($@"DepositInvestingGame\Saves\TimeTrial");
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
                Console.WriteLine("Write the name of the saved game file (without the numbers and dots) you want to open,");
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
                    LoadTimeTrialGame(wantedFile);
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

                    LoadTimeTrialGame(wantedFile);
                }

                return;
            }
        }

        #endregion

        #region The actual loading of a specific game

        public static void LoadTimeTrialGame(FileInfo saveFile)
        {
            Console.Clear();

            if (saveFile.Extension != ".txt")
            {
                throw new Exception("Error: Incorrect file format");
            }

            string[] fileInfo = File.ReadAllLines($@"DepositInvestingGame\Saves\TimeTrial\{saveFile.Name}");

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
                    foreach(Deposit deposit in game.bank.deposits)
                    {
                        allDeposits.Add(deposit);
                    }

                    int lineWhereDepositStart = fileInfoInList.IndexOf("All deposits:");

                    for (int depositNum = 0; depositNum < allDeposits.Count; depositNum++)
                    {
                        Deposit currentDeposit = allDeposits.ElementAt(depositNum);

                        foreach(Deposit depositAtStart in game.bank.deposits)
                        {
                            if(depositAtStart.name == currentDeposit.name)
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
                                else if(currentDeposit.whoItBelongsTo == game.bank.name && currentDeposit.wasItReleasedInLastTurn)
                                {
                                    currentDeposit.whoReleasedItLastTurn = fileInfoInList.ElementAt(depositIndexStartName + 3);
                                }

                                updatedDeposits.Add(currentDeposit);

                                #endregion
                            }
                        }
                    }

                    game.bank.deposits.RemoveRange(0, game.bank.deposits.Count);
                    
                    foreach(Deposit anUpdatedDeposit in updatedDeposits)
                    {
                        game.bank.deposits.Add(anUpdatedDeposit);
                    }

                    List<Deposit> depositsWithExitPoints = new List<Deposit>();

                    foreach (Deposit depositForUpdatingExitPoints in game.bank.deposits)
                    {
                        if(depositForUpdatingExitPoints.whoItBelongsTo != game.bank.name)
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

                    foreach(Deposit deposit in game.bank.deposits)
                    {
                        if(deposit.whoItBelongsTo == game.bank.name)
                        {
                            theRestOfTheDeposits.Add(deposit);
                        }
                    }

                    game.bank.deposits.RemoveRange(0, game.bank.deposits.Count);

                    foreach(Deposit anotherDeposit in theRestOfTheDeposits)
                    {
                        game.bank.deposits.Add(anotherDeposit);
                    }

                    foreach(Deposit finalUpdatedDeposit in depositsWithExitPoints)
                    {
                        game.bank.deposits.Add(finalUpdatedDeposit);
                    }

                    #endregion

                    #region Creating the current player object

                    int playerStartInfoIndex = fileInfoInList.IndexOf("Player:");

                    string playerName = fileInfoInList.ElementAt(playerStartInfoIndex + 1);

                    Player player = new Player(game, playerName);

                    int numOfDeposits = int.Parse(fileInfoInList.ElementAt(playerStartInfoIndex + 3));

                    if(numOfDeposits > 0)
                    {
                        List<Deposit> playerUpdatedDeposits = new List<Deposit>();

                        for(int playerDepositNumber = 0; playerDepositNumber < numOfDeposits; playerDepositNumber++)
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

                    int playerDetailsIndex = fileInfoInList.IndexOf("Other player details:");

                    player.InPanickMode = bool.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 1));
                    player.RowOfChoosingToDoNothing = int.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 2));
                    player.savingsAviliabe = double.Parse(fileInfoInList.ElementAt(playerDetailsIndex + 3));

                    #endregion

                    #region Creating the current time trial mode

                    int gameCurrentTimeStartIndex = fileInfoInList.IndexOf("Time:");

                    int CurrentGameTimeMonth = int.Parse(fileInfoInList.ElementAt(gameCurrentTimeStartIndex + 1));
                    int CurrentGameTimeYear = int.Parse(fileInfoInList.ElementAt(gameCurrentTimeStartIndex + 2));

                    TimeTrial currentTimeTrialGame = new TimeTrial();
                    currentTimeTrialGame.month = CurrentGameTimeMonth;
                    currentTimeTrialGame.year = CurrentGameTimeYear;

                    #endregion

                    #region And finally, brining the player to the game

                    Console.Clear();
                    Console.WriteLine($"The saved game '{saveFile.Name}' has been loaded successfully.");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to enter the saved game.");
                    Console.WriteLine();
                    Console.ReadLine();

                    TimeTrial.NextTurn(game, player, currentTimeTrialGame);

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
            Console.WriteLine($"Congrats! you unlocked a {textType}! And it seems to be about: '{textName}'");
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
                ChooseGameToPlayInTimeTrial(games);
            }

            if(input == "2")
            {
                CreatingCustomGame();
            }

            else if(input == "3")
            {
                ViewHighScore(games);
            }

            else if (input == "4")
            {
                ChooseAGameToLoad();
            }

            else if(input == "5")
            {
                BookScroll manual = new BookScroll(XDocument.Load(@"DepositInvestingGame\Manual\Manual.xml"));
                manual.next("manual");
            }

            else if(input == "6")
            {
                BookScroll tips = new BookScroll(XDocument.Load(@"DepositInvestingGame\Tips\Tips.xml"));
                tips.next("tip");
            }

            else if (input == "7")
            {
                BookScroll enrichement = new BookScroll(XDocument.Load(@"DepositInvestingGame\Enrichement\Enrichement.xml"));
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