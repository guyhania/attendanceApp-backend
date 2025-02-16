using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceApi.Data;
using AttendanceApi.Models;
using AttendanceApi.Data.UnitOfWork;
using System.Text.Json;
using NuGet.Common;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/AttendanceReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceReport>>> GetAttendanceReports()
        {
            return Ok(await _unitOfWork.AttendanceReports.GetAllReportsAsync());
        }

        // GET: api/AttendanceReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceReport>> GetAttendanceReport(int id)
        {
            var attendanceReport = await _unitOfWork.AttendanceReports.GetReportByIdAsync(id);

            if (attendanceReport == null)
            {
                return NotFound();
            }

            return Ok(attendanceReport);
        }

        // PUT: api/AttendanceReports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendanceReport(int id, [FromBody] Dictionary<string, object> updates)
        {
            var attendanceReport = await _unitOfWork.AttendanceReports.GetReportByIdAsync(id);
            if (attendanceReport == null)
            {
                return NotFound();
            }

            // Iterate through the provided updates and apply them dynamically
            foreach (var update in updates)
            {
                var property = typeof(AttendanceReport).GetProperty(update.Key);
                if (property != null && property.CanWrite)
                {

                    try
                    {
                        object? convertedValue = update.Value;
                        var newValue = "";

                        if (update.Value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
                        {
                            newValue = jsonElement.GetString()!; // Extract string
                        }

                        if (property.PropertyType == typeof(TimeSpan) || property.PropertyType == typeof(TimeSpan?))
                        {

                            if (TimeSpan.TryParse(newValue, out TimeSpan timeSpanValue))
                            {
                                convertedValue = timeSpanValue;
                            }
                            else
                            {
                                Console.WriteLine($"Invalid TimeSpan format for property '{update.Key}': {convertedValue}");
                                continue;
                            }
                        }
                        else
                        {
                            convertedValue = newValue;
                        }

                        property.SetValue(attendanceReport, convertedValue);
                    }
                    catch (Exception innerEx)
                    {
                        return BadRequest($"Error updating property '{update.Key}': {innerEx.Message}");
                    }
                }
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _unitOfWork.AttendanceReports.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new
            {
               id,
               updates
            });
        }


        // POST: api/AttendanceReports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AttendanceReport>> PostAttendanceReport(AttendanceReport attendanceReport)
        {
            attendanceReport.Date = attendanceReport.Date.Date; // Store only the date part
            await _unitOfWork.AttendanceReports.AddReportAsync(attendanceReport);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction("GetAttendanceReport", new { id = attendanceReport.Id }, attendanceReport);
        }

        // DELETE: api/AttendanceReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendanceReport(int id)
        {
            var attendanceReport = await _unitOfWork.AttendanceReports.GetReportByIdAsync(id);
            if (attendanceReport == null)
            {
                return NotFound();
            }

            await _unitOfWork.AttendanceReports.DeleteReportAsync(attendanceReport.Id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        //private bool AttendanceReportExists(int id)
        //{
        //    return await _unitOfWork.AttendanceReports.ExistsAsync(id);
        //}
    }
}
