using Guna.UI2.WinForms;
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
    public partial class Form14 : Form
    {
      
        public Form14(Employee employee)
        {
            InitializeComponent();

            // Display employee information in textboxes
            guna2TextBox1.Text = employee?.ID.ToString();
            guna2TextBox2.Text = employee?.FirstName;
            guna2TextBox3.Text = employee?.MiddleName;
            guna2TextBox4.Text = employee?.LastName;
            guna2TextBox5.Text = employee?.Email;
            guna2TextBox6.Text = employee?.PhoneNumber;

        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            Form13 EmployeeDashboard = new Form13();
            EmployeeDashboard.Show();
            this.Hide();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox1.DisabledState.FillColor = System.Drawing.Color.White;
            guna2TextBox1.DisabledState.ForeColor = System.Drawing.Color.Black;
            guna2TextBox1.Font = new Font("Arial", 10);
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox2.DisabledState.FillColor = System.Drawing.Color.White;
            guna2TextBox2.DisabledState.ForeColor = System.Drawing.Color.Black;
            guna2TextBox2.Font = new Font("Arial", 10);

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox3.DisabledState.FillColor = System.Drawing.Color.White;
            guna2TextBox3.DisabledState.ForeColor = System.Drawing.Color.Black;
            guna2TextBox2.Font = new Font("Arial", 10);
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox4.DisabledState.FillColor = System.Drawing.Color.White;
            guna2TextBox4.DisabledState.ForeColor = System.Drawing.Color.Black;
            guna2TextBox4.Font = new Font("Arial", 10);
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox5.DisabledState.FillColor = System.Drawing.Color.White;
            guna2TextBox5.DisabledState.ForeColor = System.Drawing.Color.Black;
            guna2TextBox5.Font = new Font("Arial", 10);
        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox6.DisabledState.FillColor = System.Drawing.Color.White;
            guna2TextBox6.DisabledState.ForeColor = System.Drawing.Color.Black;
            guna2TextBox6.Font = new Font("Arial", 10);
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
