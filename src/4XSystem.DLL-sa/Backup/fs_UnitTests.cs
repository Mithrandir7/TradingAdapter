
//
// 4X Lab.NET © Copyright 2005-2006 ASCSE LLC
// http://www.4xlab.net
// email: 4xlab@4xlab.net
//

using System;

namespace _4XLab.NET
{

	public class fUnitTester: ForexObject
	{
		public fUnitTester()
		{
			Framework.Logger(0,"Constructor Called");
		}

		public override void Init(string pParameters)
		{
			Framework.Logger(0,"Init Called");
			Parameters = pParameters;
			UnitTests.PerformTests();
			//Framework.TickServer.RegisterTickListener("System",this);
		}

		public override void ReceiveTick(sTick pTick)
		{
			Framework.Logger(99,"ERROR: ReceiveTick should never be called");
		}

		public override string Title()
		{
			return "Unit Tester";
		}
	}

	public class UnitTests
	{
		static string throwexception;

		public static void TestCQueue()
		{
			Framework.Logger(2,"Circular Queue Test Started");

			CQueue cq = new CQueue(4);

			cq.Add("1");
			cq.Add("2");
			cq.Add("3");
			cq.Add("4");

			string s1;
			s1 = (string)cq.GetItem(4);
			s1 = (string)cq.GetItem(0);
			s1 = (string)cq.GetItem(3);

			cq.Add("5");
			cq.Add("6");
			cq.Add("7");

			s1 = (string)cq.GetItem(4);
			s1 = (string)cq.GetItem(0);
			s1 = (string)cq.GetItem(3);

			Framework.Logger(2,"Circular Queue Test Ended");
		}

		public static void TestLoadFile()
		{
			Framework.Logger(2,"LoadFile Test");
			
			// this should succeed even if the file is opened in excel
			System.Collections.ArrayList file1;
			file1 = Framework.LoadFile("EURUSDTicksWeek.csv");

			System.Collections.ArrayList file2;
			file2 = Framework.LoadFile("NonExistentFile.csv");
		}

		public static void TestSaveFile()
		{
			Framework.Logger(2,"SaveFile Test");

			System.Collections.ArrayList file1;
			file1 = new System.Collections.ArrayList();
			for (int i=0; i<20; i++)
			{
				file1.Add("Item"+i);
			}

			Framework.SaveFile("TestFile.csv",file1);
		}

		public static void InstantiateForexObject()
		{
			Framework.Logger(2,"Instantiation of two ForexObject Test");

			ForexObject fo1 = new ForexObject();
			ForexObject fo2 = new ForexObject();

			Framework.Logger(0,fo1.Title());
			Framework.Logger(0,fo2.Title());
		}

		public static void ParameterParser()
		{
			Framework.Logger(2,"Parameter Parsing Test");

			/*
				MACDF:8-16,2; MACDS:12-24,2; MACDSG:7-9; EMA:26; STOK:3; STOKS:2; STOD:3;
				MACDF:12; macds:26; MACDSG: 9 ; EMA: 26 ; STOK:3 ; STOKS:2 ; STOD:3
				P: 14
				IN: *.CSV ; OUT: *.CSV
			 */

			string opt = "MACDF:8-14,2; MACDS:12-16,2; MACDSG:7-9; EMA:26; STOK:3; STOKS:2-3,0.2; STOD:3; FLAG:TRUE";
			string val1 = "MACDF:12; macds:26; MACDSG: 9 ; EMA: 26 ; STOK:3 ; STOKS:2 ; STOD:3; TRUE-RANGE:TRUE";

			ParameterParser pp1 = new ParameterParser(opt);
			ParameterParser pp2 = new ParameterParser(val1);
			ParameterParser pp3 = new ParameterParser("IN: *.CSV ; OUT: *.CSV");

			double v1 = pp1.GetDouble("flag",-1);
			string v="";

			Framework.Logger(0,"started opt with: "+opt);
			while (pp1.GetOptimizationParameters(ref v))
			{
				Framework.Logger(0,v);
			}

			Framework.Logger(0,"started opt with: "+val1);
			while (pp2.GetOptimizationParameters(ref v))
			{
				Framework.Logger(0,v);
			}

			string infile = pp3.GetString("in",null);
			string outfile = pp3.GetString("out",null);
		}

		public static void VerboseStack()
		{
			Framework.Logger(2,"Verbose Stack Test");
			Framework.PushVerbose(0);
			Framework.Logger(0,"0: This will be shown");

			Framework.PushVerbose(2);
			Framework.Logger(2,"1: We are now in level 2 verbose");
			Framework.Logger(0,"this low level message will not be shown");

			Framework.PushVerbose(100);
			Framework.Logger(99,"This error message will not be shown");

			Framework.PopVerbose(); // back to level 2
			Framework.Logger(99,"2: This error message will be shown");
			Framework.Logger(2,"3: This level 2 message will be shown");
			Framework.Logger(0,"This low level message will not be shown");

			Framework.PopVerbose(); // back to level 0
			Framework.Logger(0,"4: This low level message will be shown");

			Framework.PopVerbose(); // try stack underflow
			Framework.Logger(0,"5: Survived stack underflow");
		}
		
		public static void TestAccount()
		{
			sTick tick = new sTick();
			cAccount acct = new cAccount("M:1000.02; L:50; A:0.30;");

			// trailstop test

			tick.Bid = 1.1940;
			tick.Ask = 1.19415;

			acct.ReceiveTick(tick);
			acct.EnterTrade(1);
			acct.SetStop(0.0005); // 5 pip stop
			acct.SetTakeProfit(0.0015); // 15 pip take profit
			acct.SetTrailingStop(0.0003); // 3 pip trailing stop

			tick.Bid = 1.1945;
			tick.Ask = 1.19465;
			acct.ReceiveTick(tick); // tighten trailstop

			tick.Bid = 1.1943;
			tick.Ask = 1.19445;
			acct.ReceiveTick(tick); // this should trigger trailstop

			// stop test
			
			tick.Bid = 1.1940;
			tick.Ask = 1.19415;

			acct.ReceiveTick(tick);
			acct.EnterTrade(1);
			acct.SetStop(0.0005); // 5 pip stop
			acct.SetTakeProfit(0.0015); // 15 pip take profit
			acct.SetTrailingStop(0.0003); // 3 pip trailing stop

			tick.Bid = 1.1935;
			tick.Ask = 1.19365;

			acct.ReceiveTick(tick); // this should trigger stop

			// take profit test
			
			tick.Bid = 1.1940;
			tick.Ask = 1.19415;

			acct.ReceiveTick(tick);
			acct.EnterTrade(1);
			acct.SetStop(0.0005); // 5 pip stop
			acct.SetTakeProfit(0.0015); // 15 pip take profit
			acct.SetTrailingStop(0.0003); // 3 pip trailing stop

			tick.Bid = 1.19565;
			tick.Ask = 1.1957;

			acct.ReceiveTick(tick); // this should trigger take profit

		}

		public static void TestHMA()
		{
			iFMA FMA = new iFMA(10);
			for (int i=0; i<10; i++)
			{
				FMA.ReceiveTick(i);
			}

			double v = FMA.Value();   // should be 9.5

			if (Math.Abs(9.5-v)<0.000001)
			{
				Framework.Logger(2,"FMA Returns correct value: 9.5");
			}
		}

		public static void TestWMA()
		{
			iWMA WMA = new iWMA(5);
			WMA.ReceiveTick(16);		
			WMA.ReceiveTick(17);
			WMA.ReceiveTick(17);
			WMA.ReceiveTick(10);
			WMA.ReceiveTick(17);

			double v = WMA.Value();		// should be 15.066

			if (Math.Abs(15.066-v)<0.001)
			{
				Framework.Logger(2,"FMA Returns correct value: 15.06");
			}
		}



		public static void TestCCI()
		{
			iCCI cci = new iCCI(20);

			cci.ReceiveTick(1302.67,1302.67,1263.23,1263.84);
			cci.ReceiveTick(1268.65,1294.65,1261,1292.3101);
			cci.ReceiveTick(1274.76,1274.76,1251,1251);
			cci.ReceiveTick(1280.27,1304.02,1280.27,1295.3);
			cci.ReceiveTick(1286.75,1310.33,1270.73,1304.6);
			cci.ReceiveTick(1306.13,1322.4301,1299.53,1320.09);
			cci.ReceiveTick(1328.35,1347.27,1314.96,1315.45);
			cci.ReceiveTick(1305.72,1305.72,1279.09,1279.6801);
			cci.ReceiveTick(1272.9399,1292.36,1270.59,1291.4);
			cci.ReceiveTick(1286.85,1292.73,1267.6899,1275.88);
			cci.ReceiveTick(1292.91,1298.5,1258.85,1259.9399);
			cci.ReceiveTick(1244.52,1263.9,1233.08,1252.13);
			cci.ReceiveTick(1233.9399,1242.91,1216.1899,1216.45);
			cci.ReceiveTick(1229.47,1232.96,1216.24,1221.09);
			cci.ReceiveTick(1209.13,1209.72,1177.41,1184.9301);
			cci.ReceiveTick(1170.95,1200.45,1169.04,1182.17);
			cci.ReceiveTick(1195.6,1227.23,1184.12,1222.29);
			cci.ReceiveTick(1231.85,1239.62,1206.91,1221.61);

			cci.ReceiveTick(1213.77,1235.08,1198.12,1199.16);
			double v;
			v = cci.Value();
			if ((v!=0) || (cci.isPrimed()))
			{
				Framework.Logger(2,"ERROR: CCI should not be primed yet");	
			}
			
			cci.ReceiveTick(1187.48,1190.74,1160.0699,1172.0601);
			v = cci.Value();
			if (Math.Abs(v-(-136.0742924))<0.001)
			{
				Framework.Logger(2,"CCI returned correct value -136.074");	
			}

			cci.ReceiveTick(1180.26,1214.01,1160.71,1213.72);
			v = cci.Value();
			if (Math.Abs(v-(-87.47))<0.01)
			{
				Framework.Logger(2,"CCI returned correct value -87.47");	
			}
		}

		public static void TestBBands()
		{
			iBollingerBands BB;
			
			BB = new iBollingerBands(10,-1);
			BB = new iBollingerBands(20,-1);
			BB = new iBollingerBands(50,-1);

			BB = new iBollingerBands(21,-1);


			double[] val = {6.92,6.89,6.82,6.82,6.83,6.79,6.75,6.71,6.71,6.66,6.59,6.56,6.59,6.56,6.61,6.68,6.65,6.68,6.54,6.49,6.42};

			for (int i=0; i< val.GetUpperBound(0)+1; i++)
			{
				BB.ReceiveTick(val[i]);
			}

			double BP,MA,BM,pb,bw;
			BB.Value(out BP,out MA,out BM,out pb,out bw);

			if (Math.Abs(MA-6.68)<0.01)
			{
				Framework.Logger(2,"MA Returns correct value: " + MA);
			}

			if (Math.Abs(BP-6.94)<0.01)
			{
				Framework.Logger(2,"BP Returns correct value: " + BP);
			}

			if (Math.Abs(BM-6.41)<0.01)
			{
				Framework.Logger(2,"BM Returns correct value: " + BM);
			}

		}


		public static void PerformTests()
		{
			throwexception = null;

			UnitTests.TestCQueue();
			UnitTests.TestLoadFile();
			UnitTests.TestSaveFile();
			UnitTests.InstantiateForexObject();
			UnitTests.ParameterParser();
			UnitTests.VerboseStack();
			UnitTests.TestAccount();
			UnitTests.TestHMA();
			UnitTests.TestWMA();
			UnitTests.TestCCI();
			UnitTests.TestBBands();

			Framework.Logger(99,"Going into an almost infinite loop! (will throw exception in 20 seconds)");
			System.DateTime a = System.DateTime.Now;

			while(true)
			{
				Framework.Logger(0,"loop!");

				System.TimeSpan t = System.DateTime.Now - a;

				if (t.Seconds > 20)
				{
					throwexception.ToUpper();
				}
			}

		}

	}

}

