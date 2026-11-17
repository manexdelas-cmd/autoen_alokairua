using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace autoen_alokairua.Models
{
    /// <summary>
    /// Ibilgailu baten datu-eredua adierazten duen klasea.
    /// Datu-baseko 'Ibilgailuak' taularen erregistroak eta interfazean erakusteko propietate laguntzaileak biltzen ditu.
    /// </summary>
    public class Autoak
    {
        // --- DATU-BASEKO OINARRIZKO EREMUAK ---

        /// <summary>
        /// Ibilgailuaren matrikula bakarra (gako nagusia).
        /// </summary>
        public string Matrikula { get; set; }

        /// <summary>
        /// Ibilgailuaren fabrikatzailea edo marka (adibidez: Toyota, Seat).
        /// </summary>
        public string Marka { get; set; }

        /// <summary>
        /// Ibilgailuaren modelo zehatza (adibidez: Yaris, Leon).
        /// </summary>
        public string Modeloa { get; set; }

        /// <summary>
        /// Ibilgailuaren fabrikazio-urtea.
        /// </summary>
        public int Urtea { get; set; }

        /// <summary>
        /// Erabiltzen duen erregai mota (Diesel, Gasolina, Elektrikoa, Hibridoa).
        /// </summary>
        public string Erregaia { get; set; }

        /// <summary>
        /// Eguneko alokairu-prezioa eurotan.
        /// </summary>
        public double EgunekoPrezioa { get; set; }

        /// <summary>
        /// Ibilgailuaren uneko egoera ('Eskuragarri', 'Alokatuta', 'Mantentze-lanetan').
        /// </summary>
        public string Egoera { get; set; }

        // --- INTERFAZERAKO (UI) PROPIETATE LAGUNTZAILEAK ---

        /// <summary>
        /// Marka eta modeloa bateratuta itzultzen dituen propietate kalkulatua.
        /// </summary>
        public string NombreCompleto => $"{Marka} {Modeloa}";

        /// <summary>
        /// Ibilgailuaren egoera testu gisa XAML interfazean lotzeko (Binding) propietatea.
        /// </summary>
        public string TextoEstado => Egoera;

        /// <summary>
        /// Ibilgailuaren egoeraren araberako kolorea itzultzen du etiketetan erakusteko:
        /// Berdea ('Eskuragarri'), Gorria ('Alokatuta') edo Laranja ('Mantentze-lanetan').
        /// </summary>
        public Color ColorEstado => Egoera switch
        {
            "Eskuragarri" => Color.FromArgb("#28A745"), // Berdea
            "Alokatuta" => Color.FromArgb("#DC3545"),   // Gorria
            _ => Color.FromArgb("#D39E00")              // Laranja
        };
    }
}