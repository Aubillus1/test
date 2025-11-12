
﻿using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Vista;
﻿using System;
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
    /// Lógica de interacción para ConsultaDNI.xaml
    /// </summary>
    public partial class ConsultaDNI : Window
    {
        private int intentos = 0;
        private const int maxIntentos = 2;
        public ConsultaDNI()
        {
            InitializeComponent();
        }
        

        private void Btn_Salir(object sender, RoutedEventArgs e) => this.Close();

        private void btn_ConsultaReniec_Click(object sender, RoutedEventArgs e) => ConsultaDNIs();

        private void document_number_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter )
                ConsultaDNIs();
        }

        private async void Btn_Grabar(object sender, RoutedEventArgs e)
        {
            string dni = document_number.Text.Trim();

            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI válido.");
                return;
            }

            // Lógica de ejemplo: crear y guardar cliente usando CliproController
            CliproController controller = new CliproController();

            var clipro = new Clipro
            {

                nrodoc = dni,
                razsoc = $"{first_name.Text} {first_last_name.Text} {second_last_name.Text}",
                direcc = "", 
                          
                cia = sisVariables.GCia,
            };

            var clienteExistente = controller.SelectClipro(dni, sisVariables.GCia)?.FirstOrDefault();

            if (clienteExistente == null)
            {
                var resultado = await controller.RegistrarClipro(clipro);
                if (resultado != null)
                {
                    MessageBox.Show("Cliente registrado correctamente.", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al registrar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var resultado = await controller.RegistrarClipro(clipro);
                if (resultado != null)
                {
                    MessageBox.Show("Cliente actualizado correctamente.", "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al actualizar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ConsultaDNIs()
        {
            string dni = document_number.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un numero de DNI válido.");
                return;
            }
            if (dni.Length == 9) {
                MessageBox.Show("El documento a consultar debe tener minimo 8 digitos");
                return;
            
            } 

            ApiReniecController apiReniec = new ApiReniecController();
            var persona = apiReniec.consultarDNI(dni);

            if (persona != null && !(persona is IList<object>))
            {
                intentos = 0;

                first_last_name.Text = persona.first_last_name ?? "";
                second_last_name.Text = persona.second_last_name ?? "";
                first_name.Text = persona.first_name ?? "";
                document_number.Text = persona.document_number ?? dni;
            }

            else
            {
                intentos++;
                if (intentos < maxIntentos)
                {
                    MessageBox.Show($"El documento a consultar no existente. \nIntente otra vez o registre manualmente al cliente. \nIntento {intentos} de {maxIntentos}");
                    return;

                }
                else
                {
                    MessageBox.Show("El cliente a consultar no existe \nIngresalo manuealmente");
                    intentos = 0;
                    var nuevaVentana = new RegistroClipro();
                    nuevaVentana.ShowDialog();
                }

            }


        }
    }

}
