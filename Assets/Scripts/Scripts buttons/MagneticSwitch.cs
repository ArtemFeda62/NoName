using UnityEngine;

public class MagneticSwitch : MonoBehaviour
{
    [SerializeField] private MagneticField _magneticField;
    [SerializeField] private GameObject _magnetVisual;
    [SerializeField] private bool _startEnabled = true;

    private bool _isEnabled;

    private void Start()
    {
        _isEnabled = _startEnabled;
        if (_magneticField != null)
            _magneticField.enabled = _isEnabled;
        if (_magnetVisual != null)
            _magnetVisual.SetActive(_isEnabled);
    }

    public void ToggleMagnet()
    {
        _isEnabled = !_isEnabled;
        _magneticField.enabled = _isEnabled;
        if (_magnetVisual != null)
            _magnetVisual.SetActive(_isEnabled);

        if (!_isEnabled)
            _magneticField.ReleaseAllObjects();

        Debug.Log($"Магнит {(_isEnabled ? "включен" : "выключен")}");
    }

    public void EnableMagnet()
    {
        _isEnabled = true;
        _magneticField.enabled = true;
        if (_magnetVisual != null)
            _magnetVisual.SetActive(true);
    }

    public void DisableMagnet()
    {
        _isEnabled = false;
        _magneticField.enabled = false;
        _magneticField.ReleaseAllObjects();
        if (_magnetVisual != null)
            _magnetVisual.SetActive(false);
    }
}