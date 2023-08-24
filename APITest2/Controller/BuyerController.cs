using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APITest2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace APITest2.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuyerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BuyerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult ExecuteQuery()
        {
            string connectionString = _configuration.GetConnectionString("OracleConnection");

            List<Buyer> results = new List<Buyer>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT *from Buyer";
                using (OracleCommand command = new OracleCommand(sql, connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Buyer buyer = new Buyer
                            {
                                Id = reader["Id"].ToString(),
                                Name = reader["Name"].ToString(),
                                PaymentMethod = reader["PaymentMethod"].ToString()
                            };
                            results.Add(buyer);
                        }
                    }
                }
            }
            return Ok(results);
        }

        [HttpPost]
        public IActionResult Post(Buyer b)
        {
            using (OracleConnection connection = new OracleConnection(_configuration.GetConnectionString("OracleConnection")))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("CreateBuyer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_Id", OracleDbType.Varchar2).Value = b.Id;
                    command.Parameters.Add("p_Name", OracleDbType.Varchar2).Value = b.Name;
                    command.Parameters.Add("p_PaymentMethod", OracleDbType.Varchar2).Value = b.PaymentMethod;

                    try
                    {
                        command.ExecuteNonQuery();
                        return Ok("Buyer created successfully.");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Failed to create buyer: " + ex.Message);
                    }
                }
            }
        }

        [HttpPut("{buyerId}")]
        public IActionResult UpdateBuyer(Buyer buyerInput , string buyerId)
        {
            using (OracleConnection connection = new OracleConnection(_configuration.GetConnectionString("OracleConnection")))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("updateBuyer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_Id", OracleDbType.Varchar2).Value = buyerId;
                    command.Parameters.Add("p_Name", OracleDbType.Varchar2).Value = buyerInput.Name;
                    command.Parameters.Add("p_PaymentMethod", OracleDbType.Varchar2).Value = buyerInput.PaymentMethod;

                    try
                    {
                        command.ExecuteNonQuery();
                        return Ok("Buyer updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Failed to update buyer: " + ex.Message);
                    }
                }
            }
        }
        [HttpDelete("{buyerId}")]
        public IActionResult DeleteBuyer(string buyerId)
        {
            using (OracleConnection connection = new OracleConnection(_configuration.GetConnectionString("OracleConnection")))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("DeleteBuyerById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_BuyerId", OracleDbType.Varchar2).Value = buyerId;

                    try
                    {
                        command.ExecuteNonQuery();
                        return Ok("Buyer deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Failed to delete buyer: " + ex.Message);
                    }
                }
            }
        }
    }
}