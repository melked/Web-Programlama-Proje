using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforUygulamasi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migrationnn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int[]>(
                name: "UzmanlikAlanlari",
                table: "Calisanlar",
                type: "integer[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UzmanlikAlanlari",
                table: "Calisanlar",
                type: "text",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");
        }
    }
}
