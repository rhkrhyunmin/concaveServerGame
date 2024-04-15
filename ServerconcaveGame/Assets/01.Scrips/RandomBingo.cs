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
        // ����Ʈ���� 9���� ��Ҹ� �������� �����Ͽ� ���
        selectedItems = SelectRandomItems(_BingoExSo.names, 9);

        // ���õ� ������ ���
        int itemCount = Mathf.Min(selectedItems.Count, NameText.Length); // �ּҰ� ���
        for (int i = 0; i < itemCount; i++)
        {
            // NameText �迭�� �ִ� �� ��ҿ� ���õ� �������� �Ҵ��Ͽ� ȭ�鿡 ���
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

    // ����Ʈ���� n���� �������� �������� �����ϴ� �Լ�
    List<string> SelectRandomItems(List<string> list, int n)
    {
        List<string> selectedItems = new List<string>();

        int itemCount = Mathf.Min(n, list.Count);

        // �̹� ���õ� ��Ҹ� ����ϱ� ���� HashSet
        HashSet<int> selectedIndices = new HashSet<int>();
        for (int i = 0; i < itemCount; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = UnityEngine.Random.Range(0, list.Count);
            } while (selectedIndices.Contains(randomIndex)); // �̹� ���õ� ����� ��� �ٽ� ����

            selectedItems.Add(list[randomIndex]);
            Debug.Log(list[randomIndex]);
            selectedIndices.Add(randomIndex);
        }

        return selectedItems;
    }
}
