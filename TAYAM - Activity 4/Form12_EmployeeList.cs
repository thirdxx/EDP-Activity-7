using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TAYAM___Activity_4
{
    public partial class Form12 : Form
    {
        public Form12()
        {
            InitializeComponent();

            // Load employee data when the form is loaded
            LoadEmployeeData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=localh" +
                "ost;uid=root;" +
            "pwd=;database=edpempattendance";
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            // Redirect to Form5 (About)
            Form5 aboutForm = new Form5();
            aboutForm.Show();
            this.Hide();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            // Redirect to Form6 (Dashboard)
            Form6 dashboardForm = new Form6();
            dashboardForm.Show();
            this.Hide();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            // Redirect to Form10 (Add Employee)
            Form10 addEmployeeForm = new Form10();
            addEmployeeForm.Show();
            this.Hide();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            // Redirect to Form11 (Update Employee)
            Form11 updateEmployeeForm = new Form11();
            updateEmployeeForm.Show();
            this.Hide();
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            // Redirect to Form12 (Employee List)
            Form12 employeeListForm = new Form12();
            employeeListForm.Show();
            this.Hide();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Clear the logged-in user and update status to 'Inactive' in the database
            UpdateUserStatusToInactive(UserSession.LoggedInUsername);
            UserSession.ClearLoggedInUser();

            RecordLogoutEvent();

            // Redirect to Form1 (Login)
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }

        private void UpdateUserStatusToInactive(string username)
        {
            // Your logic to update the user status to 'Inactive' in the database
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE employees SET Status = 'Inactive' WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void LoadEmployeeData()
        {
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    // Include 'Status' column in the SQL query
                    string query = "SELECT id, FirstName, MiddleName, LastName, Email, PhoneNumber, Username, Password, Status, UserType FROM employees";

                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the data
                            DataTable dataTable = new DataTable();

                            // Fill the DataTable with the results from the query
                            adapter.Fill(dataTable);

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the ID from the user (you can use a textbox for this)
            int employeeId;
            if (int.TryParse(textBox9.Text, out employeeId))
            {
                // Search for employee details by ID
                DataTable filteredDataTable = GetEmployeeDataTableById(employeeId);

                if (filteredDataTable != null && filteredDataTable.Rows.Count > 0)
                {
                    // Bind the filtered DataTable to the DataGridView
                    dataGridView1.DataSource = filteredDataTable;
                }
                else
                {
                    MessageBox.Show("Employee not found.");

                    // Clear the textboxes and set DataGridView to show all data
                    textBox9.Clear();
                    LoadEmployeeData();
                }
            }
            else
            {
                MessageBox.Show("Invalid Employee ID.");

                // Clear the textboxes and set DataGridView to show all data
                textBox9.Clear();
                LoadEmployeeData();
            }
        }

        private DataTable GetEmployeeDataTableById(int employeeId)
        {
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    string query;

                    // Check if employeeId is provided
                    if (employeeId > 0)
                    {
                        query = "SELECT id, FirstName, MiddleName, LastName, Email, PhoneNumber, Username, Password, Status, UserType FROM employees WHERE id = @EmployeeId";
                    }
                    else
                    {
                        query = "SELECT id, FirstName, MiddleName, LastName, Email, PhoneNumber, Username, Password, Status, UserType FROM employees";
                    }

                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        if (employeeId > 0)
                        {
                            command.Parameters.AddWithValue("@EmployeeId", employeeId);
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            Form7 form = new Form7();
            form.Show();
            this.Hide();
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            Form8 form = new Form8();
            form.Show();
            this.Hide();
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            Form9 form = new Form9();
            form.Show();
            this.Hide();
        }
        private void RecordLogoutEvent()
        {
            try
            {
                // Retrieve logged-in employee info
                Employee loggedInEmployee = GetLoggedInEmployeeInfo();

                // Get current logout time
                DateTime logoutTime = DateTime.Now;

                // Update the login_report table with logout time and status
                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();
                    string query = @"UPDATE login_report SET logout_time = @LogoutTime, status = 'Inactive' 
                      WHERE status = 'Active'";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LogoutTime", logoutTime);
                        //command.Parameters.AddWithValue("@EmployeeID", loggedInEmployee.ID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Logout event recorded successfully.");
                        }
                        else
                        {
                            MessageBox.Show("No active login session found for the current user.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error recording logout event: {ex.Message}");
            }
        }
        private Employee GetLoggedInEmployeeInfo()
        {
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT ID, FirstName, MiddleName, LastName, Email, PhoneNumber FROM employees WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Username", UserSession.LoggedInUsername);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                Employee employee = new Employee
                                {
                                    ID = reader.GetInt32("ID"),
                                    FirstName = reader.GetString("FirstName"),
                                    MiddleName = reader.GetString("MiddleName"),
                                    LastName = reader.GetString("LastName"),
                                    Email = reader.GetString("Email"),
                                    PhoneNumber = reader.GetString("PhoneNumber")
                                };

                                return employee;
                            }
                            else
                            {
                                //MessageBox.Show("No rows returned for the logged-in username.");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }

                return null;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
  
