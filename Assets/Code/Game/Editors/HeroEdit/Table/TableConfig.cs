using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableConfig
{
    public int Id { get; set; }

    // 字段名
    public string Name { get; set; }

    // 类名
    public string ClassName { get; set; }
    // 类型
    public string AttributeType { get; set; }

    // 描述字段
    public string Des { get; set; }

    // 参数列表
    public List<string> ParameterList { get; set; }

    public float UIWidth { get; set; }
    
    public string ForeignKey { get; set; }
}
