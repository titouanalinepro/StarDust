using UnityEngine;
using TMPro;

/// <summary>
/// Cœur du jeu "La Forge des Étoiles" : gère la poussière d'étoile,
/// les clics sur la forge, les améliorations et la sauvegarde.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Textes de l'interface")]
    [SerializeField] private TMP_Text stardustText;     // compteur principal
    [SerializeField] private TMP_Text perSecondText;    // production par seconde
    [SerializeField] private TMP_Text hammerButtonText; // texte du bouton Marteau
    [SerializeField] private TMP_Text apprenticeButtonText; // texte du bouton Apprenti

    [Header("Équilibrage")]
    [SerializeField] private double hammerBaseCost = 10;
    [SerializeField] private double apprenticeBaseCost = 25;
    [SerializeField] private double costGrowth = 1.15;   // +15 % de coût par niveau

    private double stardust;
    private int hammerLevel;      // +1 poussière par clic et par niveau
    private int apprenticeLevel;  // +1 poussière par seconde et par niveau

    private double PerTap => 1 + hammerLevel;
    private double PerSecond => apprenticeLevel;
    private double HammerCost => hammerBaseCost * System.Math.Pow(costGrowth, hammerLevel);
    private double ApprenticeCost => apprenticeBaseCost * System.Math.Pow(costGrowth, apprenticeLevel);

    private void Start()
    {
        Load();
        RefreshUI();
    }

    private void Update()
    {
        if (PerSecond > 0)
        {
            stardust += PerSecond * Time.deltaTime;
            RefreshUI();
        }
    }

    // ---------- Appelé par les boutons (OnClick) ----------

    public void OnForgeTapped()
    {
        stardust += PerTap;
        RefreshUI();
    }

    public void BuyHammer()
    {
        if (stardust < HammerCost) return;
        stardust -= HammerCost;
        hammerLevel++;
        RefreshUI();
    }

    public void BuyApprentice()
    {
        if (stardust < ApprenticeCost) return;
        stardust -= ApprenticeCost;
        apprenticeLevel++;
        RefreshUI();
    }

    // ---------- Interface ----------

    private void RefreshUI()
    {
        stardustText.text = $"{Format(stardust)} ✦";
        perSecondText.text = $"{Format(PerSecond)} / s";
        hammerButtonText.text = $"Marteau céleste (niv. {hammerLevel})\n{Format(HammerCost)} ✦";
        apprenticeButtonText.text = $"Apprenti (niv. {apprenticeLevel})\n{Format(ApprenticeCost)} ✦";
    }

    private static string Format(double value)
    {
        string[] suffixes = { "", "K", "M", "B", "T" };
        int i = 0;
        while (value >= 1000 && i < suffixes.Length - 1)
        {
            value /= 1000;
            i++;
        }
        return i == 0 ? Mathf.FloorToInt((float)value).ToString() : $"{value:0.##}{suffixes[i]}";
    }

    // ---------- Sauvegarde (PlayerPrefs) ----------

    private void Save()
    {
        PlayerPrefs.SetString("stardust", stardust.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.SetInt("hammerLevel", hammerLevel);
        PlayerPrefs.SetInt("apprenticeLevel", apprenticeLevel);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        double.TryParse(PlayerPrefs.GetString("stardust", "0"),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out stardust);
        hammerLevel = PlayerPrefs.GetInt("hammerLevel", 0);
        apprenticeLevel = PlayerPrefs.GetInt("apprenticeLevel", 0);
    }

    // Sur mobile, l'appli est souvent mise en pause plutôt que quittée
    private void OnApplicationPause(bool paused) { if (paused) Save(); }
    private void OnApplicationQuit() => Save();
}
