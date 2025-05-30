using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Admin_Panel__Nice_Admin_.Models;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    [CheckAccess]
    public class CustomerController : Controller
    {
        private IConfiguration configuration;

        public CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region UserDropDown
        public void UserDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_User_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<UserDropDownModel> UserList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                UserDropDownModel UserDropDownModel = new UserDropDownModel();
                UserDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                UserDropDownModel.UserName = data["UserName"].ToString();
                UserList.Add(UserDropDownModel);
            }
            ViewBag.UserList = UserList;
        }
        #endregion

        #region AddEdit
        public IActionResult AddCustomer(int? customerID)
        {
            UserDropDown();
            CustomerModel modelCustomer = new CustomerModel();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_Customer_SelectByPK]";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = (object)customerID ?? DBNull.Value;

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows && customerID != null)
                {
                    reader.Read();
                    modelCustomer.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                    modelCustomer.CustomerName = reader["CustomerName"].ToString();
                    modelCustomer.HomeAddress = reader["HomeAddress"].ToString();
                    modelCustomer.Email = reader["Email"].ToString();
                    modelCustomer.MobileNo = reader["MobileNo"].ToString();
                    modelCustomer.GST_NO = reader["GST_NO"].ToString();
                    modelCustomer.CityName = reader["CityName"].ToString();
                    modelCustomer.PinCode = reader["PinCode"].ToString();
                    modelCustomer.NetAmount = Convert.ToDecimal(reader["NetAmount"]);
                    modelCustomer.UserID = Convert.ToInt32(reader["UserID"]);
                }
            }
            finally
            {
                connection.Close();
            }

            return View(modelCustomer);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(CustomerModel modelCustomer)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelCustomer.CustomerID == null || modelCustomer.CustomerID == 0)
            {
                command.CommandText = "[dbo].[PR_Customer_Insert]";
            }
            else
            {
                command.CommandText = "[dbo].[PR_Customer_Update]";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = modelCustomer.CustomerID;
            }

            command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = modelCustomer.CustomerName;
            command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = modelCustomer.HomeAddress;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelCustomer.Email;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelCustomer.MobileNo;
            command.Parameters.Add("@GST_NO", SqlDbType.VarChar).Value = modelCustomer.GST_NO;
            command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = modelCustomer.CityName;
            command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = modelCustomer.PinCode;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = modelCustomer.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelCustomer.UserID;

            try
            {
                connection.Open();
                if (command.ExecuteNonQuery() > 0)
                {
                    TempData["Message"] = modelCustomer.CustomerID == null || modelCustomer.CustomerID == 0 ? "Record Inserted Successfully" : "Record Updated Successfully";
                }
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Customerdetail");
        }
        #endregion

        #region Delete
        public IActionResult CustomerDelete(int CustomerID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[PR_Customer_Delete]";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
                command.ExecuteNonQuery();
                if (command.ExecuteNonQuery() > 0)
                {
                    TempData["ErrorMessage"] = "Record Not Deleted";
                }
                else
                {
                    TempData["Message"] = "Record Deleted Successfully";
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Cannot delete this Order because it is associated with a user. Please delete the associated user first.";
            }
            return RedirectToAction("Customerdetail");
        }
        #endregion

        #region SelectAll
        public IActionResult Customerdetail()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            if (reader.HasRows)
            {
                table.Load(reader);
            }
            return View(table);
        }
        #endregion
    }
}
