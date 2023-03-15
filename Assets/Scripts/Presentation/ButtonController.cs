using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public int ColumnIndex;
    public Image ButtonImage;
    
    private Color normalColor = Color.white;
    private Color highlightcolor = Color.green;

    public void HighLightImage(bool highlight)
    {
        if (highlight)
        {
            ButtonImage.color = highlightcolor;
        }
        else
        {
            ButtonImage.color = normalColor;
        }
    }

    public void DropChip()
    {
        ConnectFourController.Instance.DropPlayerChip(ColumnIndex);
    }
}
