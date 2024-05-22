using System;

namespace Combination
{
	public class CombinationModel : IComparable<CombinationModel>
	{
		public int CardWeight { get; private set; }
        public CombinationType CombinationType { get; private set; }
        public int CombinataionRank { get; private set; }

        public CombinationModel(int cardWeight, CombinationType combinationType, int combinataionRank)
		{
			CardWeight = cardWeight;
            CombinationType = combinationType;
			CombinataionRank = combinataionRank;
		}

		public int CompareTo(CombinationModel other)
		{
			int result = CombinationType > other.CombinationType ? 1 : -1;
			if (CombinationType == other.CombinationType)
			{
				int thisRank = CombinataionRank == 0 ? 13 : CombinataionRank; 
				int otherRank = other.CombinataionRank == 0 ? 13 : other.CombinataionRank;   
				result = thisRank > otherRank ? 1 : -1;
				if (thisRank == otherRank)
					result = CardWeight > other.CardWeight ? 1: -1;
			}
			return result;
		}

		public static object Deserialize(byte[] data) => new CombinationModel(data[0], (CombinationType)(int)data[1], data[2]);

		public static byte[] Serialize(object customType)
		{
			var myType = (CombinationModel)customType;
			return new byte[] {(byte)myType.CardWeight, (byte)myType.CombinationType, (byte)myType.CombinataionRank };
		}

		public override string ToString()
		{
			string rank = $"{CombinataionRank + 1}"; 

			if (CombinataionRank == 0)
				rank = "Туз";
			if (CombinataionRank == 12)
				rank = "Король";
			if (CombinataionRank == 11)
				rank = "Дама";  
			if (CombinataionRank == 10)
				rank = "Валет";

			return $"Weight - {CardWeight};  {CombinationType}-{rank}";
		}
	}  
}
		
