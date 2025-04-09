using System.Collections.Generic;
using System.Web;

namespace MVCDEMOEMPLOYEE.Models
{
    public class EmployeeModel
    {
        public int empId { get; set; }
        public string empCode { get; set; }
        public string empName { get; set; }
        public int gender { get; set; }

        // To hold selected designation
       public int SelectedDesignation { get; set; }

        public string Dept { get; set; }

        public string Designation { get; set; } 

        public int designationId { get; set; }

        public int isActiveEmp { get; set; }

        public string ImagePath { get; set; } // Store file path


    }

    public class Department
    {
        public int id { get; set; }
        public string deptName { get; set; }
    }

    public class Designation
    {
        public int id { get; set; }
        public string designationName { get; set; }
    }
}