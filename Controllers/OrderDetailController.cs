using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Admin_Panel__Nice_Admin_.Models;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;
using Microsoft.Extensions.Configuration;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    [CheckAccess]
    public class OrderDetailController : Controller
    {
        private IConfiguration configuration;

        public OrderDetailController(IConfiguration _configuration)
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

        #region SelectAll
        public IActionResult Orderdetails()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_OrderDetail_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            if (reader.HasRows)
            {
                table.Load(reader);
            }
            return View(table);
        }
        #endregion

        #region Delete
        public IActionResult OrderDelete(int OrderDetailID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_OrderDetail_Delete";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;
                command.ExecuteNonQuery();
                if (command.ExecuteNonQuery() > 0)
                {
                    TempData["ErrorMessage"] = "Record Not Deleted";
                }
                else
                {
                    TempData["Message"] = "Record Deleted Successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Orderdetails");
        }
        #endregion

        #region PopulateDropDowns
        private void PopulateDropDowns()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();

            #region DropdownProductID
            SqlConnection connectionProduct = new SqlConnection(connectionString);
            SqlCommand commandProduct = new SqlCommand("PR_Product_DropDown", connectionProduct);
            commandProduct.CommandType = CommandType.StoredProcedure;

            try
            {
                connectionProduct.Open();
                SqlDataReader readerProduct = commandProduct.ExecuteReader();
                DataTable dataTableProduct = new DataTable();
                dataTableProduct.Load(readerProduct);

                foreach (DataRow row in dataTableProduct.Rows)
                {
                    ProductDropDownModel productDropDownModel = new ProductDropDownModel
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"].ToString()
                    };
                    productList.Add(productDropDownModel);
                }
            }
            finally
            {
                connectionProduct.Close();
            }
            #endregion

            #region DropdownOrderID
            SqlConnection connectionOrder = new SqlConnection(connectionString);
            SqlCommand commandOrder = new SqlCommand("PR_Order_DropDown", connectionOrder);
            commandOrder.CommandType = CommandType.StoredProcedure;

            try
            {
                connectionOrder.Open();
                SqlDataReader readerOrder = commandOrder.ExecuteReader();
                DataTable dataTableOrder = new DataTable();
                dataTableOrder.Load(readerOrder);

                foreach (DataRow row in dataTableOrder.Rows)
                {
                    OrderDropDownModel orderDropDownModel = new OrderDropDownModel
                    {
                        OrderID = Convert.ToInt32(row["OrderID"])
                    };
                    orderList.Add(orderDropDownModel);
                }
            }
            finally
            {
                connectionOrder.Close();
            }
            #endregion

            ViewBag.ProductList = productList;
            ViewBag.OrderList = orderList;
        }
        #endregion

        #region Add or Edit
        public IActionResult AddOrderDetail(int? orderDetailID)
        {
            OrderDetailModel orderDetailModel = new OrderDetailModel();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_OrderDetail_SelectByPK]";
            command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = (object)orderDetailID ?? DBNull.Value;

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows && orderDetailID != null)
                {
                    reader.Read();
                    orderDetailModel.OrderDetailID = Convert.ToInt32(reader["OrderDetailID"]);
                    orderDetailModel.OrderID = Convert.ToInt32(reader["OrderID"]);
                    orderDetailModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                    orderDetailModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    orderDetailModel.Amount = Convert.ToDecimal(reader["Amount"]);
                    orderDetailModel.TotalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                    orderDetailModel.UserID = Convert.ToInt32(reader["UserID"]);
                }
            }
            finally
            {
                UserDropDown();
                PopulateDropDowns();
                connection.Close();
            }

            return View(orderDetailModel);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(OrderDetailModel modelOrderDetail)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelOrderDetail.OrderDetailID == null)
            {
                command.CommandText = "[dbo].[PR_OrderDetail_Insert]";
            }
            else
            {
                command.CommandText = "[dbo].[PR_OrderDetail_Update]";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = modelOrderDetail.OrderDetailID;
            }

            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelOrderDetail.OrderID;
            command.Parameters.Add("@ProductID", SqlDbType.Int).Value = modelOrderDetail.ProductID;
            command.Parameters.Add("@Quantity", SqlDbType.Int).Value = modelOrderDetail.Quantity;
            command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = modelOrderDetail.Amount;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelOrderDetail.TotalAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelOrderDetail.UserID;

            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    TempData["Message"] = modelOrderDetail.OrderDetailID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
                }
            }
            finally
            {
                UserDropDown();
                PopulateDropDowns();
                connection.Close();
            }

            return RedirectToAction("Orderdetails");
        }
        #endregion

    }
}
