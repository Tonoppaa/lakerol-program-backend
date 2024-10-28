// Tässä määritellään tietokannan Kayttaja attribuutit luokkina, joita hyödynnetään tiedon hankinnassa

namespace backend_Lakerol.Models
{
    public class Tuote
    {
        public long Tuote_Id { get; set; } // BIGINT , vastaa C#:ssa long:a
        public string Tuote_Nimi { get; set; }      // VARCHAR(55)
        public string Tuote_Paino { get; set; }    // CHAR(6)
        public string EnergiaMaara { get; set; }      // VARCHAR(25)
        public string Tuote_Kuvaus { get; set; }     // VARCHAR(1000)
        public byte[] Tuote_Kuva { get; set; }     // BLOB , kuva tallennetaan byte[]-muodossa
        public string? Tuote_Kuva_Base64 { get; set; } // Base64-muodossa oleva kuva
    }
}
