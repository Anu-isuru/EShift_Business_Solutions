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
using Container = EShift_Business.Models.Container;

namespace EShift_Business.Forms
{
    public partial class Customer_Dashboard : Form
    {
        private ICustomerService _customerService;
        private int _loggedInUserId;
        private readonly IContainerService _containerService = new ContainerService();
        private IJobService _jobService = new JobService(new JobRepository());
        private ILoadService loadService;
        private IPaymentService _paymentService = new PaymentService(new PaymentRepository());

        private int currentJobId;
        private bool isEditing;

        public Customer_Dashboard(int userId)
        {
            InitializeComponent();
            _loggedInUserId = userId;
            _customerService = new CustomerService(new CustomerRepository());
            loadService = new LoadService(new LoadRepository(), new ContainerRepository());
            Customer customer = _customerService.GetCustomerByUserId(_loggedInUserId);


            LoadCustomerInfo(); // Call the method on form load
            LoadJobHistory();
            // btnNext.Enabled = false;
            LoadContainerDetails();
            LoadLoadHistory();
            LoadPaymentHistory();
            LoadCustomerProfile();
            LoadCustomerGreeting();

            // Apply read-only properties on form load
            txtCustomerID.ReadOnly = true;
            txtCustomerName.ReadOnly = true;
            txtPULocationID.ReadOnly = true; // Assuming this is the Location ID for Pickup
            txtDOLocationID.ReadOnly = true; // Assuming this is the Location ID for Dropoff
            txtLoadId.ReadOnly = true; // Assuming this is the Load ID
            isEditing = false;
            if (customer != null)
            {
                lblGreeting.Text = $"Hi {customer.FirstName}, Welcome to E-Shift";
            }
        }

        private void Customer_Dashboard_Load(object sender, EventArgs e)
        {
            // Set initial date for pickup to day after tomorrow
            dtPUDate.MinDate = DateTime.Today.AddDays(2);
            dtPUDate.Value = DateTime.Today.AddDays(2);
            dtDODate.MinDate = DateTime.Today.AddDays(2);
        }
        private void LoadCustomerInfo()
        {
            var customer = _customerService.GetCustomerByUserId(_loggedInUserId);
            if (customer != null)
            {
                txtCustomerID.Text = customer.CustomerId.ToString();
                txtCustomerName.Text = customer.FirstName + " " + customer.LastName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validation for not null fields
            if (string.IsNullOrWhiteSpace(txtPUAddress.Text) || string.IsNullOrWhiteSpace(txtPUCity.Text) || string.IsNullOrWhiteSpace(txtPUProvince.Text) ||
                string.IsNullOrWhiteSpace(txtDOAddress.Text) || string.IsNullOrWhiteSpace(txtDOCity.Text) || string.IsNullOrWhiteSpace(txtDOProvince.Text))
            {
                MessageBox.Show("Address, City, and Province for both Pickup and Dropoff cannot be null.");
                return;
            }

            // Validation for pickup date (after day after tomorrow)
            if (dtPUDate.Value <= DateTime.Today.AddDays(2))
            {
                MessageBox.Show("Pickup date must be after day after tomorrow.");
                return;
            }

            // Validation for at least 3 days gap between pickup and dropoff
            if ((dtDODate.Value - dtPUDate.Value).TotalDays < 3)
            {
                MessageBox.Show("Dropoff date must be at least 3 days after Pickup date for preparations.");
                return;
            }
            int pickupId = _jobService.AddLocation(txtPUAddress.Text, txtPUCity.Text, txtPUProvince.Text);
            int dropoffId = _jobService.AddLocation(txtDOAddress.Text, txtDOCity.Text, txtDOProvince.Text);

            Job job = new Job
            {
                UserId = _loggedInUserId, // passed from login
                PickupLocationId = pickupId,
                DropoffLocationId = dropoffId,
                PickupDate = dtPUDate.Value,
                DropoffDate = dtDODate.Value
            };

            int jobId = _jobService.SaveJob(job);
            currentJobId = jobId;
            MessageBox.Show("Job created successfully!");
            LoadJobHistory(); // Refresh history table
            btnNext.Enabled = true;
        }
        private void LoadJobHistory()
        {
            var jobs = _jobService.GetJobHistoryWithLocations(_loggedInUserId);
            dgvJobHistory.DataSource = jobs;

            // Hide unwanted technical/foreign key columns
            dgvJobHistory.Columns["PickupLocationId"].Visible = false;
            dgvJobHistory.Columns["DropoffLocationId"].Visible = false;
            dgvJobHistory.Columns["UserId"].Visible = false;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtJobID.Text, out int selectedJobId))
            {
                MessageBox.Show("Please select a job to continue to Load Assignment.");
                return;
            }

            currentJobId = selectedJobId;

            // Switch to Load Assignment tab
            tabControlCustomer.SelectedTab = tbLoadAssign;

            // Load relevant containers and load history
            LoadContainerDetails();
            LoadLoadHistory();
        }
        private void LoadContainerDetails()
        {
            var containers = _containerService.GetAvailableContainers();
            cmbContainerSize.DataSource = containers;
            cmbContainerSize.DisplayMember = "Size";          // or a custom ToString() format
            cmbContainerSize.ValueMember = "ContainerId";     // make sure this is the actual property name
            dgvContainer.DataSource = containers;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtLoadSize.Text))
            {
                MessageBox.Show("Please fill in description and load size.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLoadSize.Text))
            {
                MessageBox.Show("Please fill in the size of the load.");
                return;
            }
            if (!int.TryParse(txtLoadSize.Text, out int loadSize))
            {
                MessageBox.Show("Please enter a valid number for load size.");
                return;
            }
            if (loadSize > 20)
            {
                MessageBox.Show("The maximum size allowed for a single load is 20ft.\nPlease divide your load into multiple entries (each 20ft or less).", "Size Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (cmbContainerSize.SelectedItem is Container selectedContainer)
            {
                string sizeString = selectedContainer.Size.ToString().Replace("ft", "").Trim();

                if (!int.TryParse(sizeString, out int containerSize))
                {
                    MessageBox.Show("Selected container size is invalid.");
                    return;
                }

                if (containerSize < loadSize)
                {
                    MessageBox.Show($"Selected container is too small for the load.\nLoad size: {loadSize}ft\nContainer size: {containerSize}ft", "Invalid Container", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ Save Load
                var load = new Load
                {
                    Description = txtDescription.Text,
                    Size = loadSize.ToString(),
                    JobId = currentJobId
                };

                List<int> containerIds = new List<int> { selectedContainer.ContainerId };
                bool result = loadService.SaveLoadWithContainers(load, containerIds);

                if (result)
                {
                    MessageBox.Show("Load and container(s) assigned successfully.");
                    LoadLoadHistory();
                }
                else
                {
                    MessageBox.Show("Failed to assign load and containers.");
                }
            }
            else
            {
                MessageBox.Show("Please select a valid container.");
            }
        }


        private void LoadLoadHistory()
        {
            var loads = loadService.GetLoadsByJobId(currentJobId);
            var table = new DataTable();
            table.Columns.Add("Load ID");
            table.Columns.Add("Description");
            table.Columns.Add("Size");
            table.Columns.Add("Status");
            table.Columns.Add("Container IDs");

            foreach (var l in loads)
            {
                table.Rows.Add(l.LoadId, l.Description, l.Size, l.Status, string.Join(",", l.ContainerIds));
            }

            dgvLoadHistory.DataSource = table;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {


            if (!isEditing)
            {
                // Enter edit mode
                isEditing = true;
                btnEdit.Text = "Update"; // Change button text to indicate save action

                //// Make fields editable
                //txtPUAddress.ReadOnly = false;
                //txtPUCity.ReadOnly = false;
                //txtPUProvince.ReadOnly = false;
                //txtDOAddress.ReadOnly = false;
                //txtDOCity.ReadOnly = false;
                //txtDOProvince.ReadOnly = false;
                //dtPUDate.Enabled = true;
                //dtDODate.Enabled = true;

                // Load current job details if available
                if (currentJobId > 0)
                {
                    var job = _jobService.GetJobById(currentJobId);

                    if (job != null)
                    {
                        txtPUAddress.Text = job.PickupAddress;
                        txtPUCity.Text = job.PickupCity;
                        txtPUProvince.Text = job.PickupProvince;
                        txtDOAddress.Text = job.DropoffAddress;
                        txtDOCity.Text = job.DropoffCity;
                        txtDOProvince.Text = job.DropoffProvince;
                    }
                }
            }
            else
            {
                // Save changes
                if (string.IsNullOrWhiteSpace(txtPUAddress.Text) || string.IsNullOrWhiteSpace(txtPUCity.Text) || string.IsNullOrWhiteSpace(txtPUProvince.Text) ||
                    string.IsNullOrWhiteSpace(txtDOAddress.Text) || string.IsNullOrWhiteSpace(txtDOCity.Text) || string.IsNullOrWhiteSpace(txtDOProvince.Text))
                {
                    MessageBox.Show("Address, City, and Province for both Pickup and Dropoff cannot be null.");
                    return;
                }

                if (dtPUDate.Value <= DateTime.Today.AddDays(2))
                {
                    MessageBox.Show("Pickup date must be after day after tomorrow.");
                    return;
                }

                if ((dtDODate.Value - dtPUDate.Value).TotalDays < 3)
                {
                    MessageBox.Show("Dropoff date must be at least 3 days after Pickup date for preparations.");
                    return;
                }

                int pickupId = _jobService.AddLocation(txtPUAddress.Text, txtPUCity.Text, txtPUProvince.Text);
                int dropoffId = _jobService.AddLocation(txtDOAddress.Text, txtDOCity.Text, txtDOProvince.Text);

                var job = _jobService.GetJobById(currentJobId);
                if (job != null)
                {
                    job.PickupLocationId = pickupId;
                    job.DropoffLocationId = dropoffId;
                    job.PickupDate = dtPUDate.Value;
                    job.DropoffDate = dtDODate.Value;

                    bool result = _jobService.UpdateJob(job); // Assuming this method exists
                    if (result)
                    {
                        MessageBox.Show("Job details updated successfully!");
                        LoadJobHistory();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update job details. Check console for details.");
                    }
                }

                // Exit edit mode
                isEditing = false;
                btnEdit.Text = "Edit";
                txtPUAddress.ReadOnly = true;
                txtPUCity.ReadOnly = true;
                txtPUProvince.ReadOnly = true;
                txtDOAddress.ReadOnly = true;
                txtDOCity.ReadOnly = true;
                txtDOProvince.ReadOnly = true;
                dtPUDate.Enabled = false;
                dtDODate.Enabled = false;
            }
        }

        private void dgvJobHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvJobHistory.Rows[e.RowIndex];

                // Load Job ID
                txtJobID.Text = row.Cells["JobId"].Value.ToString();

                // Load Pickup and Dropoff location IDs
                txtPULocationID.Text = row.Cells["PickupLocationId"].Value.ToString();
                txtDOLocationID.Text = row.Cells["DropoffLocationId"].Value.ToString();

                // Load Address Details
                txtPUAddress.Text = row.Cells["PickupAddress"].Value.ToString();
                txtPUCity.Text = row.Cells["PickupCity"].Value.ToString();
                txtPUProvince.Text = row.Cells["PickupProvince"].Value.ToString();

                txtDOAddress.Text = row.Cells["DropoffAddress"].Value.ToString();
                txtDOCity.Text = row.Cells["DropoffCity"].Value.ToString();
                txtDOProvince.Text = row.Cells["DropoffProvince"].Value.ToString();

                // Load Dates
                dtPUDate.Value = Convert.ToDateTime(row.Cells["PickupDate"].Value);
                dtDODate.Value = Convert.ToDateTime(row.Cells["DropoffDate"].Value);

                // Set the current jobId for editing
                currentJobId = Convert.ToInt32(txtJobID.Text);
                btnEdit.Enabled = true; // Enable edit button
            }
        }

        private void btnEditLA_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoadId.Text) || !int.TryParse(txtLoadId.Text, out int loadId))
            {
                MessageBox.Show("Please select a load from the table to edit.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtLoadSize.Text))
            {
                MessageBox.Show("Description and load size are required.");
                return;
            }

            if (!int.TryParse(txtLoadSize.Text, out int loadSize))
            {
                MessageBox.Show("Load size must be a valid number.");
                return;
            }

            if (loadSize > 20)
            {
                MessageBox.Show("Load size must not exceed 20ft. Please divide the load.");
                return;
            }

            if (cmbContainerSize.SelectedItem is EShift_Business.Models.Container selectedContainer)
            {
                string sizeText = selectedContainer.Size.Replace("ft", "").Trim();
                if (!int.TryParse(sizeText, out int containerSize) || containerSize < loadSize)
                {
                    MessageBox.Show("Selected container is too small for this load.");
                    return;
                }

                var load = new Load
                {
                    LoadId = loadId,
                    Description = txtDescription.Text,
                    Size = txtLoadSize.Text,
                    JobId = currentJobId
                };

                bool result = loadService.UpdateLoadWithContainers(load, new List<int> { selectedContainer.ContainerId });
                if (result)
                {
                    MessageBox.Show("Load updated successfully.");
                    LoadLoadHistory();
                }
                else
                {
                    MessageBox.Show("Failed to update load.");
                }
            }
            else
            {
                MessageBox.Show("Please select a container.");
            }
            ClearLoadFields();
        }

        private void btnDeleteLA_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLoadId.Text, out int loadId))
            {
                MessageBox.Show("Please select a load to delete.");
                return;
            }

            // Confirm delete
            var confirm = MessageBox.Show("Are you sure you want to delete this load?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            // Get container IDs assigned to the load
            List<int> containerIds = loadService.GetContainerIdsByLoadId(loadId);
            foreach (int id in containerIds)
            {
                _containerService.MarkContainerAsAvailable(id);
            }

            bool result = loadService.DeleteLoad(loadId);
            if (result)
            {
                MessageBox.Show("Load deleted and container(s) released.");
                LoadLoadHistory();
            }
            else
            {
                MessageBox.Show("Failed to delete load.");
            }
            ClearLoadFields();
        }

        private void dgvLoadHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLoadHistory.Rows[e.RowIndex];

                // Load values from the selected row
                txtLoadId.Text = row.Cells["Load ID"].Value?.ToString();
                txtDescription.Text = row.Cells["Description"].Value?.ToString();
                txtLoadSize.Text = row.Cells["Size"].Value?.ToString();

                // If there are container IDs, load the first one back into the combo box
                if (row.Cells["Container IDs"].Value != null)
                {
                    var containerIds = row.Cells["Container IDs"].Value.ToString().Split(',');
                    if (containerIds.Length > 0 && int.TryParse(containerIds[0], out int containerId))
                    {
                        cmbContainerSize.SelectedValue = containerId;
                    }
                }

                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
        }
        private void ClearLoadFields()
        {
            txtLoadId.Clear();
            txtDescription.Clear();
            txtLoadSize.Clear();
            cmbContainerSize.SelectedIndex = -1;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtJobID.Text, out int jobId))
            {
                bool result = _jobService.DeleteJob(jobId);
                if (result)
                {
                    MessageBox.Show("Job deleted successfully.");
                    LoadJobHistory(); 
                }
            }
            else
            {
                MessageBox.Show("Invalid Job ID.");
            }
        }
        private void LoadTotalAmount()
        {
            if (!int.TryParse(txtJobID.Text, out int selectedJobId))
            {
                MessageBox.Show("Invalid Job ID.");
                return;
            }
            decimal total = _paymentService.GetTotalPaymentAmount(selectedJobId);
            txtTotal.Text = total.ToString("F2");
        }
        private void btnNextinLA_Click(object sender, EventArgs e)
        {
            tabControlCustomer.SelectedTab = tbPayment;
            LoadTotalAmount();
        }

        private void btnSubmitPayment_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtPaidAmount.Text, out decimal paidAmount))
            {
                MessageBox.Show("Invalid amount");
                return;
            }
            decimal total = decimal.Parse(txtTotal.Text);
            decimal PaidAmount = decimal.Parse(txtPaidAmount.Text);

            // 1. Check if payment already exists for this job
            decimal existingTotalPaid = _paymentService.GetTotalPaymentAmount(currentJobId); // Use the correct job ID


            // 2. Check if current payment + previous exceeds total
            if (existingTotalPaid  > total)
            {
                MessageBox.Show("This payment exceeds the total required amount.");
                return;
            }

            // 3. Optional: Force full payment only
            if (paidAmount < total )
            {
                MessageBox.Show($"You need to pay exactly {total - existingTotalPaid} to complete this job.");
                return;
            }

            string method = txtPaymentMethod.Text;
            DateTime paidDate = dtpPaidDate.Value;
            string status = "completed"; 

            bool success = _paymentService.InsertPayment(
                paidAmount,
                paidDate,
                method,
                status,
                currentJobId
            );

            if (success)
            {
                MessageBox.Show("Payment recorded!");
                LoadPaymentHistory();
            }
            else
            {
                MessageBox.Show("Failed to insert payment.");
            }
            LoadPaymentHistory();
        }

        private void LoadPaymentHistory()
        {
            try
            {
                List<Payment> payments = _paymentService.GetPaymentsByCustomerId(_loggedInUserId);
                dgvPaymentHistory.DataSource = payments;

                // Optional: Customize column headers or hide job ID
                dgvPaymentHistory.Columns["JobId"].HeaderText = "Job ID";
                dgvPaymentHistory.Columns["PaymentId"].HeaderText = "Payment ID";
                dgvPaymentHistory.Columns["PaidAmount"].HeaderText = "Amount";
                dgvPaymentHistory.Columns["PaidDate"].HeaderText = "Date";
                dgvPaymentHistory.Columns["PaymentMethod"].HeaderText = "Method";
                dgvPaymentHistory.Columns["Status"].HeaderText = "Status";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payment history: " + ex.Message);
            }
        }

        private void btnCPMUpdate_Click(object sender, EventArgs e)
        {
            Customer updatedCustomer = new Customer
            {
                CustomerId = int.Parse(txtCPMCustomerID.Text),
                FirstName = txtCPMFName.Text,
                LastName = txtCPMLName.Text,
                ContactNo = txtCPMContact.Text
            };

            _customerService.UpdateCustomer(updatedCustomer);
            MessageBox.Show("Profile updated successfully.");
            LoadCustomerInfo();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm(); 
            loginForm.Show();
            this.Close();
        }

        private void LoadCustomerProfile()
        {
            try
            {
                Customer customer = _customerService.GetCustomerByUserId(_loggedInUserId);
                if (customer != null)
                {
                    txtCPMCustomerID.Text = customer.CustomerId.ToString();
                    txtCPMFName.Text = customer.FirstName;
                    txtCPMLName.Text = customer.LastName;
                    txtCPMContact.Text = customer.ContactNo;
                }
                else
                {
                    MessageBox.Show("Customer not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message);
            }
        }
        private void LoadCustomerGreeting()
        {
            Customer customer = _customerService.GetCustomerByUserId(_loggedInUserId);
            if (customer != null)
            {
                lblGreeting.Text = $"Hi {customer.FirstName}, Welcome to E-Shift";
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlCustomer.SelectedTab == tbJobDetails)
            {
                LoadCustomerInfo();  
                LoadCustomerGreeting();
            }

            if (tabControlCustomer.SelectedTab == tbProfile)
            {
                LoadCustomerProfile(); 
                LoadCustomerGreeting();
            }
        }

        private void pbLogo_Click(object sender, EventArgs e)
        {

        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
