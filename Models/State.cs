using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EeshaProperty.Models
{
    public class State : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please choose Country")]
        public int CountryId { get; set; }

        [Display(Name = "State Code")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter State Code")]
        public string StateCode { get; set; }

        [Display(Name = "State")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter State")]
        public string StateName { get; set; }

        [Display(Name = "Short Name")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Short Name")]
        public string ShortName { get; set; }

        public Country Country { get; set; }
        public ICollection<City> Cities { get; set; }
    }

    public class StateView : State
    {
        [Display(Name = "Coutry")]
        public string CoutryName { get; set; }
    }
}