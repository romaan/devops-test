using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevOpsDeploy.Migrations
{
    public partial class InitialMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    first_name = table.Column<string>(maxLength: 50, nullable: true),
                    last_name = table.Column<string>(maxLength: 50, nullable: true),
                    Activated = table.Column<bool>(nullable: false),
                    lang_key = table.Column<string>(maxLength: 6, nullable: true),
                    image_url = table.Column<string>(maxLength: 256, nullable: true),
                    activation_key = table.Column<string>(maxLength: 20, nullable: true),
                    reset_key = table.Column<string>(maxLength: 20, nullable: true),
                    reset_date = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "environment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_environment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "release",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    ProjectId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_release", x => x.Id);
                    table.ForeignKey(
                        name: "FK_release_project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deployment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DeployedAt = table.Column<DateTime>(nullable: false),
                    ReleaseId = table.Column<string>(nullable: false),
                    EnvironmentId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deployment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deployment_release_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "release",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "retention",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleaseId = table.Column<string>(nullable: false),
                    EnvironmentId = table.Column<string>(nullable: false),
                    ProjectId = table.Column<string>(nullable: false),
                    DeploymentId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_retention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_retention_deployment_DeploymentId",
                        column: x => x.DeploymentId,
                        principalTable: "deployment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_retention_environment_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "environment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_retention_project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_retention_release_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "release",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "config",
                columns: new[] { "Id", "Value" },
                values: new object[] { "ReleaseRetentionCount", 2 });

            migrationBuilder.InsertData(
                table: "environment",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "Environment-1", "Staging" },
                    { "Environment-2", "Production" }
                });

            migrationBuilder.InsertData(
                table: "project",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "Project-1", "Random Quotes" },
                    { "Project-2", "Pet Shop" }
                });

            migrationBuilder.InsertData(
                table: "release",
                columns: new[] { "Id", "Created", "ProjectId", "Version" },
                values: new object[,]
                {
                    { "Release-1", new DateTime(2000, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Project-1", "1.0.0" },
                    { "Release-2", new DateTime(2000, 1, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), "Project-1", "1.0.1" },
                    { "Release-3", new DateTime(2000, 1, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), "Project-1", null },
                    { "Release-4", new DateTime(2000, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Project-2", "1.0.0" },
                    { "Release-5", new DateTime(2000, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Project-2", "1.0.1-ci1" },
                    { "Release-6", new DateTime(2000, 1, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), "Project-2", "1.0.2" },
                    { "Release-7", new DateTime(2000, 1, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), "Project-2", "1.0.3" },
                    { "Release-8", new DateTime(2000, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Project-2", "2.0.0" }
                });

            migrationBuilder.InsertData(
                table: "deployment",
                columns: new[] { "Id", "DeployedAt", "EnvironmentId", "ReleaseId" },
                values: new object[,]
                {
                    { "Deployment-1", new DateTime(2000, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-1" },
                    { "Deployment-3", new DateTime(2000, 1, 2, 11, 0, 0, 0, DateTimeKind.Unspecified), "Environment-2", "Release-1" },
                    { "Deployment-2", new DateTime(2000, 1, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-2" },
                    { "Deployment-4", new DateTime(2000, 1, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), "Environment-2", "Release-2" },
                    { "Deployment-5", new DateTime(2000, 1, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-5" },
                    { "Deployment-6", new DateTime(2000, 1, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-6" },
                    { "Deployment-7", new DateTime(2000, 1, 2, 11, 0, 0, 0, DateTimeKind.Unspecified), "Environment-2", "Release-6" },
                    { "Deployment-9", new DateTime(2000, 1, 2, 14, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-6" },
                    { "Deployment-8", new DateTime(2000, 1, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-7" },
                    { "Deployment-10", new DateTime(2000, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Environment-1", "Release-8" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_deployment_ReleaseId",
                table: "deployment",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_release_ProjectId",
                table: "release",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_DeploymentId",
                table: "retention",
                column: "DeploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_EnvironmentId",
                table: "retention",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_ProjectId",
                table: "retention",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_retention_ReleaseId",
                table: "retention",
                column: "ReleaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "retention");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "deployment");

            migrationBuilder.DropTable(
                name: "environment");

            migrationBuilder.DropTable(
                name: "release");

            migrationBuilder.DropTable(
                name: "project");
        }
    }
}
