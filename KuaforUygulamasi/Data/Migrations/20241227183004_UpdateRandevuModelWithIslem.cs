using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforUygulamasi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRandevuModelWithIslem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_AspNetUsers_KullaniciID",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "YeniAlan",
                table: "Randevular");

            migrationBuilder.RenameColumn(
                name: "KullaniciID",
                table: "Randevular",
                newName: "KullaniciId");

            migrationBuilder.RenameIndex(
                name: "IX_Randevular_KullaniciID",
                table: "Randevular",
                newName: "IX_Randevular_KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_AspNetUsers_KullaniciId",
                table: "Randevular",
                column: "KullaniciId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_AspNetUsers_KullaniciId",
                table: "Randevular");

            migrationBuilder.RenameColumn(
                name: "KullaniciId",
                table: "Randevular",
                newName: "KullaniciID");

            migrationBuilder.RenameIndex(
                name: "IX_Randevular_KullaniciId",
                table: "Randevular",
                newName: "IX_Randevular_KullaniciID");

            migrationBuilder.AddColumn<string>(
                name: "YeniAlan",
                table: "Randevular",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_AspNetUsers_KullaniciID",
                table: "Randevular",
                column: "KullaniciID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
