using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HsPvSerAPG.Vista.Reenviar.Anticipo
{
    /// <summary>
    /// Lógica de interacción para EmisionNotaPedido.xaml
    /// </summary>
    public partial class EmisionNotaPedido : Window
    {
        private string primerosCuatro, resto;

        private ConsultaImpController consultaImpController = new ConsultaImpController();
        private TabFacAnticiposController tabFacAnticiposController = new TabFacAnticiposController();
        public Anticipo anticipo = new Anticipo();

        public List<Anticipo> lista_anticipos = new List<Anticipo>();

        private List<DocApli> docapli_actual = new List<DocApli>();
        public TabFac Tipmon { get; set; }

        public bool TienAnticipo = false;
        public EmisionNotaPedido(TabFac tabFac)
        {
            InitializeComponent();
            Tipmon = tabFac;
            string desdoc = tabFac.Numdoc ?? string.Empty;
            primerosCuatro = desdoc.Length >= 4 ? desdoc.Substring(0, 4) : desdoc;
            resto = desdoc.Length > 4 ? desdoc.Substring(4) : string.Empty;
            

            var consulta = consultaImpController.consultaImp(sisVariables.GCia, 41, tabFac.Numdoc);
            if (consulta == null)
            {
                MessageBox.Show("El documento no tiene anticipos", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var anticipos = tabFacAnticiposController.selectTabfacAnticipos(sisVariables.GCia, consulta.codigo);

            DGEmision.ItemsSource = anticipos;

            NumDoc.Text = primerosCuatro;
            Doc.Text = resto;
            TienAnticipo = true;

        }

        private void Click_BtnBuscar(object sender, RoutedEventArgs e)
        {
            var consulta = consultaImpController.consultaImp(sisVariables.GCia, 41, NumDoc.Text + Doc.Text);

            if (consulta == null)
            {
                MessageBox.Show("Error al buscar (se cerrará la ventana)", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close(); 
                return;
            }

            var anticipos = tabFacAnticiposController.selectTabfacAnticipos(sisVariables.GCia, consulta.codigo);

            if (anticipos == null || anticipos.Count == 0)
                MessageBox.Show("No se encontraron anticipos para el documento ingresado.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            DGEmision.ItemsSource = anticipos;
        }

        private void DGEmision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGEmision.SelectedItem is TabFacAnticipos anticipo_seleccionado)
            {
                
                primerosCuatro = anticipo_seleccionado.numdoc.Length >= 4 ? anticipo_seleccionado.numdoc.Substring(0, 4) : anticipo_seleccionado.numdoc;
                resto = anticipo_seleccionado.numdoc.Length > 4 ? anticipo_seleccionado.numdoc.Substring(4) : string.Empty;
                anticipo.TipDoc = primerosCuatro ?? string.Empty;
                anticipo.NumDoc = resto ?? string.Empty;
                anticipo.Saldo = Convert.ToDecimal(anticipo_seleccionado.salant);
                DocApliController docApliController = new DocApliController();

                var docapli = docApliController.selectDocumentosAplicadosAntiByNumdoc(sisVariables.GCia, anticipo_seleccionado.tipdoc, anticipo_seleccionado.numdoc);
                try
                {
                    DGDocApli.ItemsSource = docapli;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    throw;
                }
                if (docapli == null || docapli.Count == 0)
                {
                    return;
                }
                //Hallamos el valor maximo de los totales
                var valor_total = docapli.Select(p => p.total).Max();


                ////Logica para restar los totales iguales
                //bool switcher = false;
                //foreach (var item in docapli)
                //{
                //    if (switcher)
                //    {
                //        if (item.total == valor_total)
                //        {
                //            switcher = true;
                //            return;
                //        }

                //        valor_total = item.total;
                //    }
                //}

                anticipo.Saldo = anticipo.Saldo;
            }
        }

        private void DG_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void DGEmision_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Return) && DGEmision.SelectedItem != null)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void DG_MouseDoubleClick(object sender, MouseButtonEventArgs e) => this.Close();
    }

    public class Anticipo
    {
        public string TipDoc { get; set; } = string.Empty;
        public string NumDoc { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}
