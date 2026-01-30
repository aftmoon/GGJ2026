using System;

[Serializable]
public class PersonData
{
    public int id;                 // 个人ID
    public float temperature;      // 体温
    public bool hasMask;            // 是否戴口罩
    public NucleicResult nucleic;   // 核酸结果
    public HealthCode healthCode;   // 健康码
}

public enum NucleicResult
{
    Negative, // 阴
    Positive  // 阳
}

public enum HealthCode
{
    Green,
    Yellow,
    Red
}