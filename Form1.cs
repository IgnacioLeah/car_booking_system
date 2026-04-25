using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CarBookRequest
{
    public partial class DashBoard : Form
    {
        HttpClient client = new HttpClient();

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
                    // DISPLAY TO LABELS
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

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm br = new LoginForm();
            br.Show();
            this.Hide();
        }

        private void btnRequests_Click(object sender, EventArgs e)
        {
            BookRequest br = new BookRequest();
            br.Show();
            this.Hide();
        }
    }
}
