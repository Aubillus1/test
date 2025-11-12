using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HsPvSerAPG.Entidad
{
    public class ConsultaImp
    {
        // ===== CABECERA =====
        [JsonProperty("tipdoc")]
        public int TipDoc { get; set; }

        [JsonProperty("codigo")]
        public int codigo { get; set; }

        [JsonProperty("dessuc")]
        public string dessuc { get; set; }

        [JsonProperty("desdoc")]
        public string desdoc { get; set; }

        [JsonProperty("numdoc")]
        public string NumDoc { get; set; }

        [JsonProperty("fecha")]
        public string Fecha { get; set; }

        [JsonProperty("desuni")]
        public string UDM { get; set; }

        [JsonProperty("codven")]
        public string codven { get; set; }
        [JsonProperty("desven")]
        public string desven { get; set; }

        [JsonProperty("razsoc")]
        public string RazonSocial { get; set; }

        [JsonProperty("bruto")]
        public decimal bruto { get; set; }

        [JsonProperty("igv")]
        public decimal igv { get; set; }

        [JsonProperty("tipmon")]
        public decimal TipMon { get; set; }

        [JsonProperty("total")]
        public decimal total { get; set; }

        public decimal importe
        {
            get => total;
            set => total = value;
        }

        public bool _isSelected { get; set; }

    }   

    public class DetalleDocumento : INotifyPropertyChanged
    {
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public string Fecha { get; set; }
        public string desuni { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadOriginal { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }

        public int Coduni { get; set; }


        private bool _isSelected;
        public int codven { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
