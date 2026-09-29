using RegistroEstudiantes.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroEstudiantes.ServiciosTemporales
{
    public static class MemoriaEstudiante
    {
        public static BindingList<Estudiante> Estudiantes { get; } = new();
    }
}
