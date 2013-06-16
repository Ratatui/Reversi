using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Napoleon_Exile
{
	/// <summary>
	/// Логика взаимодействия для Statistic.xaml
	/// </summary>
	public partial class Statistic : Window
	{
		public List<PlayerInfo> Players { get; set; }
		public Statistic()
		{
			Players = new List<PlayerInfo>();
			StreamReader f = new StreamReader(Environment.CurrentDirectory + "\\records.txt");
			string buff = "";

			while ((buff = f.ReadLine()) != null)
			{
				string[] info = buff.Split(new char[] { ' ' });
				if (buff != "")
				{
					PlayerInfo p = new PlayerInfo
					{
						GameName = info[0],
						Moves = info[1],
						Score = info[2],
						State = info[3],
						PlayerName = info[4]
					};
					Players.Add(p);
				}
			}
			f.Close();
			this.DataContext = this;
			InitializeComponent();

		}
	}
}
