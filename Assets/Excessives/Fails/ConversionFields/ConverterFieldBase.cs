using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Excessives.LinqE;

//{TODO} Place in excessives namespace

[Serializable]
public class ConverterFieldBase
{
    //Index of conversion from, conversion method
    public Func<float, float>[] conversions = new Func<float, float>[0];

    public Enum usedEnum;

    public ConverterFieldBase(Enum usedEnum)
    {
        this.usedEnum = usedEnum;
    }

    public void AddConversions(params Func<float, float>[] converions)
    {
        this.conversions = converions;
    }

    public int[] selectedConversions;

    protected bool dirty = false;

    public float from
    {
        get { return this.from; }
        set
        {
            if (this.from != value)
            {
                dirty = true;
                this.from = value;
            }
        }
    }

    public float to
    {
        get
        {
            if (dirty)
            {//Recalculate
                selectedConversions.ForEach(n => to += conversions[n](from));
                dirty = false;
            }
            return this.to;
        }
        set
        {
            this.to = value;
        }
    }
}
