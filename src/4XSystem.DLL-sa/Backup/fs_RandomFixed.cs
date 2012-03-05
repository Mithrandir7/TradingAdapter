
//
// 4X Lab.NET � Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fRandomEntryFixedProfit : ForexObject
	{
		double iTargetProfit;
		double iStopLoss;

		int Minutes;

		Random RNG;
		int randomint;

		sCandle Candle;
		double spread;
		cCandleBuilder cbx;

		public fRandomEntryFixedProfit()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"MINUTES:5;STOP:0.0003;PROFIT:3;");

			Minutes = (int)PParser.GetDouble("MINUTES",0);
			iStopLoss = PParser.GetDouble("STOP",0);
			iTargetProfit = PParser.GetDouble("PROFIT",0) * iStopLoss;
			RNG = new Random();

			cbx = new cCandleBuilder(Minutes,10);			
			Framework.TickServer.RegisterTickListener("cbx","*",cbx);
			cbx.RegisterCandleListener("cbx",this);
			Framework.TickServer.RegisterTickListener("System","*",this);

			Framework.WriteGraphLine("InTrade,Margin,C");	
		}

		public override void ReceiveTick(sTick pTick)
		{
			spread = pTick.Ask - pTick.Bid;
		}

		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
			if ((pPeriod==Minutes) && (pCBTitle=="cbx"))
			{
				Candle = pCandle;

				DecisionFunction();
			}
		}

		
		private void DecisionFunction()
		{

			randomint = (int)(RNG.NextDouble()*1000);

			switch (Framework.Account.InTrade())
			{
				case 0:
				{
					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaLong())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetStop(iStopLoss);
							Framework.Account.SetTakeProfit(iTargetProfit);
						}
					}

					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaShort())
						{
							Framework.Account.EnterTrade(-1);
							Framework.Account.SetStop(iStopLoss);
							Framework.Account.SetTakeProfit(iTargetProfit);
						}
					}
					break;
				}

				case 1:
				{
					if (ExitCriteriaLong())
					{
						//Framework.Account.ExitTrade();
					}
					break;
				}

				case -1:
				{
					if (ExitCriteriaShort())
					{
						//Framework.Account.ExitTrade();
					}
					break;
				}
			}

			double logInTrade = Framework.Account.InTrade();
			double logMargin  = Framework.Account.GetAccount().C;

			//Framework.WriteGraphLine("InTrade,Margin,C");	
			Framework.WriteGraphLine(logInTrade+","+logMargin+","+Candle.C);
		}

		private bool EntryCriteriaLong()
		{
			bool v = false;

			if (randomint >= 500)
			{
				v = true;
			}

			return v;
		}

		private bool EntryCriteriaShort()
		{
			bool v = false;

			if (randomint <= 500)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaLong()
		{
			bool v = false;

			if (v)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaShort()
		{
			bool v = false;

			if (v)
			{
				v = true;
			}

			return v;
		}

		public override bool Finished()
		{
			bool returnv = true;

			return returnv;
		}

		public override string Title()
		{
			return "Random Entries Fixed Profit/Loss";
		}
	}
}