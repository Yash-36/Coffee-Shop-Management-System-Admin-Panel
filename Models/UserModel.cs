using System.ComponentModel.DataAnnotations;

namespace Admin_Panel__Nice_Admin_.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        //[RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must contain at least 8 characters")]
        public string Password { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile No must be of 10 digits")]
        [Required(ErrorMessage = "Mobile No is required")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "User IsActive is required")]

        public bool? IsActive { get; set; }
    }

    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
