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
using System.Drawing.Text;


namespace TAYAM___Activity_4
{
    public partial class Form13 : Form
    {
        private Timer timer;
        public Form13()
        {
            InitializeComponent();
            

            // Initialize Timer
            timer = new Timer();
            timer.Interval = 1000; // Update every 1 second
            timer.Tick += timer1_Tick;

            // Start the timer
            timer.Start();
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            int employeeId = GetLoggedInEmployeeId();

            if (employeeId != -1)
            {
                try
                {
                    string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

                    string query = "SELECT status FROM leave_forms WHERE employee_id = @EmployeeId";

                    using (MySqlConnection connection = new MySqlConnection(myConnectionString))
                    {
                        MySqlCommand command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@EmployeeId", employeeId);

                        connection.Open();

                        object statusObj = command.ExecuteScalar();
                        if (statusObj != null && statusObj != DBNull.Value)
                        {
                            string status = statusObj.ToString();

                            label3.Text = $"Leave Request Status: {status}";
                        }
                        else
                        {
                            label3.Text = "No leave status found for the employee.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error retrieving leave status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                label3.Text = "Invalid employee ID.";
            }
            //string firstName = GetLoggedInEmployeeFirstName();
            //label4.Text = "Welcome, " + firstName + "!";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private string loggedInUsername;

        private void SetLoggedInUser(string username)
        {
            loggedInUsername = username;
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
                    //MessageBox.Show($"Error: {ex.Message}");
                }

                return null;
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Employee employee = GetLoggedInEmployeeInfo();

            Form14 accountInfoForm = new Form14(employee);
            accountInfoForm.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label8.Text = DateTime.Now.ToString("h:mm:ss tt : dddd, MMMM dd, yyyy");
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
     
        private void iconButton4_Click(object sender, EventArgs e)
        {
            Employee loggedInEmployee = GetLoggedInEmployeeInfo();

            if (loggedInEmployee != null)
            {
                int employeeID = loggedInEmployee.ID;

                DateTime currentTime = DateTime.Now.Date;

                // Check if the current time is after 12:00 PM
                if (DateTime.Now.TimeOfDay >= TimeSpan.FromHours(12))
                {
                    MessageBox.Show("AM IN time cannot be recorded after 12:00 PM.");
                    return;
                }

                TimeSpan workingStart = new TimeSpan(8, 0, 0);
                TimeSpan workingEnd = new TimeSpan(11, 59, 0);

                TimeSpan currentInTime = DateTime.Now.TimeOfDay;

                string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";
                string checkQuery = "SELECT COUNT(*) FROM attendance_entries WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";
                string insertQuery = "INSERT INTO attendance_entries (employee_id, entry_date, am_in, late) VALUES (@EmployeeID, @EntryDate, @AMIn, @Late)";
                string updateQuery = "UPDATE attendance_entries SET am_in = @AMIn, late = @Late WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";

                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, conn))
                    {
                        checkCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                        checkCommand.Parameters.AddWithValue("@EntryDate", currentTime);

                        int existingEntriesCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        // Calculate lateness
                        TimeSpan lateDuration = currentInTime - workingStart;
                        if (lateDuration.TotalMinutes > 0)
                        {
                            if (existingEntriesCount > 0)
                            {
                                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                    updateCommand.Parameters.AddWithValue("@EntryDate", currentTime);
                                    updateCommand.Parameters.AddWithValue("@AMIn", DateTime.Now.TimeOfDay);
                                    updateCommand.Parameters.AddWithValue("@Late", lateDuration);

                                    updateCommand.ExecuteNonQuery();
                                    string lateMessage;
                                    if (lateDuration.TotalMinutes > 0) // Employee is late
                                    {
                                        string lateDurationString;
                                        if (lateDuration.TotalHours >= 1)
                                        {
                                            lateDurationString = $"{(int)lateDuration.TotalHours} hour{(lateDuration.TotalHours >= 2 ? "s" : "")} and {lateDuration.Minutes} minute{(lateDuration.Minutes >= 2 ? "s" : "")}";
                                        }
                                        else
                                        {
                                            lateDurationString = $"{lateDuration.TotalMinutes} minute{(lateDuration.Minutes >= 2 ? "s" : "")}";
                                        }

                                        lateMessage = $"AM IN time recorded successfully. Late: {lateDurationString}";
                                    }
                                    else
                                    {
                                        lateMessage = "AM IN time recorded successfully.";
                                    }

                                    MessageBox.Show(lateMessage);
                                }
                            }
                            else
                            {
                                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                    insertCommand.Parameters.AddWithValue("@EntryDate", currentTime);
                                    insertCommand.Parameters.AddWithValue("@AMIn", DateTime.Now.TimeOfDay);
                                    insertCommand.Parameters.AddWithValue("@Late", lateDuration); // Record lateness

                                    insertCommand.ExecuteNonQuery();
                                    string lateMessage;
                                    if (lateDuration.TotalMinutes > 0) // Employee is late
                                    {
                                        string lateDurationString;
                                        if (lateDuration.TotalHours >= 1)
                                        {
                                            lateDurationString = $"{(int)lateDuration.TotalHours} hour{(lateDuration.TotalHours >= 2 ? "s" : "")} and {lateDuration.Minutes} minute{(lateDuration.Minutes >= 2 ? "s" : "")}";
                                        }
                                        else
                                        {
                                            lateDurationString = $"{lateDuration.TotalMinutes} minute{(lateDuration.Minutes >= 2 ? "s" : "")}";
                                        }

                                        lateMessage = $"AM IN time recorded successfully. Late: {lateDurationString}";
                                    }
                                    else
                                    {
                                        lateMessage = "AM IN time recorded successfully.";
                                    }

                                    MessageBox.Show(lateMessage);
                                }
                            }
                        }
                        else
                        {
                            if (existingEntriesCount > 0)
                            {
                                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                    updateCommand.Parameters.AddWithValue("@EntryDate", currentTime);
                                    updateCommand.Parameters.AddWithValue("@AMIn", DateTime.Now.TimeOfDay);
                                    updateCommand.Parameters.AddWithValue("@Late", TimeSpan.Zero);

                                    updateCommand.ExecuteNonQuery();
                                    MessageBox.Show("AM IN time updated successfully.");
                                }
                            }
                            else
                            {
                                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                    insertCommand.Parameters.AddWithValue("@EntryDate", currentTime);
                                    insertCommand.Parameters.AddWithValue("@AMIn", DateTime.Now.TimeOfDay);
                                    insertCommand.Parameters.AddWithValue("@Late", TimeSpan.Zero);

                                    insertCommand.ExecuteNonQuery();
                                    MessageBox.Show("AM IN time recorded successfully.");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
            }
            UpdateAttendanceEntries();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Employee loggedInEmployee = GetLoggedInEmployeeInfo();

            if (loggedInEmployee != null)
            {
                int employeeID = loggedInEmployee.ID;

                DateTime currentTime = DateTime.Now.Date;

                if (DateTime.Now.TimeOfDay >= TimeSpan.FromHours(12))
                {
                    MessageBox.Show("AM OUT time cannot be recorded after 12:00 PM.");
                    return;
                }

                string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";
                string checkQuery = "SELECT COUNT(*) FROM attendance_entries WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";
                string insertQuery = "INSERT INTO attendance_entries (employee_id, entry_date, am_out) VALUES (@EmployeeID, @EntryDate, @AMOut)";
                string updateQuery = "UPDATE attendance_entries SET am_out = @AMOut WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";

                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, conn))
                    {
                        checkCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                        checkCommand.Parameters.AddWithValue("@EntryDate", currentTime);

                        int existingEntriesCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingEntriesCount > 0)
                        {
                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn))
                            {
                                updateCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                updateCommand.Parameters.AddWithValue("@EntryDate", currentTime);
                                updateCommand.Parameters.AddWithValue("@AMOut", DateTime.Now.TimeOfDay);

                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("AM OUT time recorded successfully.");
                            }
                        }
                        else
                        {
                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn))
                            {
                                insertCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                insertCommand.Parameters.AddWithValue("@EntryDate", currentTime);
                                insertCommand.Parameters.AddWithValue("@AMOut", DateTime.Now.TimeOfDay);

                                insertCommand.ExecuteNonQuery();
                                MessageBox.Show("AM OUT time recorded successfully.");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
            }
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            Employee loggedInEmployee = GetLoggedInEmployeeInfo();

            if (loggedInEmployee != null)
            {
                int employeeID = loggedInEmployee.ID;

                DateTime currentTime = DateTime.Now;

                // Check if the current time is between 12:00 PM and 5:00 PM
                if (currentTime.TimeOfDay >= TimeSpan.FromHours(12) && currentTime.TimeOfDay <= TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(59)))
                {
                    string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";
                    string checkQuery = "SELECT COUNT(*) FROM attendance_entries WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";
                    string insertQuery = "INSERT INTO attendance_entries (employee_id, entry_date, pm_in) VALUES (@EmployeeID, @EntryDate, @PMIn)";
                    string updateQuery = "UPDATE attendance_entries SET pm_in = @PMIn WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";

                    using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                    {
                        conn.Open();

                        using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, conn))
                        {
                            checkCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                            checkCommand.Parameters.AddWithValue("@EntryDate", currentTime.Date);

                            int existingEntriesCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                            if (existingEntriesCount > 0)
                            {
                                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                    updateCommand.Parameters.AddWithValue("@EntryDate", currentTime.Date);
                                    updateCommand.Parameters.AddWithValue("@PMIn", currentTime.TimeOfDay);

                                    updateCommand.ExecuteNonQuery();
                                    //MessageBox.Show("PM IN time updated successfully.");

                                    // Calculate late time
                                    TimeSpan lateDuration = currentTime.TimeOfDay - TimeSpan.FromHours(13); // Consider 1:00 PM as the start of working hours
                                    if (lateDuration.TotalMinutes > 0)
                                    {
                                        string lateMessage;
                                        if (lateDuration.TotalHours >= 1)
                                        {
                                            lateMessage = $"PM IN time recorded successfully. Late: {(int)lateDuration.TotalHours} hour{(lateDuration.TotalHours >= 2 ? "s" : "")} and {lateDuration.Minutes} minute{(lateDuration.Minutes >= 2 ? "s" : "")}";
                                        }
                                        else
                                        {
                                            lateMessage = $"PM IN time recorded successfully. Late: {lateDuration.TotalMinutes} minute{(lateDuration.Minutes >= 2 ? "s" : "")}";
                                        }

                                        MessageBox.Show(lateMessage);
                                    }
                                    else
                                    {
                                        MessageBox.Show("PM IN time recorded successfully.");
                                    }
                                }
                            }
                            else
                            {
                                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                                    insertCommand.Parameters.AddWithValue("@EntryDate", currentTime.Date);
                                    insertCommand.Parameters.AddWithValue("@PMIn", currentTime.TimeOfDay);

                                    insertCommand.ExecuteNonQuery();
                                    MessageBox.Show("PM IN time recorded successfully.");
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error: PM In time can only be recorded between 12:00 PM and 5:00 PM.");
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
             Employee loggedInEmployee = GetLoggedInEmployeeInfo();

    if (loggedInEmployee != null)
    {
        int employeeID = loggedInEmployee.ID;

        DateTime currentTime = DateTime.Now;

        if (currentTime.TimeOfDay >= TimeSpan.FromHours(17) && currentTime.TimeOfDay <= TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59)))
        {
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";
            string checkQuery = "SELECT COUNT(*) FROM attendance_entries WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";
            string insertQuery = "INSERT INTO attendance_entries (employee_id, entry_date, pm_out, overtime) VALUES (@EmployeeID, @EntryDate, @PMOut, @Overtime)";
            string updateQuery = "UPDATE attendance_entries SET pm_out = @PMOut, overtime = @Overtime WHERE employee_id = @EmployeeID AND entry_date = @EntryDate";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                conn.Open();

                using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, conn))
                {
                    checkCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                    checkCommand.Parameters.AddWithValue("@EntryDate", currentTime.Date);

                    int existingEntriesCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    // Calculate overtime
                    TimeSpan workingEnd = new TimeSpan(17, 0, 0);
                    TimeSpan overtimeDuration = currentTime.TimeOfDay - workingEnd;
                    if (overtimeDuration.TotalMinutes < 0)
                    {
                        overtimeDuration = TimeSpan.Zero; // No overtime if the employee doesn't work beyond 5:00 PM
                    }

                    if (existingEntriesCount > 0)
                    {
                        using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn))
                        {
                            updateCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                            updateCommand.Parameters.AddWithValue("@EntryDate", currentTime.Date);
                            updateCommand.Parameters.AddWithValue("@PMOut", currentTime.TimeOfDay);
                            updateCommand.Parameters.AddWithValue("@Overtime", overtimeDuration);

                            updateCommand.ExecuteNonQuery();
                            if (overtimeDuration.TotalMinutes > 0)
                            {
                                string overtimeMessage;
                                if (overtimeDuration.TotalHours >= 1)
                                {
                                    overtimeMessage = $"PM OUT time updated successfully. Overtime for {(int)overtimeDuration.TotalHours} hour{(overtimeDuration.TotalHours >= 2 ? "s" : "")} and {overtimeDuration.Minutes} minute{(overtimeDuration.Minutes >= 2 ? "s" : "")}";
                                }
                                else
                                {
                                    overtimeMessage = $"PM OUT time updated successfully. Overtime for {overtimeDuration.TotalMinutes} minute{(overtimeDuration.Minutes >= 2 ? "s" : "")}";
                                }

                                MessageBox.Show(overtimeMessage);
                            }
                            else
                            {
                                MessageBox.Show("PM OUT time updated successfully.");
                            }
                        }
                    }
                    else
                    {
                        using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn))
                        {
                            insertCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                            insertCommand.Parameters.AddWithValue("@EntryDate", currentTime.Date);
                            insertCommand.Parameters.AddWithValue("@PMOut", currentTime.TimeOfDay);
                            insertCommand.Parameters.AddWithValue("@Overtime", overtimeDuration);

                            insertCommand.ExecuteNonQuery();
                            if (overtimeDuration.TotalMinutes > 0)
                            {
                                string overtimeMessage;
                                if (overtimeDuration.TotalHours >= 1)
                                {
                                    overtimeMessage = $"PM OUT time recorded successfully. Overtime for {(int)overtimeDuration.TotalHours} hour{(overtimeDuration.TotalHours >= 2 ? "s" : "")} and {overtimeDuration.Minutes} minute{(overtimeDuration.Minutes >= 2 ? "s" : "")}";
                                }
                                else
                                {
                                    overtimeMessage = $"PM OUT time recorded successfully. Overtime for {overtimeDuration.TotalMinutes} minute{(overtimeDuration.Minutes >= 2 ? "s" : "")}";
                                }

                                MessageBox.Show(overtimeMessage);
                            }
                            else
                            {
                                MessageBox.Show("PM OUT time recorded successfully.");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("Error: PM Out time can only be recorded between 5:00 PM and 11:59 PM.");
        }
    }
    else
    {
        MessageBox.Show("Error: Unable to retrieve logged-in employee information.");
    }
}

        private void iconButton7_Click(object sender, EventArgs e)
        {
            Employee loggedInEmployee = GetLoggedInEmployeeInfo(); 
            Form16 form16 = new Form16(loggedInEmployee);

            form16.Show();

            this.Close();
        }
        private int GetLoggedInEmployeeId()
        {
            string myConnectionString = "server=localhost;uid=root;pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT ID FROM employees WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Username", UserSession.LoggedInUsername);

                        // Execute the query to get the employee ID
                        object result = command.ExecuteScalar();

                        // Check if the result is not null and is convertible to int
                        if (result != null && int.TryParse(result.ToString(), out int employeeId))
                        {
                            return employeeId;
                        }
                        else
                        {
                            MessageBox.Show("Employee ID not found for the logged-in username.");
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show($"Error: {ex.Message}");
                }

                return -1; // Return -1 if employee ID is not found or an error occurs
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {
           
        }
    public void UpdateStatusLabel(string status)
        {
            label3.Text = status;
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
        // Method to update attendance entries for absence
        private void UpdateAttendanceEntries()
        {
            try
            {
                Employee loggedInEmployee = GetLoggedInEmployeeInfo();

                if (loggedInEmployee != null)
                {
                    bool hasLoggedIn = HasLoggedInToday(loggedInEmployee.ID);

                    if (!hasLoggedIn)
                    {
                        MarkAsAbsent(loggedInEmployee.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating attendance entries: {ex.Message}");
            }
        }

        private bool HasLoggedInToday(int employeeID)
        {
            DateTime today = DateTime.Today;
            using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
            {
                connection.Open();
                string query = @"SELECT COUNT(*) FROM login_report 
                             WHERE employee_id = @EmployeeID AND date = @Today";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@Today", today);

                    int loginCount = Convert.ToInt32(command.ExecuteScalar());
                    return loginCount > 0;
                }
            }
        }

        // Method to mark the employee as absent in attendance entries
        private void MarkAsAbsent(int employeeID)
        {
            try
            {
                DateTime today = DateTime.Today;
                using (MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=edpempattendance"))
                {
                    connection.Open();
                    string query = @"UPDATE attendance_entries 
                                 SET absent = absent + 1
                                 WHERE employee_id = @EmployeeID
                                 AND date = @Today";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeID);
                        command.Parameters.AddWithValue("@Today", today);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Attendance marked as absent.");
                        }
                        else
                        {
                            MessageBox.Show("No attendance entry found for today.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking attendance as absent: {ex.Message}");
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}



