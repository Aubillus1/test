using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerInstaller2.Servicios
{
    public class ProcesosService
    {
        public static List<Process> BuscarProcesos(string nombre)
        {
            int currentProcessId = Process.GetCurrentProcess().Id;
            return Process.GetProcesses()
                .Where(p => p.ProcessName.ToLower().Contains(nombre.ToLower()) && p.Id != currentProcessId)
                .ToList();
        }

        public static void CerrarProcesos(List<Process> procesos)
        {
            foreach (var proceso in procesos)
            {
                proceso.Kill();
                proceso.WaitForExit(5000);
            }
        }
    }
}
