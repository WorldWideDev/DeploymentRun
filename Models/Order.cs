using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeploymentRun.Models
{
    public class Order
    {
        [Key]
        public int OrderId {get;set;}
        public int UserId {get;set;}
        [Display(Name="Pizza")]
        public int PizzaId {get;set;}
        [Range(1, 100, ErrorMessage="Pizza quantity can only be 1 - 100")]
        public int Quantity {get;set;}
        public string Instructions {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public MainUser User {get;set;}
        public Pizza Pizza {get;set;}
    }
}