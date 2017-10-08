using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

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
            ReturnToMainMeun(name);

            Player player1 = new Player(game, name);

            Console.Clear();
            Console.WriteLine($"Welcome, '{player1.name}'!");
            Console.WriteLine();
            Console.WriteLine($"You will now be transfered to your 1st turn in this turn-based game.");
            Console.WriteLine();
            Console.WriteLine("Enter anything to start playing!");
            Console.WriteLine();
            Console.ReadLine();

            Console.Clear();
            TimeTrial timeTrialGame = new TimeTrial();
            TimeTrial.NextTurn(game, player1, timeTrialGame);
        }

        #endregion

        #region Return to main meun

        public static void MainMeunMessage()
        {
            Console.WriteLine("Enter 'm' to return to the main meun.");
            Console.WriteLine();
        }

        public static void ReturnToMainMeun(string input)
        {
            if(input.ToLower() == "m")
            {
                getTheGamesAndStart();
            }
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
            XDocument DataBase = new XDocument(XDocument.Load(@"DepositInvestingGame\HighScore.xml"));

            XElement root = DataBase.Root;

            List<XElement> games = new List<XElement>(root.Elements("Game"));

            foreach (XElement Agame in games)
            {
                if (Agame.Element("GameName").Value.ToLower() == game.name.ToLower())
                {
                    XElement GameName = new XElement(Agame.Element("GameName"));
                    XElement NumOfRecords = new XElement(Agame.Element("NumOfRecords"));
                    XElement Records = new XElement(Agame.Element("RecordsInGameTime"));
                    //List<XElement> records = new List<XElement>(Record.Elements("Record"));

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

                    Agame.RemoveNodes();
                    Agame.Add(GameName);
                    Agame.Add(NumOfRecords);
                    Agame.Add(Records);

                    games.RemoveAt(games.IndexOf(Agame));
                    games.Add(Agame);

                    root.RemoveNodes();
                    root.Add(games);

                    DataBase.ReplaceNodes(root);
                    File.Delete(@"DepositInvestingGame\HighScore.xml");
                    DataBase.Save(@"DepositInvestingGame\HighScore.xml");

                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("Your score has been sumbmitted.");
                    Console.WriteLine();
                    Console.WriteLine("You can view it by going into the 'high score' chart of this game via the main meun.");
                    Console.WriteLine();

                    return;
                }
            }
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
            Console.WriteLine($"This game's default bank has the name: {Agame.bank.name}");
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
            Console.WriteLine("Or enter 'return' to return to selecting a game to play or view its details,");
            Console.WriteLine();

            string input2 = Console.ReadLine();

            if (input2.ToLower() == "d")
            {
                ViewDeposits(Agame.bank, "choosing a game to play", Agame, null, null, games);
            }
            else if(input2.ToLower() == "return")
            {
                ChooseGameToPlayInTimeTrial(games);
            }
            else
            {
                ViewGameDetailsInTimeTrialMode(Agame, games);
            }
        }

        #endregion

        #region Choosing a game to play (time trial mode)

        static void ChooseGameToPlayInTimeTrial(List<AGame> games)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("The following are a list of all the games, each of the mentioned with his name.");
            Console.WriteLine("Enter the name of the game you want to play,");
            Console.WriteLine("Or enter 'm' to return to the main meun:");
            Console.WriteLine();
            Console.WriteLine();

            for (int gameNum = 0; gameNum < games.Count; gameNum++)
            {
                Console.WriteLine();
                AGame currentGame = games.ElementAt(gameNum);
                Console.WriteLine($"{gameNum + 1}. {currentGame.name}");
                Console.WriteLine();
                Console.WriteLine($"(Enter '{currentGame.name} info' [without the quote marks] to view information about this game)");
                Console.WriteLine();
            }

            string input = Console.ReadLine();

            ReturnToMainMeun(input);

            foreach (AGame Agame in games)
            {
                if (input.ToLower() == Agame.name.ToLower())
                {
                    #region when the player has choosen which game to play

                    PlayerNameEnterAndTimeTrialGameStart(Agame);

                    #endregion
                }

                else if(input.ToLower() == $"{Agame.name.ToLower()} info")
                {
                    ViewGameDetailsInTimeTrialMode(Agame, games);
                }

                else
                {
                    ChooseGameToPlayInTimeTrial(games);
                }
            }
        }

        #endregion

        #region Writing a bank's deposits (bank board)

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
                }

                Console.WriteLine();
                Console.WriteLine();

                if (mode.ToLower() == "high score")
                {
                    Console.WriteLine($"Enter anything to return to view the records for the game called '{game.name}'.");
                    Console.WriteLine();
                    Console.ReadLine();
                    HighScore highScoreAgain = new HighScore(game);
                    highScoreAgain.next(game);
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
                    Console.WriteLine("Enter the name of a deposit to view all its current details,");
                    Console.WriteLine("Or enter 'm' to return to choosing if to act/save/view other info/return to the main meun");
                    Console.WriteLine();
                    string input = Console.ReadLine();

                    if (input.ToLower() == "m")
                    {
                        TimeTrial.NextTurn(game, player1, timeTrial);
                    }

                    for (int choosenDepositNum = 0; choosenDepositNum < bank.deposits.Count; choosenDepositNum++)
                    {
                        if (input.ToLower() == bank.deposits.ElementAt(choosenDepositNum).name.ToLower())
                        {
                            ViewASingleDeposit(bank, bank.deposits.ElementAt(choosenDepositNum), game, player1, timeTrial);
                        }
                    }

                    ViewDeposits(game.bank, "bank board", game, player1, timeTrial);
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

            ViewDeposits(bank, "bank board", game, player1, timeTrial);

        }

        #endregion

        #endregion

        #region function that gets the games and calls the main meun

        static void getTheGamesAndStart()
        {
            List<AGame> games = new List<AGame>();
            games = WritingAllGamesInformation();

            MainMeun(games);
        }

        #endregion

        #region function that "translates" all games into objects

        public static List<AGame> WritingAllGamesInformation()
        {
            List<AGame> games = new List<AGame>();

            #region First Game

            List<Deposit> deposits = new List<Deposit>();

            string bankName = "Hamya";

            Deposit flower = new Deposit("Flower", 1, bankName, 1.02);
            deposits.Add(flower);

            Deposit bear = new Deposit("Bear", 2, bankName, 1.025);
            deposits.Add(bear);

            Deposit Kong = new Deposit("Kong", 3, bankName, 1.03);
            deposits.Add(Kong);

            Deposit Monkey = new Deposit("Monkey", 4, bankName, 1.035);
            deposits.Add(Monkey);

            Deposit Lizard = new Deposit("Lizard", 5, bankName, 1.04);
            deposits.Add(Lizard);

            Deposit Elephant = new Deposit("Elephant", 6, bankName, 1.045);
            deposits.Add(Elephant);

            Deposit Zebra = new Deposit("Zebra", 7, bankName, 1.05);
            deposits.Add(Zebra);

            Deposit Lion = new Deposit("Lion", 8, bankName, 1.055);
            deposits.Add(Lion);

            Deposit Crocodile = new Deposit("Crocodile", 9, bankName, 1.06);
            deposits.Add(Crocodile);

            Bank hamaya = new Bank(deposits);
            hamaya.name = bankName;

            double moneyToEndGame = 20000;

            hamaya.startGameMoney = moneyToEndGame / 20;
            hamaya.money = moneyToEndGame / 20;

            AGame firstGame = new AGame(0.6, moneyToEndGame, hamaya, 0, "First Game", 1000);

            games.Add(firstGame);

            #endregion

            return games;
        }

        #endregion

        #region returning bank to its default

        public static Bank ReturnBankToDefault(List<AGame> games, string gameName)
        {
            #region First game

            foreach(AGame game in games)
            {
                if(game.name.ToLower() == gameName.ToLower())
                {
                    return game.bank;
                }
            }

            #endregion

            throw new Exception("Game not found");
        }

        #endregion

        #region viewing high score

        static void ViewHighScore(List<AGame> games)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("The following are a list of all the games, each of the mentioned with his name.");
            Console.WriteLine("Enter the name of the game you want to view his high score table.");
            Console.WriteLine();

            for (int gameNum = 0; gameNum < games.Count; gameNum++)
            {
                Console.WriteLine($"{gameNum+1}. {games.ElementAt(gameNum).name}");
                Console.WriteLine();
            }

            string input = Console.ReadLine();

            ReturnToMainMeun(input);

            #region checking user input and then opening the high score

            foreach (AGame Agame in games)
            {
                if(input.ToLower() == Agame.name.ToLower())
                {
                    HighScore highScore = new HighScore(Agame);
                    highScore.next(Agame);
                }
            }

            #endregion

            ViewHighScore(games);
        }

        #endregion

        #region Saving game

        public static void SavingProgress(AGame game, Player player1, Player player2 = null, TimeTrial timeTrial = null)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Enter the name of the state you want to save,");
            Console.WriteLine("Or enter 'm' to return to the main meun");
            Console.WriteLine();
            string saveName = Console.ReadLine();

            ReturnToMainMeun(saveName);

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

                Console.WriteLine();
                Console.WriteLine("Your state has been saved successfully.");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return to choose if to act/view info/return to the main meun");
                Console.WriteLine();
                Console.ReadLine();

                TimeTrial.NextTurn(game, player1, timeTrial);
            }
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
                if (saveName == $"{file.Name.Remove(file.Name.Count() - 4, 4)}")
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
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Enter the name of the mode the saved game belongs to,");
            Console.WriteLine("Or enter 'm' to return to the main meun: ");
            Console.WriteLine();
            Console.WriteLine("Time Trial");
            Console.WriteLine();
            string input = Console.ReadLine();

            ReturnToMainMeun(input);

            if(input.ToLower() == "Time Trial".ToLower())
            {
                ChoosingAGameToLoadInTimeTrial();
            }

            else
            {
                ChooseAGameToLoad();
            }
            
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
                Console.WriteLine("There no saved files for this mode.");
                Console.WriteLine("Enter anything to return to the main meun.");
                Console.WriteLine();
                Console.ReadLine();

                ReturnToMainMeun("m");
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

                ReturnToMainMeun(wantedSave);

                foreach (FileInfo file in files)
                {
                    string actualFileName = $"{file.Name}".Remove((file.Name.Length - 4), 4);

                    if (wantedSave.ToLower() == actualFileName.ToLower())
                    {
                        LoadTimeTrialGame(file);
                    }
                }

                ChoosingAGameToLoadInTimeTrial();
            }
        }

        #endregion

        #region The actual loading of a specific game (done, but maybe not tested enough)

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
            AGame gameToLoad;

            string gameName = fileInfo.ElementAt(1);

            foreach(AGame game in allGames)
            {
                if (game.name == gameName)
                {
                    gameToLoad = new AGame(game.riskProfile, game.moneyToEndGame, game.bank, game.moneyToStartWith, game.name, game.playersIncome);

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

                    TimeTrial.NextTurn(game, player, currentTimeTrialGame);

                    #endregion
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

            theScreens.RemoveAt(theScreens.IndexOf(currentScreen));
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
            Console.WriteLine($"(Enter 'y' to read it, or 'n' to skip reading it)");
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

        #region Main Meun

        static void MainMeun(List<AGame> games)
        {
            Console.Clear();
            WritingText(@"DepositInvestingGame\MainMeun.txt");
            string input = Console.ReadLine();

            if(input.ToLower() == "time trial")
            {
                ChooseGameToPlayInTimeTrial(games);
            }

            else if(input.ToLower() == "high score")
            {
                ViewHighScore(games);
            }

            else if (input.ToLower() == "load game")
            {
                ChooseAGameToLoad();
            }

            else if(input.ToLower() == "manual")
            {
                BookScroll manual = new BookScroll(XDocument.Load(@"DepositInvestingGame\Manual\Manual.xml"));
                manual.next("manual");
            }

            else if(input.ToLower() == "tips")
            {
                BookScroll tips = new BookScroll(XDocument.Load(@"DepositInvestingGame\Tips\Tips.xml"));
                tips.next("tip");
            }

            else if (input.ToLower() == "enrichement")
            {
                BookScroll enrichement = new BookScroll(XDocument.Load(@"DepositInvestingGame\Enrichement\Enrichement.xml"));
                enrichement.next("enrichement");
            }

            else
            {
                Console.Clear();
                MainMeun(games);
            }
        }

        #endregion

        #endregion

        #region Main function

        public static void Main()
        {
            Console.Title = "Deposit Investing Game";
            Console.SetWindowSize(110, 50);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Gray;

            getTheGamesAndStart();
        }

        #endregion
    }
}