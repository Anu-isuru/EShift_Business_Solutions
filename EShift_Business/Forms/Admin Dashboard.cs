using EShift_Business.Business.Interface;
using EShift_Business.Business.Services;
using EShift_Business.Models;
using EShift_Business.Repository.Interface;
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
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;


namespace EShift_Business.Forms
{
    public partial class Admin_Dashboard : Form
    {
        private IJobService _jobService;
        private ILorryService _lorryService;
        private IPaymentService _paymentService;
        private IUserService _userService;
        private IStaffService _staffService;
        private ICustomerService _customerService;
        private ILoadService _loadService;
        private IContainerRepository _containerRepository;
        private int _selectedJobId;
        private int _selectedCustomerId;
        public Admin_Dashboard()
        {
            InitializeComponent();
            _jobService = new JobService(new JobRepository());// or inject through constructor
            _lorryService = new LorryService(new LorryRepository());
            _paymentService = new PaymentService(new PaymentRepository());
            _userService = new UserService(new UserRepository());
            _staffService = new StaffService(new StaffRepository());
            _customerService = new CustomerService(new CustomerRepository());
            _loadService = new LoadService(new LoadRepository(), new ContainerRepository());


        }

        private void Admin_Dashboard_Load(object sender, EventArgs e)
        {
            headerPanel.Visible = true;

            int totalJobs = _jobService.GetTotalJobs();
            lblTotalJobsV.Text = totalJobs.ToString();
            lblCompleteJobs2.Text = _jobService.GetCompletedJobs().ToString();
            lblAvailableLorries2.Text = _lorryService.GetAvailableLorries().ToString();
            lblPendingPayments2.Text = _paymentService.GetPendingPayments().ToString();
            lblActiveCustomers2.Text = _userService.GetActiveCustomers().ToString();
            
            LoadRevenueChart();
            LoadJobStatusChart();
            LoadStaffData();
            LoadPendingJobs();

            cmbRole.Items.Add("driver");
            cmbRole.Items.Add("assistant");
            cmbRole.Items.Add("clerk");
            cmbRole.Items.Add("manager");

        }
        private void LoadPendingJobs()
        {
            var jobs = _jobService.GetPendingJobs();
            dgvJobDetails.DataSource = jobs;
        }
        private void StaffManagementTab_Load(object sender, EventArgs e)
        {
  

        }
        private void LoadRevenueChart()
        {
            var monthlyRevenue = _paymentService.GetMonthlyRevenue();

            chrtRevenue.Series.Clear();
            Series series = new Series("Monthly Revenue");
            series.ChartType = SeriesChartType.Column;

            foreach (var entry in monthlyRevenue)
            {
                series.Points.AddXY(entry.Key, entry.Value);
            }

            chrtRevenue.Series.Add(series);
        }
        private void LoadJobStatusChart()
        {
            var statusCounts = _jobService.GetJobCountByStatus();

            chrtJobStatus.Series.Clear();
            Series series =
                new Series("Job Status Distribution");
            series.ChartType = SeriesChartType.Pie; // You can use Column or Doughnut too

            foreach (var entry in statusCounts)
            {
                series.Points.AddXY(entry.Key, entry.Value);
            }

            chrtJobStatus.Series.Add(series);
        }
        //private void btnSAdd_Click(object sender, EventArgs e)

        private void LoadStaffData()
        {
            if (dgvStaffDetails.Columns.Count == 0)
            {
                dgvStaffDetails.Columns.Add("StaffId", "Staff ID");
                dgvStaffDetails.Columns.Add("FirstName", "First Name");
                dgvStaffDetails.Columns.Add("LastName", "Last Name");
                dgvStaffDetails.Columns.Add("ContactNo", "Contact No");
                dgvStaffDetails.Columns.Add("Email", "Email");
                dgvStaffDetails.Columns.Add("Role", "Role");
                dgvStaffDetails.Columns.Add("LicenseNumber", "LicenseNumber");
                dgvStaffDetails.Columns.Add("CreatedDate", "Created Date");
            }

            List<Staff> staffList = _staffService.GetAllStaff();

            dgvStaffDetails.Rows.Clear();

            foreach (var staff in staffList)
            {
                dgvStaffDetails.Rows.Add(
                   staff.StaffId,
                   staff.FirstName,
                   staff.LastName,
                   staff.ContactNo,
                   staff.Email,
                   staff.Role,                  // Make sure this is really Role
                   staff.LicenseNumber,        // ✅ Now put license number here
                   staff.CreatedDate.ToShortDateString()
               );
            }
        }

        private void btnSAdd_Click_1(object sender, EventArgs e)
        {

            User user = new User
            {
                Email = txtSEmail.Text,
                Password = txtSPassword.Text,  // Optionally hash this
                Role = cmbRole.SelectedItem.ToString(),
                IsActive = true
            };

            int userId = _userService.RegisterUser(user); // Save user first

            if (userId > 0)
            {
                Staff staff = new Staff
                {
                    FirstName = txtSFName.Text,
                    LastName = txtSLName.Text,
                    ContactNo = txtSContact.Text,
                    Email = txtSEmail.Text,
                    LicenseNumber = txtSLicense.Text,
                    AvailabilityStatus = "available", // default
                    CreatedDate = dtpSCreatedDate.Value,
                    UserId = userId
                };

                bool result = _staffService.AddStaff(staff);

                if (result)
                {
                    MessageBox.Show("Staff added successfully!");
                    LoadStaffData(); // reload DataGridView if needed
                }
                else
                {
                    MessageBox.Show("Failed to add staff.");
                }
            }
            else
            {
                MessageBox.Show("User registration failed.");
            }

        }

        private void dgvJobDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int jobId = Convert.ToInt32(dgvJobDetails.Rows[e.RowIndex].Cells["JobId"].Value);
                LoadJobDetails(jobId);
            }
        }
        private void LoadJobDetails(int jobId)
        {
            var job = _jobService.GetJobById(jobId);
            var customer = _customerService.GetCustomerByUserId(job.UserId);
            var loads = _loadService.GetLoadsByJobId(jobId);
            var payment = _paymentService.GetPaymentByJobId(jobId);

            // Customer
            txtJMCustomerID.Text = customer.CustomerId.ToString();
            txtJMName.Text = $"{customer.FirstName} {customer.LastName}";
            txtJMEmail.Text = customer.Email;
            txtJMContactNo.Text = customer.ContactNo;

            // Job
            txtJMJobId.Text = job.JobId.ToString();
            txtJMPUAddress.Text = job.PickupAddress;
            txtJMDOAddress.Text = job.DropoffAddress;
            dtpJMPUDate.Value = job.PickupDate;
            dtpJMDODate.Value = job.DropoffDate;

            // Load
            dgvJMLoadDetails.DataSource = loads;

            // Payment
            if (payment != null)
            {
                txtJMPaymentId.Text = payment.PaymentId.ToString();
                txtJMTotal.Text = _paymentService.GetTotalPaymentAmount(jobId).ToString("0.00");
                txtJMPaidDate.Text = payment.PaidDate.ToShortDateString();
                txtJMPaidAmount.Text = payment.PaidAmount.ToString("0.00");
            }
            else
            {
                txtJMPaymentId.Text = "";
                txtJMTotal.Text = "";
                txtJMPaidDate.Text = "";
                txtJMPaidAmount.Text = "";
            }
        }

        private void txtJMDODate_TextChanged(object sender, EventArgs e)
        {

        }
        public void GoToAssignmentTab(int jobId)
        {
            _selectedJobId = jobId;
           // _selectedCustomerId = customerId;

            tabAdminDashboard.SelectedTab = tabAssignment;

            txtAJobId.Text = jobId.ToString();
           // txtACustomerId.Text = customerId.ToString();

            LoadAvailableDrivers();
            LoadAvailableAssistants();
            LoadAvailableLorries();
        }

        private void btnJMAssignDriver_Click(object sender, EventArgs e)
        {
            int jobId = Convert.ToInt32(dgvJobDetails.CurrentRow.Cells["JobId"].Value);
            //int customerId = Convert.ToInt32(dgvJobDetails.CurrentRow.Cells["CustomerId"].Value);

            GoToAssignmentTab(jobId);
        }
        private void LoadAvailableDrivers()
        {
            dgvDrivers.DataSource = _staffService.GetAvailableDrivers();
        }

        private void LoadAvailableAssistants()
        {
            dgvAssistant.DataSource = _staffService.GetAvailableAssistants();
        }

        private void LoadAvailableLorries()
        {
            dgvLorry.DataSource = _lorryService.GetAvailableLorriestoAssign();
        }
    }
}
