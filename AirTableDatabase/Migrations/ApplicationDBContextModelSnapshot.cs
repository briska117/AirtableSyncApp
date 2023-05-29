﻿// <auto-generated />
using System;
using AirTableDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AirTableDatabase.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AirTableDatabase.DBModels.ClientPrefix", b =>
                {
                    b.Property<string>("ClientPrefixId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ClientPrefixId");

                    b.ToTable("ClientPrefixes");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.CollectionMode", b =>
                {
                    b.Property<string>("CollectionModeId")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CollectionModeId");

                    b.ToTable("CollectionModes");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.CollectionModeRelatedTable", b =>
                {
                    b.Property<string>("CollectionModeRelatedTableId")
                        .HasColumnType("text");

                    b.Property<string>("CollectionModeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RelatedTableId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CollectionModeRelatedTableId");

                    b.ToTable("CollectionModeRelatedTables");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.CountryPrefix", b =>
                {
                    b.Property<string>("CountryPrefixId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CountryPrefixId");

                    b.ToTable("CountryPrefixes");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.Project", b =>
                {
                    b.Property<string>("ProjectId")
                        .HasColumnType("text");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClientPrefixId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CountryPrefixId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MainDatabaseID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Mode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TableSheetsToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ProjectId");

                    b.HasIndex("ClientPrefixId");

                    b.HasIndex("CountryPrefixId");

                    b.HasIndex("Mode");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.RelatedTable", b =>
                {
                    b.Property<string>("RelatedTableId")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TableId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RelatedTableId");

                    b.ToTable("RelatedTables");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.SyncEvent", b =>
                {
                    b.Property<string>("SyncEventId")
                        .HasColumnType("text");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SyncTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("SyncEventId");

                    b.ToTable("SyncEvents");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.SyncEventHistory", b =>
                {
                    b.Property<string>("SyncEventHistoryId")
                        .HasColumnType("text");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("FinishSync")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartAsync")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("StatusAsync")
                        .HasColumnType("integer");

                    b.Property<string>("SyncEventId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SyncEventHistoryId");

                    b.HasIndex("SyncEventId");

                    b.ToTable("EventHistories");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.UserProject", b =>
                {
                    b.Property<string>("UserProjectId")
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("UserProjects");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.Project", b =>
                {
                    b.HasOne("AirTableDatabase.DBModels.ClientPrefix", "ClientPrefix")
                        .WithMany()
                        .HasForeignKey("ClientPrefixId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AirTableDatabase.DBModels.CountryPrefix", "CountryPrefix")
                        .WithMany()
                        .HasForeignKey("CountryPrefixId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AirTableDatabase.DBModels.CollectionMode", "CollectionMode")
                        .WithMany()
                        .HasForeignKey("Mode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClientPrefix");

                    b.Navigation("CollectionMode");

                    b.Navigation("CountryPrefix");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.SyncEventHistory", b =>
                {
                    b.HasOne("AirTableDatabase.DBModels.SyncEvent", "SyncEvent")
                        .WithMany()
                        .HasForeignKey("SyncEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SyncEvent");
                });

            modelBuilder.Entity("AirTableDatabase.DBModels.UserProject", b =>
                {
                    b.HasOne("AirTableDatabase.DBModels.Project", "ProjectAsync")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectAsync");
                });
#pragma warning restore 612, 618
        }
    }
}
