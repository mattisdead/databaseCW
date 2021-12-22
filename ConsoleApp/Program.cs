using System;
using Npgsql;
using static System.Console;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static StudentRepo studentRepo;
        static TeacherRepo teacherRepo;
        static SubjectRepo subjectRepo;
        static GradeRepo gradeRepo;
        static void Main(string[] args)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=cwdb;User Id=postgres;Password=wakeupcall;");
            conn.Open();
            studentRepo = new StudentRepo(conn);
            teacherRepo = new TeacherRepo(conn);
            subjectRepo = new SubjectRepo(conn);
            gradeRepo = new GradeRepo(conn);

            WriteLine("Enter your command:");
            string command = ReadLine();
            if(command.Equals("Predict", StringComparison.InvariantCultureIgnoreCase))
            {
                CreateCSVFromTable();
                WriteLine("What is the id of the student?");
                int studentId= 0; 
                if(!int.TryParse(ReadLine(), out studentId))
                {
                    WriteLine("Invalid id");
                    return;
                }
                if(studentRepo.GetById(studentId).name == null)
                {
                    WriteLine("Sorry, this student doesn't exists.");
                    return;
                }

                int subjectId = 0;
                WriteLine("What is the id of the subject?");
                if(!int.TryParse(ReadLine(),out subjectId))
                {
                    WriteLine("Invalid id");
                    return;
                }
                if (subjectRepo.GetById(subjectId).name == null)
                {
                    WriteLine("Sorry, this student doesn't exists.");
                    return;
                }

                float predictedGrade = Analysis.AnalysisProgram.Predict(new Analysis.Grade()
                { 
                    studentId = studentId,
                    subjectId = subjectId,
                    grade = 0
                });
                List<Grade> grades = gradeRepo.GetListOfGradesByStudentId(studentId);
                List<Grade> lastTenGrades = new();
                for (int i = grades.Count - 10; i < grades.Count; i++)
                {
                    try
                    {
                        lastTenGrades.Add(grades[i]);
                    }
                    catch (Exception ex)
                    {

                    }
                    
                }

                List<double> doubleGrades = new();

                for(int i = 0; i < lastTenGrades.Count; i++)
                {
                    doubleGrades.Add(grades[i].grade);
                }
                try
                {
                     Graph.CreateGradeGraph(doubleGrades.ToArray(), "..EntitiesData/actualGradesGraph.png");
                    doubleGrades.Add(predictedGrade);
                        Graph.CreateGradeGraph(doubleGrades.ToArray(), "..EntitiesData/predictedGradesGraph.png");
                }
                catch (Exception ex)
                {

                }

                return;
            }
            else if(command.Equals("Log in", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteLine("Are you a teacher (t) or a student (s)?");
                string role = ReadLine();
                Person user = null;
                if (role == "t") user = new Teacher();
                else if (role == "s") user = new Student();
                else
                {
                    WriteLine("Invalid input.");
                    return;
                }

                WriteLine("What's your name?");
                user.name = ReadLine();

                WriteLine("And what is your surname?");
                user.surname = ReadLine();

                if (user.GetType().Equals(typeof(Teacher)))
                {
                    Teacher t = teacherRepo.GetByFullName(user.name, user.surname);
                    if (t.id == 0)
                    {
                        teacherRepo.Insert(new Teacher(teacherRepo.GetNewId(), user.name, user.surname));
                        t = teacherRepo.GetByFullName(user.name, user.surname);
                    }
                    TeacherLogin.MainWindow(t);
                }
                else
                {
                    Student s = studentRepo.GetByFullName(user.name, user.surname);
                    if (s.id == 0)
                    {
                        studentRepo.Insert(new Student(studentRepo.GetNewId(), user.name, user.surname));
                        s = studentRepo.GetByFullName(user.name, user.surname);
                    }
                    StudentLogin.MainWindow(s);
                }
            }
            else if(command.Equals("Generate", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteLine("How many entities would you like to generate?");
                if(!int.TryParse(ReadLine(), out int number))
                {
                    WriteLine("Invalid input");
                    return;
                }
                DataGen(number);
            }
            else if (command.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteLine("What entities would you like to delete?");
                string entity = ReadLine();

                WriteLine("What is the id?");
                if(!int.TryParse(ReadLine(), out int id))
                {
                    WriteLine("Invalid input");
                    return;
                }

                
                    if (entity == "students")
                    {
                        studentRepo.DeleteById(id);
                    }
                    else if (entity == "teachers")
                    {
                        teacherRepo.DeleteById(id);
                    }
                    else if (command == "subjects")
                    {
                        subjectRepo.DeleteById(id);
                    }
                    else
                    {
                        WriteLine("Invalid input");
                        return;
                    }
               
            }
        }
        static void DataGen(int number)
        {
            WriteLine("What entities would you like to generate?");
            string command = ReadLine();
            
            if (command == "students")
            {
                for (int i = 0; i < number; i++)
                {
                    string allNames = File.ReadAllText("C:/Users/User/projects/databaseCW/EntitiesData/names.txt");
                    string allSurnames = File.ReadAllText("C:/Users/User/projects/databaseCW/EntitiesData/surnames.txt");

                    string[] arrNames = allNames.Split("\r\n");
                    string[] arrSurnames = allSurnames.Split("\r\n");

                    Random r = new Random();

                    string name = arrNames[r.Next(0, arrNames.Length)];
                    string surname = arrSurnames[r.Next(0, arrSurnames.Length)];

                    studentRepo.Insert(new Student(1, name, surname));
                }
            }
            else if (command == "teachers")
            {
                for (int i = 0; i < number; i++)
                {
                    string allNames = File.ReadAllText("C:/Users/User/projects/databaseCW//EntitiesData/names.txt");
                    string allSurnames = File.ReadAllText("C:/Users/User/projects/databaseCW/EntitiesData/surnames.txt");

                    string[] arrNames = allNames.Split("\r\n");
                    string[] arrSurnames = allSurnames.Split("\r\n");

                    Random r = new Random();

                    string name = arrNames[r.Next(0, arrNames.Length)];
                    string surname = arrSurnames[r.Next(0, arrSurnames.Length)];

                    teacherRepo.Insert(new Teacher(1, name, surname));
                }
            }
            else if (command == "grades")
            {
                for (int i = 0; i < number; i++)
                {
                    Random r = new();

                    int perc = r.Next(0, 101);
                    int mark;

                    if (perc < 10)
                    {
                        mark = r.Next(0, 60);
                    }
                    else if (perc < 90)
                    {
                        mark = r.Next(60, 90);
                    }
                    else
                    {
                        mark = r.Next(90, 101);
                    }

                    gradeRepo.Insert(new Grade(mark, studentRepo.GetRandomStudent().id, subjectRepo.GetRandomSubject().id));
                }
            }
            else if (command == "subjects")
            {
                for (int i = 0; i < number; i++)
                {
                    string allSubjects = File.ReadAllText("C:/Users/User/projects/databaseCW/EntitiesData/subjects.txt");
                    string[] arrSubjects = allSubjects.Split("\r\n");

                    Random r = new();

                    string subject = arrSubjects[r.Next(0, arrSubjects.Length)];

                    Teacher t = teacherRepo.GetRandomTeacher();

                    subjectRepo.Insert(new Subject(0, subject, t.id));
                }
            }
            else
            {
                WriteLine("Invalid input");
                return;
            }
            WriteLine("Done!");
        }
        static void CreateCSVFromTable()
        {
            List<Grade> grades = gradeRepo.GetAll();
            string csvtext = "";
            for (int i=0; i < grades.Count; i++)
            {
                csvtext += grades[i].studentId.ToString() + ',' + grades[i].subjectId.ToString() + ',' + grades[i].grade.ToString() + "\r\n";
            }
            File.WriteAllText("C:/Users/User/projects/databaseCW/Analysis/TrainingData/GradesTableTrain.csv", csvtext);
        }
    }
}
