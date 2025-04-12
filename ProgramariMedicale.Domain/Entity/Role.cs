using System.ComponentModel.DataAnnotations;
using MedProgramari.Infrastructure.Data;

namespace MedProgramari.Domain.Data;

public class Role
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}