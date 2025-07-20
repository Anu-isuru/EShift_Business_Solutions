using EShift_Business.Business.Interface;
using EShift_Business.Business.Services;
using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EShift_Business.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            {
                cmbRole.DataSource = Enum.GetValues(typeof(UserRoles));
            }

        }

        private bool ValidateEmailAndPassword()
        {
            bool isValid = true;

            // Email
            if (string.IsNullOrWhiteSpace(txtUName.Text))
            {
                lblUNameError.Text = "Email is required.";
                lblUNameError.Visible = true;
                isValid = false;
            }
            else
            {
                lblUNameError.Text = "";
                lblUNameError.Visible = false;
            }

            // Password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblPWError.Text = "Password is required.";
                lblPWError.Visible = true;
                isValid = false;
            }
            else
            {
                lblPWError.Text = "";
                lblPWError.Visible = false;
            }
            return isValid;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateEmailAndPassword())
                    return;

                // 1. Parse selected role from ComboBox to enum
                if (!Enum.TryParse<UserRoles>(cmbRole.SelectedItem.ToString(), out var selectedRole))
                {
                    MessageBox.Show("Invalid role selected.");
                    return;
                }

                // 2. Get email and password
                string email = txtUName.Text.Trim();
                string password = txtPassword.Text;

                // 3. Call the UserService to validate login
                IUserService userService = new UserService();
                User user = userService.GetUser(email, password, selectedRole.ToString());

                // 4. If found, redirect to correct dashboard
                if (user != null)
                {
                    MessageBox.Show("Login successful!");

                    switch (selectedRole)
                    {
                        case UserRoles.customer:
                            new Customer_Dashboard(user.UserId).Show();
                            break;
                        case UserRoles.admin:
                            new Admin_Dashboard().Show();
                            break;
                        case UserRoles.clerk:
                            new Clerk_Dashboard().Show();
                            break;
                        case UserRoles.driver:
                            new Driver_Dashboard().Show();
                            break;
                        case UserRoles.assistant:
                            new Assistant_Dashboard().Show();
                            break;
                    }

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Customer_Registration_Form frm = new Customer_Registration_Form();
            frm.Show();
        }

        private void lblHeader_Click(object sender, EventArgs e)
        {

        }
    }
}
