using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProgramariMedicale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstitutiiPublice",
                columns: table => new
                {
                    Id_Institutie = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Denumire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutiiPublice", x => x.Id_Institutie);
                });

            migrationBuilder.CreateTable(
                name: "Pacienti",
                columns: table => new
                {
                    IDNP = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Info = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacienti", x => x.IDNP);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamente",
                columns: table => new
                {
                    Id_departament = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nume_departament = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sepecializare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id_MedInstitutie = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamente", x => x.Id_departament);
                    table.ForeignKey(
                        name: "FK_Departamente_InstitutiiPublice_Id_MedInstitutie",
                        column: x => x.Id_MedInstitutie,
                        principalTable: "InstitutiiPublice",
                        principalColumn: "Id_Institutie",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PacientRoles",
                columns: table => new
                {
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdRole = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacientRoles", x => new { x.IdUser, x.IdRole });
                    table.ForeignKey(
                        name: "FK_PacientRoles_Pacienti_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Pacienti",
                        principalColumn: "IDNP",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PacientRoles_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medic",
                columns: table => new
                {
                    IDNP = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specializare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_departament = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_instittutie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimpConsultatie = table.Column<TimeSpan>(type: "time", nullable: false),
                    WorkStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    WorkdayEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    DepartamenteId_departament = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medic", x => x.IDNP);
                    table.ForeignKey(
                        name: "FK_Medic_Departamente_DepartamenteId_departament",
                        column: x => x.DepartamenteId_departament,
                        principalTable: "Departamente",
                        principalColumn: "Id_departament");
                });

            migrationBuilder.CreateTable(
                name: "Programari",
                columns: table => new
                {
                    Idnp_Pacient = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Idnp_Med = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programari", x => new { x.Idnp_Pacient, x.Idnp_Med });
                    table.ForeignKey(
                        name: "FK_Programari_Medic_Idnp_Med",
                        column: x => x.Idnp_Med,
                        principalTable: "Medic",
                        principalColumn: "IDNP",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Programari_Pacienti_Idnp_Pacient",
                        column: x => x.Idnp_Pacient,
                        principalTable: "Pacienti",
                        principalColumn: "IDNP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2fc73f89-95f8-4507-869f-8564a09fe953"), "Pacient" },
                    { new Guid("35f3d3f9-f8cd-4ea3-af04-ac45bfe24fb5"), "Oaspete" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departamente_Id_MedInstitutie",
                table: "Departamente",
                column: "Id_MedInstitutie");

            migrationBuilder.CreateIndex(
                name: "IX_Medic_DepartamenteId_departament",
                table: "Medic",
                column: "DepartamenteId_departament");

            migrationBuilder.CreateIndex(
                name: "IX_PacientRoles_IdRole",
                table: "PacientRoles",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Programari_Idnp_Med",
                table: "Programari",
                column: "Idnp_Med");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PacientRoles");

            migrationBuilder.DropTable(
                name: "Programari");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Medic");

            migrationBuilder.DropTable(
                name: "Pacienti");

            migrationBuilder.DropTable(
                name: "Departamente");

            migrationBuilder.DropTable(
                name: "InstitutiiPublice");
        }
    }
}
