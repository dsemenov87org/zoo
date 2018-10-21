using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace ZooKeepers.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AnimalController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IEnumerable<AnimalModel>> Get()
        {
            using (var db = new NpgsqlConnection(ConnectionString))
            {
                await db.OpenAsync();

                var result = (await db.QueryAsync<AnimalModel>(@"
                    SELECT id, userid, imguri
                    FROM animal;
                ")).ToArray();

                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalModel>> Get(int id)
        {
            using (var db = new NpgsqlConnection(ConnectionString))
            {
                await db.OpenAsync();

                var result = (await db.QueryAsync<AnimalModel>(@"
                    SELECT id, userid, imguri
                    FROM animal
                    WHERE id = @id;
                ", new { id })
                ).FirstOrDefault();

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        public async Task<int> Post([FromBody] AnimalCreateModel model)
        {
            using (var db = new NpgsqlConnection(ConnectionString))
            {
                await db.OpenAsync();

                var result = await db.QuerySingleAsync<int>(@"
                    INSERT INTO animal (userid, imguri)
                    VALUES (@UserId, @ImgUri)
                    RETURNING id;",
                    model);

                return result;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string ConnectionString =>
            _config.GetValue<string>(
                "ConnectionString",
                "server=localhost;userid=postgres;port=5432;password=1;database=zoo;");
    }

    public class AnimalModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ImgUri { get; set; }
    }

    public class AnimalCreateModel
    {
        public string UserId { get; set; }
        public string ImgUri { get; set; }
    }
}
