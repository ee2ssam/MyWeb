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
    public class MyLevelup
    {
        //Empty
    }

    public interface ILevelupRepository
    {
        LevelupResult UserLevelup(Levelup levelup);
    }

    public class LevelupRepository : ILevelupRepository
    {
        private IDbConnection _db;
        //생성자
        public LevelupRepository(IConfiguration config)
        {
            _db = new SqlConnection(
                config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public LevelupResult UserLevelup(Levelup levelup)
        {
            string sql = "usp_Levelup @UserId";
            var level = _db.Query<int>(sql, levelup).Single();

            if(level > 0)
            {
                LevelupResult levelupResult = new LevelupResult
                {
                    Protocol = -levelup.Protocol,
                    Result = 0,
                    UserID = levelup.UserID,
                    Level = level
                };
                return levelupResult;
            }
            else
            {
                LevelupResult levelupResult = new LevelupResult
                {
                    Protocol = -levelup.Protocol,
                    Result = 1,
                    UserID = levelup.UserID,
                    Level = 0
                };
                return levelupResult;
            }
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserLevelupServiceController : ControllerBase
    {
        private ILevelupRepository _repository;

        public UserLevelupServiceController(ILevelupRepository repository)
        {
            _repository = repository;
        }

        // POST api/<UserLoginServiceController>
        [HttpPost]
        public IActionResult Post([FromBody] Levelup levelup)
        {
            try
            {
                LevelupResult levelupResult = _repository.UserLevelup(levelup);
                return Ok(levelupResult);
            }
            catch (Exception)
            {
                LevelupResult levelupResult = new LevelupResult
                {
                    Protocol = -levelup.Protocol,
                    Result = 2,
                    UserID = levelup.UserID,
                    Level = 0
                };
                return Ok(levelupResult);
            }
        }
    }

    /// <summary>
    /// 모델 클래스 - 유저 로그인 요청 (Protocol: 1101)
    /// </summary>
    public class Levelup
    {
        public int Protocol { get; set; }
        [Required]
        [MaxLength(8)]
        public string UserID { get; set; }
    }

    /// <summary>
    /// 모델 클래스 - 유저 로그인 응답 (Protocol: -1101)
    /// </summary>
    public class LevelupResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserID { get; set; }
        public int Level { get; set; }
    }
}
