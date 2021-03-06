// <auto-generated />
using System;
using InfoRetrieval.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace InfoRetrieval.Migrations
{
    [DbContext(typeof(documentsContext))]
    [Migration("20220320222314_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("InfoRetrieval.Models.Documents", b =>
                {
                    b.Property<int>("docID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("processedDocument")
                        .HasColumnType("text");

                    b.Property<string>("rawDocument")
                        .HasColumnType("text");

                    b.Property<string[]>("terms")
                        .HasColumnType("text[]");

                    b.HasKey("docID");

                    b.ToTable("Documents");
                });
#pragma warning restore 612, 618
        }
    }
}
