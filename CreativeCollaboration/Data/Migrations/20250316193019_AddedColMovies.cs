using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeCollaboration.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedColMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MovieCost",
                table: "Movies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieCost",
                table: "Movies");
        }
    }
}
