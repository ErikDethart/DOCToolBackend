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

        [HttpGet("{id}")]
        public ActionResult<TeamViewer> Get(string hostName)
        {
            TeamViewer? teamViewer = TeamViewerService.Get(hostName);

            if(teamViewer == null)
                return NotFound();

            return teamViewer;
        }

        [HttpPost]
        public IActionResult Create(TeamViewer teamViewer) {
            try {
                TeamViewerService.Add(teamViewer);
                return CreatedAtAction(nameof(Create), new {hostname = teamViewer.HostName}, teamViewer);
            } catch (SqliteException) {
                return Conflict();
            }
        }

        // PUT action

        // DELETE action
        [HttpDelete("{hostName}")]
        public IActionResult Delete(string hostName) {
            var teamViewer = TeamViewerService.Get(hostName);

            if (teamViewer is null) {
                return NotFound();
            }

            TeamViewerService.Delete(hostName);

            return NoContent();
        }
    }
}