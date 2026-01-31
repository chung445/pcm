using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "999_Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "999_Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "999_Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "999_Transactions");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "999_Transactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "999_Transactions");
        }
    }
}
