using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforUygulamasi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUzmanlikAlaniToIslem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UzmanlikAlani",
                table: "Islemler",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UzmanlikAlani",
                table: "Islemler");
        }
    }
}
