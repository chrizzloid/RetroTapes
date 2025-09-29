using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetroTapes.Migrations
{
    /// <inheritdoc />
    public partial class AddReservation_Clean : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    reservation_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(nullable: false),
                    inventory_id = table.Column<int>(nullable: false),
                    reserved_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getutcdate()"),
                    expires_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<byte>(nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation", x => x.reservation_id);
                    table.ForeignKey(
                        name: "FK_reservation_customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reservation_inventory_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventory",
                        principalColumn: "inventory_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservation_customer_id",
                table: "reservation",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_inventory_id",
                table: "reservation",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_inventory_status_expires",
                table: "reservation",
                columns: new[] { "inventory_id", "status", "expires_at" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "reservation");
        }


    }
}
