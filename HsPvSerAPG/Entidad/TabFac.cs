using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    using HsPvSerAPG.Utilis;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class TabFac : INotifyPropertyChanged

    {
        // Cabecera
        public int cia { get; set; }

        

        [JsonProperty("tipdoc")]
        public int Tipdoc { get; set; }

        [JsonProperty("dessuc")]
        public string Dessuc { get; set; }

        [JsonProperty("desdoc")]
        public string Desdoc { get; set; }

        public string nroser { get; set; }


        [JsonProperty("numdoc")]
        public string Numdoc { get; set; }


        [JsonProperty("codigo")]
        public int codigo { get; set; }

        [JsonProperty("razsoc")]
        public string Razsoc { get; set; }
        public int docref { get; set; }
        public string numref { get; set; }


        [JsonProperty("fecha")]
        public string Fecha { get; set; }     
        

        public string fecven { get; set; }      
        public int tiponc { get; set; }


        [JsonProperty("tipmon")]
        public int Tipmon { get; set; }

        public string MonedaDescripcion
        {
            get
            {
                switch (Tipmon)
                {
                    case 1:
                        return "SOLES";
                    case 2:
                        return "DOLARES";
                    default:
                        return "DESCONOCIDO";
                }
            }
        }


        [JsonProperty("desmon")]
        public string Desmon { get; set; }

        [JsonProperty("desabrmon")]
        public string Desabrmon { get; set; }


        public int codven { get; set; }
        public int forpag { get; set; }
        public int dias { get; set; }
        public string orden { get; set; }
        public string guias { get; set; }
        public string numpro { get; set; }


        [JsonProperty("bruto")]
        public float Bruto { get; set; }

        public float descto { get; set; }


        [JsonProperty("igv")]
        public float Igv { get; set; }

        [JsonProperty("total")]
        public float Total { get; set; }


        public float flete { get; set; }
        public float seguro { get; set; }

        public int st { get; set; } = 0;

        public int stentregado { get; set; }

        public int tipven { get; set; }
        public int codusr { get; set; }
        public int docnp { get; set; }
        public string numnp { get; set; }
        public float saldocredito { get; set; }
        public int coddetrac { get; set; }
        public float pordetrac { get; set; }
        public float impdetracd { get; set; }
        public float impdetracs { get; set; }
        public int chkretencion { get; set; }
        public float porretencion { get; set; }
        public float impretencd { get; set; }
        public float impretencs { get; set; }
        public int stanticipo { get; set; }


        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        [JsonIgnore]
        public decimal SaldoOriginal { get; set; }


        [JsonProperty("tipcam")]
        public float Tipcam { get; set; }

        public decimal Importe
        {
            get
            {
                if (Tipmon == 1) // Soles
                {
                    return Saldo;
                }
                else if (Tipmon == 2)
                {
                    return Math.Round(Saldo * (decimal)Tipcam, 2);
                }

                return 0;
            }
            set
            {
                if (Tipmon == 1) // SOLES → asignar en USD significa guardar en soles
                {
                    Saldo = value;
                }
                else if (Tipmon == 2) // DOLARES → se asigna directo
                {
                    Saldo = Math.Round(value / (decimal)Tipcam, 2);
                }
            }
        }

        public decimal ImporteDolares
        {
            get
            {
                if (Tipmon == 1) // Soles
                {
                    return Saldo == 0.00m ? 0 : Math.Round(Saldo / (decimal)Tipcam, 2);
                }
                else if (Tipmon == 2)
                {
                    return Saldo;
                }

                return 0;
            }
            set
            {
                if (Tipmon == 1) // SOLES → asignar en USD significa guardar en soles
                {
                    Saldo = Math.Round(value * (decimal)Tipcam, 2);
                }
                else if (Tipmon == 2) // DOLARES → se asigna directo
                {
                    Saldo = value;
                }
            }
        }
        public decimal salant { get; set; }
        public string tipopago { get; set; } // Efectivo, Tarjeta, etc.

        private bool _isSelected;
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
        // Detalles
        public List<Detfac> detfacs { get; set; } = new List<Detfac>();
        public List<Detpago> detpagos { get; set; } = new List<Detpago>();
            

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

    public class Detfac
    {
        public int correl { get; set; }
        public string codlin { get; set; }
        public string codart { get; set; }
        public string desdet { get; set; }
        public float cantid { get; set; }
        public float prelis { get; set; }
        public float bruto { get; set; }
        public float precio { get; set; }
        public float desc1 { get; set; }
        public float descto { get; set; }
        public float neto { get; set; }
        public int tipfac { get; set; }
        public int codaut { get; set; }
        public int correl1 { get; set; }
        public int coduni { get; set; }
        public int sticbper { get; set; }
        public string tipopago { get; internal set; }
        public object tipmon { get; internal set; }
        

    }
    public class Detpago
    {
        public int ptipopago { get; set; }
        public float pimpsol { get; set; }
        public float pimpdol { get; set; }
        public float pvuesol { get; set; }
        public float pvuodol { get; set; } // 
        public int pcodbantar { get; set; }
        public string pnrocheope { get; set; } // 
        public string pfeccheope { get; set; }
        public string pfecdif { get; set; }
        public float ptipcam { get; set; }
        public string pcobobs { get; set; }
    }

    //PARA ACTUALIZAR 

    public class TabFacUpdateDto
    {
        public int cia { get; set; }
        public int tipdoc { get; set; }
        public string nroser { get; set; }
        public string numdoc { get; set; }
        public int codigo { get; set; }
        public string fecha { get; set; }
        public string fecven { get; set; }
        public string numref { get; set; }
        public int tipmon { get; set; }
        public float tipcam { get; set; }
        public int codven { get; set; }
        public float bruto { get; set; }
        public float igv { get; set; }
        public float total { get; set; }
        public int st { get; set; }
        public int stanticipo { get; set; }
        public float salant { get; set; }
        public List<DetFacDto> detfacs { get; set; }
    }

    public class DetFacDto
    {
        public int correl { get; set; }
        public string codlin { get; set; }
        public string codart { get; set; }
        public string desdet { get; set; }
        public double cantid { get; set; }
        public double prelis { get; set; }
        public double bruto { get; set; }
        public double precio { get; set; }
        public double neto { get; set; }
        public int tipfac { get; set; }
        public int coduni { get; set; }
    }


}
