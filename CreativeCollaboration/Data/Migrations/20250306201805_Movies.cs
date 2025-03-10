using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeCollaboration.Data.Migrations
{
    /// <inheritdoc />
    public partial class Movies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorDOB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorBirthPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorGender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorNationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorAwardWon = table.Column<int>(type: "int", nullable: false),
                    ActorDebutYear = table.Column<int>(type: "int", nullable: false),
                    ActorNetWorth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    StudioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudioName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudioCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudioEstablishedYear = table.Column<int>(type: "int", nullable: false),
                    StudioCEO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudioHeadquarter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasPic = table.Column<bool>(type: "bit", nullable: false),
                    PicExtension = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.StudioID);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieReleaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieDuration = table.Column<int>(type: "int", nullable: false),
                    MovieDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieBudget = table.Column<double>(type: "float", nullable: false),
                    MovieBoxOfficeCollection = table.Column<double>(type: "float", nullable: false),
                    MovieRating = table.Column<double>(type: "float", nullable: false),
                    MovieAwardNomination = table.Column<int>(type: "int", nullable: false),
                    MovieAwardWin = table.Column<int>(type: "int", nullable: false),
                    StudioID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieID);
                    table.ForeignKey(
                        name: "FK_Movies_Studios_StudioID",
                        column: x => x.StudioID,
                        principalTable: "Studios",
                        principalColumn: "StudioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActorMovie",
                columns: table => new
                {
                    ActorsActorId = table.Column<int>(type: "int", nullable: false),
                    MoviesMovieID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorMovie", x => new { x.ActorsActorId, x.MoviesMovieID });
                    table.ForeignKey(
                        name: "FK_ActorMovie_Actors_ActorsActorId",
                        column: x => x.ActorsActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorMovie_Movies_MoviesMovieID",
                        column: x => x.MoviesMovieID,
                        principalTable: "Movies",
                        principalColumn: "MovieID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovie_MoviesMovieID",
                table: "ActorMovie",
                column: "MoviesMovieID");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_StudioID",
                table: "Movies",
                column: "StudioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorMovie");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Studios");
        }
    }
}
