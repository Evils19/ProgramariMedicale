using System.Text.Json;
using MedProgramari.Common.DTO;
using Microsoft.AspNetCore.Mvc;
using ProgramariMedicale.Aplication.Service;
using ProgramariMedicale.Common.DTO;

namespace ProgramariMedicale.Controllers
{
    [ApiController]
    public class MedicalAssistantController : ControllerBase
    {
        private readonly AiService _aiService;
        private readonly AzureSevice _azureSevice;

        public MedicalAssistantController(AiService aiService, AzureSevice azureSevice)
        {
            _aiService = aiService;
            _azureSevice = azureSevice;
        }


        [HttpPost]
        [Route("/asistent-medical")]
        public async Task<IActionResult> GetAsistentMedical()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                // Deserializarea conținutului primit în obiectul necesar
                var medical = JsonSerializer.Deserialize<MedicalRequestAsistentDto>(body);

                if (medical == null)
                {
                    return BadRequest("Datele primite sunt invalide.");
                }

                // var response = await _aiService.GetMedicalAssistantResponseAsync(medical.UserMessage, medical.DtoPacient, medical.Messages);

                var response = await _aiService.ProcesingResponze(medical.UserMessage, medical.DtoPacient, medical.Messages);


                // var response = await _azureSevice.GetMedicalAssistantResponseAsync(medical.UserMessage, medical.DtoPacient, medical.Messages);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }




}