using System;
using static System.Console;
using Npgsql;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class TeacherLogin
    {
        static Teacher user;
        static StudentRepo studentRepo;
        static TeacherRepo teacherRepo;
        static SubjectRepo subjectRepo;
        static GradeRepo gradeRepo;
        static NpgsqlConnection connection;
        public static void MainWindow(Teacher teacher)
        {
            user = teacher;
            connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=cwdb;User Id=postgres;Password=wake up call;");
            connection.Open();
            studentRepo = new StudentRepo(connection);
            teacherRepo = new TeacherRepo(connection);
            subjectRepo = new SubjectRepo(connection);
            gradeRepo = new GradeRepo(connection);

            WriteLine("To see list of commands type in \"Help\"\r\n");
            while (true)
            {
                WriteLine("Enter your command:");
                string command = ReadLine();
                if (command.Equals("Rate", StringComparison.InvariantCultureIgnoreCase)) // works
                {
                    string stName;
                    Student st = new Student();
                    while (true)
                    {
                        WriteLine("What is the full name of your student?");
                        stName = ReadLine();
                        if (stName == "exit") MainWindow(user);
                        string[] arr = stName.Split();

                        try
                        {
                            st = studentRepo.GetByFullName(arr[0], arr[1]);
                            if (st.id == 0) throw new Exception("No result");
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                            WriteLine("Please, check spelling one more time");
                            continue;
                        }
                        break;
                    }

                    WriteLine("What grade would you like to give them?");
                    int grade = 0;
                    while (true)
                    {
                        if (!int.TryParse(ReadLine(), out grade) || grade < 1 || grade > 100)
                        {
                            WriteLine("Invalid input");
                            continue;
                        }
                        break;
                    }

                    Subject sub = new Subject();
                    while (true)
                    {
                        WriteLine("What is the name of your subject?");
                        string subName = ReadLine();
                        if (subName == "exit") MainWindow(user);
                        string correctSubName = subName[0].ToString().ToUpper() + subName.Remove(0, 1);
                        sub = subjectRepo.GetByName(correctSubName);

                        if (sub.id == 0)
                        {
                            WriteLine("Nothing was found. Check spelling again");
                            continue;
                        }
                        break;
                    }

                    gradeRepo.Insert(new Grade(grade, st.id, sub.id));
                }
                else if (command.Equals("Subjects", StringComparison.InvariantCultureIgnoreCase)) // works
                {
                    List<Subject> subjects = subjectRepo.GetByTeacherId(user.id);
                    if (subjects.Count == 0)
                    {
                        WriteLine("Nothing was found.");
                        continue;
                    }
                    for (int i = 0; i < subjects.Count; i++)
                    {
                        WriteLine(subjects[i].name);
                    }
                }
                else if (command.Equals("Help", StringComparison.InvariantCultureIgnoreCase)) Help();
            }
        }
        static void Help()
        {
            WriteLine(@"Rate - rate your students
Subjects - see a list of subjects that you teach
Students - see all your students and what grades they have in your subject");
        }
    }
}