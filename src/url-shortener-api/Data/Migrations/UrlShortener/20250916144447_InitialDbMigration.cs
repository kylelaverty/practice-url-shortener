using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Url.Shortener.Api.Data.Migrations.UrlShortener
{
    /// <inheritdoc />
    public partial class InitialDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "url");

            migrationBuilder.CreateTable(
                name: "shortened_urls",
                schema: "url",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    original_url = table.Column<string>(type: "text", nullable: false),
                    generated_code = table.Column<string>(type: "text", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shortened_urls", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shortened_urls_generated_code",
                schema: "url",
                table: "shortened_urls",
                column: "generated_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shortened_urls",
                schema: "url");
        }
    }
}
