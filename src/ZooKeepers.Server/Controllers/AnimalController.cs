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
                    SELECT id, name, imguri
                    FROM animal;
                ")).ToArray();

                return result;
            }
        }

        [HttpGet("of-user/{userId}")]
        public async Task<IEnumerable<AnimalModel>> GetByUser(string userId)
        {
            using (var db = new NpgsqlConnection(ConnectionString))
            {
                await db.OpenAsync();

                var result = (await db.QueryAsync<AnimalModel>(@"
                    SELECT a.id, a.name, a.imguri
                    FROM animal AS a
                        INNER JOIN user_animal AS ua
                            ON a.id = ua.animalid
                    WHERE ua.userid = @userId;
                ", new{ userId })).ToArray();

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
                    SELECT id, name, imguri
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
                    INSERT INTO animal (name, imguri)
                    VALUES (@Name, @ImgUri)
                    RETURNING id;",
                    model);

                return result;
            }
        }

        [HttpPut("{id}/users/{userId}")]
        public async Task PutUser(int id, string userId)
        {
            using (var db = new NpgsqlConnection(ConnectionString))
            {
                await db.OpenAsync();

                await db.ExecuteAsync(@"
                    INSERT INTO user_animal (animalid, userid)
                    VALUES (@id, @userId);",
                    new { id, userId });
            }
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        private string ConnectionString => "Server=db;Port=5432;Database=zoo;User ID=postgres;";
    }

    public class AnimalModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUri { get; set; }
    }

    public class AnimalCreateModel
    {
        public string Name { get; set; }
        public string ImgUri { get; set; }
    }
}
