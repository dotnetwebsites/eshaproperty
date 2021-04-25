using Microsoft.EntityFrameworkCore.Migrations;

namespace EeshaProperty.Migrations
{
    public partial class updenq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeRange",
                table: "Enquiries");

            migrationBuilder.AddColumn<string>(
                name: "AlternateNo",
                table: "Enquiries",
                type: "nvarchar(20)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateNo",
                table: "Enquiries");

            migrationBuilder.AddColumn<int>(
                name: "IncomeRange",
                table: "Enquiries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
