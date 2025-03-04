using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace CompNetworksAuctionHouse
{
    class clsData
    {
        string _strConnectionString = clsGlobal.DatabaseConnectionString; // variable to link to connectionstring
        string _strSQL = ""; // variable to link to SQL
        DataTable dtData; // data table for data


        //String to contain connection string to database
        public string ConnectionString
        {
            get
            {
                return _strConnectionString;
            }
            set
            {
                _strConnectionString = value;
                FillDataTable();
            }
        }

        // Store query to database
        public string SQL
        {
            get
            {
                return _strSQL;
            }
            set
            {
                _strSQL = value;
                FillDataTable(); // fill data table
            }
        }

        // data table accessible from application
        public DataTable dt
        {
            get
            {
                return dtData;
            }
            set
            {
                dtData = value;
            }
        }

        // fill data table with data based on SQL and ConnectionString
        private void FillDataTable()
        {
            // if connection string and sql are filled/ proceed
            if (ConnectionString != "" && SQL != "")
            {
                //create connection to database
                OleDbConnection conn = new OleDbConnection(ConnectionString);
                // open connection
                conn.Open();
                // create dataset
                DataSet ds = new DataSet();
                // fill dataset with data adapter
                OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, ConnectionString);
                adapter.Fill(ds);
                // close connection to database
                conn.Close();
                // fill datatable with dataset
                dtData = ds.Tables[0];
            }
           

        }
 
        // takes 4 arguments to accomadate all necessary info for frmInfo and pertinent textboxes, then sends data to database (for add button)
        public void CreateMemorabilia(string Sport, string Type, string Condition, string Description)
        {
            // create sql statement for new memorabilia
            SQL = "SELECT memID, memSport, memType, memCondition, memDescription FROM tblMemorabilia WHERE memID = 0";
            //create connection to database
            OleDbConnection conn = new OleDbConnection(ConnectionString);
            // open connection
            conn.Open();
            // create dataset
            DataSet ds = new DataSet();
            // fill dataset with data adapter
            OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, ConnectionString);
            adapter.Fill(ds);
            // create data row
            DataRow dr = ds.Tables[0].NewRow();
            // update values
            dr["memSport"] = Sport;
            dr["memType"] = Type;
            dr["memCondition"] = Condition;
            dr["memDescription"] = Description;

            // add data row to table
            ds.Tables[0].Rows.Add(dr);
            // create command builder
            System.Data.OleDb.OleDbCommandBuilder cb = new System.Data.OleDb.OleDbCommandBuilder(adapter);
            // update adapter
            adapter.Update(ds.Tables[0]);
            // close connection
            conn.Close();
        }

        // takes a datatable and SQL line as an argument to be used to edit or update linked database (for edit button on frmInfo)
        public void UpdateData(DataTable _DataTable, string _SQLStatement)
        {
            // update property with _SQLStatement
            SQL = _SQLStatement;
            // if connection string and sql are filled/ proceed
            if (ConnectionString != "" && SQL != "")
            {
                //create connection to database
                OleDbConnection conn = new OleDbConnection(ConnectionString);
                // open connection
                conn.Open();
                // create dataset
                DataSet ds = new DataSet();
                // fill dataset with data adapter
                OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, ConnectionString);
                adapter.Fill(ds);
                // create command builder
                System.Data.OleDb.OleDbCommandBuilder cb = new System.Data.OleDb.OleDbCommandBuilder(adapter);
                // update database with datatable
                adapter.Update(_DataTable);
                // close connection to database
                conn.Close();
                
            }
        }

        // deletes selected record in combobox
        public void DeleteRecord(string _SQLStatement)
        {
            // create connection to database
            OleDbConnection conn = new OleDbConnection(ConnectionString);
            // open connection
            conn.Open();
            // create command
            OleDbCommand command = new OleDbCommand(_SQLStatement, conn);
            // execute command
            command.ExecuteNonQuery();
            // close connection
            conn.Close();
        }

    }
}
