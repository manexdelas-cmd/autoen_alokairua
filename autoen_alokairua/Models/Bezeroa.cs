namespace autoen_alokairua.Models
{
    public class Bezeroa
    {
        public string NAN { get; set; }
        public string Izena { get; set; }
        public string Abizenak { get; set; }
        public string Telefonoa { get; set; }
        public string Emaila { get; set; }

        public string NombreCompleto => $"{Izena} {Abizenak}";
    }
}