﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMFightAcademy.Client.Contract;
using PMFightAcademy.Client.Contract.Dto;
using PMFightAcademy.Client.Models;
using PMFightAcademy.Client.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PMFightAcademy.Client.Controllers
{
    /// <summary>
    /// Service controller for sign up for a service/workout.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [SwaggerTag("This controller is for working with booking and getting active booking list or booking history.")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _service;

#pragma warning disable 1591
        public BookingController(BookingService service)
        {
            _service = service;
        }
#pragma warning restore 1591

        #region Old version for returning booking data
        ///// <summary>
        ///// Gets missing in <paramref name="booking"/> data for a booking depending on set filters in <paramref name="booking"/>.
        ///// </summary>
        ///// <param name="booking">Booking filters.</param>
        ///// <returns>
        ///// <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        ///// <see cref="HttpStatusCode.OK"/> if client is authorized and there is at least one element that corresponds set filters.
        ///// <see cref="HttpStatusCode.NotFound"/> if client is authorized and there is no element that corresponds set filters.
        ///// </returns>
        ///// <remarks>
        ///// Returns Unauthorized if client is unauthorized.
        ///// Returns OK if client is authorized and there is at least one element that corresponds set filters.
        ///// Returns NotFound if client is authorized and there is no element that corresponds set filters.
        ///// </remarks>
        //[HttpPost("getMissingData")]
        //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType(typeof(IEnumerable<DataForBookingDto>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        //public Task<IActionResult> GetMissingDataForBooking([FromBody] BookingDto booking)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        /// <summary>
        /// Get available services for client booking.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>
        /// Returns <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// Returns <see cref="HttpStatusCode.OK"/> with services list if client is authorized and there is at least one available service.
        /// Returns <see cref="HttpStatusCode.NotFound"/> with <c>string</c> message if client is authorized and there is no available service.
        /// </returns>
        /// <remarks>
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with services list if client is authorized and there is at least one available service.
        /// Returns NotFound with <c>string</c> message if client is authorized and there is no available service.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<Service>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetServicesForBooking(CancellationToken token)
        {
            var result = await _service.GetServicesForBooking();
            if (result != null) 
                return Ok(result);

            return NotFound();
        }

        /// <summary>
        /// Get available coaches which can provide service with id <paramref name="serviceId"/>.
        /// </summary>
        /// <param name="serviceId"></param> service id
        /// <param name="token"></param>
        /// <returns>
        /// Returns <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// Returns <see cref="HttpStatusCode.OK"/> with coaches list if client is authorized and there is at least one available coach.
        /// Returns <see cref="HttpStatusCode.NotFound"/> with <c>string</c> message if client is authorized and there is no available coach.
        /// </returns>
        /// <remarks>
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with coaches list if client is authorized and there is at least one available coach.
        /// Returns NotFound with <c>string</c> message if client is authorized and there is no available coach.
        /// </remarks>
        [HttpGet("{serviceId}")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<CoachDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCoachesForBooking([FromRoute] int serviceId,
            CancellationToken token)
        {
            var result = await _service.GetCoachesForBooking(serviceId);
            if (result != null)
                return Ok(result);

            return NotFound();
        }

        /// <summary>
        /// Get available dates to provide a service with id <paramref name="serviceId"/> by coach with id <paramref name="coachId"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// Returns <see cref="HttpStatusCode.OK"/> with dates list if client is authorized and there is at least one available date.
        /// Returns <see cref="HttpStatusCode.NotFound"/> with <c>string</c> message if client is authorized and there is no available date.
        /// </returns>
        /// <remarks>
        /// Dates will be returned in format "MM/dd/yyyy" as a <c>string</c>.
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with dates list if client is authorized and there is at least one available date.
        /// Returns NotFound with <c>string</c> message if client is authorized and there is no available date.
        /// </remarks>
        [HttpGet("{serviceId}/{coachId}")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDatesForBooking([FromRoute] int serviceId
            , [FromRoute] int coachId
            , CancellationToken token)
        {
            try
            {
                var result = await _service.GetDatesForBooking(serviceId, coachId);
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Get available time slots to provide a service with id <paramref name="serviceId"/> 
        /// by coach with id <paramref name="coachId"/> as of the <paramref name="date"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// Returns <see cref="HttpStatusCode.OK"/> with time slots list if client is authorized and there is at least one available time slot.
        /// Returns <see cref="HttpStatusCode.NotFound"/> with <c>string</c> message if client is authorized and there is no available time slot.
        /// </returns>
        /// <remarks>
        /// Date should be in format "MM/dd/yyyy" as a <c>string</c>.
        /// Time will be returned in format "HH:mm" as a <c>string</c>.
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with time slots list if client is authorized and there is at least one available time slot.
        /// Returns NotFound with <c>string</c> message if client is authorized and there is no available time slot.
        /// </remarks>
        [HttpGet("{serviceId}/{coachId}/{date}")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTimeSlotsForBooking([FromRoute] int serviceId,
            [FromRoute] int coachId,
            [FromRoute] string date,
            CancellationToken token)
        {
            try
            {
                var result = await _service.GetTimeSlotsForBooking(serviceId, coachId, date);
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Adds a booking.
        /// </summary>
        /// <param name="booking">Booking filters.</param>
        /// <param name="token"></param>
        /// <returns>
        /// <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// <see cref="HttpStatusCode.OK"/> with <c>string</c> message if client is authorized and a booking was successfully added.
        /// <see cref="HttpStatusCode.BadRequest"/> with <c>string</c> message if client is authorized and booking time is not available anymore
        ///  or booking model is invalid.
        /// </returns>
        /// <remarks>
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with <c>string</c> message if client is authorized and a booking was successfully added.
        /// Returns BadRequest with <c>string</c> message if client is authorized and booking time is not available anymore
        ///  or booking model is invalid.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddBooking([FromBody] BookingDto booking,
            CancellationToken token)
        {
            try
            {
                var claim = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(ClaimTypes.UserData);
                if (claim == null)
                    return BadRequest("No UserData in JWT-token for authorization");
                var clientId = int.Parse(claim.Value);
                await _service.AddBooking(booking, clientId);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (NullReferenceException)
            {
                return BadRequest("No Identity in JWT-token for authorization");
            }
        }

        /// <summary>
        /// Gets active bookings of services.
        /// </summary>
        /// <param name="pageSize">The count of active booking records to return at one time.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="token"></param>
        /// <returns>
        /// <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// <see cref="HttpStatusCode.OK"/> with active booking list if client is authorized and there is at least one record in the active booking list.
        /// <see cref="HttpStatusCode.NotFound"/> with <c>string</c> message if client is authorized and there is no record in the active booking list.
        /// </returns>
        /// <remarks>
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with active booking list if client is authorized and there is at least one record in the active booking list.
        /// Returns NotFound with <c>string</c> message if client is authorized and there is no record in the active booking list.
        /// </remarks>
        [HttpGet("{pageSize}/{page}")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(GetDataContract<HistoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public Task<IActionResult> GetActiveBookings([FromRoute] int pageSize,
            [FromRoute] int page,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets booking history.
        /// </summary>
        /// <param name="pageSize">The count of history records to return at one time.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="token"></param>
        /// <returns>
        /// <see cref="HttpStatusCode.Unauthorized"/> if client is unauthorized.
        /// <see cref="HttpStatusCode.OK"/> with booking history if client is authorized and there is at least one record in the history.
        /// <see cref="HttpStatusCode.NotFound"/> with <c>string</c> message if client is authorized and there is no record in the history.
        /// </returns>
        /// <remarks>
        /// Returns Unauthorized if client is unauthorized.
        /// Returns OK with booking history if client is authorized and there is at least one record in the history.
        /// Returns NotFound with <c>string</c> message if client is authorized and there is no record in the history.
        /// </remarks>
        [HttpGet("history/{pageSize}/{page}")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(GetDataContract<HistoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public Task<IActionResult> GetHistory([FromRoute] int pageSize,
            [FromRoute] int page,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
