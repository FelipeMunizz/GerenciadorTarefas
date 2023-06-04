using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Helpers;

public static class ProjetoHelpers
{
    public static Projetos ProjetoFinalizado(int idProjeto)
    {
        var query = @"
          select * from projetos where id_usuario = @IdUsuario and id_projeto = @IdProjeto
        ";

        Projetos projeto = new Projetos(); 

        using(SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdProjeto", idProjeto);
            var reader = command.ExecuteReader();

            if (reader == null)
                return projeto;
            if (reader.Read())
            {
                projeto.ProjetosId = (int)reader["ID_PROJETO"];
                projeto.NomeProjeto = (string)reader["NOME_PROJETO"];
                projeto.Descricao = (string)reader["DESCRICAO"];
                projeto.DataInicio = (DateTime)reader["DATA_INICIO"];
                projeto.DataFim = (DateTime)reader["DATA_FIM"];
            }

            reader.Close();
            connection.Close();

            return projeto;
        }
    }
}
