using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CarBookRequest
{
    public partial class BookingDetailsControl : UserControl
    {
        HttpClient client = new HttpClient();
        int bookingId = 0;

        public BookingDetailsControl()
        {
            InitializeComponent();
        }

        public Action OnStatusChanged { get; internal set; }

        // SET DATA FROM GRID
        public void SetBooking(dynamic booking)
        {
            bookingId = booking.BOOK_ID;

            lblName.Text = booking.FNAME + " " + booking.LNAME;
            lblCar.Text = booking.CAR_NAME;
            lblPhone.Text = booking.PHONE_NUMBER;
            lblPlace.Text = booking.BOOK_PLACE;
            lblDate.Text = Convert.ToDateTime(booking.BOOK_DATE).ToShortDateString();
            lblReturn.Text = Convert.ToDateTime(booking.RETURN_DATE).ToShortDateString();
            lblDestination.Text = booking.DESTINATION;
            lblDuration.Text = booking.DURATION + " days";

            // ✅ FIXED PRICE FORMAT
            lblPrice.Text = "₱" + Convert.ToDecimal(booking.PRICE).ToString("N2");

            lblStatus.Text = booking.BOOK_STATUS;
        }

        // ✅ APPROVE
        private async void btnApprove_Click_1(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Approve booking?", "Confirm", MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var request = new HttpRequestMessage(
                        new HttpMethod("PATCH"),
                        $"http://localhost:3000/api/bookings/{bookingId}/status"
                    );

                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(new { status = "APPROVED" }),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Approved!");
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
        private async void btnReject_Click_1(object sender, EventArgs e)
        {
            string reason = txtMessage.Text;

            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Enter reason first!");
                return;
            }

            try
            {
                var request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    $"http://localhost:3000/api/bookings/{bookingId}/status"
                );

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        status = "REJECTED",
                        message = reason
                    }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Rejected!");
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