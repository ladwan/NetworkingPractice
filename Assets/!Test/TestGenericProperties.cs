using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;


[Serializable]
public class ObservableValue<T>
{
    [SerializeField] private T value;

    public event Action<T> ValueChanged;

    public T Value
    {
        get => value;
        set
        {
            // prevents spam / infinite loops
            if (Equals(this.value, value))
                return;

            this.value = value;
            ValueChanged?.Invoke(this.value);
        }
    }

    public ObservableValue(T initialValue = default)
    {
        value = initialValue;
    }
}

