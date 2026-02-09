using Microsoft.AspNetCore.Mvc;
using MyFive.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace MyFive.Components
{
    /// <summary>
    /// 빈 클래스
    /// </summary>
    public class FiveComponent
    {
    }

    /// <summary>
    /// 모델클래스 : FiveDB fiveTable과 1:1 매핑 설계
    /// </summary>
    public class Five
    {
        public int Id { get; set; }
        public string Note { get; set; }
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 인터페이스
    /// 기능 정의
    /// </summary>
    public interface IFiveRepository
    {
        List<Five> GetAll();
        Five GetById(int id);
        void Add(Five five);
        void Update(Five five);
        void Remove(int id);
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 구현 클래스
    /// </summary>
    public class FiveRepository : IFiveRepository
    {
        //private ApplicationDbContext _context;
        private IDbConnection _db;

        //생성자
        /*public FiveRepository(ApplicationDbContext context)
        {
            _context = context;
        }*/

        public FiveRepository(IConfiguration config)
        {
            _db = new SqlConnection(
                config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public void Add(Five five)
        {
            Five model = new Five() { Note = five.Note };
            string sql = "insert into Five (Note) values (@Note)";
            _db.Execute(sql, model);
        }

        public List<Five> GetAll()
        {
            // 예시로 하드코딩된 값을 반환
            /*return new List<Five>
            {
                new Five { Id = 1, Note = "First five note" },
                new Five { Id = 2, Note = "Second five note" }
            };*/
            //return _context.Five.ToList();

            // 데이터베이스에서 fiveTable의 모든 레코드 조회 로직 구현
            string sql = "select * from Five order by id desc";
            return _db.Query<Five>(sql).ToList();

        }

        public Five GetById(int id)
        {
            string sql = "select * from Five where Id = @Id";
            return _db.Query<Five>(sql, new { Id = id }).Single();
        }

        public void Remove(int id)
        {
            string sql = "delete from Five where Id = @Id";
            _db.Execute(sql, new { Id = id });
        }

        public void Update(Five five)
        {
            string sql = "update Five "
                + "set "
                + "Note = @Note "
                + "where Id = @Id";
            _db.Execute(sql, five);
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FiveServiceController : ControllerBase
    {
        private IFiveRepository _repository;

        //생성자
        public FiveServiceController(IFiveRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<PointServiceController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var fives = _repository.GetAll();
                return Ok(fives);
            }
            catch (Exception)
            {
                return BadRequest();
            }            
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
               var five = _repository.GetById(id);
                return Ok(five);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] Five five)
        {
            try
            {
                _repository.Add(five);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Five five)
        {
            try
            {
                var oldModel = _repository.GetById(id);
                if(oldModel == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다");
                }

                _repository.Update(five);
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var oldModel = _repository.GetById(id);
                if (oldModel == null)
                {
                    return NotFound($"{id}번 데이터가 없습니다");
                }

                _repository.Remove(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
