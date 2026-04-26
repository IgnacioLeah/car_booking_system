using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Drawing;

namespace CarBookRequest
{
    public partial class Reports : Form
    {
        HttpClient client = new HttpClient();
        List<Booking> allBookings = new List<Booking>();

        //  COLORS
        Color defaultColor = Color.FromArgb(0, 192, 192); // sidebar color
        Color activeColor = Color.FromArgb(0, 160, 215);  // highlight color

        public Reports()
        {
            InitializeComponent();

            comboYear.SelectedIndexChanged += comboYear_SelectedIndexChanged;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        // 🔥 LOAD DATA FROM API
        private async void LoadReports()
        {
            try
            {
                string url = "http://localhost:3000/api/bookings";

                var response = await client.GetStringAsync(url);
                dynamic result = JsonConvert.DeserializeObject(response);

                var data = JsonConvert.DeserializeObject<List<Booking>>(result.data.ToString());

                List<Booking> filteredList = new List<Booking>();

                foreach (var b in data)
                {
                    if (b.BOOK_STATUS != null)
                    {
                        string status = b.BOOK_STATUS.ToUpper();

                        if (status == "APPROVED" || status == "REJECTED")
                        {
                            filteredList.Add(b);
                        }
                    }
                }

                allBookings = filteredList;

                DisplayData(allBookings);
                LoadYearFilter();
                LoadSummaryCards(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reports: " + ex.Message);
            }
        }

        // DISPLAY GRID
        private void DisplayData(List<Booking> list)
        {
            var displayList = list.Select(b => new
            {
                b.BOOK_ID,
                FullName = b.FNAME + " " + b.LNAME,
                b.EMAIL,
                b.CAR_NAME,
                b.BOOK_DATE,
                b.RETURN_DATE,
                b.PRICE,
                b.BOOK_STATUS
            }).ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = displayList;

            // HEADERS
            dataGridView1.Columns["BOOK_ID"].HeaderText = "ID";
            dataGridView1.Columns["FullName"].HeaderText = "Full Name";
            dataGridView1.Columns["EMAIL"].HeaderText = "Email";
            dataGridView1.Columns["CAR_NAME"].HeaderText = "Car";
            dataGridView1.Columns["BOOK_DATE"].HeaderText = "Booking Date";
            dataGridView1.Columns["RETURN_DATE"].HeaderText = "Return Date";
            dataGridView1.Columns["PRICE"].HeaderText = "Price";
            dataGridView1.Columns["BOOK_STATUS"].HeaderText = "Status";

            // FORMAT
            dataGridView1.Columns["BOOK_DATE"].DefaultCellStyle.Format = "yyyy-MM-dd";
            dataGridView1.Columns["RETURN_DATE"].DefaultCellStyle.Format = "yyyy-MM-dd";
            dataGridView1.Columns["PRICE"].DefaultCellStyle.Format = "₱#,##0.00";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
        }

        // SUMMARY CARDS (LIKE IMAGE)
        private void LoadSummaryCards()
        {
            int approvedCount = allBookings.Count(b => b.BOOK_STATUS == "APPROVED");
            int rejectedCount = allBookings.Count(b => b.BOOK_STATUS == "REJECTED");

            decimal totalRevenue = allBookings
                .Where(b => b.BOOK_STATUS == "APPROVED")
                .Sum(b => b.PRICE);

            // SET THESE LABELS IN DESIGNER
            lblApprovedCount.Text = approvedCount.ToString();
            lblRejectedCount.Text = rejectedCount.ToString();
            lblRevenue.Text = "₱" + totalRevenue.ToString("#,##0.00");
        }

        //  YEAR FILTER
        private void LoadYearFilter()
        {
            var years = allBookings
                .Select(b => b.BOOK_DATE.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            comboYear.Items.Clear();
            comboYear.Items.Add("All");

            foreach (var y in years)
            {
                comboYear.Items.Add(y.ToString());
            }

            comboYear.SelectedIndex = 0;
        }

        // FILTER BY YEAR
        private void comboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboYear.Text == "All")
            {
                DisplayData(allBookings);
                LoadSummaryCards();
                return;
            }

            int selectedYear = Convert.ToInt32(comboYear.Text);

            var filtered = allBookings
                .Where(b => b.BOOK_DATE.Year == selectedYear)
                .ToList();

            DisplayData(filtered);

            // UPDATE SUMMARY BASED ON FILTER
            int approvedCount = filtered.Count(b => b.BOOK_STATUS == "APPROVED");
            int rejectedCount = filtered.Count(b => b.BOOK_STATUS == "REJECTED");

            decimal totalRevenue = filtered
                .Where(b => b.BOOK_STATUS == "APPROVED")
                .Sum(b => b.PRICE);

            lblApprovedCount.Text = approvedCount.ToString();
            lblRejectedCount.Text = rejectedCount.ToString();
            lblRevenue.Text = "₱" + totalRevenue.ToString("#,##0.00");
        }

        // STATUS COLOR (LIKE IMAGE)
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "BOOK_STATUS")
            {
                string status = e.Value?.ToString();

                if (status == "APPROVED")
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (status == "REJECTED")
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

        // FORM LOAD
        private void Reports_Load(object sender, EventArgs e)
        {
            LoadReports();
            SetActiveButton(btnReport);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnDashboard);

            DashBoard br = new DashBoard();
            br.Show();
            this.Hide();
        }

        private void btnRequests_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnRequests);

            BookRequest br = new BookRequest();
            br.Show();
            this.Hide();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnReport);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm br = new LoginForm();
            br.Show();
            this.Hide();
        }


        // BUTTON HIGHLIGHT FUNCTION
        private void SetActiveButton(Button activeBtn)
        {
            // reset all buttons
            btnReport.BackColor = defaultColor;

            // highlight selected
            activeBtn.BackColor = activeColor;
        }
    }
}