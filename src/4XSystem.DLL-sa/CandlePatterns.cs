
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{
	public enum Patterns {ThreeWhiteSoldiers, Harami, Hammer, Doji}

	public struct PatternName
	{
		public string	Name;
		public Patterns Type;

		public PatternName(string pName, Patterns pType)
		{
			Name = pName;
			Type = pType;
		}
	}

	public struct Pattern
	{
		public string   Name;
		public Patterns Type;
		public double	Strength;
		public string	Comments;

		public Pattern(string pName, Patterns pType, double pStrength, string pComments)
		{
			Name = pName;
			Type = pType;
			Strength = pStrength;
			Comments = pComments;
		}
	}

	public class oCandlePatterns
	{
		System.Collections.Hashtable patterns;
		iTrend Trend;
		double trend;

		int tickcount;
		int CandlesToBuffer;
		sCandle[] Candles;
		
		public oCandlePatterns()
		{
			Trend = new iTrend(10);
			trend = 0;

			CandlesToBuffer = 7;
			Candles = new sCandle[CandlesToBuffer];
			tickcount = 0;
			
			patterns = new System.Collections.Hashtable();

			// list the patterns recognized by this class
			AddPattern(Patterns.Doji,"Doji");
			AddPattern(Patterns.Harami,"Harami");
			AddPattern(Patterns.ThreeWhiteSoldiers,"Three White Soldiers");
			AddPattern(Patterns.Hammer,"Hammer/Hanging Man"); 

		}

		private void AddPattern(Patterns pPattern, string pName)
		{
			patterns.Add(pPattern,new PatternName(pName,pPattern));
		}
		
		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			Trend.ReceiveTick(pO,pH,pL,pC);

			sCandle Candle = new sCandle();

			Candle.O = pO;
			Candle.H = pH;
			Candle.L = pL;
			Candle.C = pC;

			Candles[(tickcount % CandlesToBuffer)]=Candle;

			// avoid overflow by restricting range of tickcount
			// when indicator is fully primed
			tickcount++;
			if (tickcount == (3*CandlesToBuffer))
			{
				tickcount = (2*CandlesToBuffer);
			}
		}

		public System.Collections.ArrayList GetPattern(ref double pTrend)
		{
			System.Collections.ArrayList dpatterns = new System.Collections.ArrayList();

			if (tickcount > CandlesToBuffer)
			{
				trend = Trend.Value();
				dpatterns = DetectPattern();
			}

			pTrend = trend;
			return dpatterns;

		}

		public string TranslatePattern(System.Collections.ArrayList pPatterns)
		{
			string v="";

			for (int i=0; i< pPatterns.Count; i++)
			{
				string comments = ((Pattern)pPatterns[i]).Comments;

				v += ((Pattern)pPatterns[i]).Name;

				if  (comments!="")
					v += ":" + comments;

				if ((i+1)!=pPatterns.Count)
				{
					v+="; ";
				}
			}

			return v;
		}

		private System.Collections.ArrayList DetectPattern()
		{
			System.Collections.ArrayList dpatterns = new System.Collections.ArrayList();

			foreach (Patterns p in Enum.GetValues(typeof(Patterns)))
			{
				bool ispattern = false;
				string comments = "";
				double strength = 0;

				switch (p)
				{
					case Patterns.Doji:
					{
						ispattern = isDoji(ref strength, ref comments);
						break;
					}

					case Patterns.Harami:
					{
						ispattern = isHarami(ref strength, ref comments);
						break;
					}

					case Patterns.ThreeWhiteSoldiers:
					{
						ispattern = isThreeWhiteSoldiers(ref strength, ref comments);
						break;
					}

					case Patterns.Hammer:
					{
						ispattern = isHammer(ref strength, ref comments);
						break;
					}
				
				}

				if (ispattern)
				{
					Pattern pattern = new Pattern(((PatternName)patterns[p]).Name,((PatternName)patterns[p]).Type,strength,comments);
					dpatterns.Add(pattern);
				}

			}
			
			return dpatterns;
		}

		// 0 = newest candle   4 = oldest candle
		private int CandleIndex(int candle, int tickcount)
		{
			int index = 0;
			int position = tickcount % CandlesToBuffer;

			if ((position-(candle+1))>=0)
			{
				index = position - (candle+1);
			}
			else
			{
				index = CandlesToBuffer - Math.Abs(position - (candle+1));
			}
				
			return index;
		}

		private bool isDoji(ref double pStrength, ref string pComments)
		{
			bool v = false;
			pStrength = -1;
			pComments = "";

			double O=Candles[CandleIndex(0,tickcount)].O;
			double H=Candles[CandleIndex(0,tickcount)].H;
			double L=Candles[CandleIndex(0,tickcount)].L;
			double C=Candles[CandleIndex(0,tickcount)].C;

			if (
				(Math.Abs(O-C)<0.0002)	&&					// open and close near each other
				(Math.Abs(H-Math.Max(O,C)) > 0.0001) &&		// has upper body
				(Math.Abs(L-Math.Min(O,C)) > 0.0001)		// has lower body
				)  
			{
				v = true;
			}

			if (v)
			{
				if (
					(Math.Abs(H-Math.Max(O,C)) > 0.0004) &&
					(Math.Abs(L-Math.Min(O,C)) > 0.0004) &&
					(Math.Abs(Math.Abs(H-Math.Max(O,C)) / Math.Abs(L-Math.Min(O,C))) > 0.8)
					)
				{
					pComments += " Long Legged";
				}

				if (
					(( Math.Abs(H-Math.Max(O,C)) / Math.Abs(L-Math.Min(O,C)) ) < 0.1)
					)
				{
					pComments += " Dragonfly";
				}

				if (
					((Math.Abs(L-Math.Min(O,C)) / Math.Abs(H-Math.Max(O,C))) < 0.1)
					)
				{
					pComments += " Gravestone";
				}

			}

			return v;
		}

		private bool isHarami(ref double pStrength, ref string pComments)
		{
			bool v = false;
			pStrength = 1;
			pComments = "";

			// mother candle
			double O1=Candles[CandleIndex(1,tickcount)].O;
			double H1=Candles[CandleIndex(1,tickcount)].H;
			double L1=Candles[CandleIndex(1,tickcount)].L;
			double C1=Candles[CandleIndex(1,tickcount)].C;

			// baby candle
			double O2=Candles[CandleIndex(0,tickcount)].O;
			double H2=Candles[CandleIndex(0,tickcount)].H;
			double L2=Candles[CandleIndex(0,tickcount)].L;
			double C2=Candles[CandleIndex(0,tickcount)].C;


			if (
				(Math.Abs(O1-C1)>0.0005)	&&				   // mother: long body
				(Math.Abs(O2-C2)<0.0004)    &&				   // baby:   small body
				((Math.Max(O1,C1)-Math.Max(O2,C2))>0.0001) &&  // top of baby body < mother body
				((Math.Min(O2,C2)-Math.Min(O1,C1))>0.0001)     // bottom of baby body > mother body
				)
			{
				v = true;
			}

			return v;
		}

		private bool isThreeWhiteSoldiers(ref double pStrength, ref string pComments)
		{
			bool v = false;
			pStrength = 1;
			pComments = "";

			return v;
		}

		private bool isHammer(ref double pStrength, ref string pComments)
		{
			bool v = false;
			pStrength = 1;
			pComments = "";

			double O=Candles[CandleIndex(0,tickcount)].O;
			double H=Candles[CandleIndex(0,tickcount)].H;
			double L=Candles[CandleIndex(0,tickcount)].L;
			double C=Candles[CandleIndex(0,tickcount)].C;

			if (
				( (Math.Abs(H-Math.Max(O,C)) / Math.Abs(O-C) ) <0.2)	&&	// relative small upper body:
				( (Math.Abs(O-C)/(Math.Abs(L-Math.Min(O,C)))) >=2 )	    &&
				( Math.Abs(O-C)>0.0003 )
				)
			{
				v = true;
			}

			if (v)
			{
				if (trend>2)
				{
					pComments = "Hanging Man";
				}

				if (trend<2)
				{
					pComments = "Hammer";
				}
			}

			return v;
		}

	}
}