using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using autoen_alokairua.Models;
using autoen_alokairua.Services;

namespace autoen_alokairua
{
    /* 
     * Bezeroak kudeatzeko orria.
     * Fitxategi honek bezeroen zerrenda erakusten du eta bezeroak bilatu, gehitu,
     * editatu, ezabatu eta euren alokairuak ikusteko UI gertaerak kudeatzen ditu.
     */
    public partial class Bezeroak : ContentPage
    {
        /* 
         * Klase mailako aldagaiak.
         * Datu-base zerbitzua, uneko hautatutako bezeroa eta edizio-egoera kontrolatzeko.
         */
        private readonly DatabaseService _dbService;
        private Bezeroa _bezeroaSeleccionado;
        private bool _isEditing = false;

        /// <summary>
        /// Eraikitzailea. Osagaiak hasieratzen ditu eta datu-base zerbitzuaren instantzia sortzen du.
        /// </summary>
        public Bezeroak()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        /// <summary>
        /// Orria pantailan agertzean exekutatzen da, bezeroen zerrenda eguneratuz.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarBezeroak();
        }

        /// <summary>
        /// Datu-basetik bezero guztiak eskuratzen ditu eta pantailako zerrenda freskatzen du.
        /// </summary>
        private void CargarBezeroak()
        {
            listaBezeroak.ItemsSource = _dbService.GetClientes();
        }

        /// <summary>
        /// Bilatzailearen testua aldatzean exekutatzen da. Zerrenda iragazten du NANaren edo izen-abizenen arabera.
        /// </summary>
        /// <param name="sender">Gertaera abiarazi duen bilatzailea.</param>
        /// <param name="e">Testu aldaketaren argumentuak.</param>
        private void OnBuscarBezeroaChanged(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = e.NewTextValue?.ToLower() ?? "";
            var todosLosClientes = _dbService.GetClientes();

            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                listaBezeroak.ItemsSource = todosLosClientes;
            }
            else
            {
                // NANa edo izen osoa bilatutako testuarekin bat datozen bezeroak iragazi
                listaBezeroak.ItemsSource = todosLosClientes.Where(c =>
                    (c.NAN != null && c.NAN.ToLower().Contains(textoBusqueda)) ||
                    (c.NombreCompleto != null && c.NombreCompleto.ToLower().Contains(textoBusqueda))
                ).ToList();
            }
        }

        /// <summary>
        /// Zerrendako bezero bat hautatzean exekutatzen da. Bere datu pertsonalak eta alokairuen historiala kargatzen ditu.
        /// </summary>
        /// <param name="sender">Zerrenda kontrola.</param>
        /// <param name="e">Hautapen aldaketaren argumentuak.</param>
        private void OnBezeroaSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            _bezeroaSeleccionado = e.CurrentSelection.FirstOrDefault() as Bezeroa;

            if (_bezeroaSeleccionado != null)
            {
                // Bezeroaren oinarrizko datuak etiketetan erakutsi
                lblNan.Text = _bezeroaSeleccionado.NAN;
                lblIzenAbizenak.Text = _bezeroaSeleccionado.Izena;
                lblTelefonoa.Text = _bezeroaSeleccionado.Telefonoa;
                lblEmaila.Text = _bezeroaSeleccionado.Emaila;

                // Bezero honi dagozkion alokairuak bilatu eta kargatu
                var todosLosAlquileres = _dbService.GetTodosLosAlquileres();
                var alquileresDelCliente = todosLosAlquileres.Where(a => a.BezeroaNAN == _bezeroaSeleccionado.NAN).ToList();

                if (alquileresDelCliente.Any())
                {
                    listaAlquileresCliente.ItemsSource = alquileresDelCliente;
                    listaAlquileresCliente.IsVisible = true;
                    lblSinAlquileres.IsVisible = false;
                }
                else
                {
                    listaAlquileresCliente.ItemsSource = null;
                    listaAlquileresCliente.IsVisible = false;
                    lblSinAlquileres.IsVisible = true;
                }

                RestaurarModoLectura();
            }
            else
            {
                // Hautapenik ez badago, eremuak lehenetsitako balioetara itzuli
                lblNan.Text = "---";
                lblIzenAbizenak.Text = "---";
                lblTelefonoa.Text = "---";
                lblEmaila.Text = "---";

                listaAlquileresCliente.ItemsSource = null;
                listaAlquileresCliente.IsVisible = false;
                lblSinAlquileres.IsVisible = true;

                RestaurarModoLectura();
            }
        }

        // --- BEZERO BERRIA GEHITU ---

        /// <summary>
        /// Bezero berria gehitzeko leihoa edo formularioa kudeatzen du.
        /// </summary>
        /// <param name="sender">Gehitu botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private void OnGehituBezeroaClicked(object sender, EventArgs e)
        {
            DisplayAlert("Gehitu", "Bezero berria gehitzeko leihoa (hemen sortu behar da logika).", "Ados");
        }

        // --- BEZEROA EDITATU (LERROAN) ---

        /// <summary>
        /// Hautatutako bezeroaren edizio-modua aktibatzen du, edo egindako aldaketak datu-basean gordetzen ditu.
        /// </summary>
        /// <param name="sender">Editatu/Gorde botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private async void OnEditatuBezeroaClicked(object sender, EventArgs e)
        {
            if (_bezeroaSeleccionado == null)
            {
                await DisplayAlert("Abisua", "Mesedez, aukeratu bezero bat editatzeko.", "Ados");
                return;
            }

            if (!_isEditing)
            {
                // Edizio-moduan sartu: testu-eremuak bete eta ikusgai jarri
                _isEditing = true;
                btnEditatu.Text = "Gorde";
                btnEditatu.BackgroundColor = Color.FromArgb("#28A745");

                entryNan.Text = _bezeroaSeleccionado.NAN;
                entryIzenAbizenak.Text = _bezeroaSeleccionado.Izena;
                entryTelefonoa.Text = _bezeroaSeleccionado.Telefonoa;
                entryEmaila.Text = _bezeroaSeleccionado.Emaila;

                AlternarVisibilidadEdicion(true);
            }
            else
            {
                // Aldaketak objektuan gorde
                _bezeroaSeleccionado.NAN = entryNan.Text;

                // Testu-kaxako edukia garbitu eta hitzetan banatu errepikapenak saihesteko
                string inputTexto = entryIzenAbizenak.Text?.Trim() ?? "";
                var hitzak = inputTexto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (hitzak.Length >= 2)
                {
                    _bezeroaSeleccionado.Izena = $"{hitzak[0]} {hitzak[1]}";
                }
                else if (hitzak.Length == 1)
                {
                    _bezeroaSeleccionado.Izena = hitzak[0];
                }

                _bezeroaSeleccionado.Telefonoa = entryTelefonoa.Text;
                _bezeroaSeleccionado.Emaila = entryEmaila.Text;

                try
                {
                    // Datu-basean bezeroaren erregistroa eguneratu
                    _dbService.ActualizarCliente(_bezeroaSeleccionado);

                    // Etiketak datu berriekin freskatu
                    lblNan.Text = _bezeroaSeleccionado.NAN;
                    lblIzenAbizenak.Text = _bezeroaSeleccionado.NombreCompleto;
                    lblTelefonoa.Text = _bezeroaSeleccionado.Telefonoa;
                    lblEmaila.Text = _bezeroaSeleccionado.Emaila;

                    RestaurarModoLectura();
                    CargarBezeroak();

                    await DisplayAlert("Ondo", "Datuak behar bezala gorde dira.", "Ados");
                }
                catch
                {
                    await DisplayAlert("Errorea", "Ezin izan dira datuak gorde.", "Ados");
                }
            }
        }

        // --- BEZEROA EZABATU ---

        /// <summary>
        /// Erabiltzaileari baiezkoa eskatu ondoren, hautatutako bezeroa datu-basetik ezabatzen du.
        /// </summary>
        /// <param name="sender">Ezabatu botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private async void OnEzabatuBezeroaClicked(object sender, EventArgs e)
        {
            if (_bezeroaSeleccionado == null) return;

            bool confirmacion = await DisplayAlert("Ezabatu", $"Ziur zaude {_bezeroaSeleccionado.NombreCompleto} ezabatu nahi duzula?", "Bai", "Ez");

            if (confirmacion)
            {
                _dbService.EliminarCliente(_bezeroaSeleccionado.NAN);

                listaBezeroak.SelectedItem = null;
                CargarBezeroak();
                await DisplayAlert("Ondo", "Bezeroa ondo ezabatu da.", "Ados");
            }
        }

        // --- LAGUNTZAILE METODOAK (UI EGOERA) ---

        /// <summary>
        /// Interfazea irakurketa modura itzultzen du, Editatu botoia leheneratuz.
        /// </summary>
        private void RestaurarModoLectura()
        {
            _isEditing = false;
            btnEditatu.Text = "Editatu";
            btnEditatu.BackgroundColor = Color.FromArgb("#F59E0B");
            AlternarVisibilidadEdicion(false);
        }

        /// <summary>
        /// Etiketen (Label) eta sarrera-eremuen (Entry) ikusgaitasuna txandakatzen du edizio-egoeraren arabera.
        /// </summary>
        /// <param name="enEdicion">True edizio-modua aktibatzeko, False irakurketa-modurako.</param>
        private void AlternarVisibilidadEdicion(bool enEdicion)
        {
            lblNan.IsVisible = !enEdicion;
            entryNan.IsVisible = enEdicion;

            lblIzenAbizenak.IsVisible = !enEdicion;
            entryIzenAbizenak.IsVisible = enEdicion;

            lblTelefonoa.IsVisible = !enEdicion;
            entryTelefonoa.IsVisible = enEdicion;

            lblEmaila.IsVisible = !enEdicion;
            entryEmaila.IsVisible = enEdicion;
        }
    }
}