using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoLive.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedOptionsToPriorities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HTMLcode",
                table: "TaskPriorityDB",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Option1",
                table: "TaskPriorityDB",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Option2",
                table: "TaskPriorityDB",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Option3",
                table: "TaskPriorityDB",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HTMLcode",
                table: "TaskPriorityDB");

            migrationBuilder.DropColumn(
                name: "Option1",
                table: "TaskPriorityDB");

            migrationBuilder.DropColumn(
                name: "Option2",
                table: "TaskPriorityDB");

            migrationBuilder.DropColumn(
                name: "Option3",
                table: "TaskPriorityDB");
        }
    }
}
