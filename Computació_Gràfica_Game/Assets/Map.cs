using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject solidSquare;
    public GameObject foodItem;

    public int ROWS = 10; // Número de linhas no mapa
    public int COLS = 23; // Número de colunas no mapa
    public int platformGap = 2; // Distância mínima entre plataformas na mesma linha
    public int lineGap = 2; // Espaço fixo de 2 linhas de distância

    int[,] squares;

    void Start()
    {
        CreateMap();

        // Mover os objetos originais para fora da área visível
        MoveOutOfView(solidSquare);
        MoveOutOfView(foodItem);
    }

    void MoveOutOfView(GameObject obj)
    {
        // Movendo os objetos originais para fora da área visível
        obj.transform.position = new Vector3(-1000, -1000, 0);
    }

    void CreateMap()
    {
        InitMap();
        BuildRandomMap();  // Gerar mapa aleatoriamente
        DrawMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyMap();
            CreateMap();
        }
    }

    void DestroyMap()
    {
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t != this.transform)
                Destroy(t.gameObject);
        }
    }

    void InitMap()
    {
        squares = new int[ROWS, COLS];

        for (int i = 1; i < (ROWS - 1); i++)
            for (int j = 1; j < (COLS - 1); j++)
                squares[i, j] = 0;

        // Definir as bordas do mapa como sólidas
        for (int i = 0; i < ROWS; i++)
        {
            squares[i, 0] = 1;
            squares[i, COLS - 1] = 1;
        }

        for (int j = 1; j < (COLS - 1); j++)
        {
            squares[0, j] = 1;
        }
    }

    void BuildRandomMap()
    {
        // Pular sempre 2 linhas para garantir o espaçamento vertical fixo
        for (int row = 2; row < ROWS - 1; row += lineGap + 1) 
        {
            int maxPlatforms = CalculateMaxPlatformsPerRow(row);  
            List<int> usedCols = new List<int>(); // Para acompanhar as colunas já ocupadas

            for (int i = 0; i < maxPlatforms; i++) 
            {
                int platformLength = CalculatePlatformLength(row);  
                int startCol;

                // Tentar encontrar uma posição inicial aleatória que não sobreponha outra plataforma e que respeite o gap
                int attempts = 0;
                do
                {
                    startCol = Random.Range(1, COLS - platformLength - 1); // Gera uma posição inicial aleatória
                    attempts++;
                    if (attempts > 10) break; // Limita o número de tentativas para evitar loops infinitos
                } while (!IsSpaceAvailable(startCol, platformLength, usedCols));

                if (attempts > 10) continue; // Se não encontrar espaço, ignora essa plataforma

                // Marcar as colunas usadas para evitar sobreposição
                for (int col = startCol; col < startCol + platformLength; col++)
                {
                    usedCols.Add(col);
                    squares[row, col] = 1;  // Define blocos como sólidos
                }

                // Reservar exatamente o gap de 2 colunas entre plataformas
                for (int col = startCol + platformLength; col < startCol + platformLength + platformGap; col++)
                {
                    usedCols.Add(col);  // Reservar o espaço exato do gap
                }
            }
        }
    }

    bool IsSpaceAvailable(int startCol, int platformLength, List<int> usedCols)
    {
        // Verifica se as colunas necessárias para a plataforma estão livres, incluindo o espaço de gap
        for (int col = startCol; col < startCol + platformLength + platformGap; col++)
        {
            if (usedCols.Contains(col)) return false; // Espaço já ocupado
        }
        return true; // Espaço disponível
    }

    int CalculatePlatformLength(int row)
    {
        // Determinar o comprimento da plataforma de maneira simples e aleatória
        return Random.Range(3, 7); // Tamanho das plataformas variando entre 3 e 6 blocos
    }

    int CalculateMaxPlatformsPerRow(int row)
    {
        // Garante que serão geradas entre 2 e 4 plataformas por linha
        return Random.Range(2, 4); 
    }

    void DrawMap()
    {
        GameObject obj;

        float xOffset = COLS / 2.0f - 0.5f;
        float yOffset = ROWS / 2.0f - 0.5f;

        // Desenhar o mapa visualmente, criando blocos para cada célula "sólida"
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (squares[i, j] == 1) // Se o bloco for sólido (1)
                {
                    obj = Instantiate(solidSquare) as GameObject;
                    obj.transform.position = new Vector3(j - xOffset, i - yOffset, 0.0f);
                    obj.transform.parent = this.gameObject.transform;
                }
                else if (squares[i, j] == 0 && i > 0 && squares[i - 1, j] == 1) // Se for caminhável e houver um bloco abaixo
                {
                    obj = Instantiate(foodItem) as GameObject; // Instanciar comida
                    obj.transform.position = new Vector3(j - xOffset, i - yOffset, -1f);
                    obj.transform.parent = this.gameObject.transform;
                }
            }
        }
    }
}
