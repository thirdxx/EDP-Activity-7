using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TAYAM___Activity_4
{
    public partial class Form17 : Form
    {
        public Form17()
        {
            InitializeComponent();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Form5 aboutForm = new Form5();
            aboutForm.Show();
            this.Hide();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Form6 dashboardForm = new Form6();
            dashboardForm.Show();
            this.Hide();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Form10 addEmployeeForm = new Form10();
            addEmployeeForm.Show();
            this.Hide();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            Form11 updateEmployeeForm = new Form11();
            updateEmployeeForm.Show();
            this.Hide();
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            Form12 employeeListForm = new Form12();
            employeeListForm.Show();
            this.Hide();
        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
            this.Hide();
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.Show();
            this.Hide();
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.Show();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadLeaveFormsData();

        }
        private void LoadLeaveFormsData()
        {
            try
            {
                // Clear existing data in DataGridView
                dataGridView1.Rows.Clear();

                // SQL query to select all columns from leave_forms table
                string query = "SELECT * FROM leave_forms";

                // Create MySqlConnection using the connection string
                using (MySqlConnection connection = new MySqlConnection(myConnectionString))
                {
                    // Create MySqlCommand
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Open connection
                    connection.Open();

                    // Execute command and read data
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Read each row and add it to the DataGridView
                        while (reader.Read())
                        {
                            // Add row to DataGridView
                            dataGridView1.Rows.Add(
                                reader["form_id"],
                                reader["employee_id"],
                                reader["start_date"],
                                reader["end_date"],
                                reader["status"]
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
