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
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Register()
        {
            return View();
        }

        // Create
        [HttpPost("create")]
        public IActionResult Create(MainUser user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Register");
                }

                PasswordHasher<MainUser> hasher = new PasswordHasher<MainUser>();
                user.Password = hasher.HashPassword(user, user.Password);

                var newUser = dbContext.Users.Add(user).Entity;
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("userId", newUser.UserId);

                return RedirectToAction("Index", "Pizza");
            }
            
            return View("Register");
        }
        [HttpGet("login")]
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                MainUser toLogin = dbContext.Users.FirstOrDefault(u => u.Email == user.EmailAttempt);
                if(toLogin == null)
                {
                    ModelState.AddModelError("EmailAttempt", "Invalid Email/Password");
                    return View("Login");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, toLogin.Password, user.PasswordAttempt);
                if(result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("EmailAttempt", "Invalid Email/Password");
                    return View("Login");
                }
                // Log user into session
                HttpContext.Session.SetInt32("userId", toLogin.UserId);
                return RedirectToAction("Index", "Pizza");
            }
            return View("Login");
        }
        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Register");
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            if(HttpContext.Session.GetInt32("userId") == null)
                return RedirectToAction("Register");
            return View();
        }
        [HttpGet("{userId}")]
        public IActionResult Show(int userId)
        {
            // query for a user
            // include PizzasOrdered
            // and theninclude pizza for each order

            var userModel = dbContext.Users
                // "u" refers to a MainUser
                .Include(u => u.PizzasOrdered)
                // "order" refers to a Order
                .ThenInclude(order => order.Pizza)
                .FirstOrDefault(u => u.UserId == userId);

            return View(userModel);
        }
    }
}
