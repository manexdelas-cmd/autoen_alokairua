using System;
using System.Linq;
using Microsoft.Maui.Controls;
using autoen_alokairua.Models;
using autoen_alokairua.Services;

namespace autoen_alokairua
{
    public partial class MainPage : ContentPage
    {
        /* 
         * Klase mailako aldagaiak.
         * Datu-base zerbitzua, uneko hautatutako ibilgailua, edizio egoera eta eskuragarritasun iragazkia kontrolatzeko.
         */
        private readonly DatabaseService _dbService;
        private Autoak _autoaSeleccionado;
        private bool _isEditing = false;
        private bool _soloEskuragarri = false;

        /// <summary>
        /// Eraikitzailea. Osagaiak hasieratzen ditu eta datu-base zerbitzuaren instantzia sortzen du.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        /// <summary>
        /// Orria pantailan agertzean exekutatzen da, ibilgailuen zerrenda eguneratuz.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarAutoak();
        }

        /// <summary>
        /// Datu-basetik ibilgailuak eskuratzen ditu eta aktibo dauden iragazkiak aplikatuz zerrenda freskatzen du.
        /// </summary>
        private void CargarAutoak()
        {
            AplicarFiltros();
        }

        /// <summary>
        /// Eskuragarritasun iragazkia eta testu-bilatzailea konbinatzen ditu zerrenda eguneratzeko.
        /// </summary>
        private void AplicarFiltros()
        {
            string textoBusqueda = searchautoa?.Text?.ToLower() ?? "";
            var resultados = _dbService.GetVehiculos().AsEnumerable();

            // 1. Eskuragarri daudenak bakarrik iragazi botoia aktibatuta badago
            if (_soloEskuragarri)
            {
                resultados = resultados.Where(c => c.Egoera == "Eskuragarri");
            }

            // 2. Matrikula, marka edo modeloa bilatutako testuarekin bat datozen ibilgailuak iragazi
            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                resultados = resultados.Where(c =>
                    (c.Matrikula != null && c.Matrikula.ToLower().Contains(textoBusqueda)) ||
                    (c.Marka != null && c.Marka.ToLower().Contains(textoBusqueda)) ||
                    (c.Modeloa != null && c.Modeloa.ToLower().Contains(textoBusqueda))
                );
            }

            listaautoas.ItemsSource = null;
            listaautoas.ItemsSource = resultados.ToList();
        }

        /// <summary>
        /// Eskuragarri dauden ibilgailuak bakarrik erakusteko iragazki botoia sakatzean exekutatzen da.
        /// </summary>
        /// <param name="sender">Iragazki botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private void OnFiltroEskuragarriClicked(object sender, EventArgs e)
        {
            _soloEskuragarri = !_soloEskuragarri;

            if (_soloEskuragarri)
            {
                btnFiltroEskuragarri.Text = "✓ Bakarrik Eskuragarri";
                btnFiltroEskuragarri.BackgroundColor = Color.FromArgb("#15803D");
                btnFiltroEskuragarri.TextColor = Colors.White;
            }
            else
            {
                btnFiltroEskuragarri.Text = "Iragazi: Guztiak erakutsi";
                btnFiltroEskuragarri.BackgroundColor = Color.FromArgb("#E2E8F0");
                btnFiltroEskuragarri.TextColor = Color.FromArgb("#334155");
            }

            AplicarFiltros();
        }

        /// <summary>
        /// Bilatzailearen testua aldatzean exekutatzen da. Zerrenda iragazten du matrikularen, markaren edo modeloaren arabera.
        /// </summary>
        /// <param name="sender">Gertaera abiarazi duen bilatzailea.</param>
        /// <param name="e">Testu aldaketaren argumentuak.</param>
        private void OnBuscarAutoaChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        /// <summary>
        /// Zerrendako ibilgailu bat hautatzean exekutatzen da. Bere datuak eskuineko panelean kargatzen ditu.
        /// </summary>
        /// <param name="sender">Zerrenda kontrola.</param>
        /// <param name="e">Hautapen aldaketaren argumentuak.</param>
        private void OnAutoaSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            _autoaSeleccionado = e.CurrentSelection.FirstOrDefault() as Autoak;

            if (_autoaSeleccionado != null)
            {
                lblTituloautoa.Text = $"{_autoaSeleccionado.Marka} {_autoaSeleccionado.Modeloa}".Trim();
                lblMatricula.Text = _autoaSeleccionado.Matrikula;
                lblMarka.Text = _autoaSeleccionado.Marka;
                lblModeloa.Text = _autoaSeleccionado.Modeloa;
                lblUrtea.Text = _autoaSeleccionado.Urtea.ToString();
                lblErregaia.Text = _autoaSeleccionado.Erregaia;
                lblPrecio.Text = _autoaSeleccionado.EgunekoPrezioa.ToString();
                switchEskuragarri.IsToggled = (_autoaSeleccionado.Egoera == "Eskuragarri");

                RestaurarModoLectura();
            }
            else
            {
                // Hautapenik ez badago, eremuak lehenetsitako balioetara itzuli
                lblTituloautoa.Text = "Aukeratu ibilgailu bat";
                lblMatricula.Text = "---";
                lblMarka.Text = "---";
                lblModeloa.Text = "---";
                lblUrtea.Text = "---";
                lblErregaia.Text = "---";
                lblPrecio.Text = "---";
                switchEskuragarri.IsToggled = false;

                RestaurarModoLectura();
            }
        }

        // --- IBILGAILU BERRIA GEHITU (MODALA) ---

        /// <summary>
        /// Ibilgailu berria gehitzeko leiho modala irekitzen du eta sarrera-eremuak garbitzen ditu.
        /// </summary>
        /// <param name="sender">Gehitu botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private void OnGehituAutoaClicked(object sender, EventArgs e)
        {
            entryBerriMatricula.Text = "";
            entryBerriMarka.Text = "";
            entryBerriModeloa.Text = "";
            entryBerriUrtea.Text = "";
            pickerBerriErregaia.SelectedIndex = -1;
            entryBerriPrecio.Text = "";

            vistaGehitu.IsVisible = true;
        }

        /// <summary>
        /// Ibilgailu berria gehitzeko leiho modala ezkutatzen du datuak gorde gabe.
        /// </summary>
        /// <param name="sender">Utzi botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private void OnUtziGehituClicked(object sender, EventArgs e)
        {
            vistaGehitu.IsVisible = false;
        }

        /// <summary>
        /// Formularioan sartutako datuak balioztatzen ditu eta ibilgailu berria datu-basean gordetzen du.
        /// </summary>
        /// <param name="sender">Gorde botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private async void OnGordeBerriaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entryBerriMatricula.Text))
            {
                await DisplayAlert("Abisua", "Matrikula jartzea derrigorrezkoa da.", "Ados");
                return;
            }

            int.TryParse(entryBerriUrtea.Text, out int urtea);
            double.TryParse(entryBerriPrecio.Text, out double prezioa);
            string erregaia = pickerBerriErregaia.SelectedItem?.ToString() ?? "Ezezaguna";

            var autoBerria = new Autoak
            {
                Matrikula = entryBerriMatricula.Text.Trim(),
                Marka = entryBerriMarka.Text?.Trim() ?? "",
                Modeloa = entryBerriModeloa.Text?.Trim() ?? "",
                Urtea = urtea,
                Erregaia = erregaia,
                EgunekoPrezioa = prezioa,
                Egoera = "Eskuragarri"
            };

            try
            {
                _dbService.InsertarVehiculo(autoBerria);

                vistaGehitu.IsVisible = false;
                CargarAutoak();

                await DisplayAlert("Ondo", "Ibilgailu berria ondo gorde da.", "Ados");
            }
            catch
            {
                await DisplayAlert("Errorea", "Ezin izan da ibilgailua gorde.", "Ados");
            }
        }

        // --- IBILGAILUA EDITATU (LERROAN) ---

        /// <summary>
        /// Hautatutako ibilgailuaren edizio modua aktibatzen du, edo aldaketak datu-basean gordetzen ditu.
        /// </summary>
        /// <param name="sender">Editatu/Gorde botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private async void OnEditatuAutoaClicked(object sender, EventArgs e)
        {
            if (_autoaSeleccionado == null)
            {
                await DisplayAlert("Abisua", "Mesedez, aukeratu ibilgailu bat editatzeko.", "Ados");
                return;
            }

            if (!_isEditing)
            {
                // Edizio modura pasa: testu-eremuak bete eta ikusgai jarri
                _isEditing = true;
                btnEditatu.Text = "Gorde";
                btnEditatu.BackgroundColor = Color.FromArgb("#28A745");

                entryMatricula.Text = _autoaSeleccionado.Matrikula;
                entryMarka.Text = _autoaSeleccionado.Marka;
                entryModeloa.Text = _autoaSeleccionado.Modeloa;
                entryUrtea.Text = _autoaSeleccionado.Urtea.ToString();
                entryErregaia.Text = _autoaSeleccionado.Erregaia;
                entryPrecio.Text = _autoaSeleccionado.EgunekoPrezioa.ToString();

                AlternarVisibilidadEdicion(true);
            }
            else
            {
                // Aldaketak objektuan gorde eta datu-basean eguneratu
                _autoaSeleccionado.Matrikula = entryMatricula.Text;
                _autoaSeleccionado.Marka = entryMarka.Text;
                _autoaSeleccionado.Modeloa = entryModeloa.Text;
                _autoaSeleccionado.Erregaia = entryErregaia.Text;
                _autoaSeleccionado.Egoera = switchEskuragarri.IsToggled ? "Eskuragarri" : "Alokatuta";

                if (int.TryParse(entryUrtea.Text, out int urtea))
                    _autoaSeleccionado.Urtea = urtea;

                if (double.TryParse(entryPrecio.Text, out double prezioa))
                    _autoaSeleccionado.EgunekoPrezioa = prezioa;

                try
                {
                    _dbService.ActualizarVehiculo(_autoaSeleccionado);

                    lblTituloautoa.Text = $"{_autoaSeleccionado.Marka} {_autoaSeleccionado.Modeloa}".Trim();
                    lblMatricula.Text = _autoaSeleccionado.Matrikula;
                    lblMarka.Text = _autoaSeleccionado.Marka;
                    lblModeloa.Text = _autoaSeleccionado.Modeloa;
                    lblUrtea.Text = _autoaSeleccionado.Urtea.ToString();
                    lblErregaia.Text = _autoaSeleccionado.Erregaia;
                    lblPrecio.Text = _autoaSeleccionado.EgunekoPrezioa.ToString();

                    RestaurarModoLectura();
                    CargarAutoak();

                    await DisplayAlert("Ondo", "Datuak behar bezala gorde dira.", "Ados");
                }
                catch
                {
                    await DisplayAlert("Errorea", "Ezin izan dira datuak gorde.", "Ados");
                }
            }
        }

        // --- IBILGAILUA EZABATU ---

        /// <summary>
        /// Erabiltzaileari baiezkoa eskatu ondoren, hautatutako ibilgailua datu-basetik ezabatzen du.
        /// </summary>
        /// <param name="sender">Ezabatu botoia.</param>
        /// <param name="e">Gertaeraren argumentuak.</param>
        private async void OnEzabatuAutoaClicked(object sender, EventArgs e)
        {
            if (_autoaSeleccionado == null) return;

            bool confirmacion = await DisplayAlert("Ezabatu", $"Ziur zaude {_autoaSeleccionado.Matrikula} ezabatu nahi duzula?", "Bai", "Ez");

            if (confirmacion)
            {
                _dbService.EliminarVehiculo(_autoaSeleccionado.Matrikula);

                listaautoas.SelectedItem = null;
                CargarAutoak();
                await DisplayAlert("Ondo", "Ibilgailua ondo ezabatu da.", "Ados");
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
        /// Etiketen (Label) eta sarrera-eremuen (Entry) ikusgaitasuna txandakatzen du edizio egoeraren arabera.
        /// </summary>
        /// <param name="enEdicion">True edizio modua aktibatzeko, False irakurketa modurako.</param>
        private void AlternarVisibilidadEdicion(bool enEdicion)
        {
            lblMatricula.IsVisible = !enEdicion;
            entryMatricula.IsVisible = enEdicion;

            lblMarka.IsVisible = !enEdicion;
            entryMarka.IsVisible = enEdicion;

            lblModeloa.IsVisible = !enEdicion;
            entryModeloa.IsVisible = enEdicion;

            lblUrtea.IsVisible = !enEdicion;
            entryUrtea.IsVisible = enEdicion;

            lblErregaia.IsVisible = !enEdicion;
            entryErregaia.IsVisible = enEdicion;

            lblPrecio.IsVisible = !enEdicion;
            entryPrecio.IsVisible = enEdicion;

            switchEskuragarri.IsEnabled = enEdicion;
        }
    }
}