using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View
{
	public class GameState
	{
		public DekShape From;
		public DekShape To;
		public List<CardShape> Cards;
		public int Score;

		public GameState(DekShape from, DekShape to, List<CardShape> cards, int score)
		{
			From = from;
			To = to;
			Cards = cards;
			Score = score;
		}
	}
}
