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
    public partial class FrmRegistroEstudiante : Form
    {
        private readonly EstudianteRepository estudianteRepository = new();
        private readonly CatalogoRepository catalogoRepository = new();
        private Estudiante? estudianteEnEdicion;
        public FrmRegistroEstudiante()  
        {
            InitializeComponent();
           
            CargarCarreras();
        }

        public FrmRegistroEstudiante(Estudiante estudiante) : this()
        {
            estudianteEnEdicion = estudiante;
            CargarDatosEstudiante();
        }

        private void CargarDatosEstudiante()
        {
            if (estudianteEnEdicion == null)
                return;

            txtCarnet.Text =
                estudianteEnEdicion.Carnet;

            txtNombres.Text =
                estudianteEnEdicion.Nombres;

            txtApellidos.Text =
                estudianteEnEdicion.Apellidos;

            txtCorreo.Text =
                estudianteEnEdicion.Correo;

            nudPromedio.Value =
                estudianteEnEdicion.Promedio;

            cboCarrera.SelectedValue =
                estudianteEnEdicion.IdCarrera;

            cboMunicipio.SelectedValue =
                estudianteEnEdicion.IdMunicipio;

            // continuar con los demás controles
        }

        private void CargarCarreras()
        {
            List<OpcionCatalogo> carreras = catalogoRepository.ListarCarreras();

            cboCarrera.DataSource = null;

            cboCarrera.DisplayMember = "Nombre";
            cboCarrera.ValueMember = "Id";
            cboCarrera.DataSource = carreras;

            cboCarrera.SelectedIndex = -1;
        }

        private Estudiante ConstruirEstudianteDesdeFormulario()
        {
            Sexo sexo = Enum.TryParse(
                cboSexo.SelectedItem?.ToString(),
                out Sexo sexoSeleccionado)
                ? sexoSeleccionado
                : Sexo.M;

            AreaConocimiento area = Enum.TryParse(
                cboArea.SelectedItem?.ToString(),
                out AreaConocimiento areaSeleccionada)
                ? areaSeleccionada
                : AreaConocimiento.Otra;

            return new Estudiante
            {
                Carnet = txtCarnet.Text,
                Cedula = txtCedula.Text.Trim(),
                Nombres = txtNombres.Text,
                Apellidos = txtApellidos.Text,
                Sexo = sexo,
                FechaNacimiento = dtpFechaNacimiento.Value.Date,
                Nacionalidad = cboNacionalidad.Text.Trim(),
                NivelAcademico = cboNivelAcademico.Text.Trim(),
                TieneTutor = chkTieneTutor.Checked,
                AreaConocimiento = area,

                IdCarrera = cboCarrera.SelectedValue is int idCarrera
                    ? idCarrera
                    : 0,

                Carrera = cboCarrera.Text.Trim(),

                Departamento = cboDepartamento.Text.Trim(),
                IdMunicipio = cboMunicipio.SelectedValue is int idMunicipio ? idMunicipio: 0,
                Municipio = cboMunicipio.Text.Trim(),
                Etnia = cboEtnia.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                Promedio = nudPromedio.Value,
                TieneDiscapacidadFisica = chkDiscapacidad.Checked,
                DescripcionDiscapacidad =
                    txtDescripcionDiscapacidad.Text.Trim()
            };
        }
        private bool ValidarFormulario(Estudiante estudiante)
        {
            errorProvider1.Clear();
            List<string> errores = ValidadorEstudiante.Validar(estudiante);

            if (string.IsNullOrWhiteSpace(estudiante.Carnet))
                errorProvider1.SetError(txtCarnet, "Ingrese el carnet.");

            if (string.IsNullOrWhiteSpace(estudiante.Nombres))
                errorProvider1.SetError(txtNombres, "Ingrese los nombres.");

            if (errores.Count == 0)
                return true;

            MessageBox.Show(string.Join(Environment.NewLine, errores),
                "Revise la información",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        private void chkTieneTutor_CheckedChanged(object sender, EventArgs e)
        {
            // grpTutor.Enabled = chkTieneTutor.Checked;
        }

        private void chkDiscapacidad_CheckedChanged(object sender, EventArgs e)
        {
            txtDescripcionDiscapacidad.Enabled = chkDiscapacidad.Checked;

            if (!chkDiscapacidad.Checked)
                txtDescripcionDiscapacidad.Clear();
        }

        private void cboArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarreras();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

           
            try
            {
                Estudiante estudiante =
                    ConstruirEstudianteDesdeFormulario();
                MessageBox.Show(
      $"Municipio: {cboMunicipio.Text}\n" +
      $"SelectedIndex: {cboMunicipio.SelectedIndex}\n" +
      $"SelectedValue: {cboMunicipio.SelectedValue}\n" +
      $"IdMunicipio del estudiante: {estudiante.IdMunicipio}",
      "Prueba Municipio");

                if (!ValidarFormulario(estudiante))
                    return;

                // NUEVO ESTUDIANTE
                if (estudianteEnEdicion == null)
                {
                    if (estudianteRepository
                        .ExisteCarnet(estudiante.Carnet))
                    {
                        errorProvider1.SetError(
                            txtCarnet,
                            "El carnet ya existe en la base de datos.");

                        return;
                    }

                    estudianteRepository.Insertar(estudiante);

                    MessageBox.Show(
                        "Estudiante guardado correctamente.",
                        "Registro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LimpiarFormulario();
                }
                // ESTUDIANTE EN EDICIÓN
                else
                {
                    estudiante.Id =
                        estudianteEnEdicion.Id;

                    estudianteRepository.Actualizar(
                        estudiante);

                    MessageBox.Show(
                        "Estudiante actualizado correctamente.",
                        "Actualización",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    Close();
                }
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

        private void LimpiarFormulario()
        {
            txtCarnet.Clear();
            txtCedula.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            cboSexo.SelectedIndex = -1;
            cboNacionalidad.SelectedIndex = -1;
            cboNivelAcademico.SelectedIndex = -1;
            chkTieneTutor.Checked = false;
            cboArea.SelectedIndex = -1;
            cboCarrera.SelectedIndex = -1;
            cboDepartamento.SelectedIndex = -1;
            cboMunicipio.SelectedIndex = -1;
            cboEtnia.SelectedIndex = -1;
            txtCorreo.Clear();
            nudPromedio.Value = nudPromedio.Minimum;
            chkDiscapacidad.Checked = false;
            txtDescripcionDiscapacidad.Clear();
            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-18);
            errorProvider1.Clear();
            txtCarnet.Focus();
        }
    }
}
