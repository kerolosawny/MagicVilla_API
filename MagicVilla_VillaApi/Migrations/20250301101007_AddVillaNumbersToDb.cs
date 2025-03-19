using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_VillaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVillaNumbersToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "villaNumbers",
                columns: table => new
                {
                    VillaNo = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    SpecialDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_villaNumbers", x => x.VillaNo);
                    table.ForeignKey(
                        name: "FK_villaNumbers_villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 1, 12, 10, 6, 625, DateTimeKind.Local).AddTicks(850));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 1, 12, 10, 6, 625, DateTimeKind.Local).AddTicks(901));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 1, 12, 10, 6, 625, DateTimeKind.Local).AddTicks(904));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 1, 12, 10, 6, 625, DateTimeKind.Local).AddTicks(907));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 1, 12, 10, 6, 625, DateTimeKind.Local).AddTicks(910));

            migrationBuilder.CreateIndex(
                name: "IX_villaNumbers_VillaId",
                table: "villaNumbers",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "villaNumbers");

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 2, 25, 22, 19, 3, 276, DateTimeKind.Local).AddTicks(6292));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 2, 25, 22, 19, 3, 276, DateTimeKind.Local).AddTicks(6339));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 2, 25, 22, 19, 3, 276, DateTimeKind.Local).AddTicks(6342));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 2, 25, 22, 19, 3, 276, DateTimeKind.Local).AddTicks(6345));

            migrationBuilder.UpdateData(
                table: "villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 2, 25, 22, 19, 3, 276, DateTimeKind.Local).AddTicks(6348));
        }
    }
}
