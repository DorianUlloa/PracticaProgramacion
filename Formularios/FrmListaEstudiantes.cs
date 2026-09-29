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
            dgvEstudiantes.DataSource = null;

            dgvEstudiantes.DataSource =
                estudianteRepository.Buscar(
                    txtBuscar.Text);
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.CurrentRow?.DataBoundItem
       is not Estudiante estudiante)
            {
                MessageBox.Show(
                    "Seleccione un estudiante.");

                return;
            }

            DialogResult respuesta =
                MessageBox.Show(
                    $"¿Desea desactivar a {estudiante.NombreCompleto}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            estudianteRepository.Desactivar(
                estudiante.Id);

            CargarEstudiantes();
        }
    }
}
