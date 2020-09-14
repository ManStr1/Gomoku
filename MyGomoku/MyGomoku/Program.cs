using System;
using System.Collections.Generic;
using System.Threading;

namespace MyGomoku {
    class Game {
        public static Tile[,] tiles;                        // map
        public static int rows;                             // sizes     
        public static int columns;
        public static Player player_1;                      // players
        public static Player player_2;
        public static bool endgame;                         // indicator that game over
        public static int turn;                             // turn counter

        static void Start() {   // set sizes and default values
            rows = 15;                          
            columns = 15;
            tiles = new Tile[rows, columns];
            endgame = false;
            turn = 0;
            player_1 = new Player('x', 7, 7);   // first two turns are identified
            player_2 = new Player('o', 7, 8);

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    Tile tile = new Tile('_', i, j);
                    tiles[i, j] = tile;
                }
            }

        }
        static void Main(string[] args) {

            Start();

            while (!endgame) {
                turn++;

                ShowBoard();
                Console.ReadLine();     // to prevent the next move without command
                Console.Clear();

                if (turn % 2 == 1) {
                    endgame = player_1.MyTurn(tiles, turn, player_2.last_turn, rows, columns);
                } else {
                    endgame = player_2.MyTurn(tiles, turn, player_1.last_turn, rows, columns);
                }
            }
            ShowBoard();
        }

        public static void ShowBoard() {
            
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    Console.Write(tiles[i, j].sign + " ");
                }
                Console.WriteLine();
            }
            
        }
    }

    class Player {
        public char sign;                   // sign of player
        public Tile last_turn;              // player last turn (ex. [4, 5])
        List<Tile> result;                  /* list of 5 coordinates, to identify which is clean, 
                                            if enemy player turn is dangerous or current player turn is dangerous for enemy */
        List<List<Tile>> results;           // list of results
        List<string> pictures;              // scheme of results
        bool find_hard_step;                // indicator that enemy player turn is dangerous
        bool find_my_step;                  // indicator that current player turn is dangerous

        public Player(char ch, int i, int j) {
            sign = ch;
            last_turn = new Tile(sign, i, j);
        }

        public bool MyTurn(Tile[,] tiles, int turn, Tile last_enemy_tile, int rows, int columns) {
            int count_of_sign;                          // counter of signs in line (if more than 2, then turn is potentially dangerous)
            string picture;                             // one scheme of result
            Tile temp_tile;                             // tile for iterating over options
            pictures = new List<string>();
            result = new List<Tile>();
            results = new List<List<Tile>>();
            find_hard_step = false;
            find_my_step = false;

            if (turn == 1) {
                tiles[7, 7].sign = sign;
            } else
            if (turn == 2) {
                tiles[7, 8].sign = sign;
            } else {
                // checking enemy moves

                temp_tile = new Tile(last_enemy_tile.sign, last_enemy_tile.i - 4, last_enemy_tile.j - 4); // set left top tile
                // test \
                for (int i = 0; i < 5; i++) {
                    if (temp_tile.i < 0 || temp_tile.j < 0) {   // exception 1
                        temp_tile.i++;
                        temp_tile.j++;
                        continue;
                    } else
                    if (temp_tile.i + 4 > rows - 1 || temp_tile.j + 4 > columns - 1) { // exception 2
                        break;
                    } else {
                        result = new List<Tile>();
                        picture = "";
                        count_of_sign = 0;

                        for (int k = 0; k < 5; k++) { // working with diagonal line
                            result.Add(tiles[temp_tile.i + k, temp_tile.j + k]);        // set possible dangerous tiles
                            picture += tiles[temp_tile.i + k, temp_tile.j + k].sign;    // creating scheme
                            if (tiles[temp_tile.i + k, temp_tile.j + k].sign == temp_tile.sign) count_of_sign++;  // count enemy signs in line
                        }
                        temp_tile.i++;
                        temp_tile.j++;
                        if (count_of_sign > 2) {
                            pictures.Add(picture);  
                            results.Add(result);
                        }
                    }
                }

                temp_tile = new Tile(last_enemy_tile.sign, last_enemy_tile.i - 4, last_enemy_tile.j);
                // test |
                for (int i = 0; i < 5; i++) {
                    if (temp_tile.i < 0) {
                        temp_tile.i++;
                        continue;
                    } else
                    if (temp_tile.i + 4 > rows - 1) {
                        break;
                    } else {
                        result = new List<Tile>();
                        picture = "";
                        count_of_sign = 0;

                        for (int k = 0; k < 5; k++) {
                            result.Add(tiles[temp_tile.i + k, temp_tile.j]);
                            picture += tiles[temp_tile.i + k, temp_tile.j].sign;
                            if (tiles[temp_tile.i + k, temp_tile.j].sign == temp_tile.sign) count_of_sign++;
                        }
                        temp_tile.i++;
                        if (count_of_sign > 2) {
                            pictures.Add(picture);
                            results.Add(result);
                        }
                    }
                }

                temp_tile = new Tile(last_enemy_tile.sign, last_enemy_tile.i, last_enemy_tile.j - 4);
                // test -
                for (int i = 0; i < 5; i++) {
                    if (temp_tile.j < 0) {
                        temp_tile.j++;
                        continue;
                    } else
                    if (temp_tile.j + 4 > columns - 1) { 
                        break;
                    } else {
                        result = new List<Tile>();
                        picture = "";
                        count_of_sign = 0;

                        for (int k = 0; k < 5; k++) {
                            result.Add(tiles[temp_tile.i, temp_tile.j + k]);
                            picture += tiles[temp_tile.i, temp_tile.j + k].sign;
                            if (tiles[temp_tile.i, temp_tile.j + k].sign == temp_tile.sign) count_of_sign++;
                        }
                        temp_tile.j++;
                        if (count_of_sign > 2) {
                            pictures.Add(picture);
                            results.Add(result);
                        }
                    }
                }

                temp_tile = new Tile(last_enemy_tile.sign, last_enemy_tile.i - 4, last_enemy_tile.j + 4);
                // test /
                for (int i = 0; i < 5; i++) {
                    if (temp_tile.i < 0 || temp_tile.j > columns - 1) {
                        temp_tile.i++;
                        temp_tile.j--;
                        continue;
                    } else
                    if (temp_tile.j - 4 < 0 || temp_tile.i + 4 > rows - 1) {
                        break;
                    } else {
                        result = new List<Tile>();
                        picture = "";
                        count_of_sign = 0;

                        for (int k = 0; k < 5; k++) {
                            result.Add(tiles[temp_tile.i + k, temp_tile.j - k]);
                            picture += tiles[temp_tile.i + k, temp_tile.j - k].sign;
                            if (tiles[temp_tile.i + k, temp_tile.j - k].sign == temp_tile.sign) count_of_sign++;
                        }
                        temp_tile.i++;
                        temp_tile.j--;
                        if (count_of_sign > 2) {
                            pictures.Add(picture);
                            results.Add(result);
                        }
                    }
                }

                bool winner = false;


                for (int i = 0; i < pictures.Count; i++) { // preventing good combination
                    if (last_enemy_tile.sign == 'o') {
                        switch (pictures[i]) {
                            case "ooooo":
                                winner = true;
                                break;
                            case "_oooo":
                            case "o_ooo":
                            case "oo_oo":
                            case "ooo_o":
                            case "oooo_":
                                find_hard_step = true;
                                tiles[results[i][pictures[i].IndexOf('_')].i, results[i][pictures[i].IndexOf('_')].j].sign = sign; 
                                break;

                            case "__ooo":
                            case "o__oo":
                            case "o_o_o":
                            case "o_oo_":
                                find_hard_step = true;
                                tiles[results[i][1].i, results[i][1].j].sign = sign;
                                break;

                            case "oo__o":
                            case "_o_oo":
                            case "oo_o_":
                                find_hard_step = true;
                                tiles[results[i][2].i, results[i][2].j].sign = sign;
                                break;

                            case "_oo_o":
                            case "ooo__":
                                find_hard_step = true;
                                tiles[results[i][3].i, results[i][3].j].sign = sign;
                                break;

                            case "_ooo_":
                                find_hard_step = true;
                                tiles[results[i][4].i, results[i][4].j].sign = sign;
                                break;
                        }
                    } else if (last_enemy_tile.sign == 'x') {
                        switch (pictures[i]) {
                            case "xxxxx":
                                winner = true;
                                break;
                            case "_xxxx":
                            case "x_xxx":
                            case "xx_xx":
                            case "xxx_x":
                            case "xxxx_":
                                find_hard_step = true;
                                tiles[results[i][pictures[i].IndexOf('_')].i, results[i][pictures[i].IndexOf('_')].j].sign = sign; 
                                break;

                            case "__xxx":
                            case "x__xx":
                            case "x_x_x":
                            case "x_xx_":
                                find_hard_step = true;
                                tiles[results[i][1].i, results[i][1].j].sign = sign;
                                break;

                            case "xx__x":
                            case "_x_xx":
                            case "xx_x_":
                                find_hard_step = true;
                                tiles[results[i][2].i, results[i][2].j].sign = sign;
                                break;

                            case "_xx_x":
                            case "xxx__":
                                find_hard_step = true;
                                tiles[results[i][3].i, results[i][3].j].sign = sign;
                                break;

                            case "_xxx_":
                                find_hard_step = true;
                                tiles[results[i][4].i, results[i][4].j].sign = sign;
                                break;
                        }
                    }

                    if (winner) {
                        Console.WriteLine(last_enemy_tile.sign + " WINNER!!!");
                        return true;
                    }
                    
                    if (find_hard_step) break;
                }

                if (!find_hard_step) {
                    // checking current player moves

                    pictures = new List<string>();
                    result = new List<Tile>();
                    results = new List<List<Tile>>();

                    temp_tile = new Tile(last_turn.sign, last_turn.i - 4, last_turn.j - 4);
                    // test \
                    for (int i = 0; i < 5; i++) {
                        if (temp_tile.i < 0 || temp_tile.j < 0) {
                            temp_tile.i++;
                            temp_tile.j++;
                            continue;
                        } else
                        if (temp_tile.i + 4 > rows - 1 || temp_tile.j + 4 > columns - 1) { 
                            break;
                        } else {
                            result = new List<Tile>();
                            picture = "";
                            count_of_sign = 0;

                            for (int k = 0; k < 5; k++) {
                                result.Add(tiles[temp_tile.i + k, temp_tile.j + k]);
                                picture += tiles[temp_tile.i + k, temp_tile.j + k].sign;
                                if (tiles[temp_tile.i + k, temp_tile.j + k].sign == temp_tile.sign) count_of_sign++;
                            }
                            temp_tile.i++;
                            temp_tile.j++;
                            if (count_of_sign > 2) {
                                pictures.Add(picture);
                                results.Add(result);
                            }
                        }
                    }

                    temp_tile = new Tile(last_turn.sign, last_turn.i - 4, last_turn.j);
                    // test |
                    for (int i = 0; i < 5; i++) {
                        if (temp_tile.i < 0) {
                            temp_tile.i++;
                            continue;
                        } else
                        if (temp_tile.i + 4 > rows - 1) {
                            break;
                        } else {
                            result = new List<Tile>();
                            picture = "";
                            count_of_sign = 0;

                            for (int k = 0; k < 5; k++) {
                                result.Add(tiles[temp_tile.i + k, temp_tile.j]);
                                picture += tiles[temp_tile.i + k, temp_tile.j].sign;
                                if (tiles[temp_tile.i + k, temp_tile.j].sign == temp_tile.sign) count_of_sign++;
                            }
                            temp_tile.i++;
                            if (count_of_sign > 2) {
                                pictures.Add(picture);
                                results.Add(result);
                            }
                        }
                    }

                    temp_tile = new Tile(last_turn.sign, last_turn.i, last_turn.j - 4);
                    // test -
                    for (int i = 0; i < 5; i++) {
                        if (temp_tile.j < 0) {
                            temp_tile.j++;
                            continue;
                        } else
                        if (temp_tile.j + 4 > columns - 1) { // instead of 14 need to be rows
                            break;
                        } else {
                            result = new List<Tile>();
                            picture = "";
                            count_of_sign = 0;

                            for (int k = 0; k < 5; k++) {
                                result.Add(tiles[temp_tile.i, temp_tile.j + k]);
                                picture += tiles[temp_tile.i, temp_tile.j + k].sign;
                                if (tiles[temp_tile.i, temp_tile.j + k].sign == temp_tile.sign) count_of_sign++;
                            }
                            temp_tile.j++;
                            if (count_of_sign > 2) {
                                pictures.Add(picture);
                                results.Add(result);
                            }
                        }
                    }

                    temp_tile = new Tile(last_turn.sign, last_turn.i - 4, last_turn.j + 4);
                    // test /
                    for (int i = 0; i < 5; i++) {
                        if (temp_tile.i < 0 || temp_tile.j > columns - 1) {
                            temp_tile.i++;
                            temp_tile.j--;
                            continue;
                        } else
                        if (temp_tile.j - 4 < 0 || temp_tile.i + 4 > rows - 1) { // instead of 14 need to be rows
                            break;
                        } else {
                            result = new List<Tile>();
                            picture = "";
                            count_of_sign = 0;

                            for (int k = 0; k < 5; k++) {
                                result.Add(tiles[temp_tile.i + k, temp_tile.j - k]);
                                picture += tiles[temp_tile.i + k, temp_tile.j - k].sign;
                                if (tiles[temp_tile.i + k, temp_tile.j - k].sign == temp_tile.sign) count_of_sign++;
                            }
                            temp_tile.i++;
                            temp_tile.j--;
                            if (count_of_sign > 2) {
                                pictures.Add(picture);
                                results.Add(result);
                            }
                        }
                    }

                    
                    for (int i = 0; i < pictures.Count; i++) { // setting good combination
                        if (sign == 'o') {
                            switch (pictures[i]) {
                                case "_oooo":
                                case "o_ooo":
                                case "oo_oo":
                                case "ooo_o":
                                case "oooo_":
                                    tiles[results[i][pictures[i].IndexOf('_')].i, results[i][pictures[i].IndexOf('_')].j].sign = sign;
                                    Console.WriteLine("o WINNER!!!");
                                    return true;

                                case "__ooo":
                                case "o__oo":
                                case "o_o_o":
                                case "o_oo_":
                                    find_my_step = true;
                                    tiles[results[i][1].i, results[i][1].j].sign = sign;
                                    break;

                                case "oo__o":
                                case "_o_oo":
                                case "oo_o_":
                                    find_my_step = true;
                                    tiles[results[i][2].i, results[i][2].j].sign = sign;
                                    break;

                                case "_oo_o":
                                case "ooo__":
                                    find_my_step = true;
                                    tiles[results[i][3].i, results[i][3].j].sign = sign;
                                    break;

                                case "_ooo_":
                                    find_my_step = true;
                                    tiles[results[i][4].i, results[i][4].j].sign = sign;
                                    break;
                            }
                        } else if (sign == 'x') {
                            switch (pictures[i]) {
                                case "_xxxx":
                                case "x_xxx":
                                case "xx_xx":
                                case "xxx_x":
                                case "xxxx_":
                                    tiles[results[i][pictures[i].IndexOf('_')].i, results[i][pictures[i].IndexOf('_')].j].sign = sign;
                                    Console.WriteLine("x WINNER!!!");
                                    return true;

                                case "__xxx":
                                case "x__xx":
                                case "x_x_x":
                                case "x_xx_":
                                    find_my_step = true;
                                    tiles[results[i][1].i, results[i][1].j].sign = sign;
                                    break;

                                case "xx__x":
                                case "_x_xx":
                                case "xx_x_":
                                    find_my_step = true;
                                    tiles[results[i][2].i, results[i][2].j].sign = sign;
                                    break;

                                case "_xx_x":
                                case "xxx__":
                                    find_my_step = true;
                                    tiles[results[i][3].i, results[i][3].j].sign = sign;
                                    break;

                                case "_xxx_":
                                    find_my_step = true;
                                    tiles[results[i][4].i, results[i][4].j].sign = sign;
                                    break;
                            }
                        }

                        if (find_my_step) break;
                    }

                    if (!find_my_step) { // if there's no good combinations
                        Random random = new Random();
                        bool b = false;
                        int n = 1;
                        int u = 2;
                        int count = 0;

                        do {
                            if (count > 20) {   //to raise the range of search
                                u++;
                                count = 0;
                            }
                            n = random.Next(1, u);

                            int i = last_turn.i + random.Next(-n, n);
                            int j = last_turn.j + random.Next(-n, n);
                            if (i >= 0 && j >= 0 && i <= 14 && j <= 14) b = (tiles[i, j].sign == '_') ? true : false;

                            if (b) {
                                tiles[i, j].sign = sign;
                                last_turn = tiles[i, j];
                            }
                            count++;
                        } while (!b);
                    }
                }

            }
            return false;
        }
    }

    class Tile {
        public char sign;       // store sign of tile
        public int i;           // store row of tile
        public int j;           // store column of tile

        public Tile(char sign, int i, int j) {
            this.sign = sign;
            this.i = i;
            this.j = j;
        }

    }
}
