using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedProgramari.Domain.Data;

public class Departamente
{
   [Key]
    public string Id_departament { get; set; }
    public string Nume_departament { get; set; }
    public string Sepecializare { get; set; }
    public string Id_MedInstitutie { get; set; }

    [ForeignKey("Id_MedInstitutie")]
    public MedIndtitut InstitutiePublica { get; set; }

    public virtual ICollection<Medic> Medici { get; set; } = new List<Medic>();

}