using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace CLIProfessor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixVectorType2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "LearnedCorrections",
                type: "vector",
                nullable: true,
                oldClrType: typeof(float[]),
                oldType: "real[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float[]>(
                name: "Embedding",
                table: "LearnedCorrections",
                type: "real[]",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector",
                oldNullable: true);
        }
    }
}
