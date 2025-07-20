using EShift_Business.Business.Interface;
using EShift_Business.Business.Services;
using EShift_Business.Models;
using EShift_Business.Repository.Services;
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
    public partial class Customer_Registration_Form : Form
    {
        private ICustomerService _customerService;
        private IUserService _userService;



        public Customer_Registration_Form()
        {
            InitializeComponent();
            _customerService = new CustomerService(new CustomerRepository());
            _userService = new UserService(new UserRepository());
        }

        private bool ValidateNames()
        {
            bool isValid = true;

            // First Name
            if (string.IsNullOrWhiteSpace(txtFName.Text))
            {
                lblFNameError.Text = "First name is required.";
                lblFNameError.Visible = true;
                isValid = false;
            }
            else
            {
                lblFNameError.Text = "";
                lblFNameError.Visible = false;
            }

            // Last Name
            if (string.IsNullOrWhiteSpace(txtLName.Text))
            {
                lblLNameError.Text = "Last name is required.";
                lblLNameError.Visible = true;
                isValid = false;
            }
            else
            {
                lblLNameError.Text = "";
                lblLNameError.Visible = false;
            }

            return isValid;
        }
        private bool ValidateEmailAndPassword()
        {
            bool isValid = true;

            // Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblEmailError.Text = "Email is required.";
                lblEmailError.Visible = true;
                isValid = false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                lblEmailError.Text = "Invalid email format.";
                lblEmailError.Visible = true;
                isValid = false;
            }
            else
            {
                lblEmailError.Text = "";
                lblEmailError.Visible = false;
            }

            // Password
            if (string.IsNullOrWhiteSpace(txtPW1.Text))
            {
                lblPW1Error.Text = "Password is required.";
                lblPW1Error.Visible = true;
                isValid = false;
            }
            else if (!IsValidPassword(txtPW1.Text))
            {
                lblPW1Error.Visible = true;
                lblPW1Error.Text = "Must contain uppercase, lowercase, and a number.";
                isValid = false;
            }
            else
            {
                lblPW1Error.Text = "";
                lblPW1Error.Visible = false;
            }

            // Confirm Password
            if (string.IsNullOrWhiteSpace(txtPW2.Text))
            {
                lblPW2Error.Text = "Please confirm your password.";
                lblPW2Error.Visible = true;
                isValid = false;
            }
            else if (txtPW1.Text != txtPW2.Text)
            {
                lblPW2Error.Text = "Passwords do not match.";
                lblPW2Error.Visible = true;
                isValid = false;
            }
            else
            {
                lblPW2Error.Text = "";
                lblPW2Error.Visible = false;
            }

            return isValid;
        }
        private bool ValidateContact()
        {
            bool isValid = true;

            string contact = txtContact.Text.Trim();

            if (string.IsNullOrWhiteSpace(contact))
            {
                lblContactError.Text = "Contact number is required.";
                lblContactError.Visible = true;
                isValid = false;
            }
            else if (!contact.All(char.IsDigit))
            {
                lblContactError.Text = "Contact number must contain digits only.";
                lblContactError.Visible = true;
                isValid = false;
            }
            else if (contact.Length < 10 || contact.Length > 15)
            {
                lblContactError.Text = "Contact number must be 10–15 digits.";
                lblContactError.Visible = true;
                isValid = false;
            }
            else
            {
                lblContactError.Text = "";
                lblContactError.Visible = false;
            }

            return isValid;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //validation
            if (!ValidateNames() || !ValidateEmailAndPassword() || !ValidateContact())
                return;

            string email = txtEmail.Text.Trim();
            //string password = txtPW1.Text;

            //if (!IsValidEmail(email))
            //{
            //    MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //if (!IsValidPassword(password))
            //{
            //    MessageBox.Show("Password must be at least 6 characters and contain uppercase, lowercase, and a number.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (txtPW1.Text != txtPW2.Text)
            //{
            //    MessageBox.Show("Passwords do not match.", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            // Check if email already exists
            var existingCustomer = _customerService.GetCustomerByEmail(email);
            if (existingCustomer != null)
            {
                MessageBox.Show("This email is already registered. Please log in or use another email.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserRoles role = UserRoles.customer;
            // 1. Save user credentials to user table
            User user = new User
            {
                Email = txtEmail.Text.Trim(),
                Password = txtPW1.Text, // Hash later if needed
                Role = role.ToString(), // Convert enum to string for DB
                IsActive = true

            };

            int userId = _userService.RegisterUser(user); // Returns new user_id

            // 2. Save customer details to customer_details table
            Customer customer = new Customer
            {
                UserId = userId,
                FirstName = txtFName.Text.Trim(),
                LastName = txtLName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                ContactNo = txtContact.Text.Trim(),
                RegistrationDate = DateTime.Now
            };

            _customerService.Register(customer);

            MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 3. Redirect to login form
            LoginForm login = new LoginForm();
            login.Show();
            this.Hide();

        }
        private void txtFName_TextChanged(object sender, EventArgs e)
        {
            lblFNameError.Text = "";
        }

        private void txtLName_TextChanged(object sender, EventArgs e)
        {
            lblLNameError.Text = "";
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            lblEmailError.Text = "";
            lblEmailError.Visible = false;
        }

        private void txtPW1_TextChanged(object sender, EventArgs e)
        {
            lblPW1Error.Text = "";
            lblPW1Error.Visible = false;
        }

        private void txtPW2_TextChanged(object sender, EventArgs e)
        {
            lblPW2Error.Text = "";
            lblPW2Error.Visible = false;
        }
        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            lblContactError.Text = "";
            lblContactError.Visible = false;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 6)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasDigit;
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            this.Close();
            login.Show();
        }

        //private void btnShowPW1_Click(object sender, EventArgs e)
        //{
        //    isPassword1Visible = !isPassword1Visible;

        //    txtPW1.UseSystemPasswordChar = !isPassword1Visible;
        //    btnShowPW1.Text = isPassword1Visible ? "🙈" : "👁";
        //}

        //private void btnShowPW2_Click(object sender, EventArgs e)
        //{
        //    isPassword2Visible = !isPassword2Visible;

        //    txtPW2.UseSystemPasswordChar = !isPassword2Visible;
        //    btnShowPW2.Text = isPassword2Visible ? "🙈" : "👁";
        //}
    }
}
