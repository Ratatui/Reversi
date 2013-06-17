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

		List<DekShape> ground = new List<DekShape>();
		DekShape dealer = new DekShape();
		DekShape stack = new DekShape();

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

			Canvas.SetLeft(stack, 10);
			Canvas.SetTop(stack, space + 100);
			Canvas.SetZIndex(stack, 1);
			cnv.Children.Add(stack);

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
			stack.Clear();

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

			//Добавляем в основную колоду
			for (int i = 0; i < 104; i++)
			{
				CardShape cp = new CardShape();
				cp.Card = deal.TopCard;
				deal.TakeTopCard();
				cp.MouseLeftButtonDown += cp_MouseLeftButtonDown;
				//cp.MouseDoubleClick += cp_MouseDoubleClick;
				dealer.Add(cp);
			}

			// по 1 карты в стопку
			for (int i = 0; i < 3; i++)
			{
				bool visible = true;
				for (int j = 0; j < 7; j++)
				{
					ground[(i * 7) + j].Add(dealer.TopCardShape);
					ground[(i * 7) + j].Deck.TopCard.Visible = visible;
					visible = !visible;
				}
			}

			for (int j = 0; j < ground.Count; j++)
			{
				ground[j].Deck.TopCard.Enabled = true;
				ground[j].Deck.TopCard.IsDragable = false;
			}

			dealer.Deck.MakeAllCardsDragable(false);
			dealer.Deck.TopCard.Visible = false;
			dealer.Deck.TopCard.IsDragable = false;
			Canvas.SetZIndex(dealer, 0);
			timer.Start();
		}

		private bool IsWon()
		{
			if (dealer.Count > 0) return false;

			foreach (var dg in ground)
			{
				if (dg.Count > 0) return false;
			}
			return true;
		}

		private void MakeMove(DekShape From, DekShape To, bool calcScore = false)
		{
			gameShape.MakeMove(From, To, calcScore);
			stack.Deck.TopCard.Visible = true;
			TotalScore.Text = gameShape.Score.ToString();
			TotalMoves.Text = gameShape.MoveCount.ToString();
		}

		void cp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			CardShape current = sender as CardShape;

			if (current.DeckShape == dealer)
			{
				gameShape.MoveCount++;
				foreach (var d in ground)
				{
					if (d.Count > 0 && d.Deck.TopCard.Visible && dealer.Count > 0)
					{
						var card = dealer.TopCardShape;
						dealer.Remove(card);
						d.Add(card);
						gameShape.Moves.Add(new GameState(dealer, d, new List<CardShape>() { card }, gameShape.MoveCount));
						d.Deck.TopCard.Visible = true;
					}
				}
				oldDeck = null;
			}

			if (oldDeck != null)
			{
				foreach (var d in ground)
				{
					if (d.Deck == current.Card.Deck)
					{
						if (current.DeckShape.Deck.TopCard.Number == oldDeck.Deck.TopCard.Number)
						{
							var firstDeck = ground.IndexOf(current.DeckShape);
							var secondDeck = ground.IndexOf(oldDeck);
							if (Math.Abs(secondDeck - firstDeck) == 2)
							{
								gameShape.MoveCount++;
								if (firstDeck > secondDeck)
								{
									MakeMove(d, stack, true);
									MakeMove(ground[firstDeck - 1], stack, true);
								}
								else
								{
									MakeMove(oldDeck, stack, true);
									MakeMove(ground[secondDeck - 1], stack, true);
								}

								if (firstDeck % 7 == 0 || secondDeck % 7 == 0)
								{
									for (int i = 0; i < 3; i++)
									{
										int k = 0;
										for (int j = 0; j < 7; j++)
										{
											if (ground[(i * 7) + j].Count > 0)
												k++;
										}
										if (k == 1)
										{
											for (int j = 0; j < 7; j++)
											{
												MakeMove(ground[(i * 7) + j], stack, true);
											}
										}
									}
								}

								for (int i = 0; i < 3; i++)
									for (int j = 0; j < 7; j++)
									{
										if (ground[(i * 7) + j].Count == 0)
										{
											for (int k = j; k < 6; k++)
												MakeMove(ground[(i * 7) + k + 1], ground[(i * 7) + k]);
										}
									}
								for (int i = 0; i < 3; i++)
									for (int j = 0; j < 7; j++)
									{
										if (ground[(i * 7) + j].Count == 0)
										{
											for (int k = j; k < 6; k++)
												MakeMove(ground[(i * 7) + k + 1], ground[(i * 7) + k]);
										}
									}
							}
						}
						break;
					}
				}
				oldDeck = null;
			}
			else
			{
				foreach (var d in ground)
				{
					if (d.Deck == current.Card.Deck)
					{
						oldDeck = d;
						break;
					}
				}
			}
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
