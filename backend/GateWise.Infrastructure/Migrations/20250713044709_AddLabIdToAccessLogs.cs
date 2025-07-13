using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLabIdToAccessLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "access_logs",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lab_id",
                table: "access_logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_access_logs_lab_id",
                table: "access_logs",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "ix_access_logs_user_id",
                table: "access_logs",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_access_logs_labs_lab_id",
                table: "access_logs",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_access_logs_users_user_id",
                table: "access_logs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_access_logs_labs_lab_id",
                table: "access_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_access_logs_users_user_id",
                table: "access_logs");

            migrationBuilder.DropIndex(
                name: "ix_access_logs_lab_id",
                table: "access_logs");

            migrationBuilder.DropIndex(
                name: "ix_access_logs_user_id",
                table: "access_logs");

            migrationBuilder.DropColumn(
                name: "lab_id",
                table: "access_logs");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "access_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)");
        }
    }
}
