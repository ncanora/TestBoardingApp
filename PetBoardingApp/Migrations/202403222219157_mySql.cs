﻿namespace PetBoardingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mySql : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingID = c.Guid(nullable: false),
                        BookingStartTime = c.DateTime(nullable: false, precision: 0),
                        BookingEndTime = c.DateTime(nullable: false, precision: 0),
                        PetID = c.Guid(nullable: false),
                        TimeCheckedIn = c.DateTime(precision: 0),
                        TimeCheckedOut = c.DateTime(precision: 0),
                        Name = c.String(nullable: false, unicode: false),
                        Notes = c.String(maxLength: 500, storeType: "nvarchar"),
                        Status = c.String(maxLength: 100, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.BookingID)
                .ForeignKey("dbo.Pets", t => t.PetID, cascadeDelete: true)
                .Index(t => t.PetID);
            
            CreateTable(
                "dbo.Pets",
                c => new
                    {
                        PetID = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Breed = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Age = c.Int(),
                        Notes = c.String(unicode: false),
                        Owner_Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.PetID)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        FirstName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        LastName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Email = c.String(maxLength: 256, storeType: "nvarchar"),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        PhoneNumber = c.String(unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderKey = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ContactUsSubmissions",
                c => new
                    {
                        ContactUsSubmissionID = c.Guid(nullable: false),
                        Email = c.String(nullable: false, unicode: false),
                        FirstName = c.String(maxLength: 50, storeType: "nvarchar"),
                        LastName = c.String(maxLength: 50, storeType: "nvarchar"),
                        Message = c.String(nullable: false, maxLength: 1000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ContactUsSubmissionID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Bookings", "PetID", "dbo.Pets");
            DropForeignKey("dbo.Pets", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Pets", new[] { "Owner_Id" });
            DropIndex("dbo.Bookings", new[] { "PetID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ContactUsSubmissions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Pets");
            DropTable("dbo.Bookings");
        }
    }
}
