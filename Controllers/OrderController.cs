using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Admin_Panel__Nice_Admin_.Models;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    [CheckAccess]
    public class OrderController : Controller
    {
        private IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
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

        #region AddOrder
        public IActionResult AddOrder(int? OrderID = null)
        {
            OrderModel modelOrder = new OrderModel();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            #region Dropdown
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel
                {
                    CustomerID = Convert.ToInt32(dataRow["CustomerID"]),
                    CustomerName = dataRow["CustomerName"].ToString()
                };
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;
            connection1.Close();
            #endregion

            UserDropDown();

            #region AddEdit
            if (OrderID != null)
            {
                SqlConnection connection2 = new SqlConnection(connectionString);
                connection2.Open();
                SqlCommand command2 = connection2.CreateCommand();
                command2.CommandType = CommandType.StoredProcedure;
                command2.CommandText = "[dbo].[PR_Order_SelectByPK]";
                command2.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;

                SqlDataReader reader2 = command2.ExecuteReader();
                if (reader2.HasRows)
                {
                    reader2.Read();
                    modelOrder.OrderID = Convert.ToInt32(reader2["OrderID"]);
                    modelOrder.OrderDate = Convert.ToDateTime(reader2["OrderDate"]);
                    modelOrder.CustomerID = Convert.ToInt32(reader2["CustomerID"]);
                    modelOrder.PaymentMode = reader2["PaymentMode"].ToString();
                    modelOrder.TotalAmount = Convert.ToDecimal(reader2["TotalAmount"]);
                    modelOrder.ShippingAddress = reader2["ShippingAddress"].ToString();
                    modelOrder.UserID = Convert.ToInt32(reader2["UserID"]);
                }
                connection2.Close();
            }
            return View(modelOrder);
            #endregion
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(OrderModel modelOrder)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelOrder.OrderID == null)
            {
                command.CommandText = "[dbo].[PR_Order_Insert]";
            }
            else
            {
                command.CommandText = "[dbo].[PR_Order_Update]";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelOrder.OrderID;
            }

            command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = modelOrder.OrderDate;
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = modelOrder.CustomerID;
            command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = modelOrder.PaymentMode;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelOrder.TotalAmount;
            command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = modelOrder.ShippingAddress;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelOrder.UserID;

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                TempData["Message"] = modelOrder.OrderID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
            }

            #region Dropdown
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel
                {
                    CustomerID = Convert.ToInt32(dataRow["CustomerID"]),
                    CustomerName = dataRow["CustomerName"].ToString()
                };
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;
            connection1.Close();
            #endregion

            UserDropDown();

            connection.Close();

            return RedirectToAction("Orderdetail");
        }
        #endregion

        #region Delete
        public IActionResult OrderDelete(int OrderID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[PR_Order_Delete]";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
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
            return RedirectToAction("Orderdetail");
        }
        #endregion

        #region SelectALL
        public IActionResult Orderdetail()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectAll";
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
