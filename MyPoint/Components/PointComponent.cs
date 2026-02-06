using Microsoft.AspNetCore.Mvc;

namespace MyPoint.Components
{
    /// <summary>
    /// 빈 클래스
    /// </summary>
    public class PointComponent
    {
        //Empty class
    }

    /// <summary>
    /// 모델 클래스 : PointDB pointTable과 1:1 매핑 설계
    /// </summary>
    public class Point
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TotalPoints { get; set; }
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 인터페이스
    /// 기능 정의
    /// </summary>
    public interface IPointRepository
    {
        int GetTotalPointsByUserId(int userId);
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 구현 클래스
    /// </summary>
    public class PointRepository : IPointRepository
    {
        public int GetTotalPointsByUserId(int userId = 1234)
        {
            // 데이터베이스에서 포인트 합계 조회 로직 구현
            // 예시로 하드코딩된 값을 반환
            return userId; // 예시 값
        }   
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PointServiceController : ControllerBase
    {
        private IPointRepository _repository;

        //생성자
        public PointServiceController(IPointRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<PointServiceController>
        [HttpGet]
        public IActionResult Get()
        {
            var json = new { Point = 5678 };
            return Ok(json);
        }

        // GET api/<PointServiceController>/5
        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            //return $"넘어온 값: {id}";
            var myPoint = _repository.GetTotalPointsByUserId(userId);
            var json = new { Point = myPoint };
            return Ok(json);
        }

    }

}
