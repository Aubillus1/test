using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using HsPvSerAPG.Vista;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HsPvSerAPG
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        TabusrController tabusrController = new TabusrController();
        TabciaController tabciaController = new TabciaController();
        private VersionActualController VerificarNuevaVersionAsync = new VersionActualController();
        private TabcajaController tabcajaController = new TabcajaController();

        public Login()
        {
            InitializeComponent();
            TBUsuarios.Focus();
            Llenar_Caja();
            VerificarVersion();
            //if (HsPvSerAPG.Properties.Settings.Default.IsDarkTheme)
            //{
            //    ApplyTheme("TemaOscuro.xaml");
            //    SwitchTema.IsChecked = true;
            //}
            //else
            //{
            //    ApplyTheme("TemaClaro.xaml");
            //    SwitchTema.IsChecked = false;
            //}
        }

        private void Llenar_Caja()
        {
            var companias = tabciaController.selectTabCia(0);

            if (companias != null && companias.Count > 0)
            {
                CBCia.ItemsSource = companias;
                CBCia.DisplayMemberPath = "des"; // ← lo que se muestra en el ComboBox
                CBCia.SelectedValuePath = "cia";         // ← valor interno asociado a cada item
                CBCia.SelectedIndex = 0;
            }
        }
        private async Task login() {
            Tabusr tabusr = tabusrController.selectLoginUsr(TBUsuarios.Text, TBPassword.Password);

            if (tabusr == null)
            {
                MessageBox.Show("Error al conectar con el servidor. Por favor, intente más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (tabusr.usuario.Equals("") || tabusr.cod.Equals(""))
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CBCia.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una compañía.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            else
            {
                Tabcia tabcia = (Tabcia)CBCia.SelectedItem;
                sisVariables.GCodusr = tabusr.cod;
                sisVariables.GUsuario = tabusr.usuario;
                sisVariables.GCia = tabcia.cia;
                sisVariables.GCodSuc = tabusr.sucact;
                sisVariables.GPeresp = tabusr.peresp;
                sisVariables.GVerstock = tabusr.verstock;
                sisVariables.GCodven = tabusr.codven;
            }

            CompañiaSelección compañia_seleccion = new CompañiaSelección(CBCia.Text);
            compañia_seleccion.Show();
            this.Close();

        }

        private async void VerificarVersion()
        {

            await VerificarNuevaVersionAsync.VerificarNuevaVersionAsync();
        }

        private void Evento_Logear(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true; // Evita que se propague el evento
                login();
            }
        }

        private async void BtnIngresar_Click(object sender, RoutedEventArgs e) => await login();

        private void ApplyTheme(string themeFile)
        {
            // Limpiar diccionarios previos
            Application.Current.Resources.MergedDictionaries.Clear();

            // Cargar nuevo
            var dict = new ResourceDictionary();
            dict.Source = new System.Uri($"Temas/{themeFile}", System.UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
        //private void SwitchTemaOscuro_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplyTheme("TemaOscuro.xaml");

        //    HsPvSerAPG.Properties.Settings.Default.IsDarkTheme = true;
        //    HsPvSerAPG.Properties.Settings.Default.Save();
        //}
        //private void SwitchTemaClaro_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplyTheme("TemaClaro.xaml");
        //    HsPvSerAPG.Properties.Settings.Default.IsDarkTheme = false;
        //    HsPvSerAPG.Properties.Settings.Default.Save();
        //}

    }
}
