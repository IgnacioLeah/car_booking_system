using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CarBookRequest
{
    public partial class DashBoard : Form
    {
        HttpClient client = new HttpClient();

        //  COLORS
        Color defaultColor = Color.FromArgb(0, 192, 192); // sidebar color
        Color activeColor = Color.FromArgb(0, 160, 215);  // highlight color

        public DashBoard()
        {
            InitializeComponent();
        }

        private async Task LoadDashboardStats()
        {
            try
            {
                string url = "http://127.0.0.1:3000/api/admin/stats";

                var response = await client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(result);

                if (response.IsSuccessStatusCode && data.success == true)
                {
                    lblRenters.Text = data.renters.ToString();
                    lblAvailableCars.Text = data.available.ToString();
                    lblApproved.Text = data.approved.ToString();
                    lblPending.Text = data.pending.ToString();
                    lblRejected.Text = data.rejected.ToString();

                    int rented = (int)data.approved + (int)data.pending;
                    lblRentedCars.Text = rented.ToString();
                }
                else
                {
                    MessageBox.Show("Failed to load dashboard data.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard: " + ex.Message);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadDashboardStats();

            //  DEFAULT ACTIVE BUTTON
            SetActiveButton(btnDashboard);
        }

        // BUTTON HIGHLIGHT FUNCTION
        private void SetActiveButton(Button activeBtn)
        {
            // reset all buttons
            btnDashboard.BackColor = defaultColor;

            // highlight selected
            activeBtn.BackColor = activeColor;
        }

        // REQUEST CLICK
        private void btnRequests_Click_1(object sender, EventArgs e)
        {
            SetActiveButton(btnRequests);

            BookRequest br = new BookRequest();
            br.Show();
            this.Hide();
        }

        // LOGOUT
        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            LoginForm br = new LoginForm();
            br.Show();
            this.Hide();
        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            SetActiveButton(btnDashboard);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnReport);

            Reports d = new Reports();
            d.Show();
            this.Hide();
        }
    }
}