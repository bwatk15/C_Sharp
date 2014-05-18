/* Matthew Mckeller
 * This program allows a user to update customer information / add new customer / delete existing customer
 * Checks the phone/name/email to make sure they are in the proper format
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb; // needed for database
using System.Text.RegularExpressions; // used for regular expressions

namespace updateCustomers
{
    public partial class updateCustomersFRM : Form
    {
        public updateCustomersFRM()
        {
            InitializeComponent();
        }

        // method to make sure name does not contain numbers
        bool validateName(string currentName)
        {
            bool nameIsOK = true; // set flag variable (name has no errors) to true

            if (Regex.IsMatch(currentName, "[0-9]")) // checks to see if the name contains numbers
            {
                nameIsOK = false; // if name contains numbers set the flag variable to false -- name is not ok
            }

            return nameIsOK; // return flag variable
        }

        // method to make sure phone number is in the format (###)###-####
        bool validatePhone(string currentPhone)
        {
            bool phoneIsOK = true; // set flag variable (phone number has no errors) to true

            //fixed the boundary to have a max of 4 digits for the last part of the phone number
            if (!Regex.IsMatch(currentPhone, @"\(\d{3}\)\d{3}-\d{4}\b")) // check if phone number is NOT in the format (###)###-####
            {
                phoneIsOK = false; // phone is not in the correct format so set flag variable to false -- phone is NOT ok
            }
            return phoneIsOK; // return flag variable
        }

        // method to check if email has any number of characters, @, any number of characters, ., 3 characters
        bool validateEmail(string currentEmail)
        {
            bool emailIsOK = true; // set flag variable (email has no errors) to true

            if (!Regex.IsMatch(currentEmail, @".+@.+\.[a-zA-Z]{3}\b")) // check to see if format of email doesn't match format described above (line 53)
            {
                emailIsOK = false; // email is not in correct format, set flag variable to false
            }

            return emailIsOK; // return flag variable
        }

        //Prevents from having to use code 2x just saves space
        bool somethingWasClicked()
        {
            string currentName = nameTXT.Text;
            string currentPhone = phoneTXT.Text;
            string currentEmail = emailTXT.Text;
            bool anyErrors = false; // flag variable to see if there are errors

            string fieldsErrors = ""; // string variable to hold label of boxes with errors
            // set all of the text box colors to white
            nameTXT.BackColor = Color.White;
            phoneTXT.BackColor = Color.White;
            emailTXT.BackColor = Color.White;

            if (!validateName(currentName)) // call method to check if name is invalid
            {
                nameTXT.BackColor = Color.Red; // change color of name box 
                anyErrors = true; // set flag variable that there are errors
                fieldsErrors += "Name "; // add name to the list of boxes with errors
            }

            if (!validatePhone(currentPhone)) // call method to check if phone number is invalid
            {
                phoneTXT.BackColor = Color.Red;  // change color of phone number box
                anyErrors = true;   // set flag variable that there are errors
                fieldsErrors += "Phone "; // add phone to the list of boxes with errors
            }

            if (!validateEmail(currentEmail)) // call method to check if email is invalid
            {
                emailTXT.BackColor = Color.Red;  // change color of email box
                anyErrors = true;   // set flag variable that there are errors
                fieldsErrors += "Email "; // add email to the list of boxes with errors
            }

            if (anyErrors) // check to see if there were any errors
            {
                // message box to show fields that have errors
                MessageBox.Show("There were errors in your data: " + fieldsErrors,
                    "Errors in Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // message box to say there weren't any errors
                MessageBox.Show("All data was valid!",
                    "Valid Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return true;
            }

        }

        // database connection
        OleDbConnection Conn = new OleDbConnection(); // connection object
        string selectedCustomer; // variable to hold selected customer

        // method to open the database
        private void openDatabase ()
        {
           try // try to catch errors opening the database
            {
                string conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=customerOrdersMore.accdb;Persist Security Info=False;"; // connection string
                Conn.ConnectionString = conn; // add connection string to connection object
                // open database connection
                Conn.Open();
            }
            catch (Exception ex) // catch errors when opening the database
            {
                // error message if database could not open
                MessageBox.Show("Database could not open because: " + ex,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // method to fill the ids drop down box
        private void fillIDs()
        {
            try // try to catch errors when filling the drop down box
            {
                // clear customer number data set
                customerNumbersDS.Clear();
                numberTXT.DisplayMember = "";
                numberTXT.ValueMember = "";

                // select customer ids
                string sql = "select distinct customer_id from customers order by customer_id";

                // use adapter to send query to database
                OleDbDataAdapter daCustomerNum = new OleDbDataAdapter(sql, Conn);
                // fill customer number data set
                daCustomerNum.Fill(customerNumbersDS, "customer_id");

                // assign data source to customer id combobox
                numberTXT.DataSource = customerNumbersDS.Tables[0];
                numberTXT.DisplayMember = "customer_id";
                numberTXT.ValueMember = "customer_id";
            }
            catch (Exception ex) // catch errors when filling the combo box
            {
                // display message if id numbers could not be pulled
                MessageBox.Show("Trouble pulling customers from database because: " + ex,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void fillZips()
        {
            try // try to catch errors when filling the drop down box
            {
                // clear customer number data set
                zipCodeDS.Clear();
                zipTXT.DisplayMember = "";
                zipTXT.ValueMember = "";

                // select customer ids
                string sql = "select distinct zip_code from zip_code_tbl order by zip_code";


                // use adapter to send query to database
                OleDbDataAdapter daCustomerNum = new OleDbDataAdapter(sql, Conn);
                // fill customer number data set
                daCustomerNum.Fill(zipCodeDS, "zip_code");

                // assign data source to customer id combobox
                zipTXT.DataSource = zipCodeDS.Tables[0];
                zipTXT.DisplayMember = "zip_code";
                zipTXT.ValueMember = "zip_code";
            
            }
            
            catch (Exception ex) // catch errors when filling the combo box
            {
                // display message if id numbers could not be pulled
                MessageBox.Show("Trouble pulling zip codes from database because: " + ex,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            
        }

        // method to set readonly true or false to text boxes
        private void editText(bool canEdit)
        {
            // all text boxes setting readonly property value is passed in
            nameTXT.ReadOnly = canEdit;
            addressTXT.ReadOnly = canEdit;
            phoneTXT.ReadOnly = canEdit;
            emailTXT.ReadOnly = canEdit;
            cityTXT.ReadOnly = true;
            stateTXT.ReadOnly = true;
        }

        // method to clear text boxes
        private void clearTextBoxes()
        {
            // clear text boxes
            nameTXT.Clear();
            addressTXT.Clear();
            cityTXT.Clear();
            stateTXT.Clear();
            phoneTXT.Clear();
            emailTXT.Clear();
            zipTXT.SelectedIndex = -1;
        }

        // form load event handler
        private void updateCustomersFRM_Load(object sender, EventArgs e)
        {
            openDatabase(); // call method to open database
            fillIDs(); // call method to fill combo box
            fillZips(); // call method to fill zip combo box
        }

        // selected index change event handler for customer ids combo box
        private void numberTXT_SelectedIndexChanged(object sender, EventArgs e)
        {
            try // check for errors
            {
                 clearTextBoxes(); // call method to clear text boxes
                 selectedCustomer = numberTXT.Text;

                 if (selectedCustomer != "System.Data.DataRowView" &&
                    selectedCustomer != "")
                {

                    string customerInfo = "SELECT customers.customer_id, customers.cust_name, customers.address, customers.zip_code AS customers_zip_code, " +
                         "customers.email_address, customers.phone_number, zip_code_tbl.zip_code AS zip_code_tbl_zip_code, zip_code_tbl.city, zip_code_tbl.state " +
                         "FROM zip_code_tbl INNER JOIN customers ON zip_code_tbl.[zip_code] = customers.[zip_code] " +
                         "where customers.customer_id = '" + selectedCustomer + "'";

                    // define the command object
                    OleDbCommand customerInfoCMD;

                    // setting the command object
                    customerInfoCMD = new OleDbCommand(customerInfo, Conn);

                    // define the reader
                    OleDbDataReader customerRDR = null;

                    //execute the reader
                    customerRDR = customerInfoCMD.ExecuteReader();

                    //read the data
                    customerRDR.Read();

                    //fill text boxes with data
                    nameTXT.Text = customerRDR[1].ToString();
                    addressTXT.Text = customerRDR[2].ToString();
                    zipTXT.Text = customerRDR[3].ToString(); 
                    phoneTXT.Text = customerRDR[5].ToString();
                    emailTXT.Text = customerRDR[4].ToString();

                    zipTXT.Enabled = false;
                    
                }
            }
            catch (Exception excepHelp) // catch any errors
            {
                // display errors in message box
                MessageBox.Show("There was an error: " + excepHelp, "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // add button click
        private void addBTN_Click(object sender, EventArgs e)
        {

            if (addBTN.Text == "Add") // if add button text is Add
            {
                editText(false); // call method to make text boxes not read only
                clearTextBoxes(); // call method to clear all text boxes
                addBTN.Text = "Insert this customer"; // change text on add button
                // disable other buttons
                numberTXT.Enabled = false;
                deleteBTN.Enabled = false;
                updateBTN.Enabled = false;
                zipTXT.Enabled = true;
            }
            else  // if add button text is not Add
            {
                try // try to catch errors
                {
                    //Checks to see if all the names are correct wont let them go on unless its all correct but im too lazy to put cancel button
                    bool continueQuestionMark = somethingWasClicked();
                    if (continueQuestionMark)
                    { 
                    addBTN.Text = "Add"; // change text on add button
                    // collect all the information from the text boxes into variables
                    string name = nameTXT.Text;
                    string address = addressTXT.Text;
                    string phone_num = phoneTXT.Text;
                    string email_address = emailTXT.Text;
                    string zip = zipTXT.SelectedValue.ToString();

                    // define variables for customer number
                    string maxCustomerID;
                    string newCustomerID;

                    //define command object
                    OleDbCommand newIDNumber;

                    //query for getting largest customer number
                    string largestID = "select max(customer_id) from customers";

                    //set the command object up with query and connection
                    newIDNumber = new OleDbCommand(largestID, Conn);

                    //define reader
                    OleDbDataReader numberReader = null;
                    //execute the reader
                    numberReader = newIDNumber.ExecuteReader();
                    // read the data
                    numberReader.Read();
                    // put information into a variable
                    maxCustomerID = numberReader[0].ToString();

                    // create new customer number:
                    //     1. convert maximum number to a double
                    //     2. Add one to that number
                    //     3. convert to string to use in query

                    // create new customer number
                    newCustomerID = Convert.ToString(Convert.ToDouble(maxCustomerID) + 1);

                    // query to insert data into the database
                    string insertQuery = "insert into customers (customer_id, cust_name, " +
                        "address, zip_code, phone_number, email_address ) values ('" + newCustomerID +
                        "', '" + name + "', '" + address + "', '" + zip + "', '" + phone_num + "', '" + email_address + "' )";

                    // command object
                    OleDbCommand insertCommand;
                    // set up command object with our query and connection
                    insertCommand = new OleDbCommand(insertQuery, Conn);

                    //tell the database to execute command
                    insertCommand.ExecuteNonQuery();

                    // reload customer id combo box so new customer number will display
                    fillIDs();
                    // set textboxes to readonly
                    editText(true);
                    // enable buttons
                    numberTXT.Enabled = true;
                    deleteBTN.Enabled = true;
                    updateBTN.Enabled = true;
                    zipTXT.Enabled = false;
                }
            }
                catch (Exception exceptionError) // catch errors when inserting
                {
                    // message box to show error
                    MessageBox.Show("Error when inserting customer: " + exceptionError,
                        "Error during insert",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // delete button click event handler
        private void deleteBTN_Click(object sender, EventArgs e)
        {
            // create dialogresult variable
            DialogResult sure;
            // ask the user and store the answer in the variable
            sure = MessageBox.Show("Are you sure you would like to delete this customer?",
                "Deletion Confirmation",
                MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

            // if the user selects yes continue with the delete
            if (sure == DialogResult.Yes)
            {
                try // try to catch errors
                {
                    // retrieve selected value from drop down box
                    selectedCustomer = numberTXT.SelectedValue.ToString();
                    OleDbCommand deleteCommand; // command object to send query
                    // query used to delete selected customer

                    //!?! Change this string
                    string deleteQuery = "delete from customers where customer_id = '" +
                        selectedCustomer + "'";
                    // set up command object with query and connection
                    deleteCommand = new  OleDbCommand(deleteQuery, Conn);

                    deleteCommand.ExecuteNonQuery(); // execute query
                    // message box to display that customer was deleted successfully
                    MessageBox.Show("Customer was deleted successfully",
                        "Deletion Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    clearTextBoxes(); // call method to clear text boxes
                    fillIDs(); // call method to refill combo box so deleted customer will not show
                }
                catch (Exception exceptionVariable) // catch errors when deleting chosen customer
                {
                    // message box to display error when deleting chosen customer
                    MessageBox.Show("There was an error when trying to delete customer: " + exceptionVariable,
                        "Error during delete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // update button click event handler
        private void updateBTN_Click(object sender, EventArgs e)
        {

            try // try to catch errors
            {

                if (updateBTN.Text == "Update") // if text on button is Update....
                {
                    editText(false); // call method to set read only property on text boxes to false
                    // disable other buttons
                    numberTXT.Enabled = false;
                    deleteBTN.Enabled = false;
                    addBTN.Enabled = false;
                    zipTXT.Enabled = true;
                    // change text on button
                    updateBTN.Text = "Save these changes";
                }
                else // if text on button is not Update
                {
                    //Checks to see if all the names are correct wont let them go on unless its all correct but im too lazy to put cancel button
                    bool continueQuestionMark = somethingWasClicked();
                    if(continueQuestionMark)
                    {
                        selectedCustomer = numberTXT.SelectedValue.ToString();//retrieve selected customer id
                        // pull data from text boxes into variables
                        string name = nameTXT.Text;
                        string address = addressTXT.Text;
                        string phone_num = phoneTXT.Text;
                        string email_address = emailTXT.Text;
                        string zip = zipTXT.SelectedValue.ToString();

                        // command object to update database
                        OleDbCommand updateCommand;
                        // query to update customer record

                        string updateQuery = "update customers set cust_name = '" + name + 
                            "', address = '" + address + "', zip_code = '" + zip + "', phone_number = '" + phone_num + "', email_address = '" + email_address + "'" +
                            " where customer_id = '" + selectedCustomer + "'";

                        updateCommand = new OleDbCommand(updateQuery, Conn); // set up command object with query and connection
                        updateCommand.ExecuteNonQuery(); // execute query

                        editText(true); // call method to change the read only property to true for all the text boxes
                        // enable buttons
                        numberTXT.Enabled = true;
                        deleteBTN.Enabled = true;
                        addBTN.Enabled = true;
                        //Zip unenabling editability
                        zipTXT.Enabled = false;
                        // change text on button
                        updateBTN.Text = "Update";
                        // message box to inform user that changes have been saved successfully
                        MessageBox.Show("Changes have been saved successfully",
                            "Successful Update",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                }
            catch (Exception exceptionVariable) // catch errors
            {
                // message box to display error when editing chosen customer
                MessageBox.Show("There was an error when trying to update customer record: " + exceptionVariable,
                    "Error during update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void zipTXT_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Whenever the zip is changed it updates the city/state to go along with it

            try // check for errors
            {
                string selectedZip = zipTXT.Text;
                if (selectedZip != "System.Data.DataRowView"
                    && selectedZip != ""
                     )
                {
                    string zipInfo = "SELECT zip_code_tbl.[zip_code], zip_code_tbl.[city], zip_code_tbl.[state] " +
                        "FROM zip_code_tbl " +
                         "where zip_code_tbl.[zip_code] = '" + selectedZip + "'";

                    // define the command object
                    OleDbCommand zipCMD;

                    // setting the command object
                    zipCMD = new OleDbCommand(zipInfo, Conn);

                    // define the reader
                    OleDbDataReader zipRDR = null;

                    //execute the reader
                    zipRDR = zipCMD.ExecuteReader();

                    //read the data
                    zipRDR.Read();

                    //fill text boxes with data
                    cityTXT.Text = zipRDR[1].ToString();
                    stateTXT.Text = zipRDR[2].ToString();

                }
             
            }

            catch (Exception excepHelp) // catch any errors
            {
                // display errors in message box
                MessageBox.Show("There was an error: " + excepHelp, "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void numberLBL_Click(object sender, EventArgs e)
        {

        }
    }
}
