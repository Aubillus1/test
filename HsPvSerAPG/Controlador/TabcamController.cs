using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Windows;
using System.Linq;
using System.Collections.Generic;


namespace HsPvSerAPG.Controlador
{

    public class TabcamController
    {
        private static readonly HttpClient client = new HttpClient();
        public bool MonedaEnDolares { get; private set; } = false;
        private bool monedaEnDolares = false;
        private int monedaActual = 1;

        public Tabcam  SelectTabcam()
        {
            string url = sisVariables.GAPI + "selectTabcam";
            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string responseJson = response.Content.ReadAsStringAsync().Result;
                var tabcam = JsonConvert.DeserializeObject<Tabcam>(responseJson);

                return tabcam;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consumir API Tabcam: {ex.Message}");
                return null;
            }
        }

        public void SetMoneda(int moneda)
        {
            monedaActual = moneda;
            MonedaEnDolares = (moneda == 2);
        }

        public void CambiarMonedaPorSeleccion(IEnumerable<Producto> productos, int moneda, decimal parale)
        {
            if (productos == null || !productos.Any())
                return;

            monedaActual = moneda;
            bool MonedaEnDolares = (moneda == 2);

            foreach (var prod in productos)
            {
                if (MonedaEnDolares)
                {
                    // Usar directamente el valor calculado en dólares
                    prod.Unit = prod.UnitDolares;
                }
                else
                {
                    // Usar directamente el valor calculado en soles
                    prod.Unit = prod.UnitSoles;
                }

                prod.OnPropertyChanged(nameof(prod.Unit));
                prod.OnPropertyChanged(nameof(prod.Total));
            }
        }

    }
}