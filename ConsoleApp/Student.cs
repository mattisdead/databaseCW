namespace ConsoleApp
{
    public class Student : Person
    {
        public Student() : base() { }
        public Student(int id, string name, string surname)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
        }
    }
}