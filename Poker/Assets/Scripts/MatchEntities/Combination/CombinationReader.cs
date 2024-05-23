using Cards;
using Players;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Combination
{
    public static class CombinationReader
    {
        public static Task<CombinationModel> GetCombination(PlayerModel playerModel, List<CardModel> tableCards)
        {
            CardModel firstCard = playerModel.Cards[0].CardModel;
            CardModel secondCard = playerModel.Cards[1].CardModel;
            int firstWeight = firstCard.Rank == 0 ? 13 : firstCard.Rank; 
            int secondWeight = secondCard.Rank == 0 ? 13 : secondCard.Rank;
            int playerCardWeight = firstWeight + secondWeight;

            tableCards.Add(firstCard);
            tableCards.Add(secondCard);
            tableCards.Sort();

            List <CombinationModel> combinations = new();
            int highCardIndex = tableCards.Any(x => x.Rank == 0) ? 0 : tableCards.Count - 1;

            CombinationModel combination = new(playerCardWeight, CombinationType.HighCard, tableCards[highCardIndex].Rank);
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
                    combinations.Add(new(playerCardWeight,CombinationType.Pair, card.Rank));

                if (repeatCount == 3 && !combinations.Any(x => x.CombinataionRank == card.Rank && x.CombinationType == CombinationType.ThreeOfAKind))
                    combinations.Add(new(playerCardWeight,CombinationType.ThreeOfAKind, card.Rank));

                if (repeatCount == 4 && !combinations.Any(x => x.CombinataionRank == card.Rank && x.CombinationType == CombinationType.FourOfAKind))
                    combinations.Add(new(playerCardWeight,CombinationType.FourOfAKind, card.Rank));  

                if (countOneSuitForFlush >= 5)
                    combinations.Add(new(playerCardWeight,CombinationType.Flush, card.Rank));

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
                    if(combinationType == CombinationType.StraightFlush && lastCard.Rank == 0)
                        combinations.Add(new(playerCardWeight, CombinationType.RoaylFlush, lastCard.Rank));
                    else
                        combinations.Add(new(playerCardWeight, combinationType, lastCard.Rank));
                }

                lastCard = card;
            }

            combinations.Sort();
            CombinationModel result = combinations[combinations.Count - 1];
            foreach (CombinationModel combo in combinations)
            {
                int repeatCount = combinations.Where(x => x.CombinationType == combo.CombinationType).Count();
                if (repeatCount >= 2)   
                {
                    CombinationModel secondPair = combinations.FindLast(x => x.CombinationType == combo.CombinationType);
                    result = new CombinationModel(playerCardWeight,CombinationType.TwoPair, secondPair.CombinataionRank + combo.CombinataionRank);
                }
                if (combo.CombinationType == CombinationType.ThreeOfAKind && combinations.Any(x => x.CombinationType == CombinationType.Pair))
                {
                    CombinationModel pair = combinations.FindLast(x => x.CombinationType == CombinationType.Pair);
                    result = new CombinationModel(playerCardWeight,CombinationType.FullHouse, combo.CombinataionRank + pair.CombinataionRank);
                }
            }

            return Task.FromResult(result);
        }
    }
}
