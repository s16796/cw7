using cw5.Models;
using cw5.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/enrollments")]

    public class EnrollmentsController : ControllerBase
    {

        readonly private IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }


        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult AddStudent(EnrollRequest request)
        {

            try
            {
                return Ok(_dbService.EnrollStudent(request));
            }catch(Exception exc)
            {
                return BadRequest(exc.Message);
            }

        }

        [HttpPost]
        [Route("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents(PromoteRequest request)
        {

            try
            {
                return Ok(_dbService.PromoteStudents(request));
            }catch(Exception exc)
            {
                return BadRequest(exc.Message);
            }
         
        }
    }
}
