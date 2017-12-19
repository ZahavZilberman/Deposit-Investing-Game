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

        public BookScroll(XDocument database, string mode)
        {
            DataBase = new XDocument(database);
            Screens = new List<XElement>(database.Root.Elements("Screen"));
            RecentScreen = new XElement(Screens.First());
            Path = new XElement(RecentScreen.Element("Path"));
            UnlockedScreens = new List<XElement>();

            if(mode.ToLower() == "tip" || mode.ToLower() == "enrichment")
            {
                UnlockedScreens =
                    Screens.Where(screen => bool.Parse(screen.Element("Unlocked").Value) == true);

                if (UnlockedScreens.Count() == 0)
                {
                    Console.Clear();

                    Console.WriteLine();
                    Console.WriteLine($"You haven't unlocked any {mode}s yet!");

                    Console.WriteLine();
                    Console.WriteLine("Enter anything to return to the main meun");
                    Console.WriteLine();
                    Console.ReadLine();

                    return;
                }

                RecentScreen = new XElement(UnlockedScreens.First());
                Path = new XElement(RecentScreen.Element("Path"));
            }
        }

        #endregion

        #region The actual function

        public void next(string mode)
        {
            if (UnlockedScreens.Count() == 0 && (mode.ToLower() == "tip" || mode.ToLower() == "enrichment"))
            {
                return;
            }

            Console.Clear();

            WritingText(Path.Value);
            Console.WriteLine();
            Console.WriteLine($"enter 'n' to go to the next {mode}, or 'p' to go to the previous {mode}.");
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

                    if (mode.ToLower() == "manual")
                    {
                        Path = RecentScreen.Element("PreviousScreen");
                    }

                    else if (mode == "tip" || mode == "enrichment")
                    {
                        int index = IndexOfCurrentScreen(UnlockedScreens, RecentScreen);

                        if (index > 0)
                        {
                            RecentScreen = UnlockedScreens.ElementAt(index - 1);
                            Path = RecentScreen.Element("Path");
                        }

                        else
                        {
                            return;
                        }
                    }

                    break;

                case "n":

                    if (mode.ToLower() == "manual")
                    {
                        Path = RecentScreen.Element("NextScreen");
                    }

                    else if (mode == "tip" || mode == "enrichment")
                    {
                        int index = IndexOfCurrentScreen(UnlockedScreens, RecentScreen);

                        if (index + 1 == UnlockedScreens.Count())
                        {
                            return;
                        }

                        else
                        {
                            RecentScreen = UnlockedScreens.ElementAt(index + 1);
                            Path = RecentScreen.Element("Path");
                        }
                    }

                    break;
            }

            if (string.IsNullOrEmpty(Path.Value)) // this is after the "m" input because path needs to update in the switch
            {
                return;
            }

            if (mode == "manual")
            {
                foreach (var screen in Screens)
                {
                    if (screen.Element("Path").Value == Path.Value)
                    {
                        RecentScreen = screen;
                    }
                }
            }

            next(mode);
        }

        #endregion

        #region Returning the index of the current screen

        private static int IndexOfCurrentScreen(IEnumerable<XElement> unlockedScreens, XElement recentScreen)
        {
            int index = 0;

            foreach (XElement screen in unlockedScreens)
            {
                if (screen.Value == recentScreen.Value)
                {
                    index = unlockedScreens.ToList().IndexOf(screen);
                }
            }

            return index;
        }

        #endregion

        #region Properties

        private XDocument DataBase { get; set; }
        private IEnumerable<XElement> Screens { get; set; }
        private XElement Path { get; set; }
        private XElement RecentScreen { get; set; }
        private IEnumerable<XElement> UnlockedScreens { get; set; }

        #endregion
    }
}
