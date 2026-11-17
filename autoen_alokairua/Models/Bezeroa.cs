namespace autoen_alokairua.Models
{
    /// <summary>
    /// Bezero baten datu-eredua adierazten duen klasea.
    /// Datu-baseko 'Bezeroak' taularen erregistroak eta interfazean erakusteko propietate laguntzaileak biltzen ditu.
    /// </summary>
    public class Bezeroa
    {
        // --- DATU-BASEKO OINARRIZKO EREMUAK ---

        /// <summary>
        /// Bezeroaren Nortasun Agiri Nazionala (gako nagusia).
        /// </summary>
        public string NAN { get; set; }

        /// <summary>
        /// Bezeroaren izena.
        /// </summary>
        public string Izena { get; set; }

        /// <summary>
        /// Bezeroaren abizenak.
        /// </summary>
        public string Abizenak { get; set; }

        /// <summary>
        /// Bezeroaren harremanetarako telefono zenbakia.
        /// </summary>
        public string Telefonoa { get; set; }

        /// <summary>
        /// Bezeroaren posta elektronikoaren helbidea.
        /// </summary>
        public string Emaila { get; set; }

        // --- INTERFAZERAKO (UI) PROPIETATE LAGUNTZAILEAK ---

        /// <summary>
        /// Bezeroaren izena eta abizenak bateratuta itzultzen dituen propietate kalkulatua.
        /// </summary>
        public string NombreCompleto => $"{Izena} {Abizenak}";
    }
}