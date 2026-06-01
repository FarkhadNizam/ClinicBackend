using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBackend.Migrations
{
    /// <inheritdoc />
    public partial class Fix_ScheduleSlot_Id_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ScheduleSlots_ScheduleSlotId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ScheduleSlotId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ScheduleSlotId1",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleSlotId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduleSlotId",
                table: "Appointments",
                column: "ScheduleSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ScheduleSlots_ScheduleSlotId",
                table: "Appointments",
                column: "ScheduleSlotId",
                principalTable: "ScheduleSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ScheduleSlots_ScheduleSlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ScheduleSlotId",
                table: "Appointments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleSlotId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleSlotId1",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduleSlotId1",
                table: "Appointments",
                column: "ScheduleSlotId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ScheduleSlots_ScheduleSlotId1",
                table: "Appointments",
                column: "ScheduleSlotId1",
                principalTable: "ScheduleSlots",
                principalColumn: "Id");
        }
    }
}
