using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsWithRefferenceModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelId",
                table: "EmployeeProjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjects_EmployeeModelId",
                table: "EmployeeProjects",
                column: "EmployeeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjects_ProjectId",
                table: "EmployeeProjects",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjects_Employees_EmployeeModelId",
                table: "EmployeeProjects",
                column: "EmployeeModelId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProjects_Projects_ProjectId",
                table: "EmployeeProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjects_Employees_EmployeeModelId",
                table: "EmployeeProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProjects_Projects_ProjectId",
                table: "EmployeeProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProjects_EmployeeModelId",
                table: "EmployeeProjects");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProjects_ProjectId",
                table: "EmployeeProjects");

            migrationBuilder.DropColumn(
                name: "EmployeeModelId",
                table: "EmployeeProjects");
        }
    }
}
