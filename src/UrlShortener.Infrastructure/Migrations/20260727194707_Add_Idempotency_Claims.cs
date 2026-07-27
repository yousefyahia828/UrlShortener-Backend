using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Add_Idempotency_Claims : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "completed_on_utc",
            table: "idempotent_commands",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "status",
            table: "idempotent_commands",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "completed_on_utc",
            table: "idempotent_commands");

        migrationBuilder.DropColumn(
            name: "status",
            table: "idempotent_commands");
    }
}
