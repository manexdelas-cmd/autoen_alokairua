using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace autoen_alokairua.Models
{
    public class Autoak
    {
        public string Matrikula { get; set; }
        public string Marka { get; set; }
        public string Modeloa { get; set; }
        public int Urtea { get; set; }
        public string Erregaia { get; set; }
        public double EgunekoPrezioa { get; set; }
        public string Egoera { get; set; } // 'Eskuragarri', 'Alokatuta', 'Mantentze-lanetan'

        public string NombreCompleto => $"{Marka} {Modeloa}";
        public string TextoEstado => Egoera; // Añadido para que el XAML muestre el estado

        public Color ColorEstado => Egoera switch
        {
            "Eskuragarri" => Color.FromArgb("#28A745"), // Berdea
            "Alokatuta" => Color.FromArgb("#DC3545"),   // Gorria
            _ => Color.FromArgb("#D39E00")              // Laranja
        };
    }
}