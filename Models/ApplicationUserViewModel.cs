using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EeshaProperty.Areas.Identity.Data;
using EeshaProperty.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EeshaProperty.Models
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser User { get; set; }                
    }
}