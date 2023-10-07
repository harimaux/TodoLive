namespace TodoLive.Models
{
    public class MainVM
    {
        public Todos? Todos { get; set; }
        public List<Todos>? TodosList { get; set; }
        public TaskPriority TaskPriority { get; set; }
        public List<TaskPriority> TaskPriorityList { get; set; }
        public bool GeneratePartialTask { get; set; }

        public MainVM()
        {
            Todos = new Todos();

            TodosList = new List<Todos>();

            TaskPriority = new TaskPriority();

            TaskPriorityList = new List<TaskPriority>();
        }
    }
}
