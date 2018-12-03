using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using DeploymentRun.Models;

namespace DeploymentRun.Controllers
{
    [Route("orders")]
    public class OrderController : Controller
    {
        public MainUser ActiveUser
        {
            get { return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); }
        }
        private MyContext dbContext;
        public OrderController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            var viewModel = dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Pizza).ToList();
            
            ViewBag.UserId = ActiveUser.UserId;

            return View(viewModel);
        }
        [HttpGet("new")]
        public IActionResult New()
        {
            var allPizzas = dbContext.Pizzas.ToList();
            ViewBag.Pizzas = allPizzas;
        return View();
        }
        [HttpPost("create")]
        public IActionResult Create(Order order)
        {
            if(ModelState.IsValid)
            {
                order.UserId = ActiveUser.UserId;
                dbContext.Orders.Add(order);
                dbContext.SaveChanges();
                return RedirectToAction("Index", "Pizza");
            }
            var allPizzas = dbContext.Pizzas.ToList();
            ViewBag.Pizzas = allPizzas;
            return View("New");
        }

    }
}