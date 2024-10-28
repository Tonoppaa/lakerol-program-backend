
// Tässä määritellään tietokannan Kayttaja attribuutit luokkina, joita hyödynnetään tiedon hankinnassa
namespace backend_Lakerol.Models
{
    public class Kayttaja
    {
        public string Kayttaja_Id { get; set; } // CHAR(8)
        public string Sposti { get; set; }      // VARCHAR(50)
        public string Salasana { get; set; }    // VARCHAR(50)
        public string Etunimi { get; set; }      // VARCHAR(20)
        public string Sukunimi { get; set; }     // VARCHAR(40)
    }
}
