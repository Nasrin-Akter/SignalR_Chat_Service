namespace project8.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using project8.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<project8.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(project8.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var result = roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Member"))
            {
                var result = roleManager.Create(new IdentityRole { Name = "Member" });
            }
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (!context.Users.Any(r => r.UserName == "a@a.com"))
            {
                var user = new ApplicationUser { UserName = "a@a.com", PhoneNumber = "123456" };
                var result = userManager.Create(user, "@Test123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }
            List<UserAccess> lst = new List<UserAccess>
            { 
                //new UserAccess {RoleName = "Admin",ActionName = "Create",ControllerName = "GroupInfoes",MenuItem = "Create"},
                //new UserAccess {RoleName = "Admin",ActionName = "PendingRequest",ControllerName = "RequestInfoes",MenuItem = "Request"},
                new UserAccess {RoleName = "Admin",ActionName = "Index",ControllerName = "GroupInfoes",MenuItem = "View"},
                //new UserAccess {RoleName = "Member",ActionName = "Index",ControllerName = "GroupInfoes",MenuItem = "View"},
                new UserAccess {RoleName = "Member",ActionName = "Chat",ControllerName = "Home",MenuItem = "Chat"},
            };
            lst.ForEach(s => context.UserAccesss.Add(s));

            //context.UserAccesss.Add(uaccess);
            context.SaveChanges();
        }
    }
}
