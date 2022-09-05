﻿// <auto-generated />
using EmailListFilter.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmailListFilter.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("EmailListFilter.Entity.UserIndexed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(767)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.ToTable("UsersIndexed");
                });

            modelBuilder.Entity("EmailListFilter.Entity.UserUnindexed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UsersUnindexed");
                });
#pragma warning restore 612, 618
        }
    }
}