namespace autoen_alokairua.Models
{
    public class Alokairua
    {
        public int Id { get; set; }
        public string BezeroaNAN { get; set; }
        public string IbilgailuaMatrikula { get; set; }
        public string HasieraData { get; set; }
        public string AmaieraData { get; set; }
        public double PrezioOsoa { get; set; }

        public string NombreCliente { get; set; }
        public string InfoVehiculo { get; set; }
        public string RangoFechas => $"{HasieraData} → {AmaieraData}";
    }
}