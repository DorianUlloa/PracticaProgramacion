using Microsoft.Data.SqlClient;
using RegistroEstudiantes.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroEstudiantes.Datos
{
    public class CatalogoRepository
    {
        public List<OpcionCatalogo> ListarCarreras()
        {
            const string sql = "SELECT IdCarrera, Nombre FROM Carreras WHERE Activa = 1 ORDER BY Nombre;";
            List<OpcionCatalogo> lista = new();
            using SqlConnection conexion = ConexionBD.CrearConexion();
            using SqlCommand comando = new(sql, conexion);
            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new OpcionCatalogo
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1)
                });
            }
            return lista;
        }
    }
}
