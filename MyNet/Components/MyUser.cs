using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

// MyNetDb의 유저 테이블 읽어오기
namespace MyNet.Components
{
    /// <summary>
    /// 빈 클래스
    /// </summary>
    public class MyUser
    {
        //Empty
    }

    /// <summary>
    /// 모델 클래스 : MyNetDb userTable과 1:1 매핑 설계
    /// </summary>
    public class User
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public DateTime MDate { get; set; }

    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 인터페이스
    /// 기능 정의
    /// </summary>
    public interface IUserRepository
    {
        List<User> GetAll();
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 구현 클래스
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private IDbConnection _db;
        //생성자
        public UserRepository(IConfiguration config)
        {
            _db = new SqlConnection(
                config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public List<User> GetAll()
        {
            string sql = "SELECT [userID], [name], [level], [health], [gold], [mDate] " +
                         "FROM [dbo].[userTbl]";
            var users = _db.Query<User>(sql).ToList();
            return users;
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private IUserRepository _repository;

        public UserServiceController(IUserRepository repository)
        {
            _repository = repository;
        }


        // GET: api/<PointServiceController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var users = _repository.GetAll();
                return Ok(users);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}

/*
SELECT TOP (1000) [userID]
      ,[name]
      ,[level]
      ,[health]
      ,[gold]
      ,[mDate]
  FROM [MyNetDb].[dbo].[userTbl]
 */