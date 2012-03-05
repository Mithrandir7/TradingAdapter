
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fXChannelBreakout : ForexObject
	{
		int Minutes;
		int channel;
		double trailstop;

		iMinMax minmax;

		sCandle Candle;
		double spread;
		cCandleBuilder cbx;

		public fXChannelBreakout()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"MINUTES:5;CHANNEL:25;TRAILSTOP:0.0010;");

			Minutes = (int)PParser.GetDouble("MINUTES",0);
			channel = (int)PParser.GetDouble("CHANNEL",0);
			trailstop = PParser.GetDouble("TRAILSTOP",0);

			minmax = new iMinMax(2*channel);

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

				if (minmax.isPrimed())
				{
					DecisionFunction();
				}

				// do not include current candle in Min/Max data
				// until after the decision function
				minmax.ReceiveTick(Candle.H);
				minmax.ReceiveTick(Candle.L);
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
						if (EntryCriteriaLong())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetTrailingStop(trailstop);
						}
					}

					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaShort())
						{
							Framework.Account.EnterTrade(-1);
							Framework.Account.SetTrailingStop(trailstop);
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

			if (Candle.C > minmax.Max())
			{
				v = true;
			}

			return v;
		}

		private bool EntryCriteriaShort()
		{
			bool v = false;

			if (Candle.C < minmax.Min())
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
			return "X Length Channel Breakout";
		}
	}
}