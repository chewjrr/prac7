using System;
using System.Diagnostics;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
class Program
{
    public static string ReplaceRows(string input)
    {
        string[] rows = input.Split(' ');

        bool spr = false;

        for (int i = 0; i < rows.Length - 1; i++)
        {
            if (rows[i] == "Б1" && rows[i + 1] == "Б1" && !spr)
            {
                rows[i] = "Б11";
            }
            else if (rows[i - 1] == "Б11" && rows[i] == "Б1" && !spr)
            {
                rows[i] = "Б11";
                spr = true;
            }
            if (rows[i] == "Б1" && spr)
            {
                rows[i] = "Б12";
            }
        }

        return string.Join(" ", rows);
    }
    static void reprt(string input)
    {
        input = ReplaceRows(input);

        string[] tokens = input.Split();

        string currentType = tokens[0];
        DateTime startDate = new DateTime(2023, 9, 1);
        DateTime endDate = startDate;
        int weeksCount = 0;

        foreach (string token in tokens)
        {
            if (token == currentType)
            {
                endDate = endDate.AddDays(7);
                weeksCount++;
            }
            else
            {
                PrintSchedule(currentType, startDate, endDate, weeksCount);

                currentType = token;
                startDate = endDate;
                endDate = startDate;
                weeksCount = 0;
            }
        }

        PrintSchedule(currentType, startDate, endDate, weeksCount);
    }
    public static string retype(string input)
    {
        if (input == "Б11") return "теоретическое обучение (1 семестр) ";
        if (input == "Б12") return "теоретическое обучение (2 семестр) ";
        if (input == "Б2") return "практика                           ";
        if (input == "Э") return "промежуточная аттестация           ";
        if (input == "К") return "каникулы                           ";
        if (input == "У") return "учебная практика                   ";
        if (input == "П") return "производственная практика          ";
        if (input == "НИР") return "научно-исследовательская работа    ";
        if (input == "Д") return "государственная итоговая аттестация";
        else return "Неопр.                             ";
    }
    static void PrintSchedule(string type, DateTime startDate, DateTime endDate, int weeksCount)
    {
        Console.WriteLine($"{retype(type)}    {startDate.ToShortDateString()} -> {endDate.AddDays(-1).ToShortDateString()} ({weeksCount + 1} недель)");
    }
    public static string ProcessString(string input)
    {
        return input
        .Replace('"', '*')
        .Replace("</p>", "\n")
        .Replace("<p>", "")
        .Replace("<div>", "")
        .Replace("</div>", "")
        .Replace("<br>", "")
        .Replace("<p class=*MsoNormal*>", "")
        .Replace("</br>", "");
    }
    public static T[] Merge<T>(T[] array1, T[] array2)
    {
        T[] mergedArray = new T[array1.Length + array2.Length];
        Array.Copy(array1, mergedArray, array1.Length);
        Array.Copy(array2, 0, mergedArray, array1.Length, array2.Length);
        return mergedArray;
    }
    public static JArray JMerge(JArray array1, JArray array2)
    {
        JArray combinedArray = new JArray();

        foreach (var item in array1)
        {
            combinedArray.Add(item);
        }

        foreach (var item in array2)
        {
            combinedArray.Add(item);
        }

        return combinedArray;
    }
    public static bool CheckFirstWord(string input)
    {
        string firstWord = input.Substring(0, input.IndexOf(' '));
        firstWord = firstWord.Replace(".", "");

        return int.TryParse(firstWord, out _);
    }
    public static string add_separator(int g = 1)
    {
        string S = "";
        for (int i = 0; i < g; i++)
        {
            S += "-";
        }
        return S;
    }



    static void DisplayProfessionalStandardsInfo()
    {
        string json = System.IO.File.ReadAllText("file.json");
        JObject opop = JObject.Parse(json);
        JArray professionalStandards = (JArray)opop["content"]["section4"]["professionalStandards"];

        int count = 0;

        Console.WriteLine("Номер | Код | Название");
        Console.WriteLine(add_separator(60));
        foreach (JToken standard in professionalStandards)
        {
            count += 1;
            string content = (string)standard["content"];
            Console.Write(count.ToString() + " | ");
            if (CheckFirstWord(content))
            {
                Console.Write(content.Split(" ")[0] + " | ");
                Console.Write(content.Substring(content.IndexOf(' ')));
            }
            else
            {
                Console.Write(content);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    static void DisplayCompetencyInfo(string comp)
    {
        string json = System.IO.File.ReadAllText("file.json");
        JObject opop = JObject.Parse(json);
        JArray arr1 = (JArray)opop["content"]["section4"]["universalCompetencyRows"];
        JArray arr2 = (JArray)opop["content"]["section4"]["commonCompetencyRows"];

        // Эта строчка для professionalCompetences. которые ПК-1, их нужно обработать.
        // JArray arr3 = (JArray)opop["content"]["section4"]["section4_2"];

        JArray arr = JMerge(arr1, arr2);

        int count = 0;

        bool ifa = false;

        Console.WriteLine(add_separator(60));
        foreach (JToken standard in arr)
        {
            count += 1;

            if (comp == "ALL" || (comp.Length >= 4 && comp.Substring(0, 4) == standard["competence"]["code"].ToString().Substring(0, 4)))
            {
                ifa = true;

                Console.WriteLine(standard["competence"]["code"]);
                Console.WriteLine();
                Console.WriteLine(standard["competence"]["title"]);
                Console.WriteLine();

                foreach (JToken trt in standard["indicators"])
                {
                    Console.WriteLine(trt["content"]);
                }

                Console.WriteLine();
                Console.WriteLine(add_separator(60));
            }
        }

        if (!ifa)
        {
            Console.WriteLine("Ничего не найдено");
            Console.WriteLine(add_separator(60));
        }
    }


    static void DisplayEducationalPlanDetails(string comp)
    {
        string json = System.IO.File.ReadAllText("file.json");
        JObject opop = JObject.Parse(json);
        JArray arr1 = (JArray)opop["content"]["section5"]["eduPlan"]["block2"]["subrows"];
        JArray arr2 = (JArray)opop["content"]["section5"]["eduPlan"]["gias"];
        JArray arr3 = (JArray)opop["content"]["section5"]["eduPlan"]["block1"]["subrows"];
        JArray arr4 = (JArray)opop["content"]["section5"]["eduPlan"]["block2Variety"]["subrows"];
        JArray arr5 = (JArray)opop["content"]["section5"]["eduPlan"]["block1Variety"]["subrows"];

        JArray all_arr = JMerge(arr1, JMerge(arr2, JMerge(arr3, JMerge(arr4, arr5))));

        int count = 0;

        bool ifa = false;

        Console.WriteLine(add_separator(60));
        foreach (JToken standard in all_arr)
        {
            count += 1;

            if (comp == standard["index"].ToString() || comp == "ALL")
            {

                ifa = true;

                Console.Write(standard["index"] + " ");
                Console.WriteLine(standard["title"]);
                Console.WriteLine();

                if (standard["description"] != null)
                {
                    Console.WriteLine(ProcessString(standard["description"].ToString()));
                    Console.WriteLine();
                }

                if (standard["competences"] != null)
                {
                    Console.Write("Компетенции - ");
                    foreach (JToken com in standard["competences"])
                    {
                        Console.Write(com["code"] + " ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Зачётные единицы - " + standard["unitsCost"]);
                Console.WriteLine();

                Console.WriteLine("Семестры - ");
                foreach (bool tr in standard["terms"])
                {
                    if (tr) Console.Write("O ");
                    else Console.Write("X ");
                }
                Console.WriteLine();

                Console.WriteLine(add_separator(60));
            }
        }

        if (!ifa)
        {
            Console.WriteLine("Ничего не найдено");
            Console.WriteLine(add_separator(60));
        }
    }

    static void FindAndPrintMatchingStandards(int find)
    {
        string json = System.IO.File.ReadAllText("file.json");
        JObject opop = JObject.Parse(json);
        JArray arr1 = (JArray)opop["content"]["section5"]["eduPlan"]["block2"]["subrows"];
        JArray arr2 = (JArray)opop["content"]["section5"]["eduPlan"]["gias"];
        JArray arr3 = (JArray)opop["content"]["section5"]["eduPlan"]["block1"]["subrows"];
        JArray arr4 = (JArray)opop["content"]["section5"]["eduPlan"]["block2Variety"]["subrows"];
        JArray arr5 = (JArray)opop["content"]["section5"]["eduPlan"]["block1Variety"]["subrows"];

        JArray all_arr = JMerge(arr1, JMerge(arr2, JMerge(arr3, JMerge(arr4, arr5))));

        int count;
        bool F;

        bool ifa = false;

        Console.WriteLine(add_separator(60));
        foreach (JToken standard in all_arr)
        {
            count = 0;
            F = false;

            foreach (bool tr in standard["terms"])
            {
                count += 1;
                if (count == find && tr) F = true;
            }

            if (F)
            {

                ifa = true;

                Console.Write(standard["index"] + " " + standard["title"]);

                Console.WriteLine();

                Console.WriteLine(add_separator(60));
            }
        }
        if (!ifa)
        {
            Console.WriteLine("Ничего не найдено");
            Console.WriteLine(add_separator(60));
        }
    }

    static void DisplayCalendarPlanTableCourses(int find)
    {
        string json = System.IO.File.ReadAllText("file.json");
        JObject opop = JObject.Parse(json);
        JArray arr = (JArray)opop["content"]["section5"]["calendarPlanTable"]["courses"];

        int count = 0;
        string inp;

        bool ifa = false;

        Console.WriteLine(add_separator(60));
        foreach (JToken standard in arr)
        {
            count += 1;

            if (find == count || find == -1)
            {
                ifa = true;
                Console.WriteLine("Курс - " + count.ToString());
                Console.WriteLine();
                inp = "";
                foreach (String weeks in standard["weekActivityIds"])
                {
                    inp += " " + weeks;
                }
                reprt(inp.Substring(1));
                Console.WriteLine();
                Console.WriteLine(add_separator(60));
            }
        }
        if (!ifa)
        {
            Console.WriteLine("Ничего не найдено");
            Console.WriteLine(add_separator(60));
        }
    }

    static void menu()
    {
        Console.WriteLine("1    - Вывод списка профессиональных стандартов");
        Console.WriteLine("2    - Поиск компитенций и индикаторов достижения");
        Console.WriteLine("3    - Поиск дисциплин по коду");
        Console.WriteLine("4    - Поиск дисциплин по семестрам");
        Console.WriteLine("5    - Поиск графика учёбного процесса по курсам");
        Console.WriteLine("help - Повторный вывод этого сообщения");
        Console.WriteLine("exit - Выход");
        Console.WriteLine(add_separator(60));
    }
    static void Main()
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string userInput;
        string Input1;
        int Input2;

        menu();
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Введите команду:");
            userInput = Console.ReadLine();
            switch (userInput)
            {
                case "1":
                    Console.WriteLine();
                    DisplayProfessionalStandardsInfo();
                    Console.WriteLine(add_separator(60));
                    break;
                case "2":
                    Console.WriteLine();
                    Input1 = GetUserInput("Введите код компитенции (или 'ALL' для вывода всех результатов):");
                    DisplayCompetencyInfo(Input1);
                    break;
                case "3":
                    Console.WriteLine();
                    Input1 = GetUserInput("Введите код дисциплины (или 'ALL' для вывода всех результатов):");
                    DisplayEducationalPlanDetails(Input1);
                    break;
                case "4":
                    Console.WriteLine();
                    Input2 = GetIntegerInput("Введите номер семестра (1-8):");
                    FindAndPrintMatchingStandards(Input2);
                    break;
                case "5":
                    Console.WriteLine();
                    Input2 = GetIntegerInput("Введите номер курса (1-4):");
                    DisplayCalendarPlanTableCourses(Input2);
                    break;
                case "help":
                    menu();
                    break;
                case "exit":
                    return;
                default:
                    Console.WriteLine();
                    Console.WriteLine("Неизвестная команда. Для помощи воспользуйтесь командой 'help'.");
                    Console.WriteLine(add_separator(60));
                    break;
            }
        }
    }
    static string GetUserInput(string text)
    {
        string userInput;
        while (true)
        {
            Console.WriteLine(text);
            userInput = Console.ReadLine();
            if (userInput.Length == 0)
            {
                Console.WriteLine("Пустой ввод. Повторите попытку.");
                Console.WriteLine(add_separator(60));
            }
            else
            {
                return userInput;
            }
        }
    }
    static int GetIntegerInput(string text)
    {
        int userInput;
        while (true)
        {
            Console.WriteLine(text);
            if (int.TryParse(Console.ReadLine(), out userInput))
            {
                return userInput;
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Повторите попытку.");
                Console.WriteLine(add_separator(60));
            }
        }
    }
}