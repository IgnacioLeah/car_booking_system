using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBookRequest
{
    public class Booking
    {
        public int BOOK_ID { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public string EMAIL { get; set; }
        public string CAR_NAME { get; set; }
        public DateTime BOOK_DATE { get; set; }
        public DateTime RETURN_DATE { get; set; }
        public decimal PRICE { get; set; }
        public string BOOK_STATUS { get; set; }
    }
}
