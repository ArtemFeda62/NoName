using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Референсы")]
    [SerializeField] private PlayerRay _playerRay;
    [SerializeField] private Transform _promptsContainer;
    [SerializeField] private GameObject _promptPrefab;

    [Header("Текста")]
    [SerializeField] private string _pickupText = "ЛКМ - Поднять / Отпустить";
    [SerializeField] private string _gravityText = "E - Сменить гравитацию";
    [SerializeField] private string _sizeText = "Колесико - Изменить размер";

    [Header("Цвета")]
    [SerializeField] private Color _pickupColor = Color.white;
    [SerializeField] private Color _gravityColor = new Color(0.3f, 0.8f, 1f); 
    [SerializeField] private Color _sizeColor = new Color(0.3f, 1f, 0.3f); 

    private List<GameObject> _activePrompts = new List<GameObject>();

    private void Start()
    {
        if (_promptsContainer == null)
        {
            Debug.LogError("PromptsContainer не назначен");
        }

        if (_promptPrefab == null)
        {
            Debug.LogError("PromptPrefab не назначен");
        }
    }

    private void Update()
    {
        UpdatePrompts();
    }

    private void UpdatePrompts()
    {
        ClearPrompts();

        if (_playerRay == null) return;

        bool isItemPicked = _playerRay.IsItemPicked;
        Selectable selected = _playerRay.CurrentSelectable;
        string currentTool = _playerRay.CurrentTool; 

        if (isItemPicked)
        {
            AddPrompt(_pickupText, _pickupColor);
            return;
        }
        if (selected == null) return;

        bool hasRigidbody = selected.GetComponent<Rigidbody>() != null;
        bool hasGravityChanger = selected.GetComponent<GravityChanger>() != null;
        bool hasSizeChanger = selected.GetComponent<SizeChanger>() != null;

        if (hasRigidbody)
        {
            AddPrompt(_pickupText, _pickupColor);
        }

        if (currentTool == "gravity" && hasGravityChanger)
        {
            AddPrompt(_gravityText, _gravityColor);
        }

        if (currentTool == "size" && hasSizeChanger)
        {
            AddPrompt(_sizeText, _sizeColor);
        }
    }

    private void AddPrompt(string text, Color color)
    {
        if (_promptPrefab == null || _promptsContainer == null) return;

        GameObject prompt = Instantiate(_promptPrefab, _promptsContainer);

        TextMeshProUGUI textComponent = prompt.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = color;
        }

        prompt.SetActive(true);
        _activePrompts.Add(prompt);
    }

    private void ClearPrompts()
    {
        foreach (GameObject prompt in _activePrompts)
        {
            if (prompt != null)
                Destroy(prompt);
        }
        _activePrompts.Clear();
    }
}