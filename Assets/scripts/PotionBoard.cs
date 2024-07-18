using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; // TextMeshPro'yu kullanmak için ekleyin

// Bu deðiþkeni PotionBoard sýnýfýnda tanýmla

public class PotionBoard : MonoBehaviour
{
    // define the size for the board
    public int width = 6;
    public int height = 8;
    // define some spacing for the board
    public float spacingX;
    public float spacingY;
    // her a reference to our potion prefabs
    public GameObject[] potionPrefabs;
    // her a reference to the collection nodes potionBoard + Go 
    public Node[,] potionBoard;
    public GameObject potionBoardGo;
    private bool isProcessingMove;
    public TextMeshProUGUI scoreText;
    private int score;
    //layoutArray
    public ArrayLayout arrayLayout;
    //public static of potionboard
    public static PotionBoard Instance;
    // Liste tanýmý
    private List<Potion> potionsToRemove;
    private Potion selectedPotion;
    public int totalMoves = 20; // Toplam hamle sayýsý
    public TextMeshProUGUI movesText; // UI'daki hamle sayýsýný gösteren text
    public GameObject winPanel; // Kazanma paneli
    public GameObject losePanel; // Kaybetme paneli
    private bool isBoardLocked = false;
    private bool canTouch= true;
    private int scorefornext = 200;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

       
    }

    private void Start()
    {
        potionsToRemove = new List<Potion>(); // Listeyi baþlatma
        InitailizeBoard();
        UpdateMovesText(); // Hamle sayýsýný güncelle
    }
    private void UpdateMovesText()
    {
        if (movesText != null)
        {
            movesText.text = System.Convert.ToString(totalMoves);
        }
    }
    private void EndGame(bool hasWon)
    {
        if (hasWon)
        {
            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }
    }
    private void CheckEndGame()
    {
        if (totalMoves <= 0)
        {
            if (score >= scorefornext)
            {
                EndGame(true); // Kazandý
            }
            else
            {
                EndGame(false); // Kaybetti
            }
        }
    }
    public void NextLevel()
    {
        // Bir sonraki seviyeye geçiþ için gerekli iþlemler
        // Örneðin, yeni bir sahne yüklemek veya oyunu yeniden baþlatmak
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        totalMoves -= 2;
        scorefornext += 100;

    
    }

    public void Retry()
    {
        // Oyunu yeniden baþlatmak için gerekli iþlemler
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    private void Update()
    {
        if (canTouch == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }
    }

    private void OnMouseDown()
    {   

        // Fare düðmesine basýldýðýnda yapýlacak iþlemler buraya yazýlacak
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);

        if (hit.collider != null)
        {
            //Debug.Log("Hit detected: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<Potion>() != null)
            {
                if (isProcessingMove)
                {
                    return;
                }

                Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                //Debug.Log("I have clicked a potion it is: " + potion.gameObject);
                SelectPotion(potion);
            }
            else
            {
                //Debug.Log("Hit object is not a Potion");
            }
        }
        else
        {
            //Debug.Log("No hit detected");

        }
    }

    private void OnMouseUp()
    {
        // Fare düðmesi býrakýldýðýnda yapýlacak iþlemler buraya yazýlacak
        // Fare düðmesine basýldýðýnda yapýlacak iþlemler buraya yazýlacak
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);

        if (hit.collider != null)
        {
            //Debug.Log("Hit detected: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<Potion>() != null)
            {
                if (isProcessingMove)
                {
                    return;
                }

                Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                //Debug.Log("I have clicked a potion it is: " + potion.gameObject);
                SelectPotion(potion);
            }
            else
            {
                //Debug.Log("Hit object is not a Potion");
            }
        }
        else
        {
            //Debug.Log("No hit detected");
            selectedPotion = null;
        }
    }
    void ClearBoard()
    {
        // Tüm hücreleri temizle ve objeleri yok et
        if (potionBoard != null)
        {
            foreach (Node node in potionBoard)
            {
                if (node != null && node.isUsable && node.potion != null)
                {
                    Destroy(node.potion.gameObject);
                }
            }
        }
    }
    void InitailizeBoard()
    {
        ClearBoard();

        potions = new List<Potion>(); // potions listesini baþlatýn

        potionBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2f;
        spacingY = (float)(height - 1) / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                    Vector2 position = new Vector2(x - spacingX, y - spacingY);

                    int randomIndex = Random.Range(0, potionPrefabs.Length);
                    GameObject potionObject = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity);
                    potionObject.transform.parent = potionBoardGo.transform;

                    Potion potion = potionObject.GetComponent<Potion>();
                    potion.SetIndicies(x, y);

                    potionBoard[x, y] = new Node(true, potionObject);
                    potions.Add(potion); // potions listesine iksiri ekle
                }
            }
        

        if (CheckBoard())
        {
            //Debug.Log("Matches found. Re-initializing board...");
            InitailizeBoard(); // Eðer eþleþme varsa tekrar initialize et
        }
        else
        {
            //Debug.Log("No matches. Game starts now.");
        }
        foreach (Potion potion in potions)
        {
            potion.IsTouchable = !isBoardLocked;
        }
    }
    private List<Potion> potions;

    public bool CheckBoard()
    {
        //Debug.Log("Checking Board");
        

        List<Potion> potions = new();
        bool hasMatched = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log(potionBoard.Length);
                if (potionBoard[x, y].isUsable)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();

                    if (!potion.isMatched)
                    {
                        // Horizontal check
                        MatchResult matchedPotions = IsConnected(potion, new Vector2Int(1, 0), new Vector2Int(-1, 0));
                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }

                        // Vertical check
                        matchedPotions = IsConnected(potion, new Vector2Int(0, 1), new Vector2Int(0, -1));
                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    MatchResult IsConnected(Potion potion, Vector2Int dir1, Vector2Int dir2)
    {
        List<Potion> connectedPotions = new();
        PotionType potionType = potion.potionType;
        connectedPotions.Add(potion);

        CheckDirection(potion, dir1, connectedPotions);
        CheckDirection(potion, dir2, connectedPotions);

        int matchCount = connectedPotions.Count;

        if (matchCount == 3)
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Normal
            };
        }
        else if (matchCount > 3)
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Long
            };
        }
        else
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.None
            };
        }
    }


    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotions)
    {
        PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x, y].isUsable)
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();

                // does our potionType match? it must also not be matched
                if (!neighbourPotion.isMatched && neighbourPotion.potionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    public void SelectPotion(Potion _potion)
    {
        if (selectedPotion == null)
        {
            //Debug.Log(_potion);
            selectedPotion = _potion;
        }
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }
        else if (selectedPotion != _potion)
        {
            if (isProcessingMove)
            {
                return;
            }

            isProcessingMove = true;
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
    }
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        if (!ISAdjacent(_currentPotion, _targetPotion))
        {
            isProcessingMove = false;
            return;
        }

        DoSwap(_currentPotion, _targetPotion);
        StartCoroutine(ProcessMatches(_currentPotion, _targetPotion));
    }



    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        //Debug.Log("Swapping potions: " + _currentPotion.gameObject.name + " and " + _targetPotion.gameObject.name);

        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;
        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        // update indicies
        int tempXIndex = _currentPotion.xIndex;
        int tempYIndex = _currentPotion.yIndex;
        _currentPotion.xIndex = _targetPotion.xIndex;
        _currentPotion.yIndex = _targetPotion.yIndex;
        _targetPotion.xIndex = tempXIndex;
        _targetPotion.yIndex = tempYIndex;

        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);

        //Debug.Log("Swap completed.");
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void AddScore(int matchCount)
    {
        if (matchCount == 3)
        {
            score += 3;
        }
        else if (matchCount >= 4)
        {
            score += 5;
        }

        UpdateScoreText(); // Puaný güncelle
    }

    private void ExplodeMatch()
    {
      

        StartCoroutine(ExplodeMatchCoroutine());
    }

    private IEnumerator ExplodeMatchCoroutine()
    {   
        canTouch = false;
        // Tüm potionsToRemove listesini kontrol et
        foreach (Potion potion in potionsToRemove)
        {
            if (potion == null)
            {
                continue;
            }
            Debug.Log("patlamasý gerekenler" + potion.gameObject.name);
            potion.Explode();
        }

        // Patlama efektlerinin oynatýlmasýný bekle
        yield return new WaitForSeconds(0.5f);

        // Tüm potionsToRemove listesini tekrar kontrol et ve öðeleri yok et
        for (int i = potionsToRemove.Count - 1; i >= 0; i--)
        {
            Potion potion = potionsToRemove[i];
            if (potion == null)
            {
                potionsToRemove.RemoveAt(i);
                continue;
            }

            // Puan ekle
            AddScore(3); // Üçlü eþleþme için
                         // Eðer daha büyük eþleþmeler varsa bunu da hesaba kat
            if (potionsToRemove.Count > 3)
            {
                AddScore(potionsToRemove.Count); // Dörtlü veya daha fazla eþleþme için
            }

            // Ýksiri yok et ve board'u güncelle
            Destroy(potion.gameObject);
            potionBoard[potion.xIndex, potion.yIndex].potion = null;
        }

        // Listeyi temizle
        potionsToRemove.Clear();

        FillBoard();
        yield return new WaitForSeconds(1.0f); // Yeni potionslarýn düþmesini bekle
        if (CheckBoard())
        {
            StartCoroutine(ExplodeMatchCoroutine()); // Yeni eþleþmeler varsa tekrar patlat
        }
        canTouch = true;
    }





    private void FillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion == null)
                {
                    for (int k =y; k < height; k++)
                    {
                        if (potionBoard[x, k].isUsable && potionBoard[x, k].potion != null)
                        {
                            potionBoard[x, y].potion = potionBoard[x, k].potion;
                            potionBoard[x, k].potion = null;
                           
                            Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();
                            potion.SetIndicies(x, y);
                            potion.MoveToTarget(new Vector2(x - spacingX, y - spacingY));
                            break;
                        }
                    }
                }
            }
        }

        DropPotions();
    }

    private void DropPotions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion == null)
                {
                    Vector2 position = new Vector2(x - spacingX, y - spacingY);
                    int randomIndex = Random.Range(0, potionPrefabs.Length);
                    GameObject potionObject = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
                    potionObject.transform.parent = potionBoardGo.transform;
                    Potion potion = potionObject.GetComponent<Potion>();
                    potion.SetIndicies(x, y);
                    potionBoard[x, y].potion = potionObject;

                    potion.MoveToTarget(position);
                }
            }
        }
    }

    private IEnumerator ProcessMatches(Potion _currentPotion, Potion _targetPotion)
    {
        isBoardLocked = true; // Tahtayý dokunulmaz yap

        yield return new WaitForSeconds(0.2f);
        bool hasMatch = CheckBoard();

        if (!hasMatch)
        {
            DoSwap(_currentPotion, _targetPotion);
        }
        else
        {
            ExplodeMatch();
            totalMoves--; // Hamle sayýsýný azalt
            UpdateMovesText(); // Hamle sayýsýný güncelle
            CheckEndGame(); // Oyunun bitip bitmediðini kontrol et
        }

        isBoardLocked = false; // Tahtayý tekrar dokunulabilir yap

        isProcessingMove = false;
    }

    private bool ISAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        return Mathf.Abs(_currentPotion.xIndex - _targetPotion.xIndex) + Mathf.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
    }

    public class MatchResult
    {
        public List<Potion> connectedPotions;
        public MatchDirection direction;
    }

    public enum MatchDirection
    {
        Normal,
        Long,
        Super,
        None
    }
}