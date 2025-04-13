using MedProgramari.Common.DTO;

namespace ProgramariMedicale.Common.DTO;

public class MedicalRequestAsistentDto
{



        public string UserMessage { get; set; }
        public DtoPacient DtoPacient { get; set; }
        public List<ChatMessage>  Messages { get; set; }


}