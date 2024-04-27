using MySql.Data.MySqlClient;
using System.Data.SqlClient;
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
    public partial class Form11 : Form
    {
        private Timer timer;
        public Form11()
        {
            InitializeComponent();

            // Initialize combobox with no items
            comboBox1.DataSource = null;

            // Initialize Timer
            timer = new Timer();
            timer.Interval = 1000; // Update every 1 second
            timer.Tick += timer1_Tick;

            // Start the timer
            timer.Start();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            // Redirect to Form6 (Dashboard)
            Form6 dashboardForm = new Form6();
            dashboardForm.Show();
            this.Hide();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            // Redirect to Form5 (About)
            Form5 aboutForm = new Form5();
            aboutForm.Show();
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

        private void button1_Click(object sender, EventArgs e)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";

            // Get the ID from the user (you can use a textbox for this)
            int employeeId;
            if (int.TryParse(textBox9.Text, out employeeId))
            {
                // Search for employee details by ID
                EmployeeDetails employeeDetails = GetEmployeeDetailsById(employeeId);

                if (employeeDetails != null)
                {
                    // Populate textboxes with employee details
                    textBox1.Text = employeeDetails.FirstName;
                    textBox2.Text = employeeDetails.MiddleName;
                    textBox3.Text = employeeDetails.LastName;
                    textBox4.Text = employeeDetails.Email;
                    textBox5.Text = employeeDetails.PhoneNumber;
                    textBox6.Text = employeeDetails.Username;
                    textBox7.Text = employeeDetails.Password;
                    comboBox1.DataSource = GetUserTypeList(employeeDetails.UserType);
                }
                else
                {
                    MessageBox.Show("Employee not found.");
                }
            }
            else
            {
                MessageBox.Show("Invalid Employee ID.");
            }
        }

        private EmployeeDetails GetEmployeeDetailsById(int employeeId)
        {
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM employees WHERE id = @EmployeeId";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new EmployeeDetails
                                {
                                    FirstName = reader["FirstName"].ToString(),
                                    MiddleName = reader["MiddleName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    UserType = reader["UserType"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
        public class EmployeeDetails
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string UserType { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Get the ID from the user (you can use a textbox for this)
            int employeeId;
            if (int.TryParse(textBox9.Text, out employeeId))
            {
                // Get values from textboxes and combobox
                string firstName = textBox1.Text;
                string middleName = textBox2.Text;
                string lastName = textBox3.Text;
                string email = textBox4.Text;
                string phoneNumber = textBox5.Text;
                string username = textBox6.Text;
                string password = textBox7.Text;
                string userType = comboBox1.SelectedItem?.ToString(); // Assumes user types are listed in the combobox

                // Validate input (you can add more validation as needed)

                // Update employee in the database
                if (UpdateEmployeeInDatabase(employeeId, firstName, middleName, lastName, email, phoneNumber, username, password, userType))
                {
                    MessageBox.Show("Employee account updated successfully!");

                    // Clear the textboxes
                    textBox9.Clear();
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    textBox7.Clear();
                    comboBox1.SelectedIndex = -1; // Clear the combobox selection
                }
                else
                {
                    MessageBox.Show("Failed to update employee account.");
                }
            }
            else
            {
                MessageBox.Show("Invalid Employee ID.");
            }
        }

        private bool UpdateEmployeeInDatabase(int employeeId, string firstName, string middleName, string lastName, string email, string phoneNumber, string username, string password, string userType)
        {
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    string query = "UPDATE employees SET FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, " +
                                   "Email = @Email, PhoneNumber = @PhoneNumber, Username = @Username, Password = @Password, UserType = @UserType " +
                                   "WHERE id = @EmployeeId";

                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", employeeId);
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@MiddleName", middleName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@UserType", userType);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
       
        }
        private List<string> GetUserTypeList(string selectedUserType)
        {
            // Assuming you have a list of user types (admin, employee)
            List<string> userTypes = new List<string> { "admin", "employee" };

            // Set selected user type as the first item
            userTypes.Insert(0, selectedUserType);

            return userTypes;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Format and update the label with the current time and date
            label10.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void iconButton7_Click(object sender, EventArgs e)
        {

        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            Form7 form = new Form7();
            form.Show();
            this.Hide();
        }

        private void iconButton7_Click_1(object sender, EventArgs e)
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
    }

}
