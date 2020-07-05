using CallCenterMVC.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterMVC.Models
{
    public class ManagerIndexModel
    {
        public List<Team> Teams { get; set; }

        public List<User> Agents { get; set; }

        public int NumberOfAgents { get; set; }

        public int NumberOfTeams{ get; set; }
    }


    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TeamName { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }

        public string PicturePath { get; set; }



    }

     public class TeamModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
    }
}