using HsPvSerAPG.Utilis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace HsPvSerAPG.Entidad
{
    public class Producto : INotifyPropertyChanged
    {
        public string codigo;
        public string codfab;
        public string descripcion;
        public string umd;
        public decimal cant;
        public decimal unit;
        public double total;
        public double d;
        public string codart;
        public int coduni;
        

        public int stanticipo { get; set; }

        public decimal salant { get; set; }
        public BitmapImage Imagen { get; set; }
        public decimal UnitSoles { get; set; }
        public decimal UnitDolares { get; set; }
        public int TipMonOriginal { get; set; }

        public int MonedaEditada { get; set; }
        public bool Convertido { get; set; } = false;
        public string Codigo
        {
            get => codigo;
            set { codigo = value; OnPropertyChanged(); }
        }

        public string CodFab
        {
            get => codfab;
            set { codfab = value; OnPropertyChanged(); }
        }

        public string Descripcion
        {
            get => descripcion;
            set { descripcion = value; OnPropertyChanged(); }
        }

        public string UMD { get; set; } = "";

        public decimal Cant
        {
            get => cant;
            set
            {
                if (cant != value)
                {
                    cant = value;
                    OnPropertyChanged(nameof(Cant));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public decimal Unit
        {
            get => unit;
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChanged(nameof(Unit));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public decimal UnitValorOriginal { get; set; } = 0;
        public decimal PrecioBaseSoles { get; set; } = 0;
        public decimal PrecioBaseDolares { get; set; } = 0;
        
        public decimal PrecioOriginal { get; set; } 

        public double D { get; set; } = 0;

        public string Codart {
            get => codart;
            set { codart = value; OnPropertyChanged(); }
        }

        public int Coduni
        {
            get => coduni;
            set { coduni = value; OnPropertyChanged(); }
        }

        public decimal Total => Math.Round(Cant * Unit, 2);

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int esanti { get; set; }
    }
}
