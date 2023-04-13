﻿// <auto-generated />
using System;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bazza.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20230413074916_Sales_and_UserRights")]
    partial class Sales_and_UserRights
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bazza.Models.Database.Article", b =>
                {
                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int?>("SaleId")
                        .HasColumnType("int");

                    b.Property<string>("SaleUsername")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SaleUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Size")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PersonId", "ArticleId");

                    b.HasIndex("SaleId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Bazza.Models.Database.Person", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PersonId"));

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("PersonId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Bazza.Models.Database.Sale", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("Bazza.Models.Database.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DateTimeValue")
                        .HasColumnType("datetime2");

                    b.Property<int?>("NumberValue")
                        .HasColumnType("int");

                    b.Property<string>("StringValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Bazza.Models.Database.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("CanManageAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("CanManagePersons")
                        .HasColumnType("bit");

                    b.Property<bool>("CanManageSales")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PasswordSetUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("RequiresPasswordReset")
                        .HasColumnType("bit");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Bazza.Models.Database.Article", b =>
                {
                    b.HasOne("Bazza.Models.Database.Sale", "Sale")
                        .WithMany("Articles")
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Sale");
                });

            modelBuilder.Entity("Bazza.Models.Database.Sale", b =>
                {
                    b.Navigation("Articles");
                });
#pragma warning restore 612, 618
        }
    }
}
