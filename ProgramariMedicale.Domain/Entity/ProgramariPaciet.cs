using System.ComponentModel.DataAnnotations.Schema;
using MedProgramari.Domain.Data;
using ProgramariMedicale.Domain.Entity;

namespace MedProgramari.Infrastructure.Data;

public class ProgramariPaciet
{

    public string Idnp_Pacient { get; set; }
    [ForeignKey("Idnp_Pacient")]
    public Pacient Pacient { get; set; }
    public string Idnp_Med { get; set; }
    [ForeignKey("Idnp_Med")]
    public Medic Medic { get; set; }
    public DateTime DataStart{ get; set; }
    public DateTime DateEnd { get; set; }

}