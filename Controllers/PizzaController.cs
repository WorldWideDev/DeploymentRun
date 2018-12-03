using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DeploymentRun.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DeploymentRun.Controllers
{
    [Route("pizza")]
    public class PizzaController : Controller
    {
        public MainUser ActiveUser
        {
            get { return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); }
        }
        private MyContext dbContext;
        public PizzaController(MyContext context)
        {
            dbContext = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            // check to see if user is in session,
            // if not return to login view
            if(ActiveUser == null)
                return RedirectToAction("Login", "Home");

            // Get all pizzas!
            var model = dbContext.Pizzas
                .Include(p => p.Chef)
                .Include(p => p.Orders).ToList();

            ViewBag.UserId = ActiveUser.UserId;

            // pass view model!
            return View(model);
        }
        [HttpGet("new")]
        public IActionResult New()
        {
            if(ActiveUser == null)
                return RedirectToAction("Login", "Home");
            return View();
        }
        [HttpPost("create")]
        public IActionResult Create(Pizza newPizza)
        {
            if(ModelState.IsValid)
            {
                newPizza.UserId = ActiveUser.UserId;

                // stages an insert!
                dbContext.Pizzas.Add(newPizza);

                // executes the insert
                dbContext.SaveChanges();

                
                // create pizza
                return RedirectToAction("Index");
            }
            ViewBag.UserId = ActiveUser.UserId;
            return View("New");
        }
        [HttpGet("{pizzaId}")]
        public IActionResult Show(int pizzaId)
        {
            if(ActiveUser == null)
                return RedirectToAction("Login", "Home");

            // get a pizza with pizza id

            Pizza onePizza = dbContext.Pizzas
                .Include(p => p.Chef)
                .Include(p => p.Orders)
                    .ThenInclude(o => o.User)
                .FirstOrDefault(p => p.PizzaId == pizzaId);

            // redirect if no pizza exists with pizzaId
            if(onePizza == null)
                return RedirectToAction("Index");

            return View(onePizza);

        }
        [HttpGet("edit/{pizzaId}")]
        public IActionResult Edit(int pizzaId)
        {
            if(ActiveUser == null)
                return RedirectToAction("Login", "Home");

            // get a pizza with pizza id

            Pizza onePizza = dbContext.Pizzas
                .Include(p => p.Chef).FirstOrDefault(p => p.PizzaId == pizzaId);

            // redirect if no pizza exists with pizzaId
            if(onePizza == null)
                return RedirectToAction("Index");

            return View(onePizza);
        }
        [HttpPost("update/{pizzaId}")]
        public IActionResult Update(Pizza pizza, int pizzaId)
        {
            if(ModelState.IsValid)
            {
                Pizza toUpdate = dbContext.Pizzas.FirstOrDefault(p => p.PizzaId == pizzaId);

                toUpdate.Name = pizza.Name;
                toUpdate.Chef = pizza.Chef;
                toUpdate.Description = pizza.Description;

                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            // update with pizza object and id
            return View("Edit");
           
        }
        [HttpGet("delete/{pizzaId}")]
        public IActionResult Delete(int pizzaId)
        {
            // query db for pizza with pizzaId
            Pizza toDelete = dbContext.Pizzas.FirstOrDefault(p => p.PizzaId == pizzaId);
            if(toDelete == null)
                return RedirectToAction("Index");

            // stages deletion
            dbContext.Pizzas.Remove(toDelete);

            // execute deletion
            dbContext.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpGet("results")]
        public IActionResult Results()
        {
            // find the user with the most pizzas
            var usersWithPizzas = dbContext.Users
                .Include(u => u.Pizzas);
            
            var maxPizzaNum = usersWithPizzas.Max(u => u.Pizzas.Count);

            var mostPizzas = usersWithPizzas.FirstOrDefault(u => u.Pizzas.Count == maxPizzaNum);

            ViewBag.MostPizzas = mostPizzas;
            return View();
        }
    }
}
