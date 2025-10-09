using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartAPI.Migrations;

/// <inheritdoc />
public partial class SeedMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Users",
            columns: ["Id", "Name"],
            values: new object[,]
            {
                {1, "John" },
                {2, "Francis" },
                {3, "Bruno" },
                {4, "Peter" },
                {5, "Gustave" },
                {6, "Maelle" },
            });

        migrationBuilder.InsertData(
            table: "Items",
            columns: ["Id", "Name", "Price"],
            values: new object[,]
            {
                {1, "Item 1", 10.22M },
                {2, "Item 2", 5.5M },
                {3, "Item 3", 22.30M },
                {4, "Item 4", 40.00M },
                {5, "Item 5", 5.99M  },
                {6, "Item 6", 1.15M },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
