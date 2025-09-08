using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetroTapes.Migrations
{
    /// <inheritdoc />
    public partial class InitSakilaFromModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "actor",
                columns: table => new
                {
                    actor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    last_name = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__actor__8B2447B422F2400E", x => x.actor_id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__category__D54EE9B40E25CE9D", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    country_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    country = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__country__7E8CD055DF24035C", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "film_text",
                columns: table => new
                {
                    film_id = table.Column<short>(type: "smallint", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__film_tex__349764A9D7D4BF07", x => x.film_id);
                });

            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    language_id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__language__804CF6B387E7DDA7", x => x.language_id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    city = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    country_id = table.Column<short>(type: "smallint", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__city__031491A8401EF8D4", x => x.city_id);
                    table.ForeignKey(
                        name: "fk_city_country",
                        column: x => x.country_id,
                        principalTable: "country",
                        principalColumn: "country_id");
                });

            migrationBuilder.CreateTable(
                name: "film",
                columns: table => new
                {
                    film_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true, defaultValueSql: "(NULL)"),
                    release_year = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: true),
                    language_id = table.Column<byte>(type: "tinyint", nullable: false),
                    original_language_id = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(NULL)"),
                    rental_duration = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)3),
                    rental_rate = table.Column<decimal>(type: "decimal(4,2)", nullable: false, defaultValueSql: "((4.99))"),
                    length = table.Column<short>(type: "smallint", nullable: true, defaultValueSql: "(NULL)"),
                    replacement_cost = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValueSql: "((19.99))"),
                    rating = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "G"),
                    special_features = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__film__349764A9B8CD0D16", x => x.film_id);
                    table.ForeignKey(
                        name: "fk_film_language",
                        column: x => x.language_id,
                        principalTable: "language",
                        principalColumn: "language_id");
                    table.ForeignKey(
                        name: "fk_film_language_original",
                        column: x => x.original_language_id,
                        principalTable: "language",
                        principalColumn: "language_id");
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    address2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "(NULL)"),
                    district = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    city_id = table.Column<int>(type: "int", nullable: false),
                    postal_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValueSql: "(NULL)"),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__address__CAA247C8F43D9C09", x => x.address_id);
                    table.ForeignKey(
                        name: "fk_address_city",
                        column: x => x.city_id,
                        principalTable: "city",
                        principalColumn: "city_id");
                });

            migrationBuilder.CreateTable(
                name: "film_actor",
                columns: table => new
                {
                    actor_id = table.Column<int>(type: "int", nullable: false),
                    film_id = table.Column<int>(type: "int", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__film_act__086D31FECA82AC99", x => new { x.actor_id, x.film_id });
                    table.ForeignKey(
                        name: "fk_film_actor_actor",
                        column: x => x.actor_id,
                        principalTable: "actor",
                        principalColumn: "actor_id");
                    table.ForeignKey(
                        name: "fk_film_actor_film",
                        column: x => x.film_id,
                        principalTable: "film",
                        principalColumn: "film_id");
                });

            migrationBuilder.CreateTable(
                name: "film_category",
                columns: table => new
                {
                    film_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<byte>(type: "tinyint", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__film_cat__69C38A328EB0F374", x => new { x.film_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_film_category_category",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "fk_film_category_film",
                        column: x => x.film_id,
                        principalTable: "film",
                        principalColumn: "film_id");
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    last_name = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "(NULL)"),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    active = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false, defaultValue: "Y"),
                    create_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__customer__CD65CB85583B59C0", x => x.customer_id);
                    table.ForeignKey(
                        name: "fk_customer_address",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "address_id");
                });

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    inventory_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    film_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__inventor__B59ACC49A232CFB8", x => x.inventory_id);
                    table.ForeignKey(
                        name: "fk_inventory_film",
                        column: x => x.film_id,
                        principalTable: "film",
                        principalColumn: "film_id");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<byte>(type: "tinyint", nullable: false),
                    rental_id = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(NULL)"),
                    amount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__payment__ED1FC9EA086038D5", x => x.payment_id);
                    table.ForeignKey(
                        name: "fk_payment_customer",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                });

            migrationBuilder.CreateTable(
                name: "rental",
                columns: table => new
                {
                    rental_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rental_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    inventory_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    return_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    staff_id = table.Column<byte>(type: "tinyint", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__rental__67DB611B4865A07A", x => x.rental_id);
                    table.ForeignKey(
                        name: "fk_rental_customer",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "fk_rental_inventory",
                        column: x => x.inventory_id,
                        principalTable: "inventory",
                        principalColumn: "inventory_id");
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    staff_id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    last_name = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    picture = table.Column<byte[]>(type: "image", nullable: true, defaultValueSql: "(NULL)"),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "(NULL)"),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    username = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                    password = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true, defaultValueSql: "(NULL)"),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__staff__1963DD9C0EAD06A5", x => x.staff_id);
                    table.ForeignKey(
                        name: "fk_staff_address",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "address_id");
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    store_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    manager_staff_id = table.Column<byte>(type: "tinyint", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__store__A2F2A30C026725D4", x => x.store_id);
                    table.ForeignKey(
                        name: "fk_store_address",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "address_id");
                    table.ForeignKey(
                        name: "fk_store_staff",
                        column: x => x.manager_staff_id,
                        principalTable: "staff",
                        principalColumn: "staff_id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_actor_last_name",
                table: "actor",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "idx_fk_city_id",
                table: "address",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_country_id",
                table: "city",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_address_id",
                table: "customer",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_store_id",
                table: "customer",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "idx_last_name",
                table: "customer",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "idx_fk_language_id",
                table: "film",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_original_language_id",
                table: "film",
                column: "original_language_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_film_actor_actor",
                table: "film_actor",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_film_actor_film",
                table: "film_actor",
                column: "film_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_film_category_category",
                table: "film_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_film_category_film",
                table: "film_category",
                column: "film_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_film_id",
                table: "inventory",
                column: "film_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_film_id_store_id",
                table: "inventory",
                columns: new[] { "store_id", "film_id" });

            migrationBuilder.CreateIndex(
                name: "idx_fk_customer_id",
                table: "payment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_staff_id",
                table: "payment",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_rental_id",
                table: "payment",
                column: "rental_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_customer_id",
                table: "rental",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_inventory_id",
                table: "rental",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_staff_id",
                table: "rental",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "idx_uq",
                table: "rental",
                columns: new[] { "rental_date", "inventory_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fk_address_id",
                table: "staff",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_store_id",
                table: "staff",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "idx_fk_address_id",
                table: "store",
                column: "manager_staff_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fk_store_address",
                table: "store",
                column: "address_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customer_store",
                table: "customer",
                column: "store_id",
                principalTable: "store",
                principalColumn: "store_id");

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_store",
                table: "inventory",
                column: "store_id",
                principalTable: "store",
                principalColumn: "store_id");

            migrationBuilder.AddForeignKey(
                name: "fk_payment_rental",
                table: "payment",
                column: "rental_id",
                principalTable: "rental",
                principalColumn: "rental_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_payment_staff",
                table: "payment",
                column: "staff_id",
                principalTable: "staff",
                principalColumn: "staff_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rental_staff",
                table: "rental",
                column: "staff_id",
                principalTable: "staff",
                principalColumn: "staff_id");

            migrationBuilder.AddForeignKey(
                name: "fk_staff_store",
                table: "staff",
                column: "store_id",
                principalTable: "store",
                principalColumn: "store_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_address_city",
                table: "address");

            migrationBuilder.DropForeignKey(
                name: "fk_staff_address",
                table: "staff");

            migrationBuilder.DropForeignKey(
                name: "fk_store_address",
                table: "store");

            migrationBuilder.DropForeignKey(
                name: "fk_staff_store",
                table: "staff");

            migrationBuilder.DropTable(
                name: "film_actor");

            migrationBuilder.DropTable(
                name: "film_category");

            migrationBuilder.DropTable(
                name: "film_text");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "actor");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "rental");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "inventory");

            migrationBuilder.DropTable(
                name: "film");

            migrationBuilder.DropTable(
                name: "language");

            migrationBuilder.DropTable(
                name: "city");

            migrationBuilder.DropTable(
                name: "country");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "staff");
        }
    }
}
