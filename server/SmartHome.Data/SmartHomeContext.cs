﻿using Microsoft.EntityFrameworkCore;
using SmartHome.Data.Models;
using SmartHome.Data.Infrastructure.Extensions;
using SmartHome.Data.Model.Enums;
using System;

namespace SmartHome.Data
{
    public class SmartHomeContext : DbContext
    {
        public SmartHomeContext()
        {
        }

        public SmartHomeContext(DbContextOptions<SmartHomeContext> options) : base(options)
        {
        }

        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentData> ComponentData { get; set; }
        public DbSet<ComponentType> ComponentTypes { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<SmartHomeEntity> SmartHomeEntities { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnComponentCreated(modelBuilder);
            OnComponentDataCreated(modelBuilder);
            OnComponentTypeCreated(modelBuilder);
            OnModuleCreated(modelBuilder);
            OnRoomCreated(modelBuilder);
            OnSmartHomeEntityCreated(modelBuilder);
            OnUserCreated(modelBuilder);
            OnUserSmartHomeEntityCreated(modelBuilder);
        }

        private void OnComponentCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Component>();

            entity
                .ToTable("Component");

            entity
                .HasKey(x => x.ComponentId);

            entity
                .Property(x => x.ComponentState)
                .HasConversion
                (
                    x => x.ToStringExtended(),
                    x => (ComponentStateEnum)Enum.Parse(typeof(ComponentStateEnum), x)
                )
                .HasMaxLength(10)
                .IsRequired();

            entity
                .HasOne(x => x.ComponentType)
                .WithMany(x => x.Components)
                .HasForeignKey(x => x.ComponentTypeId)
                .IsRequired();

            entity
                .HasOne(x => x.Module)
                .WithMany(x => x.Components)
                .HasForeignKey(x => x.ModuleId)
                .IsRequired();

            entity
                .HasMany(x => x.ComponentData)
                .WithOne(x => x.Component)
                .HasForeignKey(x => x.ComponentId);
        }

        private void OnComponentDataCreated(ModelBuilder modelBuilder)
        {

            var entity = modelBuilder.Entity<ComponentData>();

            entity
                .ToTable("ComponentData");

            entity
                .HasKey(x => x.ComponentDataId);

            entity
               .Property(x => x.Reading)
               .HasColumnType("decimal(6, 2)")
               .IsRequired();

            entity
                .Property(x => x.Message)
                .HasMaxLength(30)
                .IsRequired();

            entity
                .Property(x => x.Timestamp)
                .IsRequired();

            entity
                .HasOne(x => x.Component)
                .WithMany(x => x.ComponentData)
                .HasForeignKey(x => x.ComponentId)
                .IsRequired();

        }

        private void OnComponentTypeCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ComponentType>();

            entity
                .ToTable("ComponentType");

            entity
                .HasKey(x => x.ComponentTypeId);

            entity
                .Property(x => x.Type)
                .HasConversion
                (
                    v => v.ToStringExtended(),
                    v => (ComponentTypeEnum)Enum.Parse(typeof(ComponentTypeEnum), v)
                )
                .HasMaxLength(30)
                .IsRequired();

            entity
                .Property(x => x.IsSwitchable)
                .IsRequired();

        }

        private void OnModuleCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Module>();

            entity
                .ToTable("Module");

            entity
                .HasKey(x => x.ModuleId);

            entity
                .Property(x => x.IsConnected)
                .IsRequired();

            entity
                .HasOne<Room>(x => x.Room)
                .WithMany(x => x.Modules)
                .HasForeignKey(x => x.RoomId)
                .IsRequired();

        }

        private void OnRoomCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Room>();

            entity
                 .ToTable("Room");

            entity
                .HasKey(x => x.RoomId);

            entity
                .Property(x => x.Name)
                .HasMaxLength(30)
                .IsRequired();

            entity
                .HasOne(x => x.SmartHomeEntity)
                .WithMany(x => x.Rooms)
                .HasForeignKey(x => x.SmartHomeEntityId)
                .IsRequired();
        }

        private void OnSmartHomeEntityCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<SmartHomeEntity>();

            entity
                .ToTable("SmartHomeEntity");

            entity
                .HasKey(x => x.SmartHomeEntityId);

            entity
                .Property(x => x.RegisterTimestamp)
                .IsRequired();

            entity
                .Property(x => x.IpAddress)
                .HasMaxLength(15)
                .IsFixedLength(true)
                .IsRequired();

        }

        private void OnUserCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity
                .ToTable("User");

            entity
                .HasKey(x => x.UserId);

            entity
               .Property(x => x.Username)
               .HasMaxLength(20)
               .IsRequired();

            entity
                .HasIndex(x => x.Username)
                .IsUnique();

            entity
                .HasIndex(x => x.Email)
                .IsUnique();

            entity
                .Property(x => x.Password)
                .HasMaxLength(64)
                .IsFixedLength(true)
                .IsRequired();

            entity
                .Property(x => x.Email)
                .HasMaxLength(40)
                .IsRequired();

        }

        private void OnUserSmartHomeEntityCreated(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<UserSmartHomeEntity>();

            entity
                .ToTable("UserSmartHomeEntity");

            entity
                .HasKey(x => new { x.UserId, x.SmartHomeEntityId });

            entity
                .HasOne(x => x.User)
                .WithMany(x => x.UserSmartHomeEntities)
                .HasForeignKey(x => x.UserId);

            entity
               .HasOne(x => x.SmartHomeEntity)
               .WithMany(x => x.UserSmartHomeEntities)
               .HasForeignKey(x => x.SmartHomeEntityId);

            entity
                .Property(x => x.IsAdmin)
                .IsRequired();
        }

    }
}