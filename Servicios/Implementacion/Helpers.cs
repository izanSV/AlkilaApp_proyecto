using Firebase.Storage;

namespace AlkilaApp.Servicios.Implementacion
{

    public class Helpers
    {

        /// <summary>
        /// Sube una imagen a Firebase Storage.
        /// </summary>
        /// <param name="imageStream">Stream que contiene la imagen a subir.</param>
        /// <param name="fileName">Nombre del archivo de la imagen.</param>
        /// <returns>La URL de la imagen almacenada en Firebase Storage si la carga fue exitosa, de lo contrario, null.</returns>
        public async Task<string> AddFoto(Stream imageStream, string fileName)
        {
            try
            {
                // Crear una referencia a la raíz de Firebase Storage
                var firebaseStorage = new FirebaseStorage(Setting.FirebaseStoreBucket);

                // Subir la imagen a Firebase Storage en el directorio raíz del bucket con el nombre del archivo
                var imageUrl = await firebaseStorage
                    .Child(fileName)   // Nombre del archivo (sin incluir la ruta al directorio)
                    .PutAsync(imageStream);

                Console.WriteLine("URL de la imagen generada: " + imageUrl);

                // Si la carga fue exitosa, imageUrl contiene la URL de la imagen almacenada en Firebase Storage
                return imageUrl;
            }
            catch (FirebaseStorageException ex)
            {
                // Manejar la excepción específica de Firebase Storage
                Console.WriteLine("Error al subir la imagen a Firebase Storage: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                Console.WriteLine("Error al subir la imagen: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteFoto(string imageUrl)
        {
            try
            {
                // Verificar si la URL de la imagen es válida
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    // Si la URL está vacía, no se puede eliminar ninguna imagen
                    Console.WriteLine("La URL de la imagen es inválida.");
                    return false;
                }

                string foto = imageUrl+"/";
                // Crear una referencia al archivo en Firebase Storage
                var firebaseStorage = new FirebaseStorage(Setting.FirebaseStoreBucket);
                var storageReference = firebaseStorage.Child(foto);

                // Eliminar el archivo de Firebase Storage
                await storageReference.DeleteAsync();

                Console.WriteLine("Imagen eliminada correctamente: " + foto);

                // Si se eliminó correctamente, devolver true
                return true;
            }
            catch (FirebaseStorageException ex)
            {
                // Manejar la excepción específica de Firebase Storage
                Console.WriteLine("Error al eliminar la imagen de Firebase Storage: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                Console.WriteLine("Error al eliminar la imagen: " + ex.Message);
                return false;
            }
        }
    }
}
