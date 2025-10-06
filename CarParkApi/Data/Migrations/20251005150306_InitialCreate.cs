using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814

namespace CarParkApi.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        private const string TableName = "parking_spaces";
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: TableName,
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vehicle_reg = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    vehicle_type = table.Column<string>(type: "text", nullable: true),
                    space_number = table.Column<int>(type: "integer", nullable: false),
                    time_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    time_out = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_occupied = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parking_spaces", x => x.id);
                });

            migrationBuilder.InsertData(
                table: TableName,
                columns: new[] { "id", "space_number", "time_in", "time_out", "vehicle_reg", "vehicle_type" },
                values: new object[,]
                {
                    { 1, 1, null, null, null, null },
                    { 2, 2, null, null, null, null },
                    { 3, 3, null, null, null, null },
                    { 4, 4, null, null, null, null },
                    { 5, 5, null, null, null, null },
                    { 6, 6, null, null, null, null },
                    { 7, 7, null, null, null, null },
                    { 8, 8, null, null, null, null },
                    { 9, 9, null, null, null, null },
                    { 10, 10, null, null, null, null },
                    { 11, 11, null, null, null, null },
                    { 12, 12, null, null, null, null },
                    { 13, 13, null, null, null, null },
                    { 14, 14, null, null, null, null },
                    { 15, 15, null, null, null, null },
                    { 16, 16, null, null, null, null },
                    { 17, 17, null, null, null, null },
                    { 18, 18, null, null, null, null },
                    { 19, 19, null, null, null, null },
                    { 20, 20, null, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_parking_spaces_space_number",
                table: TableName,
                column: "space_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parking_spaces_vehicle_reg",
                table: TableName,
                column: "vehicle_reg",
                unique: true,
                filter: "vehicle_reg IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: TableName);
        }
    }
}
