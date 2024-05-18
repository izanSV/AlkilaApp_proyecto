using System.Net.Http;
using System.Threading.Tasks;

namespace AlkilaApp.Interfaces
{
    /// <summary>
    /// Interfaz que define los métodos para enviar mensajes de WhatsApp.
    /// </summary>
    public interface IServicioWhatsApp
    {
        /// <summary>
        /// Envía un mensaje de texto a través de WhatsApp.
        /// </summary>
        /// <param name="recipient">Número de teléfono del destinatario del mensaje.</param>
        /// <param name="text">Texto del mensaje a enviar.</param>
        /// <returns>Respuesta de la solicitud HTTP.</returns>
        Task<HttpResponseMessage> EnviarMensaje(string recipient, string text);

        /// <summary>
        /// Envía una plantilla de mensaje a través de WhatsApp.
        /// </summary>
        /// <param name="recipient">Número de teléfono del destinatario del mensaje.</param>
        /// <param name="foto">URL de la foto a enviar en la plantilla.</param>
        /// <returns>Respuesta de la solicitud HTTP.</returns>
        Task<HttpResponseMessage> EnviarPlantilla(string recipient, string foto);
    }
}
