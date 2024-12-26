using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;
using System.Collections.ObjectModel;
using System.Security;
using static System.Net.Mime.MediaTypeNames;

namespace RegistroEstudiantes.AppMovil.Vistas;

public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-4bea9-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos { get; set; }
    public ObservableCollection<string> ListaCursos { get; set; } = new ObservableCollection<string>();
    private Estudiante estudianteActualizado = new Estudiante();
    private string estudianteId;
    public EditarEstudiante(string idEstudiante)
    {
        InitializeComponent();
        BindingContext = this;
        estudianteId = idEstudiante;
        CargarListaCursos();
        CargarEstudiante(estudianteId);

    }

    private async void CargarListaCursos()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListaCursos.Clear();
            foreach (var curso in cursos)
            {
                ListaCursos.Add(curso.Object.Nombre);
            }
        }
        catch (Exception ex)
        {

            await DisplayAlert("Error", "Error:" + ex.Message, "Ok");
        }

    }

    private async void CargarEstudiante(string idEstudiante)
    {
        var estudiante = await client.Child("Estudiantes").Child(idEstudiante).OnceSingleAsync<Estudiante>();

        if (estudiante != null)
        {
            EditPrimerNombreEntry.Text = estudiante.PrimerNombre;
            EditSegundoNombreEntry.Text = estudiante.SegundoNombre;
            EditPrimerApellidoEntry.Text = estudiante.PrimerApellido;
            EditSegundoApellidoEntry.Text = estudiante.SegundoApellido;
            EditCorreoEntry.Text = estudiante.CorreoElectronico;
            EditEdadEntry.Text = estudiante.Edad.ToString();
            EditCursoPicker.SelectedItem = estudiante.Curso?.Nombre;
        }
    }

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditCorreoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditEdadEntry.Text) ||
                EditCursoPicker.SelectedItem == null)
            {

                await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            if (!EditCorreoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "El correo electrónico no es válido", "OK");
                return;
            }

            if (!int.TryParse(EditEdadEntry.Text, out int edad))
            {
                await DisplayAlert("Error", "La edad debe ser un número válido", "OK");
                return;
            }

            if (edad <= 0)
            {
                await DisplayAlert("Error", "La edad debe ser mayor a 0", "OK");
                return;
            }

            estudianteActualizado.Id = estudianteId;
            estudianteActualizado.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            estudianteActualizado.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            estudianteActualizado.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            estudianteActualizado.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            estudianteActualizado.CorreoElectronico = EditCorreoEntry.Text.Trim();
            estudianteActualizado.Edad = edad;
            estudianteActualizado.Estado = estadoSwitch.IsToggled;
            estudianteActualizado.Curso = new Curso { Nombre = EditCursoPicker.SelectedItem.ToString() };

            await client.Child("Eestudiantes").Child(estudianteActualizado.Id).PutAsync(estudianteActualizado);

            await DisplayAlert("Éxito", "El estudiante se ha actualizado correctamente", "OK");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {

            await DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
    }
}