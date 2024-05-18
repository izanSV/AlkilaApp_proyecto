
namespace AlkilaApp.Vistas
{

    /// <summary>
    /// Clase parcial que representa la ventana modal para la verificación del Número de Identificación Fiscal (NIF) de una empresa.
    /// </summary>

    public partial class VentanaModalEmpresa : ContentPage
    {
       static string nif = "";

        /// <summary>
        /// Constructor de la clase <see cref="VentanaModalEmpresa"/>.
        /// </summary>
        public VentanaModalEmpresa()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Método invocado cuando se hace clic en el botón para comprobar el NIF.
        /// </summary>
        /// <param name="sender">El objeto que invocó el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private async void ComprobarNifClicked(object sender, EventArgs e)
        {
            nif = campo1.Text;

            // Comprobar si el NIF está vacío
            if (nif == null)
            {
                await DisplayAlert("Error", "El NIF no puede estar vacío.", "ACEPTAR");
                return;
            }

            // Verificar si el usuario es empresario
            if (EsEmpresarioElUsuario())
            {
                await DisplayAlert("Aceptado", "El Número de Identificación Fiscal es correcto.", "ACEPTAR");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "El Número de Identificación Fiscal no es correcto.", "ACEPTAR");
            }
        }



        /// <summary>
        /// Método estático para verificar si el usuario es un empresario basado en su NIF.
        /// </summary>
        /// <returns>True si el usuario es empresario, de lo contrario, False.</returns>
        public static bool EsEmpresarioElUsuario()
        {
            // Verificar la longitud del NIF
            if (nif.Length != 9 || nif.Equals(null))
            {
                return false;
            }

            int numero = int.Parse(nif.Substring(1, 8));

            // Calcular la letra del NIF y compararla con la proporcionada
            if (CalcularLetraNIF(numero) == nif[0])
            {
                return true;
            }
            return false;
        }



        /// <summary>
        /// Método estático para calcular la letra correspondiente a un número del NIF.
        /// </summary>
        /// <param name="nif">El número del NIF.</param>
        /// <returns>La letra correspondiente al número del NIF.</returns>
      public  static char CalcularLetraNIF(int nif)
        {
            char[] tabla = { 'T', 'R', 'W', 'A', 'G', 'M', 'Y', 'F', 'P', 'D', 'X', 'B', 'N', 'J', 'Z', 'S', 'Q', 'V', 'H', 'L', 'C', 'K', 'E' };
            return tabla[nif % 23];
        }



        /// <summary>
        /// Método invocado cuando se hace clic en el botón para salir de la ventana modal.
        /// </summary>
        /// <param name="sender">El objeto que invocó el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private async void SalirClicked(object sender, EventArgs e)
        {
            // Volver hacia atrás
            await Navigation.PopModalAsync();
        }
    }
}
