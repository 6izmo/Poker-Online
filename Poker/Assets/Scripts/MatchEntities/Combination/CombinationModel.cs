using System;

namespace Combination
{
	public class CombinationModel : IComparable<CombinationModel>, IEquatable<CombinationModel>
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

        public bool Equals(CombinationModel other) => CombinataionRank == other.CombinataionRank && CombinationType == other.CombinationType;

        public static object Deserialize(byte[] data) => new CombinationModel(data[0], (CombinationType)(int)data[1], data[2]);

		public static byte[] Serialize(object customType)
		{
			var myType = (CombinationModel)customType;
			return new byte[] {(byte)myType.CardWeight, (byte)myType.CombinationType, (byte)myType.CombinataionRank };
		}
    }  
}
		
