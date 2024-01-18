﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteColumToSharesErrorHandling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SharesErrorHandlings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "SharesErrorHandlings");
        }
    }
}
