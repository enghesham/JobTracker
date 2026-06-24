using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_UserId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId",
                table: "Companies");

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM ""Companies"" WHERE ""UserId"" IS NULL) THEN
                        RAISE EXCEPTION 'Cannot apply AddCompanyOwnership while Companies.UserId contains null values. Assign every existing company to a user before running this migration.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Companies",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedWebsite",
                table: "Companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE ""Companies""
                SET ""NormalizedName"" = UPPER(BTRIM(""Name"")),
                    ""NormalizedWebsite"" = NULLIF(LOWER(RTRIM(BTRIM(COALESCE(""Website"", '')), '/')), '')
                WHERE ""NormalizedName"" = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId_NormalizedName",
                table: "Companies",
                columns: new[] { "UserId", "NormalizedName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_UserId",
                table: "Companies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_UserId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserId_NormalizedName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NormalizedWebsite",
                table: "Companies");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Companies",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_UserId",
                table: "Companies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
