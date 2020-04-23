using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsORM.Models;
using Microsoft.EntityFrameworkCore;

namespace SportsORM.Controllers
{
    public class HomeController : Controller
    {

        private static Context context;

        public HomeController(Context DBContext)
        {
            context = DBContext;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.BaseballLeagues = context.Leagues
                .Where(l => l.Sport.Contains("Baseball"));
            return View();
        }

        [HttpGet("level_1")]
        public IActionResult Level1()
        {
            // ~~~~~~~~~~~ Leagues ~~~~~~~~~~~
            // #1: Womens
            ViewBag.Level1One = context.Leagues
                .Where(l => l.Name.Contains("Womens'"));
            // #2: Hockey
            ViewBag.Level1Two = context.Leagues
                .Where(l => l.Sport.Contains("Hockey"));
            // #3: Any sport but football
            ViewBag.Level1Three = context.Leagues
                .Where(l => !l.Sport.Contains("Football"));
            // #4: Conferences
            ViewBag.Level1Four = context.Leagues
                .Where(l => l.Name.Contains("Conference"));
            // #5: Atlantic
            ViewBag.Level1Five = context.Leagues
                .Where(l => l.Name.Contains("Atlantic"));
            
            // ~~~~~~~~~~~ Teams ~~~~~~~~~~~
            // #6: Location Dallas
            ViewBag.Level1Six = context.Teams
                .Where(t => t.Location.Contains("Dallas"));
            // #7: Name Raptors
            ViewBag.Level1Seven = context.Teams
                .Where(t => t.TeamName.Contains("Raptors"));
            // #8: Location "City"
            ViewBag.Level1Eight = context.Teams
                .Where(t => t.Location.Contains("City"));
            // #9: Name starts with "T"
            ViewBag.Level1Nine = context.Teams
                .Where(t => t.TeamName.StartsWith("T"));
            // #10: All Teams (Alphabetical Location)
            ViewBag.Level1Ten = context.Teams
                .OrderBy(t => t.Location);
            // #11: All Teams (Alphabetical Team Name, Reverse)
            ViewBag.Level1Eleven = context.Teams
                .OrderByDescending(t => t.TeamName);
            
            // ~~~~~~~~~~~ Players ~~~~~~~~~~~
            // #12: All Last Name "Cooper" Players
            ViewBag.Level1Twelve = context.Players
                .Where(p => p.LastName == "Cooper");
            // #13: All First Name "Joshua" Players
            ViewBag.Level1Thirteen = context.Players
                .Where(p => p.FirstName == "Joshua");

            // #14: All Last Name "Cooper" Players except "Joshua Cooper"
            ViewBag.Level1Fourteen = context.Players
                .Where(p => p.LastName == "Cooper" && p.FirstName !="Joshua");
            // #15: All Players with First Name "Alexander" or "Wyatt"
            ViewBag.Level1Fifteen = context.Players
                .Where(p => p.FirstName == "Alexander" || p.FirstName == "Wyatt")
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName);
            
            return View();
        }

        [HttpGet("level_2")]
        public IActionResult Level2()
        {
            // #1: All Teams in "Atlantic Soccer Conference"
            ViewBag.Level2One = context.Teams
                .Include(team => team.CurrLeague)
                .Where(t => t.CurrLeague.Name == "Atlantic Soccer Conference");
            
            // #2: All Current Players on "Boston Penguins"
            ViewBag.Level2Two = context.Players
                .Include(player => player.CurrentTeam)
                .Where(p => p.CurrentTeam.Location == "Boston")
                .Where(p => p.CurrentTeam.TeamName == "Penguins");
            
            // #3: All Teams in "International Collegiate Baseball Conference"
            ViewBag.Level2Three = context.Teams
                .Include(team => team.CurrLeague)
                .Where(t => t.CurrLeague.Name == "International Collegiate Baseball Conference");
            
            // #4: All Teams in "American Conference of Amateur Football"
            ViewBag.Level2Four = context.Teams
                .Include(team => team.CurrLeague)
                .Where(t => t.CurrLeague.Name == "American Conference of Amateur Football");
            
            // #5: All Football Teams
            ViewBag.Level2Five = context.Teams
                .Include(team => team.CurrLeague)
                .Where(t => t.CurrLeague.Sport == "Football");
            
            // #6: All Teams with a Player "Sophia"
            ViewBag.Level2Six = context.Players
                .Where(p => p.FirstName == "Sophia" || p.LastName == "Sophia")
                .Select(p => p.CurrentTeam)
                .ToList();

            // #7: All Players with LastName "Flores" who doesn't currently play for the Raptors
            ViewBag.Level2Seven = context.Players
                .Include(player => player.CurrentTeam)
                .Where(p => p.LastName == "Flores");
                // The "No Raptors" filter is currently done in cshtml, but
                //      could have also been done here with a 2nd .Where
                // .Where(p => p.CurrentTeam.TeamName != "Raptors");

            // #8: All current players with the "Manitoba Tiger-Cats"
            ViewBag.Level2Eight = context.Players
                .Include(player => player.CurrentTeam)
                .Where(p => p.CurrentTeam.Location == "Manitoba")
                .Where(p => p.CurrentTeam.TeamName == "Tiger-Cats")
                .ToList();

            // #9: All Teams with 12+ Players All Time
            ViewBag.Level2Nine = context.Teams
                .Include(team => team.AllPlayers)
                .Include(team => team.CurrLeague)
                .Where(t => t.AllPlayers.Count >= 12)
                .ToList();

            return View();
        }

        [HttpGet("level_3")]
        public IActionResult Level3()
        {
            // #1: All Teams "Alexander Bailey" has played with
            ViewBag.Level3OneCurr = context.Players
                .Include(player => player.CurrentTeam)
                .Where(p => p.FirstName == "Alexander" && p.LastName == "Bailey");
            ViewBag.Level3OnePast = context.Players
                .Include(player => player.AllTeams)
                .ThenInclude(pt => pt.TeamOfPlayer)
                .Where(p => p.FirstName == "Alexander" && p.LastName == "Bailey");

            // #2 All Players who have played on the Manitoba Tiger-Cats
            ViewBag.Level3TwoCurr = context.Players
                .Include(player => player.CurrentTeam)
                .Where(p => p.CurrentTeam.Location == "Manitoba" && p.CurrentTeam.TeamName == "Tiger-Cats");
            ViewBag.Level3TwoPast = context.Teams
                .Include(team => team.AllPlayers)
                .ThenInclude(pt => pt.PlayerOnTeam)
                .Where(t => t.Location == "Manitoba" && t.TeamName == "Tiger-Cats");

            // #3 All Former Players who have played on the Wichita Vikings
            ViewBag.Level3Three = context.Teams
                .Include(team => team.AllPlayers)
                .ThenInclude(pt => pt.PlayerOnTeam)
                .Where(t => t.Location == "Wichita" && t.TeamName == "Vikings");
            
            // #4 Every team that "Emily Sanchez" played for before she joined the Indiana Royals
            ViewBag.Level3Four = context.Players
                .Include(player => player.AllTeams)
                .ThenInclude(pt => pt.TeamOfPlayer)
                .Where(p => p.FirstName == "Emily" && p.LastName == "Sanchez");
            
            // #5 Everyone named "Levi" who has ever played in the Atlantic Federation of Amateur Baseball Players
            ViewBag.Level3Five = context.Players
                .Include(p => p.CurrentTeam)
                .ThenInclude(ct => ct.CurrLeague)
                .Include(p => p.AllTeams)
                .ThenInclude(at => at.TeamOfPlayer)
                .ThenInclude(tp => tp.CurrLeague)
                // Name is Levi AND (they have at least one team with league name match OR their current team league name matches)
                .Where(p => p.FirstName == "Levi" && (
                    p.AllTeams.Where(at => at.TeamOfPlayer.CurrLeague.Name == "Atlantic Federation of Amateur Baseball Players").Count() > 0
                    || 
                    p.CurrentTeam.CurrLeague.Name == "Atlantic Federation of Amateur Baseball Players"
                ));

            ViewBag.Level3Six = context.Players
                .Include(p => p.AllTeams)
                .Include(p => p.CurrentTeam)
                .OrderByDescending(p => p.AllTeams.Count());

            return View();
        }

    }
}