using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Admin_Panel__Nice_Admin_.Models;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    [CheckAccess]
    public class BillsController : Controller
    {
        private IConfiguration configuration;

        public BillsController(IConfiguration _configuration)
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
        public IActionResult AddBill(int? billID)
        {
            UserDropDown();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_DropDown";

            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();

            try
            {
                connection1.Open();
                SqlDataReader reader1 = command1.ExecuteReader();
                DataTable dataTable1 = new DataTable();
                dataTable1.Load(reader1);

                foreach (DataRow dataRow in dataTable1.Rows)
                {
                    OrderDropDownModel orderDropDownModel = new OrderDropDownModel
                    {
                        OrderID = Convert.ToInt32(dataRow["OrderID"])
                    };
                    orderList.Add(orderDropDownModel);
                }
                ViewBag.orderList = orderList;
            }
            finally
            {
                connection1.Close();
            }

            BillsModel modelBills = new BillsModel();
            SqlConnection connection2 = new SqlConnection(connectionString);
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = CommandType.StoredProcedure;
            command2.CommandText = "[dbo].[PR_Bills_SelectByPK]";
            command2.Parameters.Add("@BillID", SqlDbType.Int).Value = (object)billID ?? DBNull.Value;
            try
            {
                connection2.Open();
                SqlDataReader reader2 = command2.ExecuteReader();

                if (reader2.HasRows && billID != null)
                {
                    reader2.Read();
                    modelBills.BillID = Convert.ToInt32(reader2["BillID"]);
                    modelBills.BillNumber = reader2["BillNumber"].ToString();
                    modelBills.BillDate = Convert.ToDateTime(reader2["BillDate"]);
                    modelBills.OrderID = Convert.ToInt32(reader2["OrderID"]);
                    modelBills.TotalAmount = Convert.ToDecimal(reader2["TotalAmount"]);
                    modelBills.Discount = Convert.ToDecimal(reader2["Discount"]);
                    modelBills.NetAmount = Convert.ToDecimal(reader2["NetAmount"]);
                    modelBills.UserID = Convert.ToInt32(reader2["UserID"]);
                }
            }
            finally
            {
                connection2.Close();
            }

            return View(modelBills);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(BillsModel modelBills)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelBills.BillID == null)
            {
                command.CommandText = "[dbo].[PR_Bills_Insert]";
            }
            else
            {
                command.CommandText = "[dbo].[PR_Bills_Update]";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = modelBills.BillID;
            }

            command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = modelBills.BillNumber;
            command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = modelBills.BillDate;
            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = modelBills.OrderID;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = modelBills.TotalAmount;
            command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = modelBills.Discount;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = modelBills.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelBills.UserID;

            try
            {
                connection.Open();
                if (command.ExecuteNonQuery() > 0)
                {
                    TempData["Message"] = modelBills.BillID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
                }
            }
            finally
            {
                connection.Close();
            }

            return RedirectToAction("Billsdetail");
        }
        #endregion

        #region Delete
        public IActionResult BillsDelete(int BillID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[PR_Bills_Delete]";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
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
            return RedirectToAction("Billsdetail");
        }
        #endregion

        #region SelectAll
        public IActionResult Billsdetail()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bills_SelectAll";
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
