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
    public class MyUserInfo
    {
        //Empty
    }

    public interface IUserInfoRepository
    {
        UserInfoResult GetUserInfo(UserInfo userInfo);
    }

    public class UserInfoRepository : IUserInfoRepository
    {
        private IDbConnection _db;
        //생성자
        public UserInfoRepository(IConfiguration config)
        {
            _db = new SqlConnection(
                config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public UserInfoResult GetUserInfo(UserInfo userInfo)
        {
            string sql = "usp_UserInfo @UserId";
            var result = _db.Query<UserInfoResult>(sql, userInfo).Single();

            if(result.UserID != null)
            {
                UserInfoResult infoResult = new UserInfoResult
                {
                    Protocol = -userInfo.Protocol,
                    Result = 0,
                    UserID = result.UserID,
                    Name = result.Name,
                    Level = result.Level,
                    Health = result.Health,
                    Gold = result.Gold
                };
                return infoResult;
            }
            else
            {
                UserInfoResult infoResult = new UserInfoResult
                {
                    Protocol = -userInfo.Protocol,
                    Result = 1,
                    UserID = userInfo.UserID,
                    Name = "",
                    Level = 0,
                    Health = 0,
                    Gold = 0
                };
                return infoResult;
            }            
        }
    }

    /// <summary>
    /// Web API 컨트롤러
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoServiceController : ControllerBase
    {
        private IUserInfoRepository _repository;

        public UserInfoServiceController(IUserInfoRepository repository)
        {
            _repository = repository;
        }

        // POST api/<UserLoginServiceController>
        [HttpPost]
        public IActionResult Post([FromBody] UserInfo userInfo)
        {
            try
            {
                UserInfoResult infoResult = _repository.GetUserInfo(userInfo);
                return Ok(infoResult);
            }
            catch (Exception)
            {
                UserInfoResult infoResult = new UserInfoResult
                {
                    Protocol = -userInfo.Protocol,
                    Result = 2,
                    UserID = userInfo.UserID,
                    Name = "",
                    Level = 0,
                    Health = 0,
                    Gold = 0
                };
                return Ok(infoResult);
            }
        }
    }

    /// <summary>
    /// 모델 클래스 - 유저 정보 요청 (Protocol: 1103)
    /// </summary>
    public class UserInfo
    {
        public int Protocol { get; set; }
        [Required]
        [MaxLength(8)]
        public string UserID { get; set; }
    }

    /// <summary>
    /// 모델 클래스 - 유저 정보 응답 (Protocol: -1103)
    /// </summary>
    public class UserInfoResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
    }
}
