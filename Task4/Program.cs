using System;
using System.IO;
using System.Collections.Generic;

namespace Task4
{
    class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal AverageScore { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            string binFilePath = "students.dat";
            string outputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students");

            // Проверяем, существует ли директория, если нет - создаем
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            List<Student> students = ReadStudentsFromBinFile(binFilePath);

            // Группируем студентов по группам
            Dictionary<string, List<Student>> studentsByGroup = new Dictionary<string, List<Student>>();
            foreach (Student student in students)
            {
                if (!studentsByGroup.ContainsKey(student.Group))
                {
                    studentsByGroup.Add(student.Group, new List<Student>());
                }
                studentsByGroup[student.Group].Add(student);
            }

            // Записываем студентов в текстовые файлы по группам
            foreach (KeyValuePair<string, List<Student>> group in studentsByGroup)
            {
                string filePath = Path.Combine(outputDirectory, $"{group.Key}.txt");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (Student student in group.Value)
                    {
                        writer.WriteLine($"{student.Name}, {student.DateOfBirth:yyyy-MM-dd}, {student.AverageScore}");
                    }
                }
            }
        }


        static void WriteStudentsToBinFile(List<Student> students, string fileName)
        {
            using FileStream fs = new FileStream(fileName, FileMode.Create);
            using BinaryWriter bw = new BinaryWriter(fs);

            foreach (Student student in students)
            {
                bw.Write(student.Name);
                bw.Write(student.Group);
                bw.Write(student.DateOfBirth.ToBinary());
                bw.Write(student.AverageScore);
            }
            bw.Flush();
            bw.Close();
            fs.Close();
        }

        static List<Student> ReadStudentsFromBinFile(string fileName)
        {
            List<Student> result = new();
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            using BinaryReader br = new BinaryReader(fs);

            while (fs.Position < fs.Length)
            {
                Student student = new Student();
                student.Name = br.ReadString();
                student.Group = br.ReadString();
                long dt = br.ReadInt64();
                student.DateOfBirth = DateTime.FromBinary(dt);
                student.AverageScore = br.ReadDecimal();

                result.Add(student);
            }

            fs.Close();
            return result;
        }
    }
}