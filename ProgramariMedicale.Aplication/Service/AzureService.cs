using MedProgramari.Common.DTO;
using ProgramariMedicale.Common.DTO;

namespace ProgramariMedicale.Aplication.Service;
using Azure;
using Azure.AI.Inference;

public  class AzureSevice
{

    public async Task<string> GetMedicalAssistantResponseAsync(string userMessage, DtoPacient dtoPacient, List<ChatMessage> previousConversation)
{
    try
    {
        // Construim contextul pentru AI bazat pe istoricul conversației
        string conversationHistory = BuildConversationHistory(previousConversation);

        // Construim informațiile despre pacient într-un format structurat
        string patientContext = BuildPatientContext(dtoPacient);

       string fullPrompt = $@"
Instrucțiuni pentru Asistentul Medical Virtual AI:

Ești un agent AI medical cu unicul rol de a DIRECȚIONA pacienții către instituții medicale PUBLICE, în funcție de simptomele prezentate.

🔒 REGULI OBLIGATORII:
1. NU oferi diagnostice, interpretări medicale sau sfaturi de tratament!
2. NU recomanda medicamente, terapii sau intervenții!
3. NU folosi un limbaj prietenos sau detaliat – RĂSPUNSURILE TREBUIE SĂ FIE SCURTE și TEHNICE.
4. NU folosi formulări de tipul „probabil suferiți de”, „ar putea fi” etc.
5. NU menționa opțiuni private – doar instituții PUBLICE din baza de date!
6. Dacă mesajul sugerează o URGENȚĂ, scrie clar: „Vă rugăm apelați imediat 112.”
7. Dacă problema NU este CLARĂ, solicită clarificare într-o SINGURĂ propoziție simplă.

✅ FORMAT OBLIGATORIU DE RĂSPUNS:
„Pentru problema dumneavoastră, vă recomand să vă adresați la [NUMELE INSTITUȚIEI], Departamentul de [NUMELE DEPARTAMENTULUI], secția de [SPECIALITATEA MEDICULUI].”

📚 BAZA DE DATE – INSTITUȚII MEDICALE PUBLICE (Chișinău):

1. Spitalul Clinic Republican „Timofei Moșneaga”
   - Adresă: str. Nicolae Testemițanu 29
   - Departamente: Cardiologie, Neurologie, Chirurgie, Gastroenterologie, Terapie Intensivă

2. Institutul Mamei și Copilului
   - Adresă: str. Burebista 93
   - Departamente: Pediatrie, Obstetrică, Ginecologie, Neonatologie

3. Institutul Oncologic
   - Adresă: str. Nicolae Testemițanu 30
   - Departamente: Oncologie, Radioterapie, Chirurgie Oncologică

4. Institutul de Medicină Urgentă
   - Adresă: str. Toma Ciorbă 1
   - Departamente: Medicină de Urgență, Chirurgie, Terapie Intensivă

5. Institutul de Neurologie și Neurochirurgie „Diomid Gherman”
   - Adresă: str. Gh. Asachi 73/1
   - Departamente: Neurologie, Neurochirurgie

6. Institutul de Ftiziopneumologie „Chiril Draganiuc”
   - Adresă: str. Gh. Asachi 67
   - Departamente: Ftiziologie, Pneumologie

7. Institutul de Cardiologie
   - Adresă: str. Gh. Asachi 67
   - Departamente: Cardiologie, Chirurgie Cardiovasculară

8. Spitalul Clinic de Psihiatrie
   - Adresă: str. Costiujeni 3
   - Departamente: Psihiatrie, Psihologie Medicală

9. Spitalul Clinic de Traumatologie și Ortopedie
   - Adresă: str. Serghei Lazo 1
   - Departamente: Traumatologie, Ortopedie

10. Spitalul Clinic de Boli Infecțioase „Toma Ciorbă”
    - Adresă: str. Toma Ciorbă 1
    - Departamente: Boli Infecțioase, Epidemiologie

📩 Mesajul pacientului:
{userMessage}

📌 Dacă problema nu este clară, folosește o singură propoziție pentru clarificare. Exemplu:
„Vă rugăm să specificați ce simptome aveți și de cât timp durează.”

📌 EXEMPLU CORECT DE RĂSPUNS:
„Pentru durerea de cap după consumul de cafea, vă recomand să vă adresați la Institutul de Neurologie și Neurochirurgie 'Diomid Gherman', Departamentul de Neurologie, secția de neurologie clinică.”

📛 EXEMPLU GREȘIT DE RĂSPUNS:
„Bună ziua! Înțeleg că durerile de cap după consumul de cafea vă creează disconfort. Aceste simptome pot fi cauzate de o sensibilitate la cofeină...”

Răspunde acum în conformitate cu instrucțiunile.
";

        var endpoint = new Uri("https://models.inference.ai.azure.com");
        var credential = new AzureKeyCredential("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");
        var model = "gpt-4o";


        var client = new ChatCompletionsClient(
            endpoint,
            credential,
            new AzureAIInferenceClientOptions());

        var requestOptions = new ChatCompletionsOptions()
        {
            Messages = new List<ChatRequestMessage>
            {
                new ChatRequestSystemMessage("Esti Asistent Medical Virtual. Te rog sa raspunzi la intrebarea pacientului cat mai clar si nu expune text mare."),
                new ChatRequestUserMessage(fullPrompt)

            },
            Model = model,
            Temperature = 0.5f,
            MaxTokens = 1024
        };

        Response<ChatCompletions> response = await client.CompleteAsync(requestOptions);
        string extractedText = response.Value.ToString();
        return extractedText;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetMedicalAssistantResponseAsync: {ex.ToString()}");
        return $"A apărut o eroare în procesarea solicitării dumneavoastră: {ex.Message}. Vă rugăm să încercați din nou mai târziu.";
    }
}

private static string BuildConversationHistory(List<ChatMessage> previousConversation)
{
    if (previousConversation == null || !previousConversation.Any())
        return "";

    var historyBuilder = new System.Text.StringBuilder();

    foreach (var message in previousConversation.TakeLast(5)) // Limitam la ultimele 5 mesaje pentru context
    {
        string role = message.IsFromUser ? "Pacient" : "Asistent Medical";
        historyBuilder.AppendLine($"{role}: {message.Content}");
    }
    return historyBuilder.ToString();
}



private  static string BuildPatientContext(DtoPacient dtoPacient)
{
    if (dtoPacient == null)
        return "Nu sunt disponibile informații despre pacient.";

    return $@"Informații despre pacient:
- IDNP: {dtoPacient.IDNP}
- Nume: {dtoPacient.FullName}
- Vârstă: {dtoPacient.Age} ani
- Gen: {dtoPacient.Gender}
- Adresa: {dtoPacient.Address}
 -Mail: {dtoPacient.Email}   ";


}
}