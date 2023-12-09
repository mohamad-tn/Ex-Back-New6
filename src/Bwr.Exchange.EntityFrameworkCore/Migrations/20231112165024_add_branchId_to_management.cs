using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bwr.Exchange.Migrations
{
    public partial class add_branchId_to_management : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ManagementStatement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ManagementStatement");
        }
    }
}
