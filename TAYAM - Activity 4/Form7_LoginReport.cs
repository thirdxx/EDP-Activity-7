using MySql.Data.MySqlClient;
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
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Runtime.InteropServices;


namespace TAYAM___Activity_4
{
    public partial class Form7 : Form
    {
        private Timer timer;
        public Form7()
        {
            InitializeComponent();

            LoadLoginReportData();

            // Initialize Timer
            timer = new Timer();
            timer.Interval = 1000; // Update every 1 second
            timer.Tick += timer1_Tick;

            // Start the timer
            timer.Start();

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Format and update the label with the current time and date
            label6.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }

        private void Form7_Load(object sender, EventArgs e)
        {

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
            Form7 loginform = new Form7();
            loginform.Show();
            this.Hide();
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            Form8 dailyattendance = new Form8();
            dailyattendance.Show();
            this.Hide();
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            Form9 monthlyattendance = new Form9();
            monthlyattendance.Show();
            this.Hide();
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

        private void iconButton2_Click(object sender, EventArgs e)
        {
            UpdateUserStatusToInactive(UserSession.LoggedInUsername);
            UserSession.ClearLoggedInUser();

            RecordLogoutEvent();

            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }
        // Method to load login report data into DataGridView in Form7
        private void LoadLoginReportData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();
                    string query = "SELECT first_name, middle_name, last_name, date, login_time, logout_time, status FROM login_report";
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";
        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }
        private void ExportToExcel()
        {
            try
            {
                string templateFilePath = @"C:\Users\63912\Desktop\EDP\Activity 4\LoginReport.xlsx";

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
                 
                    worksheet.Cells[rowIndex, 5] = row.Cells["first_name"].Value?.ToString(); 
                    worksheet.Cells[rowIndex, 6] = row.Cells["middle_name"].Value?.ToString(); 
                    worksheet.Cells[rowIndex, 7] = row.Cells["last_name"].Value?.ToString();
                    worksheet.Cells[rowIndex, 8] = row.Cells["date"].Value?.ToString(); 
                    worksheet.Cells[rowIndex, 9] = row.Cells["login_time"].Value?.ToString(); 
                    worksheet.Cells[rowIndex, 10] = row.Cells["logout_time"].Value?.ToString(); 
                    worksheet.Cells[rowIndex, 11] = row.Cells["status"].Value?.ToString(); 
                    rowIndex++; 
                }

                Excel.Range dateCell = worksheet.Range["F7"];
                dateCell.NumberFormat = "dddd, MMMM d, yyyy";

                dateCell.Value = DateTime.Now;

                Excel.Worksheet worksheet2 = (Excel.Worksheet)workbook.Sheets[2];
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet2.ChartObjects();
                Excel.ChartObject chartObject = chartObjects.Add(100, 100, 300, 300); 
                Excel.Chart chart = chartObject.Chart;

                Dictionary<DateTime, int> loginCountsPerDate = new Dictionary<DateTime, int>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DateTime date = Convert.ToDateTime(row.Cells["date"].Value).Date;

                    if (date.Year == 2024) 
                    {
                        if (loginCountsPerDate.ContainsKey(date))
                        {
                            loginCountsPerDate[date]++;
                        }
                        else
                        {
                            loginCountsPerDate[date] = 1;
                        }
                    }
                }

                int chartRowIndex = 1;
                foreach (var entry in loginCountsPerDate)
                {
                    worksheet2.Cells[chartRowIndex, 1].Value = entry.Key.ToString("MM/dd"); 
                    worksheet2.Cells[chartRowIndex, 2].Value = entry.Value; 
                    chartRowIndex++;
                }

                Excel.Range chartRange = worksheet2.Range["A1:B" + (chartRowIndex - 1)]; 
                chart.SetSourceData(chartRange);

                chart.ChartType = Excel.XlChartType.xlColumnClustered;

                chart.SeriesCollection(1).Name = "Login Count";

                chart.SeriesCollection(1).Format.Fill.ForeColor.RGB = ColorTranslator.ToOle(Color.Green);

                chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary).TickLabelFormat = "mm/dd";

                chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary).TickLabelFormat = "0";

                workbook.Save();

                //workbook.Close();
                //excelApp.Quit();
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                MessageBox.Show("Login report exported to Excel file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
               // MessageBox.Show($"An error occurred while exporting the login report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


