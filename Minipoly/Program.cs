// Ben Martin
// Mini-opoly

using System;
using System.Collections.Generic;

namespace Minipoly
{
    class Property
    {
        public string name;
        public int price, rent, housePrice, position, numhouses;
        public string owner, color;

        public Property(string str, int x, int y, int z, int pos, string col)   // Constructor gets name, price, rent, house price, and position of properties. 
        {
            name = str;
            price = x;
            rent = y;
            housePrice = z;
            position = pos;
            owner = "Unowned";
            color = col;
            numhouses = 0;
        }
        
        public void buildHouse(Player player)
        {
            if (numhouses == 5)     // Will not let player build more than 4 houses and a hotel
                Console.WriteLine("You have already built 4 houses and a Hotel here, you cannot build any more.");
            else if (player.money < housePrice)     // Will not let player buy houses if they don't have enough money
                Console.WriteLine("You don't have enough money.");
            else
            {
                player.setMoney(-housePrice);       // Player pays for the house and updates the rent depending on which house they buy
                switch(numhouses)               // I had to assess for myself what would be a balanced amount for rent increases. Normal Monopoly has these values a bit higher, but I scaled them down for Minipoly
                {
                    case 0: rent *= 3; player.houses++;  break;       // First House - Rent increases by 3x
                    case 1: rent *= 2; player.houses++; break;       // Second House - Rent increases by 2x
                    case 2: rent *= 2; player.houses++; break;       // Third House - Rent increases by 2x
                    case 3: rent = rent * 3 / 2; player.houses++; break;   // Fourth House - Rent increases by 1.5x
                    case 4: rent *= 2; player.hotels++; break;       // Hotel - Rent increases by 2x
                    default: break;
                }
                numhouses++;
                if (numhouses == 5)
                {
                    Console.WriteLine("You built a hotel on {0}! Rent is now ${1}.", name, rent);
                    name += " *";
                }
                else
                {
                    Console.WriteLine("You built a house on {0}! Rent is now ${1}.", name, rent);
                    name += " +";
                }
            }
        }
    }

    class Player
    {
        public string name;
        public int money, position, jailcounter, houses, hotels;
        public bool injail, outofjailfree, canbuyhouses;
        public int[] monopCounter = { 0, 0, 0, 0 };
        public bool[] monopOwned = { false, false, false, false };
        public List<Property> propsOwned = new List<Property>();

        public Player(string x, int y)   // Constructor that receives name and starting money. Sets initial position to 0
        {
            name = x;
            money = y;
            position = 0;
            jailcounter = 0;
            houses = 0;
            hotels = 0;
            injail = false;
            outofjailfree = false;
            canbuyhouses = false;
        }

        public void passGo()            // Prints out that player passed Go, adds $200 to money
        {
            money += 200;
            Console.WriteLine("{0} passed Go! {0} now has ${1}.", name, money);
        }

        public void setMoney(int x)     // Updates player's money and prints out their new amount
        {
            money += x;
            Console.WriteLine("{0} now has ${1}.", name, money);
        }   

        public void setPosition(int x)      // Updates position and wraps around when passing Go
        {
            int prevposition = position;
            position = (position + x) % 19;
            if (prevposition > position)
            {
                passGo();
            }
        }      

        public void goTo(int x)             // Sends player to a specific position while still checking to see if they pass go
        {
            int prevposition = position;
            position = x;
            if (prevposition > position)
            {
                passGo();
            }
        }

        public void goToJail()              // Sends a player to Jail and does not check if they pass go
        {
            position = 5;
            injail = true;

            if (outofjailfree)              // Get out of jail free card is consumed automatically and player is placed in just visiting
            {
                injail = false;
                outofjailfree = false;
                Console.WriteLine("{0} used their Get Out of Jail Free card and is no longer in Jail.", name);
            }
        }

        public void buyProp(Property prop)  // Buys the property player landed on
        {
            Console.WriteLine("{0} now owns {1}!", name, prop.name);
            prop.owner = name;
            setMoney(-prop.price);
            propsOwned.Add(prop);
            switch (prop.color)
            {
                case "blue":
                    monopCounter[0]++;
                    if (monopCounter[0] == 3) monopOwned[0] = true;
                    break;
                case "yellow":
                    monopCounter[1]++;
                    if (monopCounter[1] == 3) monopOwned[1] = true;
                    break;
                case "red":
                    monopCounter[2]++;
                    if (monopCounter[2] == 3) monopOwned[2] = true;
                    break;
                case "green":
                    monopCounter[3]++;
                    if (monopCounter[3] == 3) monopOwned[3] = true;
                    break;
                default: break;
            }
        }
    }



    class Program
    {
        // Money players start the game with
        const int STARTMONEY = 1000;

        // All tile names
        public static String[] tile = { "Go!", "Oriental Avenue", "Vermont Avenue",  "Chance",  "Connecticut Avenue",
            "Jail",  "St. James Place",  "Tennessee Avenue",  "Community Chest",  "New York Avenue",  "Free Parking",
            "Atlantic Avenue",  "Ventnor Avenue",  "Chance",  "Marvin Gardens",  "Go To Jail",  "Pacific Avenue",
            "North Carolina Avenue", "Community Chest",  "Pennsylvania Avenue"};

        public static int freeparking = 0;

        // Properties
        public static Property oriental = new Property(tile[1], 100, 6, 50, 1, "blue");
        public static Property vermont = new Property(tile[2], 100, 6, 50, 2, "blue");
        public static Property connecticut = new Property(tile[4], 120, 8, 50, 4, "blue");
        public static Property stjames = new Property(tile[6], 180, 14, 100, 6, "yellow");
        public static Property tennessee = new Property(tile[7], 180, 14, 100, 7, "yellow");
        public static Property newyork = new Property(tile[9], 200, 16, 100, 9, "yellow");
        public static Property atlantic = new Property(tile[11], 260, 22, 150, 11, "red");
        public static Property ventnor = new Property(tile[12], 260, 22, 150, 12, "red");
        public static Property marvin = new Property(tile[14], 280, 24, 150, 14, "red");
        public static Property pacific = new Property(tile[16], 340, 30, 200, 16, "green");
        public static Property carolina = new Property(tile[17], 340, 30, 200, 17, "green");
        public static Property pennsylvania = new Property(tile[19], 360, 32, 200, 18, "green");

        public static Property[,] Monopoly = new Property[4, 3];        // 2D array of Properties keeps track of monopolies
        
        // Players 1 and 2, initialized as p1 and p2 with the constant STARTMONEY as their initial money
        public static Player p1 = new Player("p1", STARTMONEY);
        public static Player p2 = new Player("p2", STARTMONEY);

        static void Main(string[] args)
        {
            Monopoly[0, 0] = oriental; Monopoly[0, 1] = vermont; Monopoly[0, 2] = connecticut;      // Blue Properties
            Monopoly[1, 0] = stjames; Monopoly[1, 1] = tennessee; Monopoly[1, 2] = newyork;         // Yellow Properties
            Monopoly[2, 0] = atlantic; Monopoly[2, 1] = ventnor; Monopoly[2, 2] = marvin;           // Red Properties
            Monopoly[3, 0] = pacific; Monopoly[3, 1] = carolina; Monopoly[3, 2] = pennsylvania;     // Green Properties

            Console.WriteLine("Welcome to Mini-opoly!");
            Console.Write("Please enter Player 1 name: ");
            p1.name = Console.ReadLine();
            Console.WriteLine("{0} has ${1}", p1.name, p1.money);
            Console.Write("Please enter Player 2 name: ");
            p2.name = Console.ReadLine();
            Console.WriteLine("{0} has ${1}", p2.name, p2.money);
            Console.WriteLine("Press Enter to start the game!");
            Console.ReadKey();

            play(p1, p2);           // Calls the play function and starts the game
            Console.ReadKey();
        }

        static void play(Player p1, Player p2)
        {
            bool gameover = false;

            while (!gameover)       // Game will keep playing until one player is bankrupt
            {
                Console.WriteLine();
                roll(p1, p2);           // Calls roll function
                if (p1.money <= 0)
                {
                    Console.WriteLine("{0} has gone bankrupt. {1} wins!", p1.name, p2.name);
                    gameover = true;
                    continue;
                }

                Console.WriteLine();
                roll(p2, p1);           // Calls roll function
                if (p2.money <= 0)
                {
                    Console.WriteLine("{0} has gone bankrupt. {1} wins!", p2.name, p1.name);
                    gameover = true;
                }
            }
        }

        static void roll(Player p1, Player p2)
        {
            if (p1.injail)      // Checks to see if the player is in jail. If they are, jailroll overwrites this function
                jailroll(p1, p2);
            else
            {
                char choice = ' ';
                while (choice != 'r' && choice != 'R')
                { 
                    Console.Write("{0}: Roll | Inventory | Buy Houses | Trade | (r/i/h/t) ", p1.name);
                    choice = Convert.ToChar(Console.ReadLine());
                    switch (choice)
                    {
                        case 'r':           // Escapes to normal roll function
                        case 'R': break;
                        case 'h':           // Buys a house on a property
                        case 'H': buyHouse(p1); break;
                        case 'i':
                        case 'I': listProps(p1); break;
                        case 't':
                        case 'T': tradeProp(p1, p2); break;
                        case 'a': listAllProps(); break;
                        default: Console.WriteLine("Error, invalid input."); break;
                    }
                }
            
                Console.WriteLine("{0} rolls!", p1.name);
                Random rnd = new Random();
                int die = rnd.Next(1, 7);       // Generates a random dice roll and updates the player's position
                p1.setPosition(die);
                Console.WriteLine("{0} rolled a {1} and landed on {2}. ", p1.name, die, tile[p1.position]);
            }

            switch (p1.position)       // Splits off into proper functions depending on what tile the player landed on
            {
                case 1: propLand(p1, p2, Monopoly[0,0]); break;      // All Property tiles grouped together
                case 2: propLand(p1, p2, Monopoly[0, 1]); break;
                case 4: propLand(p1, p2, Monopoly[0, 2]); break;
                case 6: propLand(p1, p2, Monopoly[1, 0]); break;
                case 7: propLand(p1, p2, Monopoly[1, 1]); break;
                case 9: propLand(p1, p2, Monopoly[1, 2]); break;
                case 11: propLand(p1, p2, Monopoly[2, 0]); break;
                case 12: propLand(p1, p2, Monopoly[2, 1]); break;
                case 14: propLand(p1, p2, Monopoly[2, 2]); break;
                case 16: propLand(p1, p2, Monopoly[3, 0]); break;
                case 17: propLand(p1, p2, Monopoly[3, 1]); break;
                case 19: propLand(p1, p2, Monopoly[3, 2]); break;
                case 3:                                         // Both Chance tiles grouped together
                case 13: chance(p1, p2); break;
                case 8:                                         // Both Community Chest tiles grouped together
                case 18: commChest(p1, p2); break;
                case 15: p1.goToJail(); break;             // Go to Jail tile sends player to Jail
                case 10: parking(p1); break;               // Player lands on Free Parking and collects the money there
                default: break;
            }
            monopCheck(p1);             // Calls monopCheck for both players to see if either one got a monopoly this turn
            monopCheck(p2);
            Console.ReadKey();          // Waits for user to press Enter before continuing
            
        }

        static void jailroll(Player player, Player other)         // This function overwrites the roll function if the player is in jail
        {
            Random rnd = new Random();
            int die = rnd.Next(1, 7);               // Generates a random dice roll and tells the user which jail roll this is
            Console.WriteLine("Jail roll #{0} for {1}!", player.jailcounter + 1, player.name);

            if (die == 4)               // Condition for getting out of Jail is to roll a 4
            {
                player.injail = false;      // Player is released from jail, the counter is set back to 0
                player.jailcounter = 0;
                Console.WriteLine("{0} rolled a 4 and is out of jail!", player.name);
            }
            else                           // Player stays in jail and the counter goes up by 1
            {
                player.jailcounter++;
                Console.WriteLine("{0} rolled a {1} and was unable to get out of jail.", player.name, die);
            }

            if (player.jailcounter == 3)    // After Player has rolled 3 times in jail, they pay $50 and roll as normal. 
            {
                player.injail = false;
                player.setPosition(die);
                player.jailcounter = 0;
                Console.Write("{0} paid $50 to get out of Jail. ", player.name);
                player.setMoney(-50);
                freeparking += 50;          // Jail money goes to Free Parking
                Console.WriteLine("{0} rolled a {1} and landed on {2}.", player.name, die, tile[player.position]);
            }
        }

        static void monopCheck(Player p1)       // Function devoted to checking owned properties, called at the end of the roll function
        {
            for (int i = 0; i < 4; i++) // Checks to see if player owns all 3 of a color. If they do, it doubles the rent.
            {
                if (p1.monopOwned[i])
                {
                    Console.WriteLine("{0} now owns all 3 {1} tiles! Their rent is now doubled.", p1.name, Monopoly[i, 0].color);
                    Monopoly[i, 0].rent *= 2;
                    Monopoly[i, 1].rent *= 2;
                    Monopoly[i, 2].rent *= 2;
                    p1.monopOwned[i] = true;        // This boolean array is to make sure the rent doesn't double every time this function is called
                    p1.canbuyhouses = true;
                }
            }
        }

        static void propLand(Player player, Player other, Property prop)        // Function that handles actions when player lands on a property
        {
            if (prop.owner == "Unowned")        // Prompts the user if they would like to buy a property if they land on an unowned one
            {
                Console.Write("{0} is unowned, would you like to purchase it for ${1}? (y/n) ", prop.name, prop.price);
                char choice = Convert.ToChar(Console.ReadLine());
                switch (choice)
                {
                    case 'Y':
                    case 'y':
                        if (player.money < prop.price)
                        {
                            Console.WriteLine("You do not have enough money.");
                            break;
                        }
                        player.buyProp(prop);       // Passes on to function that handles buying properties
                        break;
                    case 'N':
                    case 'n':
                        break;
                    default: Console.WriteLine("Error, invalid input."); break;
                }
            }
            else if (prop.owner == player.name)             // Does nothing if the player owns the property they land on
            {
                Console.WriteLine("You own this property.");
            }
            else if (prop.owner == other.name)              // Pays rent to owner of property landed on
            {
                Console.WriteLine("{0} must pay {1} ${2} in rent.", player.name, other.name, prop.rent);
                player.setMoney(-prop.rent);
                other.setMoney(prop.rent);
            }
        }

        static void listProps(Player player)        // Prints out the Player's money and properties they own, along with its color
        {
            Console.WriteLine("{0} has ${1} and owns: ", player.name, player.money);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Monopoly[i,j].owner == player.name)
                    {
                        Console.Write("{0} ({1}) ", Monopoly[i, j].name, Monopoly[i, j].color);
                    }
                }
                Console.WriteLine();
            }
            if (player.outofjailfree)
                Console.WriteLine("| Get Out Of Jail Free! |");
        }

        static void buyHouse(Player player) 
        {
            if (!p1.canbuyhouses)     // Doesn't let player buy houses unless they have a monopoly
            {
                Console.WriteLine("You don't have any monopolies, so you can't buy any houses.");
            }
            else
            {
                for (int i = 0; i < 4; i++)             // Goes through each set of colors and checks to see if the player owns that monopoly
                {
                    if (Monopoly[i, 0].owner == player.name && Monopoly[i, 1].owner == player.name && Monopoly[i, 2].owner == player.name)
                    {
                        int choice = 0;
                        Console.WriteLine("Which {0} property do you want to build a house on: {1}, {2}, or {3}? (1, 2, 3, or 0 to skip)",
                            Monopoly[i, 0].color, Monopoly[i, 0].name, Monopoly[i, 1].name, Monopoly[i, 2].name);
                        choice = int.Parse(Console.ReadLine());
                        if (choice != 0)                // If player inputs 0, skips this set of properties without having to buy a house
                            Monopoly[i, choice - 1].buildHouse(player);
                    }
                }
            }
        }

        static void tradeProp(Player p1, Player p2)     // Function to trade properties between players
        {
            int choice = -1;
            List<Property> p1offer = new List<Property>(), p2offer = new List<Property>();
            int p1money = 0, p2money = 0;

            Console.WriteLine("{0}, what property(s) will you offer? (0 to exit)", p1.name);
            for (int i = 0; i < p1.propsOwned.Count; i++)
                Console.WriteLine("{0}. {1} ({2})", i + 1, p1.propsOwned[i].name, p1.propsOwned[i].color);

            while (choice != 0)             // Player 1 chooses properties to trade
            {
                choice = int.Parse(Console.ReadLine());
                if (choice == 0) continue;
                p1offer.Add(p1.propsOwned[choice - 1]);
            }
            Console.Write("Will you offer any money? $");
            p1money = int.Parse(Console.ReadLine());        // Player 1 offers money
            choice = -1;

            Console.WriteLine("{0}, what property will you offer? (0 to exit)", p2.name);
            for (int i = 0; i < p2.propsOwned.Count; i++)
                Console.WriteLine("{0}. {1} ({2})", i + 1, p2.propsOwned[i].name, p2.propsOwned[i].color);

            while (choice != 0)             // Player 2 chooses properties to trade
            {
                choice = int.Parse(Console.ReadLine());
                if (choice == 0) continue;
                p2offer.Add(p2.propsOwned[choice - 1]);
            }
            Console.Write("Will you offer any money? $");
            p2money = int.Parse(Console.ReadLine());        // Player 2 offers money

            char finalchoice;
            Console.Write("Finalize the deal? (y/n) ");
            finalchoice = char.Parse(Console.ReadLine());   // 'n' or any other input cancels the deal
            Console.WriteLine();

            switch (finalchoice)
            {
                case 'y':
                case 'Y':
                    string p1temp = "p1temp";       // These strings are used to avoid a bug where Player 1 gets 
                    string p2temp = "p2temp";       // both properties since their deal is processed first.

                    Console.WriteLine("{0} now owns: ", p1.name);
                    foreach (var prop in p2offer)   // Transfers properties from p1 to p2
                    {
                        Console.WriteLine(prop.name);
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (prop.name == Monopoly[i, j].name)
                                {
                                    Monopoly[i, j].owner = p1temp;      // Sets the property's name to the temporary string
                                    p2.propsOwned.Remove(prop);         // Removes the property from the propsOwned list in the Player class
                                }
                            }
                        }
                    }

                    Console.WriteLine("{0} now owns: ", p2.name);
                    foreach (var prop in p1offer)   // Transfers properties from p2 to p1
                    {
                        Console.WriteLine(prop.name);
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (prop.name == Monopoly[i, j].name)
                                {
                                    Monopoly[i, j].owner = p2temp;      // Sets the property's name to the temporary string
                                    p1.propsOwned.Remove(prop);         // Removes the property from the propsOwned list in the Player class
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (Monopoly[i, j].owner == p1temp)
                            {
                                Monopoly[i, j].owner = p1.name;         // Converts the property name from the temp string to the actual name
                                p1.propsOwned.Add(Monopoly[i, j]);      // Adds the property to the propsOwned list in the Player class
                                p1.monopCounter[i]++;
                                if (p1.monopCounter[i] == 3) p1.monopOwned[i] = true;   // Reduced version of code in buyProp function in Player class, allows monopCheck to properly work
                            }

                            if (Monopoly[i, j].owner == p2temp)
                            {
                                Monopoly[i, j].owner = p2.name;         // Converts the property name from the temp string to the actual name
                                p2.propsOwned.Add(Monopoly[i, j]);      // Adds the property to the propsOwned list in the Player class
                                p2.monopCounter[i]++;
                                if (p2.monopCounter[i] == 3) p2.monopOwned[i] = true;
                            }
                        }
                    }

                    p1.setMoney(p2money - p1money);         // Trades money between players. Functions are called this way (p1 - p2)
                    p2.setMoney(p1money - p2money);         // to avoid 2 extra print statements that give no valuable info.
                    monopCheck(p1);         // monopCheck is called here to make sure rent is updated correctly and player can buy houses 
                    monopCheck(p2);         // on the same turn that they get a monopoly through a trade (theoretically)
                    Console.WriteLine();

                    break;
                case 'n':
                case 'N':
                default: Console.WriteLine("Deal Cancelled."); break;
            }
        }

        static void listAllProps()
        {
            Console.WriteLine("\nProperty Name\t\tOwner\t\tRent\n");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.WriteLine("{0, -23} {1, -15} ${2}", Monopoly[i, j].name, Monopoly[i, j].owner, Monopoly[i,j].rent);
                }
            }
            Console.WriteLine();
        }

        static void chance(Player player, Player other)     // Contains all the chance cards and selects them randomly. Needs both p1 and p2 for Property cards
        {
            Random rnd = new Random();
            int card = rnd.Next(1, 9);
            switch (card)
            {
                case 1: Console.WriteLine("Advance to GO!");
                    player.goTo(0);
                    break;
                case 2: Console.WriteLine("Bank pays you dividend of $50!");
                    player.setMoney(50);
                    break;
                case 3: Console.WriteLine("Get out of Jail Free!");
                    player.outofjailfree = true;
                    break;
                case 4: Console.WriteLine("Go directly to Jail. Do not pass Go, do not collect $200.");
                    player.goToJail();
                    break;
                case 5: Console.WriteLine("Pay $20 for each house you own and $50 for each hotel you own.");
                    if (player.houses == 0)
                        Console.WriteLine("You do not own any houses.");
                    else
                        player.setMoney(-20 * player.houses + -50 * player.hotels);
                    break;
                case 6: Console.WriteLine("Advance to Atlantic Avenue. If you pass Go, collect $200.");
                    player.goTo(11);
                    propLand(player, other, atlantic);
                    break;
                case 7: Console.WriteLine("Advance to Connecticut Avenue. If you pass Go, collect $200.");
                    player.goTo(4);
                    propLand(player, other, connecticut);
                    break;
                case 8: Console.WriteLine("Pay a poor tax of $15");
                    player.setMoney(-15);
                    freeparking += 15;
                    break;
            }
        }

        static void commChest(Player player, Player other)      // Contains all the Community Chest cards, also needs p1 and p2
        {
            Random rnd = new Random();
            int card = rnd.Next(1, 9);
            switch (card)
            {
                case 1: Console.WriteLine("Advance to Go!");
                    player.goTo(0);
                    break;
                case 2: Console.WriteLine("Get out of Jail Free!");
                    player.outofjailfree = true;
                    break;
                case 3: Console.WriteLine("Go directly to Jail. Do not pass Go, do not collect $200.");
                    player.goToJail();
                    break;
                case 4: Console.WriteLine("Pay $50 in doctor's fees");
                    player.setMoney(-50);
                    freeparking += 50;
                    break;
                case 5: Console.WriteLine("You win 2nd in a beauty contest! Collect $10.");
                    player.setMoney(10);
                    break;
                case 6: Console.WriteLine("Advance to Pennsylvania Avenue.");
                    player.goTo(19);
                    propLand(player, other, pennsylvania);
                    break;
                case 7: Console.WriteLine("Advance to New York Avenue. If you pass Go, collect $200.");
                    player.goTo(9);
                    propLand(player, other, newyork);
                    break;
                case 8: Console.WriteLine("Advance to Pacific Avenue. If you pass Go, collect $200.");
                    player.goTo(16);
                    propLand(player, other, pacific);
                    break;

            }
        }

        static void parking(Player player)      // All money from fees (Chance cards, Jail fees, etc.) go to Free Parking. 1st player who lands on it gets the money
        {
            if (freeparking != 0)
            {
                Console.WriteLine("{0} collects the ${1} that was in Free Parking!", player.name, freeparking);
                player.setMoney(freeparking);
                freeparking = 0;
            }
        }
    }
}
