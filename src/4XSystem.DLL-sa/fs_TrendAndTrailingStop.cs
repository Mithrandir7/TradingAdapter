
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fTrendAndTrailingStop : ForexObject
	{
		// the initialization values for the indicators and candles
		int Periods;
		int Minutes;

		// the main candlebuilder and an auxiliary spread variable
		double spread;
		cCandleBuilder cbx;

		// variables used by the decision function
		iDerivatives Derivatives;
		CQueue Deriv1;
		CQueue Deriv2;

		CQueue HMAD1;

		iHMA HMA;
		iFMA FMA;
		iBollingerBands BBands;
		iStochRSI StochRSI;
		iCCI CCI;

		// constructor, called only once, setup multiple tick variables
		public fTrendAndTrailingStop()
		{
		}

		// initialization routine, called once before ticks are sent
		// if multiple tick sources are used, will get called several times
		// during the lifetime of the object, once per tick source change
		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"PERIODS:17;MINUTES:10;");

			// get periods to use for the indicator(s)
			Periods     = (int)PParser.GetDouble("PERIODS",0);
			Derivatives = new iDerivatives();
			HMA         = new iHMA(Periods);
			HMAD1       = new CQueue(Periods);
			FMA         = new iFMA(Periods);
			Deriv1      = new CQueue(Periods);
			Deriv2      = new CQueue(Periods);
			BBands      = new iBollingerBands(Periods,-1);
			StochRSI    = new iStochRSI(Periods);
			CCI			= new iCCI(Periods);

			// instantiate candlebuilder with desired timeframe
			Minutes = (int)PParser.GetDouble("MINUTES",0);
			//			cbx = new cCandleBuilder(Minutes,PeriodsLong+PeriodsShort+1);
			cbx = new cCandleBuilder(Minutes,Periods);

			// register candlebuilder as a tick listener, name unimportant
			Framework.TickServer.RegisterTickListener("cbx","*",cbx);
			// register this object as a candle listener
			// the candle name is important since we might receive
			// several candles with same period. 
			cbx.RegisterCandleListener("cbx",this);
			// multiple candlebuilders can be setup by using previous 4 lines.

			// register this object as a tick listener, name unimportant
			// this is an optional step to receive ticks in between candles
			Framework.TickServer.RegisterTickListener("System","*",this);

			// start header line of numerical output file
			Framework.WriteGraphLine("InTrade,Margin,C,TP,FMA,HMA,Deriv1,Deriv2,SMA,BBand1,BBand2,%b,Bandwidth,StochRSI,CCI");
		}

		// TickServer calls all registered functions with new ticks
		// examples are: Account (high priority, called first)
		// Candlebuilders, Systems (like this)
		public override void ReceiveTick(sTick pTick)
		{
			spread = pTick.Ask - pTick.Bid;
		}

		// CandleBuilders call this function when registered with them
		// and a new candle is built
		// one distinguished calling CandleBuilder by handle in candle and period
		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
			if ((pPeriod==Minutes) && (pCBTitle=="cbx"))
			{
				HMA.ReceiveTick(pCandle.C);
				FMA.ReceiveTick(pCandle.C);
				CCI.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
				//BBands.ReceiveTick(pCandle.C);
				//StochRSI.ReceiveTick(pCandle.C);

				if (HMA.Value() != 0)
				{
					Derivatives.ReceiveTick(HMA.Value());
					BBands.ReceiveTick(HMA.Value());
					StochRSI.ReceiveTick(HMA.Value());

				}

				if ( (cbx.candlecount > 3) && (Derivatives.isPrimed()) && (StochRSI.isPrimed()))
				{
					double deriv2; 
					double deriv1;

					Derivatives.Value(out deriv1, out deriv2);

					Deriv1.Add(deriv1);
					Deriv2.Add(deriv2);
					
					if (Deriv2.tickcount>2)
					{
						DecisionFunction();
					}
				}
			}
		}

		// decide if we are entering or exiting a long or short trade
		// different criteria can be used for each condition
		private void DecisionFunction()
		{
			switch (Framework.Account.InTrade())
			{
					// if we are not in a trade, see to enter long or short
				case 0:
				{
					// seems like a repetitive check
					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaLong())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetTrailingStop(0.0018);
						}
					}

					// now is not a repetitive check since
					// EntryCriteriaLong might have entered a trade
					// so check again
					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaShort())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetTrailingStop(0.0018);
						}

					}

					break;
				}

					// if we are currently long, look for a long exit strategy
				case 1:
				{
					if (EntryCriteriaShort())
					{
						Framework.Account.ExitTrade();
						//Framework.Account.EnterTrade(-1);
					}
					break;
				}

					// if we are currently short, look for a short exit strategy
				case -1:
				{
					if (EntryCriteriaLong())
					{
						Framework.Account.ExitTrade();
						//Framework.Account.EnterTrade(1);
					}
					break;
				}
			}

			// extract values from indicators and account
			// and log them to a file (used to create a graph with a separate app)
			double logInTrade = Framework.Account.InTrade();
			double logMargin  = Framework.Account.GetAccount().C;
			double C  = cbx.GetCandle(0).C;
			double TP = (cbx.GetCandle(0).C+cbx.GetCandle(0).H+cbx.GetCandle(0).L)/3;
			double hma = HMA.Value();
			double fma = FMA.Value();
			double deriv1 = ((double)Deriv1.GetItem(0));
			double deriv2 = ((double)Deriv2.GetItem(0));
			double sma;
			double bband1;
			double bband2;
			double pb;
			double bw;
			double srsi = StochRSI.Value();
			double cci = CCI.Value();
			BBands.Value(out bband1,out sma,out bband2,out pb,out bw);

			// write monitored valued to the numerical output file
			// Framework.WriteGraphLine("InTrade,Margin,C,TP,FMA,HMA,Deriv1,Deriv2,SMA,BBand1,BBand2,%b,Bandwidth,StochRSI,CCI");
			Framework.WriteGraphLine(logInTrade+","+logMargin+","+C+","+TP+","+fma+","+hma+","+deriv1+","+deriv2+","+sma+","+bband1+","+bband2+","+pb+","+bw+","+srsi+","+cci);
		}

		private bool EntryCriteriaLong()
		{
			bool v = false;

			//if ((Math.Sign((double)Deriv2.GetItem(0)) != Math.Sign((double)Deriv2.GetItem(1))) && (Math.Sign((double)Deriv2.GetItem(0))>0))
			//if ((Math.Sign((double)Deriv2.GetItem(0)) != Math.Sign((double)Deriv2.GetItem(1))) && (Math.Sign((double)Deriv2.GetItem(0))>0) && (Math.Sign((double)Deriv1.GetItem(0))>0))
			if ((Math.Sign((double)Deriv1.GetItem(0))>0))
			{
				v = true;
			}

			return v;
		}

		private bool EntryCriteriaShort()
		{
			bool v = false;
			//if ((Math.Sign((double)Deriv2.GetItem(0)) != Math.Sign((double)Deriv2.GetItem(1))) && (Math.Sign((double)Deriv2.GetItem(0))<0))
			//if ((Math.Sign((double)Deriv2.GetItem(0)) != Math.Sign((double)Deriv2.GetItem(1))) && (Math.Sign((double)Deriv2.GetItem(0))<0) && (Math.Sign((double)Deriv1.GetItem(0))>0))
			if ((Math.Sign((double)Deriv1.GetItem(0))<0))
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaLong()
		{
			bool v = false;

			return v;
		}

		private bool ExitCriteriaShort()
		{
			bool v = false;

			return v;
		}

		// this method is called when the ticks are over
		// the object can switch tick sources and return false
		// to force SystemTester to execute it again with the new ticks
		// this mechanism allows the object to compare an algorithm
		// with two different tick sources. 
		public override bool Finished()
		{
			bool returnv = true;

			return returnv;
		}

		// return the name of the system
		public override string Title()
		{
			return "Trend Strength and Trailing Stop System";
		}
	}

}





