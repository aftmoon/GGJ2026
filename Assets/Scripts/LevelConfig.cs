using System.Collections.Generic;

public enum CheckItem
{
    Mask,        // ¿ÚÕÖ
    Temperature,// ÌåÎÂ
    Nucleic,    // ºËËá
    HealthCode  // ½¡¿µÂë
}
[System.Serializable]
public class LevelConfig
{
    public int levelIndex;
    public List<CheckItem> enabledChecks;
}