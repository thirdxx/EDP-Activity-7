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
        private Timer timer;
        public Form17()
        {
            InitializeComponent();
            LoadLeaveFormsData();

            // Initialize Timer
            timer = new Timer();
            timer.Interval = 1000; // Update every 1 second
            timer.Tick += timer1_Tick;

            // Start the timer
            timer.Start();
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

        }
        private void LoadLeaveFormsData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();
                    string query = "SELECT form_id, employee_id, employee_name, email, phone_number, department, position, leave_type, reasons, start_date, end_date, submission_date, status FROM leave_forms WHERE status = 'Pending'";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading leave form data: {ex.Message}");
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
                    // Get the first row from the filteredDataTable
                    DataRow row = filteredDataTable.Rows[0];

                    // Display data from the selected row in the TextBoxes
                    textBox1.Text = row["form_id"].ToString();
                    textBox2.Text = row["employee_id"].ToString();
                    textBox3.Text = row["employee_name"].ToString();
                    textBox5.Text = row["email"].ToString();
                    textBox4.Text = row["phone_number"].ToString();
                    textBox6.Text = row["department"].ToString();
                    textBox10.Text = row["position"].ToString();
                    textBox11.Text = row["leave_type"].ToString();
                    textBox14.Text = row["reasons"].ToString();
                    textBox7.Text = row["start_date"].ToString();
                    textBox8.Text = row["end_date"].ToString();
                    textBox12.Text = row["submission_date"].ToString();
                    textBox13.Text = row["status"].ToString();     
                }
                else
                {
                    MessageBox.Show("Employee not found.");

                    // Clear the textboxes and set DataGridView to show all data
                    ClearTextBoxes();
                    LoadLeaveFormsData();
                }
            }
            else
            {
                MessageBox.Show("Invalid Employee ID.");

                // Clear the textboxes and set DataGridView to show all data
                ClearTextBoxes();
                LoadLeaveFormsData();
            }
        }
        private void ClearTextBoxes()
        {
            textBox1.Clear();
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
                        query = "SELECT form_id, employee_id, employee_name, email, phone_number, department, position, leave_type, reasons, start_date, end_date, submission_date, status FROM leave_forms WHERE employee_id = @EmployeeId AND  status = 'Pending' ";
                    }
                    else
                    {
                        query = "SELECT form_id, employee_id, employee_name, email, phone_number, department, position, leave_type, reasons, start_date, end_date, submission_date, status FROM leave_forms";
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
                //MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int employeeId))
            {
                try
                {
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox10.Clear();
                    textBox11.Clear();
                    textBox12.Clear();
                    textBox13.Clear();
                    textBox14.Clear();

                   
                    string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

                    string query = "UPDATE leave_forms SET status = 'Approved' WHERE employee_id = @EmployeeId";

                    using (MySqlConnection connection = new MySqlConnection(myConnectionString))
                    {
                        MySqlCommand command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@EmployeeId", employeeId);

                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Leave Request Approved!");
                        }
                        else
                        {
                            MessageBox.Show("No rows updated. Employee ID not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid Employee ID.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int employeeId))
            {
                try
                {
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox10.Clear();
                    textBox11.Clear();
                    textBox12.Clear();
                    textBox13.Clear();
                    textBox14.Clear();

                    string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

                    string query = "UPDATE leave_forms SET status = 'Denied' WHERE employee_id = @EmployeeId";

                    using (MySqlConnection connection = new MySqlConnection(myConnectionString))
                    {
                        MySqlCommand command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@EmployeeId", employeeId);

                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Leave Request Denied!");
                        }
                        else
                        {
                            MessageBox.Show("No rows updated. Employee ID not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid Employee ID.");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }
    }

}
