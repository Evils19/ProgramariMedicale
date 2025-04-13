using System.Text.Json.Serialization;

namespace ProgramariMedicale.Common.DTO;

public class DtoResponseAi
{
   [JsonPropertyName("Message")]
    public string Message { get; set; }
    [JsonPropertyName("Programare")]
    public bool Programare { get; set; }
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("TimeStart")]
    public TimeSpan TimeStart { get; set; }
    [JsonPropertyName("DoctorName")]
    public string DoctorName { get; set; }
    [JsonPropertyName("InstitutName")]
    public string InstitutName { get; set; }
    [JsonPropertyName("DeparatamentName")]
    public string DeparatamentName { get; set; }
    [JsonPropertyName("Idnp_Pacient")]
    public string Idnp_Pacient { get; set; }
    [JsonPropertyName("InstitAdresa")]
    public string InstitutAdresa { get; set; }
    [JsonPropertyName("MedInfo")]
    public string MedInfo { get; set; }


}