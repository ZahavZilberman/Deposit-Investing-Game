using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class Bank
    {
        #region ctor

        public Bank(List<Deposit> bankDeposits, double bankStartMoney, string bankName)
        {
            deposits = new List<Deposit>();
            foreach(Deposit deposit in bankDeposits)
            {
                deposits.Add(deposit);
            }
            startGameMoney = bankStartMoney;
            money = bankStartMoney;
            numOfDeposits = deposits.Count;
            numOfDepositsAviliabe = deposits.Count;
            isBankrupt = false;
            name = bankName;
        }

        #endregion

        #region Properties

        public List<Deposit> deposits { get; set; }

        public string name { get; set; }

        public double startGameMoney { get; set; }

        public double money { get; set; } // this and "startGameMoney" are disincluding the deposits

        public int numOfDeposits { get; set; }

        public int numOfDepositsAviliabe { get; set; }

        public bool isBankrupt { get; set; }

        #endregion
    }
}
