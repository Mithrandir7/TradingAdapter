using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Imsl.Chart2D;
using System.Drawing;
using Imsl.Stat;
using DataManager;

namespace tradebox
{
    class OIChart : FrameChart
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int previousOI = -1;

        private SortedDictionary<int, int> oiHistInt = new SortedDictionary<int, int>();

        private SortedDictionary<int, int> oiHistRecent = new SortedDictionary<int, int>();
        private List<int> tradeList = new List<int>();
        private List<int> oiList = new List<int>();

        private const int moniterSeconds = 600;

        private string chartName = "NA";
        Chart chart;
        AxisXY axisxy;
        Data data, data_trade, data_zero, data_recent, data_bid, data_ask;

        public OIChart(string aName)
        {
            chartName = aName;
            this.Text = chartName;
            this.init();
        }    

        private void init()
        {
            Show();          
            chart = this.Chart;            
            chart.Background.FillColor = System.Drawing.Color.LightGray;
            axisxy = new AxisXY(chart);
            //axis.AxisX.MajorTick.LineWidth = 5.0;
            axisxy.AxisX.AutoscaleInput = Data.AUTOSCALE_OFF;
            //axisxy.AxisX.AutoscaleOutput = Data.AUTOSCALE_OFF;

            data = new Data(axisxy);
            data.DataType = Data.DATA_TYPE_LINE;
            data.SetLineDashPattern(Data.DASH_PATTERN_DASH);

            //data.MarkerType = Data.MARKER_TYPE_CIRCLE_CIRCLE;
            //data.MarkerColor = System.Drawing.Color.Black;

            data_zero = new Data(axisxy);
            data_zero.LineColor = System.Drawing.Color.Red;
            data_zero.DataType = Data.DATA_TYPE_LINE;

            data_trade = new Data(axisxy);
            data_trade.LineColor = System.Drawing.Color.Tomato;
            data_trade.DataType = Data.DATA_TYPE_LINE;
            data_trade.LineWidth = 3;

            data_bid = new Data(axisxy);
            data_bid.LineColor = System.Drawing.Color.Black;
            data_bid.DataType = Data.DATA_TYPE_LINE;
            data_bid.SetLineDashPattern(Data.DASH_PATTERN_DOT);

            data_ask = new Data(axisxy);
            data_ask.LineColor = System.Drawing.Color.Black;
            data_ask.DataType = Data.DATA_TYPE_LINE;
            data_ask.SetLineDashPattern(Data.DASH_PATTERN_DOT);

            data_recent = new Data(axisxy);
            data_recent.LineColor = System.Drawing.Color.Blue;
            data_recent.DataType = Data.DATA_TYPE_LINE | Data.DATA_TYPE_MARKER;
            data_recent.MarkerType = Data.MARKER_TYPE_CIRCLE_PLUS;
            data_recent.MarkerColor = System.Drawing.Color.Blue;

            axisxy.AxisX.AxisTitle.SetTitle("Time Index");
            axisxy.AxisY.AxisTitle.SetTitle("Trade Price");           
            //axisxy.AutoscaleMinimumTimeInterval = AxisXY.SECOND;                     
        }

        private double[] int2Double(int[] val)
        {
            var ret = new double[val.Length];
            for (int i = 0; i < val.Length; i++)
            {               
                ret[i] = Convert.ToDouble(val[i]);                
            }
            return ret;
        }

        private void drawZeroLine(double lmin, double lmax)
        {
            double[] zx = new double[2];
            zx[0] = lmin;
            zx[1] = lmax;

            double[] zy = new double[2];
            zy[0] = 0;
            zy[1] = 0;

            data_zero.SetX(zx);
            data_zero.SetY(zy);
        }

        private void drawTradeLine(TickQuote aTick,double lmin, double lmax)
        {

            //logger.Info("trade;bid;ask = " + aTick.trade + ";" + aTick.bid + ";" + aTick.ask);

            double[] zx = new double[2];
            zx[0] = aTick.trade;
            zx[1] = aTick.trade;

            double[] zy = new double[2];
            zy[0] = lmin;
            zy[1] = lmax;

            data_trade.SetX(zx);
            data_trade.SetY(zy);

            double[] zxBid = new double[2];
            zxBid[0] = aTick.bid;
            zxBid[1] = aTick.bid;

            data_bid.SetX(zxBid);
            data_bid.SetY(zy);

            double[] zxAsk = new double[2];
            zxAsk[0] = aTick.ask;
            zxAsk[1] = aTick.ask;

            data_ask.SetX(zxAsk);
            data_ask.SetY(zy);

        }

        private void reDrawData(TickQuote aTick)
        {
            double[] ly = int2Double(oiHistInt.Values.ToArray());
            double[] lx = int2Double(oiHistInt.Keys.ToArray());

            data.SetY(ly);
            data.SetX(lx);


            double[] ly_recent = int2Double(oiHistRecent.Values.ToArray());
            double[] lx_recent = int2Double(oiHistRecent.Keys.ToArray());

            data_recent.SetY(ly_recent);
            data_recent.SetX(lx_recent);

            double lmin = lx.Min() - 5;
            double lmax = lx.Max() + 5;
            double ymin = ly.Min();
            double ymax = ly.Max();

            drawZeroLine(lmin, lmax);
            drawTradeLine(aTick, ymin, ymax);

            
            axisxy.AxisX.SetWindow(lmin, lmax);
            this.Refresh();
        }


        private void rebuildOiHistRecent()
        {
            oiHistRecent.Clear();
            for(int i=0; i<tradeList.Count; i++)
            {
                int tradeLevel = tradeList[i];
                int oiDiff = oiList[i];

                if (oiHistRecent.ContainsKey(tradeLevel))
                {
                    oiHistRecent[tradeLevel] = oiHistRecent[tradeLevel] + oiDiff;
                }
                else
                {
                    oiHistRecent.Add(tradeLevel, oiDiff);
                }
            }
        }


        public void addData(TickQuote aTick)
        {
            int oiDiff = 0;
            int tradeLevel = 0;

            if (previousOI < 0)
            {
                previousOI = Convert.ToInt32(aTick.oi);
                return;
            }
            else
            {
                oiDiff = Convert.ToInt32(aTick.oi) - previousOI;
                previousOI = Convert.ToInt32(aTick.oi);                
            }

            tradeLevel = Convert.ToInt32(Math.Round(aTick.trade));

            if (oiHistInt.ContainsKey(tradeLevel))
            {
                oiHistInt[tradeLevel] = oiHistInt[tradeLevel] + oiDiff;
            }
            else
            {
                oiHistInt.Add(tradeLevel, oiDiff);
            }

            
            tradeList.Add(tradeLevel);
            oiList.Add(oiDiff);

            if (tradeList.Count > moniterSeconds)
            {
                tradeList.RemoveAt(0);
                oiList.RemoveAt(0);
            }
            rebuildOiHistRecent();

            reDrawData(aTick);
            
        }

    }
}
