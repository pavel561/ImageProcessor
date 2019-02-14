using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Xml;
using System.Net;

namespace ImageProcessor
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("ImageProcessor ©StudentCorp. ");
			Console.WriteLine("-------------------------------");
			Console.WriteLine("Выберите требуемое действие: ");
			Console.WriteLine("[N] - RenameByDate. Переименование фотографий в соотв. с датой съемки ");
			Console.WriteLine("[T] - DateStamp. Режим наложения даты съемки ");
			Console.WriteLine("[S] - SortByYear. Сортировка изображений по папкам в соотв. с годом съемки  ");
			Console.WriteLine("[L] - FolderByLocation. Сортировка изображений по папкам в соотв. с местом съемки  ");
			ConsoleKey answer = Console.ReadKey(true).Key;
			switch(answer)
			{
				case ConsoleKey.N:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран режим переименования ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами >> ");
						string filePath = Console.ReadLine();
						RenameByDate(filePath);
						break;
					}
				case ConsoleKey.T:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран режим наложения даты съемки ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами >> ");
						string filePath = Console.ReadLine();
						DateStamp(filePath);
						break;
					}
				case ConsoleKey.S:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран рехим сортировки по году ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами >> ");
						string filePath = Console.ReadLine();
						SortByYear(filePath);
						break;
					}
				case ConsoleKey.L:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран рехим сортировки по геолокации ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами >> ");
						string filePath = Console.ReadLine();
						SortByLocation(filePath);
						break;
					}
			}
			Console.WriteLine("");
			Console.WriteLine("Операция выполнена успешно");
			Console.ReadKey();


		}
		public static void RenameByDate(string path)
		{
			var dir = new DirectoryInfo(path);
			string newDirPath = path + "\\" + dir.Name + "-RenameByDate";
			Directory.CreateDirectory(newDirPath);
			var files = dir.GetFiles("*.jpg");
			foreach(FileInfo file in files)
			{
				Bitmap image = new Bitmap(file.FullName);
				Image img = Image.FromFile(file.FullName);
				PropertyItem item = image.GetPropertyItem(0x132);
				string takenDate = System.Text.Encoding.UTF8.GetString(item.Value, 0, item.Value.Length);
				takenDate = takenDate.Replace(':', '-').Replace(' ', '_').Substring(0,takenDate.Length-1);
				var creationTime = file.CreationTime;
				string newName = newDirPath + "\\" + file.Name.Substring(0, file.Name.Length-4).Replace(' ', '_') + "_" + file.CreationTime.ToString().Replace(' ', '_').Replace(':','-') + ".jpg"; //file.Name.Substring(0, file.Name.Length-4).Replace(' ', '_') + "_"
				file.CopyTo(newName, true);
			}
		}
		public static void DateStamp(string path)
		{
			var sourceDirPath = new DirectoryInfo(path);
			string newDirPath = path + "\\" + sourceDirPath.Name + "-DateStamp";
			Directory.CreateDirectory(newDirPath);
			var files = sourceDirPath.GetFiles("*.jpg");
			foreach (FileInfo file in files)
			{

				Image image = new Bitmap(file.FullName);
				Bitmap bitmap = new Bitmap(image);
				PropertyItem item = image.GetPropertyItem(0x132);
				string takenDate = System.Text.Encoding.UTF8.GetString(item.Value, 0, item.Value.Length);
				takenDate = takenDate.Replace(':', '-').Replace(' ', '_').Substring(0, takenDate.Length - 1);
				var fileName = file.Name.Substring(0, file.Name.Length - 4).Replace(' ', '_');
				string newName = newDirPath + "\\" + fileName + "_" + takenDate + ".jpg";
				float h_offset = (float)(0.05 * bitmap.Height);
				float w_offes = (float)(bitmap.Width-800);
				Font drawFont = new Font("Arial", 40);
				SolidBrush drawBrush = new SolidBrush(Color.Red);
				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DisplayFormatControl;
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.DrawString(takenDate, drawFont, drawBrush, w_offes, h_offset, drawFormat);
				bitmap.Save(newName, ImageFormat.Jpeg);

			}
		}
		public static void SortByYear(string path)
		{
			// Берем файл.
			// Считываем год съемки
			// Проверяем, существует ли такая папка.
			// Если нет - создаем
			// Копируем файл в папку

			var sourceDirPath = new DirectoryInfo(path);
			string newDirPath = path + "\\" + sourceDirPath.Name + "-SortByYear";
			Directory.CreateDirectory(newDirPath);
			var files = sourceDirPath.GetFiles("*.jpg");
			foreach (FileInfo file in files)
			{

				Image image = new Bitmap(file.FullName);

				PropertyItem item = image.GetPropertyItem(0x132);
				string Year = System.Text.Encoding.UTF8.GetString(item.Value, 0, item.Value.Length).Substring(0,4);
				if(Directory.Exists(newDirPath + "\\" + Year))
				{
					file.CopyTo(newDirPath + "\\" + Year + "\\" + file.Name,true);
				}
				else
				{
					Directory.CreateDirectory(newDirPath + "\\" + Year);
					file.CopyTo(newDirPath + "\\" + Year + "\\" + file.Name,true);
				}
			}
		}
		public static void SortByLocation(string path)
		{
			var sourceDirPath = new DirectoryInfo(path);
			string newDirPath = path + "\\" + sourceDirPath.Name + "-SortByLocation";
			Directory.CreateDirectory(newDirPath);
			var files = sourceDirPath.GetFiles("*.jpg");
			//string ApiKey = "4bb2d530-2f04-40f4-a2e2-2f78c9f7dddc";
			string ApiUrl = "https://geocode-maps.yandex.ru/1.x/?apikey=4bb2d530-2f04-40f4-a2e2-2f78c9f7dddc&geocode=";
			//BitmapMetadata 
			//WebRequest locationRequest = WebRequest.Create(ApiUrl);
			foreach(FileInfo file in files)
			{
				Image image = new Bitmap(file.FullName);
				try
				{
					PropertyItem GpsLatitude = image.GetPropertyItem(0x0002);
					//GpsLatitude.Value.
					double LatitudeDeg = (double) BitConverter.ToInt32(GpsLatitude.Value, 0)/BitConverter.ToInt32(GpsLatitude.Value, 4);
					double LatitudeMin = (double) BitConverter.ToInt32(GpsLatitude.Value, 8) / BitConverter.ToInt32(GpsLatitude.Value, 12);
					double LatitudeSec = (double) BitConverter.ToInt32(GpsLatitude.Value, 16) / BitConverter.ToInt32(GpsLatitude.Value, 20);
					decimal Latitude = (decimal)LatitudeDeg + ((decimal)LatitudeMin / 60) + ((decimal)LatitudeSec / 3600);
					string LatitudeStr = String.Format("{0:f6}", Latitude).Replace(',','.');

					PropertyItem GpsLongitude = image.GetPropertyItem(0x0004);
					double LongitudeDeg = (double)BitConverter.ToInt32(GpsLongitude.Value, 0) / BitConverter.ToInt32(GpsLongitude.Value, 4);
					double LongitudeMin = (double)BitConverter.ToInt32(GpsLongitude.Value, 8) / BitConverter.ToInt32(GpsLongitude.Value, 12);
					double LongitudeSec = (double)BitConverter.ToInt32(GpsLongitude.Value, 16) / BitConverter.ToInt32(GpsLongitude.Value, 20);
					decimal Longitude = (decimal)LongitudeDeg + ((decimal)LongitudeMin / 60) + ((decimal)LongitudeSec / 3600);
					string LongitudeStr = String.Format("{0:f6}", Longitude).Replace(',', '.');

					Console.WriteLine($"{file.Name}; GpsLatitude - {LatitudeStr}; GpsLongitude - {LongitudeStr}"); 
				}
				catch
				{
					Console.WriteLine($"Файл {file.Name} не содержит информации о геолокации");
				}
			}



		}
	}
}
