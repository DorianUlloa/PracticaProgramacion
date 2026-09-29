using RegistroEstudiantes.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RegistroEstudiantes.ServiciosTemporales
{

    public static class ValidadorEstudiante
    {
        public static List<string> Validar(Estudiante estudiante)
        {
            List<string> errores = new();

            if (string.IsNullOrWhiteSpace(estudiante.Carnet))
                errores.Add("El carnet es obligatorio.");

            if (string.IsNullOrWhiteSpace(estudiante.Nombres))
                errores.Add("Los nombres son obligatorios.");

            if (string.IsNullOrWhiteSpace(estudiante.Apellidos))
                errores.Add("Los apellidos son obligatorios.");

            if (estudiante.FechaNacimiento.Date >= DateTime.Today)
                errores.Add("La fecha de nacimiento no puede ser futura.");

            if (estudiante.Edad < 15)
                errores.Add("El estudiante debe tener al menos 15 años.");

            if (estudiante.Promedio < 0 || estudiante.Promedio > 100)
                errores.Add("El promedio debe estar entre 0 y 100.");

            if (!CorreoValido(estudiante.Correo))
                errores.Add("El correo no tiene un formato válido.");

            if (estudiante.TieneDiscapacidadFisica &&
                string.IsNullOrWhiteSpace(estudiante.DescripcionDiscapacidad))
            {
                errores.Add("Debe describir la discapacidad física indicada.");
            }

            return errores;
        }

        private static bool CorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;

            try
            {
                MailAddress direccion = new(correo);
                return direccion.Address.Equals(correo,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

    }
}


