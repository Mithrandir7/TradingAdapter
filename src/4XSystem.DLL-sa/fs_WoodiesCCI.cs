
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fWoodiesCCI : ForexObject
	{
		int CCIPeriods;
		int TCCIPeriods;
		int Minutes;

		iCCI CCI,TCCI;
		iDerivatives CCISlope;
		CQueue CCIH,TCCIH;
		

		double CCIDelta,CCISpeed;
		double logInTrade;
		double logMargin;
		double CCIv,TCCIv;

		string TradeTypeText,TradeFoundText;
		string DirectionText, BelowFiftyText;
		string ZLRText;

		int TradeType,TradeFound;
		int Direction,BelowFifty;
		int PrevDir;
		int ZLR;

		sCandle Candle;
		double spread;
		cCandleBuilder cbx;

		public fWoodiesCCI()
		{
		}

		public override void Init(string pParameters)
		{
			this.InitializeParameters(pParameters,"CCI:14;TCCI:6;MINUTES:5;");

			CCIPeriods = (int)PParser.GetDouble("CCI",0);
			TCCIPeriods = (int)PParser.GetDouble("TCCI",0);
			Minutes = (int)PParser.GetDouble("MINUTES",0);

			CCI = new iCCI(CCIPeriods);
			TCCI = new iCCI(TCCIPeriods);
			CCISlope = new iDerivatives();

			CCIH = new CQueue(CCIPeriods*3);
			TCCIH = new CQueue(CCIPeriods*3);
			
			// OPTIMIZATION PARAMETER CHECK
			// if parameters are erroneous (TurboCCI length > CCI)
			// do not register candle listeners
			// SystemTester will then not send us any ticks
			// and go to the next iteration
			// need 6 cci bars to determine trend

			if ( (TCCIPeriods < CCIPeriods) || (CCIPeriods<6) ) 
			{
				cbx = new cCandleBuilder(Minutes,10);			
				Framework.TickServer.RegisterTickListener("cbx","*",cbx);
				cbx.RegisterCandleListener("cbx",this);
				Framework.TickServer.RegisterTickListener("System","*",this);

				Framework.WriteGraphLine("InTrade,Margin,C,CCI,CCIDelta,CCISpeed,TCCI,TypeOfTrade,Direction,BelowFifty,ZLR");	
			}

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
				CCI.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
				TCCI.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);

				// need to put primed values into this indicators
				// not primed right now to decrease down time
				CCIH.Add(CCI.Value());
				TCCIH.Add(CCI.Value());
				CCISlope.ReceiveTick(CCI.Value());

				if (CCI.isPrimed() && TCCI.isPrimed())
				{
					DecisionFunction();
				}
			}
		}

		private void FindTrend()
		{
			double ThisBar = 0;
			double LastBar = 0;
			int PosBars = 0;
			int NegBars = 0;

			for (int i=0; i<8; i++)
			{
				ThisBar = (double)CCIH.GetItem(i);

				if (ThisBar >0)
				{
					PosBars++;
				}
				
				if (ThisBar <0)
				{
					NegBars++;
				}
			}

			TradeType = 0;
			TradeTypeText = "";
				
			if (PosBars>=6)
			{
				TradeType = 1;
				TradeTypeText = "Long";
			}

			if (NegBars>=6)
			{
				TradeType = -1;
				TradeTypeText = "Short";
			}
		}

		private void DetectZLR()
		{
			PrevDir = Direction;
			Direction = 0;
			DirectionText = "";

			BelowFifty = 0;
			BelowFiftyText = "";

			ZLR = 0;
			ZLRText = "";

			if ((TradeType!=0) && (CCIDelta < -5))
			{
				Direction = -1;
				DirectionText = "Down";
			}

			if ((TradeType!=0) && (CCIDelta > 5))
			{
				Direction = +1;
				DirectionText = "Up";
			}

			if (TradeType == 1)
			{
				if ((CCIv<=50) && (CCIv>-15))
				{
					BelowFifty = 1;
					BelowFiftyText = "Below Fifty";
				}
			}

			if (TradeType == -1)
			{
				if ((CCIv>=-50) && (CCIv<15))
				{
					BelowFifty = -1;
					BelowFiftyText = "Below Fifty";
				}
			}

			if ((BelowFifty!=0) && (Direction==1) && (PrevDir==-1) && (TradeType==1))
			{
				ZLR = +1;
				ZLRText = "ZLR+";
			}

			if ((BelowFifty!=0) && (Direction==-1) && (PrevDir==1) && (TradeType==-1))
			{
				ZLR = -1;
				ZLRText = "ZLR-";
			}

		}
		
		private void DecisionFunction()
		{
			CCIv = CCI.Value();
			TCCIv = TCCI.Value();
			CCISlope.Value(out CCIDelta,out CCISpeed);

			// preprocessing
			FindTrend();
			DetectZLR();
	
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

			//Framework.WriteGraphLine("InTrade,Margin,C,CCI,CCIDelta,CCISpeed,TCCI,TypeOfTrade,Direction,BelowFifty,ZLR");	
			Framework.WriteGraphLine(logInTrade+","+logMargin+","+Candle.C+","+CCIv+","+CCIDelta+","+CCISpeed+","+TCCIv+","+TradeTypeText+","+DirectionText+","+BelowFiftyText+","+ZLRText);

		}

		private bool EntryCriteriaLong()
		{
			bool v = false;

			if (v != false)
			//if (LongShortTrade==1)
			{
				v = true;
			}

			return v;
		}

		private bool EntryCriteriaShort()
		{
			bool v = false;
			
			if (v != false)
			//if (LongShortTrade==-1)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaLong()
		{
			bool v = false;

			if (v != false)
			//if (LongShortTrade!=1)
			{
				v = true;
			}

			return v;
		}

		private bool ExitCriteriaShort()
		{
			bool v = false;

			if (v != false)
			//if (LongShortTrade!=-1)
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
			return "Woodies CCI System";
		}
	}





}