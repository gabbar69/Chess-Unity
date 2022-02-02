using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Chess
{
    public class ChessBoard : MonoBehaviour
    {
        EnumCharacters.SpecialMoves specialMoves;

        [SerializeField] float tileScale = 1.250f;
        const int tileCountX = 8;
        const int tileCountY = 8;
        const int whiteTeam = 0;
        const int blackTeam = 1;


        [SerializeField] Camera mainCamera;

        [SerializeField] GameObject[,] tilesArray;
        [SerializeField] ChessCharacter[,] ChessCharactersArray = new ChessCharacter[tileCountX, tileCountY];

        [SerializeField] Material tileMaterial;

        [SerializeField] float yOffset;
        [SerializeField] float selectedOffset;

        [SerializeField] Vector3 boardCenter = Vector3.zero;
        [SerializeField] Vector3 offSet = Vector3.zero;

        Vector2Int currentHover;

        [SerializeField] GameObject[] chessCharactersPrefabs;
        [SerializeField] Material[] teamMaterial;

        [SerializeField] List<Vector2Int> availableMove = new List<Vector2Int>();
        [SerializeField] List<ChessCharacter> defeatedWhiteCharacter = new List<ChessCharacter>();

        [SerializeField] List<ChessCharacter> defeatedBlackCharacter = new List<ChessCharacter>();

        [SerializeField] float defatedCharacterScale = 60;
        [SerializeField] float defatedCharacterSpacing = 0.3f;

        ChessCharacter currentlySelectedCharater;

        bool isWhiteTurn = false;

        List<Vector2Int[]> characterMovesList = new List<Vector2Int[]>();

        private void Awake()
        {
            GenerateAllTiles(tileScale, tileCountX, tileCountY);

            SpawnAllChessCharacter();
            SetPositionAllCharacters();

            isWhiteTurn = true;
        }//Awake

        void Update()
        {
            if (mainCamera != Camera.main)
            {
                return;
            }

            RaycastHit rayInfo;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayInfo, 200, LayerMask.GetMask("Tile", "Hover", "Highlight")))
            {

                Vector2Int hitPosition = LookUpTileIndex(rayInfo.transform.gameObject);
                if (currentHover == -Vector2Int.one)
                {
                    currentHover = hitPosition;

                    tilesArray[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                if (currentHover != hitPosition)
                {
                    tilesArray[currentHover.x, currentHover.y].layer = (ContainValidMove(ref availableMove, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");

                    currentHover = hitPosition;
                    tilesArray[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (ChessCharactersArray[hitPosition.x, hitPosition.y] != null)
                    {
                        if ((ChessCharactersArray[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (ChessCharactersArray[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
                        {
                            currentlySelectedCharater = ChessCharactersArray[hitPosition.x, hitPosition.y];

                            availableMove = currentlySelectedCharater.getAvailableMoves(ref ChessCharactersArray, currentlySelectedCharater.character, tileCountX, tileCountY);


                            specialMoves = currentlySelectedCharater.GetSpecialMoves(ref ChessCharactersArray, ref availableMove, ref characterMovesList, currentlySelectedCharater.character);

                            HighLightTiles();
                        }

                    }
                }

                if (Input.GetMouseButtonUp(0) && currentlySelectedCharater != null)
                {

                    Vector2Int previousCharacterPosition = new Vector2Int(currentlySelectedCharater.currentPositionX, currentlySelectedCharater.currentPositionY);

                    bool isValidMove = ChangeCharacterPosition(currentlySelectedCharater, hitPosition.x, hitPosition.y);

                    if (!isValidMove)
                    {
                        currentlySelectedCharater.SetPosition(GetTileCenter(previousCharacterPosition.x, previousCharacterPosition.y), false);

                    }
                    currentlySelectedCharater = null;
                    RemoveHighLightTiles();

                }

            }
            else
            {
                if (currentHover != -Vector2Int.one)
                {
                    tilesArray[currentHover.x, currentHover.y].layer = (ContainValidMove(ref availableMove, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");

                    currentHover = -Vector2Int.one;
                }

                if (currentlySelectedCharater && Input.GetMouseButtonUp(0))
                {
                    currentlySelectedCharater.SetPosition(GetTileCenter(currentlySelectedCharater.currentPositionX, currentlySelectedCharater.currentPositionY), false);
                    currentlySelectedCharater = null;
                    RemoveHighLightTiles();
                }
            }

            if (currentlySelectedCharater)
            {
                Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);

                float distance = 0.0f;

                if (horizontalPlane.Raycast(ray, out distance))
                {
                    currentlySelectedCharater.SetPosition(ray.GetPoint(distance) + Vector3.up * selectedOffset);
                }

            }

        }// Upadte



        void GenerateAllTiles(float _tileScale, int _tileCountX, int _tileCountY)
        {
            tilesArray = new GameObject[_tileCountX, _tileCountY];

            offSet = new Vector3((_tileCountX / 2) * tileScale, 0, (_tileCountX / 2) * tileScale) + boardCenter;

            for (int y = 0; y < _tileCountX; y++)
            {
                for (int x = 0; x < tileCountY; x++)
                {
                    tilesArray[x, y] = GenerateSingleTile(_tileScale, x, y);
                }
            }

        }// GenerateAllTile

        GameObject GenerateSingleTile(float _tileScale, int _posX, int _posY)
        {
            GameObject tileObject = new GameObject(string.Format("X:{0} Y:{1}", _posX, _posY));
            tileObject.transform.parent = transform;

            Mesh mesh = new Mesh();
            tileObject.AddComponent<MeshFilter>().mesh = mesh;
            tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

            Vector3[] vertices = new Vector3[4];


            vertices[0] = new Vector3(_posX * _tileScale, yOffset, _posY * _tileScale) - offSet;
            vertices[1] = new Vector3(_posX * _tileScale, yOffset, (_posY + 1) * _tileScale) - offSet;
            vertices[2] = new Vector3((_posX + 1) * _tileScale, yOffset, _posY * _tileScale) - offSet;
            vertices[3] = new Vector3((_posX + 1) * _tileScale, yOffset, (_posY + 1) * _tileScale) - offSet;

            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2, };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();

            tileObject.AddComponent<BoxCollider>();
            tileObject.layer = LayerMask.NameToLayer("Tile");



            return tileObject;

        }// GenerateSingleTile

        void SpawnAllChessCharacter()
        {
            #region BLACK TEAM

            ChessCharactersArray[0, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Rook, blackTeam, Vector3.zero);
            ChessCharactersArray[1, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Knight, blackTeam, new Vector3(0, -90, 0));
            ChessCharactersArray[2, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Bishop, blackTeam, Vector3.zero);
            ChessCharactersArray[3, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Queen, blackTeam, Vector3.zero);
            ChessCharactersArray[4, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.King, blackTeam, Vector3.zero);
            ChessCharactersArray[5, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Bishop, blackTeam, Vector3.zero);
            ChessCharactersArray[6, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Knight, blackTeam, new Vector3(0, -90, 0));
            ChessCharactersArray[7, 0] = SpawnSingleChessCharacter(EnumCharacters.Character.Rook, blackTeam, Vector3.zero);

            for (int i = 0; i < tileCountX; i++)
            {
                ChessCharactersArray[i, 1] = SpawnSingleChessCharacter(EnumCharacters.Character.Pawn, blackTeam, Vector3.zero);
            }

            #endregion

            #region WHITE TEAM

            ChessCharactersArray[0, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Rook, whiteTeam, Vector3.zero);
            //ChessCharactersArray[1, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Knight, whiteTeam, new Vector3(0, 90, 0));
            //ChessCharactersArray[2, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Bishop, whiteTeam, Vector3.zero);
            //ChessCharactersArray[3, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Queen, whiteTeam, Vector3.zero);
            //ChessCharactersArray[4, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.King, whiteTeam, Vector3.zero);
            //ChessCharactersArray[5, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Bishop, whiteTeam, Vector3.zero);
            //ChessCharactersArray[6, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Knight, whiteTeam, new Vector3(0, 90, 0));
            //ChessCharactersArray[7, 7] = SpawnSingleChessCharacter(EnumCharacters.Character.Rook, whiteTeam, Vector3.zero);

            //for (int i = 0; i < tileCountX; i++)
            //{
            //    ChessCharactersArray[i, 6] = SpawnSingleChessCharacter(EnumCharacters.Character.Pawn, whiteTeam, Vector3.zero);
            //}


            #endregion


        }

        ChessCharacter SpawnSingleChessCharacter(EnumCharacters.Character _character, int _team, Vector3 rotatingAxis)
        {
            ChessCharacter _ChessCharacter = Instantiate(chessCharactersPrefabs[(int)_character - 1], transform).AddComponent<ChessCharacter>();

            _ChessCharacter.character = _character;
            _ChessCharacter.team = _team;
            _ChessCharacter.GetComponent<MeshRenderer>().material = teamMaterial[_team];
            _ChessCharacter.gameObject.transform.localEulerAngles = rotatingAxis;


            return _ChessCharacter;
        }

        void SetPositionAllCharacters()
        {
            for (int j = 0; j < tileCountX; j++)
            {
                for (int i = 0; i < tileCountX; i++)
                {
                    if (ChessCharactersArray[i, j] != null)
                    {
                        SetPositionSingleCharacter(i, j, false);
                    }
                }
            }
        }

        void SetPositionSingleCharacter(int x, int y, bool forceSet = false)
        {
            ChessCharactersArray[x, y].currentPositionX = x;
            ChessCharactersArray[x, y].currentPositionY = y;

            ChessCharactersArray[x, y].SetPosition(GetTileCenter(x, y));


        }

        Vector3 GetTileCenter(int x, int y)
        {
            return new Vector3(x * tileScale, yOffset, y * tileScale) - offSet + new Vector3(tileScale / 2, 0, tileScale / 2);
        }

        Vector2Int LookUpTileIndex(GameObject tileRayHitInfo)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                for (int x = 0; x < tileCountX; x++)
                {
                    if (tilesArray[x, y] == tileRayHitInfo)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return -Vector2Int.one; // Invalid
        }

        private bool ChangeCharacterPosition(ChessCharacter _currentlySelectedCharater, int x, int y)
        {
            if (!ContainValidMove(ref availableMove, new Vector2Int(x, y)))
            {
                return false;
            }

            Vector2Int _previousCharacterPosition = new Vector2Int(_currentlySelectedCharater.currentPositionX, _currentlySelectedCharater.currentPositionY);

            if (ChessCharactersArray[x, y] != null)
            {
                ChessCharacter OtherChessCharacter = ChessCharactersArray[x, y];

                if (_currentlySelectedCharater.team == OtherChessCharacter.team)
                {
                    return false;
                }

                //Check IsEnemy
                if (OtherChessCharacter.team == 0)
                {

                    if (OtherChessCharacter.character == EnumCharacters.Character.King)
                    {
                        Checkmate();
                    }
                    defeatedWhiteCharacter.Add(OtherChessCharacter);

                    OtherChessCharacter.SetScale(Vector3.one * defatedCharacterScale);

                    OtherChessCharacter.SetPosition(new Vector3(tileCountX * tileScale, yOffset, -1 * tileScale)
                        - offSet
                        + new Vector3(tileScale / 2, 0, tileScale / 2)
                        + (Vector3.forward * defatedCharacterSpacing) * defeatedWhiteCharacter.Count);
                }
                else
                {
                    defeatedBlackCharacter.Add(OtherChessCharacter);

                    OtherChessCharacter.SetScale(Vector3.one * defatedCharacterScale);

                    OtherChessCharacter.SetPosition(new Vector3(-1 * tileScale, yOffset, 8 * tileScale)
                        - offSet
                        + new Vector3(tileScale / 2, 0, tileScale / 2)
                        + (Vector3.forward * defatedCharacterSpacing) * (1 - defeatedBlackCharacter.Count));
                }
            }

            ChessCharactersArray[x, y] = _currentlySelectedCharater;

            ChessCharactersArray[_previousCharacterPosition.x, _previousCharacterPosition.y] = null;

            SetPositionSingleCharacter(x, y, false);

            isWhiteTurn = !isWhiteTurn;

            characterMovesList.Add(new Vector2Int[] { _previousCharacterPosition, new Vector2Int(x, y) });
            SpecialMove();

            return true;
        }

        void HighLightTiles()
        {
            for (int i = 0; i < availableMove.Count; i++)
            {
                tilesArray[availableMove[i].x, availableMove[i].y].layer = LayerMask.NameToLayer("Highlight");
            }
        }

        void RemoveHighLightTiles()
        {
            for (int i = 0; i < availableMove.Count; i++)
            {
                tilesArray[availableMove[i].x, availableMove[i].y].layer = LayerMask.NameToLayer("Tile");

            }
            availableMove.Clear();
        }

        bool ContainValidMove(ref List<Vector2Int> tileInavailableMove, Vector2Int position)
        {
            for (int i = 0; i < tileInavailableMove.Count; i++)
            {
                if (tileInavailableMove[i].x == position.x && tileInavailableMove[i].y == position.y)
                {
                    return true;
                }
            }
            return false;
        }

        void Checkmate()
        {
            Debug.Log("<color=yellow>Checkmate</color>");
        }

        void SpecialMove()
        {
            if (specialMoves == EnumCharacters.SpecialMoves.EnPassant)
            {
                var myMovePos = characterMovesList[characterMovesList.Count - 1];

                ChessCharacter myPawn = ChessCharactersArray[myMovePos[1].x, myMovePos[1].y];

                var enemyPawnPos = characterMovesList[characterMovesList.Count - 2];

                ChessCharacter enemyPawn = ChessCharactersArray[enemyPawnPos[1].x, enemyPawnPos[1].y];


                if (myPawn.currentPositionX == enemyPawn.currentPositionX)
                {
                    if (myPawn.currentPositionY == enemyPawn.currentPositionY - 1 || myPawn.currentPositionY == enemyPawn.currentPositionY + 1)
                    {
                        if (enemyPawn.team == 0)
                        {
                            defeatedWhiteCharacter.Add(enemyPawn);

                            enemyPawn.SetScale(Vector3.one * defatedCharacterScale);

                            enemyPawn.SetPosition(new Vector3(tileCountX * tileScale, yOffset, -1 * tileScale)
                                - offSet
                                + new Vector3(tileScale / 2, 0, tileScale / 2)
                                + (Vector3.forward * defatedCharacterSpacing) * defeatedWhiteCharacter.Count);
                        }

                        else
                        {
                            defeatedBlackCharacter.Add(enemyPawn);

                            enemyPawn.SetScale(Vector3.one * defatedCharacterScale);

                            enemyPawn.SetPosition(new Vector3(-1 * tileScale, yOffset, 8 * tileScale)
                                - offSet
                                + new Vector3(tileScale / 2, 0, tileScale / 2)
                                + (Vector3.forward * defatedCharacterSpacing) * (1 - defeatedBlackCharacter.Count));
                        }
                    }
                }
            }


            if (specialMoves == EnumCharacters.SpecialMoves.Castling)
            {

                Vector2Int[] lastMove = characterMovesList[characterMovesList.Count - 1];

                // Left Rook
                if (lastMove[1].x == 2)// Checking King Position
                {
                    if (lastMove[1].y == 0) // Black Team 
                    {
                        ChessCharacter leftRook = ChessCharactersArray[0, 0];
                        ChessCharactersArray[3, 0] = leftRook;
                        SetPositionSingleCharacter(3, 0, false);
                        ChessCharactersArray[0, 0] = null;
                    }
                    else if (lastMove[1].y == 7) // White Team
                    {
                        ChessCharacter leftRook = ChessCharactersArray[0, 7];
                        ChessCharactersArray[3, 7] = leftRook;
                        SetPositionSingleCharacter(3, 7, false);
                        ChessCharactersArray[0, 7] = null;
                    }
                }

                // Right Rook
                else if (lastMove[1].x == 6)// Checking King Position
                {
                    if (lastMove[1].y == 0) // Black Team 
                    {
                        ChessCharacter rightRook = ChessCharactersArray[7, 0];
                        ChessCharactersArray[5, 0] = rightRook;
                        SetPositionSingleCharacter(5, 0, false);
                        ChessCharactersArray[7, 0] = null;
                    }
                    else if (lastMove[1].y == 7) // White Team
                    {
                        ChessCharacter rightRook = ChessCharactersArray[7, 7];
                        ChessCharactersArray[5, 7] = rightRook;
                        SetPositionSingleCharacter(5, 7, false);
                        ChessCharactersArray[7, 7] = null;
                    }
                }

            }


            if (specialMoves == EnumCharacters.SpecialMoves.Promotion)
            {
                Vector2Int[] lastMove = characterMovesList[characterMovesList.Count - 1];

                ChessCharacter reachedPawn = ChessCharactersArray[lastMove[1].x, lastMove[1].y];

                if (reachedPawn.character == EnumCharacters.Character.Pawn)
                {
                    if (reachedPawn.team == 1 && lastMove[1].y == 7)
                    {
                        ChessCharacter promotionQueen = SpawnSingleChessCharacter(EnumCharacters.Character.Queen, reachedPawn.team, new Vector3(0, 0, 0));

                        promotionQueen.transform.position = reachedPawn.transform.position;

                        Destroy(ChessCharactersArray[lastMove[1].x, lastMove[1].y].gameObject);

                        ChessCharactersArray[lastMove[1].x, lastMove[1].y] = null;

                        ChessCharactersArray[lastMove[1].x, lastMove[1].y] = promotionQueen;

                        SetPositionSingleCharacter(lastMove[1].x, lastMove[1].y);

                    }

                    if (reachedPawn.team == 0 && lastMove[1].y == 0)
                    {
                        ChessCharacter promotionQueen = SpawnSingleChessCharacter(EnumCharacters.Character.Queen, reachedPawn.team, new Vector3(0, 0, 0));

                        promotionQueen.transform.position = reachedPawn.transform.position;

                        Destroy(ChessCharactersArray[lastMove[1].x, lastMove[1].y].gameObject);

                        ChessCharactersArray[lastMove[1].x, lastMove[1].y] = null;

                        ChessCharactersArray[lastMove[1].x, lastMove[1].y] = promotionQueen;

                        SetPositionSingleCharacter(lastMove[1].x, lastMove[1].y);

                    }
                }
            }

        }// SpecialMove




    }// Class

}// Namespace

