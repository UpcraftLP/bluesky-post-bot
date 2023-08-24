﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Up.Bsky.PostBot.Database;

#nullable disable

namespace Up.Bsky.PostBot.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230824100621_TrackWebhooks")]
    partial class TrackWebhooks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BskyUserDiscordChannel", b =>
                {
                    b.Property<Guid>("DiscordChannelId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TrackedUsersId")
                        .HasColumnType("uuid");

                    b.HasKey("DiscordChannelId", "TrackedUsersId");

                    b.HasIndex("TrackedUsersId");

                    b.ToTable("BskyUserDiscordChannel");
                });

            modelBuilder.Entity("Up.Bsky.PostBot.Model.Bluesky.BskyUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Did")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Did")
                        .IsUnique();

                    b.ToTable("TrackedUsers");
                });

            modelBuilder.Entity("Up.Bsky.PostBot.Model.Bluesky.PostEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AtUri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserDid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AtUri")
                        .IsUnique();

                    b.HasIndex("UserDid");

                    b.ToTable("SeenPosts");
                });

            modelBuilder.Entity("Up.Bsky.PostBot.Model.Discord.DiscordChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("WebhookId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId")
                        .IsUnique();

                    b.ToTable("DiscordChannels");
                });

            modelBuilder.Entity("BskyUserDiscordChannel", b =>
                {
                    b.HasOne("Up.Bsky.PostBot.Model.Discord.DiscordChannel", null)
                        .WithMany()
                        .HasForeignKey("DiscordChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Up.Bsky.PostBot.Model.Bluesky.BskyUser", null)
                        .WithMany()
                        .HasForeignKey("TrackedUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Up.Bsky.PostBot.Model.Bluesky.PostEntry", b =>
                {
                    b.HasOne("Up.Bsky.PostBot.Model.Bluesky.BskyUser", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserDid")
                        .HasPrincipalKey("Did")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Up.Bsky.PostBot.Model.Bluesky.BskyUser", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
