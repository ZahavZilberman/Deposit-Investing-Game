using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class TimeTrial : Program
    {
        #region ctor

        public TimeTrial()
        {
            month = 1;
            year = 1;
        }

        #endregion

        #region the actual function (the 4 main choices)
        
        public static void NextTurn(AGame game, Player player1, TimeTrial timeTrial)
        {
            #region Conditions neccesary

            if(player1.InPanickMode)
            {
                PanickModeStateNotification(false, null);

                PanickModeTurn(player1, game, timeTrial);

                return;
            }

            #endregion

            player1.hasThePlayerFinishedHisTurn = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" == Your turn ==");
            Console.WriteLine();
            Console.WriteLine("In-game date:");
            Console.WriteLine($"year {timeTrial.year}, month {timeTrial.month}.");
            Console.WriteLine();
            Console.WriteLine($"Sum of money that isn't invested: {player1.savingsAviliabe} dollars");
            Console.WriteLine();
            Console.WriteLine($"Enter the number of your choice out of the following:");
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

            if(ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            while(input != "1" && input != "2" && input != "3")
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input. Enter again:");
                Console.WriteLine();
                input = Console.ReadLine();

                if(ShouldIReturnToMeunByEndingFunction(input))
                {
                    return;
                }
            }

            if(input.ToLower() == "1")
            {
                ChoosingAnAction(game, player1, timeTrial);
            }

            else if (input.ToLower() == "2")
            {
                ViewingAllInfo(player1, game);

                NextTurn(game, player1, timeTrial);
            }

            else if (input.ToLower() == "3")
            {
                SavingProgress(game, player1, null, timeTrial);

                NextTurn(game, player1, timeTrial);
            }

            return;
        }

        #endregion

        #region All Actions and the game's reponses to them (choosing one first)

        public static void ChoosingAnAction(AGame game, Player player, TimeTrial timeTrial)
        {
            Console.Clear();
            Console.WriteLine();

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
                NextTurn(game, player, timeTrial);
            }

            else if (actionChoosen == "1")
            {
                if(player.savingsAviliabe == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Trying to be a smartass huh? Investing without having any money? Well then.. *charges attack*");
                    Console.WriteLine();
                    Console.WriteLine("Just kidding, enter anything to return to choose an action.");
                    Console.WriteLine();
                    Console.ReadLine();

                    ChoosingAnAction(game, player, timeTrial);

                    return;
                }

                BuyingADeposit(player, game, timeTrial);
            }

            else if (actionChoosen == "2")
            {
                ChooseADepositToRelease(player, game, timeTrial);
            }

            else if (actionChoosen == "3")
            {
                DoingNothing(game, player, timeTrial);
            }

            return;
        }

        #region Doing nothing

        public static void DoingNothing(AGame game, Player player, TimeTrial timeTrial)
        {
            #region How many times the player choose this?!

            player.RowOfChoosingToDoNothing = player.RowOfChoosingToDoNothing + 1;

            if (player.RowOfChoosingToDoNothing > 3)
            {
                Console.WriteLine();
                Console.WriteLine("It's the 4th turn in a row you choose doing nothing! You can't do nothing for more than 3 turns in a row.");
                Console.WriteLine("(BTW, I really wonder if the richest people in the world spent most of their life doing nothing?)");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return to choose an action, either to ask to release a deposit or buy one.");
                Console.WriteLine();

                player.RowOfChoosingToDoNothing = player.RowOfChoosingToDoNothing - 1;

                string nonNothingInput = Console.ReadLine();
                ChoosingAnAction(game, player, timeTrial);

                return;
            }

            #endregion

            #region If not too much, then here we go to the next turn after update!

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("You successfully sit on your ass for 6 months not even calling the bank,");
            Console.WriteLine("while proably singing 'the lazy song' or 'gangam style'..");
            Console.WriteLine();
            Console.WriteLine("Enter anything to continue to your next turn");
            Console.WriteLine();
            Console.ReadLine();

            UpdateDataForNextTurn(game, player, timeTrial);

            return;

            #endregion
        }

        #endregion

        #region Buying a deposit

        #region choosing deposit to buy

        public static void BuyingADeposit(Player player, AGame game, TimeTrial timeTrial)
        {
            ViewDeposits(game.bank, "time trial buy deposit", game, player, timeTrial, null);

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

            while(input.ToLower() != "m" && input.ToLower() != "v" && !doesInputMatchDeposit)
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
                NextTurn(game, player, timeTrial);

                return;
            }

            if (input.ToLower() == "v")
            {
                ViewDeposits(game.bank, "bank board", game, player, timeTrial);

                NextTurn(game, player, timeTrial);

                return;
            }

            if(doesInputMatchDeposit)
            {
                if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo == player.name
                    && game.bank.deposits.ElementAt(depositNumIfExists).amountOfMoneyPutInDeposit > 0)
                {
                    WritingYouAlreadyOwnTheDeposit();

                    BuyingADeposit(player, game, timeTrial);
                }

                else if((game.bank.deposits.ElementAt(depositNumIfExists).minimumMoneyForDeposit / game.riskProfile) > player.savingsAviliabe)
                {
                    WritingYouDontHaveEnoughMoneyForDeposit(game, depositNumIfExists);

                    BuyingADeposit(player, game, timeTrial);
                }

                else if ((game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo == game.bank.name))
                {
                    PuttingMoneyIntoChoosenDeposit(player, game, timeTrial, game.bank.deposits.ElementAt(depositNumIfExists));
                }

                // There's no way for a clash between players in this mode
            }

            return;
        }

        #endregion

        #region Choosing the amount of money for the choosen deposit

        public static void PuttingMoneyIntoChoosenDeposit(Player player, AGame game, TimeTrial timeTrial, Deposit choosenDeposit)
        {
            bool isMinimumMoneyRiskProfileBased = (((game.riskProfile / 10) * player.savingsAviliabe) >= choosenDeposit.minimumMoneyForDeposit);
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"Looks like the deoposit '{choosenDeposit.name}' is available!");
            Console.WriteLine();
            Console.WriteLine($"Seeing as your risk profile is {game.riskProfile * 100}%,");

            if(!isMinimumMoneyRiskProfileBased)
            {
                Console.WriteLine($"the amount of money you can put in is between {choosenDeposit.minimumMoneyForDeposit} (minimum money for deposit) and {game.riskProfile * 100}% of your savings,");
            }

            else
            {
                Console.WriteLine($"the amount of money you can put in is between {game.riskProfile * 10}% and {game.riskProfile * 100}% of your savings,");
            }

            Console.WriteLine($"And that's out of the {player.savingsAviliabe} dollars you have uninvested right now!");
            Console.WriteLine();
            Console.WriteLine("Enter the amount of money you want to put in this deposit,");
            Console.WriteLine("Or enter 'm' to return choosing a deposit to buy.");
            Console.WriteLine();
            Console.WriteLine($"Note: if the game doesn't allow you to put {game.riskProfile * 100}% of your money, it's a bug - try to put 1 dollar less than that)");
            Console.WriteLine();

            string moneyInputForDeposit = Console.ReadLine();

            if(moneyInputForDeposit.ToLower() == "m")
            {
                BuyingADeposit(player, game, timeTrial);

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

                PuttingMoneyIntoChoosenDeposit(player, game, timeTrial, choosenDeposit);

                return;
            }

            else if((moneyForDeposit < (player.savingsAviliabe * (game.riskProfile / 10))) && isMinimumMoneyRiskProfileBased)
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

                if(input.ToLower() == "d")
                {
                    DoingNothing(game, player, timeTrial);
                }

                else
                {
                    PuttingMoneyIntoChoosenDeposit(player, game, timeTrial, choosenDeposit);
                }

                return;
            }

            else if((moneyForDeposit < choosenDeposit.minimumMoneyForDeposit) && !isMinimumMoneyRiskProfileBased)
            {
                Console.WriteLine();
                Console.WriteLine($"Can't allow this.. You must put at least {choosenDeposit.minimumMoneyForDeposit} dollars to get away with it.");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return choosing the amount of money to put in this deposit");
                Console.WriteLine();
                Console.ReadLine();

                PuttingMoneyIntoChoosenDeposit(player, game, timeTrial, choosenDeposit);

                return;
            }

            else if (moneyForDeposit > (player.savingsAviliabe * game.riskProfile))
            {
                Console.WriteLine();
                Console.WriteLine("This amount of money is higher than your risk profile allows you to put.");
                Console.WriteLine("I would've had to make you go into panick mode if I weren't such a nice game developer *_*");
                Console.WriteLine();
                Console.WriteLine("Enter anything to return choosing the amount of money to put in this deposit");
                Console.WriteLine();
                Console.ReadLine();

                PuttingMoneyIntoChoosenDeposit(player, game, timeTrial, choosenDeposit);

                return;
            }

            #endregion

            UpdatingAfterBuyingDeposit(player, game, timeTrial, choosenDeposit, moneyForDeposit);

            return;
        }

        #endregion

        #region Updating after player buys a deposit

        public static void UpdatingAfterBuyingDeposit(Player player, AGame game, TimeTrial timeTrial,
            Deposit choosenDeposit, double amountOfMoneyForDeposit)
        {
            choosenDeposit.whoItBelongsTo = player.name;
            choosenDeposit.wasItReleasedInLastTurn = false;
            choosenDeposit.amountOfMoneyPutInDeposit = amountOfMoneyForDeposit;

            player.savingsAviliabe = player.savingsAviliabe - amountOfMoneyForDeposit;

            DateTime buyingTime = new DateTime();

            buyingTime = AnyDataUpdate(timeTrial.month, timeTrial.year);
            choosenDeposit.whenWasItBought = buyingTime;

            DateTime releaseTime = new DateTime();

            double depositTimeSpan = choosenDeposit.timeSpan;

            double YearOfDepositRelease = buyingTime.Year + depositTimeSpan;

            releaseTime = AnyDataUpdate(timeTrial.month, YearOfDepositRelease);
            choosenDeposit.whenItShouldBeReleased = releaseTime;

            double defaultInterest = choosenDeposit.DefaultinterestPerYear;
            double actualInterest = 0;

            if(amountOfMoneyForDeposit < game.moneyToEndGame / 10)
            {
                actualInterest = choosenDeposit.DefaultinterestPerYear;
            }
            
            else if(amountOfMoneyForDeposit >= (game.moneyToEndGame / 10) && amountOfMoneyForDeposit < (game.moneyToEndGame / 4))
            {
                actualInterest = Math.Pow(choosenDeposit.DefaultinterestPerYear, 1.5);
            }

            else if(amountOfMoneyForDeposit >= (game.moneyToEndGame / 4))
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

                    if(currentdate.Month + choosenDeposit.exitPointsGap > 12)
                    {
                        int yearsAddition = (int)(currentdate.Month + choosenDeposit.exitPointsGap) / 12;
                        double actualMonths = (currentdate.Month + choosenDeposit.exitPointsGap) - (yearsAddition * 12);

                        exitPoint = AnyDataUpdate(actualMonths, currentdate.Year + yearsAddition);
                    }

                    else
                    {
                        exitPoint = AnyDataUpdate(currentdate.Month + choosenDeposit.exitPointsGap, currentdate.Year);
                    }

                    if(((exitPoint.Month + (exitPoint.Year * 12)) <= ((choosenDeposit.whenItShouldBeReleased.Month)
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

            player.depositsOwned.Add(choosenDeposit);
            player.RowOfChoosingToDoNothing = 0;

            #region And when the update is done..

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"The deposit '{choosenDeposit.name}' has been locked for you, for {choosenDeposit.timeSpan} years, with {choosenDeposit.amountOfMoneyPutInDeposit} dollars in it (..)");
            Console.WriteLine();
            Console.WriteLine("Enter anything to go on to the next turn");
            Console.WriteLine();
            Console.ReadLine();

            UpdateDataForNextTurn(game, player, timeTrial);

            return;

            #endregion
        }

        #endregion

        #endregion

        #region Trying to release a deposit

        #region Choosing a deposit to release

        public static void ChooseADepositToRelease(Player player, AGame game, TimeTrial timeTrial)
        {
            Console.Clear();

            ViewDeposits(game.bank, "time trial release deposit", game, player, timeTrial, null);

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
                NextTurn(game, player, timeTrial);

                return;
            }

            if (input.ToLower() == "v")
            {
                ViewDeposits(game.bank, "bank board", game, player, timeTrial);

                NextTurn(game, player, timeTrial);

                return;
            }

            if (doesInputMatchDeposit)
            {
                if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo.ToLower() != player.name.ToLower())
                {
                    Console.WriteLine();
                    Console.WriteLine("You don't have this deposit!");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to return to choosing a deposit to ask to release.");
                    Console.WriteLine();
                    Console.ReadLine();

                    ChooseADepositToRelease(player, game, timeTrial);

                    return;
                }

                else if (game.bank.deposits.ElementAt(depositNumIfExists).whoItBelongsTo.ToLower() == player.name.ToLower())
                {
                    BankAnswerForReleaseRequest(player, game, timeTrial, game.bank.deposits.ElementAt(depositNumIfExists));

                    return;
                }
                
                // There's no way for a clash between players in this mode
            }

        }

        #endregion

        #region bank responses to the release request (no prosecuting yet)

        public static void BankAnswerForReleaseRequest(Player player, AGame game, TimeTrial timeTrial, Deposit choosenDeposit)
        {
            bool canTheDepositBeReleased = false;

            if (choosenDeposit.allPossibleExitPoints != null
                && choosenDeposit.allPossibleExitPoints.Count > 0)
            {
                foreach (DateTime possibleReleaseDate in choosenDeposit.allPossibleExitPoints)
                {
                    if (timeTrial.month == possibleReleaseDate.Month
                        && timeTrial.year == possibleReleaseDate.Year)
                    {
                        canTheDepositBeReleased = true;
                    }
                }
            }

            if(player.InPanickMode)
            {
                canTheDepositBeReleased = true;
            }

            if (canTheDepositBeReleased)
            {
                player.RowOfChoosingToDoNothing = 0;

                double howMuchTheBankWillPayYou = 0;
                double theMulct = 0;

                int whereIsThisDepositForThePlayer = player.depositsOwned.IndexOf(choosenDeposit);

                double theFinalInterest = Math.Pow(choosenDeposit.actualInterestPerYear, 0.5);

                double monthsOfBuyTime = ((choosenDeposit.whenWasItBought.Year - 1) * 12) + choosenDeposit.whenWasItBought.Month;

                double monthsOfCurrentDate = ((timeTrial.year - 1) * 12) + timeTrial.month;

                double depositLockedTimeInYears = (monthsOfCurrentDate - monthsOfBuyTime) / 12;

                for (double i = 0.5; i < depositLockedTimeInYears; i = i + 0.5)
                {
                    theFinalInterest = theFinalInterest * Math.Pow(choosenDeposit.actualInterestPerYear, 0.5);
                }

                howMuchTheBankWillPayYou = choosenDeposit.amountOfMoneyPutInDeposit * (theFinalInterest - 1);

                theMulct = howMuchTheBankWillPayYou / 2;

                if (game.bank.money > (howMuchTheBankWillPayYou - theMulct))
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine(" == Notification == ");
                    Console.WriteLine();
                    Console.WriteLine($"The bank has released your deposit ('{choosenDeposit.name}')!");
                    Console.WriteLine();
                    Console.WriteLine($"You have received all its money ({choosenDeposit.amountOfMoneyPutInDeposit} dollars) and an additional {(howMuchTheBankWillPayYou)} dollars for the interest.");
                    Console.WriteLine();
                    Console.WriteLine($"While he somehow managed to mulct you with none but his stubborness with {theMulct} dollars.");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to continue to your next turn");
                    Console.WriteLine();
                    Console.ReadLine();

                    player.savingsAviliabe =
                    player.savingsAviliabe + choosenDeposit.amountOfMoneyPutInDeposit
                    + howMuchTheBankWillPayYou - theMulct;

                    game.bank.money = game.bank.money - howMuchTheBankWillPayYou + theMulct;

                    game.bank.numOfDepositsAviliabe = game.bank.numOfDepositsAviliabe + 1;
                }

                else
                {
                    Console.Clear();
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
                    Console.WriteLine($"and all your other deposits have been released automatically (without any interests), increasing your savings.");
                    Console.WriteLine();
                    Console.WriteLine($"The bank has been 're-created' automatically with the same status it had at the start of this game.");
                    Console.WriteLine();
                    Console.WriteLine("Enter anything to continue.");
                    Console.WriteLine();
                    Console.ReadLine();

                    game.bank.isBankrupt = true;
                    player.savingsAviliabe = player.savingsAviliabe + choosenDeposit.amountOfMoneyPutInDeposit + game.bank.money;
                    player.depositsOwned.RemoveAt(player.depositsOwned.IndexOf(choosenDeposit));

                    #region What to do if the bank went bankrupt?

                    List<Deposit> playerDepositsToRemove = new List<Deposit>();

                    foreach (Deposit DepositUsedToBeInBank in game.bank.deposits)
                    {
                        foreach (Deposit playerDeposit in player.depositsOwned)
                        {
                            if (DepositUsedToBeInBank.name == playerDeposit.name)
                            {
                                playerDepositsToRemove.Add(playerDeposit);
                            }
                        }
                    }

                    foreach (Deposit depositToRemove in playerDepositsToRemove)
                    {
                        player.savingsAviliabe = player.savingsAviliabe + depositToRemove.amountOfMoneyPutInDeposit;
                    }

                    player.depositsOwned.RemoveRange(0, player.depositsOwned.Count);

                    List<AGame> games = WritingAllGamesInformation();

                    Bank newBank = ReturnBankToDefault(games, game.name);

                    game.bank = newBank;

                    TheDataUpdatedWhateverDepositUpdateHappenedOrNot(
                    player, game, timeTrial); // there's no need to call the 'UpdateDataForNextTurn' function

                    return;

                    #endregion
                }

                int depositIndexForTheBank = game.bank.deposits.IndexOf(choosenDeposit);

                double defaultInterest = game.bank.deposits.ElementAt(depositIndexForTheBank).DefaultinterestPerYear;

                game.bank.deposits.ElementAt(depositIndexForTheBank).wasItReleasedInLastTurn = true;
                game.bank.deposits.ElementAt(depositIndexForTheBank).whoReleasedItLastTurn = player.name;
                game.bank.deposits.ElementAt(depositIndexForTheBank).whoItBelongsTo = game.bank.name;
                game.bank.deposits.ElementAt(depositIndexForTheBank).actualInterestPerYear = defaultInterest;
                game.bank.deposits.ElementAt(depositIndexForTheBank).amountOfMoneyPutInDeposit = 0;
                game.bank.deposits.ElementAt(depositIndexForTheBank).whenWasItBought = DateTime.Parse("02/02/9999");
                game.bank.deposits.ElementAt(depositIndexForTheBank).whenItShouldBeReleased = DateTime.Parse("02/03/9999");

                player.depositsOwned.RemoveAt(whereIsThisDepositForThePlayer);

                UpdateDataForNextTurn(game, player, timeTrial);

                return;
            }

            else
            {
                Console.Clear();
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

                NextTurn(game, player, timeTrial);

                return;
            }
        }

        #endregion

        #endregion

        #region Updating all data to go to the next turn

        public static void UpdateDataForNextTurn(AGame game, Player player, TimeTrial timeTrial)
        {
            bool didBankWentBankrupt = false;

            #region Updating the deposits, the player and the bank accordingly

            double monthsOfGameTime = timeTrial.month + ((timeTrial.year - 1) * 12);

            if(player.depositsOwned.Count > 0)
            {
                List<Deposit> backUpPlayerDeposits = new List<Deposit>();
                foreach (Deposit playerDeposit in player.depositsOwned)
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

                            int whereIsThisDepositForThePlayer = player.depositsOwned.IndexOf(depositOfPlayer);

                            double theFinalInterest = depositOfPlayer.actualInterestPerYear;

                            for (int i = 1; i < depositOfPlayer.timeSpan; i++)
                            {
                                theFinalInterest = theFinalInterest * depositOfPlayer.actualInterestPerYear;
                            }

                            double bankAdditionalReturnMoneyToPlayer = depositOfPlayer.amountOfMoneyPutInDeposit * (theFinalInterest - 1);

                            if (game.bank.money > bankAdditionalReturnMoneyToPlayer)
                            {
                                player.savingsAviliabe =
                                player.savingsAviliabe + depositOfPlayer.amountOfMoneyPutInDeposit + bankAdditionalReturnMoneyToPlayer;

                                game.bank.money = game.bank.money - bankAdditionalReturnMoneyToPlayer;

                                game.bank.numOfDepositsAviliabe = game.bank.numOfDepositsAviliabe + 1;

                                Console.Clear();
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
                                player.savingsAviliabe = player.savingsAviliabe + depositOfPlayer.amountOfMoneyPutInDeposit + game.bank.money;
                                player.depositsOwned.RemoveAt(player.depositsOwned.IndexOf(depositOfPlayer));

                                #region What to do if the bank went bankrupt?

                                List<Deposit> playerDepositsToRemove = new List<Deposit>();

                                foreach (Deposit DepositUsedToBeInBank in game.bank.deposits)
                                {
                                    foreach (Deposit playerDeposit in player.depositsOwned)
                                    {
                                        if (DepositUsedToBeInBank.name == playerDeposit.name)
                                        {
                                            playerDepositsToRemove.Add(playerDeposit);
                                        }
                                    }
                                }

                                foreach (Deposit depositToRemove in playerDepositsToRemove)
                                {
                                    player.savingsAviliabe = player.savingsAviliabe + depositToRemove.amountOfMoneyPutInDeposit;
                                }

                                player.depositsOwned.RemoveRange(0, player.depositsOwned.Count);

                                Console.Clear();
                                Console.WriteLine();
                                Console.WriteLine(" == Notification ==");
                                Console.WriteLine();
                                Console.WriteLine("The bank has went bankrupt.");
                                Console.WriteLine();
                                Console.WriteLine($"When he released the deposit '{depositOfPlayer.name}',");
                                Console.WriteLine($"he was supposed to pay you {Math.Round(bankAdditionalReturnMoneyToPlayer)} dollars (in addition to the money you put in the deposit),");
                                Console.WriteLine($"but he only had {game.bank.money} dollars left to pay you.");
                                Console.WriteLine();
                                Console.WriteLine($"So you've lost {Math.Round(bankAdditionalReturnMoneyToPlayer) - (game.bank.money)} dollars you were supposed to earn for this deposit,");
                                Console.WriteLine($"and all your other deposits have been released automatically (without any interests)");
                                Console.WriteLine();
                                Console.WriteLine($"So now, you overall have {player.savingsAviliabe} uninvested dollars.");
                                Console.WriteLine();
                                Console.WriteLine($"The bank has been 're-created' automatically with the same status it had at the start of this game.");
                                Console.WriteLine();
                                Console.WriteLine("Enter anything to continue.");
                                Console.WriteLine();
                                Console.ReadLine();

                                List<AGame> games = WritingAllGamesInformation();

                                Bank newBank = ReturnBankToDefault(games, game.name);

                                game.bank = newBank;

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
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whoReleasedItLastTurn = player.name;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whoItBelongsTo = game.bank.name;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).actualInterestPerYear = defaultInterest;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).amountOfMoneyPutInDeposit = 0;
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whenWasItBought = DateTime.Parse("02/02/9999");
                                game.bank.deposits.ElementAt(depositIndexForTheBank).whenItShouldBeReleased = DateTime.Parse("02/03/9999");

                                player.depositsOwned.RemoveAt(whereIsThisDepositForThePlayer);
                            }

                            #endregion
                        }
                    }
                }
            }

            #endregion

            TheDataUpdatedWhateverDepositUpdateHappenedOrNot(
                player, game, timeTrial);

            return;
        }
        
        public static void TheDataUpdatedWhateverDepositUpdateHappenedOrNot(
            Player player, AGame game, TimeTrial timeTrial)
        {
            #region Is the game over or should it continue? (+ game time and player updates + game over events)

            if (Math.Round(player.savingsAviliabe + 1) >= game.moneyToEndGame)
            {
                string inputForHighScore = TimeTrialGameFinished(game, player, timeTrial);

                while(inputForHighScore.ToLower() != "y" && inputForHighScore.ToLower() != "n")
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Enter again:");
                    Console.WriteLine();
                    inputForHighScore = Console.ReadLine();
                }

                if(inputForHighScore.ToLower() == "y")
                {
                    EnteringScoreIntoHighScore(game, player, timeTrial);
                }

                if(inputForHighScore.ToLower() == "n")
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("Your score has (not) been submitted successfully.");
                    Console.WriteLine();
                }

                Console.WriteLine("Enter anything to continue.");
                Console.WriteLine("(Before we'll rush to the main meun, you may've unlocked tips/enrichements..)");
                Console.WriteLine();
                Console.ReadLine();

                UnlockingTipsAndEnRichements(game, player, timeTrial);

                return;
            }

            else
            {
                #region Updating game time

                if (timeTrial.month == 7)
                {
                    timeTrial.year = timeTrial.year + 1;
                    timeTrial.month = 1;
                }

                else if (timeTrial.month == 1)
                {
                    timeTrial.month = 7;
                }

                #endregion

                #region Updating the player with nothing released

                #region panick mode next turn or not?

                int countLongDeposits = 0;
                double sumOfInvestements = 0;

                if (player.depositsOwned.Count > 0)
                {
                    foreach (Deposit playerDeposit in player.depositsOwned)
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
                    isPlayerInvestingAlmostTooMuch = (sumOfInvestements / player.savingsAviliabe) >= (game.riskProfile - 0.05);
                }

                bool AreTheTermsTooLong =
                    countLongDeposits >= 2;

                player.InPanickMode = (AreTheTermsTooLong && isPlayerInvestingAlmostTooMuch);

                #endregion

                player.savingsAviliabe = player.savingsAviliabe + player.income;
                player.hasThePlayerFinishedHisTurn = true;

                #endregion

                if (Math.Round(player.savingsAviliabe + 1) >= game.moneyToEndGame)
                {
                    string inputForHighScore = TimeTrialGameFinished(game, player, timeTrial);

                    while (inputForHighScore.ToLower() != "y" && inputForHighScore.ToLower() != "n")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid input. Enter again:");
                        Console.WriteLine();
                        inputForHighScore = Console.ReadLine();
                    }

                    if (inputForHighScore.ToLower() == "y")
                    {
                        EnteringScoreIntoHighScore(game, player, timeTrial);
                    }

                    if (inputForHighScore.ToLower() == "n")
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("Your score has (not) been submitted successfully.");
                        Console.WriteLine();
                    }

                    Console.WriteLine("Enter anything to continue.");
                    Console.WriteLine("(Before we'll rush to the main meun, you may've unlocked tips/enrichements..)");
                    Console.WriteLine();
                    Console.ReadLine();

                    UnlockingTipsAndEnRichements(game, player, timeTrial);

                    return;
                }

                else
                {
                    NextTurn(game, player, timeTrial);

                    return;
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region what if the player is in panick mode?

        public static void PanickModeTurn(Player player, AGame game, TimeTrial timeTrial)
        {
            #region The actual action

            Deposit depositToRelease = WhichDepositToReleaseInPanicMode(player);

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($" == Your turn ==");
            Console.WriteLine();
            Console.WriteLine("In-game date:");
            Console.WriteLine($"year {timeTrial.year}, month {timeTrial.month}.");
            Console.WriteLine();
            Console.WriteLine($"Sum of money that isn't invested: {player.savingsAviliabe} dollars");
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

            if(ShouldIReturnToMeunByEndingFunction(input))
            {
                return;
            }

            while(input != "1" && input != "2")
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
                BankAnswerForReleaseRequest(player, game, timeTrial, depositToRelease);
            }

            else if (input == "2")
            {
                SavingProgress(game, player, null, timeTrial);

                NextTurn(game, player, timeTrial);
            }

            return;

            #endregion
        }

        #endregion

        #region What if the player won?

        #region Unlocking tips and enrichements

        public static void UnlockingTipsAndEnRichements(AGame game, Player player1,
            TimeTrial timeTrial = null)
        {
            XDocument theTips = new XDocument(XDocument.Load(@"DepositInvestingGame\Tips\Tips.xml"));
            XElement tipRoot = theTips.Root;
            IEnumerable<XElement> allTips = new List<XElement>(tipRoot.Elements("Screen"));
            XElement RecentTip = new XElement(allTips.First());
            XElement RecentTipPath = new XElement(RecentTip.Element("Path"));
            XElement tipUnlocked = new XElement(RecentTip.Element("Unlocked"));
            XElement recentTipNextScreen = new XElement(RecentTip.Element("NextScreen"));
            XElement recentTipPreviousScreen = new XElement(RecentTip.Element("PreviousScreen"));

            XDocument theEnrichements = new XDocument(XDocument.Load(@"DepositInvestingGame\Enrichement\Enrichement.xml"));
            XElement enrichementRoot = theEnrichements.Root;
            IEnumerable<XElement> allEnrichements = new List<XElement>(enrichementRoot.Elements("Screen"));
            XElement RecentEnrichement = new XElement(allEnrichements.First());
            XElement RecentEnrichementPath = new XElement(RecentEnrichement.Element("Path"));
            XElement enrichementUnlocked = new XElement(RecentEnrichement.Element("Unlocked"));
            XElement recentEnrichementNextScreen = new XElement(RecentEnrichement.Element("NextScreen"));
            XElement recentEnrichementPreviousScreen = new XElement(RecentEnrichement.Element("PreviousScreen"));

            List<XElement> allTheTips = new List<XElement>(allTips.ToList());

            #region organizing all newly unlocked stuff

            string[] UnlockedTipsAndEnrichementsPath = null;
            List<string> UnlockedTips = new List<string>();
            List<string> UnlockedEnrichements = new List<string>();

            double whenGameFinished = timeTrial.month + (timeTrial.year * 12);

            if (game.name == "Level 1")
            {
                double timeToUnlock = (12 * 3) + 1;
                double additionMoneyLeft = player1.savingsAviliabe - game.moneyToEndGame;

                if ((additionMoneyLeft > 0 && whenGameFinished == timeToUnlock)
                    || whenGameFinished < timeToUnlock)
                {
                    UnlockedTipsAndEnrichementsPath = File.ReadAllLines(@"DepositInvestingGame\Games\Unlocking\Level1Unlocking.txt");
                }
            }

            else if (game.name == "Level 2")
            {
                double timeToUnlock = (12 * 6) + 7;
                double additionMoneyLeft = player1.savingsAviliabe - game.moneyToEndGame;

                if ((additionMoneyLeft > 0 && whenGameFinished == timeToUnlock)
                    || whenGameFinished < timeToUnlock)
                {
                    UnlockedTipsAndEnrichementsPath = File.ReadAllLines(@"DepositInvestingGame\Games\Unlocking\Level2Unlocking.txt");
                }
            }

            else if (game.name == "Level 3")
            {
                double timeToUnlock = (12 * 10) + 7;

                double additionMoneyLeft = player1.savingsAviliabe - game.moneyToEndGame;

                if ((additionMoneyLeft >= 500 && whenGameFinished == timeToUnlock)
                    || whenGameFinished < timeToUnlock)
                {
                    UnlockedTipsAndEnrichementsPath = File.ReadAllLines(@"DepositInvestingGame\Games\Unlocking\Level3Unlocking.txt");
                }
            }

            else if (game.name == "More realistic level 3")
            {
                double timeToUnlock = (12 * 10) + 7;

                double additionMoneyLeft = player1.savingsAviliabe - game.moneyToEndGame;

                if ((additionMoneyLeft >= 500 && whenGameFinished == timeToUnlock)
                    || whenGameFinished < timeToUnlock)
                {
                    UnlockedTipsAndEnrichementsPath = File.ReadAllLines(@"DepositInvestingGame\Games\Unlocking\MoreReleasticLevel3Unlocking.txt");
                }
            }

            if (UnlockedTipsAndEnrichementsPath != null)
            {
                List<string> UnlockedTipsAndEnrichementsPathList = new List<string>();

                foreach (string line in UnlockedTipsAndEnrichementsPath)
                {
                    UnlockedTipsAndEnrichementsPathList.Add(line);
                }

                int tipIndex = UnlockedTipsAndEnrichementsPathList.IndexOf("Tips:");

                int tipCount = 1;

                while (UnlockedTipsAndEnrichementsPathList.ElementAt(tipIndex + tipCount) != "")
                {
                    UnlockedTips.Add(UnlockedTipsAndEnrichementsPathList.ElementAt(tipIndex + tipCount));

                    tipCount++;
                }

                int enrichementIndex = UnlockedTipsAndEnrichementsPathList.IndexOf("Enrichements:");

                int enrichementCount = 1;

                while (UnlockedTipsAndEnrichementsPathList.ElementAt(enrichementIndex + enrichementCount) != "")
                {
                    UnlockedEnrichements.Add(UnlockedTipsAndEnrichementsPathList.ElementAt(enrichementIndex + enrichementCount));

                    enrichementCount++;
                }
            }

            #endregion

            if (UnlockedTips.Count > 0)
            {
                foreach (XElement tip in allTips)
                {
                    XElement currentTipUnlocked = new XElement(tip.Element("Unlocked"));

                    if (!bool.Parse(currentTipUnlocked.Value))
                    {
                        RecentTipPath = new XElement(tip.Element("Path"));
                        recentTipNextScreen = new XElement(tip.Element("NextScreen"));
                        recentTipPreviousScreen = new XElement(tip.Element("PreviousScreen"));

                        foreach (string tipToUnlock in UnlockedTips)
                        {
                            if (RecentTipPath.Value == tipToUnlock)
                            {
                                changeUnlockTipOrEnrichementInXML(theTips, tipRoot, allTips, RecentTipPath,
                            recentTipNextScreen, recentTipPreviousScreen, currentTipUnlocked,
                            tip, @"DepositInvestingGame\Tips\Tips.xml");

                                string basePath = @"DepositInvestingGame\Tips\";
                                int baseForMathCharCount = (basePath.Count());

                                string tipFileFromFilePath = RecentTipPath.Value.Remove(0, baseForMathCharCount);
                                string theTipName = tipFileFromFilePath.Remove(tipFileFromFilePath.IndexOf("."), 4);

                                MessagesPopUpWhenAPlayerUnlocksTipOrEnrichement("tip", RecentTipPath.Value.ToString(), theTipName);
                            }
                        }
                    }
                }
            }

            if (UnlockedEnrichements.Count > 0)
            {
                foreach (XElement enrichement in allEnrichements)
                {
                    XElement currentEnrichementUnlocked = new XElement(enrichement.Element("Unlocked"));

                    if (!bool.Parse(currentEnrichementUnlocked.Value))
                    {
                        RecentEnrichementPath = new XElement(enrichement.Element("Path"));
                        recentEnrichementNextScreen = new XElement(enrichement.Element("NextScreen"));
                        recentEnrichementPreviousScreen = new XElement(enrichement.Element("PreviousScreen"));

                        foreach (string enrichementToUnlock in UnlockedEnrichements)
                        {
                            if (RecentEnrichementPath.Value == enrichementToUnlock)
                            {
                                changeUnlockTipOrEnrichementInXML(theEnrichements, enrichementRoot,
                            allEnrichements, RecentEnrichementPath, recentEnrichementNextScreen,
                            recentEnrichementPreviousScreen, currentEnrichementUnlocked,
                            enrichement, @"DepositInvestingGame\Enrichement\Enrichement.xml");

                                string basePath = @"DepositInvestingGame\Enrichement\";
                                int baseForMathCharCount = (basePath.Count());

                                string enrichementFileFromFilePath = RecentEnrichementPath.Value.Remove(0, baseForMathCharCount);
                                string theEnrichementName = enrichementFileFromFilePath.Remove(enrichementFileFromFilePath.IndexOf("."), 4);

                                MessagesPopUpWhenAPlayerUnlocksTipOrEnrichement("enrichment", RecentEnrichementPath.Value.ToString(), theEnrichementName);
                            }
                        }
                    }
                }
            }

        }
        
        #endregion

        public static string TimeTrialGameFinished(AGame game, Player player, TimeTrial timeTrial)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(" == GAME FINISHED! ==");
            Console.WriteLine();
            Console.WriteLine($"Congrats, you ***!! (I mean, '{player.name}'..)");
            Console.WriteLine($"you have at least {game.moneyToEndGame} dollars, and thus have finished a time trial at '{game.name}'..");
            Console.WriteLine($"(Reminder: The above calculatin does NOT take into account money you have invested in deposits)");
            Console.WriteLine();
            Console.WriteLine($"You finished the game at: {timeTrial.year} years and {timeTrial.month} months in game time.");
            Console.WriteLine($"And to top it off, you have {player.savingsAviliabe} uninvested dollars,");
            Console.WriteLine($"Which means you surpassed the amount of money needed to finish ({game.moneyToEndGame} dollars) by {Math.Round(player.savingsAviliabe - game.moneyToEndGame)} dollars.");
            Console.WriteLine();
            Console.WriteLine("Do you wish to save your score into the high score chart of this game?");
            Console.WriteLine();
            Console.WriteLine("Enter 'y' for yes, or 'n' for no.");
            Console.WriteLine();

            string input = Console.ReadLine();
            return input;
        }

        #endregion

        #region Properties

        public double month { get; set; }
        public double year { get; set; }

        #endregion
    }
}
