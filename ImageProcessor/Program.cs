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
using System.Net.Http;
using System.Xml.Serialization;

namespace ImageProcessor
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("ImageProcessor pavel561@gmail.com. ");
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
						Console.Write("Введите путь к папке с файлами (в формате C:\\Folder\\Folder) >> ");
						string filePath = Console.ReadLine();
						RenameByDate(filePath);
						break;
					}
				case ConsoleKey.T:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран режим наложения даты съемки ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами (в формате C:\\Folder\\Folder) >> ");
						string filePath = Console.ReadLine();
						DateStamp(filePath);
						break;
					}
				case ConsoleKey.S:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран рехим сортировки по году ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами (в формате C:\\Folder\\Folder) >> ");
						string filePath = Console.ReadLine();
						SortByYear(filePath);
						break;
					}
				case ConsoleKey.L:
					{
						Console.WriteLine("--------------------------------- ");
						Console.WriteLine("Вибран рехим сортировки по геолокации ");
						Console.WriteLine("--------------------------------- ");
						Console.Write("Введите путь к папке с файлами (в формате C:\\Folder\\Folder) >> ");
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
			//Берем файл
			//Считываем дату съемки
			//Сохраняем файл, переименовав его в соответствии с датой съемки
			

			var dir = new DirectoryInfo(path);
			string newDirPath = path + "\\" + dir.Name + "-RenameByDate";
			Directory.CreateDirectory(newDirPath);
			var files = dir.GetFiles("*.jpg");

			foreach(FileInfo file in files)
			{
				Bitmap image = new Bitmap(file.FullName);
				Image img = Image.FromFile(file.FullName);
				//Считываем дату съемки
				PropertyItem item = image.GetPropertyItem(0x132);
				string takenDate = System.Text.Encoding.UTF8.GetString(item.Value, 0, item.Value.Length);
				takenDate = takenDate.Replace(':', '-').Replace(' ', '_').Substring(0,takenDate.Length-1);
				var creationTime = file.CreationTime;
				string newName = newDirPath + "\\" + file.Name.Substring(0, file.Name.Length-4).Replace(' ', '_') + "_" + file.CreationTime.ToString().Replace(' ', '_').Replace(':','-') + ".jpg"; //file.Name.Substring(0, file.Name.Length-4).Replace(' ', '_') + "_"
				//Сохраняем файл, переименовав его в соответствии с датой съемки
				file.CopyTo(newName, true);
			}
		}
		public static void DateStamp(string path)
		{
			// Берем файл.
			// Считываем дату съемки
			// Создаем объект битмап
			// Рисуем в правом верхнем углу дату съемки
			// Сохраняем файл в папке назначения


			var sourceDirPath = new DirectoryInfo(path);
			string newDirPath = path + "\\" + sourceDirPath.Name + "-DateStamp";
			Directory.CreateDirectory(newDirPath);
			var files = sourceDirPath.GetFiles("*.jpg");
			foreach (FileInfo file in files)
			{
				// Создаем объект битмап
				Image image = new Bitmap(file.FullName);
				Bitmap bitmap = new Bitmap(image);
				// Считываем дату съемки
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
				// Рисуем в правом верхнем углу дату съемки
				graphics.DrawString(takenDate, drawFont, drawBrush, w_offes, h_offset, drawFormat);
				// Сохраняем файл в папке назначения
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
			string ApiKey = "4bb2d530-2f04-40f4-a2e2-2f78c9f7dddc";	//Ключ для доступа к API Яндекса
			string ApiUrl = $"https://geocode-maps.yandex.ru/1.x/?apikey=" + ApiKey+ "&geocode=";
			
			foreach(FileInfo file in files)
			{
				Image image = new Bitmap(file.FullName);
				string LatitudeStr = null;
				string LongitudeStr = null;
				try
				{
					//Счытываем информацию о геолокации из фотографии
					//и форматируем в необхлдимый для отправки формат
					PropertyItem GpsLatitude = image.GetPropertyItem(0x0002);
					double LatitudeDeg = (double)BitConverter.ToInt32(GpsLatitude.Value, 0) / BitConverter.ToInt32(GpsLatitude.Value, 4);
					double LatitudeMin = (double)BitConverter.ToInt32(GpsLatitude.Value, 8) / BitConverter.ToInt32(GpsLatitude.Value, 12);
					double LatitudeSec = (double)BitConverter.ToInt32(GpsLatitude.Value, 16) / BitConverter.ToInt32(GpsLatitude.Value, 20);
					decimal Latitude = (decimal)LatitudeDeg + ((decimal)LatitudeMin / 60) + ((decimal)LatitudeSec / 3600);
					LatitudeStr = String.Format("{0:f6}", Latitude).Replace(',', '.');

					PropertyItem GpsLongitude = image.GetPropertyItem(0x0004);
					double LongitudeDeg = (double)BitConverter.ToInt32(GpsLongitude.Value, 0) / BitConverter.ToInt32(GpsLongitude.Value, 4);
					double LongitudeMin = (double)BitConverter.ToInt32(GpsLongitude.Value, 8) / BitConverter.ToInt32(GpsLongitude.Value, 12);
					double LongitudeSec = (double)BitConverter.ToInt32(GpsLongitude.Value, 16) / BitConverter.ToInt32(GpsLongitude.Value, 20);
					decimal Longitude = (decimal)LongitudeDeg + ((decimal)LongitudeMin / 60) + ((decimal)LongitudeSec / 3600);
					LongitudeStr = String.Format("{0:f6}", Longitude).Replace(',', '.');
				}
				catch
				{
					Console.WriteLine($"Файл {file.Name} не содержит информации о геолокации");
				}
				if (LongitudeStr != null && LatitudeStr != null)
				{
					HttpWebRequest locationRequest = (HttpWebRequest)WebRequest.Create(ApiUrl + LongitudeStr + "," + LatitudeStr);
					//locationRequest.Timeout = 10000;
					HttpWebResponse locationResponse = (HttpWebResponse)locationRequest.GetResponse();
					if (locationResponse.StatusCode == HttpStatusCode.OK)
					{
						string LocationAdressStr = null;

						using (XmlReader reader = XmlReader.Create(locationResponse.GetResponseStream()))
						{
							bool AdressFounded = false;
							bool whileStop = false;
							//Десериализуем XML в ручном режиме (ищем первое вхождение нужного название поля и считываем его значение)
							while (reader.Read() && !whileStop)
							{
								switch (reader.NodeType)
								{
									case XmlNodeType.Element:
										{
											if (reader.Name == "formatted")
											{
												AdressFounded = true;
											}
											break;
										}

									case XmlNodeType.Text:
										{
											if (AdressFounded)
											{
												LocationAdressStr = reader.Value;
												whileStop = true;
											}
											break;
										}

									case XmlNodeType.EndElement:
										{
											break;
										}

									default:
										{
											break;
										}

								}
							}

						}
						//Сбрасываем состояние запроса и закрываем поток ответа
						locationRequest.Abort();
						locationResponse.Close();
						//API яндекса отдает адрес в виде сторки, отделяя запятой город, область, и т.д. 
						//Пробую отформатировать строку, и вырезать нужное количество информации для создания соответствующих папок
						//Метод Split почему-то некорректно извлекает из строки-источника подстроки, если пытаться считать только две подстроки
						//Он не видит втроую запятую, и формирует вместо двух строк вида:
						// string[1] = "Беларусь"; string[2] = "Область"
						//Формирует строки вида:
						// string[1] = "Беларусь"; string[2] = "Область, Район" (не принимает запятую за разделитель)
						//Поэтому считываем в лист три подстроки, и затем удаляем лишнюю.
						List<string> LocationStr = LocationAdressStr.Replace(", ", ",").Replace(' ','_').Split(new char[] { ',' }, 3).ToList();
						LocationStr.RemoveAt(2);
						//Формируем относительный адрес расположения папок ("\\Страна\\Область(Город)")
						string LocationAdr = null;
						foreach (string str in LocationStr)
						{
							LocationAdr += "\\";
							LocationAdr += str;
						}
						string ForderStr = newDirPath;
						//Создаем новыую папку(если папка существует, только копируем файл в неё)
						if (Directory.Exists(ForderStr + LocationAdr))
						{
							file.CopyTo(ForderStr + LocationAdr + "\\" + file.Name, true);
						}
						else
						{
							Directory.CreateDirectory(ForderStr + LocationAdr);
							file.CopyTo(ForderStr + LocationAdr + "\\" + file.Name, true);
						}
						Console.WriteLine($"Место съемки {file.Name} - {LocationAdressStr}");
					}
					else
					{
						Console.WriteLine("Ошибка");
					}
				}

			}

		}
	}
}
