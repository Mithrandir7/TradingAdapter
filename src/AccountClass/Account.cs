using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountClass
{
    public class Account
    {
        private List<Position> positions = new List<Position>();

        public string account;
        public string time;
        public string broker;        
        public string preBalance;
        public string depositWithraw;
        public string commission;
        public string closeProfit;
        public string balance;
        public string positionProfit;
        public string currMargin;
        public string Available;
        public string FrozenMargin;

        public void addPosition(Position aPosition)
        {
            foreach (Position pos in positions)
            {
                if (aPosition.Equals(pos))
                {
                    return;
                }
            }
            positions.Add(aPosition);
        }

        public void clearPosition()
        {
            positions.Clear();
        }

        public List<Position> getPositions()
        {
            return positions;
        }

        public String getInfo()
        {
            string rtnStr = time + "/" + broker + "/" + account + "/" + preBalance + "/" + depositWithraw + "/" +
                commission + "/" + closeProfit + "/" + balance + "/" + positionProfit + "/" +
                currMargin + "/" + Available + "/" + FrozenMargin + "\n";
            return (rtnStr);
        }
    }
}
