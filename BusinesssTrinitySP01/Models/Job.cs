using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Models
{
    public class Job
    {
        [Key]
        public int JobID { get; set; }

        [Required]
        [DisplayName("Job Name")]
        public string JobType { get; set; }

        [Required]
        [DisplayName("Job Code")]
        public string JobCode { get; set; }

        [Required]
        [DisplayName("Description")]
        public string JobDescription { get; set; }

        [Required]
        [DisplayName("Requirements")]
        public string JobRequirments { get; set; }

        [Required]
        [DisplayName("Closing Date")]
        [DataType(DataType.Date)]
        public DateTime ClosingDate { get; set; }

        [Required]
        [DisplayName("Post Date")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
    }

    public class Applicant
    {
        [Key]
        public int AppID { get; set; }

        [Required]
        [DisplayName("ID No.")]
        public string RSAID { get; set; }

        [Required]
        [DisplayName("Full Names")]
        public string FName { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string Lastname { get; set; }

        [Required]
        [DisplayName("Phone")]
        public string Phone { get; set; }

        [Required]
        [DisplayName("Email Address")]
        public string Email { get; set; }
    }


    public class JobApplications
    {
        [Key]
        public int JobAppID { get; set; }

        public int JobID { get; set; }

        public int AppID { get; set; }

        [Required]
        [DisplayName("Date Applied")]
        [DataType(DataType.Date)]
        public DateTime AppliedDate { get; set; }

        [Required]
        [DisplayName("Status")]
        public string Status { get; set; }

        [Required]
        [DisplayName("Resume")]
        public byte[] Resume { get; set; }

        public virtual Applicant Applicant { get; set; }
        public virtual Job Job { get; set; }

        [NotMapped]
        public ICollection<Job> jobs { get; set; }

        [NotMapped]
        [DisplayName("RSA ID")]
        public string ID { get; set; }
    }
}