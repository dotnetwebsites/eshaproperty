using Microsoft.EntityFrameworkCore.Migrations;

namespace EeshaProperty.Migrations
{
    public partial class updatecitystate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Enquiries");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Enquiries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Enquiries",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Enquiries");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Enquiries",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Enquiries",
                type: "nvarchar(100)",
                nullable: true);
        }
    }
}
