using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using KGIbox;

namespace KGIBox
{
    public partial class KGIBOX : Form
    {
        //'委成回用到的欄位, 欄位說明請參照文件
        public const int KEY1 = 350;
        public const int KEY2 = 351;
        public const int FORM_TYPE = 352;
        public const int HEADER = 353;
        public const int FTR_NAME2 = 354;
        public const int MEMO = 355;
        public const int OKSEQNO = 356;
        public const int OD_PRICE2 = 357;
        public const int OD_QTY2 = 358;
        public const int BFR_QTY = 359;
        public const int AFT_QTY = 360;
        public const int FUNCTIONCODE = 361;
        public const int SRC_CODE = 362;
        public const int ORG_QTY2 = 363;
        public const int DEAL_QTY = 364;
        public const int DEAL_QTY2 = 365;
        public const int DEAL_AVGPRICE = 366;
        public const int TRANS_DATE = 159;
        public const int MATCH_TIME = 310;
        //'public const int  BRANCH_ID = 1;
        public const int ORDNO = 147;
        public const int S_OD_SEQ = 143;
        public const int F_OD_SEQ = 208;
        //'public const int  CUST_ID = 39;
        public const int STK_ID = 85;
        public const int FTR_ID = 114;
        public const int FTR_MTH = 115;
        public const int CALLPUT = 116;
        public const int STRIKE_PRICE = 117;
        public const int BUYSELL = 148;
        public const int FTR_ID2 = 130;
        public const int FTR_MTH2 = 131;
        public const int CALLPUT2 = 132;
        public const int STRIKE_PRICE2 = 133;
        public const int BUYSELL2 = 134;
        public const int PRICE_FLAG = 150;
        public const int OPENCLOSE = 163;
        public const int OD_TYPE = 149;
        //'public const int  AGENT_ID = 5;
        public const int ERR_CODE = 68;
        public const int ERR_MSG = 69;
        public const int OD_PRICE = 151;
        public const int OBOD_PRICE = 257;
        public const int OD_QTY = 156;
        public const int OD_KEY = 201;
        public const int F_ORG_SEQ = 204;
        public const int S_ORG_SEQ = 144;
        public const int MARKET_TYPE = 101;
        public const int TRADE_TYPE = 153;
                       
        
        //艾揚下單使用                 '  證券                    期貨         選擇權             複式
        public const int ORDER_ARGS_ROCID = 0;            //'X 身份證字號(登入帳號)           *             *               *
        public const int ORDER_ARGS_PASSWORD = 1;    //     'X 登入密碼                       *             *               *
        public const int ORDER_ARGS_BRANCHID = 2;        // 'X 分公司代號                     *             *               *
        public const int ORDER_ARGS_CUSTID = 3;           //X 帳號                           *             *               *
        public const int ORDER_ARGS_AGENTID = 4;          //X 營業員代碼                     *             *               *
        public const int ORDER_ARGS_SOURCE = 5;        //   'X 來源別                         *             *               *
        public const int ORDER_ARGS_ID = 6;               //'X 商品代碼                       *             *           (第一腳)代碼
        public const int ORDER_ARGS_BS = 7;               //'X 買賣別                         *             *           (第一腳)買賣別
        public const int ORDER_ARGS_ODTYPE = 8;        //   'X 現股(0)/融資(1)/融券(2)        ""      IOC(I)/ROD(R)/FOK(F)    *
        public const int ORDER_ARGS_TRADE_TYPE = 9;  //     'X 普通(N)/盤後(F)/零股(O)        ""            *               *
        public const int ORDER_ARGS_PRICE_FLAG = 10;   //   'X 市價(1), 限價(0)               *             *               *
        public const int ORDER_ARGS_ODPRICE = 11;         //'9 價格                           *             *               *
        public const int ORDER_ARGS_ODQTY = 12;           //'9 數量                           *             *               *
        public const int ORDER_ARGS_ODKEY = 13;           //'X Order Key                      *             *               *
        public const int ORDER_ARGS_OPENCLOSE = 14;    //   'X -                          新倉(O)/平倉(C)   *               *
        public const int ORDER_ARGS_MTH = 15;             //'X -                              -       (第一腳)履約年月      *
        public const int ORDER_ARGS_CP = 16;              //'X -                              -       (第一腳)Call(C)/Put(P)  *
        public const int ORDER_ARGS_STRIKE = 17;        //  '9 -                              -       (第一腳)履約價          *
        public const int ORDER_ARGS_ID2 = 18;             //'X -                              -             -           (第二腳)代碼
        public const int ORDER_ARGS_BS2 = 19;             //'X -                              -             -           (第二腳)買賣別
        public const int ORDER_ARGS_MTH2 = 20;           // 'X -                              -             -           (第二腳)履約年月
        public const int ORDER_ARGS_CP2 = 21;             //'X -                              -             -           (第二腳)CallPut
        public const int ORDER_ARGS_STRIKE2 = 22;        // '9 -                              -             -           (第二腳)履約價
        public const int ORDER_ARGS_MARKETTYPE = 23;  //    '上市(T)/上櫃(O)                  -             -               -
        // '-表示不使用 *表示同左用法
        public const int ORDER_ARGS_EXCHANGE = 24;
        public const int ORDER_ARGS_STOPPRICE = 25;
        public const int ORDER_ARGS_DAYTRADE = 26;

        //'以下為刪單改量用到的欄位
        public const int ORDER_ARGS_ORGSEQ = 27;     // '原始網路單號
        public const int ORDER_ARGS_ORGSOURCE = 28;   //'原始來源別
        public const int ORDER_ARGS_ORDNO = 29;       //委託書號
        public const int ORDER_ARGS_CANCELQTY = 30;   //'取消股數/口數

        public const int ORDER_ARGS_AFTERQTY = 31;
        public const int ORDER_ARGS_TRANSDATE = 32;    //'交易日
        public const int ORDER_ARGS_MATCHQTY = 33; //'成交口數

        //現貨-----------------------------------------------------------------------
        //'取得分公司號
        public static string m_StkBranch = "";
        //'取得帳號
        public static string m_StkAccount = "";
        //取得帳號名稱
        public static string m_StkName = "";
        //取得營業員
        public static string m_StkAgentId = "";
        //期貨---------------------------------------------------------------------
        //    '取得分公司號
        public static string m_Branch = "";
        //    '取得帳號
        public static string m_Account = "";
        //    '取得帳號名稱
        public static string m_Name = "";
        //    '取得營業員
        public static string m_AgentId = "";

       public class TRptData
       {
           public int nType;
           public int nIndex;
           public int nGridNum;
           public String nOrderNo;       
       }

        ArrayList m_OrderReport = new ArrayList();
        ArrayList m_OBOrderReport = new ArrayList();
        ArrayList m_DealReport = new ArrayList();
        ArrayList m_OBDealReport = new ArrayList();


       int m_OrderRow;
       int m_DealRow;
       int m_OBOrderRow;
       int m_OBDealRow;


        public const int ROC_ID = 34;
        public const int CUST_PWD = 35;
        public const int BRANCH_ID = 1;
        public const int CUST_ID = 39;
        public const int CUST_NAME = 40;
        public const int AGENT_ID = 5;
        public const int ACCOUNT_TYPE = 59;


        public const int ITS_REPORT_LOGIN_OK = 1;
        public const int ITS_REPORT_LOGIN_ERROR = 2;

        public KGIBOX()
        {
            InitializeComponent();
            
        }

        private void init()
        {
            PrivateXMLReader.Instance.init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init();
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool login()
        {
            tradeApi.CAType = int.Parse(PrivateXMLReader.Instance.getAttribute("CAType"));
            tradeApi.NeedCA = bool.Parse(PrivateXMLReader.Instance.getAttribute("NeedCA"));
            tradeApi.CAPath = PrivateXMLReader.Instance.getAttribute("CAPath");
            tradeApi.CAPassword = PrivateXMLReader.Instance.getAttribute("CAPassword");

            int loginType = int.Parse(PrivateXMLReader.Instance.getAttribute("loginType"));
            string loginUrl = PrivateXMLReader.Instance.getAttribute("loginURL");
            string serverHost = PrivateXMLReader.Instance.getAttribute("server");
            int serverPort = int.Parse(PrivateXMLReader.Instance.getAttribute("port"));
            int encodingType = int.Parse(PrivateXMLReader.Instance.getAttribute("encodingType"));
            string company = PrivateXMLReader.Instance.getAttribute("company");
            string product = PrivateXMLReader.Instance.getAttribute("product");
            string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            string password = PrivateXMLReader.Instance.getAttribute("password");

            int rtn = tradeApi.Login(loginType, loginUrl, serverHost,
                serverPort, "", 0, company, product,
                ROCID, "", "", password, encodingType);

            switch (rtn)
            {
                case 0:
                    logger.Info("參數錯誤!");
                    return false;
                    
                case 1:
                    logger.Info("登入錯誤!");
                    return false;
                    
                case 2:
                    logger.Info("登入失敗!");
                    return false;
                    
                case 3:
                    logger.Info("登入中!");
                    return true;
 
                default:
                    logger.Info("狀態不明!");
                    return false;
                    
            }

        }


        private void Login_Click(object sender, EventArgs e)
        {
            login();
        }

        private void PlaceFutureOrder()
        {
            string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            string password = PrivateXMLReader.Instance.getAttribute("password");

            object[] varData = new object[28];

            logger.Info("ROCID:" + ROCID);
            varData[ORDER_ARGS_ROCID] = ROCID;//user_身分證字號

            logger.Info("pass:" + password);
            varData[ORDER_ARGS_PASSWORD] = password;//user_password

            varData[ORDER_ARGS_BRANCHID] = m_Branch;//分公司代號
            logger.Info("分公司代號:" + m_Branch);


            varData[ORDER_ARGS_CUSTID] = m_Account;//帳號
            logger.Info("帳號:" + m_Account);


            varData[ORDER_ARGS_AGENTID] = m_AgentId;//AE代號
            logger.Info("AE代號:" + m_AgentId);

            varData[ORDER_ARGS_SOURCE] = "IC";        //'來源別, IC指艾揚, 可任意填上2碼代碼,代表由何處下單

            varData[ORDER_ARGS_ID] = "FITX";//商品代號

            varData[ORDER_ARGS_MTH] = "201204";//交易月份
            
            varData[ORDER_ARGS_BS] = "B";//BS
            
            varData[ORDER_ARGS_PRICE_FLAG] = "0";//限市價,市價=1,限價=0
            
            varData[ORDER_ARGS_ODPRICE] = 7500 * 1000;        //'委託價格需乘上1000, 末三位是小數位數
            
            varData[ORDER_ARGS_ODQTY] = 1;
            
            varData[ORDER_ARGS_ODTYPE] = "ROD";  //'ROD/IOC/FOK

            varData[ORDER_ARGS_ODKEY] = "maxtest";  //'此筆下單的key, 每次下單必須有一個unique的key
            
            varData[ORDER_ARGS_OPENCLOSE] = "O";

            object Data = varData;

            logger.Info(Data.ToString());
            
            int tmp = tradeApi.PlaceFutOrder2(Data);
            
            MessageBox.Show(tmp.ToString());
        }


        private void FuturePlaceOrder_Click(object sender, EventArgs e)
        {
            //期貨下單

            PlaceFutureOrder();

            //string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            //string password = PrivateXMLReader.Instance.getAttribute("password");

            //object[] varData = new object[28];
            //varData[ORDER_ARGS_ROCID] = ROCID;//user_身分證字號
            //varData[ORDER_ARGS_PASSWORD] = password;//user_password
            //varData[ORDER_ARGS_BRANCHID] = m_Branch;//分公司代號
            //varData[ORDER_ARGS_CUSTID] = m_Account;//帳號
            //varData[ORDER_ARGS_AGENTID] = m_AgentId;//AE代號
            //varData[ORDER_ARGS_SOURCE] = "IC";        //'來源別, IC指艾揚, 可任意填上2碼代碼,代表由何處下單
            //varData[ORDER_ARGS_ID] = textBox6.Text;//商品代號
            //varData[ORDER_ARGS_MTH] = textBox15.Text;//交易月份
            //varData[ORDER_ARGS_BS] = textBox7.Text;//BS
            //varData[ORDER_ARGS_PRICE_FLAG] = textBox10.Text;//限市價,市價=1,限價=0
            //varData[ORDER_ARGS_ODPRICE] = Convert.ToInt32(textBox11.Text)*1000;        //'委託價格需乘上1000, 末三位是小數位數
            //varData[ORDER_ARGS_ODQTY] = Convert.ToInt32(textBox12.Text);
            //varData[ORDER_ARGS_ODTYPE] = textBox8.Text;  //'ROD/IOC/FOK

            //varData[ORDER_ARGS_ODKEY] = textBox14.Text;  //'此筆下單的key, 每次下單必須有一個unique的key
            //varData[ORDER_ARGS_OPENCLOSE] = textBox13.Text;
            
            //object Data = varData;

     
            //int tmp = tradeApi.PlaceFutOrder2(Data);
            //MessageBox.Show(tmp.ToString());
        }

        private void StockPlaceOrder_Click(object sender, EventArgs e)
        {
            //證券下單

            string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            string password = PrivateXMLReader.Instance.getAttribute("password");

            object[] varData = new object[24];
            varData[ORDER_ARGS_ROCID] = ROCID;//user_身分證字號
            varData[ORDER_ARGS_PASSWORD] = password;//user_password
            varData[ORDER_ARGS_BRANCHID] = m_Branch;//分公司代號
            varData[ORDER_ARGS_CUSTID] = m_Account;//帳號
            varData[ORDER_ARGS_AGENTID] = m_AgentId;//AE代號
            varData[ORDER_ARGS_SOURCE] = "IC";        //'來源別, IC指艾揚, 可任意填上2碼代碼,代表由何處下單
            varData[ORDER_ARGS_ID] = textBox6.Text;//商品代號
            varData[ORDER_ARGS_BS] = textBox7.Text;//BS
            varData[ORDER_ARGS_PRICE_FLAG] = textBox10.Text;//限市價,市價=1,限價=0
            varData[ORDER_ARGS_ODPRICE] = Convert.ToInt32(textBox11.Text) * 1000;        //'委託價格需乘上1000, 末三位是小數位數
            varData[ORDER_ARGS_ODQTY] = Convert.ToInt32(textBox12.Text);
            varData[ORDER_ARGS_ODTYPE] = textBox8.Text;  //0現股/1融資/2融券
            varData[ORDER_ARGS_TRADE_TYPE] = textBox9.Text;   //N/O/F
            varData[ORDER_ARGS_ODKEY] = textBox14.Text;  //'此筆下單的key, 每次下單必須有一個unique的key
            varData[ORDER_ARGS_MARKETTYPE] = textBox18.Text;//T/O;
            object Data = varData;

            if (ndeedCAcheckedBox.Checked == false)
                tradeApi.NeedCA = false;
            else
                tradeApi.NeedCA = true;


            int tmp = tradeApi.PlaceStkOrder2(Data);
            MessageBox.Show(tmp.ToString());
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            logout();
        }

        private void logout()
        {
            tradeApi.Logout();
        }

        private void FutureForiegn_Click(object sender, EventArgs e)
        {
            //國外期貨下單
            
            string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            string password = PrivateXMLReader.Instance.getAttribute("password");

            object[] varData = new object[28];
            varData[ORDER_ARGS_ROCID] = ROCID;//user_身分證字號
            varData[ORDER_ARGS_PASSWORD] = password;//user_password
            varData[ORDER_ARGS_BRANCHID] = m_Branch;//分公司代號
            varData[ORDER_ARGS_CUSTID] = m_Account;//帳號
            varData[ORDER_ARGS_AGENTID] = m_AgentId;//AE代號
            varData[ORDER_ARGS_SOURCE] = "IC";        //'來源別, IC指艾揚, 可任意填上2碼代碼,代表由何處下單
            varData[ORDER_ARGS_EXCHANGE] = textBox19.Text; //國外交易所
            varData[ORDER_ARGS_ID] = textBox6.Text;//商品代號
            varData[ORDER_ARGS_MTH] = textBox15.Text;//交易月份
            varData[ORDER_ARGS_BS] = textBox7.Text;//BS
            varData[ORDER_ARGS_PRICE_FLAG] = textBox10.Text;//限市價,市價=1,限價=0
            varData[ORDER_ARGS_ODPRICE] = (Convert.ToInt32(textBox11.Text) * 1000000).ToString();        //'委託價格需乘上1000, 末三位是小數位數
            varData[ORDER_ARGS_ODQTY] = Convert.ToInt32(textBox12.Text);
            varData[ORDER_ARGS_ODTYPE] = textBox8.Text;  //'ROD/IOC/FOK

            varData[ORDER_ARGS_ODKEY] = textBox14.Text;  //'此筆下單的key, 每次下單必須有一個unique的key
            varData[ORDER_ARGS_OPENCLOSE] = textBox13.Text;
            varData[ORDER_ARGS_STOPPRICE] = textBox20.Text;
            varData[ORDER_ARGS_DAYTRADE] = textBox21.Text;
            object Data = varData;

            if (ndeedCAcheckedBox.Checked == false)           
                tradeApi.NeedCA = false;            
            else
                tradeApi.NeedCA = true;


            int tmp = tradeApi.PlaceOBFutOrder2(Data);
            MessageBox.Show(tmp.ToString());
        }

        private void OptionPlaceOrder_Click(object sender, EventArgs e)
        {
            
            //選擇權下單

            string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            string password = PrivateXMLReader.Instance.getAttribute("password");

            object[] varData = new object[28];
            varData[ORDER_ARGS_ROCID] = ROCID;//user_身分證字號
            varData[ORDER_ARGS_PASSWORD] = password;//user_password
            varData[ORDER_ARGS_BRANCHID] = m_Branch;//分公司代號
            varData[ORDER_ARGS_CUSTID] = m_Account;//帳號
            varData[ORDER_ARGS_AGENTID] = m_AgentId;//AE代號
            varData[ORDER_ARGS_SOURCE] = "IC";        //'來源別, IC指艾揚, 可任意填上2碼代碼,代表由何處下單
            varData[ORDER_ARGS_ID] = textBox6.Text;//商品代號
            varData[ORDER_ARGS_CP]=textBox16.Text;//CAll/PUT
            varData[ORDER_ARGS_STRIKE]=Convert.ToInt32(textBox17.Text)*1000;
            varData[ORDER_ARGS_MTH] = textBox15.Text;//交易月份
            varData[ORDER_ARGS_BS] = textBox7.Text;//BS
            varData[ORDER_ARGS_PRICE_FLAG] = textBox10.Text;//限市價,市價=1,限價=0
            varData[ORDER_ARGS_ODPRICE] = Convert.ToInt32(textBox11.Text)*1000;        //'委託價格需乘上1000, 末三位是小數位數
            varData[ORDER_ARGS_ODQTY] = Convert.ToInt32(textBox12.Text);
            varData[ORDER_ARGS_ODTYPE] = textBox8.Text;  //'ROD/IOC/FOK

            varData[ORDER_ARGS_ODKEY] = textBox14.Text;  //'此筆下單的key, 每次下單必須有一個unique的key
            varData[ORDER_ARGS_OPENCLOSE] = textBox13.Text;
            
            object Data = varData;
            if (ndeedCAcheckedBox.Checked == false)
                tradeApi.NeedCA = false;
            else
                tradeApi.NeedCA = true;

            int tmp = tradeApi.PlaceOptOrder2(Data);
            MessageBox.Show(tmp.ToString());
        }

        private void CancealOrder_Click(object sender, EventArgs e)
        {
            //刪單

            string ROCID = PrivateXMLReader.Instance.getAttribute("ROCID");
            string password = PrivateXMLReader.Instance.getAttribute("password");

            int nType = -1;
            int nIndex = -1;
            if (textBox24.Text.Trim() != "")
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Column4"].Value.ToString().Trim() == textBox24.Text.Trim())
                    {
                        nType = Convert.ToInt16(row.Cells["DataType"].Value);
                        nIndex = Convert.ToInt16(row.Cells["DataIndex"].Value);
                        break;  
                    }
                }
                if ((nType >= 0) && (nIndex >= 0))
                {
                    object[] varData = new object[34];
                    varData[ORDER_ARGS_ROCID] = ROCID;
                    varData[ORDER_ARGS_PASSWORD] = password; ;
                    varData[ORDER_ARGS_BRANCHID]=  tradeApi.GetReportString(nType, nIndex, BRANCH_ID);
                    varData[ORDER_ARGS_CUSTID] =  tradeApi.GetReportString(nType, nIndex, CUST_ID);
                    varData[ORDER_ARGS_AGENTID] = m_AgentId;
                    varData[ORDER_ARGS_SOURCE] = "IC";
                    varData[ORDER_ARGS_ODKEY] = textBox14.Text ;  //此筆刪改單的key, 每次下單必須有一個unique的key                                   
                    varData[ORDER_ARGS_ORGSEQ] = tradeApi.GetReportString(nType, nIndex, F_OD_SEQ);
                    varData[ORDER_ARGS_ORGSOURCE] = tradeApi.GetReportString(nType, nIndex, SRC_CODE);
                    varData[ORDER_ARGS_ORDNO] = tradeApi.GetReportString(nType, nIndex, ORDNO);
                    varData[ORDER_ARGS_CANCELQTY] = tradeApi.GetReportValue(nType, nIndex, OD_QTY);
                    varData[ORDER_ARGS_ID] = tradeApi.GetReportString(nType, nIndex, FTR_ID);
                    varData[ORDER_ARGS_MTH] = tradeApi.GetReportString(nType, nIndex, FTR_MTH);
                    varData[ORDER_ARGS_BS] = tradeApi.GetReportString(nType, nIndex, BUYSELL);
                    varData[ORDER_ARGS_PRICE_FLAG] = tradeApi.GetReportString(nType, nIndex, PRICE_FLAG);
                    varData[ORDER_ARGS_ODPRICE] = tradeApi.GetReportValue(nType, nIndex, OD_PRICE);
                    varData[ORDER_ARGS_ODQTY] = tradeApi.GetReportValue(nType, nIndex, OD_QTY);
                    varData[ORDER_ARGS_OPENCLOSE] = tradeApi.GetReportString(nType, nIndex, OPENCLOSE);
                    varData[ORDER_ARGS_TRANSDATE] = tradeApi.GetReportString(nType, nIndex, TRANS_DATE);
                    varData[ORDER_ARGS_CP] = tradeApi.GetReportString(nType, nIndex, CALLPUT);
                    varData[ORDER_ARGS_STRIKE] =tradeApi.GetReportValue(nType, nIndex, STRIKE_PRICE);
                    varData[ORDER_ARGS_ODTYPE] = tradeApi.GetReportString(nType, nIndex, OD_TYPE);

                    String sHeader = tradeApi.GetReportString(nType, nIndex, F_OD_SEQ);

                    if (sHeader == "05")
                    {
                        object Data = varData;
                        int tmp = tradeApi.ReduceFutOrder2(0, Data);
                        MessageBox.Show(tmp.ToString());
                    }
                    else
                    {
                            object Data = varData;
                            int tmp = tradeApi.ReduceOptOrder2(0, Data);
                            MessageBox.Show(tmp.ToString());
                   }                    
                }
            }
        }

        private void axICETRADEAPI1_ChgDealReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_ChgDealReportEvent e)
        {
            int Row;
            foreach (TRptData tmpRptData in m_DealReport)
            {
                if ((tmpRptData.nIndex == e.nDataIndex) && (tmpRptData.nType == e.nDataType))
                {
                    Row = tmpRptData.nGridNum;
                    dataGridView2.Rows[Row].Cells[0].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_ID);
                    dataGridView2.Rows[Row].Cells[1].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_MTH);
                    dataGridView2.Rows[Row].Cells[2].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, CALLPUT);
                    dataGridView2.Rows[Row].Cells[3].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
                    dataGridView2.Rows[Row].Cells[4].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ORDNO);
                    dataGridView2.Rows[Row].Cells[5].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, BUYSELL);
                    dataGridView2.Rows[Row].Cells[6].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, OD_PRICE) / 1000;
                    dataGridView2.Rows[Row].Cells[7].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, DEAL_QTY);
                    break;
                }
            }
        }

        private void axICETRADEAPI1_ChgOrderReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_ChgOrderReportEvent e)
        {
            int Row;
            foreach (TRptData tmpRptData in m_OrderReport)
            {
                if ((tmpRptData.nIndex == e.nDataIndex) && (tmpRptData.nType == e.nDataType))
                {
                    Row = tmpRptData.nGridNum;
                    dataGridView1.Rows[Row].Cells[0].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_ID);
                    dataGridView1.Rows[Row].Cells[1].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_MTH);
                    dataGridView1.Rows[Row].Cells[2].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, CALLPUT);
                    dataGridView1.Rows[Row].Cells[3].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
                    dataGridView1.Rows[Row].Cells[4].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ORDNO);
                    dataGridView1.Rows[Row].Cells[5].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, BUYSELL);
                    dataGridView1.Rows[Row].Cells[6].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, OD_PRICE) / 1000;
                    dataGridView1.Rows[Row].Cells[7].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, OD_QTY);
                    dataGridView1.Rows[Row].Cells[8].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, OD_KEY);
                    dataGridView1.Rows[Row].Cells[9].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ERR_CODE);
                    dataGridView1.Rows[Row].Cells[10].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ERR_MSG);
                    break;
                }
            }
        }

        private void OnConnectStatusChanged(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_ConnectStatusChangedEvent e)
        {
            logger.Info("OnConnectStatusChanged");

            string nStatus = e.nStatus.ToString().Trim();
            
            if (nStatus == "0")
            {
                logger.Info("離線\n");
            }
            else if (nStatus == "1")
            {
                logger.Info("連線中\n");
            }
            else//
            {
                logger.Info("連線成功\n");

                //'連線完成, 跟API取得帳號                                
                tradeApi.OrdSubPrefix = "OrdTW";
                tradeApi.OBOrdSubPrefix = "OrdOB";

                m_OrderRow = 0;
                m_DealRow = 0;
                m_OBOrderRow = 0;
                m_OBDealRow = 0;

                string accountType = "";

                int dataCount = tradeApi.GetDataCount(ITS_REPORT_LOGIN_OK);
                logger.Info("OnConnectStatusChanged : dataCount = " + dataCount);

                for (int i = 0; i < dataCount; i++)
                {
                    //'取得帳號type, S:證券帳號   F:期權帳號
                    accountType = tradeApi.GetString(1, i, ACCOUNT_TYPE);

                    logger.Info("account type : "+ accountType);

                    if (accountType == "S") //現貨
                    {
                        //'登入取帳號時, 有以下欄位資料可取得
                        //'取得分公司號
                        m_StkBranch = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, BRANCH_ID);
                        //'取得帳號
                        m_StkAccount = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, CUST_ID);
                        //'取得帳號名稱
                        m_StkName = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, CUST_NAME);
                        //'取得營業員
                        m_StkAgentId = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, AGENT_ID);

                        logger.Info("分公司代號=" + m_StkBranch + "\n" + "股票帳號=" + m_StkAccount + "\n" + "帳號名稱=" + m_StkName + "\n" + "營業員=" + m_StkAgentId + "\n");
                        //'MsgBox "STK Account : " + m_StkBranch + "-" + m_StkAccount
                        //'帳號取得後, 跟STFY_API要求訂閱回補委回成回資料

                        if (checkBox2.Checked == false)
                            tradeApi.SubscribeByAccount(12, m_Branch, m_Account);
                        tradeApi.SubscribeByAccount(0, m_StkBranch, m_StkAccount);
                    }
                    else//期貨
                    {
                        //    '登入取帳號時, 有以下欄位資料可取得
                        //    '取得分公司號
                        m_Branch = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, BRANCH_ID);
                        //    '取得帳號
                        m_Account = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, CUST_ID);
                        //    '取得帳號名稱
                        m_Name = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, CUST_NAME);
                        //    '取得營業員
                        m_AgentId = tradeApi.GetString(ITS_REPORT_LOGIN_OK, i, AGENT_ID);

                        logger.Info("分公司代號=" + m_Branch + "\n" + "期貨帳號=" + m_Account + "\n" + "帳號名稱=" + m_Name + "\n" + "營業員=" + m_AgentId + "\n");
                        //    'MsgBox "FUT Account : " + m_Branch + "-" + m_Account
                        //    '帳號取得後, 要求訂閱回補委回成回資料

                        if (checkBox2.Checked == false)
                            tradeApi.SubscribeByAccount(12, m_Branch, m_Account);
                        tradeApi.SubscribeByAccount(0, m_Branch, m_Account);
                        tradeApi.SubscribeByAccount(1, m_Branch, m_Account);
                    }
                }
            }
        }

        private void OnNewDealReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_NewDealReportEvent e)
        {
            TRptData tmpRptData = new TRptData();
            tmpRptData.nIndex = e.nDataIndex;
            tmpRptData.nType = e.nDataType;
            tmpRptData.nGridNum = m_DealRow;
            dataGridView2.Rows.Add();
            dataGridView2.Rows[m_DealRow].Cells[0].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_ID);
            dataGridView2.Rows[m_DealRow].Cells[1].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_MTH);
            dataGridView2.Rows[m_DealRow].Cells[2].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, CALLPUT);
            dataGridView2.Rows[m_DealRow].Cells[3].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
            dataGridView2.Rows[m_DealRow].Cells[4].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ORDNO);
            dataGridView2.Rows[m_DealRow].Cells[5].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, BUYSELL);
            dataGridView2.Rows[m_DealRow].Cells[6].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, OD_PRICE) / 1000;
            dataGridView2.Rows[m_DealRow].Cells[7].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, DEAL_QTY);
            m_DealReport.Add(tmpRptData);
            m_DealRow = m_DealRow + 1;
        }

        private void OnNewOrderReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_NewOrderReportEvent e)
        {
            TRptData tmpRptData = new TRptData();
            tmpRptData.nIndex = e.nDataIndex;
            tmpRptData.nType = e.nDataType;
            tmpRptData.nGridNum = m_OrderRow;

            dataGridView1.Rows.Add();
            dataGridView1.Rows[m_OrderRow].Cells[0].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_ID);
            dataGridView1.Rows[m_OrderRow].Cells[1].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, FTR_MTH);
            dataGridView1.Rows[m_OrderRow].Cells[2].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, CALLPUT);
            dataGridView1.Rows[m_OrderRow].Cells[3].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
            dataGridView1.Rows[m_OrderRow].Cells[4].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ORDNO);
            tmpRptData.nOrderNo = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ORDNO);
            dataGridView1.Rows[m_OrderRow].Cells[5].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, BUYSELL);
            dataGridView1.Rows[m_OrderRow].Cells[6].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, OD_PRICE) / 1000;
            dataGridView1.Rows[m_OrderRow].Cells[7].Value = tradeApi.GetReportValue(e.nDataType, e.nDataIndex, OD_QTY);
            dataGridView1.Rows[m_OrderRow].Cells[8].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, OD_KEY);
            dataGridView1.Rows[m_OrderRow].Cells[9].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ERR_CODE);
            dataGridView1.Rows[m_OrderRow].Cells[10].Value = tradeApi.GetReportString(e.nDataType, e.nDataIndex, ERR_MSG);
            dataGridView1.Rows[m_OrderRow].Cells[11].Value = e.nDataType;
            dataGridView1.Rows[m_OrderRow].Cells[12].Value = e.nDataIndex;
            m_OrderReport.Add(tmpRptData);
            m_OrderRow = m_OrderRow + 1;
        }

        private void OnOBChangeDealReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_OBChgDealReportEvent e)
        {
            int Row;
            foreach (TRptData tmpRptData in m_OBDealReport)
            {
                if ((tmpRptData.nIndex == e.nDataIndex) && (tmpRptData.nType == e.nDataType))
                {
                    Row = tmpRptData.nGridNum;
                    dataGridView4.Rows[Row].Cells[0].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_ID);
                    dataGridView4.Rows[Row].Cells[1].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_MTH);
                    dataGridView4.Rows[Row].Cells[2].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, CALLPUT);
                    dataGridView4.Rows[Row].Cells[3].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
                    dataGridView4.Rows[Row].Cells[4].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ORDNO);
                    dataGridView4.Rows[Row].Cells[5].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, BUYSELL);
                    dataGridView4.Rows[Row].Cells[6].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, OBOD_PRICE) / 1000;
                    dataGridView4.Rows[Row].Cells[7].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, DEAL_QTY);
                    break;
                }
            }
        }

        private void OnOBChangeOrderReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_OBChgOrderReportEvent e)
        {
            int Row;
            LogTextBox.AppendText("chg\n");
            foreach (TRptData tmpRptData in m_OBOrderReport)
            {
                if ((tmpRptData.nIndex == e.nDataIndex) && (tmpRptData.nType == e.nDataType))
                {
                    Row = tmpRptData.nGridNum;
                    dataGridView3.Rows[Row].Cells[0].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_ID);
                    dataGridView3.Rows[Row].Cells[1].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_MTH);
                    dataGridView3.Rows[Row].Cells[2].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, CALLPUT);
                    dataGridView3.Rows[Row].Cells[3].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
                    dataGridView3.Rows[Row].Cells[4].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ORDNO);
                    dataGridView3.Rows[Row].Cells[5].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, BUYSELL);
                    dataGridView3.Rows[Row].Cells[6].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, OD_PRICE) / 1000;
                    dataGridView3.Rows[Row].Cells[7].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, OD_QTY);
                    dataGridView3.Rows[Row].Cells[8].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, OD_KEY);
                    dataGridView3.Rows[Row].Cells[9].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ERR_CODE);
                    dataGridView3.Rows[Row].Cells[10].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ERR_MSG);
                    dataGridView3.Rows[Row].Cells[11].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, FORM_TYPE);
                    break;
                }
            }
        }

        private void OnOBNewDealReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_OBNewDealReportEvent e)
        {
            TRptData tmpRptData = new TRptData();
            tmpRptData.nIndex = e.nDataIndex;
            tmpRptData.nType = e.nDataType;
            tmpRptData.nGridNum = m_OBDealRow;
            dataGridView4.Rows.Add();
            dataGridView4.Rows[m_OBDealRow].Cells[0].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_ID);
            dataGridView4.Rows[m_OBDealRow].Cells[1].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_MTH);
            dataGridView4.Rows[m_OBDealRow].Cells[2].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, CALLPUT);
            dataGridView4.Rows[m_OBDealRow].Cells[3].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
            dataGridView4.Rows[m_OBDealRow].Cells[4].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ORDNO);
            dataGridView4.Rows[m_OBDealRow].Cells[5].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, BUYSELL);
            dataGridView4.Rows[m_OBDealRow].Cells[6].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, OBOD_PRICE) / 1000;
            dataGridView4.Rows[m_OBDealRow].Cells[7].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, DEAL_QTY);
            m_OBDealReport.Add(tmpRptData);
            m_OBDealRow = m_OBDealRow + 1;
        }

        private void OnOBNewOrderReport(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_OBNewOrderReportEvent e)
        {
            TRptData tmpRptData = new TRptData();
            tmpRptData.nIndex = e.nDataIndex;
            tmpRptData.nType = e.nDataType;
            tmpRptData.nGridNum = m_OBOrderRow;
            LogTextBox.AppendText("new\n");
            dataGridView3.Rows.Add();
            dataGridView3.Rows[m_OBOrderRow].Cells[0].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_ID);
            dataGridView3.Rows[m_OBOrderRow].Cells[1].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, FTR_MTH);
            dataGridView3.Rows[m_OBOrderRow].Cells[2].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, CALLPUT);
            dataGridView3.Rows[m_OBOrderRow].Cells[3].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, STRIKE_PRICE) / 1000;
            dataGridView3.Rows[m_OBOrderRow].Cells[4].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ORDNO);
            tmpRptData.nOrderNo = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ORDNO);
            dataGridView3.Rows[m_OBOrderRow].Cells[5].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, BUYSELL);
            dataGridView3.Rows[m_OBOrderRow].Cells[6].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, OD_PRICE) / 1000;
            dataGridView3.Rows[m_OBOrderRow].Cells[7].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, OD_QTY);
            dataGridView3.Rows[m_OBOrderRow].Cells[8].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, OD_KEY);
            dataGridView3.Rows[m_OBOrderRow].Cells[9].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ERR_CODE);
            dataGridView3.Rows[m_OBOrderRow].Cells[10].Value = tradeApi.GetOBReportString(e.nDataType, e.nDataIndex, ERR_MSG);
            dataGridView3.Rows[m_OBOrderRow].Cells[11].Value = tradeApi.GetOBReportValue(e.nDataType, e.nDataIndex, FORM_TYPE);

            m_OBOrderReport.Add(tmpRptData);
            m_OBOrderRow = m_OBOrderRow + 1;
        }

        private void axICETRADEAPI1_OBOrdRestoreComplete(object sender, EventArgs e)
        {
            LogTextBox.AppendText("OB RestoreComplete Trigger\n");
        }

        private void OnError(object sender, AxICETRADEAPILib._DICETRADEAPIEvents_OnErrorEvent e)
        {
            logger.Info(e.errCode.ToString() + ":" + e.errMsg + '\n');
        }

        private void axICETRADEAPI1_OrdRestoreComplete(object sender, EventArgs e)
        {
            LogTextBox.AppendText("RestoreComplete Trigger\n");
        }

        private void KGIBOX_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.Info("logout!");
            logout();
        }


    }
}
