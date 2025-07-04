using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "labs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    imagem_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    floor = table.Column<int>(type: "integer", nullable: false),
                    building = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    open_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    close_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    registration_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    user_avatar_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_type = table.Column<int>(type: "integer", nullable: false),
                    operational_system = table.Column<string>(type: "text", nullable: false),
                    operational_system_version = table.Column<string>(type: "text", nullable: false),
                    device_model = table.Column<string>(type: "text", nullable: false),
                    device_manufacture_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "access_grants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authorized_user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    granted_by_user_id = table.Column<string>(type: "varchar(36)", nullable: true),
                    lab_id = table.Column<int>(type: "integer", nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access_grants", x => x.id);
                    table.ForeignKey(
                        name: "fk_access_grants_labs_lab_id",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_access_grants_users_authorized_user_id",
                        column: x => x.authorized_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_access_grants_users_granted_by_user_id",
                        column: x => x.granted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lab_access_managers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lab_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lab_access_managers", x => x.id);
                    table.ForeignKey(
                        name: "fk_lab_access_managers_labs_lab_id",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lab_access_managers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_access_grants_authorized_user_id",
                table: "access_grants",
                column: "authorized_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_access_grants_granted_by_user_id",
                table: "access_grants",
                column: "granted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_access_grants_lab_id",
                table: "access_grants",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "ix_lab_access_managers_lab_id",
                table: "lab_access_managers",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "ix_lab_access_managers_user_id",
                table: "lab_access_managers",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_grants");

            migrationBuilder.DropTable(
                name: "lab_access_managers");

            migrationBuilder.DropTable(
                name: "labs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
