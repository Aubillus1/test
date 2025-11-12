using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HsPvSerAPG.Vista.CajaChica
{
    public partial class RegistroIngreso : Window
    {
        private TabRCCController rccController = new TabRCCController();
        private ApiTabserService tabserService = new ApiTabserService();




        public RegistroIngreso(int registro)
        {
            InitializeComponent();
            RellenarDatos();
            this.Title = registro == 0 ? "Registro de Ingreso de Caja Chica" : "Registro de Egreso de Caja Chica";
            CB.SelectedIndex = registro;
            CargarMotivos(); // Llamada al API para llenar CBMotivo
            CargarSeriesAsync();
            CargarUltimoNumDocAsync();
        }

        private void RellenarDatos()
        {
            // Tipo de documento visual
            bool esIngreso = this.Title.Contains("Ingreso");
            CB.SelectedIndex = esIngreso ? 0 : 1; // 0 = INGRESO, 1 = EGRESO

            TBUsuario.Text = sisVariables.GUsuario;
            TBCaja.Text = sisVariables.GSucursal;
            TBFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // CMBserie y TextBlock se actualizarán después de cargar la serie y el nuevo número
        }

        private async void CargarUltimoNumDocAsync()
        {
            try
            {
                // Determinar tipdoc según si es ingreso o egreso
                int tipdoc = this.Title.Contains("Ingreso") ? 97 : 98;

                TBNumDoc.Text = "?";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando el último número de documento: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TBNumDoc.Text = "000000000000";
            }
        }


        private async void CargarSeriesAsync()
        {
            try
            {
                int cia = sisVariables.GCia;
                int codcaja = sisVariables.GCodCaja;
                int elec_tipdoc = 0; 
                int st_anti = 0;

                // Llamamos al servicio (actualmente síncrono, se ejecuta en un Task)
                var series = await Task.Run(() => tabserService.selectTabser(cia, codcaja, elec_tipdoc, st_anti));

                if (series != null && series.Count > 0)
                {
                    // Filtrar según si es ingreso o egreso
                    string filtroSerie = this.Title.Contains("Ingreso") ? "0001" : "";
                    var seriesFiltradas = series
                        .Where(s => s.nroser.EndsWith(filtroSerie))
                        .ToList();


                    if (seriesFiltradas.Count > 0)
                    {
                        CMBserie.ItemsSource = seriesFiltradas;
                        CMBserie.DisplayMemberPath = "nroser";  // Lo que se mostrará en el ComboBox
                        CMBserie.SelectedValuePath = "nroser";  // El valor que se obtiene al hacer SelectedValue
                        CMBserie.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron series disponibles para el tipo de documento.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("No se encontraron series disponibles.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando series: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CargarMotivos()
        {
            try
            {
                int sting = this.Title.Contains("Ingreso") ? 1 : 0; // Ingreso = 1, Egreso = 0
                int stegr = this.Title.Contains("Ingreso") ? 0 : 1; // Según tu tabla

                var motivos = await rccController.SelectMotivosAsync(sting, stegr);

                if (motivos != null && motivos.Count > 0)
                {
                    CBMotivo.ItemsSource = motivos;
                    CBMotivo.DisplayMemberPath = "des";
                    CBMotivo.SelectedValuePath = "cod";
                    CBMotivo.SelectedIndex = 0;
                    CBMotivo.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("No se encontraron motivos disponibles.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando motivos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void Click_BtnGrabar(object sender, RoutedEventArgs e)
        {
            if (CBMotivo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un motivo.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TBMonto.Text, out decimal monto))
            {
                MessageBox.Show("Ingrese un monto válido.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obtener tipmon como int: 1 = S/., 2 = $
            int tipMonInt = 1; // valor por defecto
            if (CBTipMon.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                tipMonInt = Convert.ToInt32(selectedItem.Tag);
            }

            bool esIngreso = this.Title.Contains("Ingreso");

            // Crear objeto JSON con nombres exactos en minúsculas
            var reciboJson = new
            {
                cia = 1,
                tipdoc = esIngreso ? "97" : "98",
                nroser = 1, // siempre 1 para ambos documentos
                numdoc = "0",
                fecha = DateTime.Parse(TBFecha.Text).ToString("yyyy-MM-dd"),
                razsoc = txtRAZSOC.Text, // Asignar cliente o destinatario si aplica
                tipmon = tipMonInt, // ahora es int: 1 o 2
                monto = monto,
                obs = TBDetalle.Text,
                codcaja = sisVariables.GCodCaja.ToString(),
                stanticipo = 0,
                codcli = "1"
            };

            // Enviar al API usando tu método existente
            bool exito = await rccController.InsertUpdateRCCAsync(reciboJson);

            if (exito)
            {
                MessageBox.Show("Recibo registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al registrar el recibo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Click_BtnSalir(object sender, RoutedEventArgs e) => this.Close();
    }
}
