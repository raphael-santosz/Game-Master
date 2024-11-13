using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public Tilemap tilemap; // Referência ao Tilemap para os blocos sólidos
    public TileBase solidTile; // Tile para o bloco sólido
    public GameObject foodItem; // Objeto de comida (agora um GameObject)

    public int ROWS = 19; // Número de linhas no mapa original
    public int COLS = 23; // Número de colunas no mapa
    public int platformGap = 2; // Distância mínima entre plataformas na mesma linha
    public int lineGap = 2; // Espaço fixo de 2 linhas de distância

    int[,] squares;

    void Start()
    {
        CreateMap();
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
        tilemap.ClearAllTiles(); // Limpa todos os tiles do Tilemap

        // Destrói os GameObjects instanciados (como os objetos de comida)
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t != this.transform)
                Destroy(t.gameObject);
        }
    }

    void InitMap()
    {
        // Cria um array bidimensional `squares` com o novo total de linhas `ROWS` e colunas `COLS`.
        squares = new int[ROWS, COLS];

        // Configura a base original do mapa como sólida, ou seja, a linha mais baixa.
        for (int j = 1; j < COLS - 1; j++)
        {
            squares[0, j] = 1; // Linha mais baixa como sólida (base)
        }

        // Adiciona 4 linhas sólidas logo acima da base, começando da linha 1 até a linha 4
        for (int i = 1; i < 5 && i < ROWS; i++) // Configura apenas 4 linhas sólidas
        {
            for (int j = 0; j < COLS; j++)
            {
                squares[i, j] = 1; // Define blocos como sólidos nas 4 linhas acima da base
            }
        }

        // Configura as células restantes acima das 4 linhas sólidas como vazias
        for (int i = 5; i < ROWS - 1; i++) // Limita até ROWS - 1 para não ultrapassar o limite
        {
            for (int j = 1; j < COLS - 1; j++)
            {
                squares[i, j] = 0; // Blocos vazios nas linhas acima das 4 sólidas
            }
        }

        // Configura a linha superior do mapa como sólida (opcional)
        if (ROWS > 1) // Certifica-se de que haja pelo menos uma linha para configurar o topo
        {
            for (int j = 1; j < COLS - 1; j++)
            {
                squares[ROWS - 1, j] = 1; // Linha superior sólida
            }
        }
    }

    void BuildRandomMap()
    {
        // Pular sempre `lineGap` linhas para garantir o espaçamento vertical fixo
        for (int row = 2; row < ROWS - 1; row += lineGap) 
        {
            // Se esta for a última iteração, adiciona um gap extra
            if (row + lineGap >= ROWS - 1) // Verifica se esta é a última linha antes de ROWS - 1
            {
                row += lineGap; // Adiciona um lineGap extra
            }

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
            if (usedCols.Contains(col))
            {
                return false; // Espaço já ocupado
            }
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
        return Random.Range(3, 5); 
    }

    void DrawMap()
    {
        tilemap.ClearAllTiles(); // Limpa o tilemap antes de desenhar novamente

        GameObject obj;

        // Obter o tamanho do tile no Tilemap para usar como base para a altura do foodItem
        float tileHeight = tilemap.cellSize.y; // Tamanho da célula (altura) do tile no Tilemap
        float tileWidth = tilemap.cellSize.x;  // Tamanho da célula (largura) do tile no Tilemap

        // Desenhar o mapa visualmente, criando blocos para cada célula "sólida"
        for (int i = 0; i < ROWS; i++) // Corrigido para ROWS em vez de ROWS + 10
        {
            for (int j = 0; j < COLS; j++)
            {
                if (squares[i, j] == 1) // Se o bloco for sólido (1)
                {
                    tilemap.SetTile(new Vector3Int(j, i, 0), solidTile); // Definir o tile sólido
                }
                else if (squares[i, j] == 0 && i > 0 && squares[i - 1, j] == 1) // Se for caminhável e houver um bloco sólido abaixo
                {
                    // Instanciar o foodItem diretamente acima do bloco sólido
                    obj = Instantiate(foodItem) as GameObject;
                    
                    // Ajustar a posição para alinhar o foodItem logo acima da plataforma e levemente à direita
                    obj.transform.position = new Vector3(j * tileWidth + 0.45f, i * tileHeight + 0.2f, 0f); // Ajuste de X e Y
                   
                    obj.transform.parent = this.gameObject.transform;
                }
            }
        }
    }
}
