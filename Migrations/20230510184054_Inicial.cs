using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.IdStatus);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sobrenome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Projetos",
                columns: table => new
                {
                    IdProjeto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProjeto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    UsuariosIdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projetos", x => x.IdProjeto);
                    table.ForeignKey(
                        name: "FK_Projetos_Usuarios_UsuariosIdUsuario",
                        column: x => x.UsuariosIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    IdTarefa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusTarefa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdProjeto = table.Column<int>(type: "int", nullable: false),
                    PROJETOIdProjeto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.IdTarefa);
                    table.ForeignKey(
                        name: "FK_Tarefas_Projetos_PROJETOIdProjeto",
                        column: x => x.PROJETOIdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjeto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosProjetos",
                columns: table => new
                {
                    IdUsuarioProjeto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdProjeto = table.Column<int>(type: "int", nullable: false),
                    Responsavel = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: false),
                    ProjetoIdProjeto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosProjetos", x => x.IdUsuarioProjeto);
                    table.ForeignKey(
                        name: "FK_UsuariosProjetos_Projetos_ProjetoIdProjeto",
                        column: x => x.ProjetoIdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjeto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosProjetos_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    IdAnexo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTarefa = table.Column<int>(type: "int", nullable: false),
                    NomeAnexo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarefasIdTarefa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.IdAnexo);
                    table.ForeignKey(
                        name: "FK_Anexos_Tarefas_TarefasIdTarefa",
                        column: x => x.TarefasIdTarefa,
                        principalTable: "Tarefas",
                        principalColumn: "IdTarefa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarefasStatus",
                columns: table => new
                {
                    IdTarefa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TarefasIdTarefa = table.Column<int>(type: "int", nullable: false),
                    StatusIdStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefasStatus", x => x.IdTarefa);
                    table.ForeignKey(
                        name: "FK_TarefasStatus_Status_StatusIdStatus",
                        column: x => x.StatusIdStatus,
                        principalTable: "Status",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TarefasStatus_Tarefas_TarefasIdTarefa",
                        column: x => x.TarefasIdTarefa,
                        principalTable: "Tarefas",
                        principalColumn: "IdTarefa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_TarefasIdTarefa",
                table: "Anexos",
                column: "TarefasIdTarefa");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_UsuariosIdUsuario",
                table: "Projetos",
                column: "UsuariosIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_PROJETOIdProjeto",
                table: "Tarefas",
                column: "PROJETOIdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_TarefasStatus_StatusIdStatus",
                table: "TarefasStatus",
                column: "StatusIdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TarefasStatus_TarefasIdTarefa",
                table: "TarefasStatus",
                column: "TarefasIdTarefa");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosProjetos_ProjetoIdProjeto",
                table: "UsuariosProjetos",
                column: "ProjetoIdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosProjetos_UsuarioIdUsuario",
                table: "UsuariosProjetos",
                column: "UsuarioIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "TarefasStatus");

            migrationBuilder.DropTable(
                name: "UsuariosProjetos");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Projetos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
