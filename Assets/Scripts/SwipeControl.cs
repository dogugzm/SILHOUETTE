using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class SwipeControl : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Transform content;
    private float[] pos;
    private float distance;

    void Start()
    {
        pos = new float[content.childCount];
        distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
        scrollbar.value = 0.5f;
    }

    // Implementing IEndDragHandler interface
    public void OnEndDrag(PointerEventData eventData)
    {
        float nearestPos = FindNearestPosition();
        Debug.Log(nearestPos);
        scrollbar.value = nearestPos;
        AnimateToNearest(nearestPos);
    }


    float FindNearestPosition()
    {
        float nearest = pos[0];
        float distanceToNearest = Mathf.Abs(scrollbar.value - pos[0]);
        for (int i = 1; i < pos.Length; i++)
        {
            float distanceToCurrent = Mathf.Abs(scrollbar.value - pos[i]);
            if (distanceToCurrent < distanceToNearest)
            {
                nearest = pos[i];
                distanceToNearest = distanceToCurrent;
            }
        }
        return nearest;
    }

    void AnimateToNearest(float targetPos)
    {
        int index = pos.Length - 1 - System.Array.IndexOf(pos, targetPos);
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            float scale = i == index ? 1f : 0.8f;
            child.DOScale(new Vector2(scale, scale), 0.3f);
        }
    }
}