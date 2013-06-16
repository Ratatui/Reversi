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

		public int MoveCount { get; private set; }
		public int Score { get; private set; }
		public TimeSpan gametime { get; set; }
		List<GameState> Moves = new List<GameState>();

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

		public void MakeMove(DekShape From, DekShape To, int Score, bool needflipback)
		{
			if (MoveCount != Moves.Count)
			{
				int n = Moves.Count;
				for (int i = MoveCount; i < n; i++)
				{
					Moves.RemoveAt(Moves.Count - 1);
				}
			}
			Moves.Add(new GameState(From, To, needflipback, Score));
			this.Score += Score;
			MoveCount++;
		}

		public void StepBack()
		{
			if (MoveCount > 0)
			{
				Score -= Moves[MoveCount - 1].Score;
				Moves[MoveCount - 1].From.Add(Moves[MoveCount - 1].To.TopCardShape);
				if (Moves[MoveCount - 1].NeedFlipBack)
				{
					Moves[MoveCount - 1].From.Deck.TopCard.Visible = false;
				}
				MoveCount--;
			}
		}

		public void StepForward()
		{
			if (MoveCount < Moves.Count)
			{
				Moves[MoveCount].To.Add(Moves[MoveCount].From.TopCardShape);
				if (Moves[MoveCount].NeedFlipBack)
				{
					Moves[MoveCount].From.Deck.TopCard.Visible = true;
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
