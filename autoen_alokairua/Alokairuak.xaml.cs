using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using autoen_alokairua.Models;
using autoen_alokairua.Services;

namespace autoen_alokairua
{
    /* 
     * Alokairuak kudeatzeko orri nagusia.
     * Fitxategi honek bezeroak eta ibilgailuak lotzen ditu alokairu berriak sortzeko,
     * prezioak automatikoki kalkulatuz eta datu-basean gordez.
     */

    public partial class Alokairuak : ContentPage
    {
        // Datu-basearekin komunikatzeko zerbitzua gordetzeko aldagaia
        private readonly DatabaseService _dbService;

        /// <summary>
        /// Alokairuak klasearen sortzailea (Constructor). 
        /// Osagaiak hasieratzen ditu eta hasierako gertaerak lotzen ditu.
        /// </summary>
        public Alokairuak()
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            // Hasiera eta amaiera daten gutxieneko muga gaurko eguna izatea ezartzen dugu
            dpInicio.MinimumDate = DateTime.Today;
            dpFin.MinimumDate = DateTime.Today;

            // Gertaerak (events) lotzen ditugu datak edo ibilgailua aldatzean prezioa berriro kalkulatzeko
            dpInicio.DateSelected += (s, e) => CalcularPrecio();
            dpFin.DateSelected += (s, e) => CalcularPrecio();
            pickerVehiculo.SelectedIndexChanged += (s, e) => CalcularPrecio();
        }

        /// <summary>
        /// Orria pantailaratzen den bakoitzean exekutatzen da.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarDatos();// Datuak freskatu orria irekitzean
        }

        /// <summary>
        /// Datu-basetik bezeroak, ibilgailu eskuragarriak eta alokairuen zerrenda kargatzen ditu.
        /// </summary>
        private void CargarDatos()
        {
            pickerCliente.ItemsSource = _dbService.GetClientes();
            pickerVehiculo.ItemsSource = _dbService.GetVehiculos().Where(v => v.Egoera == "Eskuragarri").ToList();
            listaAlquileres.ItemsSource = _dbService.GetTodosLosAlquileres();
        }

        /// <summary>
        /// Alokairuaren prezio osoa kalkulatzen du aukeratutako daten eta ibilgailuaren eguneko prezioaren arabera.
        /// </summary>
        private void CalcularPrecio()
        {
            /* 
                         * Erabiltzaileak hasiera-data amaiera-data baino geroago jartzen badu,
                         * amaiera-data automatikoki berdintzen da erroreak ekiditeko.
                         */

            if (dpInicio.Date > dpFin.Date)
            {
                dpFin.Date = dpInicio.Date;
            }

            if (pickerVehiculo.SelectedItem is Autoak vehiculo)
            {
                // Datak DateTime motara behartu CS1061 errorea saihesteko
                DateTime fechaInicio = (DateTime)dpInicio.Date;
                DateTime fechaFin = (DateTime)dpFin.Date;

                // Egun kopurua eta prezio osoa kalkulatu (gutxienez egun 1 kobratzen da)
                TimeSpan diferencia = fechaFin.Subtract(fechaInicio);
                int dias = diferencia.Days + 1;
                double precioTotal = dias * vehiculo.EgunekoPrezioa;

                lblPrecioCalculado.Text = $"Prezio osoa: {precioTotal:F2} €";
            }
            else
            {
                lblPrecioCalculado.Text = "Prezio osoa: 0.00 €";
            }
        }

        // --- ALOKAIRU BERRIA GORDE ---

        /// <summary>
        /// Hautatutako bezeroa, ibilgailua eta datak balioztatzen ditu, prezio osoa kalkulatzen du 
        /// eta alokairu berria datu-basean gordetzen du.
        /// </summary>
        /// <param name="sender">Alokairua gordetzeko botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private async void OnGordeAlokairuaClicked(object sender, EventArgs e)
        {
            // Bezeroa eta ibilgailua hautatuta daudela egiaztatu
            if (pickerCliente.SelectedItem is not Bezeroa bezeroa || pickerVehiculo.SelectedItem is not Autoak vehiculo)
            {
                await DisplayAlert("Abisua", "Aukeratu bezero bat eta ibilgailu bat.", "Ados");
                return;
            }

            // Datak DateTime motara behartu CS1061 errorea saihesteko
            DateTime fechaInicio = (DateTime)dpInicio.Date;
            DateTime fechaFin = (DateTime)dpFin.Date;

            // Alokairuaren iraupena eta zenbateko totala kalkulatu
            TimeSpan diferencia = fechaFin.Subtract(fechaInicio);
            int dias = diferencia.Days + 1;
            double precioTotal = dias * vehiculo.EgunekoPrezioa;

            // Alokairu objektu berria sortu datu-basean txertatzeko
            var nuevoAlquiler = new Alokairua
            {
                BezeroaNAN = bezeroa.NAN,
                IbilgailuaMatrikula = vehiculo.Matrikula,
                HasieraData = fechaInicio.ToString("yyyy-MM-dd"),
                AmaieraData = fechaFin.ToString("yyyy-MM-dd"),
                PrezioOsoa = precioTotal
            };

            try
            {
                // Datu-basean gorde (ibilgailuaren egoera ere 'Alokatuta' gisa eguneratuko da)
                _dbService.InsertarAlquiler(nuevoAlquiler);

                // Formularioa hasierako egoerara itzuli gorde ondoren
                pickerCliente.SelectedItem = null;
                pickerVehiculo.SelectedItem = null;
                dpInicio.Date = DateTime.Today;
                dpFin.Date = DateTime.Today;
                lblPrecioCalculado.Text = "Prezio osoa: 0.00 €";

                // Zerrendak eta datuak freskatu
                CargarDatos();
                await DisplayAlert("Ondo", "Alokairua behar bezala gorde da.", "Ados");
            }
            catch
            {
                await DisplayAlert("Errorea", "Ezin izan da alokairua gorde.", "Ados");
            }
        }
    }
}