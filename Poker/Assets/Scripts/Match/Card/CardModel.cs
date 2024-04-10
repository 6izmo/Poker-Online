using System;

namespace Cards
{
    public class CardModel : IComparable<CardModel>
    {
        public CardSuits Suit { get; private set; }

        public bool IsOpened { get; set; }

        public int Rank { get; private set; }

        public CardModel(int suitIndex, int rankIndex)
        {
            Suit = (CardSuits)suitIndex;
            Rank = rankIndex;
        }

        public int CompareTo(CardModel model)
        {
            int result = this.Rank > model.Rank ? 1 : -1;
            if (Rank == model.Rank)
                return 0;
            return result;
        }

        public static object Deserialize(byte[] data) => new CardModel(data[0], data[1]);

        public static byte[] Serialize(object customType)
        {
            var myType = (CardModel)customType;
            return new byte[] {(byte)myType.Suit, (byte)myType.Rank };
        }

		public override string ToString()
		{
            string rank = $"{Rank + 1}";

            if (Rank == 0)
                rank = "Туз";
            if (Rank == 12)
                rank = "Король";
            if (Rank == 11)
                rank = "Дама";
            if (Rank == 10)
                rank = "Валет";

            return $"{rank}-{Suit}"; 
		}
	}
}
