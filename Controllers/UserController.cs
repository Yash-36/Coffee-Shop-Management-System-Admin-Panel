using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Admin_Panel__Nice_Admin_.Models;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    [CheckAccess]
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult userprofile()
        {
            return View();
        }

        [CheckAccess]
        #region Add or Edit
        public IActionResult AddUser(int? UserID)
        {
            UserModel modelUser = new UserModel();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_User_SelectByPK]";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = (object)UserID ?? DBNull.Value;

            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows && UserID != null)
            {
                reader.Read();
                modelUser.UserID = Convert.ToInt32(reader["UserID"]);
                modelUser.UserName = reader["UserName"].ToString();
                modelUser.Email = reader["Email"].ToString();
                modelUser.Password = reader["Password"].ToString();
                modelUser.MobileNo = reader["MobileNo"].ToString();
                modelUser.Address = reader["Address"].ToString();
                modelUser.IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]); // Corrected here
            }
            connection.Close();

            return View(modelUser);
        }
        #endregion

        [CheckAccess]
        #region Save
        [HttpPost]
        public IActionResult Save(UserModel modelUser)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (modelUser.UserID == null || modelUser.UserID == 0)
            {
                command.CommandText = "[dbo].[PR_User_Insert]";
            }
            else
            {
                command.CommandText = "[dbo].[PR_User_Update]";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = modelUser.UserID;
            }

            command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = modelUser.UserName;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = modelUser.Email;
            command.Parameters.Add("@Password", SqlDbType.VarChar).Value = modelUser.Password;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = modelUser.MobileNo;
            command.Parameters.Add("@Address", SqlDbType.VarChar).Value = modelUser.Address;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = modelUser.IsActive;

            if (command.ExecuteNonQuery() > 0)
            {
                TempData["Message"] = modelUser.UserID == null || modelUser.UserID == 0 ? "Record Inserted Successfully" : "Record Updated Successfully";
            }
            connection.Close();

            return RedirectToAction("usersdetails");
        }
        #endregion

        [CheckAccess]
        #region SelectAll
        public IActionResult usersdetails()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            if (reader.HasRows)
            {
                table.Load(reader);
            }    
            return View(table);
        }
        #endregion

        [CheckAccess]
        #region Delete
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_Delete";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
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
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                Console.WriteLine(e.ToString());
            }
            return RedirectToAction("usersdetails");
        }
        #endregion
    }
}
