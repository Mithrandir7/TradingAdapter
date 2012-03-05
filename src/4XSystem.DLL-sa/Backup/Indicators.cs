
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

// implement StochMom (stochastics of momemtum)

// implement with CQueue:
// Momemtum, RSI, SMA, Stochastics, Trend

// implement isPrimed:
// Renko

// implement Parameter Validation in isPrimed (make sure .Value calls local .isPrimed):
// check all.

using System;

namespace _4XLab.NET
{

	public class iStub
	{
		int tickcount;
		int periods;

		//
		// declare basic indicators here
		//

		public iStub(int pPeriods)
		{
			periods = pPeriods;
			tickcount = 0;

			// instantiate any basic indicators here
		}

		public void ReceiveTick(double Val)
		{
			tickcount++;
			if (tickcount == (3*periods))
			{
				tickcount = (2*periods);	// avoid overflow (large->0) when indicator is primed
			}

			// perform indicator calculations here or send ticks to other indicators

		}

		public double Value()
		{
			double v=0;

			// compute indicator value here (note that this function might get called multiple times
			// so do not set flags or do time specific behavior

			if (isPrimed())
			{
				// perform computation of output value
				v = 1;
			}

			return v;
		}

		// returns true when indicator has seen enough values to compute a result
		// it will never return true if initialization parameters are incorrect
		// this will cause indicator to silently fail and not give results
		// on optimization runs where parameter combination is invalid.
		public bool isPrimed()
		{	
			bool v = false;

			// change this to the appropriate criteria
			if ((tickcount >= periods) && (periods>0))
			{
				v = true;
			}

			return v;
		}
	}

	public class iDerivatives
	{
		int tickcount;
		CQueue Values;

		public iDerivatives()
		{
			tickcount = 0;
			Values = new CQueue(3);
		}

		public void ReceiveTick(double Val)
		{
			Values.Add(Val);
		}

		public void Value(out double Deriv1, out double Deriv2)
		{
			Deriv1 = 0;
			Deriv2 = 0;

			if (isPrimed())
			{
				double a = (double)Values.GetItem(0);
				double b = (double)Values.GetItem(1);
				double c = (double)Values.GetItem(2);

				Deriv1 = a - b;
				Deriv2 = a - 2*b + c;
			}
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (Values.isPrimed())
			{
				v = true;
			}
			return v;
		}
	}

	public class iMACD
	{
		int pSlowEMA, pFastEMA, pSignalEMA;
		iEMA slowEMA, fastEMA, signalEMA;

		// restriction: pPFastEMA < pPSlowEMA
		public iMACD(int pPFastEMA, int pPSlowEMA, int pPSignalEMA)
		{
			pFastEMA = pPFastEMA;
			pSlowEMA = pPSlowEMA;
			pSignalEMA = pPSignalEMA;

			slowEMA = new iEMA(pSlowEMA);
			fastEMA = new iEMA(pFastEMA);
			signalEMA = new iEMA(pSignalEMA);
		}

		public void ReceiveTick(double Val)
		{
			slowEMA.ReceiveTick(Val);
			fastEMA.ReceiveTick(Val);

			if (slowEMA.isPrimed() && fastEMA.isPrimed())
			{
				signalEMA.ReceiveTick(fastEMA.Value()-slowEMA.Value());
			}
		}

		public void Value(out double MACD, out double signal, out double hist)
		{
			if (signalEMA.isPrimed())
			{
				MACD = fastEMA.Value() - slowEMA.Value();
				signal = signalEMA.Value();
				hist = MACD - signal;
			}
			else
			{
				MACD = 0;
				signal = 0;
				hist = 0;
			}
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (signalEMA.isPrimed())
			{
				v = true;
			}
			return v;
		}

	}

	public class iBollingerBands
	{
		int periods;
		double width;

		double tick;
		iSTDDEV STDDEV;

		public iBollingerBands(int pPeriods, double pWidth)
		{
			periods = pPeriods;
			STDDEV = new iSTDDEV(pPeriods);

			if (pWidth <= 0)
			{
				// this formula was hand fitted to give
				// pPeriods:10 width: ~1.9
				// pPeriods:20 width: ~2.0
				// pPeriods:50 width: ~2.1
				// as recommended by John Bollinger

				width = Math.Log(pPeriods+2,10)*0.33+1.55;
			}
			else
			{
				width = pWidth;
			}

		}

		public void ReceiveTick(double Val)
		{
			tick = Val;
			STDDEV.ReceiveTick(Val);
		}

		public void Value(out double BPlus, out double SMA, out double BMinus, out double PercentB, out double Bandwidth)
		{
			if (isPrimed())
			{
				SMA = STDDEV.SMAValue();
				BPlus = SMA + width*STDDEV.Value();
				BMinus = SMA - width*STDDEV.Value();

				PercentB = (tick - BMinus) / (BPlus-BMinus);
				Bandwidth = (BPlus - BMinus) / SMA;
			}
			else
			{
				SMA = 0;
				BPlus = 0;
				BMinus = 0;

				PercentB = 0;
				Bandwidth = 0;
			}
		}

		public bool isPrimed()
		{	
			bool v = false;

			if (STDDEV.isPrimed())
			{
				v = true;
			}

			return v;
		}

	}

	public class iCCI
	{
		int periods;
		iSMA TPSMA;
		CQueue History;
		double TP;

		public iCCI(int pPeriods)
		{
			periods = pPeriods;
			TPSMA = new iSMA(pPeriods);
			History = new CQueue(pPeriods);
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			TP = (pH+pL+pC)/3;
			TPSMA.ReceiveTick(TP);
			History.Add(TP);
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{
				double meandev=0;
				for (int i=0; i<periods; i++)
				{
					meandev += Math.Abs(((double)History.GetItem(i))-TPSMA.Value());
				}
				meandev = meandev/periods;

				v = (TP - TPSMA.Value()) / ( meandev * 0.015);
			}

			return v;
		}

		public bool isPrimed()
		{	
			return History.isPrimed();
		}
	}












	public class iSTDDEV
	{
		int periods;
		iSMA SMA;
		CQueue History;
		double tick;

		public iSTDDEV(int pPeriods)
		{
			periods = pPeriods;
			SMA = new iSMA(pPeriods);
			History = new CQueue(pPeriods);
		}

		public void ReceiveTick(double Val)
		{
			tick = Val;
			SMA.ReceiveTick(Val);
			History.Add(Val);
		}

		public double Value()
		{
			double v=0;

			if (SMA.isPrimed())
			{
				double sum=0;
				for (int i=0; i<periods; i++)
				{
					sum += Math.Pow(((double)History.GetItem(i))-SMA.Value(),2);
				}
				v = Math.Sqrt(sum/periods);
			}

			return v;
		}

		public double SMAValue()
		{
			return SMA.Value();
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (SMA.isPrimed())
			{
				v = true;
			}
			return v;
		}
	}

	public class iEMA
	{
		private int tickcount;
		private int periods;
		private double dampen;
		private double emav;

		public iEMA(int pPeriods)
		{
			periods = pPeriods;
			dampen  = 2/((double)1.0+periods);
		}

		public void ReceiveTick(double Val)
		{
			if (tickcount < periods)
				emav += Val;
			if (tickcount ==periods)
				emav /= periods;
			if (tickcount > periods)
				emav = (dampen*(Val-emav))+emav;

			if (tickcount <= (periods+1) )
			{			
				// avoid overflow by stopping use of tickcount
				// when indicator is fully primed
				tickcount++;
			}
		}

		public double Value()
		{
			double v;

			if (isPrimed())
				v = emav;
			else
				v = 0;

			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (tickcount >= periods)
			{
				v = true;
			}
			return v;
		}
	}

	public class iFMA
	{
		private iSMA longSMA;
		private iSMA shortSMA;
		private int periods;

		public iFMA(int pPeriods)
		{
			periods = pPeriods;
			longSMA = new iSMA(periods);
			shortSMA = new iSMA(periods/2);
		}

		public void ReceiveTick(double Val)
		{
			longSMA.ReceiveTick(Val);
			shortSMA.ReceiveTick(Val);
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{
				v = shortSMA.Value()+(shortSMA.Value()-longSMA.Value());
			}

			return v;
		}

		public bool isPrimed()
		{	
			return longSMA.isPrimed();
		}
	}

	public class iHMA
	{
		private iWMA longWMA;
		private iWMA shortWMA;
		private iWMA outerWMA;

		private int periods;

		public iHMA(int pPeriods)
		{
			periods = pPeriods;
			longWMA = new iWMA(periods);
			shortWMA = new iWMA(periods/2);
			outerWMA = new iWMA((int)Math.Sqrt(periods));
		}

		public void ReceiveTick(double Val)
		{
			longWMA.ReceiveTick(Val);
			shortWMA.ReceiveTick(Val);
			if (longWMA.isPrimed())
			{
				outerWMA.ReceiveTick(2*shortWMA.Value()-longWMA.Value());
			}
		}

		public double Value()
		{
			double v;
			
			if (isPrimed())
			{
				v = outerWMA.Value();
			}
			else
			{
				v = 0;
			}

			return v;
		}

		public bool isPrimed()
		{	
			return outerWMA.isPrimed();
		}
	}

	public class iWMA
	{
		private int periods;
		private CQueue values;

		public iWMA(int pPeriods)
		{
			periods = pPeriods;
			values  = new CQueue(periods);
		}

		public void ReceiveTick(double Val)
		{
			values.Add(Val);
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{
				double sum=0;

				for (int i=0; i< periods; i++)
				{
					sum += i+1;
				}

				for (int i=0; i< periods; i++)
				{
					v = v + (double)values.GetItem(i)*((periods-i)/sum);
				}
			}

			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (values.isPrimed())
			{
				v = true;
			}
			return v;
		}
	}


	public class iSMA
	{
		private int tickcount;
		private int periods;
		private double[] values;

		public iSMA(int pPeriods)
		{
			periods = pPeriods;
			values  = new double[periods];
		}

		public void ReceiveTick(double Val)
		{
			values[(tickcount % periods)]=Val;

			tickcount++;
			if (tickcount == (3*periods))
			{
				// avoid overflow by restricting range of tickcount
				// when indicator is fully primed

				tickcount = (2*periods);
			}
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{
				for (int i=0; i< periods; i++)
				{
					v = v + values[i];
				}

				v = v / periods;
			}

			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (tickcount >= periods)
			{
				v = true;
			}
			return v;
		}
	}

	public class iMomemtum
	{
		int tickcount;
		int Periods;     

		double[] Buffer;
		
		public iMomemtum(int pPeriods)
		{
			tickcount = 0;
			Periods = pPeriods;

			Buffer = new double[pPeriods];
		}

		public void ReceiveTick(double pC)
		{

			Buffer[tickcount % Periods] = pC;

			tickcount++;
			if (tickcount == (3*Periods))
			{
				// avoid overflow by restricting range of tickcount
				// when indicator is fully primed
				tickcount = 2*Periods;
			}
		}

		// 0 = newest candle   4 = oldest candle
		private int CandleIndex(int candle, int tickcount)
		{
			int index = 0;
			int position = tickcount % Periods;

			if ((position-(candle+1))>=0)
			{
				index = position - (candle+1);
			}
			else
			{
				index = Periods - Math.Abs(position - (candle+1));
			}
				
			return index;
		}


		public double Value()
		{
			int i0 =  CandleIndex(0,tickcount);
			int ip =  CandleIndex(Periods-1,tickcount);

			double v0 = Buffer[i0];
			double vp = Buffer[ip];

			double mom = 0;

			if (tickcount > Periods)
			{
				if (vp==0)
				{
					mom = 100;
				}
				else
				{
					mom = v0*100/vp; 
				}
			}

			return mom;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (tickcount > Periods)
			{
				v = true;
			}
			return v;
		}
	}

	
	public class iStochRSI
	{
		iRSI RSI;
		iStochastics Sto;
		
		public iStochRSI(int pPeriods)
		{
			RSI = new iRSI(pPeriods);
			Sto = new iStochastics(pPeriods,1,3);
		}

		public void ReceiveTick(double pC)
		{
			RSI.ReceiveTick(pC);
			if (RSI.isPrimed())
			{
				double v = RSI.Value();
				Sto.ReceiveTick(v,v,v,v);
			}
		}

		public double Value()
		{
			double K=0;
			double S=0;

			if (isPrimed())
			{
				Sto.Value(out K, out S);
			}

			return K;
		}

		public bool isPrimed()
		{	
			return Sto.isPrimed();
		}
	}


	public class iRSI
	{
		int tickcount;
		int Periods;     

		double[] Gain;
		double[] Loss;

		double   prevGain;
		double   prevLoss;
		
		double   prevC;
		double   RS;

		public iRSI(int pPeriods)
		{
			tickcount = 0;
			Periods = pPeriods;
			prevC = -1;
			RS = -1;

			Gain = new double[pPeriods];
			Loss = new double[pPeriods];
		}

		public void ReceiveTick(double pC)
		{
			if (prevC==-1)
			{
				prevC = pC;
			}

			if ((pC-prevC)>0)
			{
				Gain[tickcount % Periods] = pC - prevC;
			}
			else
			{
				Gain[tickcount % Periods] = 0;
			}

			if ((prevC-pC)>0)
			{
				Loss[tickcount % Periods] = prevC - pC;
			}
			else
			{
				Loss[tickcount % Periods] = 0;
			}

			tickcount++;
			if (tickcount == (3*Periods))
			{
				// avoid overflow by restricting range of tickcount
				// when indicator is fully primed
				tickcount = 2*Periods;
			}

			prevC = pC;
			
		}

		public double Value()
		{
			double AvgLoss=0;
			double AvgGain=0;

			double RSI;

			if ( isPrimed() )
			{
				for (int i=0; i<Periods; i++)
				{
					AvgLoss += Loss[i];
					AvgGain += Gain[i];
				}

				AvgLoss /= Periods;
				AvgGain /= Periods;

				if (RS==-1)
				{
					if (AvgLoss == 0)
					{
						RS = 100;
					}
					else
					{
						RS = AvgGain / AvgLoss;
					}
				}
				else
				{
					RS = (((prevGain*(Periods-1))+AvgGain)/Periods) / ((((prevLoss*(Periods-1))+AvgLoss)/Periods));
				}

				prevGain = AvgGain;
				prevLoss = AvgLoss;
			
				RSI = RS;

			}	
			else
			{
				RSI = 0;
			}

			return RSI;

		}

		public bool isPrimed()
		{	
			bool v = false;

			if ( tickcount >= Periods )
			{
				v = true;
			}

			return v;
		}
	}

	public class iStochastics
	{
		int tickcount;
		int KPeriods;
		int KSPeriods;
		int DPeriods;     

		double[] H;
		double[] L;
		double   C;

		double K;
		iSMA    KSmooth;
		iSMA    D;


		// restriction: pKPeriods > max(pKSPeriods,pDPeriods)
		public iStochastics(int pKPeriods, int pKSPeriods, int pDPeriods)
		{
			KPeriods = pKPeriods;
			KSPeriods = pKSPeriods;
			DPeriods = pDPeriods;

			H = new double[KPeriods];
			L = new double[KPeriods];

			KSmooth = new iSMA(KSPeriods);
			D = new iSMA(DPeriods);
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			double min,max;
			
			H[tickcount % KPeriods]=pH;
			L[tickcount % KPeriods]=pL;
			C = pC;

			if (tickcount >= KPeriods)
			{
				max = H[0];
				min = L[0];

				for (int i=0; i< KPeriods; i++)
				{
					if ( H[i]>max) 
						max = H[i];
					if ( L[i]<min) 
						min = L[i];
				}

				K = 100 * ( (C-min)/(max-min + 0.000000001)); 

				KSmooth.ReceiveTick(K);
				if (KSmooth.Value() != 0)
				{
					D.ReceiveTick(KSmooth.Value());
				}
			}

			tickcount++;
			if (tickcount == (4*KPeriods))
			{
				// avoid overflow by restricting range of tickcount
				// when indicator is fully primed

				// 3*KPeriods satisfies ( tickcount > (KPeriods+KSPeriods+DPeriods) )
				// when (KPeriods > KSPeriods) and (KPeriods > DPeriods)

				tickcount = 3*KPeriods;
			}

		}

		public void Value(out double Stochastics, out double Signal)
		{
			if ( isPrimed() )
			{
				Stochastics = KSmooth.Value();
				Signal      = D.Value();
			}	
			else
			{
				Stochastics = 0;
				Signal = 0;
			}
		}

		public bool isPrimed()
		{	
			bool v = false;

			if ( tickcount > (KPeriods + KSPeriods + DPeriods) )
			{
				v = true;
			}

			return v;
		}
	}

	public class iRenko
	{
		private double width;
		private double closing;
		private double opening;
		private int lastbricks;

		public iRenko(double pWidth)
		{
			width = pWidth;
			closing = 0;
			lastbricks = 0;
		}

		public void ReceiveTick(double Val)
		{
			if (Val!=0)
			{

				lastbricks = 0;

				if (closing==0)
				{
					opening = Val;
					closing = Val;
				}

				if ( ( Val - Math.Max(opening,closing)) >= width )
				{
					lastbricks = (int)( (Val - Math.Max(opening,closing)) /width );					
					closing = Math.Max(opening,closing) + (lastbricks*width);
				}

				if ( (Math.Min(opening,closing) - Val ) >= width )
				{
					lastbricks = (int)( (Val - Math.Min(opening,closing)) / width );					
					closing = Math.Min(opening,closing) + (lastbricks*width);
				}
				
				if (lastbricks != 0)
				{
					opening = closing - Math.Sign(lastbricks)*width;
				}

			}
		}

		public int Value()
		{
			int v= lastbricks;
			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (!v)
			{
				v = true;
			}
			return v;
		}
	}

	public class iSlope
	{
		private double v1,v2;
		private bool first;

		public iSlope()
		{
			first = true;
		}

		public void ReceiveTick(double Val)
		{
			if (first==true)
			{
				first = false;
				v1 = Val;
				v2 = Val;
			}
			else
			{
				v1 = v2;
				v2 = Val;
			}
		}

		public double Value()
		{
			double v=0;

			if (v1>v2)
			{
				v=-1;
			}
			else if (v1<v2)
			{
				v=1;
			}
			else
			{
				v=0;
			}

			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (!first)
			{
				v = true;
			}
			return v;
		}
	}

	public class iTrend	
	{
		private int maxcandles;
		private int tickcount;
		sCandle[] Candles;		

		// restriction, pMaxCandles >= 3
		public iTrend(int pMaxCandles)
		{
			maxcandles = pMaxCandles;
			Candles  = new sCandle[maxcandles];
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			sCandle Candle = new sCandle();

			Candle.O = pO;
			Candle.H = pH;
			Candle.L = pL;
			Candle.C = pC;

			Candles[(tickcount % maxcandles)]=Candle;

			// avoid overflow by restricting range of tickcount
			// when indicator is fully primed
			tickcount++;
			if (tickcount == (3*maxcandles))
			{
				tickcount = (2*maxcandles);
			}
		}

		// 0 = newest candle   4 = oldest candle
		private int CandleIndex(int candle, int tickcount)
		{
			int index = 0;
			int position = tickcount % maxcandles;

			if ((position-(candle+1))>=0)
			{
				index = position - (candle+1);
			}
			else
			{
				index = maxcandles - Math.Abs(position - (candle+1));
			}
				
			return index;
		}

		public double Value()
		{
			double v=0;
			double slope=0;
			
			if (isPrimed())
			{
				double O = Candles[CandleIndex(0,tickcount)].O;
				double H = Candles[CandleIndex(0,tickcount)].H;
				double L = Candles[CandleIndex(0,tickcount)].L;
				double C = Candles[CandleIndex(0,tickcount)].C;

				bool trend = true;
				int i;

				//Utils.Logger(0,"Trend Detection");

				for (i=2,v=i-1; i< maxcandles; i++)
				{
					double O1 = Candles[CandleIndex(i,tickcount)].O;
					double H1 = Candles[CandleIndex(i,tickcount)].H;
					double L1 = Candles[CandleIndex(i,tickcount)].L;
					double C1 = Candles[CandleIndex(i,tickcount)].C;
										
					double TP = (1/3.0)*(H+L+C);
					double TP1 = (1/3.0)*(H1+L1+C1);

					double Dy = (TP-TP1)/i;

					//Utils.Logger(0,"TP: "+TP+" TP1: "+TP1);

					for (int j=1; j<i; j++)
					{
						double Hi = Candles[CandleIndex(j,tickcount)].H;
						double Li = Candles[CandleIndex(j,tickcount)].L;

						double  y = TP-(Dy*j);
						
						if (!((Hi>=y) && (y>=Li)))
						{
							trend = false;
						}
						else
						{
						
						}
						//Utils.Logger(0,"Hi: "+Hi+" y: "+y+" Li:"+Li+" Trend: "+trend);

						if (trend==false)
						{
							break;
						}
					}

					if (trend==true)
					{
						slope = Math.Sign(Dy);
						v++;
					}
					else
					{
						break;
					}
				}
			}

			v = (v-1)*slope;
			//Utils.Logger(0,"Trend Detected: "+v);
			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;
			if (tickcount > maxcandles)
			{
				v = true;
			}
			return v;
		}
	}

	public class iMinMax
	{
		int periods;
		CQueue queue;

		double min,max;

		public iMinMax(int pPeriods)
		{
			periods = pPeriods;
			queue   = new CQueue(periods);
		}

		public void ReceiveTick(double Val)
		{
			queue.Add(Val);
			CalculateMinMax();
		}

		private void CalculateMinMax()
		{
			double v;

			if (isPrimed())
			{
				v = (double)queue.GetItem(0);
				min = v;
				max = v;

				for (int i=1; i<periods; i++)
				{
					v = (double) queue.GetItem(i);

					if (v>max)
					{
						max = v;
					}

					if (v<min)
					{
						min = v;
					}
				}
			}
		}
		
		public double Min()
		{
			double v=0;

			if (isPrimed())
			{
				v = min;
			}

			return v;
		}

		public double Max()
		{
			double v=0;

			if (isPrimed())
			{
				v = max;
			}

			return v;
		}

		public bool isPrimed()
		{	
			bool v = false;

			if (queue.isPrimed())
			{
				v = true;
			}

			return v;
		}
	}


	public class iADX
	{
		int periods;
		CQueue Prices;

		public iADX(int pPeriods)
		{
			periods = pPeriods;
			Prices = new CQueue(periods);
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			sCandle candle = new sCandle();

			candle.O = pO;
			candle.H = pH;
			candle.L = pL;
			candle.C = pC;

			Prices.Add(candle);
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{





			}

			return v;
		}

		// returns true when indicator has seen enough values to compute a result
		// it will never return true if initialization parameters are incorrect
		// this will cause indicator to silently fail and not give results
		// on optimization runs where parameter combination is invalid.
		public bool isPrimed()
		{	
			bool v = false;

			if (Prices.isPrimed())
			{
				v = true;
			}

			return v;
		}
	}

	public class iTrueRange
	{
		sCandle PreviousCandle;
		sCandle	CurrentCandle;

		public iTrueRange()
		{
			PreviousCandle = null;
			CurrentCandle = null;
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			sCandle candle = new sCandle();

			PreviousCandle = CurrentCandle;

			candle.O = pO;
			candle.H = pH;
			candle.L = pL;
			candle.C = pC;

			CurrentCandle = candle;
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{
				double range1 = CurrentCandle.H - CurrentCandle.L;
				double range2 = Math.Abs(CurrentCandle.H - PreviousCandle.C);
				double range3 = Math.Abs(CurrentCandle.L - PreviousCandle.C);

				v = Math.Max(range1,Math.Max(range2,range3));
			}

			return v;
		}

		// returns true when indicator has seen enough values to compute a result
		// it will never return true if initialization parameters are incorrect
		// this will cause indicator to silently fail and not give results
		// on optimization runs where parameter combination is invalid.
		public bool isPrimed()
		{	
			bool v = false;

			if (PreviousCandle != null)
			{
				v = true;
			}

			return v;
		}
	}

	public class iATR
	{
		int periods;
		double average;

		iTrueRange TR;

		public iATR(int pPeriods)
		{
			periods = pPeriods;
			average = 0;

			TR = new iTrueRange();
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			TR.ReceiveTick(pO,pH,pL,pC);
		}

		public double Value()
		{
			double v=0;

			if (isPrimed())
			{
				if (average==0)
				{
					average = TR.Value();
				}
				else
				{
					average = ( average*(periods-1) + TR.Value() ) / periods;
				}

				v = average;
			}

			return v;
		}

		// returns true when indicator has seen enough values to compute a result
		// it will never return true if initialization parameters are incorrect
		// this will cause indicator to silently fail and not give results
		// on optimization runs where parameter combination is invalid.
		public bool isPrimed()
		{	
			bool v = false;

			if (TR.isPrimed())
			{
				v = true;
			}

			return v;
		}
	}

	public class iSTARCBands
	{
		int periods;
		double width;

		iATR ATR;
		iSMA SMA;

		public iSTARCBands(int pPeriods, double pWidth)
		{
			periods = pPeriods;
			width   = pWidth;

			ATR = new iATR(periods);
			SMA = new iSMA(periods);
		}

		public void ReceiveTick(double pO, double pH, double pL, double pC)
		{
			ATR.ReceiveTick(pO,pH,pL,pC);
			SMA.ReceiveTick((pH+pL+pC)/3);
		}

		public void Value(out double SPlusv, out double SMAv, out double SMinusv)
		{
			if (isPrimed())
			{
				SMAv    = SMA.Value();
				SPlusv  = SMAv + width*ATR.Value();
				SMinusv = SMAv - width*ATR.Value();
			}
			else
			{
				SMAv    = 0;
				SPlusv  = 0;
				SMinusv = 0;
			}
		}

		// returns true when indicator has seen enough values to compute a result
		// it will never return true if initialization parameters are incorrect
		// this will cause indicator to silently fail and not give results
		// on optimization runs where parameter combination is invalid.
		public bool isPrimed()
		{	
			bool v = false;

			if (ATR.isPrimed() && SMA.isPrimed())
			{
				v = true;
			}

			return v;
		}
	}


}