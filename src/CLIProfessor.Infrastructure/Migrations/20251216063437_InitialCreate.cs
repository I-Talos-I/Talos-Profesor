using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLIProfessor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "CommandRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NaturalLanguageInput = table.Column<string>(type: "text", nullable: false),
                    Context_OS = table.Column<string>(type: "text", nullable: false),
                    Context_Shell = table.Column<string>(type: "text", nullable: false),
                    Context_CurrentDirectory = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearnedCorrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalInput = table.Column<string>(type: "text", nullable: false),
                    CorrectedCommand = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: false),
                    Embedding = table.Column<float[]>(type: "real[]", nullable: true),
                    LearnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnedCorrections", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandRequests");

            migrationBuilder.DropTable(
                name: "LearnedCorrections");
        }
    }
}
