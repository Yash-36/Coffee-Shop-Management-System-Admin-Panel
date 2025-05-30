using System.ComponentModel.DataAnnotations;

namespace Admin_Panel__Nice_Admin_.Models
{
    public class CustomerModel
    {
        public int? CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Home Address is required")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress] 
        public string Email { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile No must be of 10 digits")]
        [Required(ErrorMessage = "Mobile No is required")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "GST NO is required")]
        public string GST_NO { get; set; }

        [Required(ErrorMessage = "City Name is required")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "PinCode is required")]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Net Amount is required")]
        public decimal NetAmount { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Foreign Key to User
    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
