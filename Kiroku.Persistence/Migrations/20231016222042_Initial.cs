using System;
using System.Text.Json;
using Kiroku.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kiroku.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:log_level", "trace,debug,info,warning,error,fatal");

            migrationBuilder.Sql(@"CREATE TABLE logs (
    id serial NOT NULL,
    level log_level NOT NULL,
    project_id text NOT NULL,
    instance_id text NULL,
    event_code text NOT NULL,
    time timestamp with time zone NOT NULL,
    data jsonb NOT NULL
) PARTITION BY RANGE (time);");

            migrationBuilder.CreateIndex(
                name: "IX_logs_event_code",
                table: "logs",
                column: "event_code");

            migrationBuilder.CreateIndex(
                name: "IX_logs_level",
                table: "logs",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_logs_project_id",
                table: "logs",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_logs_time",
                table: "logs",
                column: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs");
        }
    }
}
