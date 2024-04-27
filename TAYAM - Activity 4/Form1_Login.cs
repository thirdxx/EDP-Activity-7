using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TAYAM___Activity_4
{
    public partial class Form1 : Form
    {
        public Point mouseLocation;
        private Timer timer;
        public Form1()
        {
            InitializeComponent();

            // Initialize Timer
            timer = new Timer();
            timer.Interval = 1000; // Update every 1 second
            timer.Tick += timer1_Tick;

            // Start the timer
            timer.Start();

        }

        private void mouse_Down(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(e.X, e.Y);
        }

        private void mouse_Move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Format and update the label with the current time and date
            label8.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);

            }
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Authenticate user and get user type
            string userType = AuthenticateAndGetUserType(username, password);

            if (userType == "admin")
            {
                // Set the logged-in admin and update status to 'Active' in the database
                UserSession.SetLoggedInUser(username, userType);
                UpdateUserStatusToActive(username);

                // Redirect to Form6 (Admin Dashboard)
                Form6 adminDashboardForm = new Form6();
                adminDashboardForm.Show();
                this.Hide();
            }
            else if (userType == "employee")
            {
                // Set the logged-in employee and update status to 'Active' in the database
                UserSession.SetLoggedInUser(username, userType);
                UpdateUserStatusToActive(username);

                // Redirect to Form13 (Employee Dashboard)
                Form13 employeeDashboardForm = new Form13();
                employeeDashboardForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect username or password.");
            }
            RecordLoginEvent();

        }
        private string GetUserTypeFromDatabase(string username, string password)
        {
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
                "pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserType FROM employees WHERE username = @Username AND password = @Password";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        object result = command.ExecuteScalar();

                        return result != null ? result.ToString() : null;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Redirect to Form2 when the LinkLabel is clicked
            Form2 passwordRecovery = new Form2();
            passwordRecovery.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private bool AuthenticateUser(string username, string password)
        {
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM employees WHERE Username = @Username AND Password = @Password";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        return count > 0;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }

            }

        }
        private string AuthenticateAndGetUserType(string username, string password)
        {
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT UserType FROM employees WHERE Username = @Username AND Password = @Password";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        object result = command.ExecuteScalar();

                        return result != null ? result.ToString() : null;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
        }

        private void UpdateUserStatusToActive(string username)
        {
            // Your logic to update the user status to 'Active' in the database
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE employees SET Status = 'Active' WHERE Username = @Username";
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void RecordLoginEvent()
        {
            try
            {
                Employee loggedInEmployee = GetLoggedInEmployeeInfo(); // Get logged-in employee info
                                                                       // Check if loggedInEmployee is null
                if (loggedInEmployee == null)
                {
                    MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
                    return;
                }


                DateTime loginTime = DateTime.Now; // Get current login time

                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();
                    string query = @"INSERT INTO login_report (first_name, middle_name, last_name, date, login_time, status) 
                         VALUES (@FirstName, @MiddleName, @LastName, @LoginDate, @LoginTime, @Status)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", loggedInEmployee.FirstName);
                        command.Parameters.AddWithValue("@MiddleName", loggedInEmployee.MiddleName);
                        command.Parameters.AddWithValue("@LastName", loggedInEmployee.LastName);
                        command.Parameters.AddWithValue("@LoginDate", DateTime.Today);
                        command.Parameters.AddWithValue("@LoginTime", loginTime);
                        command.Parameters.AddWithValue("@Status", "Active");

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error recording login event: {ex.Message}");
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
                                Form1 login = new Form1();
                                login.Show();
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
