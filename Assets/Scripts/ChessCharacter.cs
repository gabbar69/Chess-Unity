using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class ChessCharacter : MonoBehaviour
    {
        public EnumCharacters.Character character;

        public int team;
        public int currentPositionX;
        public int currentPositionY;




        Vector3 desiredPosition;
        Vector3 desiredScale = Vector3.one * 100;

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);

            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);

        }// Update


        public virtual void SetPosition(Vector3 position, bool force = false)
        {
            desiredPosition = position;

            if (force)
            {
                transform.position = desiredPosition;
            }
        }// SetPosition

        public virtual void SetScale(Vector3 scale, bool force = false)
        {
            desiredScale = scale;

            if (force)
            {
                transform.localScale = desiredScale;
            }
        }// SetScale

        public List<Vector2Int> getAvailableMoves(ref ChessCharacter[,] ChessCharacterArray, EnumCharacters.Character character, int tileCountX, int tileCountY)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>();


            switch (character)
            {

                case EnumCharacters.Character.Pawn:
                    availableMoves = PawnMoves(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);
                    return availableMoves;


                case EnumCharacters.Character.Rook:
                    availableMoves = PerpendicularMoves(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);
                    return availableMoves;




                case EnumCharacters.Character.Knight:
                    availableMoves = KnightMoves(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);
                    return availableMoves;


                case EnumCharacters.Character.Bishop:
                    availableMoves = CrossPerpendicularMoves(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);
                    return availableMoves;




                case EnumCharacters.Character.Queen:
                    availableMoves = PerpendicularMoves(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);

                    availableMoves = CrossPerpendicularMoves(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);
                    return availableMoves;




                case EnumCharacters.Character.King:
                    availableMoves = KingMove(ref ChessCharacterArray, availableMoves, tileCountX, tileCountY);
                    return availableMoves;




                default:
                    break;
            }

            return availableMoves;

        }


        public List<Vector2Int> PerpendicularMoves(ref ChessCharacter[,] chessCharactersArray, List<Vector2Int> availableMoves, int tileCountX, int tileCountY)
        {

            // UP MOVES
            for (int column = currentPositionY + 1; column < tileCountY; column++)
            {
                if (chessCharactersArray[currentPositionX, column] == null)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, column));
                }
                if (chessCharactersArray[currentPositionX, column] != null && chessCharactersArray[currentPositionX, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, column));
                    break;
                }

            }

            // DOWN MOVES
            for (int column = currentPositionY - 1; column >= 0; column--)
            {
                if (chessCharactersArray[currentPositionX, column] == null)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, column));
                }
                if (chessCharactersArray[currentPositionX, column] != null && chessCharactersArray[currentPositionX, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, column));
                    break;
                }

            }


            // RIGHT MOVES
            for (int row = currentPositionX + 1; row < tileCountX; row++)
            {
                if (chessCharactersArray[row, currentPositionY] == null)
                {
                    availableMoves.Add(new Vector2Int(row, currentPositionY));
                }
                if (chessCharactersArray[row, currentPositionY] != null && chessCharactersArray[row, currentPositionY].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, currentPositionY));
                    break;
                }

            }

            // LEFT MOVES

            for (int row = currentPositionX - 1; row >= 0; row--)
            {
                if (chessCharactersArray[row, currentPositionY] == null)
                {
                    availableMoves.Add(new Vector2Int(row, currentPositionY));
                }
                if (chessCharactersArray[row, currentPositionY] != null && chessCharactersArray[row, currentPositionY].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, currentPositionY));
                    break;
                }

            }

            return availableMoves;
        }// Perpendicular


        public List<Vector2Int> CrossPerpendicularMoves(ref ChessCharacter[,] chessCharactersArray, List<Vector2Int> availableMoves, int tileCountX, int tileCountY)
        {
            // TOP  RIGHT
            for (int row = currentPositionX + 1, column = currentPositionY + 1; row < tileCountX && column < tileCountY; row++, column++)
            {
                if (chessCharactersArray[row, column] == null)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
                else
                {
                    if (chessCharactersArray[row, column].team != team)
                    {
                        availableMoves.Add(new Vector2Int(row, column));
                    }
                    break;
                }
            }

            // TOP  LEFT
            for (int row = currentPositionX - 1, column = currentPositionY + 1; row >= 0 && column < tileCountY; row--, column++)
            {
                if (chessCharactersArray[row, column] == null)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
                else
                {
                    if (chessCharactersArray[row, column].team != team)
                    {
                        availableMoves.Add(new Vector2Int(row, column));
                    }
                    break;
                }
            }


            // BOTTOM  RIGHT

            for (int row = currentPositionX + 1, column = currentPositionY - 1; row < tileCountX && column >= 0; row++, column--)
            {
                if (chessCharactersArray[row, column] == null)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
                else
                {
                    if (chessCharactersArray[row, column].team != team)
                    {
                        availableMoves.Add(new Vector2Int(row, column));
                    }
                    break;
                }
            }



            // BOTTOM  LEFT
            for (int row = currentPositionX - 1, column = currentPositionY - 1; row >= 0 && column >= 0; row--, column--)
            {
                if (chessCharactersArray[row, column] == null)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
                else
                {
                    if (chessCharactersArray[row, column].team != team)
                    {
                        availableMoves.Add(new Vector2Int(row, column));
                    }
                    break;
                }
            }

            return availableMoves;
        }// Cross Perpendicular

        public List<Vector2Int> KnightMoves(ref ChessCharacter[,] chessCharactersArray, List<Vector2Int> availableMoves, int tileCountX, int tileCountY)
        {
            //Top Right
            int row = currentPositionX + 1;
            int column = currentPositionY + 2;
            if (row < tileCountX && column < tileCountY)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            row = currentPositionX + 2;
            column = currentPositionY + 1;
            if (row < tileCountX && column < tileCountY)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            //Top Left
            row = currentPositionX - 1;
            column = currentPositionY + 2;
            if (row >= 0 && column < tileCountY)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            row = currentPositionX - 2;
            column = currentPositionY + 1;
            if (row >= 0 && column < tileCountY)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            // Bottom Right

            row = currentPositionX + 1;
            column = currentPositionY - 2;
            if (row < tileCountX && column >= 0)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            row = currentPositionX + 2;
            column = currentPositionY - 1;
            if (row < tileCountX && column >= 0)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            // Bottom Left
            row = currentPositionX - 1;
            column = currentPositionY - 2;
            if (row >= 0 && column >= 0)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            row = currentPositionX - 2;
            column = currentPositionY - 1;
            if (row >= 0 && column >= 0)
            {
                if (chessCharactersArray[row, column] == null || chessCharactersArray[row, column].team != team)
                {
                    availableMoves.Add(new Vector2Int(row, column));
                }
            }

            return availableMoves;
        }// Knight Moves


        public List<Vector2Int> KingMove(ref ChessCharacter[,] chessCharactersArray, List<Vector2Int> availableMoves, int tileCountX, int tileCountY)
        {
            // Right
            if (currentPositionX + 1 < tileCountX)
            {
                if (chessCharactersArray[currentPositionX + 1, currentPositionY] == null || chessCharactersArray[currentPositionX + 1, currentPositionY].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX + 1, currentPositionY));
                }

                // TOP RIGHT
                if (currentPositionY + 1 < tileCountY)
                {

                    if (chessCharactersArray[currentPositionX + 1, currentPositionY + 1] == null || chessCharactersArray[currentPositionX + 1, currentPositionY + 1].team != team)
                    {
                        availableMoves.Add(new Vector2Int(currentPositionX + 1, currentPositionY + 1));
                    }

                }

                // BOTTOM RIGHT
                if (currentPositionY - 1 >= 0)
                {
                    if (chessCharactersArray[currentPositionX + 1, currentPositionY - 1] == null || chessCharactersArray[currentPositionX + 1, currentPositionY - 1].team != team)
                    {
                        availableMoves.Add(new Vector2Int(currentPositionX + 1, currentPositionY - 1));
                    }
                }

            }


            // Left
            if (currentPositionX - 1 < tileCountX)
            {
                if (chessCharactersArray[currentPositionX - 1, currentPositionY] == null || chessCharactersArray[currentPositionX + 1, currentPositionY].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX - 1, currentPositionY));
                }
                // TOP LEFT
                if (currentPositionY + 1 < tileCountY)
                {
                    if (chessCharactersArray[currentPositionX - 1, currentPositionY + 1] == null || chessCharactersArray[currentPositionX - 1, currentPositionY + 1].team != team)
                    {
                        availableMoves.Add(new Vector2Int(currentPositionX - 1, currentPositionY + 1));
                    }

                }
                // BOTTOM LEFT
                if (currentPositionY - 1 >= 0)
                {
                    if (chessCharactersArray[currentPositionX - 1, currentPositionY - 1] == null || chessCharactersArray[currentPositionX - 1, currentPositionY - 1].team != team)
                    {
                        availableMoves.Add(new Vector2Int(currentPositionX - 1, currentPositionY - 1));
                    }
                }

            }

            // UP
            if (currentPositionY + 1 < tileCountY)
            {
                if (chessCharactersArray[currentPositionX, currentPositionY + 1] == null || chessCharactersArray[currentPositionX, currentPositionY + 1].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, currentPositionY + 1));
                }

            }


            // DOWN
            if (currentPositionY - 1 >= 0)
            {
                if (chessCharactersArray[currentPositionX, currentPositionY - 1] == null || chessCharactersArray[currentPositionX, currentPositionY - 1].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, currentPositionY - 1));
                }

            }

            return availableMoves;

        }// KING MOVE

        public List<Vector2Int> PawnMoves(ref ChessCharacter[,] chessCharactersArray, List<Vector2Int> availableMoves, int tileCountX, int tileCountY)
        {

            int direction = (team == 1) ? 1 : -1;

            //One Step Ahead
            if (chessCharactersArray[currentPositionX, currentPositionY + direction] == null)
            {
                availableMoves.Add(new Vector2Int(currentPositionX, currentPositionY + direction));
            }

            // Two step Ahead
            if (chessCharactersArray[currentPositionX, currentPositionY + direction] == null)
            {
                if (team == 1 && currentPositionY == 1 && chessCharactersArray[currentPositionX, currentPositionY + (direction * 2)] == null)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, currentPositionY + (direction * 2)));
                }
                else if (team == 0 && currentPositionY == 6 && chessCharactersArray[currentPositionX, currentPositionY + (direction * 2)] == null)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX, currentPositionY + (direction * 2)));
                }
            }

            // KILL MOVE
            if (currentPositionX != tileCountX - 1)
            {
                if (chessCharactersArray[currentPositionX + 1, currentPositionY + direction] != null && chessCharactersArray[currentPositionX, currentPositionY + direction].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX + 1, currentPositionY + direction));

                }

            }
            if (currentPositionX != 0)
            {
                if (chessCharactersArray[currentPositionX - 1, currentPositionY + direction] != null && chessCharactersArray[currentPositionX, currentPositionY + direction].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentPositionX - 1, currentPositionY + direction));

                }
            }

            return availableMoves;
        } // PAWN MOVES

    }// Chess

}// Namespace


