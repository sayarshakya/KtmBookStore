using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagement.MVC.Migrations
{
    public partial class alterbookrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                schema: "Identity",
                table: "BookRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_BookId",
                schema: "Identity",
                table: "BookRequests",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookRequests_BookInventories_BookId",
                schema: "Identity",
                table: "BookRequests",
                column: "BookId",
                principalSchema: "Identity",
                principalTable: "BookInventories",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookRequests_BookInventories_BookId",
                schema: "Identity",
                table: "BookRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookRequests_BookId",
                schema: "Identity",
                table: "BookRequests");

            migrationBuilder.DropColumn(
                name: "BookId",
                schema: "Identity",
                table: "BookRequests");
        }
    }
}
