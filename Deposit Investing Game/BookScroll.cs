using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class BookScroll : Program
    {
        #region ctor

        public BookScroll(XDocument database)
        {
            DataBase = new XDocument(database);
            Screens = new List<XElement>(database.Root.Elements("Screen"));
            RecentScreen = new XElement(Screens.First());
            Path = new XElement(RecentScreen.Element("Path"));
        }

        #endregion

        #region The actual function

        public void next(string mode)
        {

            #region assigning locked/unlocked values

            if (mode.ToLower() == "manual")
            {
                isUnlocked = true;
            }
            else if(mode.ToLower() == "tip" || mode.ToLower() == "enrichement")
            {
                Path = new XElement(RecentScreen.Element("Path"));
                XElement isLocked = new XElement(RecentScreen.Element("Unlocked"));
                isUnlocked = bool.Parse(isLocked.Value);
            }

            #endregion

            Console.Clear();

            if (isUnlocked)
            {
                WritingText(Path.Value);
                Console.WriteLine();
                Console.WriteLine("enter 'n' to go to the next screen, or 'p' to go to the previous screen.");
                Console.WriteLine();
                Console.WriteLine("(Those are shortcuts for 'next'[n] and 'previous'[p], by the way)");
                Console.WriteLine();
                MainMeunMessage();
                Console.WriteLine();
                string input = Console.ReadLine();

                while (input.ToLower() != "n" && input.ToLower() != "p" && input.ToLower() != "m")
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Enter again:");
                    Console.WriteLine();
                    input = Console.ReadLine();
                }

                if (ShouldIReturnToMeunByEndingFunction(input))
                {
                    return;
                }

                switch (input.ToLower())
                {
                    case "p":
                        Path = RecentScreen.Element("PreviousScreen");
                        break;
                    case "n":
                        Path = RecentScreen.Element("NextScreen");
                        break;
                }

                if(string.IsNullOrEmpty(Path.Value)) // this is after the "m" input because path needs to update in the switch
                {
                    return;
                }

                foreach (var screen in Screens)
                {
                    if (screen.Element("Path").Value == Path.Value)
                    {
                        RecentScreen = screen;
                    }
                }

                next(mode);
            }

            #region what if the tip enrichment is locked?

            else
            {

                if (Screens.ElementAt(0).Value == RecentScreen.Value)
                {
                    Console.WriteLine();
                    Console.WriteLine($"You haven't unlocked any {mode}s yet!");
                }

                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"You haven't unlocked this {mode} yet!");
                    Console.WriteLine();
                    Console.WriteLine($"Which means you also haven't unlocked any {mode} that comes after it..");
                }

                Console.WriteLine();
                Console.WriteLine("Enter anything to return to the main meun");
                Console.WriteLine();
                Console.ReadLine();
                return;
            }

            #endregion
        }

        #endregion

        #region Properties

        private XDocument DataBase { get; set; }
        private IEnumerable<XElement> Screens { get; set; }
        private XElement Path { get; set; }
        private XElement RecentScreen { get; set; }
        private bool isUnlocked { get; set; }

        #endregion
    }
}