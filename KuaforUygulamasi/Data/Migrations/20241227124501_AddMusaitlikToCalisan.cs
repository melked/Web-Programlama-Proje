using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforUygulamasi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMusaitlikToCalisan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusaitlikSaatleri",
                table: "Calisanlar");

            migrationBuilder.AlterColumn<string>(
                name: "Durum",
                table: "Randevular",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MusaitlikBaslangic",
                table: "Calisanlar",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MusaitlikBitis",
                table: "Calisanlar",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusaitlikBaslangic",
                table: "Calisanlar");

            migrationBuilder.DropColumn(
                name: "MusaitlikBitis",
                table: "Calisanlar");

            migrationBuilder.AlterColumn<string>(
                name: "Durum",
                table: "Randevular",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "MusaitlikSaatleri",
                table: "Calisanlar",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
