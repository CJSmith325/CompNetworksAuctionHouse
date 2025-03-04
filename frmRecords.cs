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
using System.Diagnostics;

namespace CompNetworksAuctionHouse
{
    public partial class frmRecords : Form
    {
        // List parallel to combobox of memorabilia containing memID
        List<int> intMemorabiliaID = new List<int>();
        public frmRecords()
        {
            InitializeComponent();  
        }

        public static void StartClient()
        {

            try
            {

                // variables for socket
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 5555);

                // create connections
                Socket sendSock = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    // connect to server socket
                    sendSock.Connect(localEndPoint);

                    // debug writeline confirming connection
                    Debug.WriteLine("Socket connected to: " + sendSock.RemoteEndPoint.ToString());

                    // creating frame to send to server
                    byte[] msgSent = Encoding.ASCII.GetBytes("This is a client test message::");
                    int byteSent = sendSock.Send(msgSent);

                    // buffer
                    byte[] msgReceived = new byte[1024];

                    // recieve response from server
                    int bytesRecieved = sendSock.Receive(msgReceived);
                    Debug.WriteLine(Encoding.ASCII.GetString(msgReceived, 0, bytesRecieved));

                    // close socket
                    sendSock.Shutdown(SocketShutdown.Both);
                    sendSock.Close();
                }

                // exception handling
                catch (ArgumentNullException anex)
                {

                    Debug.WriteLine("Argument Null Exception: " + anex.ToString());
                }

                catch (SocketException sexc)
                {

                    Debug.WriteLine("Socket Exception: " + sexc.ToString());
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.ToString());
                }
            }

            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
            }
        }

        // used to clear combobox/list and uses SQL, clsData, and a for loop to refill combobox/list with usable info (memID and memDescription)
        private void LoadMemorabilia()
        {
            //clear combobox
            cmbMemorabilia.Items.Clear();
            // clear out list
            intMemorabiliaID.Clear();
            // create instance of class
            clsData myData = new clsData();
            // send SQL statement to class
            myData.SQL = "SELECT memID, memDescription FROM tblMemorabilia ORDER BY memID";
            // loop through datatable to get values
            for (int i = 0; i < myData.dt.Rows.Count; i++)
            {
                // add memorabilia to cmbMemorabilia
                cmbMemorabilia.Items.Add(myData.dt.Rows[i]["memDescription"].ToString());
                // add memID to list
                intMemorabiliaID.Add(int.Parse(myData.dt.Rows[i]["memID"].ToString()));
            }
            
        }

        // on form load, executes LoadMemorabilia to refresh pertinent data

        private void frmRecords_Load(object sender, EventArgs e)
        {
            LoadMemorabilia(); // load memorabilia to screen
            StartClient();
            
        }

        // used to open sale recording form, passes unique ID to be used to grab correct data

        private void btnSales_Click(object sender, EventArgs e)
        {
            if (cmbMemorabilia.SelectedIndex > -1)
            {
                frmSales SalesForm = new frmSales(intMemorabiliaID[cmbMemorabilia.SelectedIndex]);
                SalesForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an item to record a sale.");
                cmbMemorabilia.Focus();
            }
        }

 
        // Edit button event handler, verifies a selection, then shoes frmInfo and refreshes combobox/listbox

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // make sure there is a selection
            if (cmbMemorabilia.SelectedIndex > -1)
            {
                // display
                frmInfo InfoForm = new frmInfo(intMemorabiliaID[cmbMemorabilia.SelectedIndex]);
                InfoForm.ShowDialog();
                // refresh combobox
                LoadMemorabilia();
            }
            else
            {
                MessageBox.Show("Please select an item to edit.");
                cmbMemorabilia.Focus();
            }
        }
        // Add button event handler, ignores combobox selection and pulls up blank frmInfo to make a new record
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // create instance of second form
            frmInfo NewInfoForm = new frmInfo();
            NewInfoForm.ShowDialog();
            // refresh combobox
            LoadMemorabilia();
        }

        // Delete button event handler, uses dialogresult to confirm delete, then deletes selected record from database if yes is selected
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // verify an item was selected
            if (cmbMemorabilia.SelectedIndex > -1)
            {
                // verify user wants to delete item
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete " + cmbMemorabilia.Text, "Delete Item", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    // delete item from database
                    clsData myDeleteData = new clsData();
                    // send SQl statement to class
                    myDeleteData.DeleteRecord("DELETE * FROM tblMemorabilia WHERE memID = " + intMemorabiliaID[cmbMemorabilia.SelectedIndex]);
                    // send variables for sql statement over TCP
                    
                    // refresh combobox
                    LoadMemorabilia();
                }
            }
            else
            {
                // no item selected
                MessageBox.Show("Select an item to delete");
            }

        }

        
        // Sales Report button event handler, pulls up recorded sale price and additional info about the selected item in combobox
        private void btnReport_Click(object sender, EventArgs e)
        {
            // make sure there is a selection
            if (cmbMemorabilia.SelectedIndex > -1)
            {
                // display
                frmReport MemorabiliaReport = new frmReport(intMemorabiliaID[cmbMemorabilia.SelectedIndex]);
                MemorabiliaReport.ShowDialog();
                
            }
            else
            {
                MessageBox.Show("Please select an item for the report.");
                cmbMemorabilia.Focus();
            }
        }
    }
}
