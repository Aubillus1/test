using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Vista.Doc_Venta.Boleta;
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

namespace HsPvSerAPG
{
    /// <summary>
    /// Lógica de interacción para FormasDePago.xaml
    /// </summary>
    public partial class FormasDePago : Window
    {
        private string totalProductos;
        private string document_number;
        private string totalVenta;
        private string nroDoc;

        private string cliente;
        private string telefono;
        private string direccion;
        private int _codigoCliente;
        private int cantuni;
        private double igv;
        private int tipodoc;

        private List<Producto> productos;
        public FormasDePago(List<Producto> productos, string totalVenta, string nroDoc, string totalProductos, string cliente, string telefono, string direccion, int tipmon, float tipcam, int tipodoc)
        {
            InitializeComponent();
            this.totalVenta = totalVenta;
            this.nroDoc = nroDoc;
            this.productos = productos;
            this.cliente = cliente;
            this.telefono = telefono;
            this.direccion = direccion;
            this.totalProductos = totalProductos;
            this.tipodoc = tipodoc;
        }

        private void Button_Click(object sender, RoutedEventArgs e) => this.Close();


        private void Click_BtnEfectivo(object sender, RoutedEventArgs e)
        {
            //int tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;
            //float tipcam = sisVariables.Gtipcam > 0 ? (float)sisVariables.Gtipcam : 0f;
            //Efectivo efectivo = new Efectivo(productos, totalVenta, nroDoc, cliente, telefono, direccion, tipmon, tipcam);
            //efectivo.ShowDialog();

            //if (false)
            //{
            //    foreach (var prod in productos)
            //    {
            //        MessageBox.Show($"[Efectivo] Producto: {prod.Descripcion}\nCantidad: {prod.Cant}\nPrecio Unit: {prod.Unit}");
            //    } 
            //}
        }


        private void Click_BtnDetallar(object sender, RoutedEventArgs e)
        {
            //int tipmon = sisVariables.Gtipmon > 0 ? sisVariables.Gtipmon : 1;
            //float tipcam = sisVariables.Gtipcam > 0 ? (float)sisVariables.Gtipcam : 0f;
            //TipoDePago pago = new TipoDePago(productos, totalVenta, nroDoc, totalProductos, cliente, telefono, direccion, cantuni,igv,tipmon,tipcam, tipodoc);

            //pago.ShowDialog();
        }
    }
}
