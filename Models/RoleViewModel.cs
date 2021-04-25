using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EeshaProperty.Areas.Identity.Data;

namespace EeshaProperty.Models
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }
    }

    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        [Display(Name = "Role Id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }

    public class UserRoleViewModel
    {
        [Display(Name = "User Id")]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}