using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using backend_Lakerol.Models; // Tuote.cs:ssä sijaitsevat luokat
using Google.Protobuf.WellKnownTypes;
using System.Text;

namespace backend_Lakerol.Controllers
{
    [Route("api/[controller]")] // Määrittää reitin, jolla kontrolleri on käytettävissä API, eli api/Tuote
    [ApiController]
    public class TuoteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TuoteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<List<Tuote>> Get(string? searchTerm)
        /* Metodi palauttaa ActionResult-tyyppisen listan Tuote-oliosta, ja se voi vastaanottaa valinnaisen 
         * searchTerm-parametrin haun suorittamiseen */
        {
            // Kysely, joka käsittelee käyttäjän tuotehakua
            string kysely = @"
                            SELECT tuote_id, tuote_nimi, tuote_paino, energiamaara, tuote_kuvaus, tuote_kuva 
                            FROM Tuote
                            WHERE (@searchTerm IS NULL 
                            OR CAST(tuote_id AS CHAR) LIKE CONCAT('%', @searchTerm, '%')
                            OR tuote_nimi LIKE CONCAT('%', @searchTerm, '%'))
                            ";
            // Jos searchTerm-parametri on null, tämä ehto on tosi, ja kaikki tuotteet haetaan ilman suodatusta
            // 'LIKE CONCAT('%', @searchTerm, '%' -osuus tarkistaa, sisältääkö tuote_id-sarake searchTerm-parametrin

            List<Tuote> tuotteet = new List<Tuote>();
            string sqlDataSource = _configuration.GetConnectionString("lakerolCon");
            using (MySqlConnection myConn = new MySqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (MySqlCommand myCommand = new MySqlCommand(kysely, myConn))
                {
                    // Lisää hakusana parametrina
                    myCommand.Parameters.AddWithValue("@searchTerm", searchTerm);
                    using (MySqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            Tuote tuote = new Tuote
                            // Luodaan uusi Tuote-olio ja täytetään se tietokantatuloksilla
                            {
                                Tuote_Id = myReader.GetInt64(0),
                                Tuote_Nimi = myReader.GetString(1),
                                Tuote_Paino = myReader.GetString(2),
                                EnergiaMaara = myReader.GetString(3),
                                Tuote_Kuvaus = myReader.GetString(4),
                                Tuote_Kuva_Url = myReader.IsDBNull(5) ? null : myReader.GetString(5) // Hae URL-osoite suoraan
                            };

                            // Käytetään GetKuvaBase64-metodia, jos tarvetta
                            // tuote.Tuote_Kuva_Base64 = tuote.GetKuvaBase64();

                            /* Tässä noudetaan paikallisessa yhteydessä kuva

                            if (!myReader.IsDBNull(5)) // Tarkista, onko tuote_kuva-kenttä tyhjä
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    myReader.GetStream(5).CopyTo(memoryStream); // Kopioi stream muistiin
                                    tuote.Tuote_Kuva = memoryStream.ToArray();
                            // Kutsutaan MemoryStream-olion ToArray-metodia, joka muuntaa streamissä olevan datan byte-taulukoksi
                                    tuote.Tuote_Kuva_Base64 = Convert.ToBase64String(tuote.Tuote_Kuva); // Muunna Base64:ksi
                                }
                            } */

                            tuotteet.Add(tuote);
                        }
                    }
                }
            }
            return Ok(tuotteet);
        }

        [HttpGet("Attribuutit")]
        public ActionResult<List<string>> GetTuoteAttribuutit()
        // Haetaan attribuuttien nimet, jotka näytetään hakusivulla ja niiden pohjalta suoritetaan hakutoiminto
        { //                                    muutettu tuote_id:n i pieneksi eli aiemmin oli tuote_Id
            string kysely = @"
                            SELECT CASE
                            WHEN COLUMN_NAME = 'tuote_id' THEN 'EAN-koodi'
                            WHEN COLUMN_NAME = 'tuote_nimi' THEN 'Tuotteen nimi'
                            ELSE COLUMN_NAME 
                            END AS NayttoNimi,
                            COLUMN_NAME AS SarakeNimi
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = 'Tuote'
                            AND COLUMN_NAME NOT IN ('tuote_kuva', 'tuote_kuvaus', 'tuote_paino', 'energiamaara')
                            ORDER BY FIELD(COLUMN_NAME, 'tuote_nimi', 'tuote_id');
                            ";
            // ELSE COLUMN_NAME: Kaikille muille sarakkeille palautetaan niiden alkuperäinen nimi
            // END AS NayttoNimi: Tämä määrittää, että CASE-lausekkeen tulos nimetään NayttoNimi
            // COLUMN_NAME AS SarakeNimi: Sarakkeen alkuperäinen nimi tallennetaan SarakeNimi-nimiseen kolumniin
            // COLUMN_NAME NOT IN: Ehto sulkee pois sarakkeet, jotka eivät ole tarpeellisia näyttöön

            var attribuutit = new List<object>(); // Luodaan lista, johon voidaan lisätä olioita
            string sqlDataSource = _configuration.GetConnectionString("lakerolCon");

            using (MySqlConnection myConn = new MySqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (MySqlCommand myCommand = new MySqlCommand(kysely, myConn))
                {
                    using (MySqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            attribuutit.Add(new { // lisätään uusi objekti attribuutit- eli sarakkeen nimi listaan
                            // Lisää sarakkeen nimi listaan
                            NayttoNimi = myReader.GetString(0), // Haetaan näytettävä nimi
                            SarakeNimi = myReader.GetString(1)
                        });
                        }
                    }
                }
            }
            return Ok(attribuutit);
        }

    }
}
