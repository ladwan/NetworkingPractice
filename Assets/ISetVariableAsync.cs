using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ISetVariableAsync<T>
{
    public Task InitializeAsync();

    T VariableToSet { get; }

    Task SetVariable();

    IEnumerator WaitForVariableToBeSet();

    bool VariableHasBeenSet { get; }
}
