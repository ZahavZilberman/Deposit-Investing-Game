using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class HighScore : Program
    {
        #region ctor

        public HighScore(AGame game)
        {
            DataBase = new XDocument(XDocument.Load(@"DepositInvestingGame\HighScore.xml"));
            games = new List<XElement>(DataBase.Root.Elements("Game"));

            #region looking for the right game and intalizing values

            foreach(XElement Agame in games)
            {
                if(Agame.Element("GameName").Value.ToLower() == game.name.ToLower())
                {
                    GameName = new XElement(Agame.Element("GameName"));
                    NumOfRecords = new XElement(Agame.Element("NumOfRecords"));

                    if(int.Parse(NumOfRecords.Value) > 0)
                    {
                        Record = new XElement(Agame.Element("RecordsInGameTime"));
                        records = new List<XElement>(Record.Elements("Record"));
                    }
                }
            }

            #endregion
        }

        #endregion

        #region the actual function

        public void next(AGame game)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.WriteLine();

            #region What if there are no records?

            if (int.Parse(NumOfRecords.Value) == 0)
            {
                Console.WriteLine("There are no records for this game.");
                Console.WriteLine("Enter anything to return to the main meun.");
                Console.WriteLine();
                Console.ReadLine();
                return;
            }

            #endregion

            #region and if not?

            else
            {

                #region Game details

                ViewGameDetailsInAllModesSoFar(game);

                #endregion

                #region writing the records

                Console.WriteLine();
                Console.WriteLine("Records for this game:");
                Console.WriteLine();
                Console.WriteLine();

                for(int recordNum = 0; recordNum < records.Count(); recordNum++)
                {
                    mode = new XElement(records.ElementAt(recordNum).Element("Mode"));
                    month = new XElement(records.ElementAt(recordNum).Element("Month"));
                    year = new XElement(records.ElementAt(recordNum).Element("Year"));
                    player = new XElement(records.ElementAt(recordNum).Element("Player"));
                    additionalMoney = new XElement(records.ElementAt(recordNum).Element("Money"));

                    Console.WriteLine();
                    Console.WriteLine($"{recordNum + 1}.");
                    Console.WriteLine();
                    Console.WriteLine($"Mode: {mode.Value}");
                    Console.WriteLine($"By player: '{player.Value}'");
                    Console.WriteLine($"Won at: year {year.Value}, month {month.Value}");
                    Console.WriteLine($"Additional unneccesary money left: {additionalMoney.Value} dollars.");
                    Console.WriteLine();
                }

                #endregion

                #region the end where there ARE records

                Console.WriteLine();
                Console.WriteLine("Enter 'd' to view all the deposits this game's bank has at the start of this game,");
                Console.WriteLine("Or enter anything else to return to the main meun");
                Console.WriteLine();

                string input = Console.ReadLine();

                if (input.ToLower() == "d")
                {
                    ViewDeposits(game.bank, "high score", game);

                    next(game);
                }
                else
                {
                    return;
                }

                #endregion

            }

            #endregion
        }

        #endregion

        #region Properties

        XDocument DataBase { get; set; }

        IEnumerable<XElement> games { get; set; }

        XElement GameName { get; set; }

        XElement NumOfRecords { get; set; }

        XElement Record { get; set; }

        IEnumerable<XElement> records { get; set; }

        XElement month { get; set; }

        XElement year { get; set; }

        XElement player { get; set; }

        XElement mode { get; set; }

        XElement additionalMoney { get; set; }

        #endregion
    }
}