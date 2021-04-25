using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EeshaProperty.Areas.Identity.Data;
using EeshaProperty.Utilities;
using Microsoft.AspNetCore.Http;

namespace EeshaProperty.Models
{
    public class MyDocument : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select employee code")]
        [Column(TypeName = "nvarchar(20)")]
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [Required(ErrorMessage = "Please select document")]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Document")]
        public string DocumentName { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string URL { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please choose document file")]
        [Display(Name = "Document")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", "jpeg", ".jpg", ".png" })]
        public IFormFile DocumentFile { get; set; }
    }

    public class UserDocumentViewModel
    {
        public ApplicationUser Users { get; set; }
        //public List<MyDocument> MyDocuments { get; set; }
        public string PanUrl { get; set; }
        public string AadhaarUrl { get; set; }
        public string DegreeUrl { get; set; }
        public bool IsPan { get; set; }
        public bool IsAadhaar { get; set; }
        public bool IsDegree { get; set; }
    }

}