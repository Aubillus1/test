using HsPvSerInstaller2.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerInstaller2.Controlador
{
    internal class MantenimientoController
    {
        public void ReiniciarAplicacion()
        {
            string exePath = RutaHelper.ObtenerRutaExe("HsPvSer", "HsPvSerWPF.exe");
            var procesos = ProcesosService.BuscarProcesos("hspvser");

            if (procesos.Any())
                ProcesosService.CerrarProcesos(procesos);
            else
                MessageBox.Show("No se encontraron procesos activos.", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);

            if (ArchivosService.EliminarArchivo(exePath))
                MessageBox.Show("Archivo eliminado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("No se encontró el archivo a eliminar.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
