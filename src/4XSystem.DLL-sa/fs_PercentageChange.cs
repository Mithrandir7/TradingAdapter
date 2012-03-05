
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	// add bollinger bands and try to express the percentage of change
	// based on volatility

	public class fPercentageChange : ForexObject
	{
		double iPercentage;
		double iMin;
		double iMax;

		bool   FirstTick;
		sTick  Tick;
		double Spread;

		public fPercentageChange()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"PERCENT:0.001;");

			iPercentage = PParser.GetDouble("PERCENT",0);

			Framework.TickServer.RegisterTickListener("System","*",this);

			Framework.WriteGraphLine("InTrade,Margin,Tick,Min,MinTh,Max,MaxTh");

			FirstTick = true;
		}

		public override void ReceiveTick(sTick pTick)
		{
			Spread	= pTick.Ask - pTick.Bid;
			Tick	= pTick;

			DecisionFunction();
		}

		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
		}

		
		private void DecisionFunction()
		{

			if (FirstTick)
			{
				FirstTick = false;
				iMin = Tick.Bid;
				iMax = Tick.Bid;
			}
			else
			{
				if (Framework.Account.InTrade()==1)
				{
					if (Tick.Bid > iMax)
					{
						iMax = Tick.Bid;
					}
					
					if ((iMax-Tick.Bid) > iPercentage*iMax)
					{
						Framework.Account.ExitTrade();
						Framework.Account.EnterTrade(-1);
						iMin = Tick.Bid;
						iMax = 0;
					}
				}

				if (Framework.Account.InTrade()==-1)
				{
					if (Tick.Bid < iMin)
					{
						iMin = Tick.Bid;
					}
					
					if ((Tick.Bid-iMin) > iPercentage*iMin)
					{
						Framework.Account.ExitTrade();
						Framework.Account.EnterTrade(1);
						iMax = Tick.Bid;
						iMin = 0;
					}
				}

				if (Framework.Account.InTrade()==0)
				{

					if ((Tick.Bid-iMin) > iPercentage*iMin)
					{
						Framework.Account.ExitTrade();
						Framework.Account.EnterTrade(1);
						iMax = Tick.Bid;
						iMin = 0;
					}

					if ((iMax-Tick.Bid) > iPercentage*iMax)
					{
						Framework.Account.ExitTrade();
						Framework.Account.EnterTrade(-1);
						iMin = Tick.Bid;
						iMax = 0;
					}

				}

			}

			double logInTrade = Framework.Account.InTrade();
			double logMargin  = Framework.Account.GetAccount().C;

			double MinTh = iMin + iMin*iPercentage;
			double MaxTh = iMax - iMax*iPercentage;

			Framework.WriteGraphLine(logInTrade+","+logMargin+","+Tick.Bid+","+iMin+","+MinTh+","+iMax+","+MaxTh);
		}

		public override bool Finished()
		{
			bool returnv = true;

			return returnv;
		}

		public override string Title()
		{
			return "% Trend Change";
		}
	}

}