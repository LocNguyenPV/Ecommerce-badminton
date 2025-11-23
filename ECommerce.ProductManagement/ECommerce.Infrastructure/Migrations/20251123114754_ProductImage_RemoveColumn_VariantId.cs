using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductImage_RemoveColumn_VariantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Variants",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_VariantId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "ProductImages");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductVariantVariantId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductVariantVariantId",
                table: "ProductImages",
                column: "ProductVariantVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductVariants_ProductVariantVariantId",
                table: "ProductImages",
                column: "ProductVariantVariantId",
                principalTable: "ProductVariants",
                principalColumn: "VariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductVariants_ProductVariantVariantId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductVariantVariantId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductVariantVariantId",
                table: "ProductImages");

            migrationBuilder.AddColumn<Guid>(
                name: "VariantId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_VariantId",
                table: "ProductImages",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Variants",
                table: "ProductImages",
                column: "VariantId",
                principalTable: "ProductVariants",
                principalColumn: "VariantId");
        }
    }
}
