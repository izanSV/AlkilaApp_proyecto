using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlkilaApp.Animaciones
{
    /// <summary>
    /// Clase que proporciona funcionalidad para controlar la animación y el estado de los botones en la interfaz de usuario.
    /// </summary>
    internal class Botones : IDisposable
    {
        private readonly Button _button;
        private readonly ImageButton _imageButton;
        private readonly bool _originalState;

        // Constructor que acepta un Button
        public Botones(Button button)
        {
            _button = button ?? throw new ArgumentNullException(nameof(button));
            _originalState = _button.IsEnabled;
            _button.IsEnabled = false;
        }

        // Constructor que acepta un ImageButton
        public Botones(ImageButton imageButton)
        {
            _imageButton = imageButton ?? throw new ArgumentNullException(nameof(imageButton));
            _originalState = _imageButton.IsEnabled;
            _imageButton.IsEnabled = false;
        }

        /// <summary>
        /// Libera los recursos utilizados por la instancia de la clase Botones.
        /// </summary>
        public void Dispose()
        {
            // Restaura el estado original de acuerdo al tipo de control
            if (_button != null)
                _button.IsEnabled = _originalState;
            else if (_imageButton != null)
                _imageButton.IsEnabled = _originalState;
        }

        /// <summary>
        /// Método para animar un botón.
        /// </summary>
        /// <param name="sender">El objeto que desencadenó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        public static async Task animaacionButton(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            // Desactivar la interacción
            button.InputTransparent = true;

            // Simular la animación de presionar el botón
            await button.ScaleTo(0.6, 40);
            await button.ScaleTo(1, 50);
            await button.ScaleTo(0.9, 60);
            await button.ScaleTo(1.1, 70);

            // Revertir la desactivación de la interacción
            button.InputTransparent = false;
        }

        /// <summary>
        /// Método para animar un botón de imagen.
        /// </summary>
        /// <param name="sender">El objeto que desencadenó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        public static async Task animaacionImageButton(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            // Desactivar la interacción
            button.InputTransparent = true;

            // Simular la animación de presionar el botón
            await button.ScaleTo(0.6, 40);
            await button.ScaleTo(1, 50);
            await button.ScaleTo(0.9, 60);
            await button.ScaleTo(1.1, 70);

            // Revertir la desactivación de la interacción
            button.InputTransparent = false;
        }

        /// <summary>
        /// Método obsoleto para confirmar la salida de la aplicación.
        /// </summary>
        /// <param name="mensaje">Mensaje de confirmación.</param>
        /// <param name="encabezado">Encabezado del mensaje de confirmación.</param>
        /// <returns>Devuelve true si se confirma la salida, de lo contrario, false.</returns>
        [Obsolete]
        public static async Task<bool> ConfirmarSalir(string mensaje, string encabezado)
        {
            return await Device.InvokeOnMainThreadAsync(async () =>
            {
                return await Application.Current.MainPage.DisplayAlert(encabezado, mensaje, "Confirmar", "Cancelar");
            });
        }
    }
}
