using System.Collections.Generic;

namespace Cards
{ 
    public class CardDeck 
    {
        private List<CardModel> _cards = new(52);
        private System.Random _randomGenerator = new();

        public CardDeck()   
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    CardModel cardModel = new CardModel(i,j);
                    _cards.Add(cardModel);
                }
            }
        }

        public CardModel GetRandomCard()
        {
            int randomIndex = _randomGenerator.Next(_cards.Count);
            CardModel cardModel = _cards[randomIndex];
            _cards.Remove(cardModel);
            return cardModel;
        }

    }
    public enum CardSuits
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }
}
