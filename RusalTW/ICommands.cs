
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RusTW
{
	interface ICommands
	{
		static DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Анкеты");
		static FileInfo[] files = dir.GetFiles().ToArray();

		static List<string> progLang = new List<string> { "PHP", "JavaScript", "C","C++", "Java", "C#","Python","Ruby" };
		static List<int> progCount = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0};
		static List<string> Questions = new List<string>() { "ФИО:", "Дата рождения", "Любимый язык программирования", "Опыт программирования на указанном языке",
		"Мобильный телефон"};

		public delegate string myDeleg(string temp);
		
		static Dictionary<int, myDeleg> dictionary = new Dictionary<int, myDeleg>()
		{
			{0, new myDeleg(addFIO) },
			{1, new myDeleg(addYear) },
			{2, new myDeleg(addProgLang) },
			{3, new myDeleg(addExperience) },
			{4, new myDeleg(addMobilePhone) }
		};

		private static string addFIO(string temp)
		{
			return "ФИО: " + temp;
		}
		private static string addYear(string temp)
		{
			return "Дата рождения: " + temp;
		}
		private static string addProgLang(string temp)
		{
		M: if (progLang.Any(p => p == temp) == true)
				return "Любимый язык программирования: " + temp;
			else
			{
				Console.WriteLine("Введен ЯП не из списка, повторите попытку");
				temp = Console.ReadLine();
				goto M;
			}
		}
		private static string addExperience(string temp)
		{
			return "Опыт программирования на указанном языке: " + temp;
		}
		private static string addMobilePhone(string temp)
		{
			return "Мобильный телефон: " + temp;
		}

		private static string addYearNow()
		{
			return "Дата заполнения: " + DateTime.Now.ToShortDateString();
		}

		public static string[] NewProfile(string[] newProfile)
		{
			string tempPerem;
			int i = 0;
			Console.WriteLine("Заполнить новую анкету.\nВ анкете 5 вопросов:");
			while (i < dictionary.Count)
				{
					Console.WriteLine(Questions[i]);
					tempPerem = Console.ReadLine();
					string[] tempMass = tempPerem.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					if (tempMass[0] == "-goto_prev_question")
					{
						i = i - 1;
						continue;
					}
					if (tempMass[0] == "-restart_profile")
					{
						i = 0;
						continue;
					}
					if (tempMass[0] == "-goto_question")
					{
						i = Convert.ToInt32(tempMass[1]) - 1;
						continue;
					}
					newProfile[i] = dictionary[i](tempPerem);
					i++;
				}
			newProfile[dictionary.Count] = addYearNow();
			return newProfile;
		}
		public static void Save(string[] mass)
		{
			StreamWriter streamWriter = new StreamWriter(Directory.GetCurrentDirectory() + "\\Анкеты\\" + mass[0].Substring(5) + ".txt");
			foreach (string m in mass)
				streamWriter.WriteLine(m);
			streamWriter.Close();
		}
		public static void Statistics()
		{
			dir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Анкеты");
			files = dir.GetFiles().ToArray();
			int age = 0;
			int coolAge = 0;
			string coolProgrammer = "";
			foreach (FileInfo file in files)
			{
				var data = File.ReadLines(file.FullName);
				foreach (var v in data)
				{
					if (v.Split(": ")[0] == "Дата рождения")
					{
						var date = DateTime.ParseExact(v.Split(": ")[1], "dd.MM.yyyy", CultureInfo.InvariantCulture);
						age = age + DateTime.Now.Year - date.Year;
					}
					if (v.Split(": ")[0] == "Любимый язык программирования")
					{
						progCount[progLang.FindIndex(p => p == v.Split(": ")[1])] += 1;

						
					}
					if (v.Split(": ")[0] == "Опыт программирования на указанном языке")
					{
						if (Convert.ToInt32(v.Split(": ")[1]) > coolAge)
							coolProgrammer = file.Name.Substring(0, file.Name.Length - 4);
					}
				}
			}
			var srAge = age / files.Length;
			var index = progLang[progCount.IndexOf(progCount.Max())];
			Console.WriteLine($"Средний возраст: {srAge} {YearToString(srAge)}");
			Console.WriteLine($"Самый популярный язык: {index}");
			Console.WriteLine($"Самый опытный программист: {coolProgrammer}");
		}
		public static void Find(string name)
		{
			string text = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Анкеты\\" + name + ".txt");
			Console.WriteLine(text);
		}
		public static void Delete(string name)
		{
			File.Delete(Directory.GetCurrentDirectory() + "\\Анкеты\\" + name + ".txt");
		}
		public static void List()
		{
			dir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Анкеты");
			files = dir.GetFiles().ToArray();
			foreach (FileInfo file in files)
				Console.WriteLine(file.Name);
		}
		public static void ListToday()
		{
			dir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Анкеты");
			files = dir.GetFiles().OrderBy(p => p.CreationTime).ToArray();
			foreach (FileInfo file in files)
			{
				if (file.CreationTime.Date == DateTime.Now.Date)
					Console.WriteLine(file.Name);
			}
		}
		public static void Zip(string temp)
		{
			string[] answerMass = temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string newMass = "";
			for (int i = 1; i < answerMass.Length - 1; i++)
				newMass += answerMass[i] + " ";
			newMass = newMass.Substring(0, newMass.Length - 1);
			using (ZipArchive zipArchive = ZipFile.Open((answerMass[answerMass.Length-1] + "\\" + newMass + ".zip"),
						ZipArchiveMode.Create))
			{
				zipArchive.CreateEntryFromFile(Directory.GetCurrentDirectory() + "\\Анкеты\\" + newMass + ".txt", newMass);
			}
		}
		public static void Help()
		{
			string text = File.ReadAllText(Directory.GetCurrentDirectory() + "\\commands.txt");
			Console.WriteLine(text);
		}
		public static void Exit()
		{
			Environment.Exit(0);
		}

		private static string YearToString(int year)
		{
			switch (year % 10)
			{
				case 1: { return "год"; }
				case 2:
				case 3:
				case 4: { return "года"; }
				default: { return "лет"; }
			}
		}
	}
}
