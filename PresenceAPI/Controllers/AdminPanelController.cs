// Controllers/AdminPanel.cs
using Microsoft.AspNetCore.Mvc;
using domain.UseCase;
using data.RemoteData.RemoteDatabase.DAO;

namespace PresenceDesktop.API
{
    [ApiController]
    [Route("api")]
    public class AdminPanel : ControllerBase
    {
        private readonly GroupUseCase _groupUseCase;
        private readonly UserUseCase _userUseCase;
        private readonly PresenceUseCase _presenceUseCase;

        public AdminPanel(
            GroupUseCase groupUc,
            UserUseCase userUc,
            PresenceUseCase presenceUc)
        {
            _groupUseCase = groupUc;
            _userUseCase = userUc;
            _presenceUseCase = presenceUc;
        }

        // POST api/group
        [HttpPost("group")]
        public IActionResult AddGroup([FromBody] AddGroupRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.GroupName))
                return BadRequest("GroupName не может быть пустым.");

            _groupUseCase.AddGroup(req.GroupName);
            return Ok();
        }

        [HttpDelete("group/{id:int}")]
        public IActionResult DeleteGroup(int id)
        {
            _groupUseCase.DeleteGroup(id);
            return NoContent();
        }

        [HttpPost("group/{groupId:int}/students")]
        public IActionResult AddStudents(int groupId, [FromBody] StudentsRequest req)
        {
            // Для каждого ID меняем у пользователя поле GroupId
            foreach (var studentId in req.Students)
            {
                var user = _userUseCase.FindUserByUserId(studentId);
                _userUseCase.UpdateUserByUserId(studentId, user.FIO, groupId);
            }
            return Ok();
        }

        [HttpGet("presence")]
        public IActionResult GetPresence(
            [FromQuery] int groupId,
            [FromQuery] DateTime date,
            [FromQuery] int? studentId)
        {
            var d = DateOnly.FromDateTime(date);
            var list = _presenceUseCase.ShowPresenceForDateAndGroup(d, groupId);

            if (studentId.HasValue)
                list = list.Where(p => p.UserId == studentId.Value).ToList();

            return Ok(list);
        }

        // DELETE api/presence or api/presence?group=2
        [HttpDelete("presence")]
        public IActionResult DeletePresence([FromQuery] int? group)
        {
            if (group.HasValue)
                _presenceUseCase.DeletePresenceByGroup(group.Value);
            else
                _presenceUseCase.DeleteAllPresence();

            return NoContent();
        }

        // POST api/presence
        [HttpPost("presence")]
        public IActionResult CreatePresence([FromBody] List<CreatePresenceRequest> items)
        {
            // Мапим входящие DTO в DAO
            var presences = items.Select(i => new PresenceDAO
            {
                Date = DateOnly.FromDateTime(i.Date),
                LessonNumber = i.LessonNumber,
                UserId = i.StudentId,
                GroupId = i.GroupId,
                IsAttendance = i.TypeAttendance
            });
            _presenceUseCase.CreatePresences(presences);
            return Ok();
        }

        // PUT api/presence
        [HttpPut("presence")]
        public IActionResult UpdatePresence([FromBody] List<UpdatePresenceRequest> items)
        {
            foreach (var u in items)
            {
                var d = DateOnly.FromDateTime(u.Date);
                // Здесь мы просто перезаписываем флаг присутствия
                _presenceUseCase.UpdatePresence(d, u.StudentId, u.GroupId, u.LessonNumber, u.NewTypeAttendance);
            }
            return Ok();
        }
    }
}
