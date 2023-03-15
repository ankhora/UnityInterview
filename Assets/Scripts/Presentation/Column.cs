using UnityEngine;
using UnityEngine.UI;

public class Column : MonoBehaviour
{
    #region Public Members
    public Transform[] RowPositions;
    public GameObject ChipParent;
    public ButtonController Button;
    #endregion

    #region Private Members
    private bool columnFull;
    private Button button;
    #endregion

    #region MonoBehavior Events
    private void Start()
    {
        // cache button reference
        button = Button.GetComponent<Button>();
    }
    #endregion
    
    #region Public Methods
    public bool IsFull()
    {
        if (columnFull)
        {
            button.interactable = false;
            return true;
        }

        columnFull = ChipParent.GetComponentsInChildren<Chip>().Length == ConnectFourLogic.ROW_COUNT;
        return columnFull;
    }

    public void Reset()
    {
        foreach (var chip in ChipParent.GetComponentsInChildren<Chip>())
        {
            Destroy(chip.gameObject);
        }
        Button.HighLightImage(false);
        button.interactable = true;
        columnFull = false;
    }
    #endregion
}
