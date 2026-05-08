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
    [SerializeField] private Color _gravityColor = new Color(0.3f, 0.8f, 1f); // Голубой
    [SerializeField] private Color _sizeColor = new Color(0.3f, 1f, 0.3f); // Зеленый

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
        // Очищаем старые подсказки
        ClearPrompts();

        if (_playerRay == null) return;

        bool isItemPicked = _playerRay.IsItemPicked;
        Selectable selected = _playerRay.CurrentSelectable;
        string currentTool = _playerRay.CurrentTool; // Получаем текущий инструмент

        // Если предмет поднят - показываем подсказку "Отпустить"
        if (isItemPicked)
        {
            AddPrompt(_pickupText, _pickupColor);
            return;
        }

        // Если нет наведения на объект - выходим
        if (selected == null) return;

        // Проверяем какие компоненты есть на объекте
        bool hasRigidbody = selected.GetComponent<Rigidbody>() != null;
        bool hasGravityChanger = selected.GetComponent<GravityChanger>() != null;
        bool hasSizeChanger = selected.GetComponent<SizeChanger>() != null;

        // ВСЕГДА показываем поднятие, если объект можно поднять
        if (hasRigidbody)
        {
            AddPrompt(_pickupText, _pickupColor);
        }

        // Показываем действие гравитации ТОЛЬКО если выбран инструмент гравитации
        if (currentTool == "gravity" && hasGravityChanger)
        {
            AddPrompt(_gravityText, _gravityColor);
        }

        // Показываем действие размера ТОЛЬКО если выбран инструмент размера
        if (currentTool == "size" && hasSizeChanger)
        {
            AddPrompt(_sizeText, _sizeColor);
        }
    }

    private void AddPrompt(string text, Color color)
    {
        if (_promptPrefab == null || _promptsContainer == null) return;

        // Создаём новую подсказку
        GameObject prompt = Instantiate(_promptPrefab, _promptsContainer);

        // Находим текстовый компонент
        TextMeshProUGUI textComponent = prompt.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = color;
        }

        // Активируем и добавляем в список
        prompt.SetActive(true);
        _activePrompts.Add(prompt);
    }

    private void ClearPrompts()
    {
        // Удаляем все активные подсказки
        foreach (GameObject prompt in _activePrompts)
        {
            if (prompt != null)
                Destroy(prompt);
        }
        _activePrompts.Clear();
    }
}