namespace CallCenterMVC.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CallCenterMVC.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CallCenterMVC.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string managerRoleName = "Manager";
            string agentRoleName ="Agent";
            if(!roleManager.RoleExists(managerRoleName) && !roleManager.RoleExists ( agentRoleName))
            {
                roleManager.Create(new IdentityRole(managerRoleName));
                roleManager.Create(new IdentityRole(agentRoleName));
            }

        }
    }
}
