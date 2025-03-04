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
    public partial class frmInfo : Form
    {
        // create data table to update records
        DataTable dtUpdate;
        public frmInfo()
        {
            InitializeComponent();
        }
        /// <summary>
        /// alternative form load that passes LoadMemorabilia ID info to be used for MemorabiliaID
        /// </summary>
        /// <param name="MemorabiliaID"></param>
        public frmInfo(int MemorabiliaID)
        {
            InitializeComponent();
            LoadMemorabilia(MemorabiliaID); // load memID based on id passed
        }

        /// <summary>
        /// passes ID to be used to identify and display records
        /// </summary>
        /// <param name="ID"></param>
        private void LoadMemorabilia(int ID)
        {
            // connect to data
            clsData Memorabilia = new clsData();
            // pass sql
            Memorabilia.SQL = "SELECT memID, memSport, memType, memCondition, memDescription FROM tblMemorabilia WHERE memID = " + ID;
            // get values from data table and display to screen
            txtSport.Tag = ID.ToString();
            txtSport.Text = Memorabilia.dt.Rows[0]["memSport"].ToString();
            txtType.Text = Memorabilia.dt.Rows[0]["memType"].ToString();
            txtCondition.Text = Memorabilia.dt.Rows[0]["memCondition"].ToString();
            txtDescription.Text = Memorabilia.dt.Rows[0]["memDescription"].ToString();
            // get info for update
            dtUpdate = Memorabilia.dt;

        }
        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Update button event handler, two routes for if there was a combobox selection indicated by the txtSpot.Tag value, either updates selected info or creates new record using clsData methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // validate information
            if (txtSport.Text == "")
            {
                MessageBox.Show("Enter a sport in the text box");
                txtSport.Focus();

            }
            else if (txtType.Text == "")
            {
                MessageBox.Show("Enter a memorabilia type in the text box");
                txtType.Focus();
            }
            else if (txtCondition.Text == "")
            {
                MessageBox.Show("Enter a state of condition in the text box (Very Poor, Poor, Good, Very Good, Excellent, Near Mint, Mint)");
                txtCondition.Focus();
            }
            else if (txtDescription.Text == "")
            {
                MessageBox.Show("Enter a memorabilia description in the text box");
                txtDescription.Focus();
            }
            else
            {
                // verify existing customer
                if (txtSport.Tag.ToString() != "N")
                {
                    string sportSQL;
                    string typeSQL;
                    string conditionSQL;
                    string descSQL;
                    clsData myUpdateData = new clsData();
                    // update local data table
                    dtUpdate.Rows[0]["memSport"] = txtSport.Text;
                    dtUpdate.Rows[0]["memType"] = txtType.Text;
                    dtUpdate.Rows[0]["memCondition"] = txtCondition.Text;
                    dtUpdate.Rows[0]["memDescription"] = txtDescription.Text;
                    // send update to database
                    myUpdateData.UpdateData(dtUpdate, "SELECT memID, memSport, memType, memCondition, memDescription FROM tblMemorabilia WHERE memID = " + int.Parse(txtSport.Tag.ToString()));
                    // grab strings for sql statements on server side
                    sportSQL = txtSport.Text;
                    typeSQL = txtType.Text;
                    conditionSQL = txtCondition.Text;
                    descSQL = txtDescription.Text;
                    //create connections
                    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = ipHost.AddressList[0];
                    IPEndPoint endPoint = new IPEndPoint(ipAddress, 5555);
                    Socket sendSock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {


                        // connect to server socket
                        sendSock.Connect(endPoint);

                        // debug writeline confirming connection
                        Debug.WriteLine("Socket connected to: " + sendSock.RemoteEndPoint.ToString());

                        byte[] msgSent = Encoding.ASCII.GetBytes(sportSQL + Environment.NewLine);
                        int byteSent = sendSock.Send(msgSent);

                        byte[] msgSent2 = Encoding.ASCII.GetBytes(typeSQL + Environment.NewLine);
                        int byteSent2 = sendSock.Send(msgSent2);

                        byte[] msgSent3 = Encoding.ASCII.GetBytes(conditionSQL + Environment.NewLine);
                        int byteSent3 = sendSock.Send(msgSent3);

                        byte[] msgSent4 = Encoding.ASCII.GetBytes(descSQL + "::");
                        int byteSent4 = sendSock.Send(msgSent4);

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
                // assume new customer
                else
                {
                    // create instance of clsData
                    clsData MyNewData = new clsData();
                    // call update method
                    MyNewData.CreateMemorabilia(txtSport.Text, txtType.Text, txtCondition.Text, txtDescription.Text);
                    //create connections
                    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = ipHost.AddressList[0];
                    IPEndPoint endPoint = new IPEndPoint(ipAddress, 5555);
                    Socket sendSock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        //strings to send to server
                        string sportSQL = txtSport.Text;
                        string typeSQL = txtType.Text;
                        string conditionSQL = txtCondition.Text;
                        string descSQL = txtDescription.Text;
                        // connect to server socket
                        sendSock.Connect(endPoint);

                        // debug writeline confirming connection
                        Debug.WriteLine("Socket connected to: " + sendSock.RemoteEndPoint.ToString());

                        byte[] msgSent = Encoding.ASCII.GetBytes(sportSQL + ", " + typeSQL + ", " + conditionSQL + ", " + descSQL + "::");
                        int byteSent = sendSock.Send(msgSent);

                        //byte[] msgSent2 = Encoding.ASCII.GetBytes(typeSQL + Environment.NewLine);
                        //int byteSent2 = sendSock.Send(msgSent2);

                        //byte[] msgSent3 = Encoding.ASCII.GetBytes(conditionSQL + Environment.NewLine);
                        //int byteSent3 = sendSock.Send(msgSent3);

                        //byte[] msgSent4 = Encoding.ASCII.GetBytes(descSQL + "::");
                        //int byteSent4 = sendSock.Send(msgSent4);

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
            }

            // data updated, close form
            this.Close();
            
        }
    }
}
