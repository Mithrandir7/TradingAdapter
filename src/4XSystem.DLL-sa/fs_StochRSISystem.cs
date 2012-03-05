
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{
	public class fStochRSISystem : ForexObject
	{
		int RSIPeriods;
		int Minutes;

		iStochRSI StochRSI;
		iSlope    RSISlope;
		cCandleBuilder cbx;

		int ExecutionCount;

		bool TicksLoaded;
		System.Collections.ArrayList TickBackup;
		System.Collections.ArrayList Ticks1,Ticks2;

		double spread;
		cCandleBuilder cb10;

		public fStochRSISystem()
		{
			TicksLoaded = false;
			ExecutionCount = 0; // only set up when object is created
		}

		private void LoadTicks()
		{
		}

		private void SelectTicks()
		{
		}

		public override void Init(string pParameters)
		{
			InitializeParameters(pParameters,"SRSI:14;MINUTES:5;");			

			RSIPeriods = (int)PParser.GetDouble("SRSI",0);
			StochRSI = new iStochRSI(RSIPeriods);
			RSISlope = new iSlope();

			Minutes = (int)PParser.GetDouble("MINUTES",0);
			cbx = new cCandleBuilder(Minutes,10);
			cbx.RegisterCandleListener("cbx",this);
			Framework.TickServer.RegisterTickListener("cbx","*",cbx);

			Framework.TickServer.RegisterTickListener("System","*",this);

			Framework.WriteGraphLine("StochRSI,RSISlope,InTrade,Margin");


			// this is optional
			// demonstrates we have complete control over ticks served
			
			LoadTicks();		// only if specified by parameters FILE1 and FILE2
			SelectTicks();
			
			// this is optional
			// demonstrates registration of multiple period candles

			cb10 = new cCandleBuilder(10,4);
			cb10.RegisterCandleListener("cb10",this);
			Framework.TickServer.RegisterTickListener("cb10","*",cb10);

		}

		// called from TickServer
		public override void ReceiveTick(sTick pTick)
		{
			spread = pTick.Ask - pTick.Bid;
			//cb10.ReceiveTick(pTick);	// register with TickServer or call here
		}

		// called from CandleBuilder
		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
			if ((pPeriod==Minutes) && (pCBTitle=="cbx"))
			{
				StochRSI.ReceiveTick(pCandle.C);
				RSISlope.ReceiveTick(StochRSI.Value());
				DecisionFunction();
			}
		}

		private void DecisionFunction()
		{
			switch (Framework.Account.InTrade())
			{
				case 0:
				{
					if (Framework.Account.InTrade()==0)
					{
						EntryCriteriaLong();
					}

					if (Framework.Account.InTrade()==0)
					{
						EntryCriteriaShort();
					}

					break;
				}

				case 1:
				{
					ExitCriteriaLong();
					break;
				}

				case -1:
				{
					ExitCriteriaShort();
					break;
				}
			}

			double logRSI     = StochRSI.Value();
			double logSlope   = RSISlope.Value();
			double logInTrade = Framework.Account.InTrade();
			double logMargin  = Framework.Account.GetAccount().C;

			Framework.WriteGraphLine(logRSI+","+logSlope+","+logInTrade+","+logMargin);

		}

		private void EntryCriteriaLong()
		{
			if ((StochRSI.Value()<25) && (RSISlope.Value()>0))
			{
				Framework.Account.EnterTrade(1);
			}
		}

		private void EntryCriteriaShort()
		{
			if ((StochRSI.Value()>75) && (RSISlope.Value()<0))
			{
				Framework.Account.EnterTrade(-1);
			}
		}

		private void ExitCriteriaLong()
		{
			if (StochRSI.Value()>75)
			{
				Framework.Account.ExitTrade();
			}
		}

		private void ExitCriteriaShort()
		{
			if (StochRSI.Value()<25)
			{
				Framework.Account.ExitTrade();
			}
		}

		public override bool Finished()
		{
			bool returnv = true;

			return returnv;
		}

		public override string Title()
		{
			return "StochRSI System";
		}
	}

		
}