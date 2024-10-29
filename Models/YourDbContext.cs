// Tätä luokkaa tarvitaan, jotta saadaan kommunikoitua tietokannan kanssa (Railway)

namespace backend_Lakerol.Models
{
    using backend_Lakerol.Models;
    using Microsoft.EntityFrameworkCore;
    using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

    public class YourDbContext : DbContext /* DbContext on osa Entity Frameworkia, joka on Microsoftin ORM (Object-Relational Mapping) -työkalu. 
                                            * ORM mahdollistaa tietokannan tietojen käsittelyn C#-objekteina sen sijaan, että joudutaan 
                                            * kirjoittamaan SQL-kyselyjä suoraan */
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options) { }
        // konstruktorin määrittely. Kun YourDbContext-luokkaa luodaan, se ottaa vastaan DbContextOptions<YourDbContext>-tyyppisen
        // parametrin nimeltä options

        // DbSet<T> on kokoelma, joka vastaa tietokannan taulua. Jokainen DbSet-ominaisuus vastaa yhtä tietokannan taulua
        public DbSet<Tuote> Tuotteet { get; set; } /* DbSet<Tuote>, mikä tarkoittaa, että se edustaa Tuote-oliota, joka on C#-luokka, 
                                                    * joka vastaa tietokannan Tuote-taulua */
        public DbSet<Kayttaja> Kayttajat { get; set; } /* DbSet<Kayttaja>, joka edustaa Kayttaja-oliota, joka on toinen C#-luokka, joka 
                                                        * vastaa tietokannan Kayttaja-taulua */
    }

}
