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
	/// Логика взаимодействия для DekShape.xaml
	/// </summary>
	public partial class DekShape : UserControl
	{
		public List<CardShape> cards = new List<CardShape>();
		public Deck Deck = new Deck();
		public double CardSpace = 0.1;

		public CardShape TopCardShape
		{
			get
			{
				if (cards.Count > 0)
				{
					CardShape rt = cards[cards.Count - 1];
					Remove(rt);
					return rt;
				}
				return null;
			}
		}
		public int Count
		{
			get { return cards.Count; }
		}
		public DekShape()
		{
			InitializeComponent();
		}

		public void Add(CardShape card)
		{
			cards.Add(card);
			card.DeckShape = this;
			Canvas.SetTop(card, CardSpace * 0);
			Canvas.SetLeft(card, 0);
			Canvas.SetZIndex(card, cards.Count + 1);
			LayoutRoot.Children.Add(card);
			Deck.TopCard = card.Card;
			card.Card.Deck = this.Deck;
		}

		public void Remove(CardShape card)
		{
			cards.Remove(card);
			LayoutRoot.Children.Remove(card);
			Deck.TakeTopCard();
		}

		public void Clear()
		{
			Deck.Clear();
			while (cards.Count != 0)
			{
				LayoutRoot.Children.Remove(cards[0]);
				cards.RemoveAt(0);
			}
		}

		private void rectBorderBack_MouseEnter(object sender, MouseEventArgs e)
		{
			rectBorder.Visibility = Visibility.Visible;
		}

		private void rectBorderBack_MouseLeave(object sender, MouseEventArgs e)
		{
			rectBorder.Visibility = Visibility.Collapsed;
		}
	}
}
