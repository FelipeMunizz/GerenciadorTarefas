﻿using System.Data;
using System.Data.SqlClient;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository
{
    public class ProjetosRepository : IProjetosRepository
    {

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
                         insert into PROJETOS (NOME_PROJETO, DESCRICAO, DATA_INICIO, DATA_FIM, ID_USUARIO)
                                       values (@NomeProjeto, @Descricao, @DataInicio, @DataFim,  @IdUsuario)
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
                int result = await command.ExecuteNonQueryAsync();

                if (result < 0)
                {
                    await connection.CloseAsync();
                    throw new Exception("Não foi possivel adicionar o projeto");
                }

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

                if (result < 0)
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

                if (result < 0)
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

                if (result <= 0)
                    throw new Exception("Não foi possivel excluir o projeto");

                await connection.CloseAsync();
            }
        }
    }
}