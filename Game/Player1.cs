using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player1 : Player
    {
        public void getPlayers              // players ids
        (
            ref string player1_1, 
            ref string player1_2
        )  
        {
            player1_1 = "123456789";        // id1
            player1_2 = "123456789";        // id2
        }
        public Tuple<int, int> playYourTurn
        (
            Board       board,
            TimeSpan    timesup,
            char        playerChar          // 1 or 2
        )
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
      

            Tuple<int, int> toReturn = getBestMove(board, playerChar, timesup);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            //Console.WriteLine(ts.Milliseconds);
  
            return toReturn;
        }
        private Tuple<int,int> getBestMove(Board board,char playerCh, TimeSpan ts)
        {
            double alpha = Double.MinValue;
            double beta = Double.MaxValue;
            double sumScore = (Math.Pow(board._n, 2) * (Math.Pow(board._n, 2) - 1)) / 2;

            double record = int.MinValue;
            Tuple<int,int> bestMove = null;
            Board subBoard = new Board(board);
            if (board.isTheGameEnded())
            {
                record = board.gameScore().Item1;
            }
            else
            {
                List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerCh);
                int depth = 1;

                if (ts.Ticks == 50)
                {
                    if (board._n < 7)
                        depth = 4;
                    else if (board._n < 9)
                        depth = 3;
                    else if (board._n > 9 && board._n < 15)
                        depth = 2;
                    else
                        depth = 1;
                }
                else if (ts.Ticks == 80|| ts.Ticks == 100)
                {
                    if (board._n < 7)
                        depth = 4;
                    else if (board._n < 9)
                        depth = 3;
                    else if(board._n>9&&board._n < 15)
                        depth = 2;
                    else
                        depth = 1;
                    
                }
                else if(ts.Ticks == 150|| ts.Ticks == 200)
                {
                    if (board._n < 9)
                        depth = 4;
                    else if (board._n < 11)
                        depth = 3;
                    else if (board._n < 19)
                        depth = 2;
                    else
                        depth = 1;

                }
             
               

                
                              
                for(int i = 0; i < possibleMoves.Count; i++)
                {
                    double gamma = 0.8;
                    if (depth == 1)
                        gamma = 0.5;
                    subBoard = new Board(board);
            		subBoard.fillPlayerMove(playerCh, possibleMoves[i].Item1, possibleMoves[i].Item2);
                    double huristic = distHeuristic(board, possibleMoves[i]);
                    double score = valueMin(subBoard, Board.otherPlayer(playerCh), depth - 1, sumScore, alpha,beta) / sumScore;
                    double result = gamma * score +(1- gamma) * huristic;
                    if (result > record)
                    {
            			record = result;
                        bestMove = possibleMoves[i];
        			}
         		}
            }
            //Console.WriteLine("record " + record);
            return bestMove;
        }












        private double valueMax(Board board, char playerCh, int depth,double sumScore,double alpha,double beta)
        {
            if (depth <= 0 || board.isTheGameEnded())
            {
                return board.gameScore().Item1 / sumScore;
            }
            double best = 0;
            List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerCh);
            if (possibleMoves != null)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    Board tempBoard = new Board(board);
                    tempBoard.fillPlayerMove(playerCh, possibleMoves[i].Item1, possibleMoves[i].Item2);
                    char nextPlayer = Board.otherPlayer(playerCh);
                    if (tempBoard.getLegalMoves(nextPlayer).Count == 0)
                        continue;
                    double value = valueMin(tempBoard, nextPlayer, depth - 1, sumScore, alpha, beta);
                    if (value > best)
                    {
                        best = value;
                        if (value > alpha)
                            alpha = value;
                    }
                    if (alpha >= beta)
                        return best;
                }
            }
            else
                best= valueMin(board, Board.otherPlayer(playerCh), depth - 1, sumScore, alpha, beta);//board.gameScore().Item1 / sumScore;
            return best;
        }
        private double valueMin(Board board, char playerCh, int depth, double sumScore, double alpha, double beta)
        {
            if (depth <= 0 || board.isTheGameEnded())
            {
                return board.gameScore().Item1 / sumScore;
            }
            double best = 1;
            List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerCh);
            if (possibleMoves != null)
            {
                for (int i = 0; i < possibleMoves.Count; i++)
                {

                    Board tempBoard = new Board(board);
                    tempBoard.fillPlayerMove(playerCh, possibleMoves[i].Item1, possibleMoves[i].Item2);
                    char nextPlayer = Board.otherPlayer(playerCh);
                    if (board.getLegalMoves(nextPlayer).Count == 0)
                        continue;
                    double value = valueMax(tempBoard, nextPlayer, depth - 1, sumScore, alpha, beta);
                   // Console.WriteLine("value " + value);
                    if (value < best)
                    {
                        best = value;
                        if (value < beta)
                            beta = value;
                    }
                    if (alpha >= beta)
                        return best;
                }
            }
            else
            {
                best = valueMax(board, Board.otherPlayer(playerCh), depth - 1, sumScore, alpha, beta);
            }
            return best;
        }
        private double distHeuristic(Board board, Tuple<int,int> move) //Region 5>Region 3 > Region 1>Region 2 > Region4
        {
            if (move.Item1 > 1 && move.Item1 < board._n - 2 && move.Item2 > 1 && move.Item2 < board._n - 2) // Region1
            {
                return 0.6;
            }
            if ((move.Item1==0 && move.Item2>1 && move.Item2 < board._n-2)|| (move.Item1 == board._n-1 && move.Item2 > 1 && move.Item2 < board._n - 2)|| (move.Item2 == 0 && move.Item1 > 1 && move.Item1 < board._n - 2) || (move.Item2 == board._n-1 && move.Item1 > 1 && move.Item1 < board._n - 2)) // Region 3 
            {
                return 0.8;
            }
            if ((move.Item1 == 1 && move.Item2 > 1 && move.Item2 < board._n - 2) || (move.Item1 == board._n - 2 && move.Item2 > 1 && move.Item2 < board._n - 2) || (move.Item2 == 1 && move.Item1 > 1 && move.Item1 < board._n - 2) || (move.Item2 == board._n - 2 && move.Item1 > 1 && move.Item1 < board._n - 2))//Region 2 
            {
                return 0.4;
            }
            if ((board._n - 1 == move.Item1 && board._n - 1 == move.Item2) || (0 == move.Item1 && 0 == move.Item2) || (board._n - 1 == move.Item1 && 0 == move.Item2) || (0 == move.Item1 && board._n - 1 == move.Item2))//Region 5
            {
                return 1.0;
            }
            return 0.2; // Region 4
            

        }


    }
}
