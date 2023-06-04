using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Alteracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projetos_Usuarios_UsuariosId",
                table: "Projetos");

            migrationBuilder.DropForeignKey(
                name: "FK_TarefasStatus_Tarefas_TarefasId",
                table: "TarefasStatus");

            migrationBuilder.DropIndex(
                name: "IX_Projetos_UsuariosId",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "UsuariosId",
                table: "Projetos");

            migrationBuilder.AlterColumn<int>(
                name: "TarefasId",
                table: "TarefasStatus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TarefasStatus_Tarefas_TarefasId",
                table: "TarefasStatus",
                column: "TarefasId",
                principalTable: "Tarefas",
                principalColumn: "TarefasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarefasStatus_Tarefas_TarefasId",
                table: "TarefasStatus");

            migrationBuilder.AlterColumn<int>(
                name: "TarefasId",
                table: "TarefasStatus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuariosId",
                table: "Projetos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_UsuariosId",
                table: "Projetos",
                column: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projetos_Usuarios_UsuariosId",
                table: "Projetos",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "UsuariosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TarefasStatus_Tarefas_TarefasId",
                table: "TarefasStatus",
                column: "TarefasId",
                principalTable: "Tarefas",
                principalColumn: "TarefasId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
