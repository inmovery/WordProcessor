using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordProcessor.Migrations
{
	public partial class FixedMigration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "Count",
				table: "Words",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Count",
				table: "Words");
		}
	}
}
