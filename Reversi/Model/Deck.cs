using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
	public class Deck
	{
		List<Card> Cards = new List<Card>();
		public Card TopCard
		{
			get
			{
				if (Cards.Count > 0)
				{
					return Cards[Cards.Count - 1];
				}
				else
					return null;
			}
			set
			{
				if (value != null)
				{
					Cards.Add(value);
					if (Cards.Count > 1)
					{
						Cards[Cards.Count - 2].IsDragable = false;
						Cards[Cards.Count - 2].Enabled = false;

					}
					Cards[Cards.Count - 1].IsDragable = true;
					Cards[Cards.Count - 1].Enabled = true;

				}
			}
		}
		public Deck()
		{

		}

		public Card TakeTopCard()
		{
			if (Cards.Count > 0)
			{
				Cards.RemoveAt(Cards.Count - 1);
				Card returned = TopCard;
				if (returned != null)
				{
					Cards[Cards.Count - 1].IsDragable = true;
					Cards[Cards.Count - 1].Enabled = true;
				}
				return returned;
			}
			return null;
		}

		public void Shuffle(int times)
		{
			for (int time = 0; time < times; time++)
			{
				for (int i = 0; i < Cards.Count; i++)
				{
					Random r = new Random();
					int n = r.Next(Cards.Count - i);
					var tmp = Cards[n];
					Cards[n] = Cards[i];
					Cards[i] = tmp;
				}
			}
		}
		public void EnableAllCards(bool enable)
		{
			for (int i = 0; i < Cards.Count; i++)
			{
				Cards[i].Enabled = enable;
			}
		}

		public void MakeAllCardsDragable(bool isDragable)
		{
			for (int i = 0; i < Cards.Count; i++)
			{
				Cards[i].IsDragable = isDragable;
			}
		}

		public void FlipAllCards(bool IsSuit)
		{
			for (int i = 0; i < Cards.Count; i++)
			{
				Cards[i].Visible = IsSuit;
			}
		}

		public void Clear()
		{
			Cards.Clear();
		}


	}
}
