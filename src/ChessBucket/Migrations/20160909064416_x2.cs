﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChessBucket.Migrations
{
    public partial class x2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreeNodeChildParents");

            migrationBuilder.CreateTable(
                name: "TreeNodeTransitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    ChildId = table.Column<int>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    San = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeNodeTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeNodeTransitions_TreeNodes_ChildId",
                        column: x => x.ChildId,
                        principalTable: "TreeNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeNodeTransitions_TreeNodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TreeNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreeNodeTransitions_ChildId",
                table: "TreeNodeTransitions",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeNodeTransitions_ParentId",
                table: "TreeNodeTransitions",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreeNodeTransitions");

            migrationBuilder.CreateTable(
                name: "TreeNodeChildParents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    ChildId = table.Column<int>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    San = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeNodeChildParents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeNodeChildParents_TreeNodes_ChildId",
                        column: x => x.ChildId,
                        principalTable: "TreeNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeNodeChildParents_TreeNodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TreeNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreeNodeChildParents_ChildId",
                table: "TreeNodeChildParents",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeNodeChildParents_ParentId",
                table: "TreeNodeChildParents",
                column: "ParentId");
        }
    }
}
