using HsPvSerAPG.Utilis;
using HsPvSerAPG.Vista.Doc_Venta.Boleta;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Utils.Clases
{
    public class PagoCalculo : INotifyPropertyChanged
    {
        private string tipoPago;
        private double importe;
        private string detalle;
        private string tarjeta;
        private string nrolote;
        private string nrooperacion;
        private string banco;
        private string fechaoperacion;
        private int codigopago;
        private int moneda;
        private decimal tipcam;
        public string TipoPago
        {
            get => tipoPago;
            set { tipoPago = value; OnPropertyChanged(); }
        }
        public double Importe { get; set; }


        public double ImporteEnSoles
        {
            get
            {
                if (Moneda == 1) return Importe;
                if (Moneda == 2) return Math.Round(Importe * (double)sisVariables.Gtipcam, 6);
                return Importe;
            }
        }
        public int Moneda
        {
            get => moneda;
            set { moneda = value; OnPropertyChanged(nameof(Moneda)); }

            //set
            //{
            //    if (_moneda != value)
            //    {
            //        _moneda = value;
            //        OnPropertyChanged(nameof(Moneda));
            //        OnPropertyChanged(nameof(ImporteEnSoles)); // 👈 refresca el importe convertido
            //    }
            //}
        }

        public string Detalle
        {
            get => detalle;
            set { detalle = value; OnPropertyChanged(); }
        }

        public string Tarjeta
        {
            get => tarjeta;
            set { tarjeta = value; OnPropertyChanged(); }
        }

        public string NroLote
        {
            get => nrolote;
            set { nrolote = value; OnPropertyChanged(); }
        }

        public string NrOperacion
        {
            get => nrooperacion;
            set { nrooperacion = value; OnPropertyChanged(); }
        }
        public string Banco
        {
            get => banco;
            set { banco = value; OnPropertyChanged(); }
        }
        public string FechaOperacion
        {
            get => fechaoperacion;
            set { fechaoperacion = value; OnPropertyChanged(); }
        }

        public decimal TipCam
        {
            get => tipcam;
            set { tipcam = value; OnPropertyChanged(); }
        }
        public int Codigo
        {
            get => codigopago;
            set { codigopago = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
