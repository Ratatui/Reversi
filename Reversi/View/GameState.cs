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
		public bool NeedFlipBack;
		public int Score;

		public GameState(DekShape from, DekShape to, bool needflipback, int score)
		{
			From = from;
			To = to;
			NeedFlipBack = needflipback;
			Score = score;
		}
	}
}
