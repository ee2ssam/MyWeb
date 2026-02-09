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
    public class MyUserLogin
    {
        //Empty
    }

    public interface IUserLoginRepository
    {
        UserLoginResult Login(UserLogin userLogin);
    }

    public class UserLoginRepository : IUserLoginRepository
    {
        private IDbConnection _db;
        //생성자
        public UserLoginRepository(IConfiguration config)
        {
            _db = new SqlConnection(
                config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public UserLoginResult Login(UserLogin userLogin)
        {
            string sql = "usp_UserLogin @UserId";
            var result = _db.Query<int>(sql, userLogin).Single();

            UserLoginResult loginResult = new UserLoginResult
            {
                Protocol = -userLogin.Protocol,
                Result = result,
                UserID = userLogin.UserID
            };
            return loginResult;
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginServiceController : ControllerBase
    {
        private IUserLoginRepository _repository;

        public UserLoginServiceController(IUserLoginRepository repository)
        {
            _repository = repository;
        }

        // POST api/<UserLoginServiceController>
        [HttpPost]
        public IActionResult Post([FromBody] UserLogin userLogin)
        {
            try
            {
                UserLoginResult loginResult = _repository.Login(userLogin);
                return Ok(loginResult);
            }
            catch (Exception)
            {
                UserLoginResult loginResult = new UserLoginResult
                {
                    Protocol = -userLogin.Protocol,
                    Result = 2,
                    UserID = userLogin.UserID
                };
                return Ok(loginResult);
            }
        }
    }

    /// <summary>
    /// 모델 클래스 - 유저 로그인 요청 (Protocol: 1101)
    /// </summary>
    public class UserLogin
    {
        public int Protocol { get; set; }
        [Required]
        [MaxLength(8)]
        public string UserID { get; set; }
    }

    /// <summary>
    /// 모델 클래스 - 유저 로그인 응답 (Protocol: -1101)
    /// </summary>
    public class UserLoginResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserID { get; set; }
    }
}
