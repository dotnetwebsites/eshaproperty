using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EeshaProperty.Models
{
    public class Country : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Country Code")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Country Code")]
        public string CountryCode { get; set; }

        [Display(Name = "Country")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Country")]
        public string CountryName { get; set; }

        [Display(Name = "Short Name")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Short Name")]
        public string ShortName { get; set; }

        public ICollection<State> States { get; set; }
    }
}