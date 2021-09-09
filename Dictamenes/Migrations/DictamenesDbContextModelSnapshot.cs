﻿// <auto-generated />
using System;
using Dictamenes.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dictamenes.Migrations
{
    [DbContext(typeof(DictamenesDbContext))]
    partial class DictamenesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.18");

            modelBuilder.Entity("Dictamenes.Models.ArchivoPDF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("contenido")
                        .HasColumnType("TEXT");

                    b.Property<string>("path")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ArchivoPDF");
                });

            modelBuilder.Entity("Dictamenes.Models.Asunto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<bool>("EstaActivo")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EstaHabilitado")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdUsuarioModificacion")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UsuarioModificacionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioModificacionId");

                    b.ToTable("Asunto");
                });

            modelBuilder.Entity("Dictamenes.Models.Dictamen", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ArchivoLigadoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Detalle")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EsPublico")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EstaActivo")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaCarga")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdArchivoLigado")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdAsunto")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdSujetoObligado")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdTipoDictamen")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdUsuarioGenerador")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdUsuarioModificacion")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NroExpediente")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(30);

                    b.Property<string>("NroGDE")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(30);

                    b.Property<int?>("UsuarioGeneradorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UsuarioModificacionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex("ArchivoLigadoId");

                    b.HasIndex("IdAsunto");

                    b.HasIndex("IdSujetoObligado");

                    b.HasIndex("IdTipoDictamen");

                    b.HasIndex("UsuarioGeneradorId");

                    b.HasIndex("UsuarioModificacionId");

                    b.ToTable("Dictamenes");
                });

            modelBuilder.Entity("Dictamenes.Models.SujetoObligado", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Apellido")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int>("CuilCuit")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EstaActivo")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdTipoSujetoObligado")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdUsuarioModificacion")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nombre")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("RazonSocial")
                        .HasColumnType("TEXT")
                        .HasMaxLength(80);

                    b.Property<int?>("UsuarioModificacionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.HasIndex("IdTipoSujetoObligado");

                    b.HasIndex("UsuarioModificacionId");

                    b.ToTable("SujetoObligado");
                });

            modelBuilder.Entity("Dictamenes.Models.TipoDictamen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<bool>("EstaActivo")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EstaHabilitado")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdUsuarioModificacion")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UsuarioModificacionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioModificacionId");

                    b.ToTable("TipoDictamen");
                });

            modelBuilder.Entity("Dictamenes.Models.TipoSujetoObligado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<bool>("EstaActivo")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EstaHabilitado")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdUsuarioModificacion")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UsuarioModificacionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioModificacionId");

                    b.ToTable("TipoSujetoObligado");
                });

            modelBuilder.Entity("Dictamenes.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Apellido")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int>("Cuil")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nombre")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("Dictamenes.Models.Asunto", b =>
                {
                    b.HasOne("Dictamenes.Models.Usuario", "UsuarioModificacion")
                        .WithMany()
                        .HasForeignKey("UsuarioModificacionId");
                });

            modelBuilder.Entity("Dictamenes.Models.Dictamen", b =>
                {
                    b.HasOne("Dictamenes.Models.ArchivoPDF", "ArchivoLigado")
                        .WithMany()
                        .HasForeignKey("ArchivoLigadoId");

                    b.HasOne("Dictamenes.Models.Asunto", "Asunto")
                        .WithMany()
                        .HasForeignKey("IdAsunto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dictamenes.Models.SujetoObligado", "SujetoObligado")
                        .WithMany()
                        .HasForeignKey("IdSujetoObligado")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dictamenes.Models.TipoDictamen", "TipoDictamen")
                        .WithMany()
                        .HasForeignKey("IdTipoDictamen")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dictamenes.Models.Usuario", "UsuarioGenerador")
                        .WithMany()
                        .HasForeignKey("UsuarioGeneradorId");

                    b.HasOne("Dictamenes.Models.Usuario", "UsuarioModificacion")
                        .WithMany()
                        .HasForeignKey("UsuarioModificacionId");
                });

            modelBuilder.Entity("Dictamenes.Models.SujetoObligado", b =>
                {
                    b.HasOne("Dictamenes.Models.TipoSujetoObligado", "TipoSujetoObligado")
                        .WithMany()
                        .HasForeignKey("IdTipoSujetoObligado")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dictamenes.Models.Usuario", "UsuarioModificacion")
                        .WithMany()
                        .HasForeignKey("UsuarioModificacionId");
                });

            modelBuilder.Entity("Dictamenes.Models.TipoDictamen", b =>
                {
                    b.HasOne("Dictamenes.Models.Usuario", "UsuarioModificacion")
                        .WithMany()
                        .HasForeignKey("UsuarioModificacionId");
                });

            modelBuilder.Entity("Dictamenes.Models.TipoSujetoObligado", b =>
                {
                    b.HasOne("Dictamenes.Models.Usuario", "UsuarioModificacion")
                        .WithMany()
                        .HasForeignKey("UsuarioModificacionId");
                });
#pragma warning restore 612, 618
        }
    }
}
