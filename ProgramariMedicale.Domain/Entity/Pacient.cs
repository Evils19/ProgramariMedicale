using System.ComponentModel.DataAnnotations;
using MedProgramari.Common.DTO;
using MedProgramari.Domain.Data;

namespace ProgramariMedicale.Domain.Entity;

public class Pacient
{

    [Key]
    public string IDNP { get; set; }
    public string Prenume { get; set; }
    public string Nume { get; set; }
    public string Phone { get; set; }

    public DateTime Birthday { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public Info Info { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();


}