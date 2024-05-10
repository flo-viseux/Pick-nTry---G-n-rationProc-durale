using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] private Transform cratere = null;

    [SerializeField] private float r = 4.0f;

    [SerializeField] private int k = 10;

    [SerializeField] private int size = 4;
    #endregion

    #region Attributes
    List<Transform> grid = new List<Transform>();
    #endregion

    #region UnityMethods
    private void Start()
    {
        grid = new List<Transform>();

        for (int i = 0; i < size * size; i++)
        {
            grid.Add(null);
        }

        StartCoroutine(Generate());
    }
    #endregion

    #region Private
    IEnumerator Generate()
    {
        List<int> activeIndices = new List<int>();

        int firstIndex = Random.Range(0, size * size);
        activeIndices.Add(firstIndex); 
        grid[firstIndex] = Instantiate(cratere, new Vector3(firstIndex % size - 1, 0, firstIndex / size - 1), Quaternion.identity);

        int maxCount = 9999;
        int count = 0;

        while (activeIndices.Count > 0 && count < maxCount)
        {
            int selectedIndex = Random.Range(0, activeIndices.Count);
            int activeIndex = activeIndices[selectedIndex];
            int samples = 0;
            Vector3 samplePos = new Vector3();

            bool valid = false;
            while (!valid && samples < k)
            {
                float radius = Random.Range(r, 2 * r);
                Vector2 randomVec = Random.insideUnitCircle * radius;
                samplePos = grid[activeIndex].position + new Vector3(randomVec.x, 0, randomVec.y);

                valid = IsSampleValid(samplePos);

                samples++;

                yield return new WaitForSeconds(.01f);
            }

            if (valid)
            {
                int i = Mathf.FloorToInt(samplePos.z / r);
                int j = Mathf.FloorToInt(samplePos.x / r);

                int sampleIndex = i * size + j;

                grid[sampleIndex] = Instantiate(cratere, samplePos, Quaternion.identity);
                activeIndices.Add(sampleIndex);
            }
            else
                activeIndices.RemoveAt(selectedIndex);

            count++;
        }
    }

    bool IsSampleValid(Vector3 samplePos)
    {
        int sampleIndex = GetGridIndex(samplePos);

        if (IsValid(sampleIndex + 1, samplePos)
            && IsValid(sampleIndex - 1, samplePos)
            && IsValid(sampleIndex + size, samplePos)
            && IsValid(sampleIndex + size + 1, samplePos)
            && IsValid(sampleIndex + size - 1, samplePos)
            && IsValid(sampleIndex - size, samplePos)
            && IsValid(sampleIndex - size + 1, samplePos)
            && IsValid(sampleIndex - size - 1, samplePos))
            return true;

        return false;
    }

    bool IsValid(int index, Vector3 samplePos)
    {
        if (index >= 0 && index < size * size)
            return Vector3.Distance(grid[index].position, samplePos) > r;
        else
            return true;
    }

    int GetGridIndex(Vector3 samplePos)
    {
        int i = Mathf.FloorToInt(samplePos.z / r);
        int j = Mathf.FloorToInt(samplePos.x / r);

        int sampleIndex = i * size + j;

        return sampleIndex;
    }
    #endregion
}
