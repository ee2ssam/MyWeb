using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace MyNet.Components
{
    /// <summary>
    /// 빈 클래스
    /// </summary>
    public class MyUserAdd
    {
        //Empty
    }

    public interface IUserAddRepository
    {
        UserAddResult Add(UserAdd userAdd);
    }

    public class UserAddRepository : IUserAddRepository
    {
        private IDbConnection _db;
        //생성자
        public UserAddRepository(IConfiguration config)
        {
            _db = new SqlConnection(
                config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public UserAddResult Add(UserAdd userAdd)
        {
            string sql = "usp_AddUser @UserId, @Name";
            var result = _db.Query<int>(sql, userAdd).Single();

            UserAddResult addResult = new UserAddResult
            {
                Protocol = -userAdd.Protocol,
                Result = result,
                UserID = userAdd.UserID
            };
            return addResult;
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddServiceController : ControllerBase
    {
        private IUserAddRepository _repository;

        public UserAddServiceController(IUserAddRepository repository)
        {
            _repository = repository;
        }

        // POST api/<UserLoginServiceController>
        [HttpPost]
        public IActionResult Post([FromBody] UserAdd userAdd)
        {
            try
            {
                UserAddResult addResult = _repository.Add(userAdd);
                return Ok(addResult);
            }
            catch (Exception)
            {
                UserAddResult addResult = new UserAddResult
                {
                    Protocol = -userAdd.Protocol,
                    Result = 2,
                    UserID = userAdd.UserID
                };
                return Ok(addResult);
            }
        }
    }

    /// <summary>
    /// 모델 클래스 - 유저 추가 요청 (Protocol: 1102)
    /// </summary>
    public class UserAdd
    {
        public int Protocol { get; set; }
        [Required]
        [MaxLength(8)]
        public string UserID { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
    }

    /// <summary>
    /// 모델 클래스 - 유저 추가 응답 (Protocol: -11012)
    /// </summary>
    public class UserAddResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserID { get; set; }
    }
}
