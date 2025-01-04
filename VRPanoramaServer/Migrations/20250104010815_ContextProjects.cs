using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VRPanoramaServer.Migrations
{
    /// <inheritdoc />
    public partial class ContextProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_UserId",
                schema: "identity",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                schema: "identity",
                table: "Project");

            migrationBuilder.RenameTable(
                name: "Project",
                schema: "identity",
                newName: "Projects",
                newSchema: "identity");

            migrationBuilder.RenameIndex(
                name: "IX_Project_UserId",
                schema: "identity",
                table: "Projects",
                newName: "IX_Projects_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                schema: "identity",
                table: "Projects",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                schema: "identity",
                table: "Projects",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                schema: "identity",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                schema: "identity",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "identity",
                newName: "Project",
                newSchema: "identity");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_UserId",
                schema: "identity",
                table: "Project",
                newName: "IX_Project_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                schema: "identity",
                table: "Project",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_UserId",
                schema: "identity",
                table: "Project",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
