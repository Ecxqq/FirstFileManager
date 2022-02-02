/*
 * Created by SharpDevelop.
 * User: Ignatova
 * Date: 12.11.2021
 * Time: 17:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;//подключаем функции работы с файлами
using System.Diagnostics;//подключаем функции для выбора жесткого диска и просмотра аттрибутов
using Microsoft.VisualBasic;//подключаем библиотеку имен из visual basic

namespace FailoviiProcessor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{	//ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ
		private bool isFile = false;//Файл или директория?
		
		private bool isFileForCopy;//Файл ли? для копирования
		
		private string FilePath= "C:\\"; //путь к файлу
		
		private string currentlySelectedItemName = ""; //выбранный файл / папка
		
		private string oldFileDir =""; //старый путь (для копирования и тд)
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//создание кнопок контекстного меню>
			var copyMenuItem = new ToolStripMenuItem("Копировать"); 
			copyMenuItem.Click+=copyMenuItem_Click;
			
			var pasteMenuItem = new ToolStripMenuItem("Вставить");
			pasteMenuItem.Click+=pasteMenuItem_Click;
			
			var copyThisMenuItem = new ToolStripMenuItem("Создать копию");
			copyThisMenuItem.Click+=copyThisMenuItem_Click;
			
			var deleteMenuItem = new ToolStripMenuItem("Удалить");
			deleteMenuItem.Click+=deleteMenuItem_Click;
			
			var renameMenuItem = new ToolStripMenuItem("Переименовать");
			renameMenuItem.Click+=renameMenuItem_Click;
			
			var createDirMenuItem = new ToolStripMenuItem("Создать папку");
			createDirMenuItem.Click+=createDirMenuItem_Click;
			
			var createFileMenuItem = new ToolStripMenuItem("Создать файл");
			createFileMenuItem.Click+=createFileMenuItem_Click;
			
			//добавление в обьект контекстного меню созданных кнопок
			contextMenuStrip1.Items.AddRange(new[] {copyMenuItem, pasteMenuItem, copyThisMenuItem, deleteMenuItem, renameMenuItem, createDirMenuItem, createFileMenuItem});
			listView1.ContextMenuStrip = contextMenuStrip1;//прикрепление контекстного меню к LISTVIEW1
			
			foreach(var drives in DriveInfo.GetDrives())//проходимся по дискам на пк
			{
				comboBox1.Items.Add(Convert.ToString(drives));//добавляем их в комбобокс для возможности выбора дисков
			}
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		private void MainFormLoad(object sender, EventArgs e)//при включении приложения подгружаем файлы 
		{
			LoadFilesAndDirectories();//вызываеем функцию вывода всех файлов
			textBox1.Text = FilePath;//показываем в текстбоксе текущий путь
		}
		
		public void removeBackSlash()//метод удаления слеша (первого с конца)
        {
            string path = textBox1.Text;//временная переменная для хранения пути
            if(path.LastIndexOf("/") == path.Length - 1)
            {
               textBox1.Text = path.Substring(0, path.Length - 1);
            }
        }
		
		public void LoadFilesAndDirectories()//основной метод который подгружает все папки и файлы и входа в выбранные паки или открытия файлов
		{
			DirectoryInfo fileList;
			string tempFilePath=""; //временный путь
			FileAttributes fileAttr;//переменная для хранения аттрибутов файлов или папок
			
			try
			{//если файл уже выбран
				if(isFile)//если файл
				{
					tempFilePath = FilePath + "/" + currentlySelectedItemName;//временный путь это бывший путь + выбранный файл или папка
					FileInfo fileDetails = new FileInfo(tempFilePath);
					label2.Text=fileDetails.Name;//вносим в лейбл2 название файла
					label4.Text=fileDetails.Extension;//вносим в лейбл4 его расширение
					fileAttr = File.GetAttributes(tempFilePath);
					Process.Start(tempFilePath);//запускаем файл
				}
				else
				{
					fileAttr = File.GetAttributes(FilePath);
				}
				if ((fileAttr & FileAttributes.Directory)==FileAttributes.Directory) //если директория (логическое и сравнивает аттрибут выбранного файла и аттрибут директории)
				{
					
					fileList = new DirectoryInfo(FilePath);
					FileInfo[] files = fileList.GetFiles(); //Получим все файлы
					DirectoryInfo[] dirs = fileList.GetDirectories();//Получим все папки
					string fileExtension="";
					listView1.Items.Clear(); //перед добавлением файлов на экран очищаем листвью
					
					for (int i=0; i<files.Length; i++) //проверка какое расширение у файла, для выбора иконки и задания его имени
					{
						fileExtension = files[i].Extension.ToUpper();
                        switch(fileExtension)
                        {
                            case ".MP3":
                            case ".MP2":
                                listView1.Items.Add(files[i].Name, 2);
                                break;
                            case ".EXE":
                            case ".COM":
                            case ".BAT":
                                listView1.Items.Add(files[i].Name, 8);
                                break;

                            case ".MP4":
                            case ".AVI":
                            case ".MKV":
                            case ".MOV":
                                listView1.Items.Add(files[i].Name, 3);
                                break;
                            case ".PDF":
                                listView1.Items.Add(files[i].Name, 4);
                                break;
                            case ".DOC":
                            case ".DOCX":
                                listView1.Items.Add(files[i].Name, 0);
                                break;
                            case ".PNG":
                            case ".JPG":
                            case ".JPEG":
                                listView1.Items.Add(files[i].Name, 5);
                                break;

                            default:
                                listView1.Items.Add(files[i].Name, 7);
                                break;
                        }
					}
					
					for (int i=0; i<dirs.Length; i++)
					{
						listView1.Items.Add(dirs[i].Name, 1); //добавляем папки
					}
				}
				else 
				{
					label2.Text=this.currentlySelectedItemName;
				}
				
			}
			catch
			{
				if (oldFileDir=="") MessageBox.Show("У вас нет доступа к этому каталогу!"); //если произошло исключение
				else MessageBox.Show("Такой каталог уже существует!");
				textBox1.Text=textBox1.Text.Substring(0,textBox1.Text.LastIndexOf("/")); //выходим из паки в которую пытались зайти
				FilePath=textBox1.Text;
			}
		}
		
		public void RefreshDirs() //очистка листвью и заполнение его заново(для отображения новых элементов при копировании и скрытии старых при удалении)
		{
			DirectoryInfo fileList;
			fileList = new DirectoryInfo(FilePath);
			FileInfo[] files = fileList.GetFiles(); //Получим все файлы
			DirectoryInfo[] dirs = fileList.GetDirectories();//Получим все папки
			string fileExtension="";
			listView1.Items.Clear();
					
			for (int i=0; i<files.Length; i++)
			{
				fileExtension = files[i].Extension.ToUpper();
          	  	switch(fileExtension)
        		{
         			case ".MP3":
            	    case ".MP2":
            	    	listView1.Items.Add(files[i].Name, 2);
            	        break;
            	    case ".EXE":
            	    case ".COM":
            	    case ".BAT":
            	        listView1.Items.Add(files[i].Name, 8);
            	        break;
	
            	    case ".MP4":
            	    case ".AVI":
            	    case ".MKV":
            	    case ".MOV":
            	        listView1.Items.Add(files[i].Name, 3);
            	        break;
            	    case ".PDF":
            	        listView1.Items.Add(files[i].Name, 4);
            	        break;
            	    case ".DOC":
            	    case ".DOCX":
            	        listView1.Items.Add(files[i].Name, 0);
            	        break;
            	    case ".PNG":
            	    case ".JPG":
            	    case ".JPEG":
            	        listView1.Items.Add(files[i].Name, 5);
            	    	break;
	
            	    default:
            	        listView1.Items.Add(files[i].Name, 7);
            	        break;
            	}
			}
          	for (int i=0; i<dirs.Length; i++)
			{
				listView1.Items.Add(dirs[i].Name, 1);
			}
				
		}
		
		public void LoadButtonAction() //обработчик действия кнопок
		{
			
			removeBackSlash(); //удаляем слеш
			FilePath = textBox1.Text; //задаем новый путь
			LoadFilesAndDirectories(); //входим в каталог и выводим его содержимое на экран
			isFile = false; //выбранное больше не файл
		}
		
		public void goBack()//метод кнопки назад
        {
            try
            {
                removeBackSlash();
                string path = textBox1.Text;
                path = path.Substring(0, path.LastIndexOf("/"));//путь теперь не включает папки из которой мы вышли
                this.isFile = false; //не файл
                textBox1.Text = path;//выводим новый путь
                removeBackSlash();//удаляем слеш
            }
            catch
            {

            }
        }
		

		void Button2Click(object sender, EventArgs e)//кнопка далее
		{
			LoadButtonAction();
		}
		void ListView1ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)//выбранный элемент
		{
			currentlySelectedItemName = e.Item.Text;//выбранный элемент равен тому что выбрал пользователь
			try//блок для вывода в лейблы названия и расширения файла
			{
				FileAttributes fileAttr = File.GetAttributes(FilePath + "/" + currentlySelectedItemName);
				if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)//если выбрана папка 
				{
					isFile= false;//не файл
					textBox1.Text = FilePath + "/" + currentlySelectedItemName;
					label2.Text="--";//нет имени
					label4.Text="--";//нет расширения
				}
				else
				{
					isFile= true;
					label2.Text=currentlySelectedItemName.Substring(0,currentlySelectedItemName.LastIndexOf("."));//пишем название файла
					label4.Text=currentlySelectedItemName.Substring(currentlySelectedItemName.LastIndexOf("."),4);//пишем расширение файла
				}	
			}
			catch{}
			
			
		}
		void ListView1MouseDoubleClick(object sender, MouseEventArgs e)//открытие спомощью двойного клика мыши
		{
			LoadButtonAction();
		}
		void Button1Click(object sender, EventArgs e) //кнопка назад
		{
			goBack();
			LoadButtonAction();
		}
		
		void pasteMenuItem_Click(object sender, EventArgs e) //кнопка вставить
   		{
			if (isFileForCopy)//если файл
			{
				try
				{
					FileInfo TempfileDetails = new FileInfo(oldFileDir);
		
					File.Copy(oldFileDir,(oldFileDir.Substring(0,oldFileDir.LastIndexOf("."))+"^"+TempfileDetails.Extension),true); //создаем в папке откуда копируем файл его копию
					if (oldFileDir.Contains("/"))
					File.Move(oldFileDir.Substring(0,oldFileDir.LastIndexOf("."))+"^"+TempfileDetails.Extension,FilePath+oldFileDir.Substring(oldFileDir.LastIndexOf("/"),oldFileDir.Length-oldFileDir.LastIndexOf("/")));//переносим созданную копию в новый путь
					else File.Move(oldFileDir.Substring(0,oldFileDir.LastIndexOf("."))+"^"+TempfileDetails.Extension,FilePath+oldFileDir.Substring(oldFileDir.LastIndexOf("\\"),oldFileDir.Length-oldFileDir.LastIndexOf("\\")));//если переносим из коренного каталолга диска
				}
				catch{}
			}
			else//если папка
			{
            	// First create all of the directories
            	try
				{
            		Directory.CreateDirectory(FilePath+oldFileDir.Substring(oldFileDir.LastIndexOf("/"),oldFileDir.Length-oldFileDir.LastIndexOf("/")));//создаем папку которую копируем
            		textBox1.Text+="/"+oldFileDir.Substring(oldFileDir.LastIndexOf("/"),oldFileDir.Length-oldFileDir.LastIndexOf("/"));//меняем путь
            	
            		FilePath=FilePath+"/"+oldFileDir.Substring(oldFileDir.LastIndexOf("/"),oldFileDir.Length-oldFileDir.LastIndexOf("/"));//входим в созданную папку по новому пути имитируя нажатие кнопки далее
            	
            		foreach (string dirPath in Directory.GetDirectories(oldFileDir, "*", SearchOption.AllDirectories))//перебираем все папки внутри папки которую копируем и вставляем их в новую папку
            		{
                		Directory.CreateDirectory(dirPath.Replace(oldFileDir, FilePath));
            		}
 
            		foreach (string newPath in Directory.GetFiles(oldFileDir, "*.*", SearchOption.AllDirectories))//перебираем все файлы и вставляем их
            		{
                		File.Copy(newPath, newPath.Replace(oldFileDir, FilePath), true);
            		}
				}
            	catch
            	{
            		MessageBox.Show("файл не выбран или в этом каталоге он уже существует!");//если произошло исключение
            	}
			}
			oldFileDir="";//очищаем путь к файлу который уже скопировали
			RefreshDirs();//обновляем папки и файлы на экране
		}
			
		
		void copyMenuItem_Click(object sender, EventArgs e)//копия
   		{
			
			{
				if (currentlySelectedItemName.Contains(".")) isFileForCopy=true;
				else isFileForCopy=false;
				if (!isFileForCopy)
				oldFileDir = textBox1.Text;//копируем путь к выбранному элементу
				else oldFileDir = FilePath+currentlySelectedItemName;
			}
    	}
		
		void copyThisMenuItem_Click(object sender, EventArgs e)//копия файла в эту же папку
   		{
			oldFileDir = FilePath + "/" + currentlySelectedItemName; //сохраняем путь к файлу который копируем
			
			var TempI = 0;
			FileInfo TempfileDetails = new FileInfo(oldFileDir);
			
			try
			{
				while (File.Exists(oldFileDir.Substring(0,oldFileDir.LastIndexOf("."))+"(Copied)"+TempI+TempfileDetails.Extension)) //существует ли файл с таким же именем?
				{
					TempI++; //добавляем цифру в конец
				}
				
				File.Copy(oldFileDir,(oldFileDir.Substring(0,oldFileDir.LastIndexOf("."))+"(Copied)"+TempI+TempfileDetails.Extension));//создаем копию с новым именем
			}
			catch
			{	
			}
			RefreshDirs();
    	}
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)//выбор диска при нажатии на кнопку в комбобоксе с проверкой рекурсией
		{
			int temp=0;//для обработки исключения когда нельзя получить доступ к диску
			try
			{
				currentlySelectedItemName="";//выбранный файл очищаем во избежание ошибок
				isFile=false; //не файл
				FilePath = comboBox1.SelectedItem.ToString(); //путь меняется на тот что выбран в комбобоксе
				textBox1.Text = (comboBox1.SelectedItem.ToString());//выводим путь
				LoadButtonAction();//имитируем нажатие кнопки далее
				textBox1.Text = (comboBox1.SelectedItem.ToString()).Substring(0,2);//корректируем путь в текстбоксе
			}
			catch//если диск недоступен то
			{
				comboBox1.SelectedIndex=temp;  //выбранный диск равняется диску с номером темп
				temp++; //увеличиваем темп, чтобы если следующий диск недоступен мы могли снова сдвинуться на другой диск
				ComboBox1SelectedIndexChanged(comboBox1, e); //снова вызываем метод для смены диска
			}
		}
		
		void deleteMenuItem_Click(object sender, EventArgs e)//удалить
   		{
			try
			{
				File.Delete(FilePath + "/" + currentlySelectedItemName);//удаляем выбранное
				currentlySelectedItemName="";//выбранного больше нет
				isFile=false; //больше не файл
			}
			catch
			{
				Directory.Delete(FilePath + "/" + currentlySelectedItemName,true);//если выбранное было папкой то удаляем ее с разрешением на удаление всех вложенных папок и файлов
				currentlySelectedItemName="";//вбранного больше нет
				textBox1.Text=textBox1.Text.Substring(0,textBox1.Text.LastIndexOf("/"));//выводим путь без названия удаленной папки
			}
			RefreshDirs();//обновляем экран
    	}
		
		void renameMenuItem_Click(object sender, EventArgs e)//переименовать
   		{
			string Input = Microsoft.VisualBasic.Interaction.InputBox("Введите новое название файла","Файловый менеджер","",300,300);//вызываем окно со вводом нового названия
			try
			{
				File.Move(FilePath+"/"+currentlySelectedItemName,FilePath+"/"+Input+currentlySelectedItemName.Substring(currentlySelectedItemName.LastIndexOf("."),4));//переименовываем если файл
			}
			catch{}
			try
			{
				Directory.Move(FilePath+"/"+currentlySelectedItemName,FilePath+"/"+Input);//переименовываем если папка
			}
			catch{}
			
			
			RefreshDirs();
    	}
		
		void createDirMenuItem_Click(object sender, EventArgs e)//создание папки
   		{
			string Input = Microsoft.VisualBasic.Interaction.InputBox("Введите название папки","Файловый менеджер","",300,300);//вызываем окно со вводом нового названия
			
			try
			{
				Directory.CreateDirectory(FilePath+"/"+Input);//создаем директорию с введеным именем
			}
			catch{}
			
			RefreshDirs();
    	}
		
		void createFileMenuItem_Click(object sender, EventArgs e)//создание файла
   		{
			string Input = Microsoft.VisualBasic.Interaction.InputBox("Введите название файла, и его расширение через точку","Файловый менеджер","",300,300);//вызываем окно со вводом нового названия
			
			try
			{
				if (Input.Contains(".")) //содержит ли новое имя расширение?
				File.Create(FilePath+"/"+Input).Close(); //создаем директорию с введеным именем если введено расширение
				else File.Create(FilePath+"/"+Input+".txt").Close(); //создаем директорию с введеным именем если не введено расширение
			}
			catch{}
			
			RefreshDirs();
    	}
	}
}
