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

        public AGame(string docGamePath)
        {
            XDocument docGame = new XDocument(XDocument.Load(docGamePath));
            XElement root = new XElement(docGame.Root);

            XElement docGameName = new XElement(root.Element("Name"));
            XElement docRiskProfile = new XElement(root.Element("RiskProfile"));
            XElement docMoneyToEnd = new XElement(root.Element("MoneyToEnd"));
            XElement docGameBank = new XElement(root.Element("BankName"));
            XElement docPlayerStartMoney = new XElement(root.Element("StartMoney"));
            XElement docGameIncome = new XElement(root.Element("Income"));

            XElement docBank = new XElement(root.Element("Bank"));
            XElement docBankStartMoney = new XElement(docBank.Element("StartBankMoney"));

            IEnumerable<XElement> docDeposits = new List<XElement>(root.Elements("Deposit"));
            XElement docRecentDeposit = new XElement(docDeposits.First());
            XElement docDepositName = new XElement(docRecentDeposit.Element("Name"));
            XElement docDepositTimeSpan = new XElement(docRecentDeposit.Element("TimeSpan"));
            XElement docDefaultInterest = new XElement(docRecentDeposit.Element("DefaultInterestPerYear"));
            XElement docMinimumMoneyForDeposit = new XElement(docRecentDeposit.Element("MinimumMoney"));
            XElement docExitPointsGap = new XElement(docRecentDeposit.Element("ExitPointsGap"));
            // the 2 elements above is worth checking
            string gameBankName = docGameBank.Value;
            List<Deposit> gameDeposits = new List<Deposit>();

            for(int depositNum = 0; depositNum < docDeposits.Count(); depositNum++)
            {
                docRecentDeposit = new XElement(docDeposits.ElementAt(depositNum));
                docDepositName = new XElement(docRecentDeposit.Element("Name"));
                docDepositTimeSpan = new XElement(docRecentDeposit.Element("TimeSpan"));
                docDefaultInterest = new XElement(docRecentDeposit.Element("DefaultInterestPerYear"));
                docMinimumMoneyForDeposit = new XElement(docRecentDeposit.Element("MinimumMoney"));
                docExitPointsGap = new XElement(docRecentDeposit.Element("ExitPointsGap"));

                string depositName = docDepositName.Value;
                double depositTimeSpan = double.Parse(docDepositTimeSpan.Value);
                double depositDefaultInterest = double.Parse(docDefaultInterest.Value);
                double minimumMoneyForDeposit = double.Parse(docMinimumMoneyForDeposit.Value);
                double theGapBetweenExitPoints = double.Parse(docExitPointsGap.Value);

                Deposit aDeposit = new Deposit(depositName, depositTimeSpan, gameBankName,
                    depositDefaultInterest, minimumMoneyForDeposit,
                    theGapBetweenExitPoints);

                gameDeposits.Add(aDeposit);
            }

            double gameBankStartMoney = double.Parse(docBankStartMoney.Value);
            Bank gameBank = new Bank(gameDeposits, gameBankStartMoney, gameBankName);

            string gameName = docGameName.Value;
            double gameRiskProfile = double.Parse(docRiskProfile.Value);
            double gameMoneyToEnd = double.Parse(docMoneyToEnd.Value);
            double gamePlayerStartMoney = double.Parse(docPlayerStartMoney.Value);
            double gamePlayersIncome = double.Parse(docGameIncome.Value);

            name = gameName;
            riskProfile = gameRiskProfile;
            moneyToEndGame = gameMoneyToEnd;
            bank = gameBank;
            moneyToStartWith = gamePlayerStartMoney;
            playersIncome = gamePlayersIncome;
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