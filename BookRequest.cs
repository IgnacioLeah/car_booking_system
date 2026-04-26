using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CarBookRequest
{
    public partial class BookRequest : Form
    {
        HttpClient client = new HttpClient();
        List<dynamic> allBookings = new List<dynamic>();

        Color defaultColor = Color.FromArgb(0, 192, 192);
        Color activeColor = Color.FromArgb(0, 160, 215);

        public BookRequest()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;

            // 🔥 CONNECT EVENT
            bookingDetailsControl1.OnStatusChanged += RefreshBookings;
        }

        // 🔥 UPDATED REFRESH FUNCTION (THIS IS THE FIX)
        private void RefreshBookings()
        {
            // ❌ REMOVE APPROVED + REJECTED FROM LIST
            allBookings = allBookings
                .Where(b =>
                    b.BOOK_STATUS != null &&
                    b.BOOK_STATUS.ToString().ToUpper() != "APPROVED" &&
                    b.BOOK_STATUS.ToString().ToUpper() != "REJECTED"
                )
                .ToList();

            // 🔄 REBIND GRID
            var displayList = allBookings.Select(b => new
            {
                b.BOOK_ID,
                FullName = b.FNAME + " " + b.LNAME,
                b.EMAIL,
                b.CAR_NAME,
                b.BOOK_STATUS
            }).ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = displayList;

            dataGridView1.Columns["BOOK_ID"].Visible = false;
            dataGridView1.Columns["FullName"].HeaderText = "Full Name";
            dataGridView1.Columns["EMAIL"].HeaderText = "Email";
            dataGridView1.Columns["CAR_NAME"].HeaderText = "Car";
            dataGridView1.Columns["BOOK_STATUS"].HeaderText = "Status";
        }

        // LOAD BOOKINGS
        private async void LoadBookings()
        {
            try
            {
                string url = "http://localhost:3000/api/bookings";

                var response = await client.GetStringAsync(url);
                dynamic result = JsonConvert.DeserializeObject(response);

                allBookings = result.data.ToObject<List<dynamic>>();

                // 🔥 FILTER (ONLY PENDING / NOT PROCESSED)
                allBookings = allBookings
                    .Where(b =>
                        b.BOOK_STATUS != null &&
                        b.BOOK_STATUS.ToString().ToUpper() != "APPROVED" &&
                        b.BOOK_STATUS.ToString().ToUpper() != "REJECTED"
                    )
                    .ToList();

                var displayList = allBookings.Select(b => new
                {
                    b.BOOK_ID,
                    FullName = b.FNAME + " " + b.LNAME,
                    b.EMAIL,
                    b.CAR_NAME,
                    b.BOOK_STATUS
                }).ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = displayList;

                dataGridView1.Columns["BOOK_ID"].Visible = false;
                dataGridView1.Columns["FullName"].HeaderText = "Full Name";
                dataGridView1.Columns["EMAIL"].HeaderText = "Email";
                dataGridView1.Columns["CAR_NAME"].HeaderText = "Car";
                dataGridView1.Columns["BOOK_STATUS"].HeaderText = "Status";

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BookRequest_Load(object sender, EventArgs e)
        {
            LoadBookings();
            bookingDetailsControl1.Visible = false;

            SetActiveButton(btnRequests);
        }

        // ACTIVE BUTTON
        private void SetActiveButton(Button activeBtn)
        {
            btnRequests.BackColor = defaultColor;

            activeBtn.BackColor = activeColor;
        }

        // CLICK ROW
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["BOOK_ID"].Value);

            var booking = allBookings.FirstOrDefault(b => b.BOOK_ID == id);

            if (booking != null)
            {
                bookingDetailsControl1.Visible = true;
                bookingDetailsControl1.SetBooking(booking);
            }
        }

        // NAVIGATION
        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            SetActiveButton(btnDashboard);

            DashBoard d = new DashBoard();
            d.Show();
            this.Hide();
        }

        private void btnRequests_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnRequests);
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            LoginForm l = new LoginForm();
            l.Show();
            this.Hide();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnReport);

            Reports d = new Reports();
            d.Show();
            this.Hide();
        }

        // STATUS COLOR
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "BOOK_STATUS")
            {
                string status = e.Value?.ToString();

                if (status == "APPROVED")
                    e.CellStyle.BackColor = Color.LightGreen;

                else if (status == "REJECTED")
                    e.CellStyle.BackColor = Color.LightCoral;

                else
                    e.CellStyle.BackColor = Color.LightYellow;
            }
        }
    }
}