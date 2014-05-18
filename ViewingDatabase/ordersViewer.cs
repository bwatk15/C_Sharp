/* Matthew Mckeller
 * 
 * Accesses a database and displays the information of the customer 
 *   along with a data grid view of their orders
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;      // needed for database connection
namespace orders
{
    public partial class orderViewerFRM : Form
    {
        public orderViewerFRM()
        {
            InitializeComponent();
        }

        // connection to database
        OleDbConnection Conn = new OleDbConnection();

        // open database connection
        public void openDatabase()
        {
            try
            {
                string conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=final_database.accdb;" +
                    "Persist Security Info=False;";
                Conn.ConnectionString = conn;
                // open database connection
                Conn.Open();
            }
            catch (Exception ex)
            {
                // error message when a problem arises when trying to connect to database
                MessageBox.Show("Database could not open because: " + ex,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // fill the combo box with invoice id numbers
        public void fillComboBox()
        {
            try
            {
                ordersDS.Clear();
                customerCB.DisplayMember = "";
                customerCB.ValueMember = "";
                

                // query to pull invoice ids
                string sql = "select distinct id from customers order by id";

                // adapter to send query to database
                OleDbDataAdapter daInvoices = new OleDbDataAdapter(sql, Conn);


                // fill dataset
                daInvoices.Fill(ordersDS, "customers");

                // declare data source for invoice id combo box
                customerCB.DataSource = ordersDS.Tables[0];
                customerCB.DisplayMember = "id";
                customerCB.ValueMember = "id";

                customerCB.SelectedIndex = -1;
               
            }
            catch (Exception ex)
            {
                // error message if trouble pulling invoice ids from database
                MessageBox.Show("Trouble pulling invoices from database because: " + ex,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void invoiceViewerFRM_Load(object sender, EventArgs e)
        {
            // call open database method to open database
            openDatabase();
            // call method to fill order numbers in combo box
            fillComboBox();
        }

        // pull information needed and display to form when invoice is selected
        private void invoiceCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //For the opening make sure it doesnt get error as there is no customer id of -1
            if(customerCB.SelectedIndex != -1)
            {
            // get selected invoice id from combo box
            string currentInvoice = customerCB.SelectedValue.ToString();
            // make sure this only gets done when the invoice is selected (not when combo box is loaded)
            if (currentInvoice != "System.Data.DataRowView")
            {
                this.customerOrdersDGV.Sort(this.OrderDate, ListSortDirection.Ascending);
                // define variables needed for method
                string order_Number;
                string order_Date;
                string order_ShippedDate;
                decimal shipping_Fee;

                // clear datasets used in this method
                customerDS.Clear();
                detailsDS.Clear();

                // query to pull customer information from database
                string sql = "SELECT customers.ID, customers.Last_Name, customers.First_Name, " +
                              "customers.Business_Phone, customers.Address, customers.Zip AS customers_Zip, " +
                              "customers.Email_Address, zip.State, zip.City, zip.Zip AS zip_Zip " +
                              "FROM zip INNER JOIN customers ON zip.[Zip] = customers.[Zip] " +
                              "where customers.ID = " + currentInvoice;


                // adapter used to pull customer info from database
                OleDbDataAdapter daCustomer = new OleDbDataAdapter(sql, Conn);
                // fill customer dataset
                daCustomer.Fill(customerDS, "customer");
                // data row that contains information from database
                DataRow currentData = customerDS.Tables["customer"].Rows[0];

                // put customer data into form
                lastNameTXT.Text = currentData[1].ToString();
                firstNameTXT.Text = currentData[2].ToString();
                businessPhoneTXT.Text = currentData[3].ToString();
                addressTXT.Text = currentData[4].ToString();
                zipTXT.Text = currentData[5].ToString();
                emailTXT.Text = currentData[6].ToString();
                stateTXT.Text = currentData[7].ToString();
                cityTXT.Text = currentData[8].ToString();




                // query to pull order data from database
                string sql2 = "SELECT orders.ID AS orders_ID, orders.Order_Number, orders.Order_Date, " +
                              "orders.Order_Shipped, orders.Shipping_Fee, customers.ID AS customers_ID " +
                              "FROM customers INNER JOIN orders ON customers.[ID] = orders.[ID] " +
                              "where orders.id = " + currentInvoice;

                // adapter to send query to database
                OleDbDataAdapter daItems = new OleDbDataAdapter(sql2, Conn);
                // fill invoice detail data into dataset
                daItems.Fill(detailsDS, "details");
                // clear data grid view
                customerOrdersDGV.Rows.Clear();

                customerOrdersDGV.Sort(OrderDate, ListSortDirection.Ascending );
                // loop through all items for selected customer
                foreach (DataRow items in detailsDS.Tables[0].Rows)
                {
                    // put data from datarow into variables
                    //
                    order_Number = items[1].ToString(); 
                    order_Date = items[2].ToString();
                    order_ShippedDate = items[3].ToString();
                    shipping_Fee = Convert.ToDecimal(items[4].ToString());

                    // put row into data grid view
                    customerOrdersDGV.Rows.Add(order_Number, order_Date,
                        order_ShippedDate, String.Format("{0:C}", shipping_Fee));

                }
            }
            }
                 
        }

        private void cityTXT_TextChanged(object sender, EventArgs e)
        {

        }

        private void invoiceItemsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
