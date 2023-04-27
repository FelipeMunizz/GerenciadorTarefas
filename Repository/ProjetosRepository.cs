using System.Data;
using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Helpers.Interfaces;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository
{
    public class ProjetosRepository : IProjetosRepository
    {
        private readonly IUsuarioRepository _usuario;
        private readonly IEmailHelpers _email;

        public ProjetosRepository(IUsuarioRepository usuario, IEmailHelpers email)
        {
            _usuario = usuario;
            _email = email;
        }

        public async Task<List<Projetos>> ListarPorUsuario(int idUsuario)
        {
            string query = @"
            select * from projetos where id_usuario = @IdUsuario
            ";
            List<Projetos> projetos = new List<Projetos>();

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Projetos projeto = new Projetos
                    {
                        IdProjeto = (int)reader["ID_PROJETO"],
                        NomeProjeto = (string)reader["NOME_PROJETO"],
                        Descricao = (string)reader["DESCRICAO"],
                        DataInicio = (DateTime)reader["DATA_INICIO"],
                        IdUsuario = (int)reader["ID_USUARIO"]
                    };
                    projetos.Add(projeto);
                }
                reader.Close();
                await connection.CloseAsync();

                return projetos;
            }
        }

        public async Task<Projetos> ObterProjeto(int idProjeto, int idUsuario)
        {
            var query = @"
                select * from projetos where id_usuario = @IdUsuario and id_projeto = @IdProjeto
            ";

            Projetos projeto = new Projetos();

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdProjeto", idProjeto);
                var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                    throw new Exception("Projeto não encontrado");

                if (reader.Read())
                {
                    projeto.IdProjeto = (int)reader["ID_PROJETO"];
                    projeto.NomeProjeto = (string)reader["NOME_PROJETO"];
                    projeto.Descricao = (string)reader["DESCRICAO"];
                    projeto.DataInicio = (DateTime)reader["DATA_INICIO"];
                    projeto.DataFim = (DateTime)reader["DATA_FIM"];
                    projeto.IdUsuario = (int)reader["ID_USUARIO"];
                }

                reader.Close();
                await connection.CloseAsync();

                return projeto;
            }
        }        

        public async Task Add(Projetos projeto, int idUsuario)
        {
            string query = @"
                         INSERT INTO Projetos (NOME_PROJETO, DESCRICAO, DATA_INICIO, DATA_FIM, ID_USUARIO)
                         OUTPUT INSERTED.ID_PROJETO
                         VALUES (@NomeProjeto, @Descricao, @DataInicio, @DataFim, @IdUsuario)
            ";

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NomeProjeto", projeto.NomeProjeto);
                command.Parameters.AddWithValue("@Descricao", projeto.Descricao);
                command.Parameters.AddWithValue("@DataInicio", DateTime.Now.ToString("yyyy/MM/dd"));
                command.Parameters.AddWithValue("@DataFim", DateTime.MinValue.ToString("yyyy/MM/dd"));
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                await connection.OpenAsync();
                object idProjetoObj = await command.ExecuteScalarAsync();
                int idProjeto = idProjetoObj != null && int.TryParse(idProjetoObj.ToString(), out int result) ? result : 0;

                if (idProjeto < 1)
                {
                    await connection.CloseAsync();
                    throw new Exception("Não foi possivel adicionar o projeto");
                }

                Usuarios usuario = await _usuario.ObterUsuario(idUsuario);

                await AdicionarUsuarioProjeto(idProjeto, usuario.Usuario, idUsuario, true);

                await connection.CloseAsync();
            }
        }

        public async Task Update(Projetos projeto, int idUsuario)
        {
            string query = @"
            update Projetos set NOME_PROJETO = @NomeProjeto, DESCRICAO = @Descricao where ID_USUARIO = @IdUsuario and ID_PROJETO = @IdProjeto
            ";

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NomeProjeto", projeto.NomeProjeto);
                command.Parameters.AddWithValue("@Descricao", projeto.Descricao);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdProjeto", projeto.IdProjeto);

                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();

                if (result < 1)
                {
                    await connection.CloseAsync();
                    throw new Exception("Não foi possivel editar o projeto");
                }

                await connection.CloseAsync();
            }
        }        

        public async Task<Projetos> FinalizarProjeto(int idProjeto, int idUsuario)
        {
            string query = @"
                update Projetos set DATA_FIM = @DataFim where ID_USUARIO = @IdUsuario and ID_PROJETO = @IdProjeto
            ";
            
            Projetos projeto = ProjetoHelpers.ProjetoFinalizado(idProjeto, idUsuario);

            if (projeto == null)
                throw new Exception("Projeto não encontrado");

            if (projeto.DataFim > DateTime.MinValue)
                throw new Exception("Projeto já está finalizado");

            projeto.DataFim = DateTime.Now;

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@DataFim", projeto.DataFim.ToString("yyyy/MM/dd"));
                command.Parameters.AddWithValue("@IdProjeto", projeto.IdProjeto);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                int result = await command.ExecuteNonQueryAsync();

                if (result < 1)
                {
                    await connection.CloseAsync();
                    throw new Exception();
                }

                await connection.CloseAsync();
            }
            return projeto;
        }

        public async Task Delete(int idProjeto, int idUsuario)
        {
            string query = @"
            delete from projetos where id_usuario = @IdUsuario and id_projeto = @IdProjeto
            ";

            using (SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString()))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdProjeto", idProjeto);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                var result = await command.ExecuteNonQueryAsync();

                if (result < 1)
                    throw new Exception("Não foi possivel excluir o projeto");

                await connection.CloseAsync();
            }
        }
        public async Task AdicionarUsuarioProjeto(int idProjeto, string user, int idUsuarioResponsavel = 0, bool resoponsavel = false)
        {
            string query = @"
                insert into USUARIOS_PROJETO (ID_PROJETO, ID_USUARIO, RESPONSAVEL)
                values (@IdProjeto, @IdUsuario, @Responsavel)
            "
            ;

            Usuarios usuario = await _usuario.ObterUsuarioByUser(user);

            usuario.NomeCompleto();

            if (usuario == null)
                throw new Exception("Usuario nao encontrado");

            SqlConnection connection = new SqlConnection(AppDbContext.GetConnectionString());
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdProjeto", idProjeto);
            command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
            command.Parameters.AddWithValue("@Responsavel", resoponsavel);
            await connection.OpenAsync();

            var result = await command.ExecuteNonQueryAsync();

            if (result < 1)
                throw new Exception("Não foi possivel adicionar o usuario ao projeto");

            await connection.CloseAsync();

            if (!resoponsavel)
            {
                Usuarios usuarioResponsavel = await _usuario.ObterUsuario(idUsuarioResponsavel);
                Projetos projeto = await ObterProjeto(idProjeto, idUsuarioResponsavel);
                string assunto = "Você foi adicionado a um novo projeto";
                string mensagem = $"Olá {usuario.Nome}, você foi adicionado ao projeto {projeto.NomeProjeto}, por {usuarioResponsavel.NomeCompleto()}.";
                bool sucesso = _email.Enviar(usuario.Email, assunto, mensagem);

                if (!sucesso)
                    throw new Exception("Não foi possivel enviar o email");
            }
        }

        public Task RemoverUsuarioProjeto(int idProjeto, int idUsuarioProjeto, int idResponsavel)
        {
            throw new NotImplementedException();
        }
    }
}
