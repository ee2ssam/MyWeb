using Microsoft.AspNetCore.Mvc;

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
    }

    /// <summary>
    /// 데이터베이스 액세스 레이어 구현 클래스
    /// </summary>
    public class FiveRepository : IFiveRepository
    {
        public List<Five> GetAll()
        {
            // 데이터베이스에서 fiveTable의 모든 레코드 조회 로직 구현
            // 예시로 하드코딩된 값을 반환
            return new List<Five>
            {
                new Five { Id = 1, Note = "First five note" },
                new Five { Id = 2, Note = "Second five note" }
            };
        }
    }

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
            var json = new { Point = 5678 };
            //_repository.GetAll();
            return Ok(json);
        }

    }
}
