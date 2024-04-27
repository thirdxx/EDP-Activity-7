using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;

namespace TAYAM___Activity_4
{
    public partial class Form16 : Form
    {
        public Form16(Employee loggedInEmployee)
        {
            InitializeComponent();
            LoadLeaveFormDataFromDatabase();

            if (loggedInEmployee != null)
            {
                guna2TextBox1.Text = loggedInEmployee.ID.ToString(); 
                guna2TextBox2.Text = $"{loggedInEmployee.FirstName} {loggedInEmployee.MiddleName} {loggedInEmployee.LastName}";
                guna2TextBox6.Text = loggedInEmployee.Email; 
                guna2TextBox9.Text = loggedInEmployee.PhoneNumber; 
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
            }
        }


            private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form16_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog1.FileName;

            try
            {
                string fileContent = File.ReadAllText(filePath);

                MessageBox.Show($"File content:\n\n{fileContent}");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a File";
                openFileDialog.Filter = "All Files (*.*)|*.*";

                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    MessageBox.Show($"Selected file: {filePath}");
                }
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            Form13 EmployeeDashboard = new Form13();
            EmployeeDashboard.Show();
            this.Hide();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

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
                                MessageBox.Show("No rows returned for the logged-in username.");
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
        private void iconButton4_Click(object sender, EventArgs e)
        {
            string leaveType = comboBox1.SelectedItem?.ToString();
            string reasons = guna2TextBox4.Text;
            string department = guna2TextBox5.Text;
            string position = guna2TextBox3.Text;
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            Employee loggedInEmployee = GetLoggedInEmployeeInfo();
            if (loggedInEmployee != null)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                    {
                        connection.Open();

                        string query = "SELECT COUNT(*) FROM leave_forms WHERE employee_id = @EmployeeID";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EmployeeID", loggedInEmployee.ID);

                            int existingRecordCount = Convert.ToInt32(command.ExecuteScalar());

                            if (existingRecordCount > 0)
                            {
                                // Update existing record
                                query = @"UPDATE leave_forms 
                                  SET employee_name = @EmployeeName,
                                      email = @Email,
                                      phone_number = @PhoneNumber,
                                      department = @Department,
                                      position = @Position,
                                      leave_type = @LeaveType,
                                      reasons = @Reasons,
                                      start_date = @StartDate,
                                      end_date = @EndDate,
                                      submission_date = @SubmissionDate,
                                      status = 'Pending'
                                  WHERE employee_id = @EmployeeID";
                            }
                            else
                            {
                                // Insert new record
                                query = @"INSERT INTO leave_forms (employee_id, employee_name, email, phone_number, department, position, leave_type, reasons, start_date, end_date, submission_date, status)
                                  VALUES (@EmployeeID, @EmployeeName, @Email, @PhoneNumber, @Department, @Position, @LeaveType, @Reasons, @StartDate, @EndDate, @SubmissionDate, 'Pending')";
                            }

                            // Reuse the command object with the updated query
                            command.CommandText = query;

                            command.Parameters.AddWithValue("@EmployeeName", $"{loggedInEmployee.FirstName} {loggedInEmployee.MiddleName} {loggedInEmployee.LastName}");
                            command.Parameters.AddWithValue("@Email", loggedInEmployee.Email);
                            command.Parameters.AddWithValue("@PhoneNumber", loggedInEmployee.PhoneNumber);
                            command.Parameters.AddWithValue("@Department", department);
                            command.Parameters.AddWithValue("@Position", position);
                            command.Parameters.AddWithValue("@LeaveType", leaveType);
                            command.Parameters.AddWithValue("@Reasons", reasons);
                            command.Parameters.AddWithValue("@StartDate", startDate);
                            command.Parameters.AddWithValue("@EndDate", endDate);
                            command.Parameters.AddWithValue("@SubmissionDate", DateTime.Now);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Leave form submitted successfully.");

                                // Get the instance of Form13 and update the label
                                Form13 form13 = Application.OpenForms.OfType<Form13>().FirstOrDefault();
                                if (form13 != null)
                                {
                                    form13.UpdateStatusLabel("Leave Request Status: Pending");
                                }
                                else
                                {
                                    form13 = new Form13();
                                    form13.UpdateStatusLabel("Leave Request Status: Pending");
                                    form13.Show();
                                }
                                this.Close();
                                form13.Show();
                            }
                            else
                            {
                                MessageBox.Show("Failed to submit leave form.");
                            }
                        }
                    }

                    this.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
            }
        }
        // Load leave form data from the leave_forms table
        private void LoadLeaveFormDataFromDatabase()
        {
            Employee loggedInEmployee = GetLoggedInEmployeeInfo();
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();

                    string query = "SELECT * FROM leave_forms WHERE employee_id = @EmployeeID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", loggedInEmployee.ID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                comboBox1.SelectedItem = reader["leave_type"].ToString();
                                guna2TextBox4.Text = reader["reasons"].ToString();
                                guna2TextBox5.Text = reader["department"].ToString();
                                guna2TextBox3.Text = reader["position"].ToString();
                                dateTimePicker1.Value = Convert.ToDateTime(reader["start_date"]);
                                dateTimePicker2.Value = Convert.ToDateTime(reader["end_date"]);

                                Form13 form13 = Application.OpenForms.OfType<Form13>().FirstOrDefault();
                                if (form13 != null)
                                {
                                    form13.UpdateStatusLabel("Pending");
                                }
                                else
                                {
                                    MessageBox.Show("Form13 instance not found.");
                                }
                            }
                            else
                            {
                                ClearLeaveFormFields();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show($"Error: {ex.Message}");
            }
        }
 
        private void ClearLeaveFormFields()
        {
            comboBox1.SelectedItem = null;
            guna2TextBox4.Text = "";
            guna2TextBox5.Text = "";
            guna2TextBox3.Text = "";
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
