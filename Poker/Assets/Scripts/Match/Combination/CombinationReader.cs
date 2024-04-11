using Cards;
using Players;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Combination
{
    public class CombinationReader
    {
        public Task<CombinationModel> GetCombination(PlayerModel playerModel, List<CardModel> tableCards)
        {
            tableCards.Add(playerModel.Cards[0].CardModel);
            tableCards.Add(playerModel.Cards[1].CardModel);
            tableCards.Sort();

            List<CombinationModel> combinations = new();
            int highCardIndex = tableCards.Any(x => x.Rank == 0) ? 0 : tableCards.Count - 1;

            CombinationModel combination = new(CombinationType.HighCard, tableCards[highCardIndex].Rank);
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
                else if (card.Rank != lastCard.Rank)
                    countCardsForStraight = 1;

                if (countCardsForStraight >= 5)
                {
                    CombinationType combinationType = countOneSuitForStraight >= 5 ? CombinationType.StraightFlush : CombinationType.Straight;
                    combinations.Add(new(combinationType, lastCard.Rank));
                }

                lastCard = card;
            }

            combinations.Sort();
            CombinationModel result = combinations[combinations.Count - 1];
            foreach (CombinationModel combo in combinations)
            {
                int repeatCount = combinations.Where(x => x.CombinationType == combo.CombinationType).Count();
                if (repeatCount == 2)
                {
                    CombinationModel secondPair = combinations.FindLast(x => x.CombinationType == combo.CombinationType);
                    result = new CombinationModel(CombinationType.TwoPair, secondPair.CombinataionRank + combo.CombinataionRank);
                }
                if (combo.CombinationType == CombinationType.ThreeOfAKind && combinations.Any(x => x.CombinationType == CombinationType.Pair))
                {
                    CombinationModel pair = combinations.FindLast(x => x.CombinationType == CombinationType.Pair);
                    result = new CombinationModel(CombinationType.FullHouse, combo.CombinataionRank + pair.CombinataionRank);
                }
            }

            return Task.FromResult(result);
        }
    }
}
