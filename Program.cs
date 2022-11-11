using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace CPP_Lab_1._4
{
    [Serializable]
    public class Prisoner
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Middle_Name { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Dates_of_Convictions { get; set; }
        public DateTime Date_of_Last_Imprisonment { get; set; }
        public DateTime? Date_of_Last_Dismissal { get; set; }
        
    }

    //[Serializable]
    //public class Police_file<T> 
    //{
    //    public T priosoner;
    //   
    //    public void fillList(T p)
    //    {
    //        pf.Add(p);
    //    }
    //    public Police_file(T prosoner)
    //    {
    //        this.priosoner = prosoner;
    //    }
    //    public T getPrisoner() { return priosoner; }
    //    public void setPrisoner(T prisoner) { priosoner = prisoner; }
    //}

    public class Utilities<T>
    {
        public static void Read_File(string filePfth, ref List<Prisoner> pf1)
        {
            try
            {
                List<string> lines = File.ReadAllLines(filePfth).ToList();

                foreach (string line in lines)
                {
                    Prisoner prisoner = new Prisoner();
                    string[] entries = line.Split(',');

                    prisoner.ID = Convert.ToInt32(entries[0]);
                    prisoner.Surname = entries[1];

                    prisoner.Name = entries[2];
                    prisoner.Middle_Name = entries[3];
                    prisoner.Birthday = Convert.ToDateTime(entries[4]);

                    string[] date = new string[entries.Length - 5];
                    int i = 5, j = 0;

                    while (i < entries.Length)
                    {
                        date[j] = entries[i];
                        i++;
                        j++;

                    }
                    prisoner.Dates_of_Convictions = date;
                    prisoner.Date_of_Last_Imprisonment = Convert.ToDateTime(date[j - 2]);
                    if (date[j - 1] != "...") prisoner.Date_of_Last_Dismissal = Convert.ToDateTime(date[j - 1]);

                    pf1.Add(prisoner);
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

          
        }


        static int tableWidth = 188;

        public static void PrintTable(List<Prisoner> police_Files)
        {

            PrintLine();
            PrintRow("ID", "П.І.П", "Дата народження ", "Дати ув'язнень", "Дата ост ув'язнення", "Дата ост звільнення");
            PrintLine();
            foreach (Prisoner p in police_Files)
            {
                //Prisoner p = pf.getPrisoner();
                if (p.Dates_of_Convictions.Length == 2)
                {
                    string[] str = {p.ID.ToString(),p.Surname+" "+p.Name+" "+ p.Middle_Name,String.Format("{0:dd/MM/yyyy}", p.Birthday),
                    p.Dates_of_Convictions[0]+"-"+ p.Dates_of_Convictions[1],String.Format("{0:dd/MM/yyyy}",
                    p.Date_of_Last_Imprisonment),String.Format("{0:dd/MM/yyyy}", p.Date_of_Last_Dismissal)};

                    PrintRow(str);
                }
                else
                {
                    string[] str = {p.ID.ToString(),p.Surname+" "+p.Name+" "+ p.Middle_Name,String.Format("{0:dd/MM/yyyy}", p.Birthday),
                    p.Dates_of_Convictions[0]+"-"+ p.Dates_of_Convictions[1],String.Format("{0:dd/MM/yyyy}",
                    p.Date_of_Last_Imprisonment),String.Format("{0:dd/MM/yyyy}", p.Date_of_Last_Dismissal)};
                    PrintRow(str);

                    for (int i = 2; i < p.Dates_of_Convictions.Length - 1; i++)
                    {
                        PrintRow("", "", "", p.Dates_of_Convictions[i] + "-" + p.Dates_of_Convictions[i + 1], "", "");
                    }

                }
                PrintLine();
            }

        }


        public static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth - 2));
        }

        public static void PrintRow(params string[] columns)
        {

            int width = (tableWidth - 10) / 5;
            string row = "|";

            for (int i = 0; i < columns.Length; i++)
            {
                if (i != 0)
                {
                    row += AlignCentre(columns[i], width) + "|";
                }
                else
                {
                    row += AlignCentre(columns[i], 5) + "|";
                }

            }

            Console.WriteLine(row);
        }

        public static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public static void Serialize(XmlSerializer formatter, List<Prisoner> pf1)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Зберегти";
            saveFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFile.FilterIndex = 2;
            saveFile.RestoreDirectory = true;


            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(Path.GetFullPath(saveFile.FileName), FileMode.OpenOrCreate))
                {

                    formatter.Serialize(fs, pf1);
                    Console.WriteLine("Серіалізували об'єкт");
                }
            }

        }

        public static List<T> Deserialize(XmlSerializer formatter)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Відкрити";
            openFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(Path.GetFullPath(openFile.FileName), FileMode.OpenOrCreate))
                {
                    List<T> pf2 = (List<T>)formatter.Deserialize(fs);
                    Console.WriteLine("Десеріалізували об'єкт");
                    return pf2;
                }

            }
            return null;

        }

        public static void Add(ref List<Prisoner> pf1)
        {

            bool errors = false;
            Regex regex = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", RegexOptions.IgnorePatternWhitespace);
            List<string> data = new List<string>();


            Prisoner newPrisoner = new Prisoner();
            newPrisoner.ID = pf1[pf1.Count() - 1].ID + 1;
            Console.WriteLine("  ID :" + newPrisoner.ID.ToString());


            Console.WriteLine("  Введіть:");
            Console.Write("  Прізвище: "); data.Add(Console.ReadLine());
            Console.Write("  Ім'я:"); data.Add(Console.ReadLine());
            Console.Write("  По батькові:"); data.Add(Console.ReadLine());

            do
            {
                Console.Write("  День народження(дд.мм.рррр): "); data.Add(Console.ReadLine());
                Match x = regex.Match(data[3]);
                if (x.Success == true)
                {
                    if (Convert.ToDateTime(data[3]) < DateTime.Now)
                    {
                        errors = true;
                    }
                }
                else
                {
                    data.RemoveAt(3);
                    Console.WriteLine("Дата введена на правильно");
                }

            } while (errors == false);

            errors = false;
            do
            {
                Console.Write("  Дата ув'язнення (дд.мм.рррр):"); data.Add(Console.ReadLine());
                Match x = regex.Match(data[4]);
                if (x.Success == true)
                {
                    if (Convert.ToDateTime(data[4]) < DateTime.Now)
                    {
                        errors = true;
                    }

                }
                else
                {
                    data.RemoveAt(4);
                    Console.WriteLine("Дата введена на правильно");
                }

            } while (errors == false);

            newPrisoner.Surname = data[0];
            newPrisoner.Name = data[1];
            newPrisoner.Middle_Name = data[2];
            newPrisoner.Birthday = Convert.ToDateTime(data[3]);
            newPrisoner.Date_of_Last_Imprisonment = Convert.ToDateTime(data[4]);
            string[] date = { data[4], "..." };
            newPrisoner.Dates_of_Convictions = date;

            pf1.Add(newPrisoner);
         
        }

        public static void Edit(ref List<Prisoner> pf1)
        {

            long id;
            Prisoner prisoner = new Prisoner();
            do
            {
                Console.Write("  Введіть ID: "); id = Convert.ToInt32(Console.ReadLine());
                foreach(Prisoner p in pf1)
                {
                    if(p.ID == id)
                    {
                        prisoner = p;
                        pf1.Remove(p);
                        break;
                    }     
                }
               
                if (prisoner == null) Console.WriteLine(" Неправильне ID!");

            } while (prisoner == null);

            string date;
            bool errors = false;
            Regex regex = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", RegexOptions.IgnorePatternWhitespace);


            Console.WriteLine("  ID :" + prisoner.ID.ToString());
            Console.WriteLine("  Прізвище: " + prisoner.Surname);
            Console.WriteLine("  Ім'я: " + prisoner.Name);
            Console.WriteLine("  По батькові: " + prisoner.Middle_Name);
            Console.WriteLine("  День народження: " + String.Format("{0:dd/MM/yyyy}", prisoner.Birthday));


            if (prisoner.Date_of_Last_Dismissal == null)
            {
                Console.WriteLine("  Дата ув'язнення: " + String.Format("{0:dd/MM/yyyy}", prisoner.Date_of_Last_Imprisonment));
                do
                {
                    Console.Write("  Дата звільнення(дд.мм.рррр):"); date = Console.ReadLine();
                    Match x = regex.Match(date);
                    if (x.Success == true)
                    {
                        if (Convert.ToDateTime(date) <= DateTime.Now && Convert.ToDateTime(date) > prisoner.Date_of_Last_Imprisonment)
                        {
                            errors = true;
                        }
                    }
                    else
                    {
                        date = null;
                        Console.WriteLine("Дата введена на правильно");
                    }

                } while (errors == false);

                prisoner.Date_of_Last_Dismissal = Convert.ToDateTime(date);
                string[] date_arr = new string[prisoner.Dates_of_Convictions.Length];
                int i = 0;
                while (i < date_arr.Length)
                {
                    date_arr[i] = prisoner.Dates_of_Convictions[i];
                    i++;
                }
                date_arr[i - 1] = date;
                prisoner.Dates_of_Convictions = date_arr;

            }
            else
            {
                Console.WriteLine("  Дата ост ув'язнення: " + String.Format("{0:dd/MM/yyyy}", prisoner.Date_of_Last_Imprisonment));
                Console.WriteLine("  Дата ост звільнення: " + String.Format("{0:dd/MM/yyyy}", prisoner.Date_of_Last_Dismissal));
                do
                {
                    Console.Write("  Дата ув'язнення (дд.мм.рррр):"); date = Console.ReadLine();
                    Match x = regex.Match(date);
                    if (x.Success == true)
                    {
                        if (Convert.ToDateTime(date) <= DateTime.Now && Convert.ToDateTime(date) > prisoner.Date_of_Last_Dismissal)
                        {
                            errors = true;
                        }
                    }
                    else
                    {
                        date = null;
                        Console.WriteLine("Дата введена на правильно");
                    }

                } while (errors == false);

                prisoner.Date_of_Last_Imprisonment = Convert.ToDateTime(date);
                string[] date_arr = new string[prisoner.Dates_of_Convictions.Length + 2];
                int i = 0;
                while (i < date_arr.Length - 2)
                {
                    date_arr[i] = prisoner.Dates_of_Convictions[i];
                    i++;
                }
                date_arr[i] = date;
                date_arr[i + 1] = "...";
                prisoner.Dates_of_Convictions = date_arr;
                prisoner.Date_of_Last_Dismissal = null;

            }

            pf1.Add(prisoner);

        }
        
        public static void Delete(ref List<Prisoner> pf1)
        {
            bool delete = false;
            do
            {
                Console.Write("  Введіть ID: "); int id = Convert.ToInt32(Console.ReadLine());
               
                foreach(Prisoner p in pf1)
                {
                    if(p.ID == id)
                    {
                        pf1.Remove(p);
                        delete = true;
                        break;
                    }
                }

                if (delete == false) Console.WriteLine(" Неправильне ID!");

            } while (delete == false);

        }

        public static List<Prisoner> Sort_by(List<Prisoner> pf1, int sort_by)
        {
           
            switch (sort_by)
            {
                case 1: return pf1.OrderBy(order => order.Surname).ToList();
                case 2: return pf1.OrderBy(order => order.Birthday).ToList();
                case 3: return pf1.OrderBy(order => order.Date_of_Last_Imprisonment).ToList();
                case 4: return pf1.OrderBy(order => order.Date_of_Last_Dismissal).ToList();
                default:
                    {
                        Console.WriteLine("\nНевідома дія!");
                        return pf1;
                    }
            }
           
        }

    }


    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            Console.SetWindowSize(193, 50);

            List<Prisoner> police_Files = new List<Prisoner>();
            List<Prisoner> sorted_police_Files = new List<Prisoner>();

            //string filepfth = "..//..//police_file.txt";
            //Utilities.Read_File(filepfth, ref police_Files);
            //Utilities.PrintTable(police_Files);



            bool exit = false;
            do
            {
                int action = -1;
                Console.WriteLine(" Дії:");
                Console.WriteLine("  [1] - Десеріалізувати");
                Console.WriteLine("  [2] - Серіалізувати");
                Console.WriteLine("  [3] - Додати");
                Console.WriteLine("  [4] - Редагувати");
                Console.WriteLine("  [5] - Видалити");
                Console.WriteLine("  [6] - Сортувати");
                Console.WriteLine("  [0] - Завершити роботу");
                Console.Write("  Виберіть дію: "); action = Convert.ToInt32(Console.ReadLine());
                switch (action)
                {
                    case 1:
                        {
                            Console.Clear();
                            XmlSerializer formatter = new XmlSerializer(typeof(List<Prisoner>));
                            police_Files = Utilities<Prisoner>.Deserialize(formatter);
                            if (police_Files != null) Utilities<Prisoner>.PrintTable(police_Files);
                            break;
                        }
                    case 2:
                        {
                            Console.Clear();
                            XmlSerializer formatter = new XmlSerializer(typeof(List<Prisoner>));
                            Utilities<Prisoner>.Serialize(formatter, police_Files);
                            break;
                        }
                    case 3:
                        {
                            Console.Clear();
                            Utilities<Prisoner>.Add(ref police_Files);
                            Utilities<Prisoner>.PrintTable(police_Files);
                            break;
                        }
                    case 4:
                        {
                            Console.Clear();
                            Utilities<Prisoner>.Edit(ref police_Files);
                            Utilities<Prisoner>.PrintTable(police_Files);
                            break;
                        }
                    case 5:
                        {
                            Console.Clear();
                            Utilities<Prisoner>.Delete(ref police_Files);
                            Utilities<Prisoner>.PrintTable(police_Files);
                            break;
                        }
                    case 6:
                        {
                            Console.WriteLine(" Сортувати:");
                            Console.WriteLine("  [1] - за Прізвищем");
                            Console.WriteLine("  [2] - за Датаою народження");
                            Console.WriteLine("  [3] - за Дата ост. ув'язнення");
                            Console.WriteLine("  [4] - за Дата ост. звільнення");
                            Console.Write("  Виберіть дію: "); int sort_by = Convert.ToInt32(Console.ReadLine());
                            switch (sort_by)
                            {
                                case 1:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Прізвищем");
                                        sorted_police_Files = Utilities<Prisoner>.Sort_by(police_Files, 1);
                                        Utilities<Prisoner>.PrintTable(sorted_police_Files);
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Датаою народження");
                                        sorted_police_Files = Utilities<Prisoner>.Sort_by(police_Files, 2);
                                        Utilities<Prisoner>.PrintTable(sorted_police_Files);
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Дата ост ув'язнення");
                                        sorted_police_Files = Utilities<Prisoner>.Sort_by(police_Files, 3);
                                        Utilities<Prisoner>.PrintTable(sorted_police_Files);
                                        break;
                                    }
                                case 4:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Дата ост звільнення");
                                        sorted_police_Files = Utilities<Prisoner>.Sort_by(police_Files, 4);
                                        Utilities<Prisoner>.PrintTable(sorted_police_Files);
                                        break;
                                    }
                                default:
                                    {
                                        
                                        Console.WriteLine("\nНевідома дія!");
                                        break;
                                    }

                            }

                            break;
                        }

                    case 0:
                        {
                            exit = true;
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            Console.WriteLine("\nНевідома дія!");
                            break;
                        }
                }

            } while (exit == false);

        }
    }
}
