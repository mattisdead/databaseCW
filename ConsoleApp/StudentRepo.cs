using System;
using Npgsql;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class StudentRepo
    {
        NpgsqlConnection connection;
        public StudentRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static Student GetStudent(NpgsqlDataReader reader)
        {
            Student st = new Student();
            st.id = reader.GetInt32(2);
            st.name = reader.GetString(0);
            st.surname = reader.GetString(1);
            return st;
        }
        public Student GetById(int id)
        {
            string command = $"SELECT * FROM students WHERE id = {id}";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Student st = new Student();

            if (reader.Read())
            {
                st.id = reader.GetInt32(2);
                st.name = reader.GetString(0);
                st.surname = reader.GetString(1);
            }
            reader.Close();
            return st;
        }
        public Student GetByFullName(string name, string surname)
        {
            string command = $"SELECT * FROM students WHERE name = '{name}' AND surname = '{surname}'";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Student st = new Student();

            if (reader.Read())
            {
                st.id = reader.GetInt32(2);
                st.name = reader.GetString(0);
                st.surname = reader.GetString(1);
            }
            reader.Close();
            return st;
        }
        public bool Insert(Student st)
        {
            string command = $"INSERT INTO students (name, surname) VALUES ('{st.name}', '{st.surname}');";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            if (cmd.ExecuteScalar() == null) return false;
            return true;
        }
        public bool DeleteById(int id)
        {
            string command = $"DELETE FROM students WHERE id = {id};";

            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            int res = cmd.ExecuteNonQuery();

            GradeRepo gradeRepo = new GradeRepo(connection);
            gradeRepo.DeleteByStudentId(id);

            if (res == -1) return false;
            return true;
        }
        public List<Student> GetAll()
        {
            string command = "SELECT * FROM students";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Student> students = new List<Student>();
            while (reader.Read())
            {
                Student st = GetStudent(reader);
                students.Add(st);
            }
            reader.Close();
            return students;
        }
        public Student GetRandomStudent()
        {
            Random r = new Random();
            string command = $"SELECT * FROM students WHERE id = {r.Next(1, this.GetNewId())}";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Student st = new Student();

            if (reader.Read())
            {
                st.id = reader.GetInt32(2);
                st.name = reader.GetString(0);
                st.surname = reader.GetString(1);
            }
            reader.Close();
            return st;
        }
        public int GetNewId()
        {
            string command = $"SELECT * FROM students;";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();

            int id = 1;
            while (reader.Read())
            {
                int currId = reader.GetInt32(2);
                if (currId > id) id = currId;
            }
            id = id + 1;
            reader.Close();
            return id;
        }
    }
}