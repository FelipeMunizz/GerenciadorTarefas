using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Alteracoes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosProjetos_Projetos_ProjetosId",
                table: "UsuariosProjetos");

            migrationBuilder.DropColumn(
                name: "IdProjeto",
                table: "UsuariosProjetos");

            migrationBuilder.RenameColumn(
                name: "ProjetosId",
                table: "UsuariosProjetos",
                newName: "ProjetoId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosProjetos_ProjetosId",
                table: "UsuariosProjetos",
                newName: "IX_UsuariosProjetos_ProjetoId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosProjetos_Projetos_ProjetoId",
                table: "UsuariosProjetos",
                column: "ProjetoId",
                principalTable: "Projetos",
                principalColumn: "ProjetosId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosProjetos_Projetos_ProjetoId",
                table: "UsuariosProjetos");

            migrationBuilder.RenameColumn(
                name: "ProjetoId",
                table: "UsuariosProjetos",
                newName: "ProjetosId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosProjetos_ProjetoId",
                table: "UsuariosProjetos",
                newName: "IX_UsuariosProjetos_ProjetosId");

            migrationBuilder.AddColumn<int>(
                name: "IdProjeto",
                table: "UsuariosProjetos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosProjetos_Projetos_ProjetosId",
                table: "UsuariosProjetos",
                column: "ProjetosId",
                principalTable: "Projetos",
                principalColumn: "ProjetosId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
