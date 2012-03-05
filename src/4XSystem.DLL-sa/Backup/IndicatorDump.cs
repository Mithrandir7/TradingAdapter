
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{
	public class oIndicatorDump
	{
		// the initialization values for the indicators and candles
		int iPeriods;
		sCandle Candle;

		// indicators
		iATR			ATR;
		iBollingerBands BB;
		iCCI			CCI;
		iDerivatives	Derivatives;
		iEMA			EMA;
		iFMA			FMA;
		iHMA			HMA;
		iMACD			MACD;
		iMomemtum		Momemtum;
		iRSI			RSI;
		iRenko			Renko;
		iSMA			SMA;
		iSTARCBands		STARCBands;
		iSTDDEV			STDDEV;
		iSlope			Slope;
		iStochRSI		StochRSI;
		iStochastics	Stochastics;
		iStub			Stub;
		iTrend			Trend;
		iTrueRange		TrueRange;
		iWMA			WMA;

		// constructor, called only once, setup multiple tick variables
		public oIndicatorDump(int pPeriods)
		{
			iPeriods = pPeriods;

			ATR				= new iATR(pPeriods);
			BB				= new iBollingerBands(iPeriods,-1);
			CCI				= new iCCI(iPeriods);
			Derivatives		= new iDerivatives();
			EMA				= new iEMA(iPeriods);
			FMA				= new iFMA(iPeriods);
			HMA				= new iHMA(iPeriods);
			MACD			= new iMACD(12,26,9);
			Momemtum		= new iMomemtum(iPeriods);
			RSI				= new iRSI(iPeriods);
			Renko			= new iRenko(iPeriods);
			SMA				= new iSMA(iPeriods);
			STARCBands		= new iSTARCBands(iPeriods,2);
			STDDEV			= new iSTDDEV(iPeriods);
			Slope			= new iSlope();
			StochRSI		= new iStochRSI(iPeriods);
			Stochastics		= new iStochastics(3,2,1);
			Stub			= new iStub(iPeriods);
			Trend			= new iTrend(iPeriods);
			TrueRange		= new iTrueRange();
			WMA				= new iWMA(iPeriods);		
		}

		public string GetHeader()
		{
			string Header;

			Header = 
				"O,H,L,C,"+
				"ATRPrimed,ATR,"+
				"BBPrimed,B+,SMA,B-,%b,BW,"+
				"CCIPrimed,CCI,"+
				"DerPrimed,D1,D2,"+
				"EMAPrimed,EMA,"+
				"FMAPrimed,FMA,"+
				"HMAPrimed,HMA,"+
				"MACDPrimed,MACD,Signal,Hist,"+
				"MomPrimed,Mom,"+
				"RSIPrimed,RSI,"+
				"RenkoPrimed,Renko,"+
				"SMAPrimed,SMA,"+
				"STARCPrimed,STARC+,SMA,STARC-,"+
				"STDDEVPrimed,STDv,"+
				"SlopePrimed,Slope,"+
				"StoRSIPrimed,StoRSI,"+
				"StochPrimed,STOv,STOs,"+
				"StubPrimed,Value,"+
				"TrendPrimed,Trend,"+
				"TrueRangePrimed,TrueRange,"+
				"WMAPrimed,WMA";

			return Header;
		}
		
		public void ReceiveCandle(sCandle pCandle)
		{
			Candle = pCandle;

			ATR.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
			BB.ReceiveTick(pCandle.C);
			CCI.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
			Derivatives.ReceiveTick(pCandle.C);
			EMA.ReceiveTick(pCandle.C);
			FMA.ReceiveTick(pCandle.C);
			HMA.ReceiveTick(pCandle.C);
			MACD.ReceiveTick(pCandle.C);
			Momemtum.ReceiveTick(pCandle.C);
			RSI.ReceiveTick(pCandle.C);
			Renko.ReceiveTick(pCandle.C);
			SMA.ReceiveTick(pCandle.C);
			STARCBands.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
			STDDEV.ReceiveTick(pCandle.C);
			Slope.ReceiveTick(pCandle.C);
			StochRSI.ReceiveTick(pCandle.C);
			Stochastics.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
			Stub.ReceiveTick(pCandle.C);
			Trend.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
			TrueRange.ReceiveTick(pCandle.O,pCandle.H,pCandle.L,pCandle.C);
			WMA.ReceiveTick(pCandle.C);
		}

		public string GetValues()
		{
			string Values;

			double BPlus,BSMA,BMinus,PctB,BW;
			double Deriv1,Deriv2;
			double MACDv,MACDs,MACDh;
			double STARCPlus,STARCSMA,STARCMinus;
			double STOv,STOs;

			double ATRv = ATR.Value();
			BB.Value(out BPlus, out BSMA, out BMinus, out PctB, out BW);
			double CCIv	= CCI.Value();
			Derivatives.Value(out Deriv1, out Deriv2);
			double EMAv	= EMA.Value();
			double FMAv	= FMA.Value();
			double HMAv	= HMA.Value();
			MACD.Value(out MACDv,out MACDs, out MACDh);
			double MOMv	= Momemtum.Value();
			double RSIv	= RSI.Value();
			double RKOv	= Renko.Value();
			double SMAv	= SMA.Value();
			STARCBands.Value(out STARCPlus,out STARCSMA,out STARCMinus);
			double STDv	= STDDEV.Value();
			double SLPv	= Slope.Value();
			double SRSv	= StochRSI.Value();
			Stochastics.Value(out STOv, out STOs);
			double STv	= Stub.Value();
			double TRDv	= Trend.Value();
			double TRv  = TrueRange.Value();
			double WMAv	= WMA.Value();

			string ATRp = ATR.isPrimed().ToString();
			string BBp = BB.isPrimed().ToString();
			string CCIp	= CCI.isPrimed().ToString();
			string DERp = Derivatives.isPrimed().ToString();
			string EMAp	= EMA.isPrimed().ToString();
			string FMAp	= FMA.isPrimed().ToString();
			string HMAp	= HMA.isPrimed().ToString();
			string MACp = MACD.isPrimed().ToString();
			string MOMp	= Momemtum.isPrimed().ToString();
			string RSIp	= RSI.isPrimed().ToString();
			string RKOp	= Renko.isPrimed().ToString();
			string SMAp	= SMA.isPrimed().ToString();
			string STCp = STARCBands.isPrimed().ToString();
			string STDp	= STDDEV.isPrimed().ToString();
			string SLPp	= Slope.isPrimed().ToString();
			string SRSp	= StochRSI.isPrimed().ToString();
			string STOp = Stochastics.isPrimed().ToString();
			string STp	= Stub.isPrimed().ToString();
			string TRDp	= Trend.isPrimed().ToString();
			string TRp  = TrueRange.isPrimed().ToString();
			string WMAp	= WMA.isPrimed().ToString();
	
			Values = 
				Candle.O+","+Candle.H+","+Candle.L+","+Candle.C+","+
				ATRp+","+ATRv+","+
				BBp+","+BPlus+","+BSMA+","+BMinus+","+PctB+","+BW+","+
				CCIp+","+CCIv+","+
				DERp+","+Deriv1+","+Deriv2+","+
				EMAp+","+EMAv+","+
				FMAp+","+FMAv+","+
				HMAp+","+HMAv+","+
				MACp+","+MACDv+","+MACDs+","+MACDh+","+
				MOMp+","+MOMv+","+
				RSIp+","+RSIv+","+
				RKOp+","+RKOv+","+
				SMAp+","+SMAv+","+
				STCp+","+STARCPlus+","+STARCSMA+","+STARCMinus+","+
				STDp+","+STDv+","+
				SLPp+","+SLPv+","+
				SRSp+","+SRSv+","+
				STOp+","+STOv+","+STOs+","+
				STp+","+STv+","+
				TRDp+","+TRDv+","+
				TRp+","+TRv+","+
				WMAp+","+WMAv;

			return Values;
		}

	}


}