using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class P1VsP2 : Program
    {
        #region ctor

        public P1VsP2()
        {
            month = 1;
            year = 1;
        }

        #endregion

        #region deciding which player turn it is now

        public static Player WhichPlayerTurnIsIt(Player player1,
            Player player2)
        {
            if (player1.hasThePlayerFinishedHisTurn
                && !player2.hasThePlayerFinishedHisTurn)
            {
                return player2;
            }

            else if (!player1.hasThePlayerFinishedHisTurn
                && !player2.hasThePlayerFinishedHisTurn)
            {
                return player1;
            }

            else if (player1.hasThePlayerFinishedHisTurn
                && player2.hasThePlayerFinishedHisTurn)
            {
                return player1;
            }

            throw new Exception("WTF?");
        }

        #endregion

        #region the 4 main choices

        public static void NextPlayerTurn(AGame game, Player player1,
            Player player2, P1VsP2 multiplayer)
        {
            Player currentPlayer = WhichPlayerTurnIsIt(player1, player2);

            if (currentPlayer.InPanickMode)
            {
                PanickModeStateNotification(true, currentPlayer);

                PanickModeTurn(player1, player2, currentPlayer, game,
                    multiplayer);

                return;
            }

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" == It's the turn of '{currentPlayer.name}' ==");
            Console.WriteLine();
            Console.WriteLine("In-game date:");
            Console.WriteLine($"year {multiplayer.year}, month {multiplayer.month}.");
            Console.WriteLine();
            Console.WriteLine($"Sum of money that this player has uninvested: {currentPlayer.savingsAviliabe} dollars");
            Console.WriteLine();
            Console.WriteLine($"Hey '{currentPlayer.name}', enter the number of your choice out of the following:");
            Console.WriteLine();
            Console.WriteLine("1. Make an action (your move at this turn).");
            Console.WriteLine();
            Console.WriteLine("2. View the following:");
            Console.WriteLine("the details of the game, the bank board and your player status in this game.");
            Console.WriteLine();
            Console.WriteLine("3. Save your state in the current game.");
            Console.WriteLine();
            Console.WriteLine("You also may enter 'm' to return to the main meun without saving (any unsaved data will be lost)");
            Console.WriteLine();

            string input = Console.ReadLine();

            if (ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            while (input != "1" && input != "2" && input != "3")
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again:");
                Console.WriteLine();
                input = Console.ReadLine();

                if (ShouldIReturnToMeunByEndingFunction(input))
                {
                    return;
                }
            }

            if (input.ToLower() == "1")
            {
                ChoosingAnAction(game, player1, player2, currentPlayer, multiplayer);
            }

            else if (input.ToLower() == "2")
            {
                ViewingAllInfo(currentPlayer, game);

                NextPlayerTurn(game, player1, player2, multiplayer);
            }

            else if (input.ToLower() == "3")
            {
                SavingProgress(game, player1, player2, null, multiplayer);

                NextPlayerTurn(game, player1, player2, multiplayer);
            }

            return;
        }

        #endregion

        #region Choosing an action

        public static void ChoosingAnAction(AGame game, Player player1, Player player2,
            Player currentPlayer, P1VsP2 multiplayer)
        {
            Console.Clear();

            WritingWhichPlayerTurnItIs(currentPlayer);

            WritingActionChoices();

            string actionChoosen = Console.ReadLine();

            while (actionChoosen != "1" && actionChoosen != "2" && actionChoosen != "3" && actionChoosen.ToLower() != "m")
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again:");
                Console.WriteLine();
                actionChoosen = Console.ReadLine();
            }

            if (ShouldIReturnToMeunByEndingFunction(actionChoosen))
            {
                NextPlayerTurn(game, player1, player2, multiplayer);
            }

            else if (actionChoosen == "1")
            {
                if (currentPlayer.savingsAviliabe == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Trying to be a smartass huh? Investing without having any money? Well then.. *charges attack*");
                    Console.WriteLine();
                    Console.WriteLine("Just kidding, enter anything to return to choose an action.");
                    Console.WriteLine();
                    Console.ReadLine();

                    ChoosingAnAction(game, player1, player2, currentPlayer, multiplayer);

                    return;
                }

                BuyingADeposit(game, player1, player2, currentPlayer, multiplayer);
            }

            else if (actionChoosen == "2")
            {
                ChooseADepositToRelease(game, player1, player2, currentPlayer,
                    multiplayer);
            }

            else if (actionChoosen == "3")
            {
                DoingNothing(game, player1, player2, currentPlayer, multiplayer);
            }

            return;
        }

        #endregion

        #region Doing nothing

        public static void DoingNothing(AGame game, Player player1, Player player2,
            Player currentPlayer, P1VsP2 multiplayer)
        {
            #region How many times the player choose this?!

            currentPlayer.RowOfChoosingToDoNothing = currentPlayer.RowOfChoosingToDoNothing + 1;

            if (currentPlayer.RowOfChoosingToDoNothing > 3 && game.bank.numOfDepositsAviliabe > 0)
            {
                Console.WriteLine();
                Console.WriteLine("you've choosed doing nothing several times in a row.. You can't do nothing at this turn.");
                Console.WriteLine("(BTW, I really wonder if the richest people in the world spent most of their life doing nothing?)");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return to choose an action, either to ask to release a deposit or buy one.");
                Console.WriteLine();

                currentPlayer.RowOfChoosingToDoNothing = currentPlayer.RowOfChoosingToDoNothing - 1;

                string nonNothingInput = Console.ReadLine();
                ChoosingAnAction(game, player1, player2, currentPlayer, multiplayer);

                return;
            }

            #endregion

            #region If not too much, then here we go to the next turn after update!

            Console.Clear();

            WritingWhichPlayerTurnItIs(currentPlayer);

            Console.WriteLine();
            Console.WriteLine("You successfully sit on your ass for 6 months not even calling the bank,");
            Console.WriteLine("while proably singing 'the lazy song' or 'gangam style'..");
            Console.WriteLine();
            Console.WriteLine($"Enter anything to continue to the next player's turn");
            Console.WriteLine();
            Console.ReadLine();

            UpdateDataForNextTurn(game, player1, player2, currentPlayer, multiplayer);

            return;

            #endregion
        }

        #endregion

        #region Buying a deposit

        #region Choosing a deposit

        public static void BuyingADeposit(AGame game, Player player1, Player player2,
            Player currentPlayer, P1VsP2 multiplayer)
        {
            ViewDeposits(game.bank, "multiplayer buy deposit", game, player1,
                null, null, player2, multiplayer, currentPlayer);

            WritingTextForChoosingADeposit();

            string input = Console.ReadLine();
            bool doesInputMatchDeposit = false;
            int depositNumIfExists = (-1);

            for (int depositNum = 0; depositNum < game.bank.deposits.Count; depositNum++)
            {
                if (input == $"{(depositNum + 1).ToString()}")
                {
                    doesInputMatchDeposit = true;
                    depositNumIfExists = depositNum;
                }
            }

            while (input.ToLower() != "m" && input.ToLower() != "v" && !doesInputMatchDeposit)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again: ");
                Console.WriteLine();
                input = Console.ReadLine();

                for (int depositNum = 0; depositNum < game.bank.deposits.Count; depositNum++)
                {
                    if (input == $"{(depositNum + 1).ToString()}")
                    {
                        doesInputMatchDeposit = true;
                        depositNumIfExists = depositNum;
                    }
                }
            }

            if (input.ToLower() == "m")
            {
                NextPlayerTurn(game, player1, player2, multiplayer);

                return;
            }

            if (input.ToLower() == "v")
            {
                ViewDeposits(game.bank, "bank board", game, player1,
                    null, null, player2, multiplayer, currentPlayer);

                NextPlayerTurn(game, player1, player2, multiplayer);

                return;
            }

            if (doesInputMatchDeposit)
            {
                if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo == currentPlayer.name
                    && game.bank.deposits.ElementAt(depositNumIfExists).amountOfMoneyPutInDeposit > 0)
                {
                    WritingYouAlreadyOwnTheDeposit();

                    BuyingADeposit(game, player1, player2, currentPlayer, multiplayer);
                }

                else if ((game.bank.deposits.ElementAt(depositNumIfExists).minimumMoneyForDeposit / game.riskProfile) > currentPlayer.savingsAviliabe)
                {
                    WritingYouDontHaveEnoughMoneyForDeposit(game, depositNumIfExists);

                    BuyingADeposit(game, player1, player2, currentPlayer, multiplayer);
                }

                else if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo != game.bank.name
                    && game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo != currentPlayer.name)
                {
                    if (currentPlayer.name == player1.name)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"The other player, '{player2.name}', owns the deposit you choosed ('{game.bank.deposits.ElementAt(depositNumIfExists).name}')");
                    }

                    else if (currentPlayer.name == player2.name)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"The other player, '{player1.name}', owns the deposit you choosed ('{game.bank.deposits.ElementAt(depositNumIfExists).name}')");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Enter anything to return to choosing a deposit to buy.");
                    Console.WriteLine();
                    Console.ReadLine();

                    BuyingADeposit(game, player1, player2, currentPlayer, multiplayer);
                }

                else if ((game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo == game.bank.name))
                {
                    PuttingMoneyIntoChoosenDeposit(game, game.bank.deposits.ElementAt(depositNumIfExists),
                        player1, player2, currentPlayer, multiplayer);
                }

                // there's a clash between players in this mode
            }

            return;
        }

        #endregion

        #region Putting money into a deposit (warnings: double code everywhere)

        public static void PuttingMoneyIntoChoosenDeposit(AGame game, Deposit choosenDeposit,
            Player player1, Player player2, Player currentPlayer, P1VsP2 multiplayer)
        {
            bool isMinimumMoneyRiskProfileBased = (((game.riskProfile / 10) * currentPlayer.savingsAviliabe) >= choosenDeposit.minimumMoneyForDeposit);
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"Looks like the deoposit '{choosenDeposit.name}' is available!");
            Console.WriteLine();
            Console.WriteLine($"Seeing as your risk profile is {game.riskProfile * 100}%,");

            if (!isMinimumMoneyRiskProfileBased)
            {
                Console.WriteLine($"the amount of money you can put in is between {choosenDeposit.minimumMoneyForDeposit} (minimum money for deposit) and {game.riskProfile * 100}% of your savings,");
            }

            else
            {
                Console.WriteLine($"the amount of money you can put in is between {game.riskProfile * 10}% and {game.riskProfile * 100}% of your savings,");
            }

            Console.WriteLine($"And that's out of the {currentPlayer.savingsAviliabe} dollars you have uninvested right now!");
            Console.WriteLine();
            Console.WriteLine("Enter the amount of money you want to put in this deposit,");
            Console.WriteLine("Or enter 'm' to return choosing a deposit to buy.");
            Console.WriteLine();
            Console.WriteLine($"Note: if the game doesn't allow you to put {game.riskProfile * 100}% of your money, it's a bug - try to put 1 dollar less than that)");
            Console.WriteLine();

            string moneyInputForDeposit = Console.ReadLine();

            if (moneyInputForDeposit.ToLower() == "m")
            {
                BuyingADeposit(game, player1, player2, currentPlayer, multiplayer);

                return;
            }

            #region Checking the player input for the amount (סינון)

            double moneyForDeposit;

            if (!double.TryParse(moneyInputForDeposit, out moneyForDeposit))
            {
                Console.WriteLine();
                Console.WriteLine("That's neither a number nor 'm'!");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return choosing the amount of money to put in this deposit,");
                Console.WriteLine("And input either a number or 'm' next time.");
                Console.WriteLine();
                Console.ReadLine();

                PuttingMoneyIntoChoosenDeposit(game, choosenDeposit, player1, player2,
                    currentPlayer, multiplayer);

                return;
            }

            else if ((moneyForDeposit < (currentPlayer.savingsAviliabe * (game.riskProfile / 10))) && isMinimumMoneyRiskProfileBased)
            {
                Console.WriteLine();
                Console.WriteLine("This amount of money is less than 1/10 your risk profile allows you to put.");
                Console.WriteLine("You can't take such tiny risks in this game, it's basically the same as doing nothing.");
                Console.WriteLine("Maybe you'd better off doing nothing at this turn.");
                Console.WriteLine();
                Console.WriteLine("Enter 'd' to choose to do nothing at this turn instead of buying any deposit,");
                Console.WriteLine("Or enter anything else to return choosing the amount of money to put in this deposit");
                Console.WriteLine();
                string input = Console.ReadLine();

                if (input.ToLower() == "d")
                {
                    DoingNothing(game, player1, player2, currentPlayer, multiplayer);
                }

                else
                {
                    PuttingMoneyIntoChoosenDeposit(game, choosenDeposit, player1, player2,
                    currentPlayer, multiplayer);
                }

                return;
            }

            else if ((moneyForDeposit < choosenDeposit.minimumMoneyForDeposit) && !isMinimumMoneyRiskProfileBased)
            {
                Console.WriteLine();
                Console.WriteLine($"Can't allow this.. You must put at least {choosenDeposit.minimumMoneyForDeposit} dollars to get away with it.");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return choosing the amount of money to put in this deposit");
                Console.WriteLine();
                Console.ReadLine();

                PuttingMoneyIntoChoosenDeposit(game, choosenDeposit, player1, player2,
                                    currentPlayer, multiplayer);
                return;
            }

            else if (moneyForDeposit > (currentPlayer.savingsAviliabe * game.riskProfile))
            {
                Console.WriteLine();
                Console.WriteLine("This amount of money is higher than your risk profile allows you to put.");
                Console.WriteLine("I would've had to make you go into panick mode if I weren't such a nice game developer *_*");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return choosing the amount of money to put in this deposit");
                Console.WriteLine();
                Console.ReadLine();

                PuttingMoneyIntoChoosenDeposit(game, choosenDeposit, player1, player2,
                                    currentPlayer, multiplayer);
                return;
            }

            #endregion

            UpdatingAfterBuyingDeposit(game, player1, player2, currentPlayer,
                multiplayer, choosenDeposit, moneyForDeposit);

            return;
        }

        #endregion

        #region Updating after buying a deposit (warnings: double code)

        public static void UpdatingAfterBuyingDeposit(AGame game, Player player1,
            Player player2, Player currentPlayer, P1VsP2 multiplayer,
            Deposit choosenDeposit, double amountOfMoneyForDeposit)
        {
            choosenDeposit.whoItBelongsTo = currentPlayer.name;
            choosenDeposit.wasItReleasedInLastTurn = false;
            choosenDeposit.amountOfMoneyPutInDeposit = amountOfMoneyForDeposit;

            if (currentPlayer.name == player1.name)
            {
                player1.savingsAviliabe = player1.savingsAviliabe - amountOfMoneyForDeposit;
            }

            else if (currentPlayer.name == player2.name)
            {
                player2.savingsAviliabe = player2.savingsAviliabe - amountOfMoneyForDeposit;
            }

            DateTime buyingTime = new DateTime();

            buyingTime = AnyDataUpdate(multiplayer.month, multiplayer.year);
            choosenDeposit.whenWasItBought = buyingTime;

            DateTime releaseTime = new DateTime();

            double depositTimeSpan = choosenDeposit.timeSpan;

            double YearOfDepositRelease = buyingTime.Year + depositTimeSpan;

            releaseTime = AnyDataUpdate(multiplayer.month, YearOfDepositRelease);
            choosenDeposit.whenItShouldBeReleased = releaseTime;

            double defaultInterest = choosenDeposit.DefaultinterestPerYear;
            double actualInterest = 0;

            if (amountOfMoneyForDeposit < game.moneyToEndGame / 10)
            {
                actualInterest = choosenDeposit.DefaultinterestPerYear;
            }

            else if (amountOfMoneyForDeposit >= (game.moneyToEndGame / 10) && amountOfMoneyForDeposit < (game.moneyToEndGame / 4))
            {
                actualInterest = Math.Pow(choosenDeposit.DefaultinterestPerYear, 1.5);
            }

            else if (amountOfMoneyForDeposit >= (game.moneyToEndGame / 4))
            {
                actualInterest = Math.Pow(choosenDeposit.DefaultinterestPerYear, 2);
            }

            choosenDeposit.actualInterestPerYear = actualInterest;

            #region And as for the exit points..

            if (choosenDeposit.exitPointsGap == 0)
            {
                choosenDeposit.allPossibleExitPoints = null;
            }

            else
            {
                DateTime currentdate = choosenDeposit.whenWasItBought;

                while (currentdate < choosenDeposit.whenItShouldBeReleased)
                {
                    DateTime exitPoint = new DateTime();

                    if (currentdate.Month + choosenDeposit.exitPointsGap > 12)
                    {
                        int yearsAddition = (int)(currentdate.Month + choosenDeposit.exitPointsGap) / 12;
                        double actualMonths = (currentdate.Month + choosenDeposit.exitPointsGap) - (yearsAddition * 12);

                        exitPoint = AnyDataUpdate(actualMonths, currentdate.Year + yearsAddition);
                    }

                    else
                    {
                        exitPoint = AnyDataUpdate(currentdate.Month + choosenDeposit.exitPointsGap, currentdate.Year);
                    }

                    if (((exitPoint.Month + (exitPoint.Year * 12)) <= ((choosenDeposit.whenItShouldBeReleased.Month)
                        + (choosenDeposit.whenItShouldBeReleased.Year * 12))))
                    {
                        choosenDeposit.allPossibleExitPoints.Add(exitPoint);
                    }

                    currentdate = exitPoint;
                }
            }

            #endregion

            #region just to make sure the deposit and bank will be updated in the game object

            Deposit boughtDeposit = null;

            foreach (Deposit deposit in game.bank.deposits)
            {
                if (deposit.name == choosenDeposit.name)
                {
                    boughtDeposit = deposit;
                }
            }

            game.bank.deposits.Remove(boughtDeposit);
            game.bank.deposits.Add(choosenDeposit);

            game.bank.numOfDepositsAviliabe = game.bank.numOfDepositsAviliabe - 1;

            #endregion

            if (currentPlayer.name == player1.name)
            {
                player1.depositsOwned.Add(choosenDeposit);
                player1.RowOfChoosingToDoNothing = 0;
            }

            if (currentPlayer.name == player2.name)
            {
                player2.depositsOwned.Add(choosenDeposit);
                player2.RowOfChoosingToDoNothing = 0;
            }

            #region And when the update is done..

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"The deposit '{choosenDeposit.name}' has been locked for {currentPlayer.name}, for {choosenDeposit.timeSpan} years, with {choosenDeposit.amountOfMoneyPutInDeposit} dollars in it (..)");
            Console.WriteLine();
            Console.WriteLine("Enter anything to go on to the next turn");
            Console.WriteLine();
            Console.ReadLine();

            UpdateDataForNextTurn(game, player1, player2, currentPlayer,
                multiplayer);

            return;

            #endregion
        }

        #endregion

        #endregion

        #region Asking to release a deposit

        #region Choosing a deposit to release (tons of double code)

        public static void ChooseADepositToRelease(AGame game, Player player1,
            Player player2, Player currentPlayer, P1VsP2 multiplayer)
        {
            Console.Clear();

            ViewDeposits(game.bank, "multiplayer release deposit", game, player1,
                            null, null, player2, multiplayer, currentPlayer); // chaning something

            WritingTextForChoosingADeposit("release");

            string input = Console.ReadLine();

            bool doesInputMatchDeposit = false;
            int depositNumIfExists = (-1);

            for (int depositNum = 0; depositNum < game.bank.deposits.Count; depositNum++)
            {
                if (input == $"{(depositNum + 1).ToString()}")
                {
                    doesInputMatchDeposit = true;
                    depositNumIfExists = depositNum;
                }
            }

            while (input.ToLower() != "m" && input.ToLower() != "v" && !doesInputMatchDeposit)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again: ");
                Console.WriteLine();
                input = Console.ReadLine();

                for (int depositNum = 0; depositNum < game.bank.deposits.Count; depositNum++)
                {
                    if (input == $"{(depositNum + 1).ToString()}")
                    {
                        doesInputMatchDeposit = true;
                        depositNumIfExists = depositNum;
                    }
                }
            }

            if (ShouldIReturnToMeunByEndingFunction(input))
            {
                NextPlayerTurn(game, player1, player2, multiplayer); // slight difference here

                return;
            }

            if (input.ToLower() == "v")
            {
                ViewDeposits(game.bank, "bank board", game, player1,
                    null, null, player2, multiplayer, currentPlayer); // slight difference here

                NextPlayerTurn(game, player1, player2, multiplayer);

                return;
            }

            if (doesInputMatchDeposit)
            {
                if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo.ToLower() != currentPlayer.name.ToLower())
                {
                    Console.WriteLine();
                    Console.WriteLine("You don't have this deposit!");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to return to choosing a deposit to ask to release.");
                    Console.WriteLine();
                    Console.ReadLine();

                    ChooseADepositToRelease(game, player1, player2, currentPlayer,
                        multiplayer);

                    return;
                }

                else if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo.ToLower() == currentPlayer.name.ToLower())
                {
                    BankAnswerForReleaseRequest(game, player1, player2, currentPlayer,
                        multiplayer, game.bank.deposits.ElementAt(depositNumIfExists));

                    return;
                }
            }
        }

        #endregion

        #region Did deposit release succeeded and what happened? (no prosecuting yet)

        public static void BankAnswerForReleaseRequest(AGame game,
            Player player1, Player player2, Player currentPlayer,
            P1VsP2 multiplayer, Deposit choosenDeposit)
        {
            bool didBankWentBankrupt = false;
            bool canTheDepositBeReleased = false;

            if (choosenDeposit.allPossibleExitPoints != null
                && choosenDeposit.allPossibleExitPoints.Count > 0)
            {
                foreach (DateTime possibleReleaseDate in choosenDeposit.allPossibleExitPoints)
                {
                    if (multiplayer.month == possibleReleaseDate.Month
                        && multiplayer.year == possibleReleaseDate.Year)
                    {
                        canTheDepositBeReleased = true;
                    }
                }
            }

            if (currentPlayer.InPanickMode)
            {
                canTheDepositBeReleased = true;
            }

            if (canTheDepositBeReleased)
            {
                currentPlayer.RowOfChoosingToDoNothing = 0;

                double howMuchTheBankWillPayYou = 0;
                double theMulct = 0;

                int whereIsThisDepositForThePlayer = currentPlayer.depositsOwned.IndexOf(choosenDeposit);

                double theFinalInterest = Math.Pow(choosenDeposit.actualInterestPerYear, 0.5);

                double monthsOfBuyTime = ((choosenDeposit.whenWasItBought.Year - 1) * 12) + choosenDeposit.whenWasItBought.Month;

                double monthsOfCurrentDate = ((multiplayer.year - 1) * 12) + multiplayer.month; // game mode object difference

                double depositLockedTimeInYears = (monthsOfCurrentDate - monthsOfBuyTime) / 12;

                for (double i = 0.5; i < depositLockedTimeInYears; i = i + 0.5)
                {
                    theFinalInterest = theFinalInterest * Math.Pow(choosenDeposit.actualInterestPerYear, 0.5);
                }

                howMuchTheBankWillPayYou = choosenDeposit.amountOfMoneyPutInDeposit * (theFinalInterest - 1);

                theMulct = howMuchTheBankWillPayYou / 2;

                if (game.bank.money > (howMuchTheBankWillPayYou - theMulct))
                { // tons of double code from here to the end
                    Console.Clear();
                    WritingWhichPlayerTurnItIs(currentPlayer); // remember this difference
                    Console.WriteLine();
                    Console.WriteLine(" == Notification == ");
                    Console.WriteLine();
                    Console.WriteLine($"The bank has released your deposit ('{choosenDeposit.name}')!");
                    Console.WriteLine();
                    Console.WriteLine($"You have received all its money ({choosenDeposit.amountOfMoneyPutInDeposit} dollars) and an additional {(howMuchTheBankWillPayYou)} dollars for the interest.");
                    Console.WriteLine();
                    Console.WriteLine($"While he somehow managed to mulct you with none but his stubborness with {theMulct} dollars.");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to continue");
                    Console.WriteLine();
                    Console.ReadLine();

                    currentPlayer.savingsAviliabe =
                    currentPlayer.savingsAviliabe + choosenDeposit.amountOfMoneyPutInDeposit
                    + howMuchTheBankWillPayYou - theMulct;

                    if (currentPlayer.name == player1.name)
                    {
                        player1 = currentPlayer;
                    }

                    else if (currentPlayer.name == player2.name)
                    {
                        player2 = currentPlayer;
                    }

                    game.bank.money = game.bank.money - howMuchTheBankWillPayYou + theMulct;

                    game.bank.numOfDepositsAviliabe = game.bank.numOfDepositsAviliabe + 1;
                }

                else
                {
                    game.bank.isBankrupt = true;
                    currentPlayer.savingsAviliabe = currentPlayer.savingsAviliabe + choosenDeposit.amountOfMoneyPutInDeposit + game.bank.money;
                    currentPlayer.depositsOwned.RemoveAt(currentPlayer.depositsOwned.IndexOf(choosenDeposit));

                    #region What to do if the bank went bankrupt?

                    List<Deposit> playerDepositsToRemove = new List<Deposit>();

                    foreach (Deposit DepositUsedToBeInBank in game.bank.deposits)
                    {
                        foreach (Deposit playerDeposit in currentPlayer.depositsOwned)
                        {
                            if (DepositUsedToBeInBank.name == playerDeposit.name)
                            {
                                playerDepositsToRemove.Add(playerDeposit);
                            }
                        }
                    }

                    foreach (Deposit depositToRemove in playerDepositsToRemove)
                    {
                        currentPlayer.savingsAviliabe = currentPlayer.savingsAviliabe + depositToRemove.amountOfMoneyPutInDeposit;
                    }

                    currentPlayer.depositsOwned.RemoveRange(0, currentPlayer.depositsOwned.Count);

                    #region Updating the other player when bankrupt

                    Player theOtherPlayer = null;

                    if (currentPlayer.name == player1.name)
                    {
                        theOtherPlayer = player2;
                    }

                    else if (currentPlayer.name == player2.name)
                    {
                        theOtherPlayer = player1;
                    }

                    foreach (Deposit OtherPlayerDeposit in theOtherPlayer.depositsOwned)
                    {
                        theOtherPlayer.savingsAviliabe = theOtherPlayer.savingsAviliabe
                            + OtherPlayerDeposit.amountOfMoneyPutInDeposit;
                    }

                    theOtherPlayer.depositsOwned.RemoveRange(0, theOtherPlayer.depositsOwned.Count);

                    if (currentPlayer.name == player1.name)
                    {
                        player2 = theOtherPlayer;
                    }

                    else if (currentPlayer.name == player2.name)
                    {
                        player1 = theOtherPlayer;
                    }

                    #endregion

                    #endregion

                    Console.Clear();
                    WritingWhichPlayerTurnItIs(currentPlayer);
                    Console.WriteLine();
                    Console.WriteLine(" == Notification ==");
                    Console.WriteLine();
                    Console.WriteLine("The bank has agreed to release your deposit, but went bankrupt.");
                    Console.WriteLine();
                    Console.WriteLine($"When he released the deposit '{choosenDeposit.name}',");
                    Console.WriteLine($"he was supposed to pay you {(howMuchTheBankWillPayYou)} dollars (in addition to the money you put in the deposit),");
                    Console.WriteLine($"and mulct you with {(theMulct)} for the early release,");
                    Console.WriteLine($"but he only had {game.bank.money} dollars left to pay you.");
                    Console.WriteLine();
                    Console.WriteLine($"So you've lost {Math.Round(howMuchTheBankWillPayYou - theMulct) - (game.bank.money)} dollars you were supposed to earn,");
                    Console.WriteLine($"and all your other deposits (and the other player's) have been released automatically (without any interests),");
                    Console.WriteLine("Increasing both of your savings.");
                    Console.WriteLine();
                    Console.WriteLine($"The bank has been 're-created' automatically with the same status it had at the start of this game.");
                    Console.WriteLine();
                    Console.WriteLine($"So now, you overall have {currentPlayer.savingsAviliabe} uninvested dollars,");
                    Console.WriteLine($"And the other player has now {theOtherPlayer.savingsAviliabe} uninvested dollars.");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to continue.");
                    Console.WriteLine();
                    Console.ReadLine();

                    if(currentPlayer.name == player1.name)
                    {
                        player1 = currentPlayer;
                    }

                    else if(currentPlayer.name == player2.name)
                    {
                        player2 = currentPlayer;
                    }

                    List<AGame> games = WritingAllGamesInformation();

                    Bank newBank = ReturnBankToDefault(games, game.name);

                    game.bank = newBank;

                    foreach(Deposit releasedDeposit in game.bank.deposits)
                    {
                        releasedDeposit.wasItReleasedInLastTurn = true;
                        releasedDeposit.whoReleasedItLastTurn = currentPlayer.name;
                    }

                    TheDataUpdatedWhateverDepositUpdateHappenedOrNot(game, player1, player2,
                         currentPlayer, theOtherPlayer, multiplayer);

                    return;
                }

                int depositIndexForTheBank = game.bank.deposits.IndexOf(choosenDeposit);

                double defaultInterest = game.bank.deposits.ElementAt(depositIndexForTheBank).DefaultinterestPerYear;

                if (game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints != null
                    && game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints.Count > 0)
                {
                    game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints.RemoveRange(
                    0, game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints.Count);
                }

                game.bank.deposits.ElementAt(depositIndexForTheBank).wasItReleasedInLastTurn = true;
                game.bank.deposits.ElementAt(depositIndexForTheBank).whoReleasedItLastTurn = currentPlayer.name;
                game.bank.deposits.ElementAt(depositIndexForTheBank).whoItBelongsTo = game.bank.name;
                game.bank.deposits.ElementAt(depositIndexForTheBank).actualInterestPerYear = defaultInterest;
                game.bank.deposits.ElementAt(depositIndexForTheBank).amountOfMoneyPutInDeposit = 0;
                game.bank.deposits.ElementAt(depositIndexForTheBank).whenWasItBought = DateTime.Parse("02/02/9999");
                game.bank.deposits.ElementAt(depositIndexForTheBank).whenItShouldBeReleased = DateTime.Parse("02/03/9999");

                currentPlayer.depositsOwned.RemoveAt(whereIsThisDepositForThePlayer);

                UpdateDataForNextTurn(game, player1, player2, currentPlayer, multiplayer);

                return;
            }

            else
            {
                Console.Clear();
                WritingWhichPlayerTurnItIs(currentPlayer);
                Console.WriteLine();
                Console.WriteLine(" == Notification ==");
                Console.WriteLine();
                Console.WriteLine($"The bank refused to release your deposit ('{choosenDeposit.name}').");

                if (choosenDeposit.exitPointsGap > 0)
                {
                    Console.WriteLine("Based on the deposit's exit points, you can only release this deposit in the following dates:");
                    Console.WriteLine();

                    for (int date = 0; date < choosenDeposit.allPossibleExitPoints.Count;
                        date++)
                    {
                        Console.WriteLine($"{date + 1}. year {choosenDeposit.allPossibleExitPoints.ElementAt(date).Year}, month {choosenDeposit.allPossibleExitPoints.ElementAt(date).Month}");
                        Console.WriteLine();
                    }
                }

                else if (choosenDeposit.exitPointsGap == 0)
                {
                    Console.WriteLine("You can't release this deposit by requesting it - because it has no exit points.");
                    Console.WriteLine();
                }

                Console.WriteLine("Enter anything to return to choosing if to act/view info/save/return to the main meun");
                Console.ReadLine();

                NextPlayerTurn(game, player1, player2, multiplayer);

                return;
            }
        }

        #endregion

        #endregion

        #region Updating all data to go to the next turn

        public static void UpdateDataForNextTurn(AGame game, Player player1,
            Player player2, Player currentPlayer, P1VsP2 multiplayer)
        {
            bool didBankWentBankrupt = false;

            #region Updating the deposits, the player and the bank accordingly

            double monthsOfGameTime = multiplayer.month + ((multiplayer.year - 1) * 12);

            if (currentPlayer.name == player1.name)
            {
                currentPlayer = player1;
            }

            if (currentPlayer.name == player2.name)
            {
                currentPlayer = player2;
            }

            currentPlayer.hasThePlayerFinishedHisTurn = true;

            if (currentPlayer.depositsOwned.Count > 0)
            {
                List<Deposit> backUpPlayerDeposits = new List<Deposit>();
                foreach (Deposit playerDeposit in currentPlayer.depositsOwned)
                {
                    backUpPlayerDeposits.Add(playerDeposit);
                }

                foreach (Deposit depositOfPlayer in backUpPlayerDeposits)
                {
                    if (!didBankWentBankrupt)
                    {
                        double monthsOfReleaseDepositTime = (depositOfPlayer.whenItShouldBeReleased.Month +
                            ((depositOfPlayer.whenItShouldBeReleased.Year - 1) * 12));

                        if (0 <= monthsOfReleaseDepositTime - monthsOfGameTime && monthsOfReleaseDepositTime - monthsOfGameTime < 6)
                        {
                            #region then release the deposit for the player here!

                            int whereIsThisDepositForThePlayer = currentPlayer.depositsOwned.IndexOf(depositOfPlayer);

                            double theFinalInterest = depositOfPlayer.actualInterestPerYear;

                            for (int i = 1; i < depositOfPlayer.timeSpan; i++)
                            {
                                theFinalInterest = theFinalInterest * depositOfPlayer.actualInterestPerYear;
                            }

                            double bankAdditionalReturnMoneyToPlayer = depositOfPlayer.amountOfMoneyPutInDeposit * (theFinalInterest - 1);

                            if (game.bank.money > bankAdditionalReturnMoneyToPlayer)
                            {
                                currentPlayer.savingsAviliabe =
                                currentPlayer.savingsAviliabe + depositOfPlayer.amountOfMoneyPutInDeposit + bankAdditionalReturnMoneyToPlayer;

                                game.bank.money = game.bank.money - bankAdditionalReturnMoneyToPlayer;

                                game.bank.numOfDepositsAviliabe = game.bank.numOfDepositsAviliabe + 1;

                                Console.Clear();
                                WritingWhichPlayerTurnItIs(currentPlayer);
                                Console.WriteLine();
                                Console.WriteLine(" == Notification ==");
                                Console.WriteLine();
                                Console.WriteLine($"The bank released your deposit: '{depositOfPlayer.name}'");
                                Console.WriteLine();
                                Console.WriteLine($"You have received all its money ({depositOfPlayer.amountOfMoneyPutInDeposit} dollars) and an additional {(bankAdditionalReturnMoneyToPlayer)} dollars for the interest.");
                                Console.WriteLine();
                                Console.WriteLine("Enter anything to continue.");
                                Console.WriteLine();
                                Console.ReadLine();
                            }

                            else
                            {
                                game.bank.isBankrupt = true;
                                currentPlayer.savingsAviliabe = currentPlayer.savingsAviliabe + depositOfPlayer.amountOfMoneyPutInDeposit + game.bank.money;
                                currentPlayer.depositsOwned.RemoveAt(currentPlayer.depositsOwned.IndexOf(depositOfPlayer));

                                #region What to do if the bank went bankrupt?

                                List<Deposit> playerDepositsToRemove = new List<Deposit>();

                                foreach (Deposit DepositUsedToBeInBank in game.bank.deposits)
                                {
                                    foreach (Deposit playerDeposit in currentPlayer.depositsOwned)
                                    {
                                        if (DepositUsedToBeInBank.name == playerDeposit.name)
                                        {
                                            playerDepositsToRemove.Add(playerDeposit);
                                        }
                                    }
                                }

                                foreach (Deposit depositToRemove in playerDepositsToRemove)
                                {
                                    currentPlayer.savingsAviliabe = currentPlayer.savingsAviliabe + depositToRemove.amountOfMoneyPutInDeposit;
                                }

                                currentPlayer.depositsOwned.RemoveRange(0, currentPlayer.depositsOwned.Count);

                                #region Updating the other player when bankrupt

                                Player theOtherPlayer = null;

                                if (currentPlayer.name == player1.name)
                                {
                                    theOtherPlayer = player2;
                                }

                                else if (currentPlayer.name == player2.name)
                                {
                                    theOtherPlayer = player1;
                                }

                                foreach (Deposit OtherPlayerDeposit in theOtherPlayer.depositsOwned)
                                {
                                    theOtherPlayer.savingsAviliabe = theOtherPlayer.savingsAviliabe
                                        + OtherPlayerDeposit.amountOfMoneyPutInDeposit;
                                }

                                theOtherPlayer.depositsOwned.RemoveRange(0, theOtherPlayer.depositsOwned.Count);

                                if (currentPlayer.name == player1.name)
                                {
                                    player2 = theOtherPlayer;
                                }

                                else if (currentPlayer.name == player2.name)
                                {
                                    player1 = theOtherPlayer;
                                }

                                #endregion

                                Console.Clear();
                                WritingWhichPlayerTurnItIs(currentPlayer);
                                Console.WriteLine();
                                Console.WriteLine(" == Notification ==");
                                Console.WriteLine();
                                Console.WriteLine("The bank has went bankrupt.");
                                Console.WriteLine();
                                Console.WriteLine($"When he released the deposit '{depositOfPlayer.name}',");
                                Console.WriteLine($"he was supposed to pay you {Math.Round(bankAdditionalReturnMoneyToPlayer)} dollars (in addition to the {depositOfPlayer.amountOfMoneyPutInDeposit} dollars you put in the deposit),");
                                Console.WriteLine($"but he only had {game.bank.money} dollars left to pay you.");
                                Console.WriteLine();
                                Console.WriteLine($"So you've lost {Math.Round(bankAdditionalReturnMoneyToPlayer) - (game.bank.money)} dollars you were supposed to earn for this deposit,");
                                Console.WriteLine($"and all your (and the other player's) other deposits have been released automatically (without any interests),");
                                Console.WriteLine();
                                Console.WriteLine($"So now, you overall have {currentPlayer.savingsAviliabe} uninvested dollars,");
                                Console.WriteLine($"And the other player has now {theOtherPlayer.savingsAviliabe} uninvested dollars.");
                                Console.WriteLine();
                                Console.WriteLine($"The bank has been 're-created' automatically with the same status it had at the start of this game.");
                                Console.WriteLine();
                                Console.WriteLine("Enter anything to continue.");
                                Console.WriteLine();
                                Console.ReadLine();

                                List<AGame> games = WritingAllGamesInformation();

                                Bank newBank = ReturnBankToDefault(games, game.name);

                                game.bank = newBank;

                                foreach (Deposit releasedDeposit in game.bank.deposits)
                                {
                                    releasedDeposit.wasItReleasedInLastTurn = true;
                                    releasedDeposit.whoReleasedItLastTurn = currentPlayer.name;
                                }

                                didBankWentBankrupt = true;

                                #endregion
                            }

                            if (!didBankWentBankrupt)
                            {
                                int depositIndexForTheBank = game.bank.deposits.IndexOf(depositOfPlayer);

                                double defaultInterest = game.bank.deposits.ElementAt(depositIndexForTheBank).DefaultinterestPerYear;

                                if (game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints != null
                                && game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints.Count > 0)
                                {
                                    game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints.RemoveRange(
                                    0, game.bank.deposits.ElementAt(depositIndexForTheBank).allPossibleExitPoints.Count);
                                }

                                game.bank.deposits.ElementAt(depositIndexForTheBank).wasItReleasedInLastTurn = true;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whoReleasedItLastTurn = currentPlayer.name;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whoItBelongsTo = game.bank.name;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).actualInterestPerYear = defaultInterest;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).amountOfMoneyPutInDeposit = 0;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whenWasItBought = DateTime.Parse("02/02/9999");
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whenItShouldBeReleased = DateTime.Parse("02/03/9999");

                                currentPlayer.depositsOwned.RemoveAt(whereIsThisDepositForThePlayer);
                            }

                            #endregion
                        }
                    }
                }
            }

            if(currentPlayer.name == player1.name)
            {
                player1 = currentPlayer;

                TheDataUpdatedWhateverDepositUpdateHappenedOrNot(
                game, player1, player2, currentPlayer, player2, multiplayer);
            }

            else if(currentPlayer.name == player2.name)
            {
                player2 = currentPlayer;

                TheDataUpdatedWhateverDepositUpdateHappenedOrNot(
                game, player1, player2, currentPlayer, player1, multiplayer);
            }

            return;

            #endregion
        }

        public static void TheDataUpdatedWhateverDepositUpdateHappenedOrNot
            (AGame game, Player player1, Player player2, Player currentPlayer,
            Player theOtherPlayer, P1VsP2 multiplayer)
        {
            if (currentPlayer.name == player2.name) // game time and winning is only checked/changed at the end of a round
            {
                if (Math.Round(currentPlayer.savingsAviliabe + 1) >= game.moneyToEndGame
                    || Math.Round(theOtherPlayer.savingsAviliabe + 1) >= game.moneyToEndGame)
                {
                    string result = "win";

                    #region Who won??

                    Player playerWhoWon = null;
                    Player playerWhoLost = null;
                    Player drawPlayer = null;

                    if (player1.savingsAviliabe < game.moneyToEndGame)
                    {
                        playerWhoWon = player2;
                        playerWhoLost = player1;
                    }

                    else if (player2.savingsAviliabe < game.moneyToEndGame)
                    {
                        playerWhoWon = player1;
                        playerWhoLost = player2;
                    }

                    else if (player1.savingsAviliabe >= game.moneyToEndGame
                        && player2.savingsAviliabe >= game.moneyToEndGame)
                    {
                        if (player1.savingsAviliabe > player2.savingsAviliabe)
                        {
                            playerWhoWon = player1;
                            playerWhoLost = player2;

                            result = "same turn";
                        }

                        else if (player2.savingsAviliabe > player1.savingsAviliabe)
                        {
                            playerWhoWon = player2;
                            playerWhoLost = player1;

                            result = "same turn";
                        }

                        else if (player2.savingsAviliabe == player1.savingsAviliabe)
                        {
                            result = "draw";
                            drawPlayer = player1;
                        }
                    }

                    #endregion // double code!

                    string inputForHighScore = MultiplayerGameFinished
                        (game, playerWhoWon, playerWhoLost,
                        multiplayer, result, drawPlayer);

                    while (inputForHighScore.ToLower() != "y" && inputForHighScore.ToLower() != "n")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid input. Enter again:");
                        Console.WriteLine();
                        inputForHighScore = Console.ReadLine();
                    }

                    if (inputForHighScore.ToLower() == "y" && result.ToLower() != "draw")
                    {
                        EnteringScoreIntoHighScore(game, playerWhoWon, null,
                            multiplayer);
                    }

                    if (inputForHighScore.ToLower() == "n")
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Your score has (not) been submitted successfully.");
                        Console.WriteLine();
                    }

                    Console.WriteLine("Enter anything to continue.");
                    Console.WriteLine();
                    Console.ReadLine();

                    return;
                }

                else
                {
                    #region Updating game time

                    if (multiplayer.month == 7)
                    {
                        multiplayer.year = multiplayer.year + 1;
                        multiplayer.month = 1;
                    }

                    else if (multiplayer.month == 1)
                    {
                        multiplayer.month = 7;
                    }

                    #endregion

                    player1.savingsAviliabe = player1.savingsAviliabe + player1.income;
                    player2.savingsAviliabe = player2.savingsAviliabe + player2.income;

                    player1.hasThePlayerFinishedHisTurn = false;
                    player2.hasThePlayerFinishedHisTurn = false;

                    if (Math.Round(currentPlayer.savingsAviliabe + 1) >= game.moneyToEndGame
                    || Math.Round(theOtherPlayer.savingsAviliabe + 1) >= game.moneyToEndGame)
                    {
                        #region What if game finished?

                        string result = "win";

                        #region Who won??

                        Player playerWhoWon = null;
                        Player playerWhoLost = null;
                        Player drawPlayer = null;

                        if (player1.savingsAviliabe < game.moneyToEndGame)
                        {
                            playerWhoWon = player2;
                            playerWhoLost = player1;
                        }

                        else if (player2.savingsAviliabe < game.moneyToEndGame)
                        {
                            playerWhoWon = player1;
                            playerWhoLost = player2;
                        }

                        else if (player1.savingsAviliabe >= game.moneyToEndGame
                            && player2.savingsAviliabe >= game.moneyToEndGame)
                        {
                            if (player1.savingsAviliabe > player2.savingsAviliabe)
                            {
                                playerWhoWon = player1;
                                playerWhoLost = player2;

                                result = "same turn";
                            }

                            else if (player2.savingsAviliabe > player1.savingsAviliabe)
                            {
                                playerWhoWon = player2;
                                playerWhoLost = player1;

                                result = "same turn";
                            }

                            else if (player2.savingsAviliabe == player1.savingsAviliabe)
                            {
                                result = "draw";
                                drawPlayer = player1;
                            }
                        }

                        #endregion // double code!

                        string inputForHighScore = MultiplayerGameFinished
                            (game, playerWhoWon, playerWhoLost,
                            multiplayer, result, drawPlayer);

                        while (inputForHighScore.ToLower() != "y" && inputForHighScore.ToLower() != "n")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Invalid input. Enter again:");
                            Console.WriteLine();
                            inputForHighScore = Console.ReadLine();
                        }

                        if (inputForHighScore.ToLower() == "y" && result.ToLower() != "draw")
                        {
                            EnteringScoreIntoHighScore(game, playerWhoWon, null,
                                multiplayer);
                        }

                        if (inputForHighScore.ToLower() == "n")
                        {
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine("Your score has (not) been submitted successfully.");
                            Console.WriteLine();
                        }

                        Console.WriteLine("Enter anything to continue.");
                        Console.WriteLine();
                        Console.ReadLine();

                        return;

                        #endregion
                    }
                }
            }

            #region Updating the current player with nothing released

            #region panick mode next turn or not?

            int countLongDeposits = 0;
            double sumOfInvestements = 0;

            if (currentPlayer.depositsOwned.Count > 0)
            {
                foreach (Deposit playerDeposit in currentPlayer.depositsOwned)
                {
                    sumOfInvestements = sumOfInvestements + playerDeposit.amountOfMoneyPutInDeposit;

                    if (playerDeposit.timeSpan >= 5)
                    {
                        countLongDeposits = countLongDeposits + 1;
                    }
                }
            }

            bool isPlayerInvestingAlmostTooMuch = false;

            if (sumOfInvestements > 0)
            {
                isPlayerInvestingAlmostTooMuch = (sumOfInvestements / currentPlayer.savingsAviliabe) >= (game.riskProfile - 0.05);
            }

            bool AreTheTermsTooLong =
                countLongDeposits >= 2;

            currentPlayer.InPanickMode = (AreTheTermsTooLong && isPlayerInvestingAlmostTooMuch);

            #endregion

            if(currentPlayer.name == player1.name)
            {
                currentPlayer.hasThePlayerFinishedHisTurn = true;

                player1 = currentPlayer;
            }

            else if(currentPlayer.name == player2.name)
            {
                player2 = currentPlayer;
            }

            #endregion

            NextPlayerTurn(game, player1, player2, multiplayer);

            return;
        }

        public static string MultiplayerGameFinished(AGame game, Player playerWhoWon,
            Player playerWhoLost, P1VsP2 Multiplayer, string result, Player drawPlayer = null)
        {
            if (result.ToLower() == "win" || result.ToLower() == "same turn")
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(" == GAME FINISHED! ==");
                Console.WriteLine();
                Console.WriteLine($"Congrats, you ***!! (I mean, '{playerWhoWon.name}'..)");
                Console.WriteLine();
                Console.WriteLine($"you have at least {game.moneyToEndGame} dollars, and beat '{playerWhoLost.name}' at '{game.name}'..");
                Console.WriteLine();
                Console.WriteLine($"you now have {playerWhoWon.savingsAviliabe} uninvested dollars,");
                Console.WriteLine($"And the other player, '{playerWhoLost.name}', finished with {playerWhoLost.savingsAviliabe} uninvested dollars.");
                if (result.ToLower() == "same turn")
                {
                    Console.WriteLine("(Althought the other player finished at the same date, he had less money so he still lost).");
                }
                Console.WriteLine();
                Console.WriteLine($"This means that the player who won, surpassed the amount neccesary to finish by {Math.Round(playerWhoWon.savingsAviliabe - game.moneyToEndGame)} dollars.");
                Console.WriteLine($"(Reminder: The above calculatin does NOT take into account money you have invested in deposits)");
                Console.WriteLine();
                Console.WriteLine($"You finished the game at: {Multiplayer.year} years and {Multiplayer.month} months in game time.");
                Console.WriteLine();
                Console.WriteLine("Do you wish to save into the high score chart of this game? (only the winner's score will be submitted)");
                Console.WriteLine();
                Console.WriteLine("Enter 'y' for yes, or 'n' for no.");
                Console.WriteLine();

                string input = Console.ReadLine();
                return input;
            }

            else if(result.ToLower() == "draw")
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(" == GAME FINISHED! ==");
                Console.WriteLine();
                Console.WriteLine($"Well, it's a draw.");
                Console.WriteLine($"Both players reached {game.moneyToEndGame} dollars, and finished '{game.name}'.. both have {drawPlayer.savingsAviliabe} dollars.");
                Console.WriteLine($"(Reminder: The above calculatin does NOT take into account any money you may have invested in deposits)");
                Console.WriteLine();
                Console.WriteLine($"You both finished the game at: {Multiplayer.year} years and {Multiplayer.month} months in game time,");
                Console.WriteLine();

                return "y";
            }

            throw new Exception("Shouldn't be able to reach here");
        }

        #endregion

        #region Panic mode turn

        public static void PanickModeTurn(Player player1, Player player2,
            Player currentPlayer, AGame game, P1VsP2 multiPlayer)
        {
            Deposit depositToRelease = WhichDepositToReleaseInPanicMode(currentPlayer);

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" == It's the turn of '{currentPlayer.name}' ==");
            Console.WriteLine();
            Console.WriteLine("In-game date:");
            Console.WriteLine($"year {multiPlayer.year}, month {multiPlayer.month}.");
            Console.WriteLine();
            Console.WriteLine($"Sum of money that isn't invested: {currentPlayer.savingsAviliabe} dollars");
            Console.WriteLine();
            Console.WriteLine($"Since you're panick mode, you (already) forced yourself to release a deposit.");
            Console.WriteLine($"The riskiest calculated deposit you have is '{depositToRelease.name}',");
            Console.WriteLine($"And thus, you are now going to release it.");
            Console.WriteLine();
            Console.WriteLine("Enter the number of your choice out of the following:");
            Console.WriteLine();
            Console.WriteLine($"1. Continue and release that deposit.");
            Console.WriteLine("2. Save the game state.");
            Console.WriteLine("You may also enter 'm' to return to the main meun");
            Console.WriteLine();

            string input = Console.ReadLine();

            if (ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            while (input != "1" && input != "2")
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again:");
                Console.WriteLine();
                input = Console.ReadLine();

                if (ShouldIReturnToMeunByEndingFunction(input))
                {
                    return;
                }
            }

            if (input == "1")
            {
                BankAnswerForReleaseRequest(game, player1, player2, currentPlayer, multiPlayer,
                    depositToRelease);
            }

            else if (input == "2")
            {
                SavingProgress(game, player1, player2, null, multiPlayer);

                NextPlayerTurn(game, player1, player2, multiPlayer);
            }

            return;
        }

        #endregion

        #region Reminding which player turn it is

        public static void WritingWhichPlayerTurnItIs(Player currentPlayer)
        {
            Console.WriteLine();
            Console.WriteLine($"In the turn of the player: '{currentPlayer.name}'");
            Console.WriteLine();
        }

        #endregion

        #region Properties

        public double month { get; set; }
        public double year { get; set; }

        #endregion
    }
}
