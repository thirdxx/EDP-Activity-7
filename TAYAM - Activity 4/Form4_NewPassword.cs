using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TAYAM___Activity_4
{
    public partial class Form4 : Form
    {
        private string emailAddress;
        public Form4(string emailAddress)
        {
            InitializeComponent();
            this.emailAddress = emailAddress;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
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
            string newPassword = textBox1.Text;
            string confirmPassword = textBox2.Text;

            if (newPassword == confirmPassword)
            {
                if (UpdatePasswordInDatabase(newPassword))
                {
                    MessageBox.Show("Password updated successfully!");
                    // Redirect to login form (Form1)
                    Form1 loginForm = new Form1();
                    loginForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Failed to update password.");
                }
            }
            else
            {
                MessageBox.Show("Password not confirmed.");
            }
        }

        private bool UpdatePasswordInDatabase(string newPassword)
        {
            string myConnectionString;
            myConnectionString = "server=localhost;uid=root;" +
            "pwd=;database=edpempattendance";

            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE employees SET password = @Password WHERE Email = @Email";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Password", newPassword);
                        command.Parameters.AddWithValue("@Email", emailAddress);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
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

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                textBox1.UseSystemPasswordChar = false;
            }
            else
            {
                textBox1.UseSystemPasswordChar = true;
            }
        }
    }
}
