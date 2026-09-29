using Microsoft.Data.SqlClient;
using RegistroEstudiantes.Datos;
using RegistroEstudiantes.Modelos;
using RegistroEstudiantes.ServiciosTemporales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegistroEstudiantes.Formularios
{
    public partial class FrmListaEstudiantes : Form
    {
        private readonly EstudianteRepository estudianteRepository = new();
        public FrmListaEstudiantes()
        {
            InitializeComponent();

            CargarEstudiantes();

        }


        private void CargarEstudiantes()
        {
            dgvEstudiantes.AutoGenerateColumns = false;

            Carnet.DataPropertyName = "Carnet";
            Nombrecompleto.DataPropertyName = "NombreCompleto";
            Sexo.DataPropertyName = "Sexo";
            Carrera.DataPropertyName = "Carrera";
            Nivel.DataPropertyName = "NivelAcademico";
            Correo.DataPropertyName = "Correo";
            Promedio.DataPropertyName = "Promedio";

            dgvEstudiantes.DataSource = null;
            dgvEstudiantes.DataSource =
                estudianteRepository.Listar();
        }

        private Estudiante? ObtenerSeleccionado()
        {
            return dgvEstudiantes.CurrentRow?.DataBoundItem as Estudiante;
        }



        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            Estudiante? estudiante = ObtenerSeleccionado();

            if (estudiante is null)
            {
                MessageBox.Show("Seleccione un estudiante.");
                return;
            }

            using FrmDetalleEstudiante detalle = new(estudiante);
            detalle.ShowDialog(this);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = txtBuscar.Text.Trim();

                // Si no escribió nada, mostrar nuevamente todos
                if (string.IsNullOrWhiteSpace(texto))
                {
                    CargarEstudiantes();
                    return;
                }

                List<Estudiante> resultados =
                    estudianteRepository.Buscar(texto);

                dgvEstudiantes.DataSource = null;
                dgvEstudiantes.DataSource = resultados;

                if (resultados.Count == 0)
                {
                    MessageBox.Show(
                        "No se encontraron estudiantes con ese criterio.",
                        "Búsqueda",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible realizar la búsqueda.\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            Estudiante? estudiante = ObtenerSeleccionado();

            if (estudiante == null)
            {
                MessageBox.Show(
                    "Seleccione un estudiante para desactivar.",
                    "Desactivar estudiante",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult respuesta = MessageBox.Show(
                $"¿Está seguro de desactivar a {estudiante.NombreCompleto}?",
                "Confirmar desactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                estudianteRepository.Desactivar(estudiante.Id);

                MessageBox.Show(
                    "Estudiante desactivado correctamente.",
                    "Desactivación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarEstudiantes();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    $"Error de base de datos:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Estudiante? estudiante = ObtenerSeleccionado();

            if (estudiante == null)
            {
                MessageBox.Show(
                    "Seleccione un estudiante para editar.",
                    "Editar estudiante",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using FrmRegistroEstudiante formulario =
                new FrmRegistroEstudiante(estudiante);

            formulario.ShowDialog(this);

            // Refrescar la lista cuando se cierre
            CargarEstudiantes();

        }
    }
}
