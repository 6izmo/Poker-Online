using Cards;
using System;
using Players;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CombinationType = CombinationReader.CombinationType;

public class CombinationReader
{
    public enum CombinationType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
		StraightFlush,
		RoaylFlush
	}

    public Task<Combination> GetCombination(PlayerModel playerModel,  List<CardModel> tableCards)
    {
        tableCards.Add(playerModel.Cards[0].CardModel);
        tableCards.Add(playerModel.Cards[1].CardModel);
        tableCards.Sort();

        List<Combination> combinations = new();
        int highCardIndex = tableCards.Any(x => x.Rank == 0) ? 0 : tableCards.Count - 1;

		Combination combination = new(CombinationType.HighCard, tableCards[highCardIndex].Rank);
        combinations.Add(combination);

        CardModel lastCard = tableCards[0];
        int countCardsForStraight = 1;
        int countOneSuitForFlush = 1;
        int countOneSuitForStraight = 1;

        foreach (CardModel card in tableCards)
        {
            int repeatCount = tableCards.Where(x => x.Rank == card.Rank).Count();
            countOneSuitForFlush = tableCards.Where(x => x.Suit == card.Suit).Count();

            if (repeatCount == 2 && !combinations.Any(x => x.CombinataionRank == card.Rank && x.CombinationType == CombinationType.Pair))
                combinations.Add(new(CombinationType.Pair, card.Rank));

            if (repeatCount == 3 && !combinations.Any(x => x.CombinataionRank == card.Rank && x.CombinationType == CombinationType.ThreeOfAKind))
                combinations.Add(new(CombinationType.ThreeOfAKind, card.Rank));

            if (repeatCount == 4 && !combinations.Any(x => x.CombinataionRank == card.Rank && x.CombinationType == CombinationType.FourOfAKind))
                combinations.Add(new(CombinationType.FourOfAKind, card.Rank));

            if (countOneSuitForFlush >= 5)
                combinations.Add(new(CombinationType.Flush, card.Rank));

            if (card.Rank - lastCard.Rank == 1 || card.Rank - tableCards[0].Rank == 12)
            {
                if (card.Suit == lastCard.Suit)
                    countOneSuitForStraight++;
                countCardsForStraight++;
                lastCard = card;
            }
            else if(card.Rank != lastCard.Rank)
                countCardsForStraight = 1;

            if(countCardsForStraight >= 5)
            {
                CombinationType combinationType = countOneSuitForStraight >= 5 ? CombinationType.StraightFlush : CombinationType.Straight;
                combinations.Add(new(combinationType, lastCard.Rank));
            }

            lastCard = card;
        }

        combinations.Sort();
        Combination result = combinations[combinations.Count - 1];
        foreach (Combination combo in combinations)
        {
            int repeatCount = combinations.Where(x => x.CombinationType == combo.CombinationType).Count();
            if (repeatCount == 2)
            {
                Combination secondPair = combinations.FindLast(x => x.CombinationType == combo.CombinationType);
                result = new Combination(CombinationType.TwoPair, secondPair.CombinataionRank + combo.CombinataionRank);
            }
            if(combo.CombinationType == CombinationType.ThreeOfAKind && combinations.Any(x => x.CombinationType == CombinationType.Pair))
            {
                Combination pair = combinations.FindLast(x => x.CombinationType == CombinationType.Pair);
                result = new Combination(CombinationType.FullHouse, combo.CombinataionRank + pair.CombinataionRank);
            }
        }

        return Task.FromResult(result);
	}
}

public class Combination : IComparable<Combination>
{
    public CombinationType CombinationType;
    public int CombinataionRank;

    public Combination(CombinationType combinationType, int combinataionRank)
    {
        CombinationType = combinationType;
		CombinataionRank = combinataionRank;
	}

	public int CompareTo(Combination other)
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

    public static object Deserialize(byte[] data) => new Combination((CombinationType)(int)data[0], data[1]);

    public static byte[] Serialize(object customType)
    {
        var myType = (Combination)customType;
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
