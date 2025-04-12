using System.ComponentModel.DataAnnotations;

namespace MedProgramari.Domain.Data;

public class Medic
{
    [Key]
    public string IDNP { get; set; }
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string Phone { get; set; }
    public string specializare { get; set; }
    public string id_departament { get; set; }
    public string id_instittutie { get; set; }
    //30 minute
    public TimeSpan TimpConsultatie { get; set; } = new TimeSpan(0, 30, 0);
    public TimeSpan WorkStart { get; set; }  = new TimeSpan(9, 0, 0);
    public  TimeSpan WorkdayEnd { get; set; } = new TimeSpan(18, 0, 0);



}