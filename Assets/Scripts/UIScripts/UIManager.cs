using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Референс")]
    [SerializeField] private PlayerRay _playerRay;

    [Header("UI тексты")]
    [SerializeField] private TextMeshProUGUI _pickupText;
    [SerializeField] private TextMeshProUGUI _gravityText;
    [SerializeField] private TextMeshProUGUI _sizeText;

    [Header("UI при полнятии")]
    [SerializeField] private bool _showWhenPickedUp = false;

    private void Update()
    {
        UpdatePrompts();
    }

    private void UpdatePrompts()
    {
        if (_playerRay == null) return;

        bool itemPicked = _playerRay.IsItemPicked;
        Selectable selected = _playerRay.CurrentSelectable;
        string currentTool = _playerRay.CurrentTool;

        if (!itemPicked && selected != null && selected.GetComponent<Rigidbody>() != null)
        {
            _pickupText.gameObject.SetActive(true);
            _pickupText.text = "ЛКМ - поднять";
        }
        else if (_showWhenPickedUp && itemPicked)
        {
            _pickupText.gameObject.SetActive(true);
            _pickupText.text = "ЛКМ - отпустить";
        }
        else
        {
            _pickupText.gameObject.SetActive(false);
        }

        if (!itemPicked && selected != null && currentTool == "gravity" && selected.GetComponent<GravityChanger>() != null)
        {
            _gravityText.gameObject.SetActive(true);
            _gravityText.text = "Е - изм. гравитацию";
        }
        else
        {
            _gravityText.gameObject.SetActive(false);
        }

        if (!itemPicked && selected != null && currentTool == "size" && selected.GetComponent<SizeChanger>() != null)
        {
            _sizeText.gameObject.SetActive(true);
            _sizeText.text = "Колесико - изм. размер";
        }
        else
        {
            _sizeText.gameObject.SetActive(false);
        }
    }
}