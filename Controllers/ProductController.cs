using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Admin_Panel__Nice_Admin_.Models;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    [CheckAccess]
    public class ProductController : Controller
    {
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
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

        #region Add or Edit
        public IActionResult AddProduct(int? ProductID)
        {
            ProductModel modelProduct = new ProductModel();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_Product_SelectByPK]";
            command.Parameters.Add("@ProductID", SqlDbType.Int).Value = (object)ProductID ?? DBNull.Value;

            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows && ProductID != null)
            {
                reader.Read();
                modelProduct.ProductID = Convert.ToInt32(reader["ProductID"]);
                modelProduct.ProductName = reader["ProductName"].ToString();
                modelProduct.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                modelProduct.ProductCode = reader["ProductCode"].ToString();
                modelProduct.Description = reader["Description"].ToString();
                modelProduct.UserID = Convert.ToInt32(reader["UserID"]);
            }

            UserDropDown();
            connection.Close();


            return View(modelProduct);
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(ProductModel modelProduct)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelProduct.ProductID == null)
            {
                command.CommandText = "[dbo].[PR_Product_Insert]";
            }
            else
            {
                command.CommandText = "[dbo].[PR_Product_Update]";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = modelProduct.ProductID;
            }

            command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = modelProduct.ProductName;
            command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = modelProduct.ProductPrice;
            command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = modelProduct.ProductCode;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = modelProduct.Description;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelProduct.UserID;

            if (command.ExecuteNonQuery() > 0)
            {
                TempData["Message"] = modelProduct.ProductID == null ? "Record Inserted Successfully" : "Record Updated Successfully";
            }

            UserDropDown();

            connection.Close();

            return RedirectToAction("Productdetails");
        }
        #endregion

        #region Delete
        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Product_Delete";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
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
            return RedirectToAction("Productdetails");
        }
        #endregion

        #region SelectAll
        public IActionResult Productdetails()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectAll";
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
