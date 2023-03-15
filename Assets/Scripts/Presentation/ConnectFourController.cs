using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton controller responsible for mediating between
/// the UI elements and the game logic
/// </summary>
public class ConnectFourController : MonoBehaviour
{
    private static ConnectFourController _instance;
    public static ConnectFourController Instance
    {
        get { return _instance; }
    }

    #region Public Properties
    public BoardController Board;
    public Chip PlayerChip;
    public Chip ComputerChip;
    public Button ResetButton;
    public Text Message;
    public bool SmartComputer;
    #endregion

    #region Private Properties
    private ConnectFourLogic connectFour;
    private List<ButtonController> buttons;
    private string winMessage = "YOU WIN!! WOOHOOO!!";
    private string loseMessage = "AWWW YOU LOST! TRY AGAIN!";
    private string tieMessage = "LOOKS LIKE ITS A TIE! TRY AGAIN!";
    private string hintMessage = "Hint: Highlighted arrow is your best chance to win!";
    
    #endregion

    #region MonoBehaviors
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        connectFour = new ConnectFourLogic();
        
        buttons = new List<ButtonController>();
        foreach (var column in Board.Columns)
        {
            buttons.Add(column.Button);
        }
        
        HighlightBestColumns();

        PrintMessage(hintMessage);
    }
    #endregion
    
    #region Public Methods
    public void ResetBoard()
    {
        foreach (var column in Board.Columns)
        {
            column.Reset();
        }

        connectFour.ResetBoardMatrix();
        
        HighlightBestColumns();
        
        PrintMessage(hintMessage);
    }

    
    public void DropPlayerChip(int column)
    {
        if (Message.gameObject.activeInHierarchy)
        {
            Message.gameObject.SetActive(false);
        }
        
        DropChip(PlayerChip, ChipType.Player, column);
        UnlockAllButtons(false);
        if (connectFour.CheckForWin(ChipType.Player))
        {
            PrintMessage(winMessage);
            ResetButton.interactable = true;
        }
        else
        {
            DropComputerChip();
        }
    }

    public void DropComputerChip()
    {
        if(SmartComputer)
        {
            StartCoroutine(DropComputerChip(connectFour.BestColumns()[Random.Range(0, connectFour.BestColumns().Count)]));
        }
        else
        {
            StartCoroutine(DropComputerChip(connectFour.FindRandomColumn()));
        }
    }
    #endregion

    #region Private Methods
    private IEnumerator DropComputerChip(int randomColumn)
    {
        yield return new WaitForSeconds(1.5f);
        DropChip(ComputerChip, ChipType.Computer, randomColumn);
        if (connectFour.CheckForWin(ChipType.Computer))
        {
            PrintMessage(loseMessage);
            UnlockAllButtons(false);
            ResetButton.interactable = true;
        }
        else
        {
            UnlockAllButtons(true);
        }
    }

    private void DropChip(Chip chip, ChipType type, int columnIndex)
    {
        for (int row = 0; row < ConnectFourLogic.ROW_COUNT; row++)
        {
            if (connectFour.IsCellEmpty(columnIndex, row))
            {
                var finalPostion = Board.Columns[columnIndex].RowPositions[row].position;
                InstantiateChip(chip, Board.Columns[columnIndex].Button.transform.position,
                    finalPostion, Board.Columns[columnIndex].ChipParent.transform);
                
                connectFour.SetChipType(columnIndex, row, type);
                if (type == ChipType.Computer)
                {
                    HighlightBestColumns();
                }
                    
                break;
            }
        }
    }

    private void InstantiateChip(Chip chip, Vector3 startPosition, Vector3 finalPosition, Transform parent)
    {
        var newChip = Instantiate(chip, startPosition, Quaternion.identity, parent);
        newChip.StartDropAnimation(finalPosition);
    }

    private void UnlockAllButtons(bool unlock)
    {
        foreach (var column in Board.Columns)
        {
            if (!unlock || !column.IsFull())
            {
                var button = column.Button.GetComponent<Button>();
                button.interactable = unlock;
            }
        }

        ResetButton.interactable = unlock;

        if (connectFour.IsGameOver())
        {
            PrintMessage(tieMessage);
        }
    }

    private void PrintMessage(string message)
    {
        Message.text = message;
        Message.gameObject.SetActive(true);
    }

    private void HighlightBestColumns()
    {
        foreach (var button in buttons)
        {
            button.HighLightImage(false);
        } 
        
        var bestColumns = connectFour.BestColumns();
        foreach (var col in bestColumns)
        {
            buttons[col].HighLightImage(true);
        }
    }
    
    #endregion
}
