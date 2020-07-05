using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CallCenterMVC.EF;
using CallCenterMVC.Models;
using Microsoft.AspNet.Identity;

namespace CallCenterMVC.Controllers
{
    [Authorize(Roles ="Manager")]
    public class ManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CallCenterAgentsEntities managerDb = null;

        // GET: Manager
        public ActionResult Index()
        {
            // This method will create a dashboard for the manager
            using (managerDb = new CallCenterAgentsEntities())
            {
                var userId = User.Identity.GetUserId();
                var teams = (from tm in managerDb.ManagerTeams
                           join t in managerDb.Teams on tm.TeamId equals t.TeamId
                           where tm.ManagerId.Equals(userId) && t.Active == true
                           select t).ToList();

                var agents = (from ta in managerDb.AgentTeams
                              join u in managerDb.AspNetUsers on ta.AgentId equals u.Id
                              join t in managerDb.Teams on ta.TeamId equals t.TeamId
                           where ta.ManagerId.Equals(userId)
                           select  new User { Id= u.Id, FirstName= u.FirstName, LastName = u.LastName, TeamName = t.TeamName }).ToList();

                ManagerIndexModel indexModel = new ManagerIndexModel();
                indexModel.Teams = teams;
                indexModel.Agents = agents;

                indexModel.NumberOfTeams = teams.Count();
                indexModel.NumberOfAgents = agents.Count();

                return View(indexModel);
            };
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Team(TeamModel team)
        {
            if (ModelState.IsValid)
            {
                using (managerDb = new CallCenterAgentsEntities())
                {
                    var userId = User.Identity.GetUserId();
                    // Create a team and add the team to  the dataase
                    var newTeam = new Team() { TeamName = team.TeamName, Active= true };
                    managerDb.Teams.Add(newTeam);
                    managerDb.SaveChanges();
                    // Since the team has returned  an Id ,  map the team  to the team manager
                    managerDb.ManagerTeams.Add(new ManagerTeam() { ManagerId = userId, TeamId = newTeam.TeamId, CreatedOn = DateTime.Now });
                    managerDb.SaveChanges();

                    return RedirectToAction("Index");
                }
               
            }

            return RedirectToAction("Index");

        }


        [HttpGet]
        public JsonResult TeamAgent()
        {
            using (managerDb = new CallCenterAgentsEntities())
            {
                // select only user that are not  part of a team
               
                var agents = (from ta in managerDb.AspNetUsers
                              from u in managerDb.AgentTeams.Where(x => x.AgentId == ta.Id).DefaultIfEmpty()
                              where (u.AgentId == null && ta.AspNetRoles.FirstOrDefault().Name == "Agent")
                              select new User { Id = ta.Id, FirstName = ta.FirstName, LastName = ta.LastName, Email = ta.Email }).ToList();

                var teams = (from t in managerDb.Teams
                             where t.Active == true
                             select new TeamModel {Id = t.TeamId, TeamName = t.TeamName }).ToList();

                var listOfAgentAndTeamJson = new {  agent = agents, team = teams};
                return Json(listOfAgentAndTeamJson, JsonRequestBehavior.AllowGet);
            }
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamAgent(int teamId, string agentId)
        {
            using (managerDb = new CallCenterAgentsEntities())
            {
                var userId = User.Identity.GetUserId();
                managerDb.AgentTeams.Add(new AgentTeam() { AgentId = agentId, TeamId = teamId, ManagerId = userId });
                managerDb.SaveChanges();
                var message = "Team Added";
                return Json(message, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult DeleteTeam(string teamId)
        {
            using (managerDb = new CallCenterAgentsEntities())
            {
                var id = Convert.ToInt32(teamId);
                var teamToDelete = managerDb.Teams.Find(id);
                teamToDelete.Active = false;
                managerDb.Entry(teamToDelete).State =  EntityState.Modified;
                managerDb.SaveChanges();
                var message = "Team Deleted";
                return Json(message);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeam(TeamModel team)
        {
            using (managerDb = new CallCenterAgentsEntities())
            {
                var teamToDelete = managerDb.Teams.Find(team.Id);
                teamToDelete.TeamName = team.TeamName;

                managerDb.Entry(teamToDelete).State = EntityState.Modified;
                managerDb.SaveChanges();
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeamm(int teamId)
        {
            using (managerDb = new CallCenterAgentsEntities())
            {
                var teamToDelete  = managerDb.Teams.Find(teamId);
                //managerDb.AgentTeams.Add(new AgentTeam() { AgentId = agentId, TeamId = teamId, ManagerId = userId });
                managerDb.SaveChanges();
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferAgent(int teamId, int agentId)
        {
            // to change agent to another team
            using (managerDb = new CallCenterAgentsEntities())
            {
                var agentTeam = managerDb.AgentTeams.Find( agentId);
                agentTeam.TeamId = teamId;
                managerDb.Entry(agentTeam).State = EntityState.Modified;
                managerDb.SaveChanges();
                return View();
            }

        }

        //

        // GET: Manager/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = null; // db.ApplicationUsers.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,PicturePath,Address,City,LastLogin,CreatedOn,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                //db.ApplicationUsers.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: Manager/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = null; // db.ApplicationUsers.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Manager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,PicturePath,Address,City,LastLogin,CreatedOn,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Manager/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = null; // db.ApplicationUsers.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = null; // db.ApplicationUsers.Find(id);
            //db.ApplicationUsers.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
