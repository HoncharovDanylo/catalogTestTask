﻿// <auto-generated />
using System;
using CatalogApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CatalogApplication.Migrations
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20231115174136_Seeding")]
    partial class Seeding
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CatalogApplication.Models.Catalog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Catalogs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Creating Digital Images"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Resources",
                            ParentId = 1
                        },
                        new
                        {
                            Id = 4,
                            Name = "Graphic Products",
                            ParentId = 1
                        },
                        new
                        {
                            Id = 3,
                            Name = "Evidence",
                            ParentId = 1
                        },
                        new
                        {
                            Id = 5,
                            Name = "Primary Sources",
                            ParentId = 2
                        },
                        new
                        {
                            Id = 6,
                            Name = "Secondary Sources",
                            ParentId = 2
                        },
                        new
                        {
                            Id = 7,
                            Name = "Process",
                            ParentId = 4
                        },
                        new
                        {
                            Id = 8,
                            Name = "Final Product",
                            ParentId = 4
                        });
                });

            modelBuilder.Entity("CatalogApplication.Models.Catalog", b =>
                {
                    b.HasOne("CatalogApplication.Models.Catalog", "Parent")
                        .WithMany("ChildCatalogs")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("CatalogApplication.Models.Catalog", b =>
                {
                    b.Navigation("ChildCatalogs");
                });
#pragma warning restore 612, 618
        }
    }
}
