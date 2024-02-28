﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Orleans.Infra.SellerDb;

#nullable disable

namespace Orleans.Migrations
{
    [DbContext(typeof(SellerDbContext))]
    partial class SellerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Common.Entities.OrderEntry", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<int>("natural_key")
                        .HasColumnType("text");

                    b.Property<int>("customer_id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("delivery_date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("delivery_status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("freight_value")
                        .HasColumnType("real");

                    b.Property<int>("order_id")
                        .HasColumnType("integer");

                    b.Property<string>("order_status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("package_id")
                        .HasColumnType("integer");

                    b.Property<string>("product_category")
                        .HasColumnType("text");

                    b.Property<int>("product_id")
                        .HasColumnType("integer");

                    b.Property<string>("product_name")
                        .HasColumnType("text");

                    b.Property<int>("quantity")
                        .HasColumnType("integer");

                    b.Property<int>("seller_id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("shipment_date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("total_amount")
                        .HasColumnType("real");

                    b.Property<float>("total_incentive")
                        .HasColumnType("real");

                    b.Property<float>("total_invoice")
                        .HasColumnType("real");

                    b.Property<float>("total_items")
                        .HasColumnType("real");

                    b.Property<float>("unit_price")
                        .HasColumnType("real");

                    b.HasKey("id");

                    b.HasIndex(new[] { "seller_id" }, "seller_idx");

                    b.ToTable("order_entries", "public");
                });

            modelBuilder.Entity("Common.Integration.OrderSellerView", b =>
                {
                    b.Property<int>("count_items")
                        .HasColumnType("integer");

                    b.Property<int>("count_orders")
                        .HasColumnType("integer");

                    b.Property<int>("seller_id")
                        .HasColumnType("integer");

                    b.Property<float>("total_amount")
                        .HasColumnType("real");

                    b.Property<float>("total_freight")
                        .HasColumnType("real");

                    b.Property<float>("total_incentive")
                        .HasColumnType("real");

                    b.Property<float>("total_invoice")
                        .HasColumnType("real");

                    b.Property<float>("total_items")
                        .HasColumnType("real");

                    b.ToTable((string)null);

                    b.ToView("order_seller_view", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
