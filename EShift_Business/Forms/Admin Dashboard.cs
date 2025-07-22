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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;


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
            _jobService = new JobService(new JobRepository());
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
            LoadJobCounts();

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
            series.ChartType = SeriesChartType.Pie; 

            foreach (var entry in statusCounts)
            {
                series.Points.AddXY(entry.Key, entry.Value);
            }

            chrtJobStatus.Series.Add(series);
        }
       

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
                   staff.Role,                  
                   staff.LicenseNumber,        
                   staff.CreatedDate.ToShortDateString()
               );
            }
        }

        private void btnSAdd_Click_1(object sender, EventArgs e)
        {

            User user = new User
            {
                Email = txtSEmail.Text,
                Password = txtSPassword.Text,  
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
        

            tabAdminDashboard.SelectedTab = tabAssignment;

            txtAJobId.Text = jobId.ToString();
          

            LoadAvailableDrivers();
            LoadAvailableAssistants();
            LoadAvailableLorries();
        }
        public void GoToJobManagementTab(int jobId)
        {
            tabAdminDashboard.SelectedTab = tabJobManagement;

            LoadPendingJobs();
        }

        private void btnJMAssignDriver_Click(object sender, EventArgs e)
        {
            int jobId = Convert.ToInt32(dgvJobDetails.CurrentRow.Cells["JobId"].Value);
            

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

        private void btnADLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDrivers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvDrivers.Rows[e.RowIndex];
                txtADriverId.Text = selectedRow.Cells["StaffId"].Value.ToString(); // assuming StaffId is the column name
                txtADriverName.Text = selectedRow.Cells["FirstName"].Value.ToString() + " " + selectedRow.Cells["LastName"].Value.ToString();
            }
        }

        private void btnADriver_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtADriverId.Text))
            {
                int driverId = Convert.ToInt32(txtADriverId.Text);

                // Update availability state to "assigned"
                bool updated = _staffService.UpdateStaffAvailability(driverId, "assigned");

                if (updated)
                {
                    MessageBox.Show("Driver assigned successfully!");
                    LoadAvailableDrivers(); // refresh list
                    CheckAndUpdateJobStatus();
                }
                else
                {
                    MessageBox.Show("Failed to assign driver.");
                }
            }
        }

        private void dgvAssistant_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvAssistant.Rows[e.RowIndex];
                txtAAssistantId.Text = selectedRow.Cells["StaffId"].Value.ToString(); // Column name in DGV
                txtAAssistantName.Text = selectedRow.Cells["FirstName"].Value.ToString() + " " + selectedRow.Cells["LastName"].Value.ToString();
            }

        }

        private void btnAAssistant_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAAssistantId.Text))
            {
                int assistantId = Convert.ToInt32(txtAAssistantId.Text);
                bool updated = _staffService.UpdateStaffAvailability(assistantId, "assigned");

                if (updated)
                {
                    MessageBox.Show("Assistant assigned successfully!");
                    LoadAvailableAssistants(); // Refresh list
                    CheckAndUpdateJobStatus();
                }
                else
                {
                    MessageBox.Show("Failed to assign assistant.");
                }
            }

        }

        private void dgvLorry_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvLorry.Rows[e.RowIndex];
                txtALorryId.Text = selectedRow.Cells["Id"].Value.ToString(); // lorry_id
                txtAPlateNo.Text = selectedRow.Cells["Registration_No"].Value.ToString(); // registration_no
            }
        }

        private void btnALorry_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtALorryId.Text))
            {
                int lorryId = Convert.ToInt32(txtALorryId.Text);
                bool updated = _lorryService.UpdateLorryAvailability(lorryId, "assigned");

                if (updated)
                {
                    MessageBox.Show("Lorry assigned successfully!");
                    LoadAvailableLorries(); // Refresh list
                    CheckAndUpdateJobStatus();
                }
                else
                {
                    MessageBox.Show("Failed to assign lorry.");
                }
            }
        }
        private void CheckAndUpdateJobStatus()
        {
            if (!string.IsNullOrEmpty(txtADriverId.Text) &&
                !string.IsNullOrEmpty(txtAAssistantId.Text) &&
                !string.IsNullOrEmpty(txtALorryId.Text))
            {
                int jobId = Convert.ToInt32(txtAJobId.Text);
                bool updated = _jobService.UpdateJobStatus(jobId, "assigned");

                if (updated)
                    MessageBox.Show("Job status updated to 'assigned'.");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        public void GenerateMonthlyRevenuePDF(Dictionary<string, float> revenueData)
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Monthly Revenue Report";

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
                XFont headerFont = new XFont("Verdana", 12, XFontStyle.Bold);
                XFont bodyFont = new XFont("Verdana", 12);

                double yPoint = 40;

                gfx.DrawString("Monthly Revenue Report", titleFont, XBrushes.Black, new XRect(0, yPoint, page.Width, 30), XStringFormats.TopCenter);
                yPoint += 40;

                gfx.DrawString("Month", headerFont, XBrushes.Black, 50, yPoint);
                gfx.DrawString("Revenue (LKR)", headerFont, XBrushes.Black, 250, yPoint);
                yPoint += 25;

                float total = 0;
                foreach (var kvp in revenueData)
                {
                    gfx.DrawString(kvp.Key, bodyFont, XBrushes.Black, 50, yPoint);
                    gfx.DrawString(kvp.Value.ToString("0.00"), bodyFont, XBrushes.Black, 250, yPoint);
                    total += kvp.Value;
                    yPoint += 20;
                }

                yPoint += 10;
                gfx.DrawString("Total", headerFont, XBrushes.Black, 50, yPoint);
                gfx.DrawString(total.ToString("0.00"), headerFont, XBrushes.Black, 250, yPoint);

                string filePath = @"C:\Reports\Monthly_Revenue_Report.pdf";
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                document.Save(filePath);

                Process.Start(filePath); // Optional: open PDF automatically
            }

        private void btnRevenueR_Click(object sender, EventArgs e)
        {
            string selectedType = cmbRevenueType.SelectedItem.ToString();

            Dictionary<string, float> revenueData;

            if (selectedType == "Monthly")
            {
                revenueData = _paymentService.GetMonthlyRevenue();
                GenerateRevenuePDF(revenueData, "Monthly Revenue Report");
            }
            else if (selectedType == "Yearly")
            {
                revenueData = _paymentService.GetYearlyRevenue();
                GenerateRevenuePDF(revenueData, "Yearly Revenue Report");
            }
            else
            {
                MessageBox.Show("Please select a valid report type.");
            }
        }
        private void GenerateRevenuePDF(Dictionary<string, float> revenueData, string reportTitle)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = reportTitle;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
            XFont headerFont = new XFont("Verdana", 12, XFontStyle.Bold);
            XFont bodyFont = new XFont("Verdana", 12, XFontStyle.Regular);

            double yPoint = 40;

            gfx.DrawString(reportTitle, titleFont, XBrushes.Black, new XRect(0, yPoint, page.Width, 30), XStringFormats.TopCenter);
            yPoint += 40;

            gfx.DrawString("Period", headerFont, XBrushes.Black, 50, yPoint);
            gfx.DrawString("Revenue (LKR)", headerFont, XBrushes.Black, 250, yPoint);
            yPoint += 25;

            float total = 0;
            foreach (var kvp in revenueData)
            {
                gfx.DrawString(kvp.Key, bodyFont, XBrushes.Black, 50, yPoint);
                gfx.DrawString(kvp.Value.ToString("0.00"), bodyFont, XBrushes.Black, 250, yPoint);
                total += kvp.Value;
                yPoint += 20;
            }

            yPoint += 10;
            gfx.DrawString("Total", headerFont, XBrushes.Black, 50, yPoint);
            gfx.DrawString(total.ToString("0.00"), headerFont, XBrushes.Black, 250, yPoint);

            string safeTitle = string.Join("_", reportTitle.Split(Path.GetInvalidFileNameChars()));
            string filePath = "C:\\Reports\\" + safeTitle.Replace(" ", "_") + ".pdf";


            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            document.Save(filePath);

            Process.Start("explorer.exe", filePath);
        }
        private void LoadJobCounts()
        {
            txtTotalJobs.Text = _jobService.GetTotalJobs().ToString();
            txtCompletedJobs.Text = _jobService.GetCompletedJobs().ToString();           

            var statusCounts = _jobService.GetJobCountByStatus();

            // Normalize dictionary keys to lowercase
            var normalizedStatusCounts = statusCounts.ToDictionary(
                pair => pair.Key.ToLower(),
                pair => pair.Value
            );

            txtPendingJobs.Text = normalizedStatusCounts.ContainsKey("pending") ? normalizedStatusCounts["pending"].ToString() : "0";
            txtAssignedJobs.Text = normalizedStatusCounts.ContainsKey("assigned") ? normalizedStatusCounts["assigned"].ToString() : "0";
        }
        private void btnGenerateJobReport_Click(object sender, EventArgs e)
        {

        }

        private void btnReportTotalJobs_Click(object sender, EventArgs e)
        {
            var jobs = _jobService.GetAllJobDetailsWithCustomer();

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Job Summary Report";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont headerFont = new XFont("Verdana", 12, XFontStyle.Bold);
            XFont bodyFont = new XFont("Verdana", 10, XFontStyle.Regular);

            double y = 40;
            gfx.DrawString("Job Summary Report", headerFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 30;

            // Table headers
            gfx.DrawString("Job ID", headerFont, XBrushes.Black, 20, y);
            gfx.DrawString("Customer", headerFont, XBrushes.Black, 70, y);
            gfx.DrawString("Pickup", headerFont, XBrushes.Black, 180, y);
            gfx.DrawString("Dropoff", headerFont, XBrushes.Black, 320, y);
            gfx.DrawString("Pickup Date", headerFont, XBrushes.Black, 450, y);
            gfx.DrawString("Dropoff Date", headerFont, XBrushes.Black, 540, y);

            y += 20;

            foreach (var job in jobs)
            {
                if (y > page.Height - 50) // next page if too long
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(job.JobId.ToString(), bodyFont, XBrushes.Black, 20, y);
                gfx.DrawString(job.CustomerName, bodyFont, XBrushes.Black, 70, y);
                gfx.DrawString(job.PickupAddress, bodyFont, XBrushes.Black, 180, y);
                gfx.DrawString(job.DropoffAddress, bodyFont, XBrushes.Black, 320, y);
                gfx.DrawString(job.PickupDate.ToString("yyyy-MM-dd"), bodyFont, XBrushes.Black, 450, y);
                gfx.DrawString(job.DropoffDate.ToString("yyyy-MM-dd"), bodyFont, XBrushes.Black, 540, y);
                y += 20;
            }

            string filePath = $"C:\\Reports\\Job_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            document.Save(filePath);
            Process.Start("explorer.exe", filePath);

        }
        private void GenerateJobStatusReport(string status)
        {
            var jobs = _jobService.GetJobsByStatus(status);

            PdfDocument document = new PdfDocument();
            document.Info.Title = $"{status} Jobs Report";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont headerFont = new XFont("Verdana", 12, XFontStyle.Bold);
            XFont bodyFont = new XFont("Verdana", 10, XFontStyle.Regular);

            double y = 40;
            gfx.DrawString($"{status} Jobs Report", headerFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 30;

            gfx.DrawString("Job ID", headerFont, XBrushes.Black, 20, y);
            gfx.DrawString("Customer", headerFont, XBrushes.Black, 70, y);
            gfx.DrawString("Pickup", headerFont, XBrushes.Black, 180, y);
            gfx.DrawString("Dropoff", headerFont, XBrushes.Black, 320, y);
            gfx.DrawString("Pickup Date", headerFont, XBrushes.Black, 450, y);
            gfx.DrawString("Dropoff Date", headerFont, XBrushes.Black, 540, y);
            y += 20;

            foreach (var job in jobs)
            {
                if (y > page.Height - 50)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(job.JobId.ToString(), bodyFont, XBrushes.Black, 20, y);
                gfx.DrawString(job.CustomerName, bodyFont, XBrushes.Black, 70, y);
                gfx.DrawString(job.PickupAddress, bodyFont, XBrushes.Black, 180, y);
                gfx.DrawString(job.DropoffAddress, bodyFont, XBrushes.Black, 320, y);
                gfx.DrawString(job.PickupDate.ToString("yyyy-MM-dd"), bodyFont, XBrushes.Black, 450, y);
                gfx.DrawString(job.DropoffDate.ToString("yyyy-MM-dd"), bodyFont, XBrushes.Black, 540, y);
                y += 20;
            }

            string safeTitle = status.Replace(" ", "_");
            string filePath = "C:\\Reports\\" + safeTitle + "_Jobs_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            document.Save(filePath);
            Process.Start("explorer.exe", filePath);
        }
        private void btnReportPendingJobs_Click_1(object sender, EventArgs e)
        {
            GenerateJobStatusReport("Pending");
        }

        private void btnReportCompletedJobs_Click_1(object sender, EventArgs e)
        {
            GenerateJobStatusReport("Completed");
        }

        private void btnReportAssignedJobs_Click_1(object sender, EventArgs e)
        {
            GenerateJobStatusReport("Assigned");
        }
    }
}
