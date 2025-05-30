using System.ComponentModel.DataAnnotations;

namespace Admin_Panel__Nice_Admin_.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }

        [Required(ErrorMessage = "Order Date is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerID { get; set; } // Foreign Key to Customer

        [Required(ErrorMessage = "Payment Mode is required")]
        public string? PaymentMode { get; set; } // Nullable by default

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total Amount must be grater than 0")]
        public decimal? TotalAmount { get; set; } // Nullable

        [Required(ErrorMessage = "Shipping Address is required")]
        public string? ShippingAddress { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public int UserID { get; set; } // Foreign Key to User
    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
    }
}
