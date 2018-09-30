using System;
using System.Collections.Generic;
namespace Blackjack
{
    enum CardValue
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,

        // anything greater than or equal to 12, assume its value is really 10.
        King = 12,
        Queen = 13,
        Jack = 14,

        Ace = -1, // a placeholder for Ace1 or Ace2 since the player can freely choose between 1 and 11.
        Ace1 = 1,
        Ace2 = 11
    }

    enum CardSuit
    {
        Spades = 1,
        Hearts = 2,
        Diamonds = 3,
        Clubs = 4
    }

    class Card
    {
        public CardValue Value;
        public CardSuit Suit;
        public bool Shown = true;

        public static Card RandomCard(Deck deck, bool Visibility = true)
        {
            Random rand = new Random();
            Card chosenCard = deck.Cards[rand.Next(deck.Cards.Count)];
            chosenCard.Shown = Visibility;

            return chosenCard;
        }
    }

    class Deck
    {
        private readonly Random rand = new Random(); // used for shuffling
        public static CardValue[] Set = new CardValue[] {
            CardValue.Two,
            CardValue.Three,
            CardValue.Four,
            CardValue.Five,
            CardValue.Six,
            CardValue.Seven,
            CardValue.Eight,
            CardValue.Nine,
            CardValue.Ten,

            CardValue.King,
            CardValue.Queen,
            CardValue.Jack,

            CardValue.Ace
        };

        public List<Card> Cards = new List<Card>();

        public void Shuffle()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int chosen = rand.Next(i);

                Card tempCard = Cards[chosen];

                Cards[chosen] = Cards[i];
                Cards[i] = tempCard;
            }
        }

        public Deck()
        {
            // need to preload the deck with 52 cards (13 cards of each suit).

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit))) {
                foreach (CardValue itemValue in Set)
                {
                    // 13 because each suit has 13 cards.

                    Card item = new Card
                    {
                        Suit = suit,
                        Value = itemValue
                    };

                    Cards.Add(item);
                }
            }
        }
    }

    class Player
    {
        public List<Card> Hand = new List<Card>();
        public bool Dealer = false;

        private Deck deck;

        public int Total
        {
            get
            {
                int count = 0;
                foreach (Card item in Hand)
                {
                    int value = (int)item.Value;
                    if (value > 11)
                    {
                        value = 10;
                    }

                    count = count + value;
                }
                
                return count;
            }
        }
        public bool Busted
        {
            get
            {
                return (Total > 21);
            }
        }

        public Card Hit(bool show = true)
        {
            Random rand = new Random();
            Card card = Card.RandomCard(deck, show);
            if (card.Value == CardValue.Ace && Dealer) // choose for the CPU.
            {
                if (rand.Next(0, 100) % 2 == 0)
                {
                    card.Value = CardValue.Ace1;
                }
                else
                {
                    card.Value = CardValue.Ace2;
                }
            }

            Hand.Add(card);
            return card;
        }

        public void ShowAll()
        {
            foreach (Card item in Hand)
            {
                item.Shown = true;
            }
        }

        public Player(Deck playerDeck, bool isDealer = false)
        {
            this.deck = playerDeck;
            this.Dealer = isDealer;
        }
    }

    class Program
    {
        public static Player DealerPlr;
        public static Player Plr;

        static void ShowGamefield()
        {
            Console.Clear();

            Console.WriteLine("DEALER CARDS: ");
            foreach (Card card in DealerPlr.Hand)
            {
                if (card.Shown)
                {
                    int val = (int)card.Value;
                    if (val > 11)
                    {
                        val = 10;
                    }

                    if (card.Value == CardValue.King || card.Value == CardValue.Queen || card.Value == CardValue.Jack)
                    {
                        Console.Write(string.Format("[{0} ({1})] ", card.Value.ToString(), val.ToString()));
                    }
                    else if (card.Value == CardValue.Ace1 || card.Value == CardValue.Ace2)
                    {
                        Console.Write(string.Format("[Ace ({0})] ", val.ToString()));
                    }
                    else if (card.Value == CardValue.Ace)
                    {
                        Console.WriteLine("[Ace]");
                    }
                    else
                    {
                        Console.Write(string.Format("[{0}] ", val.ToString()));
                    }
                } else
                {
                    Console.Write("[X] ");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("YOUR CARDS: ");
            foreach (Card card in Plr.Hand)
            {
                int val = (int)card.Value;
                if (val >= 12)
                {
                    val = 10;
                }

                if (card.Value == CardValue.King || card.Value == CardValue.Queen || card.Value == CardValue.Jack)
                {
                    Console.Write(string.Format("[{0} ({1})] ", card.Value.ToString(), val.ToString()));
                }
                else if (card.Value == CardValue.Ace1 || card.Value == CardValue.Ace2)
                {
                    Console.Write(string.Format("[Ace ({0})] ", val.ToString()));
                }
                else if (card.Value == CardValue.Ace)
                {
                    Console.WriteLine("[Ace]");
                }
                else
                {
                    Console.Write(string.Format("[{0}] ", val.ToString()));
                }
            }

            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Deck deck = new Deck();
            deck.Shuffle();

        Start:
            DealerPlr = new Player(deck, true);
            Plr = new Player(deck);

            Console.Clear(); // Just in case.
            Console.WriteLine("Welcome to Blackjack (aka 21).");
            Console.WriteLine("SPACE: Play");
            Console.WriteLine("B: Instructions");

            ConsoleKeyInfo keyPressed = Console.ReadKey();
            if (keyPressed.Key == ConsoleKey.Spacebar)
            {
                DealerPlr.Hit(false);
                Card initCard = Plr.Hit();

                if (initCard.Value == CardValue.Ace)
                {
                    ShowGamefield();
                    Console.WriteLine("You got an Ace card! Do you want it to be a value of 1 or 11?");
                aceCheck:
                    string aceResp = Console.ReadLine();
                    if (aceResp == "1")
                    {
                        initCard.Value = CardValue.Ace1;
                    }
                    else if (aceResp == "11")
                    {
                        initCard.Value = CardValue.Ace2;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Please choose the correct option.");
                        Console.WriteLine();
                        goto aceCheck;
                    }
                }

                ShowGamefield();
                Console.WriteLine();
                Console.WriteLine();
            give:
                Console.WriteLine("Do you want to HIT?");
                Console.WriteLine("\t(y) / (n) ?");
            input1:
                string res1 = Console.ReadLine().ToLower();
                if (res1 == "(y)")
                {
                    Card chosenCard = Plr.Hit();
                    if (chosenCard.Value == CardValue.Ace)
                    {
                        Console.WriteLine("You got an Ace card! Do you want it to be a value of 1 or 11?");
                    aceCheck1:
                        string aceResp1 = Console.ReadLine();
                        if (aceResp1 == "1")
                        {
                            chosenCard.Value = CardValue.Ace1;
                        }
                        else if (aceResp1 == "11")
                        {
                            chosenCard.Value = CardValue.Ace2;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Please choose the correct option.");
                            Console.WriteLine();
                            goto aceCheck1;
                        }
                    }

                    if (Plr.Busted)
                    {
                        goto results;
                    }

                    ShowGamefield();
                }
                else if (res1 == "(n)")
                {
                    goto results;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Please choose the correct option.");
                    Console.WriteLine();
                    goto input1;
                }

                goto give;
            results:
                if (Plr.Busted)
                {
                    ShowGamefield();
                    Console.WriteLine("You busted!");
                }
                else
                {
            dealer:
                    if (DealerPlr.Total < 17)
                    {
                        // Must deal until it's above 17.
                        DealerPlr.Hit();
                        if (DealerPlr.Busted)
                        {
                            DealerPlr.ShowAll();
                            ShowGamefield();
                            Console.WriteLine("The dealer busted!");
                            Console.WriteLine("You won!");
                        }
                        else
                        {
                            ShowGamefield();
                            Console.WriteLine("The dealer has hit.");
                            Console.WriteLine("Press any key to continue.");

                            Console.ReadKey();
                            goto dealer;
                        }
                    } else
                    {
                        DealerPlr.ShowAll();
                        ShowGamefield();

                        if (Plr.Total > DealerPlr.Total)
                        {
                            // Player wins!
                            Console.WriteLine("You won!");
                        }
                        else if (Plr.Total == DealerPlr.Total)
                        {
                            // TIE
                            Console.WriteLine("It was a tie!");
                        }
                        else
                        {
                            // Player loses. :(
                            Console.WriteLine("You lost!");
                        }
                    }
                }

                Console.WriteLine("Press any key to go back to the menu.");
                Console.ReadKey();
                goto Start;


            } else if (keyPressed.Key == ConsoleKey.B)
            {
                Console.Clear();
                Console.WriteLine("The game is simple!");
                Console.WriteLine("The objective of the game is to beat the dealer (CPU) by obtaining cards whose overall values are close to 21 as possible, without actually going over 21.");
                Console.WriteLine();
                Console.WriteLine("Each card has a value. An ace card is worth a 1 or 11, depending on the player. Face cards (King, Queen, Jack) are worth 10 and any other card is worth its pip value.");
                Console.WriteLine();
                Console.WriteLine("Once the game starts, the cards in the deck are shuffled.");
                Console.WriteLine("The dealer gives one card face up to the player, then one card face up to himself.");
                Console.WriteLine("The next round, another round of cards are dealt to the player. The next card dealt to the dealer will be faced down.");
                Console.WriteLine("The player has the ability to stand depending on the situation. For example, if the player is close to 21 and is afraid of busting, he or she will stand, otherwise hit.");
                Console.WriteLine("When the player is done, the dealer will reveal his cards. If his total card value is below 17, he must hit until >= 17.");
                Console.WriteLine("If the player or dealer goes over 21, he or she busts, thus losing.");
                Console.WriteLine("However if neither busted, the winner of the game will be the one that is most closest to 21.");

                Console.WriteLine();
                Console.WriteLine("Press any key to go back to the menu.");
                Console.ReadKey();
                goto Start;
            } else
            {
                goto Start;
            }
        }
    }
}
