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

            dataGridView1.CellClick += dataGridView1_CellClick;
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

                // SELECT ONLY IMPORTANT FIELDS
                var displayList = allBookings.Select(b => new
                {
                    b.BOOK_ID,
                    FullName = b.FNAME + " " + b.LNAME,
                    b.PHONE_NUMBER,
                    b.CAR_NAME,
                    b.BOOK_PLACE,
                    b.BOOK_DATE,
                    b.DESTINATION,
                    b.RETURN_DATE,
                    b.DURATION,
                    b.PRICE
                }).ToList();

                dataGridView1.DataSource = displayList;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.RowHeadersVisible = false;

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
        }

        // CLICK ROW → SHOW DETAILS
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

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            DashBoard d = new DashBoard();
            d.Show();
            this.Hide();
        }

        private void btnRequests_Click(object sender, EventArgs e)
        {
            BookRequest d = new BookRequest();
            d.Show();
            this.Hide();
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            LoginForm l = new LoginForm();
            l.Show();
            this.Hide();
        }
    }
}