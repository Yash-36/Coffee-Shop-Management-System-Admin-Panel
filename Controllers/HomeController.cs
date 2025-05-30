using Admin_Panel__Nice_Admin_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Coffee_Shop_Management_System_Admin_Panel_.BAL;

namespace Admin_Panel__Nice_Admin_.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration configuration;

        public HomeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        //public IActionResult login()
        //{
        //    return View();
        //}

        //public IActionResult Register() {
        //    return View();
        //}

        #region login
        public IActionResult login(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                        HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        HttpContext.Session.SetString("Email", dr["Email"].ToString());
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid username or password.";               
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return View("login");
        }
        #endregion

        #region Register
        public IActionResult Register(UserModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Register";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("login", "Home");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }
            return View("Register");
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login", "Home");
        }
        #endregion

        [CheckAccess]
        public IActionResult Index()
        {
            List<String> s = new List<String>();
            s.Add("Yash");            

            ViewData["Data"] = s;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
