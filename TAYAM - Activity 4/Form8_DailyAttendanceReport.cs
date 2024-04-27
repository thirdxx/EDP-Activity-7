using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TAYAM___Activity_4
{
    public partial class Form8 : Form
    {
        private Timer timer;
        public Form8()
        {
            InitializeComponent();

            LoadAttendanceReport();


            // Initialize Timer
            timer = new Timer();
            timer.Interval = 1000; // Update every 1 second
            timer.Tick += timer1_Tick;

            // Start the timer
            timer.Start();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Form8_Load(object sender, EventArgs e)
        {

        }

        private void iconToolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Format and update the label with the current time and date
            label4.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Form5 form = new Form5();
            form.Show();
            this.Hide();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Form6 form = new Form6();
            form.Show();
            this.Hide();
        }

        private void iconButton3_Click_1(object sender, EventArgs e)
        {
            Form10 form = new Form10();
            form.Show();
            this.Hide();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            Form11 form = new Form11();
            form.Show();
            this.Hide();
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            Form12 form = new Form12();
            form.Show();
            this.Hide();
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

        private void label2_Click(object sender, EventArgs e)
        {

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
        private void LoadAttendanceReport()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();
                    string query = @"SELECT a.employee_id, e.FirstName, e.MiddleName, e.LastName,
                                    a.entry_date, a.am_in, a.am_out,
                                    a.pm_in, a.pm_out, a.late, a.overtime
                             FROM attendance_entries a
                             INNER JOIN employees e ON a.employee_id = e.ID
                             WHERE DATE(a.entry_date) = CURDATE()";

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
                MessageBox.Show($"Error loading login report data: {ex.Message}");
            }
        }
        private void ExportToExcel()
        {
            try
            {
                string templateFilePath = @"C:\Users\63912\Desktop\EDP\Activity 4\DailyAttendanceReport.xlsx";

                if (!File.Exists(templateFilePath))
                {
                    MessageBox.Show("Template file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Excel.Application excelApp = new Excel.Application();
                excelApp.Visible = true;

                Excel.Workbook workbook = excelApp.Workbooks.Open(templateFilePath);
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                int rowIndex = 12;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    worksheet.Cells[rowIndex, 5] = row.Cells["employee_id"].Value?.ToString();
                    worksheet.Cells[rowIndex, 6] = row.Cells["FirstName"].Value?.ToString();
                    worksheet.Cells[rowIndex, 7] = row.Cells["MiddleName"].Value?.ToString();
                    worksheet.Cells[rowIndex, 8] = row.Cells["LastName"].Value?.ToString();
                    worksheet.Cells[rowIndex, 9] = row.Cells["entry_date"].Value?.ToString();
                    worksheet.Cells[rowIndex, 10] = row.Cells["am_in"].Value?.ToString();
                    worksheet.Cells[rowIndex, 11] = row.Cells["am_out"].Value?.ToString();
                    worksheet.Cells[rowIndex, 12] = row.Cells["pm_in"].Value?.ToString();
                    worksheet.Cells[rowIndex, 13] = row.Cells["pm_out"].Value?.ToString();
                    worksheet.Cells[rowIndex, 14] = row.Cells["late"].Value?.ToString();
                    worksheet.Cells[rowIndex, 15] = row.Cells["overtime"].Value?.ToString();
                    rowIndex++;
                }

                Excel.Range dateCell = worksheet.Range["F7"];
                dateCell.NumberFormat = "dddd, MMMM d, yyyy";

                dateCell.Value = DateTime.Now;

                Excel.Worksheet worksheet2 = (Excel.Worksheet)workbook.Sheets[2]; 
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet2.ChartObjects();
                Excel.ChartObject chartObject = chartObjects.Add(100, 100, 300, 300);


                Excel.Chart chart = chartObject.Chart;

                Dictionary<string, double> totalWorkingHoursPerEmployee = new Dictionary<string, double>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string employeeName = row.Cells["FirstName"].Value?.ToString() + " " + row.Cells["LastName"].Value?.ToString();
                    double totalWorkingHours = CalculateTotalWorkingHours(row); 
                    if (totalWorkingHoursPerEmployee.ContainsKey(employeeName))
                    {
                        totalWorkingHoursPerEmployee[employeeName] += totalWorkingHours;
                    }
                    else
                    {
                        totalWorkingHoursPerEmployee[employeeName] = totalWorkingHours;
                    }
                }

                int chartRowIndex = 1;
                foreach (var entry in totalWorkingHoursPerEmployee)
                {
                    worksheet2.Cells[chartRowIndex, 1].Value = entry.Key; 
                    worksheet2.Cells[chartRowIndex, 2].Value = entry.Value; 
                    chartRowIndex++;
                }

                Excel.Range chartRange = worksheet2.Range["A1:B" + (chartRowIndex - 1)]; 

                chart.ChartType = Excel.XlChartType.xlColumnClustered;

                chart.SetSourceData(chartRange);

                chart.SeriesCollection(1).Name = "Working hours count";

                chart.SeriesCollection(1).Format.Fill.ForeColor.RGB = ColorTranslator.ToOle(Color.Green);


                workbook.Save();

                MessageBox.Show("Daily attendance report exported to Excel file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting the daily attendance report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private double CalculateTotalWorkingHours(DataGridViewRow row)
        {
            string amInString = row.Cells["am_in"].Value?.ToString();
            string pmOutString = row.Cells["pm_out"].Value?.ToString();

            DateTime amInTime, pmOutTime;
            if (!DateTime.TryParse(amInString, out amInTime) || !DateTime.TryParse(pmOutString, out pmOutTime))
            {
                return 0.0;
            }

            TimeSpan workingHours = pmOutTime - amInTime;

            return workingHours.TotalHours;
        }


        private void iconButton10_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

    }
}
