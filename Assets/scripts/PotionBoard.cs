using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // TextMeshPro'yu kullanmak i�in ekleyin

// Bu de�i�keni PotionBoard s�n�f�nda tan�mla

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
    // Liste tan�m�
    private List<Potion> potionsToRemove;
    private Potion selectedPotion;
     static List<int> Totalmoveslist;
    public TextMeshProUGUI movesText; // UI'daki hamle say�s�n� g�steren text
    public GameObject winPanel; // Kazanma paneli
    public GameObject losePanel; // Kaybetme paneli
    public GameObject settingPanel; 
    private bool isBoardLocked = false;
    private bool canTouch = true;
    private int scorefornext = 200;
    static public int movesindex = 0;
    public GameObject ColorBomb;
    public GameObject TNT;
    private Potion matchedPotion;
    public Button SettingButton;
    public List<Potion> ColorbombList;


    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1.0f;


    }

    private void Start()
    {
        canTouch = true;
        Totalmoveslist = new List<int> { 20, 18, 16, 14, 12, 10 };
        potionsToRemove = new List<Potion>(); // Listeyi ba�latma
        InitailizeBoard();
        UpdateMovesText(); // Hamle say�s�n� g�ncelle
        settingPanel.SetActive(false);
    }
    private void UpdateMovesText()
    {
        if (movesText != null)
        {
            movesText.text = ":" + System.Convert.ToString(Totalmoveslist[movesindex]);
        }
    }
    private void EndGame(bool hasWon)
    {
        if (hasWon)
        {
            winPanel.SetActive(true);
            isBoardLocked = true;
        }
        else
        {
            losePanel.SetActive(true);
            isBoardLocked = true;

        }
    }
    private void CheckEndGame()
    {
        if (Totalmoveslist[movesindex] <= 0)
        {
            if (score >= scorefornext)
            {
                EndGame(true); // Kazand�
            }
            else
            {
                EndGame(false); // Kaybetti
            }
        }
    }


    public void Settings()
    {
            settingPanel.SetActive(true);
    }
    public void ClosesettingButton() 
    {
        settingPanel.SetActive(false) ;
    }
    public void BackMenu()
    {
        SceneManager.LoadScene("U� scene");
    }
    public void NextLevel()
    {
        // Bir sonraki seviyeye ge�i� i�in gerekli i�lemler
        // �rne�in, yeni bir sahne y�klemek veya oyunu yeniden ba�latmak
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        movesindex++;
        scorefornext += 100;


    }

    public void Retry()
    {
        // Oyunu yeniden ba�latmak i�in gerekli i�lemler
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

        if (Totalmoveslist[movesindex] == 0)
        {
            isBoardLocked = true;
        }

    }

    private void OnMouseDown()
    {

        // Fare d��mesine bas�ld���nda yap�lacak i�lemler buraya yaz�lacak
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
        // Fare d��mesi b�rak�ld���nda yap�lacak i�lemler buraya yaz�lacak
        // Fare d��mesine bas�ld���nda yap�lacak i�lemler buraya yaz�lacak
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
        // T�m h�creleri temizle ve objeleri yok et
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

        potions = new List<Potion>(); // potions listesini ba�lat�n

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
            InitailizeBoard(); // E�er e�le�me varsa tekrar initialize et
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
        bool hasMatched = false;
        canTouch = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();

                    if (!potion.isMatched)
                    {
                        // Horizontal check
                        MatchResult matchedPotions = IsConnected(potion, new Vector2Int(1, 0), new Vector2Int(-1, 0));
                        if (matchedPotions.connectedPotions.Count == 3)
                        {
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }
                        else if (matchedPotions.connectedPotions.Count == 4)
                        {
                            matchedPotion = potion; // 4'l� e�le�me oldu�unda matchedPotion'� atay�n
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }
                        else if (matchedPotions.connectedPotions.Count >= 5)
                        {
                            matchedPotion = potion; // 5 veya daha fazla e�le�me oldu�unda matchedPotion'� atay�n
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }

                        // Vertical check
                        matchedPotions = IsConnected(potion, new Vector2Int(0, 1), new Vector2Int(0, -1));
                        if (matchedPotions.connectedPotions.Count == 3)
                        {
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }
                        else if (matchedPotions.connectedPotions.Count == 4)
                        {
                            matchedPotion = potion; // 4'l� e�le�me oldu�unda matchedPotion'� atay�n
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }
                        else if (matchedPotions.connectedPotions.Count >= 5)
                        {
                            matchedPotion = potion; // 5 veya daha fazla e�le�me oldu�unda matchedPotion'� atay�n
                            potionsToRemove.AddRange(matchedPotions.connectedPotions);
                            foreach (Potion pot in matchedPotions.connectedPotions)
                                pot.isMatched = true;
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        canTouch = true;

        return hasMatched;
    }
    private void CreateTNT(Vector2Int position)
    {
        GameObject tntObject = Instantiate(TNT, new Vector2(position.x - spacingX, position.y - spacingY), Quaternion.identity);
        tntObject.transform.parent = potionBoardGo.transform;
        Potion   tntBonus = tntObject.GetComponent<Potion>();
        tntBonus.SetIndicies(position.x, position.y);
        potionBoard[position.x, position.y].potion = tntObject;
    }

    private void CreateColorBomb(Vector2Int position)
    {
        GameObject colorBombObject = Instantiate(ColorBomb, new Vector2(position.x - spacingX, position.y - spacingY), Quaternion.identity);
        colorBombObject.transform.parent = potionBoardGo.transform;
        Potion colorBombBonus = colorBombObject.GetComponent<Potion>();
        colorBombBonus.SetIndicies(position.x, position.y);
        potionBoard[position.x, position.y].potion = colorBombObject;
    }
    MatchResult IsConnected(Potion potion, Vector2Int dir1, Vector2Int dir2)
    {
        List<Potion> connectedPotions1 = CheckDirection(potion, dir1);
        List<Potion> connectedPotions2 = CheckDirection(potion, dir2);

        // E�le�meleri kontrol etmeden �nce her iki y�n�n merkez potion'�n� ekleyin
        connectedPotions1.Insert(0, potion);
        connectedPotions2.Insert(0, potion);

        // E�le�meleri de�erlendir
        List<MatchResult> results = new List<MatchResult>();
       
        if (connectedPotions1.Count >= 3)
        {
            results.Add(new MatchResult
            {
                connectedPotions = connectedPotions1,
                direction = connectedPotions1.Count == 3 ? MatchDirection.Normal : MatchDirection.Long
            });
        }

        if (connectedPotions2.Count >= 3)
        {
            results.Add(new MatchResult
            {
                connectedPotions = connectedPotions2,
                direction = connectedPotions2.Count == 3 ? MatchDirection.Normal : MatchDirection.Long
            });
        }

        // E�le�meler yoksa None d�nd�r
        if (results.Count == 0)
        {
            return new MatchResult
            {
                connectedPotions = new List<Potion> { potion },
                direction = MatchDirection.None
            };
        }
        

        // E�er birden fazla e�le�me varsa, en uzununu d�nd�r
        return results.OrderByDescending(r => r.connectedPotions.Count).First();
    }


    List<Potion> CheckDirection(Potion pot, Vector2Int direction)
    {
        List<Potion> directionMatches = new List<Potion>();
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
                    directionMatches.Add(neighbourPotion);
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

        return directionMatches;
    }

    public void SelectPotion(Potion _potion)
    {
        if (isBoardLocked == false)
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

        UpdateScoreText(); // Puan� g�ncelle
    }

    private void ExplodeMatch()
    {   canTouch = false;
        StartCoroutine(ExplodeMatchCoroutine());
        canTouch = true;
    }
    public List<Potion> TNTPotionList;
    public IEnumerator ExplodeTNT(Potion tntPotion)
    {   

        int centerX = tntPotion.xIndex;
        int centerY = tntPotion.yIndex;

        /////////////yield return new WaitForSeconds(1f);


        // 3x3'l�k alan� kontrol etmek i�in iki d�ng� kullan�l�r.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (centerX + x < 0 || centerY + y < 0 || centerX + x > 7  || centerY + y > 7)
                {
                    continue;
                }
                if ( x == 0  &&  y == 0)
                {
                    continue;
                }
                Potion potion = potionBoard[centerX + x, centerY + y].potion.GetComponent<Potion>();
                TNTPotionList.Add(potion);

                potion.Explode();

               
            }
        }
        tntPotion.Explode();
        yield return new WaitForSeconds(0.8f);

        foreach (Potion pot in TNTPotionList)
        {
            potionBoard[pot.xIndex, pot.yIndex].potion = null;

            Destroy(pot.gameObject);
           
        }

        potionBoard[tntPotion.xIndex, tntPotion.yIndex].potion = null;

        Destroy(tntPotion.gameObject);

        TNTPotionList.Clear();

        FillBoard();
        yield return new WaitForSeconds(1.0f); // Yeni potionslar�n d��mesini bekle
        if (CheckBoard())
        {

            StartCoroutine(ExplodeMatchCoroutine()); // Yeni e�le�meler varsa tekrar patlat

        }
    }
    public IEnumerator ExplodeColorBomb(Potion Colorbomb, Potion targetpotion )
    {


        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 8; y++) 
            {
                Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();
                Debug.Log("potiontype" + potion.potionType+ "  \n " + targetpotion.potionType);
                if (potion.potionType == targetpotion.potionType) 
                {
                    potion = potionBoard[x,y].potion.GetComponent<Potion>();
                    ColorbombList.Add(potion);
                    potion.Explode();
                    
                }
            }
        }

        Colorbomb.Explode();
        yield return new WaitForSeconds(0.8f);

        foreach (Potion pot in ColorbombList)
        {
            potionBoard[pot.xIndex, pot.yIndex].potion = null;

            Destroy(pot.gameObject);

        }

        potionBoard[Colorbomb.xIndex, Colorbomb.yIndex].potion = null;

        Destroy(Colorbomb.gameObject);

        ColorbombList.Clear();

        FillBoard();
        yield return new WaitForSeconds(1.0f); // Yeni potionslar�n d��mesini bekle
        if (CheckBoard())
        {

            StartCoroutine(ExplodeMatchCoroutine()); // Yeni e�le�meler varsa tekrar patlat

        }
    }
    private IEnumerator ExplodeMatchCoroutine()
    {
        Vector2Int matchedPotionPosition = new Vector2Int(matchedPotion.xIndex, matchedPotion.yIndex);

            // T�m potionsToRemove listesini tekrar kontrol et ve ��eleri yok et
            for (int i = potionsToRemove.Count - 1; i >= 0; i--)
            {
                Potion potion = potionsToRemove[i];
                if (potion == null)
                {
                
                potionsToRemove.RemoveAt(i);
                    continue;
                }

            potion.Explode();
            // Puan ekle

            AddScore(potionsToRemove.Count);

                // �ksiri yok et ve board'u g�ncelle
               
                potionBoard[potion.xIndex, potion.yIndex].potion = null;
            }
        yield return new WaitForSeconds(0.5f);

        foreach (var potion in potionsToRemove)
        {
            Destroy(potion.gameObject);
        }


        // TNT veya ColorBomb olu�tur
        if (potionsToRemove.Count == 4)
            {
            CreateTNT(matchedPotionPosition);

        }
        else if (potionsToRemove.Count == 5)
            {
                CreateColorBomb(matchedPotionPosition);
            }

            // Listeyi temizle
            potionsToRemove.Clear();

            FillBoard();
            yield return new WaitForSeconds(0.5f); // Yeni potionslar�n d��mesini bekle
            if (CheckBoard())
            {
                StartCoroutine(ExplodeMatchCoroutine()); // Yeni e�le�meler varsa tekrar patlat
            }
        
    }


    private void FillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion == null)
                {
                    for (int k = y; k < height; k++)
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
        canTouch = true;
    }

    private IEnumerator ProcessMatches(Potion _currentPotion, Potion _targetPotion)
    {
        isBoardLocked = true; // Tahtay� dokunulmaz yap

        yield return new WaitForSeconds(0.2f);
        bool hasMatch = CheckBoard();


        if (_currentPotion.potionType == PotionType.TNT)
        {
            StartCoroutine(ExplodeTNT(_currentPotion));
            Totalmoveslist[movesindex]--; // Hamle say�s�n� azalt
            UpdateMovesText(); // Hamle say�s�n� g�ncelle
            CheckEndGame();
            hasMatch = true;
        }

        if (_targetPotion.potionType == PotionType.TNT)
        {
            StartCoroutine(ExplodeTNT(_targetPotion));
            Totalmoveslist[movesindex]--; // Hamle say�s�n� azalt
            UpdateMovesText(); // Hamle say�s�n� g�ncelle"
            CheckEndGame(); // Oyunun bitip bitmedi�ini kontrol et
            hasMatch = true;

        }
        if (_currentPotion.potionType == PotionType.ColorBomb)
        {
            StartCoroutine(ExplodeColorBomb(_currentPotion,_targetPotion));
            Totalmoveslist[movesindex]--; // Hamle say�s�n� azalt
            UpdateMovesText(); // Hamle say�s�n� g�ncelle
            CheckEndGame();
            hasMatch = true;
        }

        if (_targetPotion.potionType == PotionType.ColorBomb)
        {
            StartCoroutine(ExplodeColorBomb(_targetPotion, _currentPotion));
            Totalmoveslist[movesindex]--; // Hamle say�s�n� azalt
            UpdateMovesText(); // Hamle say�s�n� g�ncelle"
            CheckEndGame(); // Oyunun bitip bitmedi�ini kontrol et
            hasMatch = true;
        }
        if (!hasMatch)
        {
            DoSwap(_currentPotion, _targetPotion);
        }
        else
        {
            ExplodeMatch();
            Totalmoveslist[movesindex]--; // Hamle say�s�n� azalt
            UpdateMovesText(); // Hamle say�s�n� g�ncelle
            CheckEndGame(); // Oyunun bitip bitmedi�ini kontrol et
        }

      

      

        isBoardLocked = false; // Tahtay� tekrar dokunulabilir yap

        isProcessingMove = false;
    }

    private bool ISAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        return Mathf.Abs(_currentPotion.xIndex - _targetPotion.xIndex) + Mathf.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
    }

    public struct MatchResult
    {
        public List<Potion> connectedPotions;
        public MatchDirection direction;
    }
    public enum MatchDirection
    {
        None,
        Normal,
        Long
    }
}


