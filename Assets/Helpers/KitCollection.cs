using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection and comparison utilities
/// </summary>
public static class KitCollection
{
    /// <summary>
    /// Checks if two lists are equal using reference equality
    /// </summary>
    public static bool AreListsEqual<T>(List<T> list1, List<T> list2) where T : class
    {
        if (list1 == null && list2 == null) return true;
        if (list1 == null || list2 == null) return false;
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!ReferenceEquals(list1[i], list2[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if two lists contain the same elements (value equality)
    /// </summary>
    public static bool AreListsEqualByValue<T>(List<T> list1, List<T> list2)
    {
        if (list1 == null && list2 == null) return true;
        if (list1 == null || list2 == null) return false;
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Safely gets an element from a list with wrapping indices
    /// </summary>
    public static T GetWrapped<T>(this List<T> list, int index)
    {
        if (list.Count == 0) return default(T);

        int wrappedIndex = ((index % list.Count) + list.Count) % list.Count;
        return list[wrappedIndex];
        
    }

    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++) // Missing i++
        {
            int randomIndex = Random.Range(i, list.Count); // Pick from remaining elements
            T element1 = list[randomIndex];
            T element2 = list[i]; // Missing T type declaration
            list[randomIndex] = element2;
            list[i] = element1;
        }
    }
}
