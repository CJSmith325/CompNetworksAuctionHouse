using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace CompNetworksAuctionHouse
{
    public partial class frmReport : Form
    {
        public frmReport()
        {
            InitializeComponent();
            
        }

        // alternate form load with unique ID passed and LoadMemorabilia executed
        public frmReport(int memoID)
        {
            InitializeComponent();
            MemorabiliaReport(memoID);
        }

        // html report building function using c# strings to contain html code to display in web browser control
        public void MemorabiliaReport(int intMemID)
        {
            decimal sales;
            string strSales;
            string strReportHeader;
            strReportHeader = "<HTML><HEAD><TITLE>Memorabilia Report</TITLE></HEAD>";
            string strReportBody = "<BODY>";
            strReportBody += "<H1>Memorabilia Report</H1>";
            strReportBody += "<hr/>"; // horizontal line
            // connect to data
            clsData Memorabilia = new clsData();
            // pass sql
            Memorabilia.SQL = "SELECT memID, memSport, memType, memCondition, memDescription, memSalePrice FROM tblMemorabilia WHERE memID = " + intMemID;
            strSales = Memorabilia.dt.Rows[0]["memSalePrice"].ToString();
            sales = decimal.Parse(strSales);
            strSales = sales.ToString("F");
            // add memorabilia to report
            strReportBody += "<h2>Sale price: $" + strSales + "</h2>";
            strReportBody += "<br/>"; // line break
            strReportBody += "<strong>Sport: " + Memorabilia.dt.Rows[0]["memSport"].ToString() + "</strong>";
            strReportBody += "<br/>"; // line break
            strReportBody += "<strong>Memorabilia type : " + Memorabilia.dt.Rows[0]["memType"].ToString() + "</strong>";
            strReportBody += "<br/>"; // line break
            strReportBody += "<strong>Memorabilia condition : " + Memorabilia.dt.Rows[0]["memCondition"].ToString() + "</strong>";

            // close report
            strReportBody += "</body></html>";

            //display in web browser
            wbReport.DocumentText = strReportHeader + strReportBody;

        }
    }
}
