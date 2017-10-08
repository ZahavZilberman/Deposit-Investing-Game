using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace Deposit_Investing_Game
{
    public class Player
    {
        #region ctor

        public Player(AGame game, string Name)
        {
            name = Name;
            income = game.playersIncome;
            savingsAviliabe = game.moneyToStartWith;
            depositsOwned = new List<Deposit>();
            hasThePlayerFinishedHisTurn = false;
            InPanickMode = false;
            RowOfChoosingToDoNothing = 0;
        }

        #endregion

        #region Properties

        public string name { get; set; }

        public double income { get; set; }

        public double savingsAviliabe { get; set; }

        public List<Deposit> depositsOwned { get; set; }

        public bool hasThePlayerFinishedHisTurn { get; set; }

        public bool InPanickMode { get; set; }

        public int RowOfChoosingToDoNothing { get; set; }

        #endregion
    }
}