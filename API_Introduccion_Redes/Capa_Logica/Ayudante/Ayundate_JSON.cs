using System.Text.Json;

namespace Capa_Logica.Ayudante
{
    public class Ayundate_JSON
    {
        public Ayundate_JSON()
        {
            
        }

        public string Serialice_Modelo<T>(T modelo) {

            try
            {
                string json = JsonSerializer.Serialize(modelo);

                return json;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public T Deserialize_Modelo<T>(string json) {
            try
            {
                T modelo = JsonSerializer.Deserialize<T>(json);
                return modelo;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }           
        }
    }
}
