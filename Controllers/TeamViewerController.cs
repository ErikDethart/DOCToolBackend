using DOCToolBackend.Models;
using DOCToolBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;

namespace DOCToolBackend.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class TeamViewerController : ControllerBase
    {
        public TeamViewerController() {
        }

        [HttpGet]
        public ActionResult<List<TeamViewer>> GetAll() {
            try {
                return TeamViewerService.GetAll();
            } catch (SqliteException e) {
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("{hostName}")]
        public ActionResult<TeamViewer> Get(string hostName)
        {
            TeamViewer? teamViewer = TeamViewerService.Get(hostName);

            if(teamViewer == null) {
                return NotFound();
            }

            return teamViewer;
        }

        [HttpPost]
        public IActionResult Create(TeamViewer teamViewer) {
            if (TeamViewerService.Get(teamViewer.HostName) is not null) {
                return Conflict();
            }

            try {
                TeamViewerService.Add(teamViewer);
                return CreatedAtAction(nameof(Create), new {hostname = teamViewer.HostName}, teamViewer);
            } catch (SqliteException e) {
                Console.WriteLine(Environment.StackTrace);
                return BadRequest();
            }
        }

        // PUT action
        [HttpPut("{hostName}")]
        public IActionResult Update(string hostName, TeamViewer teamViewer) {
            if (hostName != teamViewer.HostName) {
                return BadRequest();
            }
            
            TeamViewer? existingTeamViewer = TeamViewerService.Get(hostName);
            if (existingTeamViewer is null) {
                return NotFound();
            }

            TeamViewerService.Update(teamViewer);

            return NoContent();
        }

        // DELETE action
        [HttpDelete("{hostName}")]
        public IActionResult Delete(string hostName) {
            TeamViewer? teamViewer = TeamViewerService.Get(hostName);

            if (teamViewer is null) {
                return NotFound();
            }

            TeamViewerService.Delete(hostName);

            return NoContent();
        }
    }
}