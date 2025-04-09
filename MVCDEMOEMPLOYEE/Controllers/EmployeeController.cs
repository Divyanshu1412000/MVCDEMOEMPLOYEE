using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDEMOEMPLOYEE.Models;
using System.IO;

namespace MVCDEMOEMPLOYEE.Controllers
{
    public class EmployeeController : Controller
    {
        SqlDataReader dr;
        private readonly string conncetionString = ConfigurationManager.ConnectionStrings["mydb"].ToString();
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult UploadFile()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string fileExtension = Path.GetExtension(file.FileName);
                        string timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");

                        string newFileName = $"{fileName}_{timeStamp}{fileExtension}";


                        string filePath = Path.Combine(Server.MapPath("~/ProfilePicture/"), newFileName);
                        file.SaveAs(filePath);

                        return Json(new { success = true, filePath = "/ProfilePicture/" + newFileName });
                    }
                }
                return Json(new { success = false, message = "No file uploaded" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public ActionResult Insert(EmployeeModel emp)
        {
            try
            {
                if (emp == null)
                {
                    return Json(new { success = false, message = "Invalid employee data" });
                }
                using (SqlConnection con = new SqlConnection(conncetionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_CrudOp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@empId", emp.empId);
                    cmd.Parameters.AddWithValue("@empCode", emp.empCode);
                    cmd.Parameters.AddWithValue("@empName", emp.empName);
                    cmd.Parameters.AddWithValue("@department", emp.Dept);
                    cmd.Parameters.AddWithValue("@designation", emp.SelectedDesignation);
                    cmd.Parameters.AddWithValue("@gender", emp.gender);
                    cmd.Parameters.AddWithValue("@CrudOp", "insert");
                    cmd.Parameters.AddWithValue("@imagePath", emp.ImagePath);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Employee Inserted Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetDepartments()
        {
            try {
                List<Department> departmentList = new List<Department>();
                SqlConnection con = new SqlConnection(conncetionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_getDataOp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "getDepartment");
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    departmentList.Add(new Department
                    {
                        id = Convert.ToInt32(dr["deptId"]),
                        deptName = dr["deptName"].ToString()
                    });
                }

                return Json(departmentList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDesignations()
        {
            try
            {
                List<Designation> designationList = new List<Designation>();
                SqlConnection con = new SqlConnection(conncetionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[sp_getDataOp]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "getDesignation");
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    designationList.Add(new Designation
                    {
                        id = Convert.ToInt32(dr["id"]),
                        designationName = dr["designationName"].ToString()
                    });
                }
                return Json(designationList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult getEmployees()
        {
            try
            {
                List<EmployeeModel> list = new List<EmployeeModel>();
                SqlConnection con = new SqlConnection(conncetionString);
                SqlCommand cmd = new SqlCommand("sp_getDataOp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@Action", "getEmployee");
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new EmployeeModel()
                    {
                        empId = Convert.ToInt32(dr["empId"]),
                        empCode = dr["empCode"].ToString(),
                        empName = dr["empName"].ToString(),
                        Dept = dr["department_name"].ToString(),
                        gender = Convert.ToInt32(dr["gender"]),
                        Designation = dr["designationName"].ToString(),
                        isActiveEmp = Convert.ToInt32(dr["isActiveEmp"])

                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getEmpId(int id)
        {
            try
            {
                EmployeeModel emp = new EmployeeModel();
                string connString = ConfigurationManager.ConnectionStrings["mydb"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    //string query = "SELECT * FROM empDetails where empId=@empId1";
                    SqlCommand cmd = new SqlCommand("sp_getDataOp", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@empId", id);
                    cmd.Parameters.AddWithValue("@Action", "edit");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emp = new EmployeeModel
                            {
                                empId = Convert.ToInt32(reader["empId"]),
                                empCode = reader["empCode"].ToString(),
                                empName = reader["empName"].ToString(),
                                Dept = reader["department"].ToString(),
                                designationId = Convert.ToInt32(reader["designation"]),
                                gender = Convert.ToInt32(reader["gender"]),
                                ImagePath = reader["imagePath"].ToString()
                            };
                        }
                    }

                }
                return Json(emp, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult delEmpId(int id)
        {
            try
            {
                EmployeeModel emp = new EmployeeModel();
                string connString = ConfigurationManager.ConnectionStrings["mydb"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    //string query = "SELECT * FROM empDetails where empId=@empId1";
                    SqlCommand cmd = new SqlCommand("sp_getDataOp", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@empId", id);
                    cmd.Parameters.AddWithValue("@Action", "delete");
                    cmd.ExecuteNonQuery();

                }
                return Json(emp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult MakeActive(int id)
        {
            try
            {
                EmployeeModel emp = new EmployeeModel();
                string connString = ConfigurationManager.ConnectionStrings["mydb"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_getDataOp", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@empId", id);
                    cmd.Parameters.AddWithValue("@Action", "MakeActive");
                    cmd.ExecuteNonQuery();

                }
                return Json(emp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult ViewEmployee(int id)
        {
            try
            {
                EmployeeModel emp = new EmployeeModel();
                string connString = ConfigurationManager.ConnectionStrings["mydb"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_getDataOp", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@empId", id);
                    cmd.Parameters.AddWithValue("@Action", "view");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emp = new EmployeeModel
                            {
                                empId = Convert.ToInt32(reader["empId"]),
                                empCode = reader["empCode"].ToString(),
                                empName = reader["empName"].ToString(),
                                Dept = reader["department_name"].ToString(),
                                gender = Convert.ToInt32(reader["gender"]),
                                Designation = reader["designationName"].ToString(),
                                //for image
                                ImagePath = reader["imagePath"].ToString()

                            };
                        }
                    }

                }
                return Json(emp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}