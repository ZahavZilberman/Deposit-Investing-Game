using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class Deposit
    {
        #region ctor

        public Deposit(string theName, double timeOfStayingLocked, string owner, 
            double lowestInterestPerYear, double minimumMoney,
            double exitPointPerWhatMonths)
        {
            name = theName;
            timeSpan = timeOfStayingLocked;
            whoItBelongsTo = owner;
            whenWasItBought = new DateTime();
            whenWasItBought = DateTime.Parse("02/02/9999");
            whenItShouldBeReleased = new DateTime();
            whenItShouldBeReleased = DateTime.Parse("02/03/9999");
            DefaultinterestPerYear = lowestInterestPerYear;
            actualInterestPerYear = lowestInterestPerYear;
            wasItReleasedInLastTurn = false;
            amountOfMoneyPutInDeposit = 0;
            minimumMoneyForDeposit = minimumMoney;
            exitPointsGap = exitPointPerWhatMonths;

            allPossibleExitPoints = new List<DateTime>();
        }

        #endregion

        #region Properties

        public string name { get; set; }

        public double timeSpan { get; set; }

        public string whoItBelongsTo { get; set; }

        public DateTime whenWasItBought { get; set; }

        public DateTime whenItShouldBeReleased { get; set; }

        public bool wasItReleasedInLastTurn { get; set; }

        public string whoReleasedItLastTurn { get; set; }

        public double amountOfMoneyPutInDeposit { get; set; }

        public double DefaultinterestPerYear { get; set; }

        public double actualInterestPerYear { get; set; }

        public double minimumMoneyForDeposit { get; set; }

        public double exitPointsGap { get; set; }

        public List<DateTime> allPossibleExitPoints { get; set; }

        #endregion
    }
}
