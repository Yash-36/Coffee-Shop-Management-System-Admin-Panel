using System.ComponentModel.DataAnnotations;

namespace Admin_Panel__Nice_Admin_.Models
{
    public class OrderDetailModel
    {
        public int? OrderDetailID { get; set; }

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderID { get; set; } // Foreign Key to Order

        [Required(ErrorMessage = "Product ID is required")]
        public int ProductID { get; set; } // Foreign Key to Product


        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be grater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be grater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total Amount must be grater than 0")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Foreign Key to User
    }
}
