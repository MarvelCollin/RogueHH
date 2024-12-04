using UnityEngine;

[CreateAssetMenu(fileName = "IntegerSO", menuName = "IntegerSO", order = 0)]
public class IntegerSO : ScriptableObject {
    public int value;

    public void IncreaseValue(){
        value++;
    }

    public void DecreaseValue(){
        value--;
    }
}