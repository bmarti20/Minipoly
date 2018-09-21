﻿using System;

namespace Minipoly
{
    class Property
    {
        public string name;
        public int price, rent, housePrice, position;
        public string owner, color;

        public Property(string str, int x, int y, int z, int pos, string col)   // Constructor gets name, price, rent, house price, and position of properties. 
        {
            name = str;
            price = x;
            rent = y;
            housePrice = z;
            position = pos;
            owner = "Thimble";
            color = col;
        }
        
    }

    class Player
    {
        public string name;
        public int money, position, jailcounter, hotelsowned = 0;
        public bool injail, outofjailfree;
        public int[] propsowned = { 0, 0, 0, 0 };
        public int[] housesowned = { 0, 0, 0, 0 };

        public Player(string x, int y)   // Constructor that receives name and starting money. Sets initial position to 0
        {
            name = x;
            money = y;
            position = 0;
            jailcounter = 0;
            injail = false;
            outofjailfree = false;
        }

        // Getters: Return players money, position, and name
        public int getMoney() { return money; }
        public int getPosition() { return position; }
        public string getName() { return name; }

        public void passGo()        // Prints out that player passed Go, adds $200 to money
        {
            money += 100;
            Console.WriteLine("{0} passed Go! {0} now has ${1}.", name, money);
        }

        public void setMoney(int x)
        {
            money += x;
            Console.WriteLine("{0} now has ${1}.", name, money);
        }   // Updates player money

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

        public void buyProp(Property prop)
        {
            Console.WriteLine("{0} now owns {1}!", name, prop.name);
            prop.owner = name;
            setMoney(-prop.price);

            switch (prop.color)             // Updaates propsowned array with a property. 0 is blues, 1 is yellows, 2 is reds, 3 is greens
            {
                case "blue": propsowned[0]++; break;
                case "yellow": propsowned[1]++; break;
                case "red": propsowned[2]++; break;
                case "green": propsowned[3]++; break;
            }
        }
        
        public bool monopoly()          // Returns true if player has a monopoly, otherwise returns false
        {
            foreach (int num in propsowned)
            {
                if (num == 3)
                    return true;
            }
            return false;
        }
    }



    class Program
    {
        // Money players start the game with
        const int STARTMONEY = 800;
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

        public static Player p1 = new Player("p1", STARTMONEY);
        public static Player p2 = new Player("p2", STARTMONEY);

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Mini-opoly!");
            Console.Write("Please enter Player 1 name: ");
            p1.name = Console.ReadLine();
            Console.WriteLine("{0} has ${1}", p1.getName(), p1.getMoney());
            Console.Write("Please enter Player 2 name: ");
            p2.name = Console.ReadLine();
            Console.WriteLine("{0} has ${1}", p2.getName(), p2.getMoney());
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
                Console.WriteLine();
                roll(p2, p1);           // Calls roll function

                if (p1.getMoney() <= 0)
                {
                    Console.WriteLine("{0} has gone bankrupt. {1} wins!", p1.getName(), p2.getName());
                    gameover = true;
                }
                else if (p2.getMoney() <= 0)
                {
                    Console.WriteLine("{0} has gone bankrupt. {1} wins!", p2.getName(), p1.getName());
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
                if (p1.monopoly())
                {
                    Console.Write("{0}, would you like to roll or buy houses? (r/h) ", p1.name);
                    char choice = Convert.ToChar(Console.ReadLine());
                    switch (choice)
                    {
                        case 'r':           // Escapes to normal roll function
                        case 'R':
                            break;      
                        case 'h':           // Buys a house on a property
                        case 'H':
                            break;
                        default: Console.WriteLine("Error, invalid input."); break;
                    }
                
                }

                Console.WriteLine("{0} rolls!", p1.name);
                Random rnd = new Random();
                int die = rnd.Next(1, 7);       // Generates a random dice roll and updates the player's position
                p1.setPosition(die);
                Console.WriteLine("{0} rolled a {1} and landed on {2}. ", p1.getName(), die, tile[p1.getPosition()]);
            }

            switch (p1.getPosition())       // Splits off into proper functions depending on what tile the player landed on
            {
                case 1: propLand(p1, p2, oriental); break;      // All Property tiles grouped together
                case 2: propLand(p1, p2, vermont); break;
                case 4: propLand(p1, p2, connecticut); break;
                case 6: propLand(p1, p2, stjames); break;
                case 7: propLand(p1, p2, tennessee); break;
                case 9: propLand(p1, p2, newyork); break;
                case 11: propLand(p1, p2, atlantic); break;
                case 12: propLand(p1, p2, ventnor); break;
                case 14: propLand(p1, p2, marvin); break;
                case 16: propLand(p1, p2, pacific); break;
                case 17: propLand(p1, p2, carolina); break;
                case 19: propLand(p1, p2, pennsylvania); break;
                case 3:                                         // Both Chance tiles grouped together
                case 13: chance(p1, p2); break;
                case 8:                                         // Both Community Chest tiles grouped together
                case 18: commChest(p1, p2); break;
                case 15: p1.goToJail(); break;             // Go to Jail tile sends player to Jail
                case 10: parking(p1); break;               // Player lands on Free Parking and collects the money there
                default: break;
            }
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

            if (player.jailcounter == 3)
            {
                player.injail = false;
                player.setPosition(die);
                player.jailcounter = 0;
                Console.Write("{0} paid $50 to get out of Jail. ", player.name);
                player.setMoney(-50);
                freeparking += 50;
                Console.WriteLine("{0} rolled a {1} and landed on {2}.", player.name, die, tile[player.position]);
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
                        player.buyProp(prop);
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
                case 4: Console.WriteLine("Go directly to Jail. Do not pass Go, do not collect $100.");
                    player.goToJail();
                    break;
                case 5: Console.WriteLine("Pay $25 for each house you own and $100 for each hotel you own.");
                    break;
                case 6: Console.WriteLine("Advance to Atlantic Avenue. If you pass Go, collect $100.");
                    player.goTo(11);
                    propLand(player, other, atlantic);
                    break;
                case 7: Console.WriteLine("Advance to Connecticut Avenue. If you pass Go, collect $100.");
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
                case 3: Console.WriteLine("Go directly to Jail. Do not pass Go, do not collect $100.");
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
                case 7: Console.WriteLine("Advance to New York Avenue. If you pass Go, collect $100.");
                    player.goTo(9);
                    propLand(player, other, newyork);
                    break;
                case 8: Console.WriteLine("Advance to Pacific Avenue. If you pass Go, collect $100.");
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