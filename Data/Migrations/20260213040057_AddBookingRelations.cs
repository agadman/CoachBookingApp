using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachBookingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "Timeslots",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "Timeslots");
        }
    }
}
