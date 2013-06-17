using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
	public class Card : IComparable<Card>
	{
		#region Rules

		public static bool IsAceBiggest = true;

		#endregion

		#region Properties

		private CardRank rank;
		public CardRank Rank
		{
			get
			{
				return rank;
			}
		}

		private CardSuit suit;
		public CardSuit Suit
		{
			get
			{
				return suit;
			}
			set
			{
				suit = value;
			}
		}

		public CardColor Color
		{
			get
			{
				if ((Suit == CardSuit.Spades) || (Suit == CardSuit.Clubs))
					return CardColor.Black;
				else
					return CardColor.Red;
			}
		}

		public int Number
		{
			get
			{
				return (int)rank;
			}
		}

		public string NumberString
		{
			get
			{
				switch (rank)
				{
					case CardRank.Ace:
						return "A";
					case CardRank.Jack:
						return "J";
					case CardRank.Queen:
						return "Q";
					case CardRank.King:
						return "K";
					default:
						return Number.ToString();
				}
			}
		}

		private Deck deck;
		public Deck Deck
		{
			get
			{
				return deck;
			}
			set
			{
				deck = value;
			}
		}

		private bool visible = true;
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				if (visible != value)
				{
					visible = value;

					if (VisibleChanged != null)
						VisibleChanged(this, null);
				}
			}
		}

		private bool enabled = true;
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		private bool isDragable = true;

		public bool IsDragable
		{
			get { return isDragable; }
			set { isDragable = value; }
		}

		#endregion

		#region Events

		public event EventHandler VisibleChanged;
		public event EventHandler DeckChanged;

		#endregion

		#region Constructors

		//public Card(CardRank rank, CardSuit suit, Deck deck)
		//{
		//    this.rank = rank;
		//    this.suit = suit;
		//    this.deck = deck;
		//    this.deck.Game.Cards.Add(this);
		//}
		public Card(CardRank rank, CardSuit suit)
		{
			this.rank = rank;
			this.suit = suit;
			this.visible = false;
		}

		//public Card(int number, CardSuit suit, Deck deck)
		//{
		//    this.rank = (CardRank)number;
		//    this.suit = suit;
		//    this.deck = deck;
		//    this.deck.Game.Cards.Add(this);
		//}

		#endregion

		#region IComparable<Card> Members

		public int CompareTo(Card other)
		{
			int value1 = this.Number;
			int value2 = other.Number;

			if (Card.IsAceBiggest)
			{
				if (value1 == 1)
					value1 = 14;

				if (value2 == 1)
					value2 = 14;
			}

			if (value1 > value2)
				return 1;
			else if (value1 < value2)
				return -1;
			else
				return 0;
		}

		#endregion

		#region Move Methods

		//public void MoveToFirst()
		//{
		//    MoveToIndex(0);
		//}

		//public void MoveToLast()
		//{
		//    MoveToIndex(Deck.Cards.Count);
		//}

		//public void Shuffle()
		//{
		//    MoveToIndex(Deck.Game.random.Next(0, Deck.Cards.Count));
		//}

		//public void MoveToIndex(int index)
		//{
		//    Deck.Cards.Remove(this);
		//    Deck.Cards.Insert(index, this);
		//}

		#endregion

		#region Generic Methods

		public override string ToString()
		{
			return this.NumberString + " of " + this.Suit.ToString();
		}

		#endregion
	}
}
