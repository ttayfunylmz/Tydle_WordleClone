using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    //this KeyCode wont be changed
    //also, readonly makes it only readable.
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H,
        KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P,
        KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
    };

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;

    private string answerWord;

    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;
    
    [Header("UI")]
    public GameObject invalidWordText;
    public Button newWordButton;
    public Button tryAgainButton;

    private void Awake() 
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start() 
    {
        LoadData();
        NewGame();
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();
        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');

        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split('\n');
    }

    private void SetRandomWord()
    {
        answerWord = solutions[Random.Range(0, solutions.Length)];
        answerWord = answerWord.ToLower().Trim();
    }

    private void Update() 
    {
        Row currentRow = rows[rowIndex];

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);
            invalidWordText.gameObject.SetActive(false);
            AudioManager.instance.Play("DeleteSound");
        }

        else if(columnIndex >= currentRow.tiles.Length)
        {
            if(Input.GetKeyDown(KeyCode.Return)) // Keycode.Return -> Enter Key
            {
                SubmitRow(currentRow);
            }
        }

        else
        {
            for(int i = 0; i < SUPPORTED_KEYS.Length; ++i)
            {
                if(Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    AudioManager.instance.Play("KeyboardSound");
                    break;
                }
            }
        }

        
    }

    private void SubmitRow(Row row)
    {
        if(!IsValidWord(row.word))
        {
            AudioManager.instance.Play("InvalidSound");
            invalidWordText.gameObject.SetActive(true);
            return;
        }

        string remaining = answerWord;

        for(int i = 0; i < row.tiles.Length; ++i)
        {
            Tile tile = row.tiles[i];

            if(tile.letter == answerWord[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if(!answerWord.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }

        for(int i = 0; i < row.tiles.Length; ++i)
        {
            Tile tile = row.tiles[i];

            if(tile.state != correctState && tile.state != incorrectState)
            {
                if(remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int correctIndex = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(correctIndex, 1);
                    remaining = remaining.Insert(correctIndex, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }

        if(HasRow(row))
        {
            enabled = false;
            AudioManager.instance.Play("TrueSound");
        }

        rowIndex++;
        columnIndex = 0;

        if(rowIndex >= rows.Length)
        {
            enabled = false;
            AudioManager.instance.Play("WrongSound");
        }

    }

    private void ClearBoard()
    {
        for(int row = 0; row < rows.Length; ++row)
        {
            for(int col = 0; col < rows[row].tiles.Length; ++col)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    private bool IsValidWord(string word)
    {
        for(int i = 0; i < validWords.Length; ++i)
        {
            if(validWords[i] == word)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasRow(Row row)
    {
        for(int i = 0; i < row.tiles.Length; ++i)
        {
            if(row.tiles[i].state != correctState)
            {
                return false;
            }
        }
        
        return true;
    }

    private void OnEnable() 
    {
        tryAgainButton.gameObject.SetActive(false);
        newWordButton.gameObject.SetActive(false);
    }

    private void OnDisable() 
    {
        tryAgainButton.gameObject.SetActive(true);
        newWordButton.gameObject.SetActive(true);
    }

}
