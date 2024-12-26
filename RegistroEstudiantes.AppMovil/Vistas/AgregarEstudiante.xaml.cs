using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;
namespace RegistroEstudiantes.AppMovil.Vistas;

public partial class AgregarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-4bea9-default-rtdb.firebaseio.com/");
    public List<Curso> Cursos { get; set; }
    public AgregarEstudiante()
    {
        InitializeComponent();
        ListarCursos();
        BindingContext = this;
    }

    private void ListarCursos()
    {
        var cursos = client.Child("Cursos").OnceAsync<Curso>();
        Cursos = cursos.Result.Select(x => x.Object).ToList();
    }

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {
        Curso curso = cursoPicker.SelectedItem as Curso;

        var estudiante = new Estudiante
        {
            PrimerNombre = primerNombreEntry.Text,
            SegundoNombre = segundoNombreEntry.Text,
            PrimerApellido = primerApellidoEntry.Text,
            SegundoApellido = segundoApellidoEntry.Text,
            CorreoElectronico = correoEntry.Text,
            Edad = int.Parse(edadEntry.Text),
            Curso = curso

        };

        try
        {
            await client.Child("Estudiantes").PostAsync(estudiante);
            await DisplayAlert("Exito", $"El Estudiante {estudiante.PrimerNombre} {estudiante.PrimerApellido} fue agregado con éxito",
            "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error!", $"No se guardaron los datos del estudiante. Error: {ex.Message}", "OK");
        }

    }
}