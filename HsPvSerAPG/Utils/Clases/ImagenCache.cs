using FontAwesome.WPF;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HsPvSerAPG.Controlador;
using System;

namespace HsPvSerAPG.Utils.Clases
{
    public static class ImagenCache
    {
        private static readonly Dictionary<string, ImageSource> _cache = new Dictionary<string, ImageSource>();

        public static ImageSource ObtenerImagen(string ruta, string codart, string formato)
        {
            string clave = codart + "." + formato;

            if (_cache.TryGetValue(clave, out var imagen))
                return imagen; // ✅ Ya está en cache

            // ❌ No está en cache → cargar desde disco
            TabartController tabartController = new TabartController();
            try
            {
                if (tabartController.ExisteImagen(ruta))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(ruta, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze(); // para poder usarlo en múltiples threads

                    _cache[clave] = bitmap;
                    return bitmap;
                }
            }
            catch
            {
                // ignorar errores y devolver ícono por defecto
            }

            // Imagen de error por defecto
            var errorImg = ImageAwesome.CreateImageSource(FontAwesome.WPF.FontAwesomeIcon.Image, Brushes.Gray);
            errorImg.Freeze();
            _cache[clave] = errorImg;
            return errorImg;
        }
    }
}
