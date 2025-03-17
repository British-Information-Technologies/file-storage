﻿// <auto-generated />
using System;
using FileStorageService.www.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FileStorageService.www.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250317003031_InitialFileModels")]
    partial class InitialFileModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("FileStorageService.www.Data.FileBlock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("BlockNumber")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("BLOB");

                    b.Property<Guid>("FileHandleId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FileHandleId");

                    b.ToTable("FileBlocks");
                });

            modelBuilder.Entity("FileStorageService.www.Data.FileHandle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FileHandles");
                });

            modelBuilder.Entity("FileStorageService.www.Data.FileBlock", b =>
                {
                    b.HasOne("FileStorageService.www.Data.FileHandle", "FileHandle")
                        .WithMany("Blocks")
                        .HasForeignKey("FileHandleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FileHandle");
                });

            modelBuilder.Entity("FileStorageService.www.Data.FileHandle", b =>
                {
                    b.Navigation("Blocks");
                });
#pragma warning restore 612, 618
        }
    }
}
