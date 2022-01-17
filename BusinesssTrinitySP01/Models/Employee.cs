using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [DisplayName("Employee Code")]
        public string EmpCode { get; set; }

        [Display(Name = "First Name")]
        public string EmpName { get; set; }

        [Display(Name = "Last Name")]
        public string EmpLName { get; set; }

        [Display(Name = "RSAID Number")]
        [MinLength(13, ErrorMessage = "Invalid ID Number")]
        [StringLength(13, ErrorMessage = "Invalid ID Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a 13 Digit Identity Number")]
        public string EmpRSAID { get; set; }

        [Display(Name = "Date Of Birth")]
        public string EmpBirthDate
        {
            get
            {
                string prefix = "19";

                if (EmpRSAID.Substring(0, 1) == "0" ||
                     EmpRSAID.Substring(0, 1) == "1" ||
                     EmpRSAID.Substring(0, 1) == "2" ||
                     EmpRSAID.Substring(0, 1) == "3")
                {
                    prefix = "20";
                }
                else
                {
                    prefix = "19";
                }
                return EmpRSAID != null ? prefix + EmpRSAID.Substring(0, 2) + "/" + EmpRSAID.Substring(2, 2) + "/" + EmpRSAID.Substring(4, 2) : "";
            }
            set
            {

            }
        }

        [Display(Name = "Age")]
        public string EmpAge
        {
            get
            {
                DateTime dt = DateTime.ParseExact(EmpBirthDate, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime today = DateTime.Today;
                int age = today.Year - dt.Year;
                if (dt > today.AddYears(-age)) age--;
                return EmpRSAID != null ? age.ToString() : "";
            }
            set
            {

            }
        }
        [Display(Name = "Gender")]
        public string EmpGender
        {
            get
            {
                var gendeCode = (EmpRSAID.Substring(6, 4));
                var gender = int.Parse(gendeCode) < 5000 ? "Female" : "Male";
                return EmpRSAID != null ? gender : "";
            }
            set
            {

            }
        }

        [Display(Name = "Mobile Number")]
        public string EmpPhone { get; set; }

        [Display(Name = "Email Address")]
        public string EmpEmail { get; set; }

        [Display(Name = "Position")]
        public string EmpPosition { get; set; }

        [Display(Name = "Duties")]
        public string EmpDuty { get; set; }
    }
}