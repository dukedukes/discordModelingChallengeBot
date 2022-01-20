﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModelChallengeBot.EF;

#nullable disable

namespace ModelChallengeBot.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20220119000826_addChallengeAcceptors")]
    partial class addChallengeAcceptors
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("ModelChallengeBot.EF.Models.AcceptedChallenge", b =>
                {
                    b.Property<int>("AcceptedChallengeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AcceptedTime")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ChallengeAcceptor")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ModelingChallengeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AcceptedChallengeId");

                    b.HasIndex("ModelingChallengeId");

                    b.ToTable("AcceptedChallenge");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.ChallengeImage", b =>
                {
                    b.Property<int>("ChallengeImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImagePath")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ModelingChallengeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ChallengeImageId");

                    b.HasIndex("ModelingChallengeId");

                    b.ToTable("ChallengeImage");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.ChannelRegistry", b =>
                {
                    b.Property<int>("ChannelRegistryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("RegisterId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("ChannelRegistryId");

                    b.ToTable("ChannelRegistry");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.ModelingChallenge", b =>
                {
                    b.Property<int>("ModelingChallengeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChallengeDescription")
                        .HasColumnType("TEXT");

                    b.Property<int>("ChallengeDurationMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ChallengeFinished")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("ChallengeFinishedThreadId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChallengeName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("ListingId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ModelingChallengeId");

                    b.ToTable("ModelingChallenge");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.RoleRegistry", b =>
                {
                    b.Property<int>("RoleRegistryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("RegisterId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("RoleRegistryId");

                    b.ToTable("RoleRegistry");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.Submission", b =>
                {
                    b.Property<int>("SubmissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ModelingChallengeId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Submitter")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("TimeTaken")
                        .HasColumnType("TEXT");

                    b.HasKey("SubmissionId");

                    b.HasIndex("ModelingChallengeId");

                    b.ToTable("Submission");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.AcceptedChallenge", b =>
                {
                    b.HasOne("ModelChallengeBot.EF.Models.ModelingChallenge", null)
                        .WithMany("ChallengeAcceptors")
                        .HasForeignKey("ModelingChallengeId");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.ChallengeImage", b =>
                {
                    b.HasOne("ModelChallengeBot.EF.Models.ModelingChallenge", null)
                        .WithMany("Images")
                        .HasForeignKey("ModelingChallengeId");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.Submission", b =>
                {
                    b.HasOne("ModelChallengeBot.EF.Models.ModelingChallenge", null)
                        .WithMany("Submissions")
                        .HasForeignKey("ModelingChallengeId");
                });

            modelBuilder.Entity("ModelChallengeBot.EF.Models.ModelingChallenge", b =>
                {
                    b.Navigation("ChallengeAcceptors");

                    b.Navigation("Images");

                    b.Navigation("Submissions");
                });
#pragma warning restore 612, 618
        }
    }
}