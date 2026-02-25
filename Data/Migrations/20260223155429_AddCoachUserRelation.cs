using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachBookingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Coaches",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_UserId",
                table: "Coaches",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_AspNetUsers_UserId",
                table: "Coaches",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_AspNetUsers_UserId",
                table: "Coaches");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_UserId",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coaches");
        }
    }
}
