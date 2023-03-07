using System.Reflection;
using System.Windows;

namespace Teste_SysMap
{
    public static class DatabaseConfig
    {
        public static string ConnectionString
        {
            get
            {
                //Obtém o diretório onde o arquivo executável está sendo executado
                string path = Assembly.GetExecutingAssembly().Location;
                string directoryPath = System.IO.Path.GetDirectoryName(path);

                //Define a string de conexão com o banco de dados com base no diretório obtido acima
                return $@"Data Source={directoryPath}\banco\banco_db.db";
            }
        }
    }
}
