
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fCandleSystem : ForexObject
	{
		int iPeriods;
		int Minutes;

		double spread;
		cCandleBuilder cbx;

		oCandlePatterns CandlePatterns;

		public fCandleSystem()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"PERIODS:14;MINUTES:5;");

			iPeriods = (int)PParser.GetDouble("PERIODS",0);

			Minutes = (int)PParser.GetDouble("MINUTES",0);
			cbx = new cCandleBuilder(Minutes,10);

			CandlePatterns = new oCandlePatterns();

			Framework.TickServer.RegisterTickListener("cbx","*",cbx);
			cbx.RegisterCandleListener("cbx",this);
			Framework.TickServer.RegisterTickListener("System","*",this);

			Framework.WriteGraphLine("InTrade,Margin");			
		}

		public override void ReceiveTick(sTick pTick)
		{
			spread = pTick.Ask - pTick.Bid;
		}

		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
			if ((pPeriod==Minutes) && (pCBTitle=="cbx"))
			{
				CandlePatterns.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
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
						if (EntryCriteriaLong())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetTrailingStop(0.0008);
						}
					}

					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaShort())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetTrailingStop(0.0008);
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

			Framework.WriteGraphLine(logInTrade+","+logMargin);

		}

		private bool EntryCriteriaLong()
		{
			bool v = false;

			if (v != false)
			{
				v = true;
			}

			return v;
		}

		private bool EntryCriteriaShort()
		{
			bool v = false;

			if (v != false)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaLong()
		{
			bool v = false;

			if (v != false)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaShort()
		{
			bool v = false;

			if (v != false)
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
			return "Candle Pattern System";
		}
	}





}