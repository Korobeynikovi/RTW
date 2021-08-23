using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RusTW
{
	class Result : ICommands
	{
		static string[] newRecordMass = new string[6];

		public void Res()
		{
			Console.WriteLine("Выберете действие");
			string answer = Console.ReadLine();

			ResultFromAnswer(answer);
			
			Res();
		}
		public static void ResultFromAnswer(string answer)
		{
			string[] answerMass = answer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			
			switch (answerMass[0])
			{
				case "-exit":
					ICommands.Exit();
					break;

				case "-help":
					ICommands.Help();
					break;

				case "-new_profile":
					newRecordMass = ICommands.NewProfile(newRecordMass);
					break;

				case "-save":
					ICommands.Save(newRecordMass);
					break;

				case "-delete":
					ICommands.Delete(answer.Substring(7, answer.Length));
					break;

				case "-find":
					ICommands.Find(answer.Substring(7, answer.Length));
					break;

				case "-zip":
					ICommands.Zip(answer);
					break;
				
				case "-list":
					ICommands.List();
					break;
				
				case "-list_today":
					ICommands.ListToday();
					break;

				case "-statistics":
					ICommands.Statistics();
					break;
				default:
					throw new ArgumentException();
			}
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			Result result = new Result();
			result.Res();
		}
	}
}
