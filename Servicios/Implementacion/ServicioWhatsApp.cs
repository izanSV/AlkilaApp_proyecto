using AlkilaApp.Interfaces;
using Firebase.Database;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System.Text;


namespace AlkilaApp.Servicios.Implementacion
{

    /// <summary>
    /// Clase que proporciona funcionalidad para enviar mensajes a través de WhatsApp Api.
    /// </summary>
    public class ServicioWhatsApp : IServicioWhatsApp
    {

        #region Atributos

        private FirebaseClient _firebase; // Cliente Firebase para interactuar con la base de datos
        private readonly HttpClient _client; // Cliente HTTP para realizar solicitudes

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase ServicioWhatsApp.
        /// </summary>
        public ServicioWhatsApp()
        {
            _firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
            _client = new HttpClient(); // Inicializa el cliente HTTP
            
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Envía un mensaje de texto a través de WhatsApp.
        /// </summary>
        /// <param name="recipient">El número de teléfono del destinatario del mensaje.</param>
        /// <param name="text">El texto del mensaje a enviar.</param>
        /// <returns>La respuesta de la solicitud HTTP.</returns>
        public async Task<HttpResponseMessage> EnviarMensaje(string recipient, string text)
        {
            // URL para enviar el mensaje
            string url = $"https://graph.facebook.com/v18.0/{236697199533490}/messages";

            // Contenido del mensaje
            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("messaging_product", "whatsapp"),
        new KeyValuePair<string, string>("to", recipient), // Número de teléfono del destinatario
        new KeyValuePair<string, string>("text", $"{{ \"body\": \"{text}\" }}") // Texto del mensaje
    });

            // Envío de la solicitud HTTP y retorno de la respuesta
            return await SendRequestAsync(HttpMethod.Post, url, content);
        }



        /// <summary>
        /// Envía una solicitud HTTP asincrónica.
        /// </summary>
        /// <param name="method">El método HTTP de la solicitud (GET, POST, etc.).</param>
        /// <param name="url">La URL a la que se enviará la solicitud.</param>
        /// <param name="content">El contenido de la solicitud (opcional).</param>
        /// <returns>La respuesta de la solicitud HTTP.</returns>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string url, HttpContent content = null)
        {
            try
            {
                // Obtenemos la api key del metodo
                string apiKey = await ObtenerApiKeyWhatsAppFirebase();

                // Configuración de la autorización de la solicitud HTTP
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                // Creación de la solicitud HTTP
                var request = new HttpRequestMessage(method, url);

                // Agregar el contenido a la solicitud si existe
                if (content != null)
                {
                    request.Content = content;
                }

                // Envío de la solicitud HTTP y retorno de la respuesta
                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                // Manejo de errores y registro en la consola
                Console.WriteLine($"Error al enviar la solicitud HTTP: {ex.Message}");
                return null;
            }
        }




        /// <summary>
        /// Envía una plantilla de mensaje a través de WhatsApp.
        /// </summary>
        /// <param name="telefonoDestinatario">El número de teléfono del destinatario del mensaje.</param>
        /// <param name="imagen">La URL de la foto a incluir en la plantilla.</param>
        /// <returns>La respuesta de la solicitud HTTP.</returns>
        public async Task<HttpResponseMessage> EnviarPlantilla(string telefonoDestinatario, string detalle)
        {
            try
            {
                // URL para enviar el mensaje
                var url = $"https://graph.facebook.com/v18.0/{236697199533490}/messages";

                // Creación del objeto de mensaje de WhatsApp
                var mensajeWhatsApp = new MensajeWhatsApp
                {
                    //el prefijo de momento solo para españa
                    messaging_product = "whatsapp",
                    to = "34"+ telefonoDestinatario,
                    type = "template",
                    template = new Template
                    {
                        name = "interative_software",
                        language = new Language { code = "es" },
                        components = new[]
                        {
                    // Componente de encabezado con una imagen
                    new Component
                    {
                        type = "header",
                        parameters = new Parameter[]
                        {
                            new Parameter { type = "image" ,image = new Image { link = Setting.FotoWhatsApp } },
                        }
                    },
                    // Componente de cuerpo con el texto del destinatario
                    new Component
                    {
                        type = "body",
                        parameters = new Parameter[]
                        {
                            new Parameter { type = "text", text = detalle },
                        }
                    }
                }
            }
        };
                // Serialización del objeto de mensaje de WhatsApp a JSON

                var json = JsonConvert.SerializeObject(mensajeWhatsApp);

                // Contenido de la solicitud HTTP
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    string apiKey = await ObtenerApiKeyWhatsAppFirebase();
                    // Configuración y envío de la solicitud HTTP
                    _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                    var request = new HttpRequestMessage(HttpMethod.Post, url);

                    if (content != null)
                    {
                        request.Content = content;
                    }

                    return await _client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar la solicitud HTTP: {ex.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtener de la base de datos de _firebase el apikey, en el caso de que venciera la fecha poder acceder a ella sin modificar el código
        /// </summary>
        public async Task<string> ObtenerApiKeyWhatsAppFirebase()
        {
            var aKey = await _firebase
                .Child("api_key")
                .OnceSingleAsync<string>();
            return aKey;
        }

        #endregion
    }


    #region Estas son las clases necesarias para poder construir la plantialla de whatsApp

    /// <summary>
    /// Clase que representa un mensaje de WhatsApp.
    /// </summary>
    public class MensajeWhatsApp
    {
        /// <summary>
        /// El _producto de mensajería utilizado para enviar el mensaje.
        /// </summary>
        public string messaging_product { get; set; }

        /// <summary>
        /// El número de teléfono del destinatario del mensaje.
        /// </summary>
        public string to { get; set; }

        /// <summary>
        /// El tipo de mensaje.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// La plantilla del mensaje.
        /// </summary>
        public Template template { get; set; }
    }

    /// <summary>
    /// Clase que representa una plantilla de mensaje de WhatsApp.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// El nombre de la plantilla.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// El idioma de la plantilla.
        /// </summary>
        public Language language { get; set; }

        /// <summary>
        /// Los componentes de la plantilla.
        /// </summary>
        public Component[] components { get; set; }
    }

    /// <summary>
    /// Clase que representa el idioma de una plantilla de mensaje.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// El código del idioma.
        /// </summary>
        public string code { get; set; }
    }

    /// <summary>
    /// Clase que representa un componente de una plantilla de mensaje.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// El tipo de componente.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Los parámetros del componente.
        /// </summary>
        public Parameter[] parameters { get; set; }
    }

    /// <summary>
    /// Clase que representa un parámetro de un componente de una plantilla de mensaje.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// El tipo de parámetro.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// El texto del parámetro.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// La imagen del parámetro.
        /// </summary>
        public Image image { get; set; }
    }

    /// <summary>
    /// Clase que representa una imagen en un parámetro de una plantilla de mensaje.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// El enlace URL de la imagen.
        /// </summary>
        public string link { get; set; }
    }

    #endregion


}