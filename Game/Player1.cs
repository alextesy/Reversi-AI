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
            Tuple<int, int> toReturn = getBestMove(board, playerChar, 5);
            return toReturn;
        }
        private Tuple<int,int> getBestMove(Board board,char playerCh,int depth)
        {
            double record = int.MinValue;
            Tuple<int,int> bestMove = null;
            Board subBoard = new Board(board);
            if (depth <= 0 || board.isTheGameEnded())
            {
                record = getScore(board, playerCh);
            }
            else {
                List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerCh);
            		if (possibleMoves.Capacity>0) {
            			foreach (Tuple<int,int> nextPossibleMove in possibleMoves) {
                            subBoard = new Board(board);
            			    subBoard.fillPlayerMove(playerCh,nextPossibleMove.Item1,nextPossibleMove.Item2);
            			    double result = valueMin(subBoard, Board.otherPlayer(playerCh), depth - 1);
          				    if (result > record) {
            			    	record = result;
                                bestMove = nextPossibleMove;
        				        }
         			    }
            		}
            	}
            return bestMove;
        }
        private double valueMax(Board board, char playerCh, int depth)
        {
            if (depth <= 0 || board.isTheGameEnded())
            {
                return getScore(board, playerCh);
            }
            double best = double.MinValue;
            List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerCh);
            foreach (Tuple<int, int> move in possibleMoves)
            {
                Board tempBoard = new Board(board);
                tempBoard.fillPlayerMove(playerCh, move.Item1, move.Item2);
                char nextPlayer = Board.otherPlayer(playerCh);
                double value = valueMin(board, nextPlayer, depth - 1);
                if (value > best)
                    best = value;
            }
            return best;
        }
        private double valueMin(Board board, char playerCh,int depth)
        {
            if (depth <= 0 || board.isTheGameEnded())
            {
                return getScore(board, playerCh);
            }
            double best = double.MaxValue;
            List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerCh);
            foreach(Tuple<int,int> move in possibleMoves)
            {
                Board tempBoard = new Board(board);
                tempBoard.fillPlayerMove(playerCh,move.Item1,move.Item2);
                char nextPlayer = Board.otherPlayer(playerCh);
                double value = valueMax(board, nextPlayer, depth - 1);
                if (value < best)
                    best = value;
            }



            return best;
        }
        private double getScore(Board board, char PlayerCh)
        {
            double score = 0;
            for(int i = 0; i < board._n; i++)
            {
                for(int j = 0; j < board._n; j++)
                {
                    if (board._boardGame[i, j] == PlayerCh)
                        score += board._boardCosts[i, j];
                }
            }
            return score;
        }
    }
}
