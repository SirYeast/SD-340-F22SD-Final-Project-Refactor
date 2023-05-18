using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_340_W22SD_Final_Project_Group6.Data.Migrations
{
    public partial class AddIdPropsToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ApplicationUser",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketWatchers_AspNetUsers_WatcherId",
                table: "TicketWatchers");

            migrationBuilder.RenameColumn(
                name: "ApplicationUser",
                table: "Tickets",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ApplicationUser",
                table: "Tickets",
                newName: "IX_Tickets_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "WatcherId",
                table: "TicketWatchers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketWatchers_AspNetUsers_WatcherId",
                table: "TicketWatchers",
                column: "WatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketWatchers_AspNetUsers_WatcherId",
                table: "TicketWatchers");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Tickets",
                newName: "ApplicationUser");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OwnerId",
                table: "Tickets",
                newName: "IX_Tickets_ApplicationUser");

            migrationBuilder.AlterColumn<string>(
                name: "WatcherId",
                table: "TicketWatchers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ApplicationUser",
                table: "Tickets",
                column: "ApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketWatchers_AspNetUsers_WatcherId",
                table: "TicketWatchers",
                column: "WatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
