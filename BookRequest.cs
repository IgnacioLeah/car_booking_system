using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CarBookRequest
{
    public partial class BookRequest : Form
    {
        HttpClient client = new HttpClient();
        List<dynamic> allBookings = new List<dynamic>();

        public BookRequest()
        {
            InitializeComponent();

            // 🔥 VERY IMPORTANT (ENSURE BUTTON CLICK WORKS)
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting_1;
        }

        // 🔥 LOAD DATA FROM API
        private async void LoadBookings()
        {
            try
            {
                string url = "http://localhost:3000/api/bookings";

                var response = await client.GetStringAsync(url);

                dynamic result = JsonConvert.DeserializeObject(response);

                allBookings = result.data.ToObject<List<dynamic>>();

                dataGridView1.DataSource = null;
                dataGridView1.Columns.Clear(); // reset columns

                dataGridView1.DataSource = allBookings;

                // ✅ HEADERS
                dataGridView1.Columns["BOOK_ID"].HeaderText = "Booking ID";
                dataGridView1.Columns["CAR_ID"].HeaderText = "Car ID";
                dataGridView1.Columns["EMAIL"].HeaderText = "Email";
                dataGridView1.Columns["BOOK_PLACE"].HeaderText = "Pickup Place";
                dataGridView1.Columns["BOOK_DATE"].HeaderText = "Booking Date";
                dataGridView1.Columns["DURATION"].HeaderText = "Days";
                dataGridView1.Columns["PHONE_NUMBER"].HeaderText = "Phone";
                dataGridView1.Columns["DESTINATION"].HeaderText = "Destination";
                dataGridView1.Columns["RETURN_DATE"].HeaderText = "Return Date";
                dataGridView1.Columns["PRICE"].HeaderText = "Price";
                dataGridView1.Columns["BOOK_STATUS"].HeaderText = "Status";

                // ✅ FORMAT
                dataGridView1.Columns["BOOK_DATE"].DefaultCellStyle.Format = "MMM dd, yyyy";
                dataGridView1.Columns["RETURN_DATE"].DefaultCellStyle.Format = "MMM dd, yyyy";
                dataGridView1.Columns["PRICE"].DefaultCellStyle.Format = "₱#,##0.00";

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.RowHeadersVisible = false;

                AddButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings: " + ex.Message);
            }
        }

        // 🔥 FORM LOAD
        private void BookRequest_Load(object sender, EventArgs e)
        {
            LoadBookings();
        }

        // 🔍 SEARCH
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower();

            var filtered = allBookings
                .Where(b => b.EMAIL != null &&
                            b.EMAIL.ToString().ToLower().Contains(keyword))
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtered;
        }

        // 🔽 FILTER
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = comboBox1.Text;

            if (selected == "All")
            {
                dataGridView1.DataSource = allBookings;
                return;
            }

            var filtered = allBookings
                .Where(b => b.BOOK_STATUS != null &&
                            b.BOOK_STATUS.ToString().ToLower().Contains(selected.ToLower()))
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filtered;
        }

        // ➕ ADD BUTTONS
        private void AddButtons()
        {
            if (!dataGridView1.Columns.Contains("Approve"))
            {
                DataGridViewButtonColumn approve = new DataGridViewButtonColumn();
                approve.Name = "Approve";
                approve.Text = "✔ Approve";
                approve.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(approve);
            }

            if (!dataGridView1.Columns.Contains("Reject"))
            {
                DataGridViewButtonColumn reject = new DataGridViewButtonColumn();
                reject.Name = "Reject";
                reject.Text = "✖ Reject";
                reject.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(reject);
            }
        }

        // 🔥 APPROVE / REJECT LOGIC (FIXED)
        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["BOOK_ID"].Value);

            // ✅ APPROVE
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Approve")
            {
                var confirm = MessageBox.Show("Approve this booking?", "Confirm", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        var content = new StringContent(
                            JsonConvert.SerializeObject(new { status = "Approved" }),
                            Encoding.UTF8,
                            "application/json"
                        );

                        var response = await client.PostAsync(
                            $"http://localhost:3000/api/bookings/{id}/approve",
                            content
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Approved!");
                            LoadBookings();
                        }
                        else
                        {
                            MessageBox.Show("API Error: " + response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

            // ❌ REJECT
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Reject")
            {
                string reason = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter reason for rejection:",
                    "Reject Booking",
                    "");

                if (!string.IsNullOrEmpty(reason))
                {
                    try
                    {
                        var content = new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                status = "Rejected",
                                reason = reason
                            }),
                            Encoding.UTF8,
                            "application/json"
                        );

                        var response = await client.PostAsync(
                            $"http://localhost:3000/api/bookings/{id}/reject",
                            content
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Rejected!");
                            LoadBookings();
                        }
                        else
                        {
                            MessageBox.Show("API Error: " + response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        // 🎨 STATUS COLOR
        private void dataGridView1_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "BOOK_STATUS")
            {
                string status = e.Value?.ToString();

                if (status == "Approved")
                    e.CellStyle.BackColor = Color.LightGreen;

                else if (status == "Rejected")
                    e.CellStyle.BackColor = Color.LightCoral;

                else
                    e.CellStyle.BackColor = Color.LightYellow;
            }
        }

        // 🔁 NAVIGATION
        private void btnRequests_Click(object sender, EventArgs e)
        {
            DashBoard br = new DashBoard();
            br.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm br = new LoginForm();
            br.Show();
            this.Hide();
        }
    }
}