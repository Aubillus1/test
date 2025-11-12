using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Utils.Clases
{
    public class PagoViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PagoCalculo> Pagos { get; set; } = new ObservableCollection<PagoCalculo>();

        private double importeInicial;
        public double ImporteInicial
        {
            get => importeInicial;
            set
            {
                if (importeInicial != value)
                {
                    importeInicial = value;
                    RecalcularTotales();
                
                }
                
            }
        }

        private double tipoCambio = (double)sisVariables.Gtipcam;
        public double TipoCambio
        {
            get => tipoCambio;
            set
            {
                if (tipoCambio != value)
                {
                    tipoCambio = value;
                    RecalcularTotales();
                }
            }
        }

        //  Totales y cálculos
        public double ImporteInicialUSD => (sisVariables.Gtipmon == 1 ? ImporteInicial / TipoCambio : ImporteInicial);
        

        public double TotalPagos { get; private set; }
        public double TotalPagosUSD { get; private set; }

        public double Restante { get; private set; }
        public double RestanteUSD { get; private set; }

        public double VueltoSoles { get; private set; }
        public double VueltoUSD { get; private set; }

        public PagoViewModel(double importeInicial = 0)
        {
            ImporteInicial = importeInicial;
            Pagos.CollectionChanged += Pagos_CollectionChanged;
            RecalcularTotales(); // inicializar
        }

        private void Pagos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PagoCalculo p in e.NewItems)
                    p.PropertyChanged += Pago_PropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (PagoCalculo p in e.OldItems)
                    p.PropertyChanged -= Pago_PropertyChanged;
            }

            RecalcularTotales();
        }

        private void Pago_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PagoCalculo.Importe) || e.PropertyName == nameof(PagoCalculo.Moneda))
            {
                RecalcularTotales();
            }
        }

        // Método único que recalcula todo
        private void RecalcularTotales()
        {
            double totalSoles = 0;
            double totalUSD = 0;

            foreach (var p in Pagos)
            {
                // 🔹 Siempre tomamos ImporteEnSoles como referencia en soles
                totalSoles += p.ImporteEnSoles;
                totalUSD += p.ImporteEnSoles / TipoCambio;
            }

            TotalPagos = totalSoles;
            TotalPagosUSD = totalUSD;

            Restante = ImporteInicial - TotalPagos;
            RestanteUSD = ImporteInicialUSD - TotalPagosUSD;

            VueltoSoles = Restante < 0 ? Math.Abs(Restante) : 0;
            VueltoUSD = RestanteUSD < 0 ? Math.Abs(RestanteUSD) : 0;

            // 🔹 Notificamos TODO
            OnPropertyChanged(nameof(ImporteInicial));
            OnPropertyChanged(nameof(ImporteInicialUSD));
            OnPropertyChanged(nameof(TotalPagos));
            OnPropertyChanged(nameof(TotalPagosUSD));
            OnPropertyChanged(nameof(Restante));
            OnPropertyChanged(nameof(RestanteUSD));
            OnPropertyChanged(nameof(VueltoSoles));
            OnPropertyChanged(nameof(VueltoUSD));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
