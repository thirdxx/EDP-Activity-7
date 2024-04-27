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
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Runtime.InteropServices;


namespace TAYAM___Activity_4
{
    public partial class Form9 : Form
    {
        private Timer timer;
        public Form9()
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

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Form9_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                DateTime monthDate = new DateTime(DateTime.Now.Year, i + 1, 1); 
                string monthName = monthDate.ToString("MMMM"); 
                comboBox1.Items[i] = monthName; 
            }

            comboBox1.SelectedIndex = DateTime.Now.Month - 1;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            label4.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Form5 form = new Form5();
            form.Show();
            this.Hide();
        }
        private void UpdateUserStatusToInactive(string username)
        {
            
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Form6 form = new Form6();
            form.Show();
            this.Hide();
        }

        private void iconButton3_Click(object sender, EventArgs e)
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
            UpdateUserStatusToInactive(UserSession.LoggedInUsername);
            UserSession.ClearLoggedInUser();

            RecordLogoutEvent();

            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }
        private void RecordLogoutEvent()
        {
            try
            {
                Employee loggedInEmployee = GetLoggedInEmployeeInfo();

                DateTime logoutTime = DateTime.Now;

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
                    string query = @"SELECT 
                                a.employee_id, 
                                CONCAT(e.FirstName, ' ', e.MiddleName, ' ', e.LastName) AS EmployeeName,
                                a.entry_date, 
                                a.am_in, 
                                a.am_out,
                                a.pm_in, 
                                a.pm_out,
                                a.late,
                                a.overtime,
                                e.TotalAbsent
                             FROM attendance_entries a
                             INNER JOIN employees e ON a.employee_id = e.ID
                             WHERE MONTH(a.entry_date) = MONTH(CURDATE()) AND YEAR(a.entry_date) = YEAR(CURDATE())
                             ORDER BY a.employee_id, a.entry_date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        adapter.Fill(dataTable);

                        DataTable groupedTable = new DataTable();

                        groupedTable.Columns.Add("EmployeeID", typeof(int));
                        groupedTable.Columns.Add("EmployeeName", typeof(string));
                        groupedTable.Columns.Add("TotalAbsent", typeof(int));
                        groupedTable.Columns.Add("EntryDate", typeof(DateTime));
                        groupedTable.Columns.Add("am_in", typeof(string)); 
                        groupedTable.Columns.Add("am_out", typeof(string)); 
                        groupedTable.Columns.Add("pm_in", typeof(string)); 
                        groupedTable.Columns.Add("pm_out", typeof(string)); 
                        groupedTable.Columns.Add("late", typeof(string)); 
                        groupedTable.Columns.Add("overtime", typeof(string)); 
                        //groupedTable.Columns.Add("TotalAbsent", typeof(int)); 


                        int currentEmployeeID = -1;
                        string currentEmployeeName = "";
                        DataRow newRow = null;

                        foreach (DataRow row in dataTable.Rows)
                        {
                            int employeeID = Convert.ToInt32(row["employee_id"]);
                            string employeeName = row["EmployeeName"].ToString();

                            if (employeeID != currentEmployeeID || employeeName != currentEmployeeName)
                            {
                                newRow = groupedTable.Rows.Add();
                                newRow["EmployeeID"] = employeeID;
                                newRow["EmployeeName"] = employeeName;
                                newRow["TotalAbsent"] = row["TotalAbsent"];
                                currentEmployeeID = employeeID;
                                currentEmployeeName = employeeName;
                            }
                            
                            newRow = groupedTable.Rows.Add();
                            newRow["EntryDate"] = row["entry_date"];
                            newRow["am_in"] = row["am_in"];
                            newRow["am_out"] = row["am_out"];
                            newRow["pm_in"] = row["pm_in"];
                            newRow["pm_out"] = row["pm_out"];
                            newRow["late"] = row["late"];
                            newRow["overtime"] = row["overtime"];
                            //newRow["TotalAbsent"] = row["TotalAbsent"];
                        }

                        dataGridView1.DataSource = groupedTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading monthly attendance report data: {ex.Message}");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            label7.Text = DateTime.Now.ToString("MMMM yyyy");
        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }
        private void ExportToExcel()
        {
            try
            {

                string templateFilePath = @"C:\Users\63912\Desktop\EDP\Activity 4\MonthlyAttendanceReport.xlsx";

                if (!File.Exists(templateFilePath))
                {
                    MessageBox.Show("Template file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Excel.Application excelApp = new Excel.Application();
                excelApp.Visible = true;

                Excel.Workbook workbook = excelApp.Workbooks.Open(templateFilePath);
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                Excel.Range dateCell = worksheet.Range["F7"];
                dateCell.NumberFormat = "dddd, MMMM d, yyyy";

                dateCell.Value = DateTime.Now;

                int rowIndex = 12;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {

                    worksheet.Cells[rowIndex, 5] = row.Cells["EmployeeID"].Value?.ToString();
                    worksheet.Cells[rowIndex, 6] = row.Cells["EmployeeName"].Value?.ToString();
                    worksheet.Cells[rowIndex, 7] = row.Cells["EntryDate"].Value?.ToString();
                    worksheet.Cells[rowIndex, 8] = row.Cells["am_in"].Value?.ToString();
                    worksheet.Cells[rowIndex, 9] = row.Cells["am_out"].Value?.ToString();
                    worksheet.Cells[rowIndex, 10] = row.Cells["pm_in"].Value?.ToString();
                    worksheet.Cells[rowIndex, 11] = row.Cells["pm_out"].Value?.ToString();
                    worksheet.Cells[rowIndex, 12] = row.Cells["late"].Value?.ToString();
                    worksheet.Cells[rowIndex, 13] = row.Cells["overtime"].Value?.ToString();
                    worksheet.Cells[rowIndex, 14] = row.Cells["TotalAbsent"].Value?.ToString();
                  
                    rowIndex++;

                }

                Excel.Worksheet worksheet2 = (Excel.Worksheet)workbook.Sheets[2];
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet2.ChartObjects();
                Excel.ChartObject chartObject = chartObjects.Add(100, 100, 300, 300);
                Excel.Chart chart = chartObject.Chart;

                Dictionary<string, int> totalAbsentDaysPerEmployee = new Dictionary<string, int>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string employeeName = row.Cells["EmployeeName"].Value?.ToString();

                    int absentDays;
                    if (int.TryParse(row.Cells["TotalAbsent"].Value?.ToString(), out absentDays))
                    {
                        if (!totalAbsentDaysPerEmployee.ContainsKey(employeeName))
                        {
                            totalAbsentDaysPerEmployee.Add(employeeName, 0);
                        }
                        totalAbsentDaysPerEmployee[employeeName] += absentDays;
                    }
                }

                int chartRowIndex = 1; 
                foreach (var entry in totalAbsentDaysPerEmployee)
                {
                    worksheet2.Cells[chartRowIndex, 1].Value = entry.Key; 
                    worksheet2.Cells[chartRowIndex, 2].Value = entry.Value; 
                    chartRowIndex++;
                }

                Excel.Range chartRange = worksheet2.Range["A1:B" + (chartRowIndex - 1)]; 
                chart.ChartType = Excel.XlChartType.xlColumnClustered;

                chart.SetSourceData(chartRange);

                chart.SeriesCollection(1).Name = "Absent Count";

                chart.SeriesCollection(1).Format.Fill.ForeColor.RGB = ColorTranslator.ToOle(Color.Green);

                workbook.Save();
                //workbook.Close();
                //excelApp.Quit();
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                MessageBox.Show("Monthly Attendance Report exported to Excel file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"An error occurred while exporting the monthly attendance report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
