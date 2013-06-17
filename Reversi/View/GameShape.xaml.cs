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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View
{
	/// <summary>
	/// Логика взаимодействия для GameShape.xaml
	/// </summary>
	public partial class GameShape : UserControl
	{

		public int MoveCount { get; set; }
		public int Score { get; set; }
		public TimeSpan gametime { get; set; }
		public List<GameState> Moves = new List<GameState>();

		public GameShape()
		{
			InitializeComponent();
		}

		public void NewGame()
		{
			Score = 0;
			Moves.Clear();
			MoveCount = 0;
			gametime = TimeSpan.FromSeconds(0);

		}

		public void MakeMove(DekShape From, DekShape To, bool calcScore = false)
		{
			if (MoveCount != Moves.Count)
			{
				int n = Moves.Count;
				for (int i = MoveCount; i < n; i++)
				{
					Moves.RemoveAt(Moves.Count - 1);
				}
			}
			List<CardShape> cards = new List<CardShape>();
			while (From.Count > 0)
			{
				if (calcScore)
					Score++;
				var card = From.TopCardShape;
				From.Remove(card);
				To.Add(card);
				cards.Add(card);
			}
			Moves.Add(new GameState(From, To, cards, MoveCount));
		}

		public void StepBack()
		{
			if (MoveCount > 0)
			{
				foreach (var a in Moves)
				{
					if (a.Score == (MoveCount))
					{
						foreach (var b in a.Cards)
						{
							a.To.Remove(b);
							a.From.Add(b);
							Score--;
						}
					}
				}
				MoveCount--;
			}
		}

		public void StepForward()
		{
			if (MoveCount < Moves.Count)
			{
				foreach (var a in Moves)
				{
					if (a.Score == (MoveCount + 1))
					{
						foreach (var b in a.Cards)
						{
							a.From.Remove(b);
							a.To.Add(b);
							Score++;
						}
					}
				}
				MoveCount++;
			}
		}

		public void ReloadGame()
		{
			while (MoveCount > 0)
			{
				StepBack();
			}
			Score = 0;
		}
	}
}
