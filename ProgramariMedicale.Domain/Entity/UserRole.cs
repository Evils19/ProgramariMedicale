using System.ComponentModel.DataAnnotations.Schema;
using ProgramariMedicale.Domain.Entity;

namespace MedProgramari.Domain.Data;

public class UserRole
{
    public string IdUser { get; set; }
    [ForeignKey("IdUser")]
    public Pacient Pacient { get; set; }
    public Guid IdRole { get; set; }
    [ForeignKey("IdRole")]
    public Role Role { get; set; }

}