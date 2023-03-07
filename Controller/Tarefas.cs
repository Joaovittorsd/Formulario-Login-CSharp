using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows;
using System.Data.SQLite;

namespace Teste_SysMap.Controller
{
    public class Tarefas
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data_Hora { get; set; }
        public int id_Usuario { get; set; }

        public Tarefas() { }

        public Tarefas(int id, string descricao, DateTime data_Hora, int id_Usuario)
        {
            Id = id;
            Descricao = descricao;
            Data_Hora = data_Hora;
            this.id_Usuario = id_Usuario;
        }

        public static DataTable BuscarTodos(global::Usuario usuario)
        {
            var sql = "SELECT Tarefas.id, Usuario.Nome, Usuario.Email, Tarefas.Descricao, Tarefas.Data_hora, Tarefas.id_Usuario " +
                "FROM Tarefas " +
                "JOIN Usuario ON Tarefas.Id_Usuario = usuario.Id ";

            //Configuração para que apenas um usuario possa ver todos das tarefas
            /*if (usuario.Email != "contato@sistemas.com.br")
            {
                sql += $"WHERE Usuario.Email = '{usuario.Email}'";
            }*/

            var dt = new DataTable();

            try
            {
                using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
                {
                    cn.Open();
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, cn))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;
        }
        public static DataTable Pesquisa(Usuario usuario, string id, string email, string nome)
        {
            var dt = new DataTable();

            string sql = "SELECT Tarefas.id, Usuario.Nome, Usuario.Email, Tarefas.Descricao, Tarefas.Data_hora, Tarefas.id_Usuario " +
                "FROM Tarefas " +
                "JOIN Usuario ON Tarefas.Id_Usuario = usuario.Id WHERE 1=1";

            if (id != "") sql += " AND Tarefas.id = @id";
            if (email != "") sql += " AND Usuario.Email LIKE '%' + @email + '%' ";
            if (nome != "") sql += " AND Usuario.Nome LIKE '%' + @nome + '%' ";

            if (usuario.Email != "ti@gsoservico.com.br")
            {
                sql += $"WHERE Usuario.Email = '{usuario.Email}'";
            }

            try
            {
                using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
                {
                    cn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, cn))
                    {
                       
                        if (id != "") cmd.Parameters.AddWithValue("@id", id);
                        if (email != "") cmd.Parameters.AddWithValue("@email", email);
                        if (nome != "") cmd.Parameters.AddWithValue("@nome", nome);
                        
                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }
    }
}
