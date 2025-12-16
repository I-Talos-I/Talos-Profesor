using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace CLIProfessor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixVectorType3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "LearnedCorrections",
                type: "vector(768)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "LearnedCorrections",
                type: "vector",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(768)",
                oldNullable: true);
        }
    }
}
