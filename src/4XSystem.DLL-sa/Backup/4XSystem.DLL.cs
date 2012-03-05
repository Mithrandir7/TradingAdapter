
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;
using System.Reflection;


namespace _4XLab.NET
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class TradingSystems
	{
		static private System.Collections.ArrayList ForexObjects;

		static TradingSystems()
		{
			ForexObjects = new System.Collections.ArrayList();

			// once your trading system is developed
			// the program must be notified of its existance
			// by adding it here. This lists it in the dropdown
			// and starts to track user parameters for it.

			ForexObjects.Add(new fRandomEntryMomentumExit());
			ForexObjects.Add(new fXChannelBreakout());
			ForexObjects.Add(new fPercentageChange());
			ForexObjects.Add(new fRandomEntryTrailingStop());
			ForexObjects.Add(new fRandomEntryFixedProfit());
			//ForexObjects.Add(new fBollingerA());
			//ForexObjects.Add(new fWoodiesCCI());
			//ForexObjects.Add(new fCandleSystem());
			ForexObjects.Add(new fStochRSISystem());
			//ForexObjects.Add(new fTrendAndTrailingStop());
			//ForexObjects.Add(new fDerivativesSystem());
			ForexObjects.Add(new fUnitTester());
			//ForexObjects.Add(new fStubSystem());
			ForexObjects.Add(new fIndicatorDump());
		}

		static public System.Collections.ArrayList GetSystems()
		{
			return ForexObjects;
		}
	}
}
