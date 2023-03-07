using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Net;
using System.Reflection;
using System.Transactions;
using System.Web;
using System.Windows;
using Teste_SysMap;
using Teste_SysMap.Controller;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public byte[] FotoPerfil { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime Data_Hora { get; set; }
    public int id_Usuario { get; set; }

    public Usuario() { }

    public Usuario(int id, DateTime data_Hora, int id_Usuario)
    {
        Id = id;
        Data_Hora = data_Hora;
        this.id_Usuario = id_Usuario;
    }


    public HttpStatusCode Salvar(Usuario usuario)
    {
        var sql = "";
        object newId;
        string Descricao = "";

        // Validação dos dados
        if (string.IsNullOrEmpty(Nome) || DataNascimento == null || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Senha))
        {
            return HttpStatusCode.BadRequest;
        }

        // Salva os dados do usuário na tabela do SQL Server
        using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
        {
            cn.Open();
            using (SQLiteTransaction transaction = cn.BeginTransaction())
            {
                try
                {
                    if (usuario.Id == 0)
                        sql = "INSERT INTO Usuario (Nome, DataNascimento, Email, Senha, FotoPerfil, DataCadastro) VALUES (@Nome, @DataNascimento, @Email, @Senha, @FotoPerfil, @DataCadastro); SELECT last_insert_rowid()";
                    else
                        sql = "UPDATE Usuario SET Nome=@Nome, DataNascimento=@DataNascimento, Email=@Email, Senha=@Senha, FotoPerfil=@FotoPerfil WHERE id=@id";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, cn, transaction))
                    {
                        if (usuario.Id > 0)
                            cmd.Parameters.AddWithValue("@id", usuario.Id);

                        cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                        cmd.Parameters.AddWithValue("@DataNascimento", usuario.DataNascimento);
                        cmd.Parameters.AddWithValue("@Email", usuario.Email);
                        cmd.Parameters.AddWithValue("@Senha", usuario.Senha);
                        cmd.Parameters.AddWithValue("@FotoPerfil", usuario.FotoPerfil);
                        if (usuario.Id == 0) cmd.Parameters.AddWithValue("@DataCadastro", DateTime.Now);
                        
                        newId = cmd.ExecuteScalar();
                    }

                    var sqlTarefas = "INSERT INTO Tarefas (Descricao, Data_hora, id_Usuario) VALUES (@Descricao, @Data_hora, @id_Usuario)";

                    if (usuario.Id == 0) Descricao = "Cadastro de Usuario";
                    else Descricao = "Atualização de Dados cadastrais";

                    using (SQLiteCommand cmd = new SQLiteCommand(sqlTarefas, cn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Descricao", Descricao);
                        cmd.Parameters.AddWithValue("@Data_hora", DateTime.Now);
                        if (usuario.Id == 0) cmd.Parameters.AddWithValue("@id_Usuario", newId);
                        else cmd.Parameters.AddWithValue("@id_Usuario", usuario.Id);

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return HttpStatusCode.Created;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                    return HttpStatusCode.BadRequest;
                }
                catch (SqlException)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }
        }
    }

    public byte[] GetFotoPerfil(Usuario usuario)
    {
        byte[] fotoPerfil = null;

        var sql = "SELECT FotoPerfil FROM Usuario WHERE Id = @Id";
        try
        {
            using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@Id", usuario.Id);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fotoPerfil = (byte[])reader["FotoPerfil"];
                    }
                }
            }
        }
        catch (SqlException)
        {
            // Tratar exceção
        }

        return fotoPerfil;
    }
    public string LoginUsuario(Usuario usuario, string senha)
    {
        var sql = "SELECT * FROM Usuario WHERE Email=@Email";

        var result = "";

        try
        {
            using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {

                cn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            if (dr.Read())
                            {
                                if (senha == dr["senha"].ToString())
                                {
                                    usuario.Id = (int)(long)dr["id"];
                                    usuario.Nome = dr["nome"].ToString();
                                    usuario.DataNascimento = (DateTime)dr["dataNascimento"];
                                    usuario.Email = dr["email"].ToString();
                                    usuario.Senha = dr["senha"].ToString();
                                    usuario.FotoPerfil = (byte[])dr["fotoPerfil"];
                                    usuario.DataCadastro = (DateTime)dr["dataCadastro"];
                                    result = "ok";
                                }
                                else
                                    result = "Usuario ou senha invalida!";
                            }
                            else
                                result = "Usuario ou senha invalida!";
                        }
                        else
                            result = "Usuario ou senha invalida!";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }
        return result;
    }

    public static DataTable BuscarTodos()
    {
        var sql = "SELECT * From Usuario";

        var dt = new DataTable();

        try
        {
            using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                cn.Open();
                using (var da = new SQLiteDataAdapter(sql, cn))
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
    public bool Excluir(Usuario usuario)
    {
        var sql = "";
        string Descricao = "Usuario Deletado ";

        sql = "DELETE FROM usuario WHERE id=@id";

        try
        {
            using (SQLiteConnection cn = new SQLiteConnection(DatabaseConfig.ConnectionString))
            {
                cn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", usuario.Id);
                    cmd.ExecuteNonQuery();
                }

                var sqlTarefas = "INSERT INTO Tarefas (Descricao, Data_hora, id_Usuario) VALUES (@Descricao, @Data_hora, @id_Usuario)";

                using (SQLiteCommand cmd = new SQLiteCommand(sqlTarefas, cn))
                {
                    cmd.Parameters.AddWithValue("@Descricao", Descricao + usuario.Id + " - " + usuario.Nome);
                    cmd.Parameters.AddWithValue("@Data_hora", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id_Usuario", usuario.Id);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
