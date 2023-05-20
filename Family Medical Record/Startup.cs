using Family_Medical_Record.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Family_Medical_Record.Startup))]
namespace Family_Medical_Record
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUser();
        }

        private void CreateRolesAndUser()
        {
            ApplicationDbContext ADC = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ADC));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ADC));

            // Creating Admin Role and registring a default Admin User 
            if (!roleManager.RoleExists("Admin"))
            {

                // Creating Admin Role  
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                // Creating Admin User (Super User) for the website
                var user = new ApplicationUser();
                user.UserName = "Admin";
                user.Email = "admin@fmr.com";
                user.Picture = "/Icons/Admin.png";

                // Admin Default Password
                string userPWD = "Admin@fmr";

                var chkUser = UserManager.Create(user, userPWD);

                // Assigning Admin Role to this user   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }


                role = new IdentityRole();
                role.Name = "Customer";
                roleManager.Create(role);

                role = new IdentityRole();
                role.Name = "Doctor";
                roleManager.Create(role);
            }
        }
    }
}
