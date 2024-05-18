
namespace AlkilaApp.Vistas
{

    /// <summary>
    /// Clase parcial que representa la ventana modal para la verificaci�n del N�mero de Identificaci�n Fiscal (NIF) de una empresa.
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
        /// M�todo invocado cuando se hace clic en el bot�n para comprobar el NIF.
        /// </summary>
        /// <param name="sender">El objeto que invoc� el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private async void ComprobarNifClicked(object sender, EventArgs e)
        {
            nif = campo1.Text;

            // Comprobar si el NIF est� vac�o
            if (nif == null)
            {
                await DisplayAlert("Error", "El NIF no puede estar vac�o.", "ACEPTAR");
                return;
            }

            // Verificar si el usuario es empresario
            if (EsEmpresarioElUsuario())
            {
                await DisplayAlert("Aceptado", "El N�mero de Identificaci�n Fiscal es correcto.", "ACEPTAR");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "El N�mero de Identificaci�n Fiscal no es correcto.", "ACEPTAR");
            }
        }



        /// <summary>
        /// M�todo est�tico para verificar si el usuario es un empresario basado en su NIF.
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
        /// M�todo est�tico para calcular la letra correspondiente a un n�mero del NIF.
        /// </summary>
        /// <param name="nif">El n�mero del NIF.</param>
        /// <returns>La letra correspondiente al n�mero del NIF.</returns>
      public  static char CalcularLetraNIF(int nif)
        {
            char[] tabla = { 'T', 'R', 'W', 'A', 'G', 'M', 'Y', 'F', 'P', 'D', 'X', 'B', 'N', 'J', 'Z', 'S', 'Q', 'V', 'H', 'L', 'C', 'K', 'E' };
            return tabla[nif % 23];
        }



        /// <summary>
        /// M�todo invocado cuando se hace clic en el bot�n para salir de la ventana modal.
        /// </summary>
        /// <param name="sender">El objeto que invoc� el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private async void SalirClicked(object sender, EventArgs e)
        {
            // Volver hacia atr�s
            await Navigation.PopModalAsync();
        }
    }
}
