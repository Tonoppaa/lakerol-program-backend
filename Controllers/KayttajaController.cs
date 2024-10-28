using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using backend_Lakerol.Models; // Kayttaja.cs:ssä sijaitsevat luokat
using static System.Net.WebRequestMethods;

// Tämä kontrolleri käsittelee käyttäjän tietokantahakuja ja kirjautumista MySQL-tietokannasta

namespace backend_Lakerol.Controllers
{
    [Route("api/[controller]")] // Asettaa reitin kontrollerille, esim. api/Kayttaja
    [ApiController]
    public class KayttajaController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KayttajaController(IConfiguration configuration)
        { /* Konstruktorissa IConfiguration-parametri tuodaan konfiguraatioasetukset kontrollerille riippuvuuden injektoinnin 
           * kautta ja tallennetaan _configuration-muuttujaan */
            _configuration = configuration;
            // jäsenmuuttuja, joka säilyttää konfiguraatioasetukset (tietokannan yhteysmerkkijonon)
        }

        [HttpGet] // Metodi käsittelee GET-pyyntöjä
        public ActionResult<List<Kayttaja>> Get()
        /* ActionResult on C# tyyppi, jota käytetään ASP.NET Core -kontrollereissa määrittämään palautettavan 
         * HTTP-vastauksen tyyppi ja sisältö */
        /* ActionResult<List<Kayttaja>> Get(): Palauttaa listan Kayttaja-olioita ActionResult-tyyppisenä */
        {
            string kysely = @"
                            SELECT * FROM Kayttaja
                            ";

            List<Kayttaja> kayttajat = new List<Kayttaja>(); // Luodaan uusi lista, joka sisältää käyttäjiä
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
                            Kayttaja kayttaja = new Kayttaja
                            //  Luodaan uusi Kayttaja-olio ja asetetaan sen kentät lukemalla tiedot sarakkeista
                            {
                                Kayttaja_Id = myReader.GetString(0),
                                Sposti = myReader.GetString(1),
                                Salasana = myReader.GetString(2),
                                Etunimi = myReader.GetString(3),
                                Sukunimi = myReader.GetString(4)
                            };
                            kayttajat.Add(kayttaja);
                        }
                    }
                }
            }
            return Ok(kayttajat); // Palauttaa kayttajat-listan HTTP 200 OK - vastauksena
        }

        [HttpPost("Login")] // Metodi käsittelee POST-pyyntöjä
        public IActionResult PostLoginInformation([FromBody] LoginDto loginData)
        {
            string query = @"
                            SELECT COUNT(*)
                            FROM Kayttaja
                            WHERE kayttaja_id = @kayttajaId AND salasana = @salasana;
                           ";

            string sqlDataSource = _configuration.GetConnectionString("lakerolCon");

            using (MySqlConnection myConn = new MySqlConnection(sqlDataSource))
            {
                myConn.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myConn))
                {
                    myCommand.Parameters.AddWithValue("@kayttajaId", loginData.Kayttaja_Id);
                    myCommand.Parameters.AddWithValue("@salasana", loginData.Salasana);

                    int userExists = Convert.ToInt32(myCommand.ExecuteScalar());
                    // Palauttaa COUNT(*) eli katsotaan, täsmääkö kyselyn tulokset

                    // Tarkistetaan, onko käyttäjiä 
                    if (userExists > 0)
                    {
                        return Ok(new { success = true, message = "Kirjautuminen onnistui" });
                    }
                    else
                    {
                        return Unauthorized(new { success = false, message = "Virheellinen käyttäjätunnus tai salasana" });
                    }
                }
            }
        }
        public class LoginDto // Tietoluokka, joka sisältää kirjautumiseen tarvittavat tiedot
        {
            public string Kayttaja_Id { get; set; }
            public string Salasana { get; set; }
        }
    }
}
