namespace TestTaskATON.Models.ResponseModels
{
    public class Response
    {
        public Response(string message, bool status)
        {
            Message = message;
            Status = status;
        }

        public Response(string message, bool status, User user)
        {
            Message = message;
            Status = status;
            User = user;
        }

        public Response(string message, bool status, List<User> users)
        {
            Message = message;
            Status = status;
            Users = users;
        }

        public string Message { get; set; } = null!;
        public User? User { get; set; }
        public List<User>? Users { get; set; }
        public bool Status { get; set; }
    }
}
