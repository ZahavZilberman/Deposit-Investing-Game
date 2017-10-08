using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class AGame
    {
        #region ctor

        public AGame(double risk, double moneyToEnd, Bank Abank,
            double startMoney, string Name, double income)
        {
            riskProfile = risk;
            moneyToEndGame = moneyToEnd;
            bank = Abank;
            moneyToStartWith = startMoney;
            name = Name;
            playersIncome = income;
        }

        #endregion

        #region Properties

        public string name { get; set; }

        public double riskProfile { get; set; }

        public double moneyToEndGame { get; set; }

        public Bank bank { get; set; }

        public double moneyToStartWith { get; set; }

        public double playersIncome { get; set; }

        #endregion
    }
}
