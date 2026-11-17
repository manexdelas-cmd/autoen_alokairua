using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using autoen_alokairua.Models;

namespace autoen_alokairua.Services
{
    /// <summary>
    /// SQLite datu-basearekin harremanetan jartzeko zerbitzu nagusia.
    /// Ibilgailuen, bezeroen eta alokairuen CRUD (Sortu, Irakurri, Eguneratu, Ezabatu) eragiketak kudeatzen ditu.
    /// </summary>
    public class DatabaseService
    {
        // Datu-base fitxategiaren kokapen fisikoa gordetzen duen bidea
        private readonly string _dbPath;

        /// <summary>
        /// Eraikitzailea. Datu-basearen bidea zehazten du eta taulak hasieratzen ditu.
        /// </summary>
        public DatabaseService()
        {
            _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "alokairuak.db");
            InicializarBaseDeDatos();
        }

        /// <summary>
        /// Datu-baseko taulak sortzen ditu existitzen ez badira eta hasierako proba-datuak txertatzen ditu.
        /// </summary>
        private void InicializarBaseDeDatos()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Ibilgailuak (
                    Matrikula TEXT PRIMARY KEY,
                    Marka TEXT NOT NULL,
                    Modeloa TEXT NOT NULL,
                    Urtea INTEGER NOT NULL,
                    Erregaia TEXT NOT NULL,
                    EgunekoPrezioa REAL NOT NULL,
                    Egoera TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS Bezeroak (
                    NAN TEXT PRIMARY KEY,
                    Izena TEXT NOT NULL,
                    Abizenak TEXT NOT NULL,
                    Telefonoa TEXT,
                    Emaila TEXT
                );
                CREATE TABLE IF NOT EXISTS Alokairuak (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BezeroaNAN TEXT NOT NULL,
                    IbilgailuaMatrikula TEXT NOT NULL,
                    HasieraData TEXT NOT NULL,
                    AmaieraData TEXT NOT NULL,
                    PrezioOsoa REAL NOT NULL,
                    FOREIGN KEY (BezeroaNAN) REFERENCES Bezeroak(NAN),
                    FOREIGN KEY (IbilgailuaMatrikula) REFERENCES Ibilgailuak(Matrikula)
                );

                INSERT OR IGNORE INTO Ibilgailuak VALUES ('1234ABC', 'Toyota', 'Yaris', 2022, 'Hibridoa', 45.0, 'Eskuragarri');
                INSERT OR IGNORE INTO Ibilgailuak VALUES ('5678DEF', 'Seat', 'Leon', 2021, 'Gasolina', 40.0, 'Alokatuta');
                INSERT OR IGNORE INTO Ibilgailuak VALUES ('9012GHI', 'Volkswagen', 'Golf', 2020, 'Diesel', 50.0, 'Mantentze-lanetan');

                INSERT OR IGNORE INTO Bezeroak VALUES ('12345678A', 'Jon', 'Agirre', '600111222', 'jon@email.com');
                INSERT OR IGNORE INTO Bezeroak VALUES ('87654321B', 'Ane', 'Etxebarria', '600333444', 'ane@email.com');
            ";
            cmd.ExecuteNonQuery();
        }

        // ==========================================
        // IBILGAILUAK (VEHÍCULOS)
        // ==========================================

        /// <summary>
        /// Datu-basean erregistratutako ibilgailu guztien zerrenda eskuratzen du.
        /// </summary>
        /// <returns>Autoak motako objektuen zerrenda.</returns>
        public List<Autoak> GetVehiculos()
        {
            var list = new List<Autoak>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Ibilgailuak;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Autoak
                {
                    Matrikula = reader.GetString(0),
                    Marka = reader.GetString(1),
                    Modeloa = reader.GetString(2),
                    Urtea = reader.GetInt32(3),
                    Erregaia = reader.GetString(4),
                    EgunekoPrezioa = reader.GetDouble(5),
                    Egoera = reader.GetString(6)
                });
            }
            return list;
        }

        /// <summary>
        /// Ibilgailu berri bat txertatzen du datu-basean.
        /// </summary>
        /// <param name="a">Gorde beharreko ibilgailuaren datuak dituen objektua.</param>
        public void InsertarVehiculo(Autoak a)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Ibilgailuak (Matrikula, Marka, Modeloa, Urtea, Erregaia, EgunekoPrezioa, Egoera)
                VALUES (@mat, @marka, @modeloa, @urtea, @erregaia, @prezioa, @egoera);";
            command.Parameters.AddWithValue("@mat", a.Matrikula);
            command.Parameters.AddWithValue("@marka", a.Marka);
            command.Parameters.AddWithValue("@modeloa", a.Modeloa);
            command.Parameters.AddWithValue("@urtea", a.Urtea);
            command.Parameters.AddWithValue("@erregaia", a.Erregaia);
            command.Parameters.AddWithValue("@prezioa", a.EgunekoPrezioa);
            command.Parameters.AddWithValue("@egoera", a.Egoera);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Existitzen den ibilgailu baten datuak eguneratzen ditu matrikularen arabera.
        /// </summary>
        /// <param name="a">Datu eguneratuak dituen ibilgailu objektua.</param>
        public void ActualizarVehiculo(Autoak a)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Ibilgailuak 
                SET Marka = @marka, Modeloa = @modeloa, Urtea = @urtea, Erregaia = @erregaia, EgunekoPrezioa = @prezioa, Egoera = @egoera
                WHERE Matrikula = @mat;";
            command.Parameters.AddWithValue("@mat", a.Matrikula);
            command.Parameters.AddWithValue("@marka", a.Marka);
            command.Parameters.AddWithValue("@modeloa", a.Modeloa);
            command.Parameters.AddWithValue("@urtea", a.Urtea);
            command.Parameters.AddWithValue("@erregaia", a.Erregaia);
            command.Parameters.AddWithValue("@prezioa", a.EgunekoPrezioa);
            command.Parameters.AddWithValue("@egoera", a.Egoera);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Ibilgailu bat datu-basetik ezabatzen du bere matrikula erabiliz.
        /// </summary>
        /// <param name="matrikula">Ezabatu beharreko ibilgailuaren matrikula.</param>
        public void EliminarVehiculo(string matrikula)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Ibilgailuak WHERE Matrikula = @mat;";
            command.Parameters.AddWithValue("@mat", matrikula);
            command.ExecuteNonQuery();
        }

        // ==========================================
        // BEZEROAK (CLIENTES)
        // ==========================================

        /// <summary>
        /// Datu-basean dauden bezero guztien zerrenda eskuratzen du.
        /// </summary>
        /// <returns>Bezeroa motako objektuen zerrenda.</returns>
        public List<Bezeroa> GetClientes()
        {
            var list = new List<Bezeroa>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT NAN, Izena, Abizenak, Telefonoa, Emaila FROM Bezeroak;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Bezeroa
                {
                    NAN = reader.GetString(0),
                    Izena = reader.GetString(1),
                    Abizenak = reader.GetString(2),
                    Telefonoa = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Emaila = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }
            return list;
        }

        /// <summary>
        /// Bezero berri bat txertatzen du datu-basean.
        /// </summary>
        /// <param name="c">Gorde beharreko bezeroaren datuak dituen objektua.</param>
        public void InsertarCliente(Bezeroa c)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Bezeroak (NAN, Izena, Abizenak, Telefonoa, Emaila)
                VALUES (@nan, @izena, @abizenak, @tel, @email);";
            command.Parameters.AddWithValue("@nan", c.NAN);
            command.Parameters.AddWithValue("@izena", c.Izena);
            command.Parameters.AddWithValue("@abizenak", c.Abizenak);
            command.Parameters.AddWithValue("@tel", c.Telefonoa ?? "");
            command.Parameters.AddWithValue("@email", c.Emaila ?? "");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Bezero baten datu pertsonalak eguneratzen ditu bere NANaren arabera.
        /// </summary>
        /// <param name="c">Datu eguneratuak dituen bezero objektua.</param>
        public void ActualizarCliente(Bezeroa c)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Bezeroak 
                SET Izena = @izena, Abizenak = @abizenak, Telefonoa = @tel, Emaila = @email
                WHERE NAN = @nan;";
            command.Parameters.AddWithValue("@nan", c.NAN);
            command.Parameters.AddWithValue("@izena", c.Izena);
            command.Parameters.AddWithValue("@abizenak", c.Abizenak);
            command.Parameters.AddWithValue("@tel", c.Telefonoa ?? "");
            command.Parameters.AddWithValue("@email", c.Emaila ?? "");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Bezero bat datu-basetik ezabatzen du bere NAN zenbakia erabiliz.
        /// </summary>
        /// <param name="nan">Ezabatu beharreko bezeroaren NAN zenbakia.</param>
        public void EliminarCliente(string nan)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Bezeroak WHERE NAN = @nan;";
            command.Parameters.AddWithValue("@nan", nan);
            command.ExecuteNonQuery();
        }

        // ==========================================
        // ALOKAIRUAK (ALQUILERES)
        // ==========================================

        /// <summary>
        /// Egindako alokairu guztien zerrenda osoa eskuratzen du, ibilgailuaren marka eta modeloarekin bateratuta.
        /// </summary>
        /// <returns>Alokairua motako objektuen zerrenda.</returns>
        public List<Alokairua> GetTodosLosAlquileres()
        {
            var list = new List<Alokairua>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();

            // Alokairuak eta Ibilgailuak taulak elkartzen ditu datu osoak lortzeko
            command.CommandText = @"
                SELECT a.Id, a.BezeroaNAN, a.IbilgailuaMatrikula, a.HasieraData, a.AmaieraData, a.PrezioOsoa, i.Marka, i.Modeloa 
                FROM Alokairuak a JOIN Ibilgailuak i ON a.IbilgailuaMatrikula = i.Matrikula;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Alokairua
                {
                    Id = reader.GetInt32(0),
                    BezeroaNAN = reader.GetString(1),
                    IbilgailuaMatrikula = reader.GetString(2),
                    HasieraData = reader.GetString(3),
                    AmaieraData = reader.GetString(4),
                    PrezioOsoa = reader.GetDouble(5),
                    InfoVehiculo = $"{reader.GetString(6)} {reader.GetString(7)} ({reader.GetString(2)})"
                });
            }
            return list;
        }

        /// <summary>
        /// Bezero jakin batek egindako alokairuen historiala eskuratzen du bere NANaren arabera.
        /// </summary>
        /// <param name="nan">Bezeroaren NAN zenbakia.</param>
        /// <returns>Bezero horren alokairuen zerrenda.</returns>
        public List<Alokairua> GetAlquileresDeCliente(string nan)
        {
            var list = new List<Alokairua>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT a.Id, a.BezeroaNAN, a.IbilgailuaMatrikula, a.HasieraData, a.AmaieraData, a.PrezioOsoa, i.Marka, i.Modeloa 
                FROM Alokairuak a JOIN Ibilgailuak i ON a.IbilgailuaMatrikula = i.Matrikula WHERE a.BezeroaNAN = @nan;";
            command.Parameters.AddWithValue("@nan", nan);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Alokairua
                {
                    Id = reader.GetInt32(0),
                    BezeroaNAN = reader.GetString(1),
                    IbilgailuaMatrikula = reader.GetString(2),
                    HasieraData = reader.GetString(3),
                    AmaieraData = reader.GetString(4),
                    PrezioOsoa = reader.GetDouble(5),
                    InfoVehiculo = $"{reader.GetString(6)} {reader.GetString(7)} ({reader.GetString(2)})"
                });
            }
            return list;
        }

        /// <summary>
        /// Alokairu berri bat erregistratzen du datu-basean eta alokatutako ibilgailuaren egoera 'Alokatuta' gisa eguneratzen du.
        /// </summary>
        /// <param name="a">Gorde beharreko alokairuaren datuak dituen objektua.</param>
        public void InsertarAlquiler(Alokairua a)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Alokairuak (BezeroaNAN, IbilgailuaMatrikula, HasieraData, AmaieraData, PrezioOsoa)
                VALUES (@nan, @mat, @hasiera, @amaiera, @prezioa);
                UPDATE Ibilgailuak SET Egoera = 'Alokatuta' WHERE Matrikula = @mat;";
            command.Parameters.AddWithValue("@nan", a.BezeroaNAN);
            command.Parameters.AddWithValue("@mat", a.IbilgailuaMatrikula);
            command.Parameters.AddWithValue("@hasiera", a.HasieraData);
            command.Parameters.AddWithValue("@amaiera", a.AmaieraData);
            command.Parameters.AddWithValue("@prezioa", a.PrezioOsoa);
            command.ExecuteNonQuery();
        }
    }
}