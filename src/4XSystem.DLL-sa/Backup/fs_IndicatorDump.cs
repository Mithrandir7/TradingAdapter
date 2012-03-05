
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fIndicatorDump : ForexObject
	{
		// the initialization values for the indicators and candles
		int iPeriods;
		int Minutes;

		// the main candlebuilder and an auxiliary spread variable
		double spread;
		sCandle Candle;
		cCandleBuilder cbx;

		oIndicatorDump IndicatorDump;

		// constructor, called only once, setup multiple tick variables
		public fIndicatorDump()
		{
		}

		// initialization routine, called once before ticks are sent
		// if multiple tick sources are used, will get called several times
		// during the lifetime of the object, once per tick source change
		public override void Init(string pParameters)
		{
			// initialize all ParameterParsers
			this.InitializeParameters(pParameters,"PERIODS:14;MINUTES:5;");

			// get periods to use for the indicator(s)
			iPeriods = (int)PParser.GetDouble("PERIODS",0);

			// now indicators must be instantiated
			// your indicator instantiation goes here

			IndicatorDump = new oIndicatorDump(iPeriods);

			// end of indicator instantiation

			// instantiate candlebuilder with desired timeframe
			Minutes = (int)PParser.GetDouble("MINUTES",0);
			cbx = new cCandleBuilder(Minutes,10);
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

			string Header = IndicatorDump.GetHeader();
			Header = "InTrade,Margin,"+Header;
			Framework.WriteGraphLine(Header);

		}

		// TickServer calls all registered functions with new ticks
		// examples are: Account (high priority, called first)
		// Candlebuilders, Systems (like this)
		public override void ReceiveTick(sTick pTick)
		{
			spread = pTick.Ask - pTick.Bid;
			//cb10.ReceiveTick(pTick);	// register with TickServer or call here
		}

		// CandleBuilders call this function when registered with them
		// and a new candle is built
		// one distinguished calling CandleBuilder by handle in candle and period
		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
			if ((pPeriod==Minutes) && (pCBTitle=="cbx"))
			{
				// must manually send candle values to indicator(s) in use
				// that way, same indicator can be used twice with different 
				// candle periods, or different values (C, O, avg(H,L,C))

				Candle = pCandle;

				IndicatorDump.ReceiveCandle(pCandle);

				// after that, call decision function 
				DecisionFunction();
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
							// set stops or trailing stops as appropriate
							Framework.Account.SetTrailingStop(0.0008);
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
							// set stops or trailing stops as appropriate
							Framework.Account.SetTrailingStop(0.0008);
						}
					}
					break;
				}

					// if we are currently long, look for a long exit strategy
				case 1:
				{
					if (ExitCriteriaLong())
					{
						Framework.Account.ExitTrade();
						// exit or flip trade
						//Framework.Account.EnterTrade(-1);
					}
					break;
				}

					// if we are currently short, look for a short exit strategy
				case -1:
				{
					if (ExitCriteriaShort())
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

			// write monitored valued to the numerical output file
			string Value = IndicatorDump.GetValues();
			Value = logInTrade+","+logMargin+","+Value;
			Framework.WriteGraphLine(Value);
		}

		private bool EntryCriteriaLong()
		{
			// apply an entry criteria here based on the indicators
			// spread, and other data collected by object
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
			// apply an exit criteria here based on the indicators
			// spread, and other data collected by object
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
			return "Indicator Dump";
		}
	}


}