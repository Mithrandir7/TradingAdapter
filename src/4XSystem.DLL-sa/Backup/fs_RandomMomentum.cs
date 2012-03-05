
//
// 4X Lab.NET � Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fRandomEntryMomentumExit : ForexObject
	{
		int Minutes;

		Random RNG;
		int randomint;

		sCandle OldCandle,NewCandle;

		double spread;
		cCandleBuilder cbx;

		public fRandomEntryMomentumExit()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"MINUTES:5;");

			Minutes = (int)PParser.GetDouble("MINUTES",0);
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
				OldCandle = NewCandle;
				NewCandle = pCandle;

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
						}
					}

					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaShort())
						{
							Framework.Account.EnterTrade(-1);
						}
					}
					break;
				}

				case 1:
				{
					if (ExitCriteriaLong())
					{
						Framework.Account.ExitTrade();
					}
					break;
				}

				case -1:
				{
					if (ExitCriteriaShort())
					{
						Framework.Account.ExitTrade();
					}
					break;
				}
			}

			double logInTrade = Framework.Account.InTrade();
			double logMargin  = Framework.Account.GetAccount().C;

			//Framework.WriteGraphLine("InTrade,Margin,C");	
			Framework.WriteGraphLine(logInTrade+","+logMargin+","+NewCandle.C);
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

			if (NewCandle.C < OldCandle.C)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaShort()
		{
			bool v = false;

			if (NewCandle.C > OldCandle.C)
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
			return "Random Entries Momentum Exit";
		}
	}
}