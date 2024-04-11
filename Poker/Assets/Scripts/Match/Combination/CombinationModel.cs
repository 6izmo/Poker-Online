using System;

namespace Combination
{
	public class CombinationModel : IComparable<CombinationModel>
	{
		public CombinationType CombinationType;
		public int CombinataionRank;

		public CombinationModel(CombinationType combinationType, int combinataionRank)
		{
			CombinationType = combinationType;
			CombinataionRank = combinataionRank;
		}

		public int CompareTo(CombinationModel other)
		{
			int result = CombinationType > other.CombinationType ? 1 : -1;
			if (CombinationType == other.CombinationType)
			{
				int thisRank = CombinataionRank == 0 ? 13 : CombinataionRank;
				int otherRank = other.CombinataionRank == 0 ? 13 : CombinataionRank;
				result = thisRank > otherRank ? 1 : -1;
				if (thisRank == otherRank)
					result = 0;
			}
			return result;
		}

		public static object Deserialize(byte[] data) => new CombinationModel((CombinationType)(int)data[0], data[1]);

		public static byte[] Serialize(object customType)
		{
			var myType = (CombinationModel)customType;
			return new byte[] { (byte)myType.CombinationType, (byte)myType.CombinataionRank };
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

			return $"{CombinationType}-{rank}";
		}
	}
}
		
