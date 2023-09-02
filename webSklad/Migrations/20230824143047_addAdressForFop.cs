using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webSklad.Migrations
{
    /// <inheritdoc />
    public partial class addAdressForFop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "FOPS",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "FOPS");
        }
    }
}
