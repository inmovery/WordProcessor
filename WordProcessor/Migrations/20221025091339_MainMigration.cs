using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordProcessor.Migrations
{
	public partial class MainMigration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Words",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Frequency = table.Column<double>(type: "float", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Words", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Words");
		}
	}
}
