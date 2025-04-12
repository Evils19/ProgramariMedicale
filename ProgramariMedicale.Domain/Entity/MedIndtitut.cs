using System.ComponentModel.DataAnnotations;
using MedProgramari.Infrastructure.Data;

namespace MedProgramari.Domain.Data;

public class MedIndtitut
{
    [Key]
    public string Id_Institutie { get; set; }
    public string Denumire { get; set; }
    public string adresa { get; set; }
    public string telefon { get; set; }
    public string Status { get; set; }
    public virtual ICollection<Departamente> Departament { get; set; } = new HashSet<Departamente>();



}