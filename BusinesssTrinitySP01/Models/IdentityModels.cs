using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BusinesssTrinitySP01.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string passNumber { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ClientAddress> cAddresses { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipment> ShippimgDetails { get; set; }
        public DbSet<ClientProfile> clientProfiles { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Employee> employees { get; set; } 
        public DbSet<OrderAssignment> orderAssignments { get; set; }
        public DbSet<Damaged> damaged { get; set; }
        public DbSet<EquipmentReturn> equipmentReturns { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<SupAddress> supAddresses { get; set; }
        public DbSet<RepairItem> repairItems { get; set; }
        public DbSet<Quote> quotes { get; set; }
        public DbSet<Rate> rates { get; set; }
        public DbSet<Theme> themes { get; set; }
        public DbSet<Color> colors { get; set; }
        public DbSet<DesignComp> designComps { get; set; }
        public DbSet<CompColor> compColors { get; set; }
        public DbSet<Package> packages { get; set; }
        public DbSet<PackageItem> packageItems { get; set; }
        public DbSet<Request> requests { get; set; }
        public DbSet<QuoteItem> quoteitems { get; set; }
        public DbSet<Job> jobs { get; set; }
        public DbSet<Applicant> applicants { get; set; }
        public DbSet<JobApplications> JobApplications { get; set; }
        public DbSet<QuoteAssignment> quoteAssignments { get; set; }


    }
}