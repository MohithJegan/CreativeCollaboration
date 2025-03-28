using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeCollaboration.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalInfoOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerAccountId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAccountId",
                table: "Orders");
        }
    }
}
