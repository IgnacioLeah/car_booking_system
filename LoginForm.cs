using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarBookRequest
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient client = new HttpClient();

        public LoginForm()
        {
            InitializeComponent();
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

                    DashBoard br = new DashBoard();
                    br.Show();
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
    }
}
