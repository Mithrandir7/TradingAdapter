
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fBollingerA : ForexObject
	{
		int iPeriods;
		double iWidth;
		int iHMALen;
		int Minutes;

		double BWThreshold,TrailingStop;
		double BWReversalThreshold;
		
		iBollingerBands BBands;
		iHMA HMA;
		iSlope SMASlope;

		sCandle Candle;
		double spread;
		cCandleBuilder cbx;

		double BPlus,SMA,BMinus,PctB,BW;
		double HMAv;

		double MinBWidth;
		double BWidth;
		int State;
		int EntryRecommend;

		public fBollingerA()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"BB:20;WIDTH:-1;HMA:10;BWT:0.0008;BWRT:0.0002;TS:0.0003;MINUTES:5;");

			State = 0;
			
			iPeriods = (int)PParser.GetDouble("BB",0);
			iWidth = (int)PParser.GetDouble("WIDTH",0);
			iHMALen = (int)PParser.GetDouble("HMA",0);
			Minutes = (int)PParser.GetDouble("MINUTES",0);
			BWThreshold = PParser.GetDouble("BWT",0);
			BWReversalThreshold = PParser.GetDouble("BWRT",0);
			TrailingStop = PParser.GetDouble("TS",0);

			BBands = new iBollingerBands(iPeriods,iWidth);
			HMA = new iHMA(iHMALen);
			SMASlope = new iSlope();

			cbx = new cCandleBuilder(Minutes,10);			
			Framework.TickServer.RegisterTickListener("cbx","*",cbx);
			cbx.RegisterCandleListener("cbx",this);
			Framework.TickServer.RegisterTickListener("System","*",this);

			Framework.WriteGraphLine("InTrade,Margin,B+,C,HMA,SMA,B-,State,BWidth,MinBWidth,EntryRecommend");	
		}

		public override void ReceiveTick(sTick pTick)
		{
			spread = pTick.Ask - pTick.Bid;
		}

		public override void ReceiveCandle(sCandle pCandle, int pPeriod, string pCBTitle)
		{
			if ((pPeriod==Minutes) && (pCBTitle=="cbx"))
			{
				double TP = (pCandle.H+pCandle.L+pCandle.C)/3;

				Candle = pCandle;

				BBands.ReceiveTick(pCandle.C);
				HMA.ReceiveTick(TP);

				if (BBands.isPrimed() && HMA.isPrimed())
				{
					DecisionFunction();
				}
			}
		}

		
		private void DecisionFunction()
		{
			BBands.Value(out BPlus,out SMA,out BMinus,out PctB,out BW);
			BWidth = BPlus - BMinus;
			SMASlope.ReceiveTick(SMA);
			
			HMAv = HMA.Value();

			// preprocessing

			StateMachine();
	
			switch (Framework.Account.InTrade())
			{
				case 0:
				{
					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaLong())
						{
							Framework.Account.EnterTrade(1);
							Framework.Account.SetTrailingStop(TrailingStop);
						}
					}

					if (Framework.Account.InTrade()==0)
					{
						if (EntryCriteriaShort())
						{
							Framework.Account.EnterTrade(-1);
							Framework.Account.SetTrailingStop(TrailingStop);
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

			//Framework.WriteGraphLine("InTrade,Margin,B+,C,HMA,SMA,B-,State,BWidth,MinBWidth,EntryRecommend");	
			Framework.WriteGraphLine(logInTrade+","+logMargin+","+BPlus+","+Candle.C+","+HMAv+","+SMA+","+BMinus+","+State+","+BWidth+","+MinBWidth+","+EntryRecommend);

		}

		private void StateMachine()
		{
			switch (State)
			{
				case 0:

					EntryRecommend = 0;

					if (BWidth<BWThreshold)
					{
						State = 1;
						MinBWidth = BWidth;
					}

					break;

				case 1:

					if (BWidth < MinBWidth)
					{
						MinBWidth = BWidth;
					}

					if ((BWidth - MinBWidth) > BWReversalThreshold)
					{
						double Slope = SMASlope.Value();

						if (Slope!=0)
						{
							EntryRecommend = (int)Slope;
							State = 2;
						}
					}
					break;

				case 2:

					if (Framework.Account.InTrade()==0)
					{
						State = 0;
					}
					break;
			}
		}

		private bool EntryCriteriaLong()
		{
			bool v = false;

			if (EntryRecommend==+1)
			{
				v = true;
			}

			return v;
		}

		private bool EntryCriteriaShort()
		{
			bool v = false;
			
			if (EntryRecommend==-1)
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
			return "Bollinger System A";
		}
	}





}