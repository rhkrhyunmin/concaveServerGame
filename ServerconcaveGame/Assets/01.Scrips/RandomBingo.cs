using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomBingo : MonoBehaviour
{
    public BingoExSo _BingoExSo;
    public TextMeshProUGUI[] NameText;

    public int index = 0;
    public List<string> selectedItems;


    void Start()
    {
        // 리스트에서 9개의 요소를 무작위로 선택하여 출력
        selectedItems = SelectRandomItems(_BingoExSo.names, 9);

        // 선택된 아이템 출력
        int itemCount = Mathf.Min(selectedItems.Count, NameText.Length); // 최소값 계산
        for (int i = 0; i < itemCount; i++)
        {
            // NameText 배열에 있는 각 요소에 선택된 아이템을 할당하여 화면에 출력
            if (index < NameText.Length)
            {
                NameText[index].text = selectedItems[i];
                index++;
            }
            else
            {
                Debug.LogError("Not enough TextMeshProUGUI elements in the NameText array to display all selected items.");
                break;
            }
        }
    }

    // 리스트에서 n개의 아이템을 무작위로 선택하는 함수
    List<string> SelectRandomItems(List<string> list, int n)
    {
        List<string> selectedItems = new List<string>();

        int itemCount = Mathf.Min(n, list.Count);

        // 이미 선택된 요소를 기록하기 위한 HashSet
        HashSet<int> selectedIndices = new HashSet<int>();
        for (int i = 0; i < itemCount; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = UnityEngine.Random.Range(0, list.Count);
            } while (selectedIndices.Contains(randomIndex)); // 이미 선택된 요소인 경우 다시 선택

            selectedItems.Add(list[randomIndex]);
            Debug.Log(list[randomIndex]);
            selectedIndices.Add(randomIndex);
        }

        return selectedItems;
    }
}
