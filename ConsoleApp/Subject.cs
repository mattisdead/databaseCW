namespace ConsoleApp
{
    public class Subject
    {
        public int id;
        public string name;
        public int teacherId;
        public Subject() : base() { }
        public Subject(int id, string name, int teacherId)
        {
            this.id = id;
            this.name = name;
            this.teacherId = teacherId;
        }
    }
}