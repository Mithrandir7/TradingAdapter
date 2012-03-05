
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fDerivativesSystem : ForexObject
	{
		// the initialization values for the indicators and candles
		int PeriodsLong;
		int PeriodsShort;
		double Delta1;
		double Delta2;
		int Minutes;

		// the main candlebuilder and an auxiliary spread variable
		double spread;
		cCandleBuilder cbx;

		// variables used by the decision function
		double AvgLong;
		double AvgShort;
		double Deriv2;
		double MaxAbsDeriv;

		// constructor, called only once, setup multiple tick variables
		public fDerivativesSystem()
		{
		}

		// initialization routine, called once before ticks are sent
		// if multiple tick sources are used, will get called several times
		// during the lifetime of the object, once per tick source change
		public override void Init(string pParameters)
		{
			// save parameters sent in case they contain optional parameters
			RealParameters = pParameters;
			// a Parameter Parser can be instantiated to check optional parameters
			RParser = new ParameterParser(pParameters);

			// this is a mandatory list of parameters.
			// Optional parameters are not specified here
			// they need to be checked before DParser.Merge is called
			// or checked from the RealParameters variable
			DefaultParameters = "PERIODLONG:17;PERIODSHORT:2;MINUTES:10;DELTA1:0.0005;DELTA2:0.0016;";

			// parameters as specified by the user
			PParser = new ParameterParser(pParameters);
			// mandatory parameter list and default values
			DParser = new ParameterParser(DefaultParameters);   

			// make a list of parameters from mandatory list
			// merge user values if existing, otherwise use defaults
			Parameters = DParser.Merge(PParser);				

			// build a new parameter parser with sanitized parameter and values 
			// now user values have been merged with desired parameters
			// extraneous, or optional parameters eliminated.
			PParser = new ParameterParser(Parameters);

			// get periods to use for the indicator(s)
			PeriodsLong = (int)PParser.GetDouble("PERIODLONG",0);
			PeriodsShort = (int)PParser.GetDouble("PERIODSHORT",0);;
			Delta1 = PParser.GetDouble("DELTA1",0);
			Delta2 = PParser.GetDouble("DELTA2",0);

			// now indicators must be instantiated

			// your indicator instantiation goes here

			// end of indicator instantiation

			// instantiate candlebuilder with desired timeframe
			Minutes = (int)PParser.GetDouble("MINUTES",0);
//			cbx = new cCandleBuilder(Minutes,PeriodsLong+PeriodsShort+1);
			cbx = new cCandleBuilder(Minutes,PeriodsLong+PeriodsShort+20);

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
			Framework.WriteGraphLine("InTrade,Margin,AvgMRLong,AvgMRShort,Der2,MaxAbsDer2");
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
				if (cbx.candlecount > PeriodsLong+PeriodsShort)
				{
					MaxAbsDeriv = -1.0E-25;

					double Sum;
					sCandle Candle;

					Sum = 0;
					for (int i=1; i<PeriodsLong+1; i++)
					{
						Candle = cbx.GetCandle(i);
						double a = (Candle.H + Candle.L)/2;
						Sum += a;

						Candle = cbx.GetCandle(i+1);
						double b = (Candle.H + Candle.L)/2;

						Candle = cbx.GetCandle(i+2);
						double c = (Candle.H + Candle.L)/2;

						double deriv2 = Math.Abs(a - 2*b + c);
						if (deriv2 > MaxAbsDeriv)
						{
							MaxAbsDeriv = deriv2;
						}

						if (i==1)
						{
							Deriv2 = deriv2;
						}
					}

					AvgLong = Sum / PeriodsLong;

					Sum = 0;
					for (int i=PeriodsLong+1; i<PeriodsLong+PeriodsShort+1; i++)
					{
						Candle = cbx.GetCandle(i);
						Sum += (Candle.H + Candle.L)/2;
					}

					AvgShort = Sum / PeriodsShort;

					// after that, call decision function 
					DecisionFunction();
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
						EntryCriteriaLong();
					}

					// now is not a repetitive check since
					// EntryCriteriaLong might have entered a trade
					// so check again
					if (Framework.Account.InTrade()==0)
					{
						EntryCriteriaShort();
					}

					break;
				}

					// if we are currently long, look for a long exit strategy
				case 1:
				{
					ExitCriteriaShort();
					//ExitCriteriaLong();
					break;
				}

					// if we are currently short, look for a short exit strategy
				case -1:
				{
					ExitCriteriaLong();
					//ExitCriteriaShort();
					break;
				}
			}

			// extract values from indicators and account
			// and log them to a file (used to create a graph with a separate app)
			double logInTrade = Framework.Account.InTrade();
			double logMargin  = Framework.Account.GetAccount().C;

			// write monitored valued to the numerical output file
			//Framework.WriteGraphLine("InTrade,Margin,AvgMRLong,AvgMRShort,Der2,MaxAbsDer2");
			Framework.WriteGraphLine(logInTrade+","+logMargin+","+AvgLong+","+AvgShort+","+Deriv2+","+MaxAbsDeriv);

		}

		private void EntryCriteriaLong()
		{
			// same as ExitCriteriaShort
			if (((AvgShort - AvgLong) > Delta1) && (MaxAbsDeriv < Delta2))
			//if (((AvgLong - AvgShort) > Delta1) && (MaxAbsDeriv < Delta2))
			{
				Framework.Account.EnterTrade(1);
			}
		}

		private void EntryCriteriaShort()
		{
			// same as ExitCriteriaLong
			if (((AvgLong - AvgShort) > Delta1) && (MaxAbsDeriv < Delta2))
			//if (((AvgShort - AvgLong) > Delta1) && (MaxAbsDeriv < Delta2))
			{
				Framework.Account.EnterTrade(-1);
			}
		}

		private void ExitCriteriaLong()
		{
			// apply an exit criteria here based on the indicators
			// spread, and other data collected by object
			if (((AvgShort - AvgLong) > Delta1) && (MaxAbsDeriv < Delta2))
			{
				Framework.Account.ExitTrade();
			}
		}

		private void ExitCriteriaShort()
		{
			if (((AvgLong - AvgShort) > Delta1) && (MaxAbsDeriv < Delta2))
			{
				Framework.Account.ExitTrade();
			}
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
			return "Derivatives System";
		}
	}





}