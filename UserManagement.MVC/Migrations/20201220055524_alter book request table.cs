using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagement.MVC.Migrations
{
    public partial class alterbookrequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReturnBy",
                schema: "Identity",
                table: "BookRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Identity",
                table: "BookRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnBy",
                schema: "Identity",
                table: "BookRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Identity",
                table: "BookRequests");
        }
    }
}
