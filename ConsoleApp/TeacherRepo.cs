using System;
using Npgsql;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class TeacherRepo
    {
        NpgsqlConnection connection;
        public TeacherRepo(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public static Teacher GetTeacher(NpgsqlDataReader reader)
        {
            Teacher teacher = new();
            teacher.id = reader.GetInt32(0);
            teacher.name = reader.GetString(1);
            teacher.surname = reader.GetString(2);
            return teacher;
        }
        public Teacher GetById(int id)
        {
            string command = $"SELECT * FROM teachers WHERE id = {id}";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Teacher teacher = new();

            if (reader.Read())
            {
                teacher.id = reader.GetInt32(0);
                teacher.name = reader.GetString(1);
                teacher.surname = reader.GetString(2);
            }
            reader.Close();
            return teacher;
        }
        public Teacher GetByFullName(string name, string surname)
        {
            string command = $"SELECT * FROM teachers WHERE name = '{name}' AND surname = '{surname}'";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            Teacher teacher = new();

            if (reader.Read())
            {
                teacher.id = reader.GetInt32(0);
                teacher.name = reader.GetString(1);
                teacher.surname = reader.GetString(2);
            }
            reader.Close();
            return teacher;
        }
        public bool Insert(Teacher teacher)
        {
            string command = $"INSERT INTO teachers (name, surname) VALUES ('{teacher.name}', '{teacher.surname}');";

            NpgsqlCommand cmd = new(command, connection);
            if (cmd.ExecuteScalar() == null) return false;
            return true;
        }
        public bool DeleteById(int id)
        {
            string command = $"DELETE FROM teachers WHERE id = {id};";

            NpgsqlCommand cmd = new(command, connection);

            SubjectRepo subjectRepo = new(connection);
            subjectRepo.DeleteByTeacherId(id);

            int res = cmd.ExecuteNonQuery();

            if (res == -1) return false;
            return true;
        }
        public List<Teacher> GetAll()
        {
            string command = "SELECT * FROM teachers";
            NpgsqlCommand cmd = new(command, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();

            List<Teacher> teachers = new();
            while (reader.Read())
            {
                Teacher teacher = GetTeacher(reader);
                teachers.Add(teacher);
            }
            reader.Close();
            return teachers;
        }
        public Teacher GetRandomTeacher()
        {
            Teacher t;
            while (true)
            {
                Random r = new();
                t = this.GetById(r.Next(GetFirstId() - 1, this.GetNewId()));
                if (t.name != null)
                {
                    break;
                }
                Console.WriteLine("T nope");
            }
            return t;
        }
        public int GetFirstId()
        {
            string command = $"SELECT * FROM teachers;";
            NpgsqlCommand cmd = new NpgsqlCommand(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();

            int id = -1;
            if (reader.Read())
            {
                id = reader.GetInt32(0);
            }
            reader.Close();
            return id;
        }
        public int GetNewId()
        {
            string command = $"SELECT * FROM teachers;";
            NpgsqlCommand cmd = new(command, connection);

            NpgsqlDataReader reader = cmd.ExecuteReader();

            int id = 1;
            while (reader.Read())
            {
                int currId = reader.GetInt32(0);
                if (currId > id) id = currId;
            }
            reader.Close();
            id++;
            return id;
        }
    }
}