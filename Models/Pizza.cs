using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeploymentRun.Models
{

    public class Pizza
    {
        [Key]
        public int PizzaId {get;set;}
        [Required]
        public string Name {get;set;}
        // Foreign Key to MainUser.UserId
        public int UserId {get;set;}
        [Required]
        public string Description {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public int DaysSinceCreated 
        {
            get { return (DateTime.Now - CreatedAt).Days; }
        } 

        // Navigation Properties
        public MainUser Chef {get;set;}
        public List<Order> Orders {get;set;}

        //  var somePizza = dbContext.Pizzas.Include(u => u.Chef)
        //    .FirstOrDefault(u => u.PizzaId == 1);
        //  somePizza.Chef.FirstName => "Devon"

        //  user with the most pizzas!

        // var mostPizzas = dbContext.Users.Include(u => u.Pizzas)
        //                      .FirstOrDefault(u => u.Pizzas.Count == THE_MOST_PIZZAS)

    }
}