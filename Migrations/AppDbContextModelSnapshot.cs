﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Data;

#nullable disable

namespace WebApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApi.Models.Anexos", b =>
                {
                    b.Property<int>("IdAnexo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdAnexo"));

                    b.Property<string>("Dados")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdTarefa")
                        .HasColumnType("int");

                    b.Property<string>("NomeAnexo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TarefasIdTarefa")
                        .HasColumnType("int");

                    b.HasKey("IdAnexo");

                    b.HasIndex("TarefasIdTarefa");

                    b.ToTable("Anexos");
                });

            modelBuilder.Entity("WebApi.Models.Projetos", b =>
                {
                    b.Property<int>("IdProjeto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProjeto"));

                    b.Property<DateTime>("DataFim")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataInicio")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<string>("NomeProjeto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UsuariosIdUsuario")
                        .HasColumnType("int");

                    b.HasKey("IdProjeto");

                    b.HasIndex("UsuariosIdUsuario");

                    b.ToTable("Projetos");
                });

            modelBuilder.Entity("WebApi.Models.Status", b =>
                {
                    b.Property<int>("IdStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdStatus"));

                    b.Property<string>("NomeStatus")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdStatus");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("WebApi.Models.Tarefas", b =>
                {
                    b.Property<int>("IdTarefa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTarefa"));

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdProjeto")
                        .HasColumnType("int");

                    b.Property<int>("PROJETOIdProjeto")
                        .HasColumnType("int");

                    b.Property<string>("StatusTarefa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titulo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdTarefa");

                    b.HasIndex("PROJETOIdProjeto");

                    b.ToTable("Tarefas");
                });

            modelBuilder.Entity("WebApi.Models.TarefasStatus", b =>
                {
                    b.Property<int>("IdTarefa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTarefa"));

                    b.Property<DateTime>("DataAlteracao")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdStatus")
                        .HasColumnType("int");

                    b.Property<int>("StatusIdStatus")
                        .HasColumnType("int");

                    b.Property<int>("TarefasIdTarefa")
                        .HasColumnType("int");

                    b.HasKey("IdTarefa");

                    b.HasIndex("StatusIdStatus");

                    b.HasIndex("TarefasIdTarefa");

                    b.ToTable("TarefasStatus");
                });

            modelBuilder.Entity("WebApi.Models.Usuarios", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Senha")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sobrenome")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Usuario")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdUsuario");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("WebApi.Models.UsuariosProjeto", b =>
                {
                    b.Property<int>("IdUsuarioProjeto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuarioProjeto"));

                    b.Property<int>("IdProjeto")
                        .HasColumnType("int");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<int>("ProjetoIdProjeto")
                        .HasColumnType("int");

                    b.Property<bool>("Responsavel")
                        .HasColumnType("bit");

                    b.Property<int>("UsuarioIdUsuario")
                        .HasColumnType("int");

                    b.HasKey("IdUsuarioProjeto");

                    b.HasIndex("ProjetoIdProjeto");

                    b.HasIndex("UsuarioIdUsuario");

                    b.ToTable("UsuariosProjetos");
                });

            modelBuilder.Entity("WebApi.Models.Anexos", b =>
                {
                    b.HasOne("WebApi.Models.Tarefas", "Tarefas")
                        .WithMany("Anexos")
                        .HasForeignKey("TarefasIdTarefa")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tarefas");
                });

            modelBuilder.Entity("WebApi.Models.Projetos", b =>
                {
                    b.HasOne("WebApi.Models.Usuarios", "Usuarios")
                        .WithMany()
                        .HasForeignKey("UsuariosIdUsuario");

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("WebApi.Models.Tarefas", b =>
                {
                    b.HasOne("WebApi.Models.Projetos", "PROJETO")
                        .WithMany()
                        .HasForeignKey("PROJETOIdProjeto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PROJETO");
                });

            modelBuilder.Entity("WebApi.Models.TarefasStatus", b =>
                {
                    b.HasOne("WebApi.Models.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusIdStatus")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApi.Models.Tarefas", "Tarefas")
                        .WithMany()
                        .HasForeignKey("TarefasIdTarefa")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Status");

                    b.Navigation("Tarefas");
                });

            modelBuilder.Entity("WebApi.Models.UsuariosProjeto", b =>
                {
                    b.HasOne("WebApi.Models.Projetos", "Projeto")
                        .WithMany()
                        .HasForeignKey("ProjetoIdProjeto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApi.Models.Usuarios", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioIdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Projeto");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("WebApi.Models.Tarefas", b =>
                {
                    b.Navigation("Anexos");
                });
#pragma warning restore 612, 618
        }
    }
}
