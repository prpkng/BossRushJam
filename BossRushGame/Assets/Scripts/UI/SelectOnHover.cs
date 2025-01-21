namespace BRJ.UI {
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.selectedObject = gameObject;
        }
        
    }
}