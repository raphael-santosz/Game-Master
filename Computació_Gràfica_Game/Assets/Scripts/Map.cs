using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public Tilemap tilemap; // Referência ao Tilemap para os blocos sólidos
    public TileBase solidTile; // Tile para o bloco sólido
    public GameObject foodItem; // Objeto de comida (agora um GameObject)
    public Character character; // Referência ao Character

    public int ROWS = 19; // Número de linhas no mapa original
    public int COLS = 23; // Número de colunas no mapa
    public int platformGap = 2; // Distância mínima entre plataformas na mesma linha
    public int lineGap = 2; // Espaço fixo de 2 linhas de distância
    public int fooditemCount;
    public int foodCollected;

    int[,] squares;

    void Start()
    {
        // Inicializa foodCollected com o valor de foods do Character
        if (character != null)
        {
            foodCollected = character.foods;
        }
        CreateMap();
    }

    void Update()
    {
       if (character != null)
        {
            foodCollected = character.foods;
        }

        if (Input.GetKeyDown(KeyCode.M) || (foodCollected == fooditemCount))
        {
            DestroyMap();
            CreateMap();
        }
    }

    void CreateMap()
    {
        InitMap();
        BuildRandomMap();  // Gerar mapa aleatoriamente
        DrawMap();
    }

   void DestroyMap()
    {
        // Limpa todos os tiles do Tilemap
        tilemap.ClearAllTiles();

        // Destrói os GameObjects instanciados (como os objetos de comida)
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t != this.transform)
                Destroy(t.gameObject);
        }

        // Zera os contadores de alimentos no Map
        fooditemCount = 0;
        foodCollected = 0;

        // Chama o método ResetFoods no Character para zerar o contador foods
        if (character != null)
        {
            character.ResetFoods();
        }
    }

    void InitMap()
    {
        squares = new int[ROWS, COLS];

        for (int j = 1; j < COLS - 1; j++)
        {
            squares[0, j] = 1; // Linha mais baixa como sólida (base)
        }

        for (int i = 1; i < 5 && i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                squares[i, j] = 1;
            }
        }

        for (int i = 5; i < ROWS - 1; i++)
        {
            for (int j = 1; j < COLS - 1; j++)
            {
                squares[i, j] = 0;
            }
        }

        if (ROWS > 1)
        {
            for (int j = 1; j < COLS - 1; j++)
            {
                squares[ROWS - 1, j] = 1;
            }
        }
    }

    void BuildRandomMap()
    {
        for (int row = 2; row < ROWS - 1; row += lineGap)
        {
            if (row + lineGap >= ROWS - 1)
            {
                row += lineGap;
            }

            int maxPlatforms = CalculateMaxPlatformsPerRow(row);
            List<int> usedCols = new List<int>();

            for (int i = 0; i < maxPlatforms; i++)
            {
                int platformLength = CalculatePlatformLength(row);
                int startCol;

                int attempts = 0;
                do
                {
                    startCol = Random.Range(1, COLS - platformLength - 1);
                    attempts++;
                    if (attempts > 10) break;
                } while (!IsSpaceAvailable(startCol, platformLength, usedCols));

                if (attempts > 10) continue;

                for (int col = startCol; col < startCol + platformLength; col++)
                {
                    usedCols.Add(col);
                    squares[row, col] = 1;
                }

                for (int col = startCol + platformLength; col < startCol + platformLength + platformGap; col++)
                {
                    usedCols.Add(col);
                }
            }
        }
    }

    bool IsSpaceAvailable(int startCol, int platformLength, List<int> usedCols)
    {
        for (int col = startCol; col < startCol + platformLength + platformGap; col++)
        {
            if (usedCols.Contains(col))
            {
                return false;
            }
        }
        return true;
    }

    int CalculatePlatformLength(int row)
    {
        return Random.Range(3, 7);
    }

    int CalculateMaxPlatformsPerRow(int row)
    {
        return Random.Range(3, 5);
    }

    void DrawMap()
    {
        tilemap.ClearAllTiles();

        GameObject obj;
        float tileHeight = tilemap.cellSize.y;
        float tileWidth = tilemap.cellSize.x;

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (squares[i, j] == 1)
                {
                    tilemap.SetTile(new Vector3Int(j, i, 0), solidTile);
                }
                else if (squares[i, j] == 0 && i > 0 && squares[i - 1, j] == 1)
                {
                    obj = Instantiate(foodItem) as GameObject;
                    obj.transform.position = new Vector3(j * tileWidth + 0.45f, i * tileHeight + 0.2f, 0f);
                    obj.transform.parent = this.gameObject.transform;
                    fooditemCount++;
                }
            }
        }
    }
}
