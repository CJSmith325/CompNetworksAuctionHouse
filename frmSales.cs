using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompNetworksAuctionHouse
{
    public partial class frmSales : Form
    {
        DataTable dtSales;
        public frmSales()
        {
            InitializeComponent();
        }

        //alternative form load that loads pertinent information with LoadMemorabilia
        public frmSales(int MemorabiliaID)
        {
            InitializeComponent();
            LoadMemorabilia(MemorabiliaID);
        }

       
        // passes ID to be used to identify and display records
        private void LoadMemorabilia(int ID)
        {
            // connect to data
            clsData SalesMemorabilia = new clsData();
            // pass sql
            SalesMemorabilia.SQL = "SELECT memID, memSport, memType, memCondition, memDescription, memSalePrice FROM tblMemorabilia WHERE memID = " + ID;
            // get values from data table and display to screen
            txtSale.Tag = ID.ToString();
            lblCondition.Text = SalesMemorabilia.dt.Rows[0]["memCondition"].ToString();
            lblDescription.Text = SalesMemorabilia.dt.Rows[0]["memDescription"].ToString();
            dtSales = SalesMemorabilia.dt;
        }

        //Closes the form
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // updates memSalePrice in database with entered numerical value in txtSale
        private void btnSale_Click(object sender, EventArgs e)
        {
            if (decimal.Parse(txtSale.Text) >= 0 && decimal.Parse(txtSale.Text) < 99999999)
            {
                clsData SalesUpdate = new clsData();
                // update local data table
                dtSales.Rows[0]["memSalePrice"] = decimal.Parse(txtSale.Text);
                
                // send update to database
                SalesUpdate.UpdateData(dtSales, "SELECT memID, memSalePrice FROM tblMemorabilia WHERE memID = " + int.Parse(txtSale.Tag.ToString()));
            }
            else
            {
                MessageBox.Show("Please enter sale amount as an integer greater than 0 with no commas. (i.e. 500, 65000)");
            }
            this.Close();
        }

        // Key press event handler for txtSale that completely restricts user input to integers. An improvement would be to event handle based on ASCII values to allow numbers and periods, and delete this event handler, but the time and experience needed was not available
        private void txtSale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
