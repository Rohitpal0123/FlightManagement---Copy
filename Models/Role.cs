
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlightManagement.Models;

public partial class Role
{
    [BindNever]
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
} 
