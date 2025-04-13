using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Demo.Pages;
using MedProgramari.Common.DTO;
using ProgramariMedicale.Common.DTO;

namespace ProgramariMedicale.Aplication.Service;

public class AiService
{
    private readonly AiModelAcces _aiModelAcces;

    public AiService(AiModelAcces aiModelAcces)
    {
        _aiModelAcces = aiModelAcces;
    }

    public async Task<string> GetMedicalAssistantResponseAsync(string userMessage, DtoPacient dtoPacient,
        List<ChatMessage> previousConversation)
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
Timpul actual este {DateTime.Now}.

🔒 REGULI OBLIGATORII:
1. NU oferi diagnostice, interpretări medicale sau sfaturi de tratament!
2. NU recomanda medicamente, terapii sau intervenții!
3. NU folosi un limbaj prietenos sau detaliat – RĂSPUNSURILE TREBUIE SĂ FIE SCURTE și TEHNICE.
4. NU folosi formulări de tipul „probabil suferiți de”, „ar putea fi” etc.
5. NU menționa opțiuni private – doar instituții PUBLICE din baza de date!
6. Dacă mesajul sugerează o URGENȚĂ, scrie clar: „Vă rugăm apelați imediat 112.”
7. Dacă problema NU este CLARĂ, dar pacientul NU poate oferi detalii, RECOMANDĂ adresa către MEDICUL DE FAMILIE al pacientului, specificând instituția unde activează acesta.
8. Dacă problema NU este CLARĂ și niciun simptom nu este menționat, solicită o CLARIFICARE într-o SINGURĂ propoziție simplă.

📌 REGULI SUPLIMENTARE PENTRU PROGRAMĂRI:

1. Dacă pacientul folosește cuvinte precum ""programează"", ""fă-mi o programare"", ""vreau o programare"", ""când pot veni"", ""când este posibil"" sau orice altă solicitare similară, TREBUIE să procesezi acest mesaj ca o cerere de programare.

2. La solicitarea unei programări, OBLIGATORIU:
   - Întreabă pacientul despre data și ora preferată (dacă nu sunt specificate)
   - Confirmă disponibilitatea în intervalul solicitat
   - Generează un JSON cu câmpul ""Programare"" setat la ""true""
   - Completează toate câmpurile relevante (Date, TimeStart, DoctorName, Departament, InstitunName)

3. EXEMPLU DE FLUX DE CONVERSAȚIE PENTRU PROGRAMARE:
   Pacient: ""Vreau să mă programez pentru durerea de picior""
   Răspuns: ""Pentru programarea referitoare la durerea de picior, vă rog să specificați data și ora preferată. Programul medicilor este între orele 09:00-18:00, cu pauză de masă 13:00-14:00. Programările sunt disponibile în intervale de 30 min.""
   
   Pacient: ""Mâine la ora 10""
   Răspuns: [JSON cu programare confirmată pentru data următoare, ora 10:00]

4. NU ignora NICIODATĂ o solicitare de programare! Verifică întotdeauna mesajul pacientului pentru orice cerere de programare, chiar dacă aceasta este formulată indirect.

5. Pentru solicitările de programare ""pentru mâine"" sau ""pentru azi"", calculează data corectă și completează câmpul ""Date"" în format ""yyyy-MM-dd"".


📌 EXCEPȚIE pentru cazuri NECLARE (pacientul nu oferă detalii):
„Vă recomand să vă adresați medicului dumneavoastră de familie, dna Ana Devizia, la Spitalul Clinic Republican „Timofei Moșneaga”.”


📌 Dacă problema nu este clară, dar pacientul ezită sau nu știe să răspundă, aplică regula 7 (adresare către medicul de familie).

📌 EXEMPLU CORECT DE RĂSPUNS:
„Pentru durerea de cap după consumul de cafea, vă recomand să vă adresați la Institutul de Neurologie și Neurochirurgie 'Diomid Gherman', Departamentul de Neurologie, secția de neurologie clinică.”

📛 EXEMPLU GREȘIT DE RĂSPUNS:
„Bună ziua! Înțeleg că durerile de cap după consumul de cafea vă creează disconfort. Aceste simptome pot fi cauzate de o sensibilitate la cofeină...”


✅ FORMAT OBLIGATORIU DE RĂSPUNS:
TOATE răspunsurile trebuie să fie returnate în format JSON, conform structurii:

```json
{{
  ""Message"": ""Textul răspunsului principal, recomandarea instituției medicale"",
  ""Programare"": false,
  ""Date"": ""0001-01-01"",
  ""TimeStart"": ""00:00:00"",
  ""DoctorName"": """",
  ""DepartamentNane"": """",
  ""InstitutName"": """",
  ""Idnp_Pacient"": """",
""InstitAdresa"": """",
  ""MedInfo"": """"
}}
📌 REGULI PENTRU PROGRAMARE:


""Programare"": true
""Date"": Data solicitată în format ""yyyy-MM-dd""
""TimeStart"": Ora de început în format ""HH:mm""
""DoctorName"": Numele medicului recomandat sau solicitat
""Departament"": Departamentul medical
""InstitunName"": Numele instituției medicale
""Idnp_Pacient"": IDNP-ul pacientului din informațiile furnizate
""MedInfo"": Un rezumat scurt al problemei medicale descrise



📚 BAZA DE DATE – INSTITUȚII MEDICALE PUBLICE (Chișinău):

Spitalul Clinic Republican „Timofei Moșneaga""

Adresă: str. Nicolae Testemițanu 29
Departamente: Cardiologie, Neurologie, Chirurgie, Gastroenterologie, Terapie Intensivă
Medici: Dr. Vasile Andrieș (Cardiologie), Dr. Maria Popescu (Neurologie), Dr. Ion Rusu (Chirurgie), Dr. Elena Carp (Gastroenterologie)


Institutul Mamei și Copilului

Adresă: str. Burebista 93
Departamente: Pediatrie, Obstetrică, Ginecologie, Neonatologie
Medici: Dr. Ana Cojocaru (Pediatrie), Dr. Mihai Lungu (Obstetrică), Dr. Victoria Botnari (Ginecologie)


Institutul Oncologic

Adresă: str. Nicolae Testemițanu 30
Departamente: Oncologie, Radioterapie, Chirurgie Oncologică
Medici: Dr. Sergiu Moraru (Oncologie), Dr. Silvia Ursu (Radioterapie)


Institutul de Medicină Urgentă

Adresă: str. Toma Ciorbă 1
Departamente: Medicină de Urgență, Chirurgie, Terapie Intensivă
Medici: Dr. Andrei Vrabie (Medicină de Urgență), Dr. Doina Mihailov (Chirurgie)


Institutul de Neurologie și Neurochirurgie „Diomid Gherman""

Adresă: str. Gh. Asachi 73/1
Departamente: Neurologie, Neurochirurgie
Medici: Dr. Valeriu Munteanu (Neurologie), Dr. Nina Rotaru (Neurochirurgie)


Institutul de Ftiziopneumologie „Chiril Draganiuc""

Adresă: str. Gh. Asachi 67
Departamente: Ftiziologie, Pneumologie
Medici: Dr. Tudor Catană (Ftiziologie), Dr. Galina Roșca (Pneumologie)


Institutul de Cardiologie

Adresă: str. Gh. Asachi 67
Departamente: Cardiologie, Chirurgie Cardiovasculară
Medici: Dr. Nicolae Ciobanu (Cardiologie), Dr. Cristina Lungu (Chirurgie Cardiovasculară)


Spitalul Clinic de Psihiatrie

Adresă: str. Costiujeni 3
Departamente: Psihiatrie, Psihologie Medicală
Medici: Dr. Alexandru Popa (Psihiatrie), Dr. Veronica Sandu (Psihologie Medicală)


Spitalul Clinic de Traumatologie și Ortopedie

Adresă: str. Serghei Lazo 1
Departamente: Traumatologie, Ortopedie
Medici: Dr. Dumitru Sofroni (Traumatologie), Dr. Alina Negru (Ortopedie)


Spitalul Clinic de Boli Infecțioase „Toma Ciorbă""

Adresă: str. Toma Ciorbă 1
Departamente: Boli Infecțioase, Epidemiologie
Medici: Dr. Laura Țurcanu (Boli Infecțioase), Dr. Petru Grama (Epidemiologie)


📌 EXCEPȚIE pentru cazuri NECLARE (pacientul nu oferă detalii):


📌 EXCEPȚIE pentru cazuri NECLARE (pacientul nu oferă detalii):
json{{
  ""Message"": ""Vă recomand să vă adresați medicului dumneavoastră de familie, dna Ana Devizia, la Spitalul Clinic Republican „Timofei Moșneaga""."",
  ""Programare"": false,
  ""Date"": ""0001-01-01"",
  ""TimeStart"": ""00:00:00"",
  ""DoctorName"": """",
  ""DepartamentName"": """",
  ""InstitutName"": """",
  ""Idnp_Pacient"": """",
   ""InstitAdresa"": """",
  ""MedInfo"": """"
}}

📌 EXEMPLE DE RĂSPUNSURI:

Pentru o simplă recomandare (fără programare):

json{{
  ""Message"": ""Pentru durerea de cap după consumul de cafea, vă recomand să vă adresați la Institutul de Neurologie și Neurochirurgie 'Diomid Gherman', Departamentul de Neurologie, secția de neurologie clinică."",
  ""Programare"": false,
  ""Date"": ""0001-01-01"",
  ""TimeStart"": ""00:00:00"",
  ""DoctorName"": """",
  ""DeparatamentName"": """",
  ""InstitutName"": """",
  ""Idnp_Pacient"": """",
 ""InstitAdresa"": """",
  ""MedInfo"": """"
}}

Pentru o programare confirmată:



json{{
  ""Message"": ""Pentru durerea de cap după consumul de cafea, vă recomand să vă adresați la Institutul de Neurologie și Neurochirurgie 'Diomid Gherman', Departamentul de Neurologie."",
  ""Programare"": true,
  ""Date"": ""2025-04-15"",
  ""TimeStart"": ""10:00:00"",
  ""DoctorName"": ""Dr. Valeriu Munteanu"",
  ""DeparatamentName"": ""Neurologie"",
  ""InstitutName"": ""Institutul de Neurologie și Neurochirurgie 'Diomid Gherman'"",
  ""Idnp_Pacient"": ""2004040012345"",
  ""InstitAdresa"": "" str. Gh. Asachi 73/1"",
  ""MedInfo"": ""Pacient prezintă dureri de cap după consumul de cafea""
}}

📛 EXEMPLU GREȘIT DE RĂSPUNS:
„Bună ziua! Înțeleg că durerile de cap după consumul de cafea vă creează disconfort. Aceste simptome pot fi cauzate de o sensibilitate la cofeină...""
Răspunde acum în conformitate cu instrucțiunile și ASIGURĂ-TE că răspunsul tău este întotdeauna un JSON valid conform structurii specificate.
"";

Răspunde acum în conformitate cu instrucțiunile.

Informatii despre pacient:
{patientContext}
Istorica Mesajelor:
{conversationHistory}

📩 Mesajul pacientului:
{userMessage}

";


            // Constructing the JSON request
            var request = new JsonObject
            {
                ["model"] = "llama-3.3-70b-versatile",
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = fullPrompt
                    }
                },
                ["temperature"] = 0.2,
                ["max_tokens"] = 1024,
                ["top_p"] = 1,
                ["stream"] = false
            };

            // Sending the request asynchronously and awaiting the response
            var response = await _aiModelAcces.CreateChatCompletionAsync(request);

            // Parsing the response
            return response?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ??
                   "Nu am putut procesa răspunsul. Vă rugăm să încercați din nou.";
        }
        catch (Exception ex)
        {
            // Log the full exception for better debugging
            Console.WriteLine($"Error in GetMedicalAssistantResponseAsync: {ex.ToString()}");
            return
                $"A apărut o eroare în procesarea solicitării dumneavoastră: {ex.Message}. Vă rugăm să încercați din nou mai târziu.";
        }
    }

    private string BuildConversationHistory(List<ChatMessage> previousConversation)
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

    private string BuildPatientContext(DtoPacient dtoPacient)
    {
        if (dtoPacient == null)
            return "Nu sunt disponibile informații despre pacient.";

        return $@"Informații despre pacient:
                     - IDNP: {dtoPacient.IDNP}
                     - Nume: {dtoPacient.FullName}
                     - Vârstă: {dtoPacient.Age} ani
                     - Gen: {dtoPacient.Gender}
                     - Adresa: {dtoPacient.Address}
                      -Mail: {dtoPacient.Email}  
                     -Medicul de familie a pacientului: Ana Devizia
                     -Institutul unde lucreaza MEdicul de famile: Spitalul Clinic Republican „Timofei Moșneaga”";
    }


    private string ExtractCleanJson(string input)
    {
        if (input.Contains("```json"))
        {
            var start = input.IndexOf("```json") + 7;
            var end = input.IndexOf("```", start);
            if (end > start)
            {
                return input.Substring(start, end - start).Trim();
            }
        }

        int startIndex = input.IndexOf('{');
        int endIndex = input.LastIndexOf('}');

        if (startIndex >= 0 && endIndex > startIndex)
        {
            var json = input.Substring(startIndex, endIndex - startIndex + 1);

            // Înlocuiește ghilimelele simple din interiorul valorilor cu ghilimele duble escape-uit
            json = json.Replace("‘", "'").Replace("’", "'").Replace("\"", "\"");

            return json;
        }

        return string.Empty;
    }





    public async Task<string> ProcesingResponze(string userMessage, DtoPacient dtoPacient,
        List<ChatMessage> previousConversation)
    {
        var response = await GetMedicalAssistantResponseAsync(userMessage, dtoPacient, previousConversation);

        var cleanJson = ExtractCleanJson(response);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        try
        {
            Console.WriteLine("Clean JSON: " + cleanJson);
            var jsonResponse = JsonSerializer.Deserialize<DtoResponseAi>(cleanJson, options);


            if (jsonResponse.Programare)
            {
               jsonResponse.Message=$"Programarea dumneavoastră  pe data  {jsonResponse.Date.ToString("yyyy-MM-dd")} la ora {jsonResponse.TimeStart} a fost confirmată. Medicul {jsonResponse.DoctorName} vă va consulta la {jsonResponse.InstitutName}, Departamentul {jsonResponse.DeparatamentName}. la adresa {jsonResponse.InstitutAdresa}.";

            }

            return jsonResponse?.Message ?? "Mesajul nu a fost furnizat.";

        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization error: {ex.Message}");
            return "Ne pare rău, nu am putut procesa răspunsul.";
        }
    }


}