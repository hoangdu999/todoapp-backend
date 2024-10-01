using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using static Dapper.SqlMapper;
using System.Reflection;
using ToDoApp_BE.Models;

namespace ToDoApp_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {

        private MySqlConnection? _connection;
        public TaskController(MySqlConnection connection)
        {
            _connection = connection;
        }
        [NonAction]
        public void Open()
        {
            _connection = new MySqlConnection("Server=localhost;Port=3306;Database=todoapp;Uid=root;Pwd=12345678;");
            _connection.Open();
        }
        [NonAction]
        public void Close()
        {
            _connection.Close();
        }

        [HttpGet]
        public IEnumerable<MTask> Get()
        {
            try
            {
                var result = new List<MTask>();
                Open();
                string sql = "select * from task";
                result = _connection.Query<MTask>(sql).ToList();
                Close();
                return result;
            }
            catch (Exception)
            {
                return new List<MTask>();
            }
        }
        [HttpPost]
        public IActionResult Insert([FromBody] MTask task)
        {
            try
            {

                // Tên store produce
                string storedProducedureName = "Proc_Task_Insert";
                task.TaskId = Guid.NewGuid();
                // Chuẩn bị parameters cho stored produce
                var parameters = new DynamicParameters();
                foreach (PropertyInfo propertyInfo in task.GetType().GetProperties())
                {
                    // Add parameters
                    parameters.Add("p_" + propertyInfo.Name, propertyInfo.GetValue(task));
                }

                // Mở kết nối
                Open();

                // Xử lý thêm dữ liệu trong stored
                int res = _connection.Execute(storedProducedureName, param: parameters, commandType: CommandType.StoredProcedure);

                // Đóng kết nối
                Close();

                //Trả kết quả về
                return StatusCode(200, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPut]
        public IActionResult Update([FromBody] MTask task)
        {
            try
            {
                // Tên store produce
                string storedProducedureName = "Proc_Task_Update";

                // Chuẩn bị parameters cho stored produce
                var parameters = new DynamicParameters();
                foreach (PropertyInfo propertyInfo in task.GetType().GetProperties())
                {
                    // Add parameters
                    parameters.Add("p_" + propertyInfo.Name, propertyInfo.GetValue(task));
                }

                // Mở kết nối
                Open();

                int res = _connection.Execute(storedProducedureName, param: parameters, commandType: CommandType.StoredProcedure);

                // Đóng kết nối
                Close();

                //Trả kết quả về
                return StatusCode(200, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        //[HttpPut]
        //[Route("Update")]
        //public IActionResult Update(MTask task)
        //{
        //    try
        //    {
        //        Open();

        //        string sql = @"
        //    UPDATE task 
        //    SET 
        //        TaskName = @TaskName,
        //        Description = @Description,
        //        Deadline = @Deadline,
        //        Priority = @Priority,
        //        Status = @Status
        //    WHERE 
        //        TaskId = @TaskId";

        //        int result = _connection.Execute(sql, task);

        //        Close();

        //        return StatusCode(200, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            try
            {
                Open();
                string sql = "delete from task where TaskId = '" + id + "'";
                int result = _connection.Execute(sql);
                Close();
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPut]
        [Route("Update-Status")]
        public IActionResult UpdateStatus(Guid id)
        {
            try
            {
                Open();
                string sql = "UPDATE task SET Status = CASE WHEN Status = 'finish' THEN 'unfinish' ELSE 'finish' END WHERE TaskId = '" + id + "'";
                int result = _connection.Execute(sql);
                Close();
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPut]
        [Route("Update-Priority")]
        public IActionResult UpdatePriority(Guid id)
        {
            try
            {
                Open();
                string sql = "UPDATE task SET Priority = CASE WHEN Priority = 'unimportant' THEN 'important' ELSE 'unimportant' END WHERE TaskId = '" + id + "'";
                int result = _connection.Execute(sql);
                Close();
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}