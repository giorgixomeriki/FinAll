using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancialProfileManagerAPI.Models;
using FinancialProfileManagerAPI.Services;

namespace FinancialProfileManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly UserProfileService _service;

    public TransactionsController(UserProfileService service)
    {
        _service = service;
    }

    [HttpPost("{userId}/add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AddTransaction(Guid userId, [FromBody] AddTransactionRequest request)
    {
        try
        {
            var user = _service.GetUserById(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (request.Amount <= 0)
                return BadRequest(new { message = "Amount must be greater than 0" });

            var transaction = new UserTransaction
            {
                Amount = request.Amount,
                Description = request.Description,
                Type = request.Type
            };

            _service.AddTransaction(userId, transaction);
            return Ok(new { message = "Transaction added successfully", transaction });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(IEnumerable<UserTransaction>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUserTransactions(Guid userId)
    {
        try
        {
            var user = _service.GetUserById(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var transactions = _service.GetUserTransactions(userId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}

public class AddTransactionRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public TransactionType Type { get; set; }
}
