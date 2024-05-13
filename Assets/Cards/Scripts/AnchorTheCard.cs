using TMPro;
using UnityEngine;  
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AnchorTheCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // позиция перед началом перетаскивания
    private Vector3 startPosition;

    // Перетаскиваемая карта
    private Transform draggedCard;


    public void OnBeginDrag(PointerEventData eventData)//выполняется единажды, при начале перетаскивания объекта
    {
        startPosition = transform.position;
        draggedCard = transform;
        // Перенесите карту выше других объектов, чтобы она не была скрыта
        draggedCard.SetAsLastSibling();

        draggedCard.gameObject.layer = LayerMask.NameToLayer("ignore");

    }

    public void OnDrag(PointerEventData eventData)// каждый кадр пока тянем объект 
    {
        
        draggedCard.position += new Vector3(eventData.delta.x, eventData.delta.y, 0)/ 8;

    }

    public void OnEndDrag(PointerEventData eventData)// единажды когда отпустим объект 
    {
     
       
        // Проверьте, есть ли цель, на которую можно положить карту
        if (eventData.pointerEnter != null)
        {

            // Получите ссылку на цель
            var dropZone = eventData.pointerEnter.GetComponent<DropZone>();

            // Проверьте, является ли цель допустимым местом для карты
            if (dropZone == null || dropZone.FreeCell == false)
            {
                // Верните карту на начальную позицию, если цель недопустима
                draggedCard.position = startPosition;
                Debug.Log("dropZone = null"); 
            }
             
            
        }
        else
        {
            // Верните карту на начальную позицию, если цель не была найдена
            draggedCard.position = startPosition;
            Debug.Log("pointerEnter равен null");
        }

        draggedCard.gameObject.layer = LayerMask.NameToLayer("Default");
    }

}
