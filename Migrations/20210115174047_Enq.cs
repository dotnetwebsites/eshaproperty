using Microsoft.EntityFrameworkCore.Migrations;

namespace EeshaProperty.Migrations
{
    public partial class Enq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Landmark",
                table: "Enquiries",
                type: "nvarchar(300)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)");

            migrationBuilder.AddColumn<string>(
                name: "EmpCode",
                table: "Enquiries",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseType",
                table: "Enquiries",
                type: "nvarchar(300)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomeRange",
                table: "Enquiries",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpCode",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "HouseType",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "IncomeRange",
                table: "Enquiries");

            migrationBuilder.AlterColumn<string>(
                name: "Landmark",
                table: "Enquiries",
                type: "nvarchar(300)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldNullable: true);
        }
    }
}
