using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace CarBookRequest
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient client = new HttpClient();

        // PASSWORD STATE
        private bool isPasswordHidden = true;

        public LoginForm()
        {
            InitializeComponent();

            //PASSWORD HIDDEN
            txtPassword.UseSystemPasswordChar = true;

            // SET DEFAULT ICON
            btnTogglePassword.Text = "👁";

            // FOCUS USERNAME
            this.Load += LoginForm_Load;
        }

        // AUTO FOCUS + HIGHLIGHT USERNAME
        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
            txtUsername.SelectAll(); // highlight text
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }

            try
            {
                string url = "http://127.0.0.1:3000/api/admin/login";

                var data = new
                {
                    admin_id = username,
                    password = password
                };

                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                dynamic obj = JsonConvert.DeserializeObject(result);

                if (response.IsSuccessStatusCode && obj.success == true)
                {
                    MessageBox.Show("Login Successful!");

                    DashBoard dashboard = new DashBoard();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(obj.message.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        //SHOW / HIDE PASSWORD
        private void btnTogglePassword_Click_1(object sender, EventArgs e)
        {
            if (isPasswordHidden)
            {
                txtPassword.UseSystemPasswordChar = false;
                btnTogglePassword.Text = "🙈";
                isPasswordHidden = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                btnTogglePassword.Text = "👁";
                isPasswordHidden = true;
            }
        }
    }
}