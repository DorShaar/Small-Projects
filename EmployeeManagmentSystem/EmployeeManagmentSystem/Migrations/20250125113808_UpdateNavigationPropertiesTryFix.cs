using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNavigationPropertiesTryFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjects_Employees_EmployeeModelId",
                table: "EmployeeProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProjects_EmployeeModelId",
                table: "EmployeeProjects");

            migrationBuilder.DropColumn(
                name: "EmployeeModelId",
                table: "EmployeeProjects");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjects_Employees_EmployeeId",
                table: "EmployeeProjects",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjects_Employees_EmployeeId",
                table: "EmployeeProjects");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelId",
                table: "EmployeeProjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjects_EmployeeModelId",
                table: "EmployeeProjects",
                column: "EmployeeModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjects_Employees_EmployeeModelId",
                table: "EmployeeProjects",
                column: "EmployeeModelId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
