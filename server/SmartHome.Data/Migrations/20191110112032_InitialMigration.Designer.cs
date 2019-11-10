﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartHome.Data;

namespace SmartHome.Data.Migrations
{
    [DbContext(typeof(SmartHomeContext))]
    [Migration("20191110112032_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartHome.Data.Models.Component", b =>
                {
                    b.Property<Guid>("ComponentId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ComponentState")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<Guid>("ComponentTypeId");

                    b.Property<Guid>("ModuleId");

                    b.HasKey("ComponentId");

                    b.HasIndex("ComponentTypeId");

                    b.HasIndex("ModuleId");

                    b.ToTable("Component");
                });

            modelBuilder.Entity("SmartHome.Data.Models.ComponentData", b =>
                {
                    b.Property<Guid>("ComponentDataId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ComponentId");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<decimal>("Reading")
                        .HasColumnType("decimal(6, 2)");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("ComponentDataId");

                    b.HasIndex("ComponentId");

                    b.ToTable("ComponentData");
                });

            modelBuilder.Entity("SmartHome.Data.Models.ComponentType", b =>
                {
                    b.Property<Guid>("ComponentTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsSwitchable");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("ComponentTypeId");

                    b.ToTable("ComponentType");
                });

            modelBuilder.Entity("SmartHome.Data.Models.Module", b =>
                {
                    b.Property<Guid>("ModuleId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsConnected");

                    b.Property<Guid>("RoomId");

                    b.HasKey("ModuleId");

                    b.HasIndex("RoomId");

                    b.ToTable("Module");
                });

            modelBuilder.Entity("SmartHome.Data.Models.Room", b =>
                {
                    b.Property<Guid>("RoomId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<Guid>("SmartHomeEntityId");

                    b.HasKey("RoomId");

                    b.HasIndex("SmartHomeEntityId");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("SmartHome.Data.Models.SmartHomeEntity", b =>
                {
                    b.Property<Guid>("SmartHomeEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .IsFixedLength(true)
                        .HasMaxLength(15);

                    b.Property<DateTime>("RegisterTimestamp");

                    b.HasKey("SmartHomeEntityId");

                    b.ToTable("SmartHomeEntity");
                });

            modelBuilder.Entity("SmartHome.Data.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("Password")
                        .IsRequired()
                        .IsFixedLength(true)
                        .HasMaxLength(64);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("SmartHome.Data.Models.UserSmartHomeEntity", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("SmartHomeEntityId");

                    b.Property<bool>("IsAdmin");

                    b.HasKey("UserId", "SmartHomeEntityId");

                    b.HasIndex("SmartHomeEntityId");

                    b.ToTable("UserSmartHomeEntity");
                });

            modelBuilder.Entity("SmartHome.Data.Models.Component", b =>
                {
                    b.HasOne("SmartHome.Data.Models.ComponentType", "ComponentType")
                        .WithMany("Components")
                        .HasForeignKey("ComponentTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartHome.Data.Models.Module", "Module")
                        .WithMany("Components")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartHome.Data.Models.ComponentData", b =>
                {
                    b.HasOne("SmartHome.Data.Models.Component", "Component")
                        .WithMany("ComponentData")
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartHome.Data.Models.Module", b =>
                {
                    b.HasOne("SmartHome.Data.Models.Room", "Room")
                        .WithMany("Modules")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartHome.Data.Models.Room", b =>
                {
                    b.HasOne("SmartHome.Data.Models.SmartHomeEntity", "SmartHomeEntity")
                        .WithMany("Rooms")
                        .HasForeignKey("SmartHomeEntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SmartHome.Data.Models.UserSmartHomeEntity", b =>
                {
                    b.HasOne("SmartHome.Data.Models.SmartHomeEntity", "SmartHomeEntity")
                        .WithMany("UserSmartHomeEntities")
                        .HasForeignKey("SmartHomeEntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SmartHome.Data.Models.User", "User")
                        .WithMany("UserSmartHomeEntities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
