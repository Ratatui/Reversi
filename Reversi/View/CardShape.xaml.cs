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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View
{
	/// <summary>
	/// Логика взаимодействия для CardShape.xaml
	/// </summary>
	public partial class CardShape : UserControl
	{
		public static double CardWidth = 72;
		public static double CardHeight = 97;
		public const double CardWidthRect = 73;
		public const double CardHeightRect = 98;
		private Point oldMousePos;
		private bool isDrag = false;

		public delegate void CardDragEventHandler(CardShape shape);
		public event CardDragEventHandler CardDragged;
		//public event MouseButtonEventHandler CardMouseLeftButtonDown;

		#region Card

		private Card card = null;
		public Card Card
		{
			get
			{
				return card;
			}
			set
			{
				if (card != null)
				{
					card.VisibleChanged -= new EventHandler(card_VisibleChanged);
					//card.DeckChanged -= new EventHandler(card_DeckChanged);
				}

				card = value;

				//Handle Card Events
				card.VisibleChanged += new EventHandler(card_VisibleChanged);
				// card.DeckChanged += new EventHandler(card_DeckChanged);

				//Adjust the clipping of the cards image to reflect the current card
				double x = 0;
				double y = 0;

				if (Card.Visible)
				{
					//Define the card position in the cards image
					if (Card.Number <= 10)
					{
						x = (Card.Number - 1) % 2;
						y = (Card.Number - 1) / 2;

						switch (Card.Suit)
						{
							case CardSuit.Spades:
								x += 6;
								break;
							case CardSuit.Hearts:
								x += 0;
								break;
							case CardSuit.Diamonds:
								x += 2;
								break;
							case CardSuit.Clubs:
								x += 4;
								break;
						}
					}
					else
					{
						int number = (Card.Number - 11);
						switch (Card.Suit)
						{
							case CardSuit.Spades:
								number += 6;
								break;
							case CardSuit.Hearts:
								number += 9;
								break;
							case CardSuit.Diamonds:
								number += 3;
								break;
							case CardSuit.Clubs:
								number += 0;
								break;
						}

						x = (number % 2) + 8;
						y = number / 2;
					}
				}
				else
				{
					//Show back of the card
					x = 7;
					y = 5;
				}

				((RectangleGeometry)imgCard.Clip).Rect = new Rect(x * CardWidthRect, y * CardHeightRect, CardWidth, CardHeight);
				foreach (Transform tran in ((TransformGroup)imgCard.RenderTransform).Children)
				{
					if (tran.GetType() == typeof(TranslateTransform))
					{
						tran.SetValue(TranslateTransform.XProperty, -x * CardWidthRect);
						tran.SetValue(TranslateTransform.YProperty, -y * CardHeightRect);
					}
				}
				imgCard.RenderTransformOrigin = new Point(0.05 + (x * 0.1), 0.08 + (y * 0.166666));
			}
		}

		public DekShape DeckShape
		{
			get;
			set;
		}
		#endregion


		public CardShape()
		{
			InitializeComponent();
		}

		private void imgCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Card.Visible == false)
			{
				Card.Visible = true;
			}
			else
			{
				if (Card.IsDragable)
				{
					imgCard.CaptureMouse();
					isDrag = true;
					oldMousePos = e.GetPosition(LayoutRoot);
				}
			}
		}

		private void imgCard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			imgCard_MouseLeftButtonDown(sender, e);
		}

		private void imgCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (isDrag)
			{
				imgCard.ReleaseMouseCapture();
				isDrag = false;
				if (CardDragged != null)
				{
					CardDragged(this);
				}
			}
		}

		private void imgCard_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			imgCard_MouseLeftButtonUp(sender, e);
		}

		private void imgCard_MouseMove(object sender, MouseEventArgs e)
		{
			if (isDrag)
			{
				Point newMousePos = e.GetPosition(LayoutRoot);

				double dx = newMousePos.X - oldMousePos.X;
				double dy = newMousePos.Y - oldMousePos.Y;

				Canvas.SetLeft(this, Canvas.GetLeft(this) + dx);
				Canvas.SetTop(this, Canvas.GetTop(this) + dy);
				Canvas.SetZIndex(this, 100);
			}
		}

		private void imgCard_MouseLeave(object sender, MouseEventArgs e)
		{
			if (Card.Enabled)
				rectBorder.Stroke = Brushes.Transparent;
		}
		private void imgCard_MouseEnter(object sender, MouseEventArgs e)
		{
			if (Card.Enabled)
				rectBorder.Stroke = Brushes.Gold;
		}

		private void card_VisibleChanged(object sender, EventArgs e)
		{
			Storyboard s = (Storyboard)this.Resources["aniFlipStartKey"];
			s.Completed += aniFlipStart_Completed;
			s.Begin();
		}
		protected void aniFlipStart_Completed(object sender, EventArgs e)
		{
			this.Card = this.Card;
			Storyboard s = (Storyboard)this.Resources["aniFlipEndKey"];
			s.Begin();
		}
	}
}
