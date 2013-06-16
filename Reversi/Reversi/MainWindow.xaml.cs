using Model;
using System;
using System.Collections.Generic;
using System.IO;
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
using View;

namespace Napoleon_Exile
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		double space = 40;
		List<DekShape> stack = new List<DekShape>();
		List<DekShape> ground = new List<DekShape>();
		DekShape dealer = new DekShape();

		DekShape oldDeck;
		System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timer_Tick;
			Canvas.SetLeft(dealer, 10);
			Canvas.SetTop(dealer, space);
			Canvas.SetZIndex(dealer, 1);
			cnv.Children.Add(dealer);

			//Игровая зона
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					DekShape ds = new DekShape();
					Canvas.SetLeft(ds, 100 + j * (CardShape.CardWidth + 10));
					Canvas.SetTop(ds, space + (CardShape.CardHeight + space) * i);
					Canvas.SetZIndex(ds, 1);
					cnv.Children.Add(ds);
					ds.CardSpace = 17;
					ground.Add(ds);
				}
			}
		}

		void timer_Tick(object sender, EventArgs e)
		{
			gameShape.gametime = gameShape.gametime.Add(TimeSpan.FromSeconds(1));
			//TotalTime.Text = gameShape.gametime.Hours.ToString() + ":" + gameShape.gametime.Minutes.ToString() + ":" + gameShape.gametime.Seconds.ToString();
			TotalTime.Text = gameShape.gametime.ToString("hh\\:mm\\:ss");
		}

		void NewGame()
		{
			timer.Stop();
			gameShape.NewGame();
			TotalScore.Text = gameShape.Score.ToString();
			TotalMoves.Text = gameShape.MoveCount.ToString();
			dealer.Clear();
			
			foreach (var d in stack)
			{
				d.Clear();
			}
			foreach (var d in ground)
			{
				d.Clear();
			}
			//Создаем карты
			Deck deal = new Deck();
			for (int i = 0; i < 2; i++)
			{
				for (int suit = 1; suit <= 4; suit++)
				{
					for (int number = 1; number <= 13; number++)
					{
						Card c = new Card((CardRank)number, (CardSuit)suit);
						deal.TopCard = c;
					}
				}
				deal.Shuffle(5);
			}
			deal.Shuffle(10);

			//Добавляем в основную колоду
			for (int i = 0; i < 104; i++)
			{
				CardShape cp = new CardShape();
				cp.Card = deal.TopCard;
				deal.TakeTopCard();
				cp.MouseLeftButtonDown += cp_MouseLeftButtonDown;
				cp.PreviewMouseRightButtonDown += cp_PreviewMouseRightButtonDown;
				//cp.MouseDoubleClick += cp_MouseDoubleClick;
				cp.CardDragged += cp_CardDragged;
				cp.MouseMove += cp_MouseMove;
				dealer.Add(cp);
			}

			//разигрываем по 1 карты в стопку
			//21
			for (int j = 0; j < ground.Count; j++)
			{
				ground[j].Add(dealer.TopCardShape);
			}

			for (int j = 0; j < ground.Count; j++)
			{
				ground[j].Deck.FlipAllCards(true);
				ground[j].Deck.TopCard.Enabled = true;
				ground[j].Deck.TopCard.IsDragable = false;
			}

			dealer.Deck.MakeAllCardsDragable(false);
			dealer.Deck.TopCard.Visible = true;
			dealer.Deck.TopCard.IsDragable = true;
			Canvas.SetZIndex(dealer, 0);
			timer.Start();
		}

		void cp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			CardShape current = sender as CardShape;
			foreach (var d in stack)
			{
				if (d.Deck == current.Card.Deck)
				{
					oldDeck = d;
					break;
				}
			}
			foreach (var d in ground)
			{
				if (d.Deck == current.Card.Deck)
				{
					oldDeck = d;
					break;
				}
			}
			
			current.IsEnabled = true;

			//ищем куда можно переместить
			if (current.Card.Rank == CardRank.Ace)
			{
				foreach (var d in stack)
				{
					if (d.Count == 0)
					{
						MakeMove(d, current, 10, false);
						break;
					}
				}
			}
			else
			{//если не туз
				bool find = false;
				foreach (var d in stack)
				{
					if (d.Count > 0)
						if (current.Card.Suit == d.Deck.TopCard.Suit && current.Card.Rank > d.Deck.TopCard.Rank && current.Card.Rank != CardRank.Ace &&
							(current.Card.Rank - d.Deck.TopCard.Rank == 1))
						{
							MakeMove(d, current, 10, false);
							d.Deck.TopCard.IsDragable = false;
							find = true;
							break;
						}
				}
				if (!find)
				{
					foreach (var d in ground)
					{
						if (current.Card.Rank < d.Deck.TopCard.Rank && current.Card.Rank != CardRank.Ace && (d.Deck.TopCard.Rank - current.Card.Rank == 1))
						{//тузов кладем только в стэк
							MakeMove(d, current, 0, false);
							find = true;
							break;
						}
					}
				}
				if (!find)
				{
					oldDeck.Remove(current);
					oldDeck.Add(current);
				}
			}
		}

		private bool IsWon()
		{
			if (dealer.Count > 0) return false;
			
			foreach (var ds in stack)
			{
				if (ds.Deck.TopCard.Rank != CardRank.King) return false;
			}
			foreach (var dg in ground)
			{
				if (dg.Count > 0) return false;
			}
			return true;
		}

		private void MakeMove(DekShape d, CardShape shape, int score, bool needflipback)
		{
			oldDeck.Remove(shape);
			d.Add(shape);
			d.Deck.TopCard.IsDragable = true;

			gameShape.MakeMove(oldDeck, d, score, needflipback);
			TotalScore.Text = gameShape.Score.ToString();
			TotalMoves.Text = gameShape.MoveCount.ToString();
		}

		void cp_MouseMove(object sender, MouseEventArgs e)
		{
			if (oldDeck != null)
				Canvas.SetZIndex(oldDeck, 300);

		}

		void cp_CardDragged(CardShape shape)
		{
			double dx = (Canvas.GetLeft(shape) + Canvas.GetLeft((UIElement)((Canvas)shape.Parent).Parent));
			double dy = (Canvas.GetTop(shape) + Canvas.GetTop((UIElement)((Canvas)shape.Parent).Parent));

			Rect rs = new Rect(dx, dy, CardShape.CardWidth, CardShape.CardHeight);
			bool find = false;
			foreach (var d in stack)
			{//ищем перемещение в стэк
				Rect r = new Rect(Canvas.GetLeft(d), Canvas.GetTop(d), CardShape.CardWidth, CardShape.CardHeight + d.CardSpace * d.Count);
				if (rs.IntersectsWith(r))
				{
					find = true;
					//правила. если колода пустая и мы кладем туза, то все хорошо
					if (d.Count == 0)
					{
						if (shape.Card.Rank == CardRank.Ace)
						{
							MakeMove(d, shape, 10, false);
							d.IsEnabled = false;
							break;
						}
					}
					else if (shape.Card.Suit == d.Deck.TopCard.Suit && shape.Card.Rank > d.Deck.TopCard.Rank && shape.Card.Rank != CardRank.Ace &&
						(shape.Card.Rank - d.Deck.TopCard.Rank == 1))
					{
						MakeMove(d, shape, 10, false);
						d.Deck.TopCard.IsDragable = false;
						break;
					}
					find = false;
					break;
				}
			}
			if (!find)
			{//если не в стэк, то может игровые
				foreach (var d in ground)
				{
					Rect r = new Rect(Canvas.GetLeft(d), Canvas.GetTop(d), CardShape.CardWidth, CardShape.CardHeight + d.CardSpace * d.Count);
					if (rs.IntersectsWith(r))
					{
						find = true;
						//правила. если колода пустая можно класть все, что угодно
						if (d.Count == 0)
						{
							MakeMove(d, shape, 0, false);
							//d.IsEnabled = false;
							break;
						}
						else if (shape.Card.Rank < d.Deck.TopCard.Rank && shape.Card.Rank != CardRank.Ace && (d.Deck.TopCard.Rank - shape.Card.Rank == 1))
						{//тузов кладем только в стэк
							MakeMove(d, shape, 0, false);
							break;
						}
						find = false;
						break;
					}
				}
			}
			if (!find)
			{//если не можем переместить, то возвращаем в старую колоду
				oldDeck.Remove(shape);
				oldDeck.Add(shape);
			}
			else
			{

				if (dealer.Deck.TopCard != null)
				{//если дилер не пуст, активизируем верхнюю карту
					dealer.Deck.TopCard.Visible = false;
					dealer.Deck.TopCard.IsDragable = true;
				}
			}
			if (IsWon())
			{
				System.Windows.MessageBox.Show("Вы выиграли");

				string name = "no name";
				GetPlayerNameDialog dlg = new GetPlayerNameDialog();
				dlg.ShowDialog();
				if (!string.IsNullOrWhiteSpace(dlg.name))
				{
					name = dlg.name;
				}
				StreamWriter f = new StreamWriter(Environment.CurrentDirectory + "\\records.txt", true);
				f.WriteLine("Reversi " + gameShape.MoveCount.ToString() + " " + gameShape.Score.ToString() + " Выигрыш " + name);
				f.Close();
				NewGame();
			}
			Canvas.SetZIndex(oldDeck, 1);
			oldDeck = null;
		}

		void cp_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			cp_MouseLeftButtonDown(sender, e);
		}

		void cp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			CardShape current = sender as CardShape;
			foreach (var d in stack)
			{
				if (d.Deck == current.Card.Deck)
				{
					oldDeck = d;
					break;
				}
			}
			foreach (var d in ground)
			{
				if (d.Deck == current.Card.Deck)
				{
					oldDeck = d;
					break;
				}
			}

			if (current.Card.Deck == dealer.Deck)
			{
				//dealer.Remove(current);
				//cnv.Children.Add(current);
				
				if (current.Card.Visible != true)
				{
					current.Card.Visible = true;
				}

			}
			current.IsEnabled = true;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (gameShape.MoveCount != 0)
			{
				if (System.Windows.MessageBox.Show("Вы уверены?", "Игра не окончена", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					string name = "no_name";
					GetPlayerNameDialog dlg = new GetPlayerNameDialog();
					dlg.ShowDialog();
					if (!string.IsNullOrWhiteSpace(dlg.name))
					{
						name = dlg.name.Replace(" ", "_");
					}
					StreamWriter f = new StreamWriter(Environment.CurrentDirectory + "\\records.txt", true);
					f.WriteLine("Reversi " + gameShape.MoveCount.ToString() + " " + gameShape.Score.ToString() + " Проигрыш " + name);
					f.Close();
					NewGame();
				}
			}
			else
			{
				NewGame();
			}

		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ReloadGame_Click(object sender, RoutedEventArgs e)
		{
			gameShape.ReloadGame();
			TotalScore.Text = gameShape.Score.ToString();
			TotalMoves.Text = gameShape.MoveCount.ToString();
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			gameShape.StepBack();
			TotalScore.Text = gameShape.Score.ToString();
			TotalMoves.Text = gameShape.MoveCount.ToString();
		}

		private void Score_Click(object sender, RoutedEventArgs e)
		{
			Statistic dlg = new Statistic();
			dlg.ShowDialog();
		}

		private void Rules_Click(object sender, RoutedEventArgs e)
		{
			RulesDialog dlg = new RulesDialog();
			dlg.ShowDialog();
		}

		private void AbouteBox_Click(object sender, RoutedEventArgs e)
		{
			About dlg = new About();
			dlg.ShowDialog();
		}

		private void Forward_Click(object sender, RoutedEventArgs e)
		{
			gameShape.StepForward();
			TotalScore.Text = gameShape.Score.ToString();
			TotalMoves.Text = gameShape.MoveCount.ToString();
		}

	}
}
