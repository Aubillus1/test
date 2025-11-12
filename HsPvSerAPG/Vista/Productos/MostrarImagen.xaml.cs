using FontAwesome.WPF;
using HsPvSerAPG.Controlador;
using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HsPvSerAPG.Vista
{
    public partial class MostrarImagen : Window
    {
        public MostrarImagen(string codArticulo, string descripcionArticulo, string formato)
        {
            InitializeComponent();
            CargarImagen(codArticulo, descripcionArticulo, formato);
        }
        private void CargarImagen(string codart, string desc, string formato)
        {
            try
            {
                string ruta = $"{sisVariables.GVimg}{sisVariables.GCia}{codart}.{formato}";
                var tabartController = new TabartController();
                this.Title = codart;
                if (tabartController.ExisteImagen(ruta))
                {
                  
                    string urlConCacheBuster = $"{ruta}?v={Guid.NewGuid()}";

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // carga directa, no usa cache
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.UriSource = new Uri(urlConCacheBuster, UriKind.Absolute);
                    bitmap.EndInit();

                    ImagenPrincipal.Source = bitmap;

                    // Ajusta al 70% de pantalla
                    double nuevo_ancho = SystemParameters.PrimaryScreenWidth * 0.7f;
                    double alto_original = SystemParameters.PrimaryScreenHeight;
                    double ancho_original = SystemParameters.PrimaryScreenWidth;
                    this.Height = (nuevo_ancho * alto_original) / ancho_original;
                    this.Width = nuevo_ancho;
                }
                else
                {
                    var dibujarerror = ImageAwesome.CreateImageSource(FontAwesome.WPF.FontAwesomeIcon.Image, Brushes.Gray);
                    dibujarerror.Freeze();
                    ImagenPrincipal.Source = dibujarerror;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠️ Error al cargar imagen: {ex.Message}");
            }
        }

    }
}
