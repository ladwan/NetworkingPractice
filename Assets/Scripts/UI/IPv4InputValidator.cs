using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class IPv4InputValidator : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button saveButton;

    void Awake()
    {
        inputField.onValidateInput += ValidateIPv4;
        inputField.onValueChanged.AddListener(OnIPChanged);
    }

    void Start()
    {
        // Initialize button state on load
        UpdateButtonState(inputField.text);
    }

    private void OnIPChanged(string ip)
    {
        UpdateButtonState(ip);
    }

    private void UpdateButtonState(string ip)
    {
        saveButton.interactable = IsValidIPv4(ip);
    }

    private bool IsValidIPv4(string ip)
    {
        if (string.IsNullOrEmpty(ip))
            return false;

        if (!IPAddress.TryParse(ip, out var address))
            return false;

        if (address.AddressFamily != AddressFamily.InterNetwork)
            return false;

        // Ensure exactly 4 segments
        string[] parts = ip.Split('.');
        if (parts.Length != 4)
            return false;

        return true;
    }

    private char ValidateIPv4(string text, int charIndex, char addedChar)
    {
        // ❌ No spaces
        if (char.IsWhiteSpace(addedChar))
            return '\0';

        // ✅ Handle digits
        if (char.IsDigit(addedChar))
        {
            string[] parts = text.Split('.');
            string currentPart = parts[parts.Length - 1];

            // ❌ Max 3 digits per segment
            if (currentPart.Length >= 3)
                return '\0';

            // ❌ Prevent values > 255
            string newPart = currentPart + addedChar;
            if (int.TryParse(newPart, out int value) && value > 255)
                return '\0';

            return addedChar;
        }

        // ✅ Handle dot
        if (addedChar == '.')
        {
            // ❌ Can't start with dot
            if (text.Length == 0)
                return '\0';

            // ❌ No double dots
            if (text[text.Length - 1] == '.')
                return '\0';

            // ❌ Max 4 segments (3 dots)
            if (text.Split('.').Length >= 4)
                return '\0';

            return addedChar;
        }

        // ❌ Block everything else
        return '\0';
    }
}