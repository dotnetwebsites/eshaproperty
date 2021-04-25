using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EeshaProperty.Utilities;
using Microsoft.AspNetCore.Http;

namespace EeshaProperty.Models
{
    public class Enquiry : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Username { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string EmpCode { get; set; }

        //[Required(ErrorMessage = "Required first name")]
        [Display(Name = "First Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Required last name")]
        [Display(Name = "Last Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Required email address")]
        [Display(Name = "Email Address")]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required(ErrorMessage = "Required mobile no")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
        [Display(Name = "Mobile No")]
        public string PhoneNumber { get; set; }

        //[Required(ErrorMessage = "Required Age")]
        // [Display(Name = "Age")]
        // [Range(10, 150, ErrorMessage = "Age only in between 10 to 150 max")]
        // public int Age { get; set; }

        //[Required(ErrorMessage = "Required Father name")]
        // [Column(TypeName = "nvarchar(200)")]
        // [Display(Name = "Father Name")]
        // public string FatherName { get; set; }

        // [Display(Name = "Spouse Name")]
        // [Column(TypeName = "nvarchar(200)")]
        // public string SpouseName { get; set; }

        [Display(Name = "Occupation")]
        [Column(TypeName = "nvarchar(200)")]
        public string Occupation { get; set; }

        // [Display(Name = "Location")]
        // [Column(TypeName = "nvarchar(400)")]
        // public string Location { get; set; }

        // [Display(Name = "AddressLine1")]
        // [Column(TypeName = "nvarchar(1000)")]
        // public string AddressLine1 { get; set; }

        // [Display(Name = "AddressLine2")]
        // [Column(TypeName = "nvarchar(1000)")]
        // public string AddressLine2 { get; set; }

        [Display(Name = "City")]
        //[Column(TypeName = "nvarchar(100)")]
        public int? CityId { get; set; }

        [Display(Name = "State")]
        //[Column(TypeName = "nvarchar(100)")]
        public int? StateId { get; set; }

        // [Display(Name = "Postal/Code")]
        // [Range(100000, 999999, ErrorMessage = "Postal code should be in 6 digit number.")]
        // public int PostalCode { get; set; }

        // [Display(Name = "Landmark")]
        // [Column(TypeName = "nvarchar(300)")]
        // public string Landmark { get; set; }

        [Display(Name = "Alternate No / Whatsapp No")]
        [Column(TypeName = "nvarchar(20)")]
        //[Required(ErrorMessage = "Required Alternate No / Whatsapp No")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "No must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Alternate No / Whatsapp No must be numeric")]
        public string AlternateNo { get; set; }

        // [Column(TypeName = "nvarchar(300)")]
        // public string HouseType { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public class EnquiryViewModel
    {
        // [Required(ErrorMessage = "Please select month")]
        // public string Month { get; set; }

        // [Required(ErrorMessage = "Please select year")]
        // [Range(2019, 2040, ErrorMessage = "Please select year")]
        // public int Year { get; set; }

        [Required(ErrorMessage = "Please choose excel file")]
        [Display(Name = "Excel File")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".xlsx", ".xls" })]
        public IFormFile ExcelFile { get; set; }

    }
}