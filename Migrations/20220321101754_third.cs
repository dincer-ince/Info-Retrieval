using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoRetrieval.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "term",
                table: "Dictionary",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "term",
                table: "Dictionary");
        }
    }
}
