using DeploymentRun.Models;
using Microsoft.EntityFrameworkCore;

namespace DeploymentRun
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}
        // Your DB Sets go HERE!
        public DbSet<MainUser> Users {get;set;}
        public DbSet<Pizza> Pizzas {get;set;}
        public DbSet<Order> Orders {get;set;}
    }
}